using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEssentials.Spline.Extensions.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtSceneView
    {
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

        /// <summary>
        /// focus on object and zoom
        /// </summary>
        /// <param name="objToFocus"></param>
        /// <param name="zoom"></param>
        public static void FocusOnSelection(GameObject objToFocus, float zoom = -1f)
        {
            FocusOnSelection(objToFocus.transform.position, zoom);
        }

        public static void FocusOnSelection(Vector3 position, float zoom = -1f)
        {
            if (ExtSceneView.IsClose(position, Vector3.zero, 0.1f))
            {
                return;
            }

            if (SceneView.lastActiveSceneView == null)
            {
                return;
            }

            SceneView.lastActiveSceneView.LookAt(position);
            if (zoom != -1)
            {
                ExtSceneView.ViewportPanZoomIn(zoom);
            }
        }

        /// <summary>
        /// test if a Vector3 is close to another Vector3 (due to floating point inprecision)
        /// compares the square of the distance to the square of the range as this
        /// avoids calculating a square root which is much slower than squaring the range
        /// </summary>
        /// <param name="val"></param>
        /// <param name="about"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private static bool IsClose(Vector3 val, Vector3 about, float range)
        {
            float close = (val - about).sqrMagnitude;
            return (close < range * range);
        }

        public static void Frame(Vector3 position)
        {
            SceneView.lastActiveSceneView.Frame(new Bounds(position, new Vector3(2, 2, 2)));
        }

        public static void Repaint()
        {
            if (SceneView.lastActiveSceneView == null)
            {
                return;
            }
            SceneView.lastActiveSceneView.Repaint();
        }

        /// <summary>
        /// display 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="toDisplay"></param>
        public static void DisplayStringInSceneView(Vector3 position, string toDisplay, Color color, int fontSize = 20)
        {
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontSize = fontSize;
            textStyle.normal.textColor = color;
            textStyle.alignment = TextAnchor.MiddleCenter;
            Handles.Label(position, toDisplay, textStyle);
        }


        public static Transform GetSceneViewCameraTransform()
        {
            return (SceneView.lastActiveSceneView.camera.gameObject.transform);
        }

        public static Camera Camera()
        {
            return (SceneView.lastActiveSceneView.camera);
        }

        public static void SetGameObjectToPositionOfSceneViewCamera(GameObject gameObject, bool alsoRotate = true, bool dezoomAfter = false)
        {
            gameObject.transform.position = SceneView.lastActiveSceneView.camera.gameObject.transform.position;
            if (alsoRotate)
            {
                gameObject.transform.rotation = SceneView.lastActiveSceneView.rotation;
            }

            if (dezoomAfter)
            {
                Vector3 dirCamera = QuaternionToDir(SceneView.lastActiveSceneView.rotation, Vector3.up);
                SceneView.lastActiveSceneView.LookAt(gameObject.transform.position + dirCamera, SceneView.lastActiveSceneView.camera.transform.rotation);
            }
        }

        /// <summary>
        /// prend un quaternion en parametre, et retourn une direction selon un repère
        /// </summary>
        /// <param name="quat">rotation d'un transform</param>
        /// <param name="up">Vector3.up</param>
        /// <returns>direction du quaternion</returns>
        public static Vector3 QuaternionToDir(Quaternion quat, Vector3 up)
        {
            return ((quat * up).normalized);
        }

        /// <summary>
        /// Set the zoom of the camera
        /// </summary>
        public static void ViewportPanZoomIn(float zoom = 5f)
        {
            if (SceneView.lastActiveSceneView.size > zoom)
            {
                SceneView.lastActiveSceneView.size = zoom;
                //SceneView.lastActiveSceneView.pivot = ;
            }

            SceneView.lastActiveSceneView.Repaint();
        }

        public static void PlaceGameObjectInFrontOfSceneView(GameObject go)
        {
            SceneView.lastActiveSceneView.MoveToView(go.transform);
        }

        public static void LockFromUnselect()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        private static Texture2D _staticRectTexture;
        private static GUIStyle _staticRectStyle;

        // Note that this function is only meant to be called from OnGUI() functions.
        public static void GUIDrawRect(Rect position, Color color)
        {
            if (_staticRectTexture == null)
            {
                _staticRectTexture = new Texture2D(1, 1);
            }

            if (_staticRectStyle == null)
            {
                _staticRectStyle = new GUIStyle();
            }

            _staticRectTexture.SetPixel(0, 0, color);
            _staticRectTexture.Apply();

            _staticRectStyle.normal.background = _staticRectTexture;
            GUI.Box(position, GUIContent.none, _staticRectStyle);
        }
    }
}
