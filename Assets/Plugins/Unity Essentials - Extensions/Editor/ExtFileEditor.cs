using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace UnityEssentials.Extensions.Editor
{
    public static class ExtFileEditor
    {
        /// <summary>
        /// Creates a directory at <paramref name="folder"/> if it doesn't exist
        /// </summary>
        /// <param name="folder">true if we created a new directory</param>
        public static bool CreateDirectoryIfNotExist(this string folder, bool refreshProject = false)
        {
            bool directoryCreated = ExtFile.CreateDirectoryIfNotExist(folder);
            if (directoryCreated && refreshProject)
            {
                AssetDatabase.Refresh();
            }
            return (directoryCreated);
        }

        public static void CreateEntirePathIfNotExist(string path)
        {
            string[] directories = path.Split('/');
            string finalPath = "";
            for (int i = 0; i < directories.Length - 1; i++)
            {
                finalPath += directories[i] + "/";
                CreateDirectoryIfNotExist(finalPath);
            }
        }
    }
}