using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEssentials.Peek.ToolbarExtender;
using System.Collections.Generic;

namespace UnityEssentials.Peek
{
    public class SceneDeletionTrigger : UnityEditor.AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            try
            {
                foreach (string deletedAsset in deletedAssets)
                {
                    string extension = Path.GetExtension(deletedAsset);
                    if (string.Equals(extension, ".unity"))
                    {
                        PeekLogic.DeleteBookMarkedGameObjectLinkedToScene();
                        return;
                    }
                }
            }
            catch { }
        }
    }
}