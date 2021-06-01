using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace UnityEssentials.Extensions.Editor.EventEditor
{
    public class CallBackSceneDeletion : UnityEditor.AssetPostprocessor
    {
        public delegate void OnSceneDeleted(List<string> scenesDeleted);
        public static event OnSceneDeleted OnDeleteScenes;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            try
            {
                List<string> sceneDeleted = new List<string>(deletedAssets);
                foreach (string deletedAsset in deletedAssets)
                {
                    string extension = Path.GetExtension(deletedAsset);
                    if (string.Equals(extension, ".unity"))
                    {
                        sceneDeleted.Add(deletedAsset);
                        return;
                    }
                }
                OnDeleteScenes?.Invoke(sceneDeleted);
            }
            catch { }
        }
    }
}