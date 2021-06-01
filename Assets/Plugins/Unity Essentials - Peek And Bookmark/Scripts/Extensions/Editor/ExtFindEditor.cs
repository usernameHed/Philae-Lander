using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UnityEssentials.Peek.Extensions.Editor
{
    public static class ExtFindEditor
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
                ExtFindEditor.Append(interfaces, GetInterfaceFromScene<T>(rootGameObjects));
            }
            return interfaces;
        }

        /// <summary>
        /// from a current list, append a second list at the end of the first list
        /// </summary>
        /// <typeparam name="T">type of content in the lists</typeparam>
        /// <param name="currentList">list where we append stuffs</param>
        /// <param name="listToAppends">list to append to the other</param>
        public static void Append<T>(this IList<T> currentList, IList<T> listToAppends)
        {
            if (listToAppends == null)
            {
                return;
            }
            for (int i = 0; i < listToAppends.Count; i++)
            {
                currentList.Add(listToAppends[i]);
            }
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
        /// use: MyScript myScript = ExtFind.GetScript<MyScript>();
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

        /// <summary>
        /// find the first asset of type T
        /// </summary>
        /// <typeparam name="T">Type to find</typeparam>
        /// <param name="directory">directory where to search the asset</param>
        /// <returns>first asset found with type T</returns>
        public static T GetAssetByGenericTypeAndName<T>(string directory = "Assets/", string nameAsset = "Default", string searchExtension = "*.asset") where T : Object
        {
            List<T> assets = GetAssetsByGenericType<T>(directory, searchExtension);
            if (assets.Count == 0)
            {
                return (null);
            }
            for (int i = 0; i < assets.Count; i++)
            {
                if (assets[i].name.Equals(nameAsset))
                {
                    return (assets[i]);
                }
            }
            return (null);
        }

        /// <summary>
        /// find the first asset of type T
        /// </summary>
        /// <typeparam name="T">Type to find</typeparam>
        /// <param name="directory">directory where to search the asset</param>
        /// <returns>first asset found with type T</returns>
        public static T GetAssetByGenericType<T>(string directory = "Assets/", string searchExtension = "*.asset") where T : Object
        {
            List<T> assets = GetAssetsByGenericType<T>(directory, searchExtension);
            if (assets.Count == 0)
            {
                return (null);
            }
            return (assets[0]);
        }

        public static T GetAssetAtPathOfType<T>(string pathAsset = "Assets/Path/of/file") where T : Object
        {
            System.Type typeToSearch = typeof(T);
            T asset = (T)AssetDatabase.LoadAssetAtPath(pathAsset, typeToSearch);
            return (asset);
        }

        public static Object GetPrefabsAtPath(string pathPrefabs)
        {
            Object asset = AssetDatabase.LoadAssetAtPath(pathPrefabs, typeof(Object));
            return (asset);
        }


        /// <summary>
        /// find all asset of type T
        /// </summary>
        /// <typeparam name="T">Type to find</typeparam>
        /// <returns>all asset found with type T, return an list<Object>, you have to cast them to use them, see GetAssetsByRaceScriptableObjects for an exemple</returns>
        public static List<T> GetAssetsByGenericType<T>(string directory = "Assets/", string searchExtension = "*.asset") where T : Object
        {
            List<T> AllAssetFound = new List<T>();

            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] infoAssets = dir.GetFiles(searchExtension, SearchOption.AllDirectories);

            System.Type typeToSearch = typeof(T);

            for (int i = 0; i < infoAssets.Length; i++)
            {
                string relativePath = ConvertAbsoluteToRelativePath(infoAssets[i].FullName);

                System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(relativePath);
                if (assetType == null)
                {
                    continue;
                }
                T asset = (T)AssetDatabase.LoadAssetAtPath(relativePath, typeToSearch);
                if (asset)
                {
                    AllAssetFound.Add(asset);
                }
            }
            return (AllAssetFound);
        }

        public static string ConvertAbsoluteToRelativePath(string absolutePath)
        {
            string gameDataFolder = Application.dataPath;
            absolutePath = ReformatPathForUnity(absolutePath);

            if (absolutePath.StartsWith(gameDataFolder))
            {
                string relativepath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
                return (relativepath);
            }
            return (absolutePath);
        }

        /// <summary>
        /// change a path from
        /// Assets\path\of\file
        /// to
        /// Assets/path/of/file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReformatPathForUnity(string path, char characterReplacer = '-')
        {
            string formattedPath = path.Replace('\\', '/');
            formattedPath = formattedPath.Replace('|', characterReplacer);
            return (formattedPath);
        }

        /// <summary>
        /// get an asset type with the path given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static T GetAssetByPath<T>(string relativePath) where T : Object
        {
            T asset = (T)AssetDatabase.LoadAssetAtPath(relativePath, typeof(T));
            return (asset);
        }
    }
}