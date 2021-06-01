using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEssentials.SceneWorkflow.extension;
using UnityEssentials.SceneWorkflow.extensions.editor;
using UnityEssentials.SceneWorkflow.PropertyAttribute.SceneReference;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.SceneWorkflow.toolbarExtent
{
    public class SceneWorkflowButtons
    {
        private const int HEIGHT_TEXT = 8;
        private const int HEIGHT_BUTTON = 15;
        private const int WIDTH_BUTTON = 23;
        private const int SIZE_SCENE_WORKFLOW = 120;
        public static readonly GUIStyle _miniText;

        private const string KEY_USED_FOR_CONTEXT_ASSET = "SCENE_WORKFLOW_CONTEXT_USED_FOR_EDITOR_TOOLBAR";

        private static ContextListerAsset _refGameAsset;

        static SceneWorkflowButtons()
        {
            _miniText = new GUIStyle()
            {
                fontSize = 9,
            };
            _miniText.normal.textColor = Color.white;
        }

        public void Init()
        {
            _refGameAsset = ExtSaveAssetGUID.GetAsset<ContextListerAsset>(KEY_USED_FOR_CONTEXT_ASSET, out string infos);
        }

        public void DisplaySliderLeft()
        {
            if (!EditorPrefs.GetBool(UnityEssentialsPreferences.SHOW_SCENE_BUTTONS, true))
            {
                return;
            }
            float percent = EditorPrefs.GetFloat(UnityEssentialsPreferences.POSITION_IN_TOOLBAR, UnityEssentialsPreferences.DEFAULT_POSITION_IN_TOOLBAR);
            if (percent > 0.5f)
            {
                return;
            }
            Rect left = ToolbarExtender.GetLeftRect();
            percent = Remap(percent, 0f, 0.5f, 0f, 1f);
            float width = (left.width - SIZE_SCENE_WORKFLOW) / 1f * percent;
            GUILayout.Label("", GUILayout.MinWidth(0), GUILayout.Width(width));
#if UNITY_2018_3_OR_NEWER
            Rect finalRect = SetupLocalRect(width);
            AddRightClickBehavior(finalRect);
#endif
            DisplaySceneForkflowButtons(width);
        }

        public void DisplaySliderRight()
        {
            if (!EditorPrefs.GetBool(UnityEssentialsPreferences.SHOW_SCENE_BUTTONS, UnityEssentialsPreferences.DEFAULT_SHOW_SCENE_BUTTON))
            {
                return;
            }
            float percent = EditorPrefs.GetFloat(UnityEssentialsPreferences.POSITION_IN_TOOLBAR, UnityEssentialsPreferences.DEFAULT_POSITION_IN_TOOLBAR);
            if (percent <= 0.5f)
            {
                return;
            }
            Rect left = ToolbarExtender.GetRightRect();
            percent = Remap(percent, 0.5f, 1f, 0f, 1f);
            float width = (left.width - SIZE_SCENE_WORKFLOW) / 1f * percent;
            GUILayout.Label("", GUILayout.MinWidth(0), GUILayout.Width(width));
#if UNITY_2018_3_OR_NEWER
            Rect finalRect = SetupLocalRect(width);
            AddRightClickBehavior(finalRect);
#endif
            DisplaySceneForkflowButtons(width);
        }

        private void DisplaySceneForkflowButtons(float width)
        {
            using (VerticalScope vertical = new VerticalScope())
            {
                GUILayout.Label("Scenes Context", ExtGUIStyles.MiniTextCentered, GUILayout.Height(HEIGHT_TEXT));
                using (HorizontalScope horizontal = new HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    _refGameAsset = EditorGUILayout.ObjectField(_refGameAsset, typeof(ContextListerAsset), false, GUILayout.Width(50), GUILayout.Height(HEIGHT_BUTTON)) as ContextListerAsset;
                    if (EditorGUI.EndChangeCheck())
                    {
                        ExtSaveAssetGUID.ResetKey(KEY_USED_FOR_CONTEXT_ASSET);
                        ExtSaveAssetGUID.SaveAssetReference(_refGameAsset, KEY_USED_FOR_CONTEXT_ASSET, "");
                    }
#if UNITY_2018_3_OR_NEWER
                    if (IsRightClickContext())
                    {
                        CreateGenericMenu();
                    }
#endif
                    if (_refGameAsset != null)
                    {
                        DisplayContextButtonsName();
                    }                    
                }
            }
        }

        private void DisplayContextButtonsName()
        {
            int currentLoaded = _refGameAsset.LastIndexUsed;
            for (int i = 0; i < _refGameAsset.CountSceneToLoad; i++)
            {
                bool currentLoadedScene = currentLoaded == i;

                bool showFullName = !EditorPrefs.GetBool(UnityEssentialsPreferences.SHOW_INDEX_OF_SCENE, UnityEssentialsPreferences.DEFAULT_SHOW_INDEX_OF_SCENE);
                string levelIndex = showFullName ? _refGameAsset.GetSceneAtIndex(i).NameContext : (i + 1).ToString();
                if (_refGameAsset.GetSceneAtIndex(i) == null)
                {
                    continue;
                }

                GUIContent content = new GUIContent(levelIndex);
                GUIStyle style = ExtGUIStyles.MicroButton;
                Vector2 size = style.CalcSize(content);

                float width = showFullName ? size.x : WIDTH_BUTTON;

                currentLoadedScene = GUILayout.Toggle(currentLoadedScene, new GUIContent(levelIndex, "Load context[" + levelIndex + "]: " + _refGameAsset.GetSceneAtIndex(i).NameContext), ExtGUIStyles.MicroButton, GUILayout.Width(width), GUILayout.Height(HEIGHT_BUTTON));
                if (Event.current.button == 0 && currentLoadedScene != (currentLoaded == i))
                {
                    if (!Application.isPlaying)
                    {
                        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            return;
                        }
                    }
                    _refGameAsset.LastIndexUsed = i;
                    _refGameAsset.LoadScenesByIndex(i, OnLoadedScenes, hardReload: true);
                    EditorGUIUtility.PingObject(_refGameAsset.GetSceneAtIndex(i));
                    Selection.activeObject = _refGameAsset.GetSceneAtIndex(i);
                    return;
                }
#if UNITY_2018_3_OR_NEWER
                if (IsRightClickContext())
                {
                    CreateGenericMenu();
                }
#endif
            }
        }

            private static ContextListerAsset GenerateContextReferencer()
        {
            ContextListerAsset globalContextLister = ExtScriptableObject.CreateAsset<ContextListerAsset>("Assets/Context Lister.asset");
            SceneContextAsset context = ExtScriptableObject.CreateAsset<SceneContextAsset>("Assets/Context 1.asset");
            context.NameContext = "Demo Scene List";
            SceneReference[] sceneItems = SceneReference.GetAllActiveScene();
            ExtList.Append(context.SceneToLoad, sceneItems.ToList());
            globalContextLister.AddContext(context);

            globalContextLister.Save();
            context.Save();
            AssetDatabase.Refresh();

            return (globalContextLister);
        }

        private void OnLoadedScenes(SceneContextAsset lister)
        {
            if (Application.isPlaying)
            {
                AbstractLinker.Instance?.InitFromPlay();
            }
            else
            {
                AbstractLinker.Instance?.InitFromEditor();
            }
        }

        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

#if UNITY_2018_3_OR_NEWER

        private bool IsRightClickContext()
        {
            return Event.current.type == EventType.Repaint &&
                                    GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)
                                    && Event.current.button == 1;
        }

        private static Rect SetupLocalRect(float width)
        {
            Rect finalRect = GUILayoutUtility.GetLastRect();
            finalRect.x += width;
            finalRect.width = SIZE_SCENE_WORKFLOW;
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

        [MenuItem("Unity Essentials/" + UnityEssentialsPreferences.SHORT_NAME_PREFERENCE + "/Settings")]
        private static void OpenSettingsViaMainContextMenu()
        {
            SettingsService.OpenProjectSettings(UnityEssentialsPreferences.NAME_PREFERENCE);
        }
#endif
    }
}