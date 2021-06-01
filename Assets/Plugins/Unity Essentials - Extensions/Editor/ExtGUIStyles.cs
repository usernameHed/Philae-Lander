using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor
{
    static class ExtGUIStyles
    {
        public static readonly GUIStyle MiniText;
        public static readonly GUIStyle MicroButton;
        public static readonly GUIStyle MiniTextCentered;

        static ExtGUIStyles()
        {
            MiniText = new GUIStyle()
            {
                fontSize = 9,
            };
            MiniText.normal.textColor = Color.white;

            MicroButton = new GUIStyle(EditorStyles.miniButton);
            MicroButton.fontSize = 9;

            MiniTextCentered = new GUIStyle()
            {
                fontSize = 9,
            };
            MiniTextCentered.normal.textColor = Color.white;
            MiniTextCentered.alignment = TextAnchor.MiddleCenter;
        }
    }
}