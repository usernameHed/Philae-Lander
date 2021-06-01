using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace UnityEssentials.Extensions.Editor
{
    /// <summary>
    /// scene view calculation
    /// </summary>
    public static class ExtSceneViewReflectionPicker
    {
        private static MethodInfo _internalPickClosestGameObject;
        private static MethodInfo _internalUnclip;

        /// <summary>
        /// usage: ExtSceneViewPicker.PickFromReflection(SceneView.lastActiveSceneView, Event.current.mousePosition);
        /// </summary>
        /// <param name="sceneView"></param>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public static GameObject PickFromReflection(SceneView sceneView, Vector2 mousePosition)
        {
            int allLayer = ~0;
            int matIndex = -1;
            return (PickObjectOnPos(sceneView.camera, allLayer, mousePosition, new GameObject[0], null, ref matIndex));
        }

        /// <summary>
        /// PICK A GAMEOBJECT FROM SCENE VIEW AT POSITION
        /// pick a gameObject from the sceneView at a given mouse position
        /// </summary>
        /// <param name="cam">current camera</param>
        /// <param name="layers">layer accepted</param>
        /// <param name="position">mouse position</param>
        /// <param name="ignore">ignored gameObjects</param>
        /// <param name="filter"></param>
        /// <param name="materialIndex"></param>
        /// <returns></returns>
        public static GameObject PickObjectOnPos(Camera cam, int layers, Vector2 position, GameObject[] ignore, GameObject[] filter, ref int materialIndex)
        {
            if (_internalUnclip == null || _internalPickClosestGameObject == null)
            {
                FindMethodByReflection();
                return (null);
            }
#if UNITY_2018_2_OR_NEWER
            position = (Vector2)_internalUnclip.Invoke(null, new object[] { position });
#else
            object[] arguments = new object[] { position };
            _internalUnclip.Invoke(null, arguments);
            position = (Vector2)arguments[0];
#endif

            position = EditorGUIUtility.PointsToPixels(position);
            position.y = Screen.height - position.y - cam.pixelRect.yMin;
            materialIndex = -1;

#if UNITY_2020_3_OR_NEWER
            return (GameObject)_internalPickClosestGameObject.Invoke(null, new object[] { cam, layers, position, ignore, filter, false, materialIndex });
#else
            return (GameObject)_internalPickClosestGameObject.Invoke(null, new object[] { cam, layers, position, ignore, filter, materialIndex });
#endif
        }

        /// <summary>
        /// Save reference of the Internal_PickClosestGO reflection method
        /// </summary>
        private static void FindMethodByReflection()
        {
            Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;
            System.Type handleUtilityType = editorAssembly.GetType("UnityEditor.HandleUtility");
            _internalPickClosestGameObject = handleUtilityType.GetMethod("Internal_PickClosestGO", BindingFlags.Static | BindingFlags.NonPublic);

            editorAssembly = typeof(GUIUtility).Assembly;
            handleUtilityType = editorAssembly.GetType("UnityEngine.GUIClip");
            _internalUnclip = handleUtilityType.GetMethod("Unclip_Vector2", BindingFlags.Static | BindingFlags.NonPublic);
        }
    }
}