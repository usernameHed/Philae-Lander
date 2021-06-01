using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.symbolicLinks.extensions.editor;

namespace UnityEssentials.symbolicLinks.options
{
    public static class UnityEssentialsPreferences
    {
        public const string SHORT_NAME_PREFERENCE = "SymLinks Tool";

        public const string DEFAULT_COLOR                       = SHORT_NAME_PREFERENCE + " " + "DefaultColor";

        public const string SHOW_INDICATOR_ON_HIERARCHY_WINDOW  = SHORT_NAME_PREFERENCE + " " + "ShowIndicatorOnHierarchyWindow";
        public const string SHOW_INDICATOR_ON_PROJECT_WINDOW    = SHORT_NAME_PREFERENCE + " " + "ShowIndicatorOnProjectWindow";
        public const string SHOW_DOT_ON_PARENTS                = SHORT_NAME_PREFERENCE + " " + "ShowDotsOnParents";

        public const string NAME_PREFERENCE = "Project/UnityEssentials/" + SHORT_NAME_PREFERENCE;
        private static Color _currentColor = Color.red;

#if UNITY_2018_3_OR_NEWER
        private static HashSet<string> SEARCH_KEYWORDS = new HashSet<string>(new[]
        {
            "UnityEssential", "UnityEssentials", "Unity Essentials", "Unity Essential",
            "Symlinks",
            "Symlink",
            "Symbolic links",
            "Links",
            "Jonctions",
            "ShowIndicatorOnHierarchyWindow",
            "Show Indicator On Hierarchy Window",
            "ShowIndicatorOnProjectWindow",
            "Show Indicator On Project Window",
            "ShowDotsOnParent",
            "Show Dots On Parent",
            "DefaultColor", "Default Color"
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

        [MenuItem("Unity Essentials/" + UnityEssentialsPreferences.SHORT_NAME_PREFERENCE + "/Settings")]
        private static void OpenSettingsViaMainContextMenu()
        {
            SettingsService.OpenProjectSettings(UnityEssentialsPreferences.NAME_PREFERENCE);
        }
#else
        [PreferenceItem(SHORT_NAME_PREFERENCE)]
        public static void CreateSettingsPreferenceItem()
        {
            OnProviderCustomGUI();
        }
#endif

        private static void OnProviderCustomGUI()
        {
            EditorGUI.BeginChangeCheck();
            PreferenceGUI();
            if (EditorGUI.EndChangeCheck())
            {

            }
        }

        /// <summary>
        /// display inside preferences
        /// </summary>
        /// <param name="serialized"></param>
        private static void PreferenceGUI()
        {
            EditorGUILayout.HelpBox(SHORT_NAME_PREFERENCE + " Preferences", MessageType.Info);

            if (!ExtGUILayout.Section("Indicator Preferences", "FolderFavorite Icon", true, "Symlink fold options", 5))
            {
                ManageColor(DEFAULT_COLOR, "Default Color", "Default color of indicator");
                GUILayout.Label("");
                ManageBool(SHOW_INDICATOR_ON_HIERARCHY_WINDOW, "Show Indicator On Hierarchy", "Show indicator on hierarchy item", true);
                ManageBool(SHOW_INDICATOR_ON_PROJECT_WINDOW, "Show Indicator On Project", "Show indicator on project item (folder & assets)", true);
                ManageBool(SHOW_DOT_ON_PARENTS, "Show Dot on parents", "Show little dot indicator on parent item that have symlink item inside", true);
                GUILayout.Space(25);
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

        /// <summary>
        /// get the current color. If this is the first time, create it
        /// </summary>
        /// <returns></returns>
        public static Color GetDefaultColor()
        {
            if (!EditorPrefs.HasKey(DEFAULT_COLOR))
            {
                EditorPrefs.SetString(DEFAULT_COLOR, EditorJsonUtility.ToJson(_currentColor));
                return (_currentColor);
            }
            _currentColor = (Color)JsonUtility.FromJson(EditorPrefs.GetString(DEFAULT_COLOR), typeof(Color));
            return (_currentColor);
        }

        /// <summary>
        /// From a given key, add a colorField, display, and save when changes are made
        /// </summary>
        /// <param name="key"></param>
        /// <param name="description"></param>
        private static void ManageColor(string key, string description, string toolTip)
        {
            if (!EditorPrefs.HasKey(DEFAULT_COLOR))
            {
                EditorPrefs.SetString(DEFAULT_COLOR, EditorJsonUtility.ToJson(_currentColor));
            }
            EditorGUI.BeginChangeCheck();
            Color color = EditorGUILayout.ColorField(new GUIContent(description, toolTip), _currentColor);
            if (EditorGUI.EndChangeCheck())
            {
                _currentColor = color;
                EditorPrefs.SetString(key, EditorJsonUtility.ToJson(_currentColor));
            }
        }
    }
}