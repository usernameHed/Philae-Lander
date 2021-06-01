using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEssentials.symbolicLinks.extensions;
using UnityEssentials.symbolicLinks.options;

namespace UnityEssentials.symbolicLinks
{
    /// <summary>
    /// Determine the marker display on ProjectWindow items
    /// </summary>
    [InitializeOnLoad]
    public static class SymLinksOnProjectWindowItemGUI
    {
        public static List<string> AllSymLinksAssetPathSaved = new List<string>(300);
        private const string SYMLINK_TOOLTIP = "this folder is a symlink";
        private const string SYMLINK_PARENT_TOOLTIP = "parent of symlink folder";

        public static void AddPathOfSymLinkAsset(string path)
        {
            if (AllSymLinksAssetPathSaved.AddIfNotContain(path))
            {
                AllSymLinksAssetPathSaved.Sort();
            }
        }

        /// <summary>
        /// Static constructor subscribes to projectWindowItemOnGUI delegate.
        /// </summary>
        static SymLinksOnProjectWindowItemGUI()
		{
            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemGUI;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        public static void ResetSymLinksDatas()
        {
            AllSymLinksAssetPathSaved.Clear();
        }

        /// <summary>
        /// Draw a little indicator if folder is a symlink on the project window
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="r"></param>
        private static void OnProjectWindowItemGUI(string guid, Rect r)
		{
            try
			{
                if (!EditorPrefs.GetBool(UnityEssentialsPreferences.SHOW_INDICATOR_ON_PROJECT_WINDOW, true))
                {
                    return;
                }

                string path = AssetDatabase.GUIDToAssetPath(guid);

				if (string.IsNullOrEmpty(path))
                {
                    return;
                }

                FileAttributes attribs = File.GetAttributes(path);
                DetermineIfAssetIsOrIsInSymLink.UpdateSymLinksParent(path, ref AllSymLinksAssetPathSaved);
                if (DetermineIfAssetIsOrIsInSymLink.IsAttributeASymLink(attribs))
                {
                    ExtSymLinks.DisplayBigMarker(r, SYMLINK_TOOLTIP, UnityEssentialsPreferences.GetDefaultColor());
                }
                else if (DetermineIfAssetIsOrIsInSymLink.IsAttributeAFileInsideASymLink(path, attribs, ref AllSymLinksAssetPathSaved))
                {
                    ExtSymLinks.DisplayTinyMarker(r, "", UnityEssentialsPreferences.GetDefaultColor());
                }
                else if (EditorPrefs.GetBool(UnityEssentialsPreferences.SHOW_DOT_ON_PARENTS, true) && ExtPaths.IsLittlePathIsInsideListOfBigPath(path, AllSymLinksAssetPathSaved))
                {
                    ExtSymLinks.DisplayPointMarker(r, SYMLINK_PARENT_TOOLTIP, UnityEssentialsPreferences.GetDefaultColor());
                }
            }
			catch {}
		}
    }
}
