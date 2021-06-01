using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEssentials.SceneWorkflow.extensions.editor;
using UnityEssentials.SceneWorkflow.PropertyAttribute.SceneReference;
using UnityEssentials.SceneWorkflow.toolbarExtent;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.SceneWorkflow.toolbarExtent
{
    public static class UnityEssentialsPreferences
    {
        public const string SHORT_NAME_PREFERENCE = "Scene Workflow";

        public const string SHOW_SCENE_BUTTONS             = SHORT_NAME_PREFERENCE + " " + "Show Scene buttons";
        public const bool DEFAULT_SHOW_SCENE_BUTTON = true;
        public const string POSITION_IN_TOOLBAR     = SHORT_NAME_PREFERENCE + " " + "PositionInToolBar";

        public const string SHOW_INDEX_OF_SCENE = SHORT_NAME_PREFERENCE + " " + "Show index instead of names";
        public const bool DEFAULT_SHOW_INDEX_OF_SCENE = true;


        public const string NAME_PREFERENCE = "Project/UnityEssentials/" + SHORT_NAME_PREFERENCE;
        public const float DEFAULT_POSITION_IN_TOOLBAR = 0.28f;

        [MenuItem("Unity Essentials/" + UnityEssentialsPreferences.SHORT_NAME_PREFERENCE + "/Remove Deleted Scenes from")]
        public static void CleanUpDeletedScenes()
        {
            var currentScenes = EditorBuildSettings.scenes;
            var filteredScenes = currentScenes.Where(ebss => File.Exists(ebss.path)).ToArray();
            EditorBuildSettings.scenes = filteredScenes;

            ExtBuildSettings.OpenBuildSettings();
        }

#if UNITY_2018_3_OR_NEWER
        private static HashSet<string> SEARCH_KEYWORDS = new HashSet<string>(new[]
        {
            "UnityEssential", "UnityEssentials", "Unity Essentials", "Unity Essential",
            "Scene",
            "Scene Workflow"
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

        private static void OnProviderCustomGUI()
        {
            EditorGUI.BeginChangeCheck();
            PreferenceGUI();
            if (EditorGUI.EndChangeCheck())
            {
                ToolbarExtender.Repaint();
            }
        }

        /// <summary>
        /// display inside preferences
        /// </summary>
        /// <param name="serialized"></param>
        private static void PreferenceGUI()
        {
            EditorGUILayout.HelpBox(SHORT_NAME_PREFERENCE + " Preferences", MessageType.Info);

            if (!ExtGUILayout.Section("Scene Workflow options", "SceneAsset Icon", true, "ICI TOOLTIP", 5))
            {
                ManageBool(SHOW_SCENE_BUTTONS, "Show Scenes Buttons", "Show main toolbar menu", DEFAULT_SHOW_SCENE_BUTTON);
                ManageSlider(POSITION_IN_TOOLBAR, "Position in toolbar: ", "Desired Position on unity toolbar", DEFAULT_POSITION_IN_TOOLBAR, 0f, 1f);
                ManageBool(SHOW_INDEX_OF_SCENE, "Show only index of context", "Disable if you want to display the full name instead of index (may be too long for your toolbar)", DEFAULT_SHOW_INDEX_OF_SCENE);
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
    }
}