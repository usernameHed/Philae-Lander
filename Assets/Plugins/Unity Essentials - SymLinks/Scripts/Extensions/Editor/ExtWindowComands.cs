using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;

namespace UnityEssentials.symbolicLinks.extensions.editor
{
    public static class ExtWindowComands
    {
        public static void Execute(string commandeLine)
        {
            try
            {
#if UNITY_EDITOR_WIN
                Process cmd = Process.Start("CMD.exe", commandeLine);
                cmd.WaitForExit();
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                Process cmd = Process.Start("ln", "-s " + commandeLine);
                cmd.WaitForExit();
#endif
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
            }
        }

        public static void ShowInExplorer(UnityEngine.Object asset)
        {
            ExtWindowComands.ShowInExplorer(AssetDatabase.GetAssetPath(asset));
        }

        public static void ShowInExplorer(string itemPath)
        {
            try
            {
                itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
#if UNITY_EDITOR_WIN
                System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
			// @todo
#endif
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
            }
        }

        /// <summary>
        /// open the folder panel, wait on output the folder path & folder name
        /// </summary>
        /// <param name="sourceFolderPath">path of the folder selected</param>
        /// <param name="sourceFolderName">name of the folder selected</param>
        /// <returns>false if abort</returns>
        public static bool OpenFolderPanel(ref string sourceFolderPath, ref string sourceFolderName)
        {
            sourceFolderPath = EditorUtility.OpenFolderPanel("Select Folder Source", "", "");
            sourceFolderName = "";

            if (sourceFolderPath.Contains(Application.dataPath))
            {
                throw new System.Exception("Cannot create a symlink to folder in your project!");
            }

            // Cancelled dialog
            if (string.IsNullOrEmpty(sourceFolderPath))
            {
                return (false);
            }

            sourceFolderName = Path.GetFileName(sourceFolderPath);

            if (string.IsNullOrEmpty(sourceFolderName))
            {
                UnityEngine.Debug.LogWarning("Couldn't deduce the folder name?");
                return (false);
            }
            return (true);
        }

        //end of class
    }
}