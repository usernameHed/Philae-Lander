using System;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Peek.Extensions;
using UnityEssentials.Peek.Extensions.Editor;
using UnityEssentials.Peek.Extensions.Editor.editorWindow;
using UnityEssentials.Peek.gui;
using UnityEssentials.Peek.Options;
using UnityEssentials.Peek.ToolbarExtender;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.Peek
{
    public class PeekEditorWindow : EditorWindow
#if UNITY_2018_1_OR_NEWER
        , IHasCustomMenu
#endif
    {
        public const int MIN_WIDTH = 200;
        private const int MIN_HEIGHT = 200;

        public const string PEEK_WINDOW = "Peek window";
        public const string OPEN_PREFERENCE_SETTINGS_TEXT = "Settings";
        public const string PEEK_ICON_WINDOW = "UnityLogo";
        private const float SIZE_SCROLL_BAR = 9f;
        private const float HEIGHT_LINE = 17f;

        private const string PIN_ICON = "FolderFavorite Icon";
        private const string PIN_TOOLTIP = "Save in bookmark";

        private const string UNPIN_ICON = "Favorite Icon";
        private const string UNPIN_ICON_HOVER = "Toolbar Minus";
        private const string UNPIN_TOOLTIP = "Remove from bookmark";

        private const string BOOKMARK_GAMEOBJECT_LOGO = "UnityLogo";
        private const string BOOKMARK_OBJECT_LOGO = "Project";
        private const string SELECTED_OBJECT_LOGO = "";

        private const string FOLD_STATE_BOOKMARK_GAMEOBJECT = "FOLD_STATE_BOOKMARK_GAMEOBJECT";
        private const string FOLD_STATE_BOOKMARK_OBJECT = "FOLD_STATE_BOOKMARK_OBJECT";
        private const string FOLD_STATE_SELECTED_OBJECT = "FOLD_STATE_SELECTED_OBJECT";

        [SerializeField] private GenericPeekListDrawer _pinnedGameObjectDrawer;
        [SerializeField] private GenericPeekListDrawer _pinnedObjectDrawer;
        [SerializeField] private GenericPeekListDrawer _selectedObjectDrawer;

        private Vector2 _scrollPosition;

        public void Init()
        {
            GUIContent title = new GUIContent(EditorGUIUtility.IconContent(PeekToolbarDrawer.OPEN_EDITOR_WINDOW_ICON));
            title.text = PEEK_WINDOW;
            this.titleContent = title;
            this.minSize = minSize;


            InitDrawer();
        }

        private void InitDrawer()
        {
            _pinnedGameObjectDrawer = new GenericPeekListDrawer(
                this,
                iconList: BOOKMARK_GAMEOBJECT_LOGO,
                listDescription: "BookMark in scenes",
                calculateExtent: false,
                foldStateKey: FOLD_STATE_BOOKMARK_GAMEOBJECT,
                canReorder: true,
                list: PeekSerializeObject.PinnedObjectsInScenesList,
                listPath: PeekSerializeObject.PinnedObjectsNameInScenesList,
                sceneLinked: PeekSerializeObject.PinnedObjectsScenesLinkList,
                iconBookMark: UNPIN_ICON,
                iconHoverBookMark: UNPIN_ICON_HOVER,
                toolTipBookMark: UNPIN_TOOLTIP,
                height: HEIGHT_LINE,
                onBookMarkClic: OnPinnedSceneGameObjectBookMarkClicked,
                onSelectItem: OnPinnedSceneGameObjectClicked,
                onInfoItem: OnPinnedSceneGameObjectInfo,
                onInfoForceItem: OnPinnedSceneGameObjectInfoOpenScene,
                onPinItem: OnPinnedSceneGameObjectPinned,
                onRemoveItem: OnPinnedSceneGameObjectRemove,
                onClear: OnPinnedSceneGameObjectListClear);

            _pinnedObjectDrawer = new GenericPeekListDrawer(
                this,
                iconList: BOOKMARK_OBJECT_LOGO,
                listDescription: "BookMark in project",
                calculateExtent: false,
                foldStateKey: FOLD_STATE_BOOKMARK_OBJECT,
                canReorder: true,
                list: PeekSerializeObject.PinnedObjectsList,
                listPath: null,
                sceneLinked: null,
                iconBookMark: UNPIN_ICON,
                iconHoverBookMark: UNPIN_ICON_HOVER,
                toolTipBookMark: UNPIN_TOOLTIP,
                height: HEIGHT_LINE,
                onBookMarkClic: OnPinnedObjectBookMarkClicked,
                onSelectItem: OnPinnedObjectClicked,
                onInfoItem: null,
                onInfoForceItem: null,
                onPinItem: OnPinnedObjectPinned,
                onRemoveItem: OnPinnedObjectRemove,
                onClear: OnPinnedObjectListClear);

            _selectedObjectDrawer = new GenericPeekListDrawer(
                this,
                iconList: SELECTED_OBJECT_LOGO,
                listDescription: "previously selected:",
                calculateExtent: true,
                foldStateKey: FOLD_STATE_SELECTED_OBJECT,
                canReorder: true,
                list: PeekSerializeObject.SelectedObjectsWithoutDuplicateList,
                listPath: null,
                sceneLinked: null,
                iconBookMark: PIN_ICON,
                iconHoverBookMark: UNPIN_ICON,
                toolTipBookMark: PIN_TOOLTIP,
                height: HEIGHT_LINE,
                onBookMarkClic: OnSelectedObjectBookMarkClicked,
                onSelectItem: OnSelectedObjectClicked,
                onInfoItem: null,
                onInfoForceItem: null,
                onPinItem: OnSelectedObjectPinned,
                onRemoveItem: OnSelectedObjectRemove,
                onClear: OnSelectedObjectListClear);
        }
          

        #region drawer
        private void OnGUI()
        {
            DrawPeekWindow();
        }

        /// <summary>
        /// display content of peek window
        /// </summary>
        public void DrawPeekWindow()
        {
            if (_pinnedGameObjectDrawer == null)
            {
                InitDrawer();
            }

            float widthEditorWindow = position.width;
            float widthWithoutScrollBar = widthEditorWindow - SIZE_SCROLL_BAR;

            using (VerticalScope verticalScope = new VerticalScope())
            {
                _scrollPosition = GUILayout.BeginScrollView(
                    _scrollPosition,
                    false,
                    false,
                    GUIStyle.none,
                    GUI.skin.verticalScrollbar,
                    GUILayout.Width(widthEditorWindow));

                _pinnedGameObjectDrawer.Display();
                GUILayout.Label("");

                _pinnedObjectDrawer.Display();
                GUILayout.Label("");

                _selectedObjectDrawer.Display();

                GUILayout.Label("", GUILayout.Width(widthWithoutScrollBar));
                GUI.EndScrollView();
            }
        }

        private void SelectItem(UnityEngine.Object toSelect, bool select = true)
        {
            PeekLogic.ForceSelection(toSelect, select: select, showNameInSceneView: true, canDoubleClic: true);
        }

        #endregion

        #region PinnedSceneGameObject functions
        private void OnPinnedSceneGameObjectBookMarkClicked(int index)
        {
            PeekSerializeObject.MoveToBookMark(index);
        }
        private void OnPinnedSceneGameObjectClicked(int index)
        {
            SelectItem(PeekSerializeObject.PinnedObjectsInScenesIndex(index));
        }
        private void OnPinnedSceneGameObjectInfo(int index)
        {
            SelectItem(PeekSerializeObject.PinnedObjectsScenesLinkIndex(index));
        }
        private void OnPinnedSceneGameObjectInfoOpenScene(int index)
        {
            PeekLogic.SwapSceneAndSelectItem(PeekSerializeObject.PinnedObjectsScenesLinkIndex(index).GetPath(), PeekSerializeObject.PinnedObjectsNameInScenesIndex(index), PeekSerializeObject.PinnedObjectsScenesLinkIndex(index).name);
        }

        private void OnPinnedSceneGameObjectPinned(int index)
        {
            SelectItem(PeekSerializeObject.PinnedObjectsInScenesIndex(index), false);
        }
        private void OnPinnedSceneGameObjectRemove(int index)
        {
            PeekSerializeObject.RemoveBookMarkedGameObjectItem(index);
        }
        private void OnPinnedSceneGameObjectListClear()
        {
            PeekSerializeObject.ClearBookMarkGameObjects();
        }
        #endregion

        #region PinnedObject functions
        private void OnPinnedObjectBookMarkClicked(int index)
        {
            PeekSerializeObject.RemoveAssetFromBookMark(index);
        }
        private void OnPinnedObjectClicked(int index)
        {
            SelectItem(PeekSerializeObject.PinnedObjectsIndex(index));
        }
        private void OnPinnedObjectPinned(int index)
        {
            SelectItem(PeekSerializeObject.PinnedObjectsIndex(index), false);
        }
        private void OnPinnedObjectRemove(int index)
        {
            PeekSerializeObject.RemoveBookMarkedItem(index);
        }
        private void OnPinnedObjectListClear()
        {
            PeekSerializeObject.ClearBookMarkAsset();
        }
        #endregion

        #region SelectedObject functions
        private void OnSelectedObjectBookMarkClicked(int index)
        {
            PeekLogic.SaveToBookMark(PeekSerializeObject.SelectedObjectsWithoutDuplicateIndex(index), index);
        }

        private void OnSelectedObjectClicked(int index)
        {
            SelectItem(PeekSerializeObject.SelectedObjectsWithoutDuplicateIndex(index));
        }
        private void OnSelectedObjectPinned(int index)
        {
            SelectItem(PeekSerializeObject.SelectedObjectsWithoutDuplicateIndex(index), false);
        }
        private void OnSelectedObjectRemove(int index)
        {
            PeekSerializeObject.RemoveItemInSelection(index);
        }
        private void OnSelectedObjectListClear()
        {
            PeekSerializeObject.ClearSelectedList();
        }
        #endregion


        private void OnSelectionChange()
        {
            Repaint();
        }

#if UNITY_2018_1_OR_NEWER
        /// <summary>
        /// This interface implementation is automatically called by Unity.
        /// </summary>
        /// <param name="menu"></param>
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            GUIContent content = new GUIContent(OPEN_PREFERENCE_SETTINGS_TEXT);
            menu.AddItem(content, false, OpenPreferenceSettings);
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