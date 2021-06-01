
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEssentials.Extensions.Editor
{
    public static class ExtObjectEditor
    {
        public static string GetPath(this UnityEngine.Object asset)
        {
            return (AssetDatabase.GetAssetPath(asset));
        }

        public static bool IsTruelyNull(this object aRef)
        {
            return aRef != null && aRef.Equals(null);
        }

        public static Transform FindCommonParent(List<GameObject> gameObjects, Transform parent)
        {
            if (gameObjects.Count == 0 || parent == null || !AreGameObjectInSameScene(gameObjects))
            {
                return (null);
            }

            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].transform.parent == parent)
                {
                    continue;
                }
                parent = gameObjects[i].transform.parent.parent;
                return (FindCommonParent(gameObjects, parent));
            }
            return (parent);
        }

        public static bool AreGameObjectInSameScene(List<GameObject> gameObjects)
        {
            if (gameObjects.Count == 0)
            {
                return (false);
            }

            Scene scene = gameObjects[0].scene;
            for (int i = 1; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].scene != scene)
                {
                    return (false);
                }
            }
            return (true);
        }
        public static bool AreGameObjectInSameScene(GameObject[] gameObjects)
        {
            if (gameObjects.Length == 0)
            {
                return (false);
            }

            Scene scene = gameObjects[0].scene;
            for (int i = 1; i < gameObjects.Length; i++)
            {
                if (gameObjects[i].scene != scene)
                {
                    return (false);
                }
            }
            return (true);
        }
    }
}