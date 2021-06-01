using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEssentials.Spline.Extensions.Editor;
using UnityEssentials.Spline.PointsOnSplineExtensions;
using UnityEssentials.Spline.Extensions;
using UnityEssentials.Spline.Controller;

namespace UnityEssentials.Spline.ControllerExtensions.EventsTrigger
{
    [CustomEditor(typeof(ControllerEvents), true)]
    public class ControllerEventsEditor : BaseEditor<ControllerEvents>
    {
        public static GameObject CreateEventTrigger(string nameGameObject = "Points Lister")
        {
            GameObject whereToPut;
            if (Selection.activeGameObject != null)
            {
                ControllerStick controller = Selection.activeGameObject.GetComponent<ControllerStick>();
                if (controller != null)
                {
                    whereToPut = Selection.activeGameObject;
                }
                else
                {
                    whereToPut = new GameObject(nameGameObject);
                    whereToPut.transform.SetParent(controller.transform);
                    whereToPut.transform.localPosition = Vector3.zero;
                    Undo.RegisterCreatedObjectUndo(whereToPut, "Create " + nameGameObject);
                }
            }
            else
            {
                whereToPut = new GameObject(nameGameObject);
                whereToPut.transform.localPosition = Vector3.zero;
                Undo.RegisterCreatedObjectUndo(whereToPut, "Create " + nameGameObject);
            }
            return (whereToPut);
        }

        private ReorderableList _controllers;

        private void OnEnable()
        {
            _controllers = null;
            this.UpdateEditor();
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<ControllerStick>(this.GetPropertie("_controller"), _target.transform);
            this.ApplyModification();
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((ControllerEvents)target), typeof(ControllerEvents), false);
            GUI.enabled = true;

            if (_controllers == null)
            {
                SetupWaypointList();
            }

            // Points List
            EditorGUI.BeginChangeCheck();
            _controllers.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            // Ordinary properties
            DrawRemainingPropertiesInInspector();

        }

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(nameof(_target.ListToCheck));
            return excluded;
        }

        void SetupWaypointList()
        {
            _controllers = new ReorderableList(
                    serializedObject, FindProperty(x => x.ListToCheck),
                    true, true, true, true);

            _controllers.drawHeaderCallback = (Rect rect) =>
            { EditorGUI.LabelField(rect, "Points List"); };

            _controllers.drawElementCallback
                = (Rect rect, int index, bool isActive, bool isFocused) =>
                { DrawWaypointEditor(rect, index); };

            _controllers.draggable = true;
            _controllers.onAddCallback = (ReorderableList l) =>
            { InsertPoint(l.index); };

            AttemptToAutoFillPointsOnSplines();
        }

        private void DrawWaypointEditor(Rect rect, int index)
        {
            float colorFieldSize = rect.width / 30;

            Rect r = rect;
            r.width -= colorFieldSize;


            SerializedProperty element = _controllers.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null || element.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(r, element);
                return;
            }

            //description & field
            PointsOnSplineExtension pointOnSpline = (PointsOnSplineExtension)element.objectReferenceValue;
            bool isActiveInHierarchy = pointOnSpline.gameObject.activeInHierarchy;
            SerializedProperty description = new SerializedObject(element.objectReferenceValue).FindProperty("_description");
            EditorGUI.BeginDisabledGroup(!isActiveInHierarchy);
            {
                EditorGUI.PropertyField(r, element, new GUIContent(description.stringValue, "Point On Splines"));
            }
            EditorGUI.EndDisabledGroup();

            //box
            GUIStyle style = new GUIStyle("Box");
            SerializedProperty color = new SerializedObject(element.objectReferenceValue).FindProperty("_colorWayPoints");
            style.normal.background = ExtTexture.MakeTex(600, 1, color.colorValue);
            r.position = new Vector2(rect.width - colorFieldSize + 45, r.position.y);
            r.width = colorFieldSize - 2;
            GUI.Box(r, "", style);
        }

        private void InsertPoint(int index)
        {
            if (AttemptToAutoFillPointsOnSplines())
            {
                return;
            }
            AttemptToFillAdditionnalComponents();
        }

        private void AttemptToFillAdditionnalComponents()
        {
            this.UpdateEditor();
            Transform splineGameObject = _target.Spline.transform;
            List<PointsOnSplineExtension> allPoints = splineGameObject.GetComponentsInChildren<PointsOnSplineExtension>().ToList();

            for (int i = 0; i < _controllers.serializedProperty.arraySize; i++)
            {
                SerializedProperty element = _controllers.serializedProperty.GetArrayElementAtIndex(i);
                if (element == null || element.objectReferenceValue == null)
                {
                    continue;
                }
                PointsOnSplineExtension listReference = (PointsOnSplineExtension)element.objectReferenceValue;
                if (allPoints.Contains(listReference))
                {
                    allPoints.Remove(listReference);
                }
            }
            if (allPoints.Count == 0)
            {
                _controllers.serializedProperty.arraySize += 1;
                this.ApplyModification();
                return;
            }

            _controllers.serializedProperty.arraySize += 1;
            SerializedProperty addedElement = _controllers.serializedProperty.GetArrayElementAtIndex(_controllers.serializedProperty.arraySize - 1);
            addedElement.objectReferenceValue = allPoints[0];
            this.ApplyModification();
        }

        public void ResetControllerEvents()
        {
            SerializedProperty listToCheck = this.GetPropertie(nameof(_target.ListToCheck));
            PointsOnSplineExtension[] allPoints = _target.Spline.transform.GetComponentsInChildren<PointsOnSplineExtension>();
            ExtSerializedProperties.ApplyArrayToSerializeArray(listToCheck, allPoints);
            this.ApplyModification();
        }

        private bool AttemptToAutoFillPointsOnSplines()
        {
            if (_target.ListToCheck.Length != 0)
            {
                return (false);
            }
            this.UpdateEditor();
            if (_target.Spline != null)
            {
                PointsOnSplineExtension[] allPoints = _target.Spline.transform.GetComponentsInChildren<PointsOnSplineExtension>();
                if (allPoints.Length == 0)
                {
                    allPoints = GameObject.FindObjectsOfType<PointsOnSplineExtension>();
                }
                ExtSerializedProperties.ApplyArrayToSerializeArray(_controllers.serializedProperty, allPoints);
                this.ApplyModification();
            }
            return (true);
        }
    }
}
