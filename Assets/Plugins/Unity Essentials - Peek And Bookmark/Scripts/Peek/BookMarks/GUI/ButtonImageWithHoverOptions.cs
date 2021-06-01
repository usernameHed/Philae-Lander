using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Peek.gui
{
    public class ButtonImageWithHoverOptions
    {
        public string IconKey { get; private set; }
        public string IconHover { get; private set; }
        public GUIStyle GuiStyle { get; private set; }
        public string ToolTip { get; private set; }
        public GUILayoutOption[] Options { get; private set; }

        public ButtonImageWithHoverOptions(string iconKey, string iconHover, GUIStyle guiStyle, string toolTip, float height, params GUILayoutOption[] options)
        {
            IconKey = iconKey;
            IconHover = iconHover;
            GuiStyle = guiStyle;
            ToolTip = toolTip;
            Options = options;

            GuiStyle.fixedHeight = height;
        }

        public bool ButtonImageWithHover(float width, bool isEnabled, float height)
        {
            GUIContent styleDefault = EditorGUIUtility.IconContent(IconKey);
            GUIContent styleHover = EditorGUIUtility.IconContent(IconHover);
            GUIContent styleGoToScene = EditorGUIUtility.IconContent("console.infoicon.sml");
            styleHover.tooltip = ToolTip;
            styleGoToScene.tooltip = "Select related scene if possible (CTRL + clic to directly change scene)";

            Rect rt = GUILayoutUtility.GetRect(styleDefault, GuiStyle, Options);
            if (!isEnabled)
            {
                return (false);
            }

            bool clicked;
            if (rt.Contains(Event.current.mousePosition))
            {
                clicked = GUI.Button(new Rect(rt.x, rt.y, width, height), styleHover, GuiStyle) && Event.current.button == 0;
            }
            else
            {
                clicked = GUI.Button(new Rect(rt.x, rt.y, width, height), styleDefault, GuiStyle) && Event.current.button == 0;
            }
            return (clicked);
        }
    }
}