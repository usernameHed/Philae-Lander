using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.Extensions.Editor
{
    public static class ExtGUILayout
    {
        private const string FOLD_ICON = "►";
        private const string UNFOLD_ICON = "▼";

        public static HorizontalScope HorizontalScope(Color color, params GUILayoutOption[] options)
        {
            GUIStyle BackGroundWhite = new GUIStyle();
            BackGroundWhite.normal.background = MakeTex(600, 1, color);
            return (new HorizontalScope(BackGroundWhite, options));
        }

        /// <summary>
        /// Use in editor:
        /// GUIStyle backGround = new GUIStyle();
        /// backGround.normal.background = MakeTex(600, 1, Color.red);
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public static void DisplayShortcutSection(UnityAction display, string title = "Section", string logo = "Settings", bool canFold = true, string keyFold = "SHORTCUT_FOLD_EDITOR_SETTING", int paddingBottom = 5, int spacingOption = 25)
        {
            if (!ExtGUILayout.Section(title, logo, canFold, keyFold, paddingBottom))
            {
                display?.Invoke();
                GUILayout.Space(spacingOption);
            }
        }

        /// <summary>
        /// usage: 
        /// ExtGUILayout.Section("BookMark in scenes", widthWithoutScrollBar, "UnityLogo");
        /// logo to find: https://github.com/halak/unity-editor-icons
        /// </summary>
        /// <param name="title"></param>
        /// <param name="currentWidth"></param>
        /// <param name="logo"></param>
        public static bool Section(string title, string logo, bool canFold, string keyFold, int paddinBottom)
        {
            using (new HorizontalScope())
            {
                GUILayout.Label("", GUILayout.Width(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(10));

                if (!string.IsNullOrEmpty(logo))
                {
                    GUIContent icon = EditorGUIUtility.IconContent(logo);
                    GUILayout.Label(icon, GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                }
                GUIStyle centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                centeredStyle.alignment = TextAnchor.UpperCenter;
                GUILayout.Label(title, centeredStyle);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MinWidth(10));
            }

            if (!canFold)
            {
                GUILayout.Space(paddinBottom);
                return (false);
            }

            bool foldState = EditorPrefs.GetBool(keyFold, false);
            string foldIcon = (foldState) ? FOLD_ICON : UNFOLD_ICON;

            Rect rect = GUILayoutUtility.GetLastRect();
            rect = new Rect(rect.x, rect.y + 4f, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            GUIStyle foldButtonStyle = GUIStyle.none;
            foldButtonStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1);
            if (GUI.Button(rect, foldIcon, foldButtonStyle))
            {
                foldState = !foldState;
                EditorPrefs.SetBool(keyFold, foldState);
            }
            GUILayout.Space(paddinBottom);

            bool isFolded = EditorPrefs.GetBool(keyFold, false);
            return (isFolded);
        }

        public static bool DrawDisplayDialog(
                string mainTitle = "main title",
                string content = "what's the dialog box say",
                string accept = "Yes",
                string no = "Cancel")
        {
            if (!EditorUtility.DisplayDialog(mainTitle, content, accept, no))
            {
                return (false);
            }
            return (true);
        }
    }
}