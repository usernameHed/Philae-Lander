using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline.Extensions.Editor;

namespace UnityEssentials.Spline.PointsOnSplineExtensions.Editor
{
    [CustomEditor(typeof(QuaternionPoints), true)]
    public class QuaternionPointsEditor : PointsOnSplineExtensionEditor
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Points Lister/Rotation", false, 10)]
        private static void CreatePointsLister()
        {
            GameObject whereToPut = CreatePointsLister("Rotation Points Lister");
            QuaternionPoints points = whereToPut.AddComponent(typeof(QuaternionPoints)) as QuaternionPoints;
            QuaternionPointsEditor pointsEditor = (QuaternionPointsEditor)CreateEditor((QuaternionPoints)points, typeof(QuaternionPointsEditor));
            pointsEditor.Construct(Color.gray);
            DestroyImmediate(pointsEditor);
            Selection.activeGameObject = points.gameObject;
        }

        protected QuaternionPoints _quaternionTarget { get { return target as QuaternionPoints; } }

        protected override SerializedProperty SerializedWaypoints
        {
            get
            {
                return (serializedObject.FindProperty(nameof(QuaternionPoints.Waypoints)));
            }
        }

        protected override Rect DisplayContent(int index, Rect rect, float rightMargin)
        {
            Rect r = new Rect(rect);
            r.width -= rightMargin;
            r.height -= _space;

            SerializedProperty waypoint = _waypointList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty point = waypoint.GetPropertie(nameof(QuaternionPoints.QuaternionWaypoint.Rotation));
            float floatFieldWidth = EditorGUIUtility.singleLineHeight * 0.5f;

            float oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = floatFieldWidth;
            {
                EditorGUI.BeginChangeCheck();
                Vector3 euler = point.quaternionValue.eulerAngles;
                Vector3 newEuler = EditorGUI.Vector3Field(r, "", euler);
                if (EditorGUI.EndChangeCheck())
                {
                    point.quaternionValue = Quaternion.Euler(newEuler);
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
            if (!_quaternionTarget.DisplayQuaternion)
            {
                return;
            }

            SerializedProperty waypoint = _waypointList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty point = waypoint.GetPropertie(nameof(QuaternionPoints.QuaternionWaypoint.Rotation));

            Quaternion currentRotationAtSpline = _quaternionTarget.SplineBase.EvaluateOrientation(_quaternionTarget.GetWayPoint(index).PathPosition);
            Quaternion localRotation = _quaternionTarget.GetRotation(index);

            Quaternion finalRotation = localRotation * currentRotationAtSpline;
            Vector3 position = _quaternionTarget.GetPositionFromPoint(index);

            ExtDrawGuizmos.DebugArrow(position, finalRotation * Vector3.forward, Color.blue);
            ExtDrawGuizmos.DebugArrow(position, finalRotation * Vector3.right, Color.red);
            ExtDrawGuizmos.DebugArrow(position, finalRotation * Vector3.up, Color.green);


            Matrix4x4 matrix = Matrix4x4.TRS(position, currentRotationAtSpline, Vector3.one * 0.6f);
            using (new Handles.DrawingScope(matrix))
            {
                EditorGUI.BeginChangeCheck();
                Quaternion rotationHandle = Handles.RotationHandle(localRotation, Vector3.zero);
                if (EditorGUI.EndChangeCheck())
                {
                    point.quaternionValue = rotationHandle;
                    this.ApplyModification();
                }
            }
        }

        //[DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy
        //   | GizmoType.InSelectionHierarchy | GizmoType.Pickable, typeof(QuaternionPoints))]
        //static void DrawGizmos(QuaternionPoints points, GizmoType selectionType)
        //{
        //    if (points.gameObject == Selection.activeGameObject)
        //    {
        //        return;
        //    }
        //    if (!points.ShowWayPointsWhenUnselected)
        //    {
        //        return;
        //    }
        //    for (int i = 0; i < points.WaypointsCount; ++i)
        //    {
        //        DrawUnselected(points, i);
        //    }
        //}

        protected static void DrawUnselected(QuaternionPoints point, int index)
        {
            if (!point.DisplayQuaternion)
            {
                return;
            }


            Quaternion currentRotationAtSpline = point.SplineBase.EvaluateOrientation(point.GetWayPoint(index).PathPosition);
            Quaternion localRotation = point.GetRotation(index);

            Quaternion finalRotation = localRotation * currentRotationAtSpline;
            Vector3 position = point.GetPositionFromPoint(index);

            ExtDrawGuizmos.DebugArrow(position, finalRotation * Vector3.forward, Color.blue);
            ExtDrawGuizmos.DebugArrow(position, finalRotation * Vector3.right, Color.red);
            ExtDrawGuizmos.DebugArrow(position, finalRotation * Vector3.up, Color.green);

            //Vector3 posInSpline = position + Vector3.down * 0.3f;
            //Vector3 rotationData = localRotation.eulerAngles;
            ////Vector3 position = posInSpline + localPosition;
            //string positionDisplay = (int)rotationData.x + " : " + (int)rotationData.y + " : " + (int)rotationData.z;
            //PointsOnSplineExtensionEditor.DisplayStringInSceneViewFrom3dPosition(posInSpline, positionDisplay, point.ColorWayPoint, 10);
        }
    }
}