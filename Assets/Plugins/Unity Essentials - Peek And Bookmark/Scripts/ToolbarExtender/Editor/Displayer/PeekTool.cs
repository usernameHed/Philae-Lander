using UnityEditor;
using UnityEngine;
using UnityEssentials.Peek.Options;
using UnityEssentials.Peek.Extensions;

namespace UnityEssentials.Peek.ToolbarExtender
{
    public class PeekTool
    {
        private const int HEIGHT_TEXT = 8;
        private const int HEIGHT_BUTTON = 15;
        private const int SIZE_SLIDER = 120;

        public static readonly GUIStyle _miniText;
        private PeekToolbarDrawer _drawer = new PeekToolbarDrawer();

        static PeekTool()
        {
            _miniText = new GUIStyle()
            {
                fontSize = 9,
            };
            _miniText.normal.textColor = Color.white;
        }

        public void Init()
        {

        }

        public void DisplayLeft()
        {
            if (!EditorPrefs.GetBool(UnityEssentialsPreferences.SHOW_PEEK_MENU, true))
            {
                return;
            }
            float percent = EditorPrefs.GetFloat(UnityEssentialsPreferences.POSITION_IN_TOOLBAR, UnityEssentialsPreferences.DEFAULT_TOOLBAR_POSITION);
            if (percent > 0.5f)
            {
                return;
            }
            Rect left = ToolbarExtender.GetLeftRect();
            percent = ExtMathf.Remap(percent, 0f, 0.5f, 0f, 1f);
            float width = (left.width - SIZE_SLIDER) / 1 * percent;
            GUILayout.Label("", GUILayout.MinWidth(0), GUILayout.Width(width));
#if UNITY_2018_3_OR_NEWER
            Rect finalRect = SetupLocalRect(width);
            AddRightClickBehavior(finalRect);
#endif
            DisplayPeek();
        }

        public void DisplayRight()
        {
            if (!EditorPrefs.GetBool(UnityEssentialsPreferences.SHOW_PEEK_MENU, true))
            {
                return;
            }
            float percent = EditorPrefs.GetFloat(UnityEssentialsPreferences.POSITION_IN_TOOLBAR, UnityEssentialsPreferences.DEFAULT_TOOLBAR_POSITION);
            if (percent <= 0.5f)
            {
                return;
            }
            Rect left = ToolbarExtender.GetRightRect();
            percent = ExtMathf.Remap(percent, 0.5f, 1f, 0f, 1f);
            float width = (left.width - SIZE_SLIDER) / 1 * percent;
            GUILayout.Label("", GUILayout.MinWidth(0), GUILayout.Width(width));
#if UNITY_2018_3_OR_NEWER
            Rect finalRect = SetupLocalRect(width);
            AddRightClickBehavior(finalRect);
#endif
            DisplayPeek();
        }

        private void DisplayPeek()
        {
            if (_drawer.DisplayPeekToolBar())
            {
#if UNITY_2018_3_OR_NEWER
                CreateGenericMenu();
#endif
            }
        }


#if UNITY_2018_3_OR_NEWER
        private static Rect SetupLocalRect(float width)
        {
            Rect finalRect = GUILayoutUtility.GetLastRect();
            finalRect.x += width;
            finalRect.width = SIZE_SLIDER;
            finalRect.height = 27;
            return finalRect;
        }

        private void AddRightClickBehavior(Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
            {
                CreateGenericMenu();
            }
        }

        private void CreateGenericMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Settings"), false, OpenPreferenceSettings);
            menu.ShowAsContext();
            if (Event.current.type == EventType.ContextClick)
            {
                Event.current.Use();
            }
        }

        private void OpenPreferenceSettings()
        {
            SettingsService.OpenProjectSettings(UnityEssentialsPreferences.NAME_PREFERENCE);
        }
#endif
    }
}