using UnityEngine;
using UnityEditor;
using UnityEssentials.Spline;
using UnityEssentials.Spline.editor;
using UnityEssentials.Geometry.MovableShape.spline;
using UnityEssentials.Geometry.MovableShape.spline.editor;
using UnityEssentials.Geometry.MovableShape.Line;
using UnityEssentials.Geometry.MovableShape.Line.editor;
using UnityEssentials.Geometry.MovableShape.triangle.editor;
using UnityEssentials.Geometry.MovableShape.triangle;

namespace UnityEssentials.Geometry.MovableShape.editor
{
	public static class ExtMovableShapeEditor
    {
        #region normal Shape
        [MenuItem("GameObject/Unity Essentials/Shape3D/Sphere", false, -100)]
        private static void CreateShapeSphere()
        {
            CreateGenericShape<MovableSphere>("SHAPE: Sphere");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Sphere Half", false, -100)]
        private static void CreateShapeSphereHalf()
        {
            CreateGenericShape<MovableSphereHalf>("SHAPE: Sphere Half");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Cube", false, -100)]
        private static void CreateShapeCube()
        {
            CreateGenericShape<MovableCube>("SHAPE: Cube");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Quad", false, -100)]
        private static void CreateShapeQuad()
        {
            CreateGenericShape<MovableQuad>("SHAPE: Quad");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Plane", false, -100)]
        private static void CreateShapePlane()
        {
            CreateGenericShape<MovablePlane>("SHAPE: Plane");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Capsule", false, -100)]
        private static void CreateShapeCapsule()
        {
            CreateGenericShape<MovableCapsule>("SHAPE: Capsule");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Capsule Half", false, -100)]
        private static void CreateShapeCapsuleHalf()
        {
            CreateGenericShape<MovableCapsuleHalf>("SHAPE: Capsule Half");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Cone", false, -100)]
        private static void CreateShapeCone()
        {
            CreateGenericShape<MovableConeSphereBase>("SHAPE: Cone");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Cylinder", false, -100)]
        private static void CreateShapeCylinder()
        {
            CreateGenericShape<MovableCylinder>("SHAPE: Cylinder");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Spline", false, -100)]
        private static void CreateShapeSpline()
        {
            MovableSpline zone = CreateGenericShape<MovableSpline>("SHAPE: Spline");
            SplineSmooth spline = SplineSmoothEditor.CreateSplineOnGameObject(zone.gameObject);

            UnityEditor.Editor splineEditorGeneric = UnityEditor.Editor.CreateEditor(zone, typeof(MovableSplineEditor));
            MovableSplineEditor zoneEditor = (MovableSplineEditor)splineEditorGeneric;
            zoneEditor.ConstructSplineZone(spline);
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Line", false, -100)]
        private static void CreateShapeLine()
        {
            MovableLine zone = CreateGenericShape<MovableLine>("SHAPE: Line");
            SimpleLine spline = LineEditor.CreateLineOnGameObject(zone.gameObject);

            UnityEditor.Editor splineEditorGeneric = UnityEditor.Editor.CreateEditor(zone, typeof(MovableLineEditor));
            MovableLineEditor zoneEditor = (MovableLineEditor)splineEditorGeneric;
            zoneEditor.ConstructSplineZone(spline);
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Triangle", false, -100)]
        private static void CreateShapeTriangle()
        {
            MovableTriangle zone = CreateGenericShape<MovableTriangle>("SHAPE: Triangle");
            Triangle spline = TriangleEditor.CreateLineOnGameObject(zone.gameObject);

            UnityEditor.Editor splineEditorGeneric = UnityEditor.Editor.CreateEditor(zone, typeof(MovableTriangleEditor));
            MovableTriangleEditor zoneEditor = (MovableTriangleEditor)splineEditorGeneric;
            zoneEditor.ConstructSplineZone(spline);
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Disc", false, -100)]
        private static void CreateShapeDisc()
        {
            CreateGenericShape<MovableDisc>("SHAPE: Disc");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Donut", false, -100)]
        private static void CreateShapeDonut()
        {
            CreateGenericShape<MovableDonut>("SHAPE: Donut");
        }
        #endregion

        #region Advanced Shape
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Sphere", false, -100)]
        private static void CreateShapeSphereAdvanced()
        {
            CreateGenericShape<MovableSphereAdvanced>("SHAPE: Sphere Advanced");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Sphere Half", false, -100)]
        private static void CreateShapeSphereHalfAdvanced()
        {
            CreateGenericShape<MovableSphereHalfAdvanced>("SHAPE: Sphere Half Advanced");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Cube", false, -100)]
        private static void CreateShapeCubeAdvanced()
        {
            CreateGenericShape<MovableCubeAdvanced>("SHAPE: Cube Advanced");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Quad", false, -100)]
        private static void CreateShapeQuadAdvanced()
        {
            CreateGenericShape<MovableQuadAdvanced>("SHAPE: Quad Advanced");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Capsule", false, -100)]
        private static void CreateShapeCapsuleAdvanced()
        {
            CreateGenericShape<MovableCapsuleAdvanced>("SHAPE: Capsule Advanced");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Capsule Half", false, -100)]
        private static void CreateShapeCapsuleHalfAdvanced()
        {
            CreateGenericShape<MovableCapsuleHalfAdvanced>("SHAPE: Capsule Half Advanced");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Cone", false, -100)]
        private static void CreateShapeConeAdvanced()
        {
            CreateGenericShape<MovableConeSphereBaseAdvanced>("SHAPE: Cone Advanced");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Cylinder", false, -100)]
        private static void CreateShapeCylinderAdvanced()
        {
            CreateGenericShape<MovableCylinderAdvanced>("SHAPE: Cylinder Advanced");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Spline", false, -100)]
        private static void CreateShapeSplineAdvanced()
        {
            MovableSplineAdvanced zone = CreateGenericShape<MovableSplineAdvanced>("SHAPE: Spline Advanced");
            SplineSmooth spline = SplineSmoothEditor.CreateSplineOnGameObject(zone.gameObject);

            UnityEditor.Editor splineEditorGeneric = UnityEditor.Editor.CreateEditor(zone, typeof(MovableSplineEditor));
            MovableSplineEditor zoneEditor = (MovableSplineEditor)splineEditorGeneric;
            zoneEditor.ConstructSplineZone(spline);
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Line", false, -100)]
        private static void CreateShapeLineAdvanced()
        {
            MovableLineAdvanced zone = CreateGenericShape<MovableLineAdvanced>("SHAPE: Line Advanced");
            SimpleLine spline = LineEditor.CreateLineOnGameObject(zone.gameObject);

            UnityEditor.Editor splineEditorGeneric = UnityEditor.Editor.CreateEditor(zone, typeof(MovableLineEditor));
            MovableLineEditor zoneEditor = (MovableLineEditor)splineEditorGeneric;
            zoneEditor.ConstructSplineZone(spline);
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Disc", false, -100)]
        private static void CreateShapeDiscAdvanced()
        {
            CreateGenericShape<MovableDiscAdvanced>("SHAPE: Disc Advanced");
        }
        [MenuItem("GameObject/Unity Essentials/Shape3D/Advanced/Donut", false, -100)]
        private static void CreateShapeDonutAdvanced()
        {
            CreateGenericShape<MovableDonutAdvanced>("SHAPE: Donut Advanced");
        }
        #endregion



        private static T CreateGenericShape<T>(string nameType) where T : MovableShape
        {
            GameObject zone = new GameObject(nameType);
            T zoneScript = zone.AddComponent<T>();

            if (Selection.activeGameObject != null)
            {
                zone.transform.SetParent(Selection.activeGameObject.transform);
            }

            Undo.RegisterCreatedObjectUndo(zone, nameType);
            Selection.activeGameObject = zone;
            SceneView.lastActiveSceneView.MoveToView(zone.transform);
            GameObjectUtility.EnsureUniqueNameForSibling(zone);

            ExtGameObjectIcon.SetIcon(zone, ExtGameObjectIcon.LabelIcon.Yellow);
            return (zoneScript);
        }
    }
}