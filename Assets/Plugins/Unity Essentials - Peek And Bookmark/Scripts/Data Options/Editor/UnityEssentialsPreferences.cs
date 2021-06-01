using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Peek.Extensions.Editor;
using UnityEssentials.Peek.Extensions.Editor.EventEditor;
using UnityEssentials.Peek.ToolbarExtender;
using static UnityEditor.EditorGUILayout;
using static UnityEssentials.Peek.Extensions.Editor.EventEditor.ExtEventEditor;
using static UnityEssentials.Peek.Extensions.Editor.EventEditor.ExtShortCutEditor;

namespace UnityEssentials.Peek.Options
{
    public static class UnityEssentialsPreferences
    {
        public const string SHORT_NAME_PREFERENCE   = "Peek And BookMark";

        private const string MAX_SELECTED_OBJECT_STORED = SHORT_NAME_PREFERENCE + " " + "Max Selected Object stored";
        public static int GetMaxSelectedObjectStored() { return (Mathf.Max(2, GetInt(MAX_SELECTED_OBJECT_STORED, 1000))); }

        private const string MAX_SELECTED_OBJECT_SHOWN = SHORT_NAME_PREFERENCE + " " + "Max Selected Object shown";
        public static int GetMaxObjectSHown() { return (Mathf.Max(2, GetInt(MAX_SELECTED_OBJECT_SHOWN, 100))); }

        public const string MAX_PINNED_OBJECT = SHORT_NAME_PREFERENCE + " " + "Max Pinned Object";
        public static int GetMaxPinnedObject() { return (Mathf.Max(2, GetInt(MAX_PINNED_OBJECT, 20))); }

        public const string SHOW_PEEK_MENU          = SHORT_NAME_PREFERENCE + " " + "Show Peek menu";
        public const string POSITION_IN_TOOLBAR     = SHORT_NAME_PREFERENCE + " " + "Position In ToolBar";
        public const string SHOW_GAMEOBJECTS_FROM_OTHER_SCENE = SHORT_NAME_PREFERENCE + " " + "Show GameOjbects from other scenes";
        public const string FONT_SIZE_PEEK_WINDOW = SHORT_NAME_PREFERENCE + " " + "Foot size peek window";
        
        public const string NAME_PREFERENCE = "Project/UnityEssentials/" + SHORT_NAME_PREFERENCE;
        private const int SPACING_SECTION = 25;
        private const int PADDING_BOTTOM_SECTION = 5;

        
        public const string SHORTCUT_MODIFIER_KEY_FOR_PREVIOUS_NEXT_SELECTION = "MODIFIER_KEY_FOR_PREVIOUS_NEXT_SELECTION";
        public const ModifierKey DEFAULT_MODIFIER_KEY = ModifierKey.CTRL;

        public const string SHORTCUT_KEY_FOR_PREVIOUS_SELECTION = "KEY_FOR_PREVIOUS_SELECTION";
        public const KeyCode DEFAULT_PREVIOUS_KEY = KeyCode.DownArrow;

        public const string SHORTCUT_KEY_FOR_NEXT_SELECTION = "KEY_FOR_NEXT_SELECTION";
        public const KeyCode DEFAULT_NEXT_KEY = KeyCode.UpArrow;

        public const int DEFAULT_FONT_PEEK_WINDOW_ITEMS = 14;
        public const float DEFAULT_TOOLBAR_POSITION = 0.05f;

#if UNITY_2018_3_OR_NEWER
        private static HashSet<string> SEARCH_KEYWORDS = new HashSet<string>(new[]
        {
            "UnityEssential", "UnityEssentials", "Unity Essentials", "Unity Essential",
            "Peek",
            "Peek n scroll",
            "Peek and scroll",
            "flow",
            "gameObject",
            "link",
            "bookmarks",
            "bookmark",
            "pin"
        });

        /// <summary>
        /// Settings Provider logic
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider(NAME_PREFERENCE, SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    OnProviderCustomGUI();
                },
                keywords = UnityEssentialsPreferences.SEARCH_KEYWORDS
            };
        }
#else
        [PreferenceItem(SHORT_NAME_PREFERENCE)]
        public static void CreateSettingsPreferenceItem()
        {
            OnProviderCustomGUI();
        }
#endif

        public static int GetInt(string key, int defaultValue)
        {
            if (!EditorPrefs.HasKey(key))
            {
                EditorPrefs.SetInt(key, defaultValue);
                return (defaultValue);
            }
            return (EditorPrefs.GetInt(key));
        }
        public static bool GetBool(string key, bool defaultValue)
        {
            if (!EditorPrefs.HasKey(key))
            {
                EditorPrefs.SetBool(key, defaultValue);
                return (defaultValue);
            }
            return (EditorPrefs.GetBool(key));
        }

        

        private static void OnProviderCustomGUI()
        {
            try
            {
                EditorGUI.BeginChangeCheck();
                PreferenceGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    ToolbarExtender.ToolbarExtender.Repaint();
                }
            }
            catch { }
        }

        /// <summary>
        /// display inside preferences
        /// </summary>
        /// <param name="serialized"></param>
        private static void PreferenceGUI()
        {
            EditorGUILayout.HelpBox(SHORT_NAME_PREFERENCE + " Preferences", MessageType.Info);

            if (!ExtGUILayout.Section("Peek Toolbar", "Slider Icon", true, "Peek toolbar fold options", PADDING_BOTTOM_SECTION))
            {
                ManageBool(SHOW_PEEK_MENU, "Show Peek menu", "Show main toolbar menu", true);
                ManageSlider(POSITION_IN_TOOLBAR, "Position in toolbar: ", "Desired Position on unity toolbar", DEFAULT_TOOLBAR_POSITION, 0f, 1f);
                GUILayout.Space(SPACING_SECTION);
            }

            if (!ExtGUILayout.Section("Shortcuts", "PreTextureAlpha", true, "Shortcut Peek Toolbar fold options", PADDING_BOTTOM_SECTION))
            {
                ExtShortCutEditor.ShortCutOneModifier2Keys("Selection Back & Forward: ",
                    "Shortcut to browse through previous selections",
                    SHORTCUT_MODIFIER_KEY_FOR_PREVIOUS_NEXT_SELECTION, DEFAULT_MODIFIER_KEY,
                    SHORTCUT_KEY_FOR_PREVIOUS_SELECTION, DEFAULT_PREVIOUS_KEY,
                    SHORTCUT_KEY_FOR_NEXT_SELECTION, DEFAULT_NEXT_KEY);

                GUILayout.Space(SPACING_SECTION);
            }

            if (!ExtGUILayout.Section("Peek Window", "winbtn_win_rest_h", true, "Peek window fold options", PADDING_BOTTOM_SECTION))
            {
                EditorGUI.BeginChangeCheck();
                {
                    ManagePositiveInt(MAX_SELECTED_OBJECT_STORED, "Max global storage", "max previous selection saved", 1000);
                    ManagePositiveInt(MAX_SELECTED_OBJECT_SHOWN, "Max previously selected shown", "max item shown inside the previously selected section", 100);
                    ManagePositiveInt(MAX_PINNED_OBJECT, "Max bookmark", "Max Bookmarked objects", 20);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (PeekSerializeObject.ShrunkListIfNeeded())
                    {
                        PeekSerializeObject.Save();
                    }                    
                }
                ManageBool(SHOW_GAMEOBJECTS_FROM_OTHER_SCENE, "Show GameOjbects from other scene", "the reference may be recover if user change scene, or Undo deletion", true);
                EditorGUI.BeginChangeCheck();
                {
                    ManageSliderInt(FONT_SIZE_PEEK_WINDOW, "Font of gameObject's name: ", "Font of the text inside each items", DEFAULT_FONT_PEEK_WINDOW_ITEMS, 6, 20);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    ExtGUIStyles.MicroButtonLeftCenter.fontSize = EditorPrefs.GetInt(FONT_SIZE_PEEK_WINDOW, 14);
                }
                DisplayClearListButton();
                GUILayout.Space(SPACING_SECTION);
            }
        }

        private static void DisplayClearListButton()
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                GUILayout.Label("Reset bookmarks & previously selected list: ", GUILayout.ExpandWidth(false));
                if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
                {
                    PeekSerializeObject.Reset();
                }
            }
        }

        private static void ManagePositiveInt(string key, string description, string toolTip, int defaultValue)
        {
            EditorGUI.BeginChangeCheck();
            int amount = EditorGUILayout.IntField(new GUIContent(description, toolTip), EditorPrefs.GetInt(key, defaultValue));
            amount = Mathf.Max(0, amount);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(key, amount);
            }
        }

        private static void ManageBool(string key, string description, string tooltip, bool defaultValue)
        {
            EditorGUI.BeginChangeCheck();
            bool show = GUILayout.Toggle(EditorPrefs.GetBool(key, defaultValue), new GUIContent(description, tooltip));
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(key, show);
            }
        }

        

        private static void ManageSlider(string key, string description, string toolTip, float defaultValue, float from, float to)
        {
            using (HorizontalScope horiz = new HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent(description, toolTip), GUILayout.ExpandWidth(false));

                EditorGUI.BeginChangeCheck();
                float sliderPosition = EditorGUILayout.Slider(EditorPrefs.GetFloat(key, defaultValue), from, to, GUILayout.Width(130));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetFloat(key, sliderPosition);
                }
            }
        }

        private static void ManageSliderInt(string key, string description, string toolTip, int defaultValue, int from, int to)
        {
            using (HorizontalScope horiz = new HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent(description, toolTip), GUILayout.ExpandWidth(false));

                EditorGUI.BeginChangeCheck();
                float sliderPosition = EditorGUILayout.Slider(EditorPrefs.GetInt(key, defaultValue), from, to, GUILayout.Width(130));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(key, (int)sliderPosition);
                }
            }
        }
    }
}