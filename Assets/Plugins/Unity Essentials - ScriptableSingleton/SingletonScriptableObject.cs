using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityEssentials.ScriptableObjectSingleton
{
    /// <summary>
    /// Abstract class for making reload-proof singletons out of ScriptableObjects
    /// Returns the asset created on the editor, or null if there is none
    /// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
    /// </summary>
    /// <typeparam name="T">Singleton type</typeparam>

    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        static T _instance = null;
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    _instance = GetAssetByGenericType<T>();
                }
#endif
                return _instance;
            }
        }

#if UNITY_EDITOR
        #region FindEditor
        /// <summary>
        /// find the first asset of type T
        /// </summary>
        /// <typeparam name="U">Type to find</typeparam>
        /// <param name="directory">directory where to search the asset</param>
        /// <returns>first asset found with type T</returns>
        public static U GetAssetByGenericType<U>(string directory = "Assets/", string searchExtension = "*.asset") where U : Object
        {
            List<U> assets = GetAssetsByGenericType<U>(directory, searchExtension);
            if (assets.Count == 0)
            {
                return (null);
            }
            return (assets[0]);
        }


        /// <summary>
        /// find all asset of type T
        /// </summary>
        /// <typeparam name="U">Type to find</typeparam>
        /// <returns>all asset found with type T, return an list<Object>, you have to cast them to use them, see GetAssetsByRaceScriptableObjects for an exemple</returns>
        public static List<U> GetAssetsByGenericType<U>(string directory = "Assets/", string searchExtension = "*.asset") where U : Object
        {
            List<U> AllAssetFound = new List<U>();

            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] infoAssets = dir.GetFiles(searchExtension, SearchOption.AllDirectories);

            for (int i = 0; i < infoAssets.Length; i++)
            {
                string relativePath = ConvertAbsoluteToRelativePath(infoAssets[i].FullName);

                System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(relativePath);
                if (assetType == null)
                {
                    continue;
                }
                U asset = (U)AssetDatabase.LoadAssetAtPath(relativePath, typeof(U));
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
        #endregion
#endif
    }
}