using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline.ControllerExtensions.EventsTrigger;
using UnityEssentials.Spline.Extensions;
using UnityEssentials.Spline.Extensions.Editor;
using UnityEssentials.Spline.Extensions.Editor.EventEditor;
using UnityEssentials.Spline.PointsOnSplineExtensions;

namespace UnityEssentials.Spline.Controller.editor
{
    [CustomEditor(typeof(ControllerStickOffset), true)]
    public class ControllerStickOffsetEditor : BaseEditor<ControllerStickOffset>
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Controller/Controller Offset From Other Sticker", false, 11)]
        private static void CreateSplineControllerStick()
        {
            string nameStickOffet = "Controller Offset From Other Sticker";

            ControllerStick stickReference = null;
            GameObject stickOffsetGameObject;
            if (Selection.activeGameObject != null)
            {
                stickReference = Selection.activeGameObject.GetComponent<ControllerStick>();
            }

            if (stickReference != null)
            {
                stickOffsetGameObject = new GameObject(nameStickOffet);
                stickOffsetGameObject.transform.SetParent(stickReference.transform.parent);
                stickOffsetGameObject.transform.SetSiblingIndex(stickReference.transform.GetSiblingIndex() + 1);
                Undo.RegisterCreatedObjectUndo(stickOffsetGameObject, nameStickOffet);
                GameObjectUtility.EnsureUniqueNameForSibling(stickOffsetGameObject);
            }
            else if (Selection.activeGameObject != null && stickReference == null)
            {
                stickOffsetGameObject = Selection.activeGameObject;
            }
            else
            {
                stickOffsetGameObject = new GameObject(nameStickOffet);
                GameObjectUtility.EnsureUniqueNameForSibling(stickOffsetGameObject);
                Undo.RegisterCreatedObjectUndo(stickOffsetGameObject, nameStickOffet);
            }

            //GameObject 
            ControllerStickOffset stickOffset = stickOffsetGameObject.AddComponent<ControllerStickOffset>();
            Selection.activeGameObject = stickOffsetGameObject;
            SceneView.lastActiveSceneView.MoveToView(stickOffsetGameObject.transform);
            ControllerStickOffsetEditor splineEditorGeneric = (ControllerStickOffsetEditor)CreateEditor((ControllerStickOffset)stickOffset, typeof(ControllerStickOffsetEditor));
            splineEditorGeneric.ConstructSticker(stickReference);
        }

        public void ConstructSticker(ControllerStick sticker)
        {
            this.UpdateEditor();
            if (sticker != null)
            {
                ExtSerializedProperties.SetObjectReferenceValueIfEmpty<ControllerStick>(this.GetPropertie("_targetStick"), sticker.transform);
                ExtSerializedProperties.SetObjectReferenceValueIfEmpty<SplineBase>(this.GetPropertie("_spline"), sticker.SplineBase.transform);
            }
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<Transform>(this.GetPropertie("_toMove"), _target.transform);
            this.ApplyModification();
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                return;
            }

            this.UpdateEditor();
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<Transform>(this.GetPropertie("_toMove"), _target.transform);
            if (_target.TargetStick != null && _target.TargetStick.SplineBase != null)
            {
                ExtSerializedProperties.SetObjectReferenceValueIfEmpty<SplineBase>(this.GetPropertie("_spline"), _target.TargetStick.SplineBase.transform);
            }
            this.ApplyModification();
        }

        private void OnDisable()
        {
            Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((ControllerStickOffset)target), typeof(ControllerStickOffset), false);
            GUI.enabled = true;
            base.OnInspectorGUI();
        }

        protected void OnSceneGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Tools.hidden = Tools.current == Tool.Move;

            if (Tools.current != Tool.Move || _target.TargetStick == null || _target.TargetStick.SplineBase == null)
            {
                return;
            }
            UpdateDraggingPositionOnSpline();
        }

        private void UpdateDraggingPositionOnSpline()
        {
            Vector3 positionOnSpline = _target.PositionWithoutOffsetFromSpline(_target.PathPosition);
            Quaternion rotationOnSpline = _target.RotationWithoutOffsetFromSpline(_target.PathPosition);
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = Handles.PositionHandle(Tools.pivotMode != PivotMode.Center ? positionOnSpline : _target.transform.position, Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity : rotationOnSpline);
            //newPosition = _target.SplineBase.AttemptToApplyGrid(newPosition, 0.1f);
            if (EditorGUI.EndChangeCheck())
            {
                SerializedProperty offset = this.GetPropertie("_offsetFromTarget");
                SerializedProperty splineProperty = this.GetPropertie("_spline");
                SplineBase spline = splineProperty.GetValue<SplineBase>();


                float closestPositionOnSpline = spline.FindClosestPoint(newPosition, 0, -1, 10);
                float currentUnits = spline.FromPathNativeUnits(closestPositionOnSpline, _target.PositionUnits);
                currentUnits = _target.SplineBase.AttemptToApplyGrid(currentUnits, 0.01f);

                offset.floatValue = currentUnits - _target.PathPositionTarget;
                UpdatePositionFromEditor(offset);
                this.ApplyModification();
            }
        }

        public void ChangeSplineOfController(SplineBase newSpline)
        {
            if (_target.SplineBase == newSpline)
            {
                return;
            }
            SerializedProperty spline = this.GetPropertie("_spline");
            spline.objectReferenceValue = newSpline;
            this.ApplyModification();
        }

        private void UpdatePositionFromEditor(SerializedProperty offset)
        {
            _target.BasicPositionAndOffset(_target.PathPositionTarget + offset.floatValue);
            _target.BasicRotationAndOffset(_target.PathPositionTarget + offset.floatValue);
            _target.ApplyKeepUp();
            Undo.RecordObject(_target.transform, "Move offset");
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
             | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(ControllerStickOffset))]
        static void DrawGizmos(ControllerStickOffset offsetSticker, GizmoType selectionType)
        {
            if (Application.isPlaying || offsetSticker == null || offsetSticker.TargetStick == null || offsetSticker.TargetStick.SplineBase == null)
            {
                return;
            }
            //ControllerStickOffsetEditor stickEditor = (ControllerStickOffsetEditor)CreateEditor((ControllerStickOffset)offsetSticker) as ControllerStickOffsetEditor;
            //SerializedProperty offset = stickEditor.GetPropertie("_offsetFromTarget");
            //SerializedProperty splineProperty = stickEditor.GetPropertie("_spline");
            //SplineBase spline = splineProperty.GetValue<SplineBase>();
            //stickEditor.UpdatePositionFromEditor(offset, spline);
            //Gizmos.DrawLine(offsetSticker.transform.position, offsetSticker.TargetStick.transform.position);
            //DestroyImmediate(stickEditor);

            offsetSticker.AttemptToStick();

            ExtDrawGuizmos.DrawBezier(offsetSticker.transform.position, offsetSticker.TargetStick.transform.position, -offsetSticker.transform.forward, Color.green, 0.5f);
            Gizmos.DrawLine(offsetSticker.transform.position, offsetSticker.PositionWithoutOffsetFromSpline(offsetSticker.PathPosition));
        }
    }
}