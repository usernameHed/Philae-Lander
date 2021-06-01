using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEssentials.Peek.Extensions;
using UnityEssentials.Peek.Extensions.Editor;
using UnityEssentials.Peek.Extensions.Editor.EventEditor;
using UnityEssentials.Peek.Options;
using static UnityEssentials.Peek.Extensions.Editor.EventEditor.ExtShortCutEditor;

namespace UnityEssentials.Peek
{
    [InitializeOnLoad]
    public static class PeekLogic
    {
        private static PeekReloadSceneBookMark _peekReloadSceneBookMark = new PeekReloadSceneBookMark();
        public static PeekReloadSceneBookMark PeekReloadBookMark { get { return (_peekReloadSceneBookMark); } }
        private static bool _isInit = false;
        private static bool _isClosed = false;
        private static EditorChrono _frequencyCoolDown = new EditorChrono();
        private static List<Scene> _sceneLoaded = new List<Scene>(10);

        private static bool _needToChangeScene = false;
        private static string _toSelectName;
        private static string _sceneOfGameObjectToSelectName;
        private static string _scenePathOfGameObjectToSelect;

        private static PeekEditorWindow _displayInside = null;
        private const string KEY_EDITOR_PREF_SAVE_LAST_SELECTION_CLOSED = "KEY_EDITOR_PREF_SAVE_LAST_SELECTION_CLOSED";

        private static bool IsEditorWindowLinked { get { return (_displayInside == null); } }

        static PeekLogic()
        {
            _isInit = false;
            EditorApplication.update += UpdateEditor;
            CallBackGlobalInputEditor.OnGlobalKeyEvent += CatchGlobalKeyEvents;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += ChangeSceneAfterPlayMode;
#endif
#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += KeepLinkAndNamesUpToDate;
#else
            EditorApplication.hierarchyWindowChanged += KeepLinkAndNamesUpToDate;
#endif
        }        

        [MenuItem("GameObject/Unity Essentials/Bookmark", false, 20)]
        [MenuItem("Assets/Unity Essentials/Bookmark")]
        private static void BookmarkSelections()
        {
            AddSelectionToBookMark();
        }

        [MenuItem("GameObject/Unity Essentials/Bookmark", true, 20)]
        [MenuItem("Assets/Unity Essentials/Bookmark", true)]
        private static bool BookmarkValidateSelections()
        {
            return (Selection.objects.Length > 0);
        }

        [PostProcessBuildAttribute(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            PeekLogic.UpdateSceneGameObjectOnLoad();
        }

        /// <summary>
        /// Useful to search a given scriptableObject
        /// Find the last object of the given type in the project
        /// (if this type has never been selected, search in project)
        /// usage:
        /// UnityEngine.Object found = null;
        /// PeekLogic.GetLastObjectOfThisTypeOrFindItInAsset<YourScriptType>(ref found, "Assets/");
        /// </summary>
        /// <typeparam name="T">type of asset to search</typeparam>
        /// <param name="found"></param>
        /// <param name="pathWhereToSearch"></param>
        /// <returns></returns>
        public static T GetLastObjectOfThisTypeOrFindItInAsset<T>(ref UnityEngine.Object found, string pathWhereToSearch = "Assets/") where T : UnityEngine.Object
        {
            T element;
            bool hasFound = PeekLogic.GetLastObjectOfThisType<T>(ref found);
            if (hasFound)
            {
                return (found as T);
            }
            else
            {
                element = ExtFindEditor.GetAssetByGenericType<T>(pathWhereToSearch);
                return (element);
            }
        }

        public static bool GetLastObjectOfThisType<T>(ref UnityEngine.Object found)
        {
            return (PeekSaveDatas.Instance.GetLastObjectOfThisType<T>(ref found));
        }

        /// <summary>
        /// try to find a given type. If not found, try to search in project,
        /// and select the [index] in the array of items found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="found"></param>
        /// <param name="pathWhereToSearch"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T GetLastObjectOfThisTypeOrFindItByIndexInAsset<T>(ref UnityEngine.Object found, string pathWhereToSearch = "Assets/", int index = 0) where T : UnityEngine.Object
        {
            bool hasFound = PeekLogic.GetLastObjectOfThisType<T>(ref found);
            if (hasFound)
            {
                return (found as T);
            }
            else
            {
                List<T> elements = ExtFindEditor.GetAssetsByGenericType<T>(pathWhereToSearch);
                if (elements.Count == 0)
                {
                    return (null);
                }
                if (index < 0)
                {
                    return (elements[0]);
                }
                if (index >= elements.Count)
                {
                    return (elements[elements.Count - 1]);
                }
                return (elements[index]);
            }
        }

        /// <summary>
        /// add a given Object into bookmark
        /// </summary>
        /// <param name="toSave"></param>
        /// <returns></returns>
        public static bool AddToBookMark(UnityEngine.Object toSave)
        {
            PeekSerializeObject.AddNewBookMark(toSave);
            PeekSerializeObject.RemoveSelectedWithoutDuplicate(toSave);
            PeekSerializeObject.Save();
            return (true);
        }

        public static bool AddSelectionToBookMark()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (Selection.objects[i] != null)
                {
                    PeekSerializeObject.AddNewBookMark(Selection.objects[i]);
                    PeekSerializeObject.RemoveSelectedWithoutDuplicate(Selection.objects[i]);
                    PeekSerializeObject.Save(1f);
                }
            }
            return (true);
        }

        /// <summary>
        /// called at first update
        /// </summary>
        public static void Init()
        {
            _isInit = true;
            _isClosed = IsEditorWindowLinked;
            _peekReloadSceneBookMark.Init();
            if (!WasOpenLastTimeWeCloseUnity())
            {
                UpdatePeekEditorWindowIfOpen();
            }
            UpdateSceneGameObjectOnLoad();
        }

        public static void UpdatePeekEditorWindowIfOpen()
        {
            bool hasUpdatedEditorWindow = false;

            if (Application.isPlaying)
            {
                UpdateSceneGameOnPlay();
            }
            else if (hasUpdatedEditorWindow)
            {
                UpdateSceneGameObjectOnLoad();
            }
        }

        public static void ManageOpenPeekEditorWindow()
        {
            ToggleOpenCloseDisplay();
            _isClosed = IsEditorWindowLinked;

            if (!_isClosed)
            {
                UpdateOnEditorWindowOpen();
            }
        }

        public static void ToggleOpenCloseDisplay()
        {
            if (_displayInside)
            {
                _displayInside.Close();
                _displayInside = null;
            }
            else
            {
                _displayInside = EditorWindow.GetWindow<PeekEditorWindow>(PeekEditorWindow.PEEK_WINDOW, true);
                _displayInside.Init();
                _displayInside.Show();
            }
            SaveCloseStatus();
        }

        public static void SaveCloseStatus()
        {
            EditorPrefs.SetBool(KEY_EDITOR_PREF_SAVE_LAST_SELECTION_CLOSED, _displayInside == null);
        }

        public static bool WasOpenLastTimeWeCloseUnity()
        {
            return (EditorPrefs.GetBool(KEY_EDITOR_PREF_SAVE_LAST_SELECTION_CLOSED));
        }

        public static void KeepLinkAndNamesUpToDate()
        {
            PeekSerializeObject.KeepLinkAndNamesUpToDate();
        }

        public static void DeleteBookMarkedGameObjectLinkedToScene()
        {
            PeekSerializeObject.DeleteBookMarkedGameObjectOfLostScene();
        }

        private static void UpdateEditor()
        {
            try
            {
                WaitUntilSceneIsValid();

                UnityEngine.Object currentSelectedObject = Selection.activeObject;

                if (currentSelectedObject != null
                    && currentSelectedObject != PeekSerializeObject.LastSelectedObject
                    && currentSelectedObject.GetType().ToString() != "Search.Pro.InspectorRecentSO")
                {
                    AttemptToRemoveNull();
                    if (PeekSerializeObject.AddNewSelection(currentSelectedObject))
                    {
                        PeekSerializeObject.Save();
                    }
                }

                if (!_isInit)
                {
                    Init();
                }
                if (_isClosed != IsEditorWindowLinked)
                {
                    SaveCloseStatus();
                    _isClosed = IsEditorWindowLinked;
                }
            }
            catch { }
        }

        private static void AttemptToRemoveNull()
        {
            if (PeekSerializeObject.SelectedObjectsList != null && PeekSerializeObject.SelectedObjectsList.IsThereNullCustomObjectInList())
            {
                PeekSerializeObject.AttemptToRemoveNull();
            }
        }

        public static void SaveToBookMark(UnityEngine.Object toSelect, int index)
        {
            PeekSerializeObject.AddNewBookMark(toSelect);
            PeekSerializeObject.RemoveSelectedWithoutDuplicateAt(index);
            PeekSerializeObject.Save();
        }

        public static void ForceSelection(UnityEngine.Object forcedSelection, bool select = true, bool showNameInSceneView = true, bool canDoubleClic = false)
        {
            bool doubleClic = PeekSerializeObject.LastSelectedObject == forcedSelection;
            PeekSerializeObject.ChangeLastSelectedObject(forcedSelection);
            PeekSerializeObject.Save(30);

            if (select)
            {
                Selection.activeObject = PeekSerializeObject.LastSelectedObject;
                EditorGUIUtility.PingObject(PeekSerializeObject.LastSelectedObject);
            }
            else
            {
                EditorGUIUtility.PingObject(PeekSerializeObject.LastSelectedObject);
            }
            if (showNameInSceneView)
            {
                //ShaderOutline.ShaderOutline.SelectionChangedByPreviousNext();
            }
            if (canDoubleClic && doubleClic)
            {
                SceneView.FrameLastActiveSceneView();
            }
        }

        public static void AddToIndex(int add)
        {
            PeekSerializeObject.AddToIndex(add);
            PeekSerializeObject.Save(30);
        }

        public static void UpdateOnEditorWindowOpen()
        {
            UpdateSceneGameObjectOnLoad();
        }

        public static void UpdateSceneGameOnPlay()
        {

        }

        public static void UpdateSceneGameObjectOnLoad()
        {
            int countScene = SceneManager.sceneCount;
            for (int i = 0; i < countScene; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                _sceneLoaded.AddIfNotContain(scene);
            }
            PeekSerializeObject.DeleteBookMarkedGameObjectOfLostScene();
        }

        public static void UpdateSceneGameObjectFromSceneLoad(Scene scene)
        {
            _sceneLoaded.AddIfNotContain(scene);
            //ShaderOutline.ShaderOutline.OnSceneLoad(scene);
        }

        private static void WaitUntilSceneIsValid()
        {
            bool mustReload = false;
            for (int i = _sceneLoaded.Count - 1; i >= 0; i--)
            {
                if (!_sceneLoaded[i].IsValid())
                {
                    mustReload = true;
                    _sceneLoaded.RemoveAt(i);
                    continue;
                }

                _peekReloadSceneBookMark.ReloadSceneFromOneLoadedScene(_sceneLoaded[i]);
                _sceneLoaded.RemoveAt(i);
            }

            if (mustReload)
            {
                UpdateSceneGameObjectOnLoad();
            }
        }

        private static void CatchGlobalKeyEvents(Event current)
        {
            if (current.type != EventType.KeyUp)
            {
                return;
            }
            ModifierKey modifier = ExtShortCutEditor.GetModifierKey(UnityEssentialsPreferences.SHORTCUT_MODIFIER_KEY_FOR_PREVIOUS_NEXT_SELECTION, (int)UnityEssentialsPreferences.DEFAULT_MODIFIER_KEY);
            KeyCode previousKey = ExtShortCutEditor.GetKey(UnityEssentialsPreferences.SHORTCUT_KEY_FOR_PREVIOUS_SELECTION, (int)UnityEssentialsPreferences.DEFAULT_PREVIOUS_KEY);
            KeyCode nextKey = ExtShortCutEditor.GetKey(UnityEssentialsPreferences.SHORTCUT_KEY_FOR_NEXT_SELECTION, (int)UnityEssentialsPreferences.DEFAULT_NEXT_KEY);

            bool shortCutPrevious = ExtShortCutEditor.IsModifierEnumMatch(Event.current, modifier) && current.keyCode == previousKey;
            bool shortCutNext = ExtShortCutEditor.IsModifierEnumMatch(Event.current, modifier) && current.keyCode == nextKey;
            if (shortCutPrevious)
            {
                if (Selection.objects.Length != 0)
                {
                    AddToIndex(-1);
                }
                ForceSelection(PeekSerializeObject.SelectedObjectsIndex(PeekSerializeObject.CurrentIndex));
            }
            else if (shortCutNext)
            {
                AddToIndex(1);
                ForceSelection(PeekSerializeObject.SelectedObjectsIndex(PeekSerializeObject.CurrentIndex));
            }
        }

        

        public static void SwapSceneAndSelectItem(string scenePath, string toSelectName, string sceneName)
        {
            _scenePathOfGameObjectToSelect = scenePath;
            _toSelectName = toSelectName;
            _sceneOfGameObjectToSelectName = sceneName;
            _needToChangeScene = false;

            if (Application.isPlaying)
            {
                if (!ExtGUILayout.DrawDisplayDialog(
                    "Change scene",
                    "Do you want to Quit play mode and swich to scene " + sceneName + "?"))
                {
                    return;
                }
                UnityEditor.EditorApplication.isPlaying = false;
                _needToChangeScene = true;
                return;
            }
            else if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }
            LoadSceneFromDatas(_scenePathOfGameObjectToSelect, _toSelectName, _sceneOfGameObjectToSelectName);
        }

        private static void LoadSceneFromDatas(string scenePath, string toSelectName, string sceneName)
        {
            EditorUtility.DisplayProgressBar("Loading scene", "", 0f);
            _peekReloadSceneBookMark.AutomaticlySelectWhenFound(toSelectName, sceneName);
            EditorSceneManager.OpenScene(scenePath);
            EditorUtility.ClearProgressBar();
        }

#if UNITY_2017_2_OR_NEWER
        public static void ChangeSceneAfterPlayMode(PlayModeStateChange state)
        {
            if (!_needToChangeScene)
            {
                return;
            }
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                LoadSceneFromDatas(_scenePathOfGameObjectToSelect, _toSelectName, _sceneOfGameObjectToSelectName);
                _needToChangeScene = false;
            }
        }
#endif
    }
}