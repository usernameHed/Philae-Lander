using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Spline.PointsOnSplineExtensions.Editor
{
    [CustomEditor(typeof(BasicPoints), true)]
    public class BasicPointsEditor : PointsOnSplineExtensionEditor
    {
        [MenuItem("GameObject/Unity Essentials/Splines/Points Lister/Basic", false, 10)]
        private static void CreatePointsLister()
        {
            GameObject whereToPut = CreatePointsLister("Basic Points Lister");
            BasicPoints points = whereToPut.AddComponent(typeof(BasicPoints)) as BasicPoints;
            BasicPointsEditor pointsEditor = (BasicPointsEditor)CreateEditor((BasicPoints)points, typeof(BasicPointsEditor));
            pointsEditor.Construct(Color.gray);
            DestroyImmediate(pointsEditor);
            Selection.activeGameObject = points.gameObject;
        }

        protected BasicPoints _vectorTarget { get { return target as BasicPoints; } }

        protected override SerializedProperty SerializedWaypoints
        {
            get
            {
                return (serializedObject.FindProperty(nameof(BasicPoints.Waypoints)));
            }
        }

        protected override Rect DisplayContent(int index, Rect rect, float rightMargin)
        {
            Rect r = new Rect(rect);
            r.width -= rightMargin;
            r.height -= _space;

            rect.position = new Vector2(rect.position.x + rect.width - rightMargin + _space, rect.position.y);
            rect.width = rightMargin;
            return (rect);
        }

        protected override void DrawInSceneViewSelected(int index)
        {
            
        }
    }
}