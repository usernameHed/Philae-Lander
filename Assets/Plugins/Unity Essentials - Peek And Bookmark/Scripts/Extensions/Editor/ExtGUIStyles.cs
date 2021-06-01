using UnityEditor;
using UnityEngine;
using UnityEssentials.Peek.Options;

namespace UnityEssentials.Peek.Extensions.Editor
{
    static class ExtGUIStyles
    {
        public static GUIStyle MiniText;
        public static GUIStyle MicroButton;
        public static GUIStyle MicroButtonLeftCenter;
        public static GUIStyle MicroButtonScene;
        public static GUIStyle MiniTextCentered;

        static ExtGUIStyles()
        {
            MiniText = new GUIStyle()
            {
                fontSize = 9,
            };
            MiniText.normal.textColor = Color.white;

            MicroButton = new GUIStyle(EditorStyles.miniButton);
            MicroButton.fontSize = 9;

            MicroButtonLeftCenter = new GUIStyle(EditorStyles.miniButton);
            MicroButtonLeftCenter.fontSize = EditorPrefs.GetInt(UnityEssentialsPreferences.FONT_SIZE_PEEK_WINDOW, 14);
            MicroButtonLeftCenter.alignment = TextAnchor.MiddleLeft;
            //overriden in peek
            MicroButtonLeftCenter.fixedHeight = EditorGUIUtility.singleLineHeight;
            MicroButtonLeftCenter.margin = new RectOffset(MicroButtonLeftCenter.margin.left, MicroButtonLeftCenter.margin.right, 0, 0);

            MicroButtonScene = new GUIStyle(EditorStyles.miniButton);
            MicroButtonScene.fontSize = EditorPrefs.GetInt(UnityEssentialsPreferences.FONT_SIZE_PEEK_WINDOW, 14);
            MicroButtonScene.alignment = TextAnchor.MiddleCenter;
            MicroButtonScene.margin = new RectOffset(MicroButtonLeftCenter.margin.left, MicroButtonLeftCenter.margin.right, 0, 0);
            //overriden in peek
            MicroButtonScene.fixedHeight = EditorGUIUtility.singleLineHeight;
            MicroButtonScene.normal.background = null;// ExtGUILayout.MakeTex(1, 1, new Color(0.5f, 0.5f, 0.5f, 0.1f));
            MicroButtonScene.hover.background = ExtGUILayout.MakeTex(1, 1, new Color(1f, 1f, 1f, 0.1f));
            MicroButtonScene.active.background = ExtGUILayout.MakeTex(1, 1, new Color(0.1f, 0.1f, 0.1f, 0.1f));

            MiniTextCentered = new GUIStyle()
            {
                fontSize = 9,
            };
            MiniTextCentered.normal.textColor = Color.white;
            MiniTextCentered.alignment = TextAnchor.MiddleCenter;
        }
    }
}