using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline.Extensions.Editor;

namespace UnityEssentials.Spline.PointsOnSplineExtensions.Editor
{
    [CustomEditor(typeof(VectorPoints), true)]
    public class VectorPointsEditor : PointsOnSplineExtensionEditor
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Points Lister/Vector3", false, 10)]
        private static void CreatePointsLister()
        {
            GameObject whereToPut = CreatePointsLister("Vector3 Points Lister");
            VectorPoints points = whereToPut.AddComponent(typeof(VectorPoints)) as VectorPoints;
            VectorPointsEditor pointsEditor = (VectorPointsEditor)CreateEditor((VectorPoints)points, typeof(VectorPointsEditor));
            pointsEditor.Construct(Color.gray);
            DestroyImmediate(pointsEditor);
            Selection.activeGameObject = points.gameObject;
        }

        protected VectorPoints _vectorTarget { get { return target as VectorPoints; } }

        protected override SerializedProperty SerializedWaypoints
        {
            get
            {
                return (serializedObject.FindProperty(nameof(VectorPoints.Waypoints)));
            }
        }

        protected override Rect DisplayContent(int index, Rect rect, float rightMargin)
        {
            Rect r = new Rect(rect);
            r.width -= rightMargin;
            r.height -= _space;

            SerializedProperty waypoint = _waypointList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty point = waypoint.GetPropertie(nameof(VectorPoints.VectorWaypoint.Point));
            float floatFieldWidth = EditorGUIUtility.singleLineHeight * 0.5f;

            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = floatFieldWidth;
            {
                EditorGUI.BeginChangeCheck();
                point.vector3Value = EditorGUI.Vector3Field(r, "", point.vector3Value);
                if (EditorGUI.EndChangeCheck())
                {
                    this.ApplyModification();
                }
            }
            EditorGUIUtility.labelWidth = oldWidth;

            rect.position = new Vector2(rect.position.x + rect.width - rightMargin + _space, rect.position.y);
            rect.width = rightMargin;
            return (rect);
        }

        protected override void DrawInSceneViewSelected(int index)
        {
            DrawUnselected(_vectorTarget, index);
        }

        [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
           | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(VectorPoints))]
        static void DrawGizmos(VectorPoints points, GizmoType selectionType)
        {
            if (points.gameObject == Selection.activeGameObject)
            {
                return;
            }
            if (!points.ShowWayPointsWhenUnselected)
            {
                return;
            }
            for (int i = 0; i < points.WaypointsCount; ++i)
            {
                DrawUnselected(points, i);
            }
        }

        protected static void DrawUnselected(VectorPoints point, int index)
        {
            if (!point.DisplayVector3)
            {
                return;
            }

            Vector3 posInSpline = point.GetPositionFromPoint(index) + Vector3.down * 0.3f;
            Vector3 localPointData = point.GetPoint(index);
            //Vector3 position = posInSpline + localPosition;
            string positionDisplay = (int)localPointData.x + " : " + (int)localPointData.y + " : " + (int)localPointData.z;
            PointsOnSplineExtensionEditor.DisplayStringInSceneViewFrom3dPosition(posInSpline, positionDisplay, point.ColorWayPoint, 10);
        }
    }
}