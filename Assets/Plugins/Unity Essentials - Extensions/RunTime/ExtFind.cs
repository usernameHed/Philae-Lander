using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UnityEssentials.Extensions
{
    public static class ExtFind
    {
        /// <summary>
        /// get the first interface found
        /// </summary>
        /// <typeparam name="T">type of interface</typeparam>
        /// <returns></returns>
        public static T GetInterface<T>()
        {
            T interfaces = default(T);
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                GameObject[] rootGameObjects = SceneManager.GetSceneAt(i).GetRootGameObjects();
                List<T> interfaceFound = GetInterfaceFromScene<T>(rootGameObjects);
                if (interfaceFound.Count > 0)
                {
                    return (interfaceFound[0]);
                }
            }
            return interfaces;
        }

        /// <summary>
        /// get all interfaces
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetInterfaces<T>()
        {
            List<T> interfaces = new List<T>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                GameObject[] rootGameObjects = SceneManager.GetSceneAt(i).GetRootGameObjects();
                interfaces.Append(GetInterfaceFromScene<T>(rootGameObjects));
            }
            return interfaces;
        }

        private static List<T> GetInterfaceFromScene<T>(GameObject[] rootGameObjects)
        {
            List<T> interfaces = new List<T>();
            foreach (var rootGameObject in rootGameObjects)
            {
                T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                {
                    interfaces.Add(childInterface);
                }
            }
            return interfaces;
        }

        /// <summary>
        /// Get a script. Use it only in editor, it's extremly expensive in performance
        /// use: MyScript myScript = ExtUtilityFunction.GetScript<MyScript>();
        /// </summary>
        /// <typeparam name="T">type of script</typeparam>
        /// <returns>return the script found, or null if nothing found</returns>
        public static T GetScript<T>()
        {
            object obj = UnityEngine.Object.FindObjectOfType(typeof(T));

            if (obj != null)
            {
                return ((T)obj);
            }
            return (default(T));
        }

        /// <summary>
        /// Get a list of scripts. Use it only in editor, it's extremly expensive in performance
        /// use: MyScript [] myScript = ExtUtilityFunction.GetScripts<MyScript>();
        /// </summary>
        /// <typeparam name="T">type of script</typeparam>
        /// <returns>return the array of script found</returns>
        public static T[] GetScripts<T>()
        {
            object[] obj = UnityEngine.Object.FindObjectsOfType(typeof(T));
            T[] objType = new T[obj.Length];


            if (obj != null)
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    objType[i] = (T)obj[i];
                }
            }

            return (objType);
        }
    }
}