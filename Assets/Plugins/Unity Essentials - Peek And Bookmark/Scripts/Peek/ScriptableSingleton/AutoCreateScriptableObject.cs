using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Peek.ScriptableObjectSingleton
{
    /// <summary>
    /// 
    /// </summary
    [InitializeOnLoad]
    public static class AutoCreateScriptableObject
    {
        static AutoCreateScriptableObject()
        {
            //CreateFmodPathAssetIfAny("PeekSaveDatas", "Assets/Peek And Bookmark/Unity Essentials - Peek And Bookmark/Resources/");
        }

        private static void CreateFmodPathAssetIfAny(string assetName, string folderParent)
        {
            if (PeekSaveDatas.Instance == null)
            {
                PeekSaveDatas saveDatas = ScriptableObject.CreateInstance<PeekSaveDatas>();
                CreateEntirePathIfNotExist(folderParent);
                AssetDatabase.CreateAsset(saveDatas, folderParent + assetName + ".asset");
            }
        }


        public static void CreateEntirePathIfNotExist(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            string[] directories = path.Split('/');
            string finalPath = "";
            for (int i = 0; i < directories.Length - 1; i++)
            {
                finalPath += directories[i] + "/";
                CreateDirectoryIfNotExist(finalPath);
            }
        }

        /// <summary>
        /// Creates a directory at <paramref name="folder"/> if it doesn't exist
        /// </summary>
        /// <param name="folder">true if we created a new directory</param>
        public static bool CreateDirectoryIfNotExist(this string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                return (false);
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                return (true);
            }
            return (false);
        }
    }
}