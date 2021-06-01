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
    [CustomEditor(typeof(ControllerStick), true)]
    public class ControllerStickEditor : BaseEditor<ControllerStick>
    {
        protected ControllerStick _splineController;
        private SplineBase.PositionUnits _positionUnits;

        //lerp & snap feature
        private bool _isLerping = false;
        private float _positionAtStartLerp = 0;
        private float _previousDraggingPosition = 0;
        private bool _lerpFinished;
        private FrequencyChrono _snapTimer = new FrequencyChrono();
        private float _minDistUnitToSnap = 1f;
        private float _snapForce = 5f;
        private float _timeBetween2Snap = 0.5f;
        private float _accuracyForSnap = 0.05f;

        private bool _anchorOption;

        [MenuItem("GameObject/Unity Essentials/Splines/Controller/Controller Stick", false, 11)]
        private static void CreateSplineControllerStick()
        {
            CreateGenericSpline<ControllerStick>("Controller Stick");
        }
        [MenuItem("GameObject/Unity Essentials/Splines/Controller/Controller Move", false, 11)]
        private static void CreateSplineControllerMove()
        {
            CreateGenericSpline<SplineControllerMove>("Controller Move");
        }
        [MenuItem("GameObject/Unity Essentials/Splines/Controller/Controller Focus Target", false, 11)]
        private static void CreateSplineControllerFocusTarget()
        {
            CreateGenericSpline<ControllerFocusTarget>("Controller Focus Target");
        }
        private static void CreateGenericSpline<T>(string nameType) where T : ControllerStick
        {
            GameObject splineObj = new GameObject(nameType);
            T splineController = splineObj.AddComponent<T>();

            SplineBase spline = null;
            if (Selection.activeGameObject != null)
            {
                spline = Selection.activeGameObject.GetComponent<SplineBase>();
                splineObj.transform.SetParent(Selection.activeGameObject.transform);
            }

            Undo.RegisterCreatedObjectUndo(splineObj, nameType);
            Selection.activeGameObject = splineObj;
            SceneView.lastActiveSceneView.MoveToView(splineObj.transform);
            GameObjectUtility.EnsureUniqueNameForSibling(splineObj);

            Editor splineEditorGeneric = CreateEditor((ControllerStick)splineController, typeof(ControllerStickEditor));
            ControllerStickEditor splineEditor = (ControllerStickEditor)splineEditorGeneric;
            splineEditor.ConstructSpline(spline);
        }

        protected virtual void OnEnable()
        {
            if (target is ControllerStick)
            {
                _splineController = (ControllerStick)target;
                this.UpdateEditor();
                ExtSerializedProperties.SetObjectReferenceValueIfEmpty<SplineBase>(this.GetPropertie("_spline"), _splineController.transform);
                ExtSerializedProperties.SetObjectReferenceValueIfEmpty<Transform>(this.GetPropertie("_toMove"), _splineController.transform);
                this.ApplyModification();
            }
            if (_splineController)
            {
                _positionUnits = _splineController.PositionUnits;
            }
        }

        private void OnDisable()
        {
            Tools.hidden = false;
        }

        //protected virtual void OnStartDragging(GameObject current)
        //{
        //    if (_splineController == null)
        //    {
        //        return;
        //    }
        //    if (current != _splineController.gameObject)
        //    {
        //        return;
        //    }
        //    this.UpdateEditor();
        //
        //    _anchorOption = this.GetPropertie("_anchorOnSplineMoved").boolValue;
        //    this.GetPropertie("_anchorOnSplineMoved").boolValue = true;
        //
        //
        //    _positionAtStartLerp = _previousDraggingPosition = _splineController.PathPosition;
        //    //SetSticky(false);
        //    this.ApplyModification();
        //}

        //protected virtual void OnEndDragging(GameObject current)
        //{
        //    if (_splineController == null)
        //    {
        //        return;
        //    }
        //    if (current != _splineController.gameObject)
        //    {
        //        return;
        //    }
        //    this.UpdateEditor();
        //
        //    this.GetPropertie("_anchorOnSplineMoved").boolValue = _anchorOption;
        //
        //
        //    //SetSticky(true);
        //    this.ApplyModification();
        //}

        public void ConstructSpline(SplineBase spline)
        {
            this.UpdateEditor();
            if (spline != null)
            {
                ExtSerializedProperties.SetObjectReferenceValueIfEmpty<SplineBase>(this.GetPropertie("_spline"), spline.transform);
            }
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<Transform>(this.GetPropertie("_toMove"), _splineController.transform);
            this.ApplyModification();
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((ControllerStick)target), typeof(ControllerStick), false);
            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                if (_positionUnits != _splineController.PositionUnits)
                {
                    ChangePosition(_target.transform.position);
                }
                _positionUnits = _splineController.PositionUnits;
            }
        }

        protected void OnSceneGUI()
        {
            Tools.hidden = Tools.current == Tool.Move;

            if (Tools.current != Tool.Move || _target == null || _target.SplineBase == null)
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
            if (EditorGUI.EndChangeCheck())
            {
                ChangePosition(newPosition);
            }
        }

        private void ChangePosition(Vector3 newPosition)
        {
            SerializedProperty pathPosition = this.GetPropertie("_pathPosition");
            SerializedProperty splineProperty = this.GetPropertie("_spline");
            SplineBase spline = splineProperty.GetValue<SplineBase>();

            float closestPositionOnSpline = spline.FindClosestPoint(newPosition, 0, -1, 10);
            float currentUnits = spline.FromPathNativeUnits(closestPositionOnSpline, _splineController.PositionUnits);
            currentUnits = _splineController.SplineBase.AttemptToApplyGrid(currentUnits, 0.01f);

            pathPosition.floatValue = _previousDraggingPosition = currentUnits;

            UpdatePositionFromEditor();
            this.GetPropertie("_lastPosition").vector3Value = _splineController.transform.position;
            this.ApplyModification();
        }

        private void UpdatePositionFromEditor()
        {
            _target.BasicPositionAndOffset(_target.PathPosition);
            _target.BasicRotationAndOffset(_target.PathPosition);
            _target.ApplyKeepUp();
            Undo.RecordObject(_target.transform, "Move offset");
        }

        public void ChangePosition(float newPosition)
        {
            SerializedProperty pathPosition = this.GetPropertie("_pathPosition");
            pathPosition.floatValue = newPosition;
            this.ApplyModification();
        }

        public bool ChangeSplineAndPosition(SplineBase splineBase, float newPosition)
        {
            if (_splineController.SplineBase == splineBase)
            {
                ChangePosition(newPosition);
                return (false);
            }
            SerializedProperty spline = this.GetPropertie("_spline");
            SerializedProperty pathPosition = this.GetPropertie("_pathPosition");
            spline.objectReferenceValue = splineBase;
            pathPosition.floatValue = newPosition;
            this.ApplyModification();

            ResetEventsOfControllersEvents(_splineController.transform);

            ControllerStickOffset[] allOffset = FindObjectsOfType<ControllerStickOffset>();
            for (int i = 0; i < allOffset.Length; i++)
            {
                if (allOffset[i] == _splineController)
                {
                    continue;
                }
                ControllerStickOffsetEditor offsetEditor = (ControllerStickOffsetEditor)UnityEditor.Editor.CreateEditor((ControllerStickOffset)allOffset[i], typeof(ControllerStickOffsetEditor));
                offsetEditor.ChangeSplineOfController(splineBase);
                DestroyImmediate(offsetEditor);

                ResetEventsOfControllersEvents(allOffset[i].transform);
            }

            this.ApplyModification();
            return (true);
        }

        private void ResetEventsOfControllersEvents(Transform currentSticker)
        {
            ControllerEvents[] controllerEventsToChange = currentSticker.GetComponentsInChildren<ControllerEvents>();
            for (int i = 0; i < controllerEventsToChange.Length; i++)
            {
                ControllerEventsEditor eventsEditor = (ControllerEventsEditor)UnityEditor.Editor.CreateEditor((ControllerEvents)controllerEventsToChange[i], typeof(ControllerEventsEditor));
                eventsEditor.ResetControllerEvents();
                DestroyImmediate(eventsEditor);
            }
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
             | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(ControllerStick))]
        static void DrawGizmos(ControllerStick sticker, GizmoType selectionType)
        {
            if (Application.isPlaying || sticker == null || sticker.SplineBase == null)
            {
                return;
            }

            if (sticker.AnchorOnSplineChange && Selection.activeGameObject != sticker.gameObject)
            {
                ControllerStickEditor controllerStickEditor = (ControllerStickEditor)CreateEditor((ControllerStick)sticker) as ControllerStickEditor;
                SerializedProperty pathPosition = controllerStickEditor.GetPropertie("_pathPosition");

                float closestPosOnSpline = sticker.SplineBase.FindClosestPoint(sticker.transform.position, 0, -1, 10, sticker.PositionUnits);
                float current = pathPosition.floatValue;

                float diff = Mathf.Abs(closestPosOnSpline - current);
                if (diff > 0.002f)
                {
                    pathPosition.floatValue = closestPosOnSpline;
                    //controllerStickEditor.UpdatePositionFromEditor();
                    //controllerStickEditor.GetPropertie("_lastPosition").vector3Value = sticker.transform.position;
                    controllerStickEditor.ApplyModification();
                    sticker.AttemptToStick();
                }
                DestroyImmediate(controllerStickEditor);
            }
            else
            {
                //Debug.Log("stick like usual");
                sticker.AttemptToStick();
            }


            Gizmos.DrawLine(sticker.transform.position, sticker.transform.position);
            Gizmos.DrawLine(sticker.transform.position, sticker.PositionWithoutOffsetFromSpline(sticker.PathPosition));
        }
    }
}