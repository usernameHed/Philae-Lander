using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;


namespace UnityEssentials.SceneWorkflow.extension
{
    public static class ExtFileEditor
    {
        /// <summary>
        /// rename an asset, and return the name
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static string RenameAsset(string oldPath, string newName, bool refreshAsset)
        {
            if (ExtFile.IsDirectoryExist(oldPath))
            {
                throw new System.Exception("a file must be given, not a directory");
            }

            string pathWhitoutName = Path.GetDirectoryName(oldPath);
            pathWhitoutName = ExtFile.ReformatPathForUnity(pathWhitoutName);
            string extension = Path.GetExtension(oldPath);
            string newWantedName = ExtFile.ReformatPathForUnity(newName);

            AssetDatabase.RenameAsset(oldPath, newWantedName);
            if (refreshAsset)
            {
                AssetDatabase.Refresh();
            }
            string newPath = pathWhitoutName + "/" + newWantedName + extension;
            Debug.Log("renamed to: " + newPath);
            return (newPath);
        }
    }
}