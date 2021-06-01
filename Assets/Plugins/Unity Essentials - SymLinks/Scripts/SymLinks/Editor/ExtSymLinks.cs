using UnityEditor;
using UnityEngine;

namespace UnityEssentials.symbolicLinks
{
    /// <summary>
    /// An editor utility for easily creating symlinks in your project.
    /// 
    /// Adds a Menu item under `Assets/Create/Folder(Symlink)`, and
    /// draws a small indicator in the Project view for folders that are
    /// symlinks.
    /// </summary>
    public static class ExtSymLinks
    {
        private static GUIStyle _symlinkMarkerStyle = null;
        private static GUIContent _guiContent = null;
        private static GUIStyle SymlinkMarkerStyle
        {
            get
            {
                if (_symlinkMarkerStyle == null)
                {
                    _symlinkMarkerStyle = new GUIStyle(EditorStyles.label);
                    _symlinkMarkerStyle.normal.textColor = Color.blue;
                    _symlinkMarkerStyle.alignment = TextAnchor.MiddleRight;
                }
                return _symlinkMarkerStyle;
            }
        }

        public static void DisplayBigMarker(Rect r, string toolTip, Color color)
        {
            SymlinkMarkerStyle.normal.textColor = color;
            _guiContent = new GUIContent("<=>", toolTip);
            GUI.Label(r, _guiContent, ExtSymLinks.SymlinkMarkerStyle);
        }
        public static void DisplayTinyMarker(Rect r, string toolTip, Color color)
        {
            SymlinkMarkerStyle.normal.textColor = color;
            _guiContent = new GUIContent("*  ", toolTip);
            GUI.Label(r, _guiContent, ExtSymLinks.SymlinkMarkerStyle);
        }
        public static void DisplayPointMarker(Rect r, string toolTip, Color color)
        {
            SymlinkMarkerStyle.normal.textColor = color;
            _guiContent = new GUIContent(".  ", toolTip);
            GUI.Label(r, _guiContent, ExtSymLinks.SymlinkMarkerStyle);
        }

        public static void ResetSymLinksDatas()
        {
            SymLinksOnProjectWindowItemGUI.ResetSymLinksDatas();
            SymLinksOnHierarchyItemGUI.ResetSymLinksDatas();
        }
    }
}
