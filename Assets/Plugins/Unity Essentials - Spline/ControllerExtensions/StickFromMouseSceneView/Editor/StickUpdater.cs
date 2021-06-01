using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline.Controller;
using UnityEssentials.Spline.Controller.editor;
using UnityEssentials.Spline.Extensions;

namespace UnityEssentials.Spline.ControllerExtensions
{
    [InitializeOnLoad]
    public static class StickUpdater
    {
        public static List<StickFromMouseSceneView> StickReference = new List<StickFromMouseSceneView>(2);
        private static FrequencyCoolDown _coolDown = new FrequencyCoolDown();


        static StickUpdater()
        {
            StickReference.Clear();
        }

        public static void SetReference(StickFromMouseSceneView stick)
        {
            if (StickReference.Count == 0)
            {
#if UNITY_2019_1_OR_NEWER
                SceneView.duringSceneGui -= DuringSceneGUI;
                SceneView.duringSceneGui += DuringSceneGUI;
#else
                SceneView.onSceneGUIDelegate -= DuringSceneGUI;
                SceneView.onSceneGUIDelegate += DuringSceneGUI;
#endif
            }
            StickReference.AddIfNotContain(stick);
        }

        /// <summary>
        /// Clean  null item (do not remove items, remove only the list)
        /// </summary>
        /// <param name="listToClean"></param>
        /// <returns>true if list changed</returns>
        public static List<T> CleanNullFromList<T>(this List<T> listToClean, ref bool hasChanged)
        {
            hasChanged = false;
            if (listToClean == null)
            {
                return (listToClean);
            }
            for (int i = listToClean.Count - 1; i >= 0; i--)
            {
                if (listToClean[i] == null || listToClean[i].Equals(null))
                {
                    listToClean.RemoveAt(i);
                    hasChanged = true;
                }
            }
            return (listToClean);
        }

        public static bool AddIfNotContain<T>(this List<T> list, T item)
        {
            if (item == null)
            {
                return (false);
            }

            if (!list.Contains(item))
            {
                list.Add(item);
                return (true);
            }
            return (false);
        }

        private static void DuringSceneGUI(SceneView sceneView)
        {
            if (StickReference.Count == 0)
            {
#if UNITY_2019_1_OR_NEWER
                SceneView.duringSceneGui -= DuringSceneGUI;
#else
                SceneView.onSceneGUIDelegate -= DuringSceneGUI;
#endif
                return;
            }

            if (_coolDown.GetTimer() > 0.1f)
            {
                _coolDown.StartCoolDown(0.1f);
            }
            if (_coolDown.IsRunning())
            {
                return;
            }
            _coolDown.StartCoolDown(0.1f);

            for (int i = StickReference.Count - 1; i >= 0; i--)
            {
                if (StickReference[i] == null)
                {
                    StickReference.RemoveAt(i);
                    continue;
                }
                if (StickReference[i].IsTriggeringAction() && EditorWindow.mouseOverWindow == sceneView)
                {
                    ApplyPosition(StickReference[i], sceneView);
                }
            }
        }

        private static void ApplyPosition(StickFromMouseSceneView stick, SceneView sceneView)
        {
            Ray mouseRay = CalculateMousePosition(sceneView);

            if (SplineLister.Instance != null && SplineLister.Instance.SplineCount > 0)
            {
                List<SplineBase> splines = SplineLister.Instance.GetSplineBases();
                Vector3[] closestSpline = new Vector3[splines.Count];
                float[] closestPathPosition = new float[splines.Count];

                for (int i = 0; i < splines.Count; i++)
                {
                    float closest = splines[i].FindClosestPointFromRay(mouseRay, 100, 1, stick.PositionUnits);
                    closestSpline[i] = splines[i].EvaluatePositionAtUnit(closest, stick.PositionUnits);
                    closestPathPosition[i] = closest;
                }

                int closestIndex = GetClosestPointToRay(mouseRay, out float minDist, closestSpline);
                ControllerStickEditor pointsEditor = (ControllerStickEditor)UnityEditor.Editor.CreateEditor((ControllerStick)stick.ControllerStick, typeof(ControllerStickEditor));
                pointsEditor.ChangeSplineAndPosition(splines[closestIndex], closestPathPosition[closestIndex]);
                GameObject.DestroyImmediate(pointsEditor);
            }
            else
            {
                float closest = stick.Spline.FindClosestPointFromRay(mouseRay, 100, 1, stick.PositionUnits);
                ChangePosition(stick.ControllerStick, closest);
            }
        }

        /// <summary>
        /// from a set of points, return the closest one to the ray
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ray"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static int GetClosestPointToRay(Ray ray, out float minDist, params Vector3[] points)
        {
            minDist = 0;
            if (points.Length == 0)
            {
                return (-1);
            }
            Vector3 p1 = ray.origin;
            Vector3 p2 = ray.GetPoint(1);
            minDist = DistancePointToLine3D(points[0], p1, p2);
            int index = 0;
            for (int i = 1; i < points.Length; i++)
            {
                float dist = DistancePointToLine3D(points[i], p1, p2);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }
            return (index);
        }

        private static float DistancePointToLine3D(Vector3 point, Vector3 lineP1, Vector3 lineP2)
        {
            return (Vector3.Cross(lineP1 - lineP2, point - lineP1).magnitude);
        }

        private static void ChangePosition(ControllerStick controller, float newPosition)
        {
            ControllerStickEditor pointsEditor = (ControllerStickEditor)UnityEditor.Editor.CreateEditor((ControllerStick)controller, typeof(ControllerStickEditor));
            pointsEditor.ChangePosition(newPosition);
            GameObject.DestroyImmediate(pointsEditor);
        }

        /// <summary>
        /// From a given scene view, return the raycast of the mouse
        /// </summary>
        /// <param name="sceneView"></param>
        /// <returns></returns>
        public static Ray CalculateMousePosition(SceneView sceneView)
        {
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.x /= sceneView.position.width; //scale to size
            mousePosition.y /= sceneView.position.height;
            mousePosition.z = 1; //set Z to a sensible non-zero value so the raycast goes in the right direction
            mousePosition.y = 1 - mousePosition.y; //invert Y because UIs are top-down and cameras are bottom-up

            Ray ray = sceneView.camera.ViewportPointToRay(mousePosition);
            return (ray);
        }
    }
}