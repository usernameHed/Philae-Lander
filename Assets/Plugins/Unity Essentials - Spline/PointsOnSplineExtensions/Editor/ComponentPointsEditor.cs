using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline.Extensions.Editor;
using System;

namespace UnityEssentials.Spline.PointsOnSplineExtensions.Editor
{
    [CustomEditor(typeof(ComponentPoints))]
    public class ComponentPointsEditor : PointsOnSplineExtensionEditor
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Points Lister/Component", false, 10)]
        private static void CreatePointsLister()
        {
            GameObject whereToPut = CreatePointsLister("Component Points Lister");
            ComponentPoints points = whereToPut.AddComponent(typeof(ComponentPoints)) as ComponentPoints;
            ComponentPointsEditor pointsEditor = (ComponentPointsEditor)CreateEditor((ComponentPoints)points, typeof(ComponentPointsEditor));
            pointsEditor.Construct(Color.yellow);
            DestroyImmediate(pointsEditor);
            Selection.activeGameObject = points.gameObject;
        }

        protected ComponentPoints _componentTarget { get { return target as ComponentPoints; } }

        protected override SerializedProperty SerializedWaypoints
        {
            get
            {
                return (serializedObject.FindProperty(nameof(ComponentPoints.Waypoints)));
            }
        }

        protected virtual string ComponentReference
        {
            get
            {
                return (nameof(ComponentPoints.ComponentWaypoint.Reference));
            }
        }

        protected virtual bool SerializeObjectField(Rect rect, SerializedProperty toChange)
        {
             return (ObjectField<Transform>(rect, toChange));
        }

        protected virtual Type[] ComponentsToAdd()
        {
            return (new Type[0]);
        }

        protected virtual string ComponentName(int index)
        {
            return ("Transform");
        }



        #region Inspector
        protected override Rect DisplayContent(int index, Rect rect, float rightMargin)
        {
            Rect r = new Rect(rect);
            r.width -= rightMargin;
            r.height -= _space;

            SerializedProperty waypoint = _waypointList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty reference = waypoint.GetPropertie(ComponentReference);

            float feldWidth = EditorGUIUtility.singleLineHeight * 0.5f;
            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = feldWidth;
            {
                SerializeObjectField(r, reference);
            }
            EditorGUIUtility.labelWidth = oldWidth;

            rect.position = new Vector2(rect.position.x + rect.width - rightMargin + _space, rect.position.y);
            rect.width = rightMargin;
            return (rect);
        }

        protected bool ObjectField<T>(Rect rect, SerializedProperty toChange) where T : Component
        {
            EditorGUI.BeginChangeCheck();
            T toPick = EditorGUI.ObjectField(rect, "", toChange.objectReferenceValue, typeof(T), true) as T;
            bool canChange = toPick == null || (toPick != null && toPick.gameObject.scene.IsValid());

            if (EditorGUI.EndChangeCheck() && canChange)
            {
                toChange.objectReferenceValue = toPick;
                this.ApplyModification();
                return (true);
            }
            return (false);
        }
        #endregion

        #region SceneView
        protected override void DrawInSceneViewSelected(int index)
        {
            Vector3 posInSpline = _componentTarget.GetPositionFromPoint(index);
            Component componentReference = _componentTarget.GetReference(index);
            if (componentReference == null)
            {
                return;
            }
            Transform reference = componentReference.transform;

            Vector3 posOfReference = reference.position;

            if (!this.GetPropertie("_showTarget").boolValue)
            {
                return;
            }

            float size = HandleUtility.GetHandleSize(posOfReference) * 0.05f;
            if (Handles.Button(posOfReference, Quaternion.identity, size, size, Handles.SphereHandleCap))
            {
                Selection.activeObject = reference.gameObject;
            }

           
            ExtDrawGuizmos.DrawBezier(posInSpline, posOfReference, _componentTarget.GetRotationFromPoint(index) * Vector3.right, _componentTarget.ColorWayPoint, 0.5f);


            if (this.GetPropertie("_showDistance").boolValue)
            {
                float distance = Vector3.Distance(posInSpline, reference.position);
                Vector3 positionDistanceLabel = (posInSpline + reference.position) * 0.5f;
                PointsOnSplineExtensionEditor.DisplayStringInSceneViewFrom3dPosition(positionDistanceLabel, distance.ToString(".0"), _componentTarget.ColorWayPoint, 8);
            }

            if (!Event.current.shift && index == _waypointList.index && index != -1 && this.GetPropertie("_showHandleTarget").boolValue)
            {
                ExtHandle.DoHandleMove(reference, true, out bool hasChanged, _target.SplineBase.GridSize);
            }
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
           | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(ComponentPoints))]
        static void DrawGizmos(ComponentPoints points, GizmoType selectionType)
        {
            if (points.gameObject == Selection.activeGameObject)
            {
                return;
            }
            if (!points.ShowWayPointsWhenUnselected)
            {
                return;
            }
            if (!points.ShowTarget)
            {
                return;
            }
            for (int i = 0; i < points.WaypointsCount; ++i)
            {
                DrawUnselected(points, i);
            }
        }

        protected static void DrawUnselected(ComponentPoints point, int index)
        {
            Vector3 posInSpline = point.GetPositionFromPoint(index);
            Component reference = point.GetReference(index);
            if (reference == null)
            {
                return;
            }

            Vector3 posOfReference = reference.transform.position;
            ExtDrawGuizmos.DrawBezier(posInSpline, posOfReference, point.GetRotationFromPoint(index) * Vector3.right, point.ColorWayPoint, 0.5f);


            float size = HandleUtility.GetHandleSize(posOfReference) * 0.05f;
            Color colorOld = Gizmos.color;
            Gizmos.color = point.ColorWayPoint;
            Gizmos.DrawSphere(posOfReference, size);
            Gizmos.color = colorOld;
        }
        #endregion

        #region misc
        protected override int InsertPoint(int index)
        {
            int currentSelected = _waypointList.index;
            if (currentSelected == -1 || _waypointList.count == 0 || serializedObject.GetPropertie("_addPointClosestToCamera").boolValue)
            {
                return (AddCloseToCamera());
            }

            SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(currentSelected);
            SerializedProperty pathPosition = element.GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PathPosition));
            return (AddPoint(pathPosition.floatValue));
        }

        protected override int AddPoint(float positionPath)
        {
            int indexToDuplicate = _waypointList.index;
            Component referenceToDuplicate = _componentTarget.GetReference(indexToDuplicate);

            int index = base.AddPoint(positionPath);

            bool canDupplicate = referenceToDuplicate != null && this.GetPropertie("_duplicateCurrentSelectedOnCreation").boolValue;


            GameObject referenceToLink;
            if (!canDupplicate)
            {
                referenceToLink = GeneratePoint(positionPath);
            }
            else
            {
                Vector3 offsetFromPoint = referenceToDuplicate.transform.position - _target.GetPositionFromPoint(indexToDuplicate);
                UnityEngine.Object prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(referenceToDuplicate.gameObject);
                if (prefabRoot != null)
                {
                    referenceToLink = PrefabUtility.InstantiatePrefab(prefabRoot, referenceToDuplicate.transform.parent) as GameObject;
                    referenceToLink.transform.position = referenceToDuplicate.transform.position;
                    referenceToLink.transform.rotation = referenceToDuplicate.transform.rotation;
                }
                else
                {
                    referenceToLink = Instantiate(referenceToDuplicate.gameObject, referenceToDuplicate.transform.position, referenceToDuplicate.transform.rotation, referenceToDuplicate.transform.parent);
                }
                referenceToLink.transform.position = _target.GetPositionFromPoint(index) + offsetFromPoint;
            }
            AddPointGenericAddition(referenceToLink, _waypointList.count - 1);

            return (_waypointList.count - 1);
        }

        private GameObject GeneratePoint(float positionPath)
        {
            GameObject referenceToLink = new GameObject(ComponentName(_waypointList.count - 1), ComponentsToAdd());
            Vector3 position = _componentTarget.GetPositionFromPoint(positionPath, SplineBase.PositionUnits.Distance);
            Quaternion rotation = _componentTarget.GetRotationFromPoint(positionPath, SplineBase.PositionUnits.Distance);
            referenceToLink.transform.SetParent(_componentTarget.transform);
            referenceToLink.transform.position = position + (rotation * Vector3.left) * 3;
            return referenceToLink;
        }

        protected virtual void AddPointGenericAddition(GameObject referenceToLink, int index)
        {
            Undo.RegisterCreatedObjectUndo(referenceToLink, "Duplicate item of waypoint");
            SerializedProperty waypoint = _waypointList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty reference = waypoint.GetPropertie(ComponentReference);
            reference.objectReferenceValue = referenceToLink;
            this.ApplyModification();
        }

        protected override void RemovePoint(int index)
        {
            int indexToRemove = _waypointList.index;
            Component referenceToDuplicate = _componentTarget.GetReference(indexToRemove);
            base.RemovePoint(index);
            if (referenceToDuplicate != null && this.GetPropertie("_removeReferenceWhenRemovingPoint").boolValue)
            {
                Undo.DestroyObjectImmediate(referenceToDuplicate.gameObject);
            }
        }

        protected override void OnMovePoint(int index, float oldPos, float newPos)
        {
            base.OnMovePoint(index, oldPos, newPos);
            if (Event.current.shift != this.GetPropertie("_holdShiftToMoveReference").boolValue)
            {
                return;
            }
            Vector3 previousWoldPosition = _target.GetPositionFromPoint(oldPos, _target.PositionUnits);
            Vector3 nextWoldPosition = _target.GetPositionFromPoint(newPos, _target.PositionUnits);
            Component reference = _componentTarget.GetReference(index);
            if (reference == null)
            {
                return;
            }
            Undo.RecordObject(reference.transform, "Move Transform point");
            reference.transform.position += nextWoldPosition - previousWoldPosition;
        }

        protected override void SelectNewWayPoint(int index, bool additionnalPing)
        {
            base.SelectNewWayPoint(index, additionnalPing);
            if (additionnalPing)
            {
                EditorGUIUtility.PingObject(_componentTarget.GetReference(index));
            }
        }

        protected override void RepairIndexWaypoints()
        {
            for (int i = 0; i < _waypointList.count; i++)
            {
                _waypointList.serializedProperty.GetArrayElementAtIndex(i).GetPropertie("_index").intValue = i;

                SerializedProperty reference = _waypointList.serializedProperty.GetArrayElementAtIndex(i).GetPropertie(ComponentReference);
                if (reference.objectReferenceValue == null)
                {
                    continue;
                }
                int index = _waypointList.serializedProperty.GetArrayElementAtIndex(i).GetPropertie("_index").intValue;
                Transform transform = (Transform)reference.objectReferenceValue;
                transform.SetSiblingIndex(index);
            }
        }

        /*
        protected void UpdateTargetPosition()
        {
            for (int i = 0; i < _waypointList.count; i++)
            {
                SerializedProperty element = _waypointList.serializedProperty.GetArrayElementAtIndex(i);
                SerializedProperty previous = element.GetPropertie(nameof(PointsOnSplineExtension.Waypoint.PreviousPosition));

                Vector3 previousWoldPosition = previous.vector3Value;
                Vector3 nextWoldPosition = _target.GetPositionFromPoint(i);
                Component reference = _componentTarget.GetReference(i);
                if (reference == null)
                {
                    return;
                }
                Undo.RecordObject(reference.transform, "Move Transform point");
                reference.transform.position += previousWoldPosition - nextWoldPosition;
            }
        }
        */
        #endregion
    }
}