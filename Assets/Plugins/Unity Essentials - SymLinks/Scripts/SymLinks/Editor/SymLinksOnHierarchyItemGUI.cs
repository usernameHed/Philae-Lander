using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEssentials.symbolicLinks.extensions;
using UnityEssentials.symbolicLinks.extensions.editor;
using UnityEssentials.symbolicLinks.options;

namespace UnityEssentials.symbolicLinks
{
    /// <summary>
    /// Determine the marker display on HierarchyWindow items
    /// </summary>
    [InitializeOnLoad]
    public static class SymLinksOnHierarchyItemGUI
    {
        private const string SYMLINK_TOOLTIP = "In Symlink: ";
        private const string SYMLINK_PARENT_TOOLTIP = "parent of symlink gameObjects";
        private const int MAX_SETUP_IN_ONE_FRAME = 10;
        private const float TIME_BETWEEN_2_CHUNK_SETUP = 0.1f;

        //Don't use dictionnary ! it crash !!
        private static List<int> _gameObjectsId = new List<int>(300);
        private static List<UnityEngine.Object> _gameObjectsList = new List<UnityEngine.Object>(300);
        private static List<int> _parentListId = new List<int>(300);
        private static List<bool> _gameObjectsHasBeenSetup = new List<bool>(300);
        private static List<bool> _gameObjectsIsInFrameWork = new List<bool>(300);
        private static List<string> _toolTipInfo = new List<string>(300);
        private static List<string> _pathSymLinksObjects = new List<string>(300);
        private static bool _needToSetup = false;
        private static int _settupedGameObject = 0;
        private static EditorChrono _timerBetween2ChunkSetup = new EditorChrono();


        /// <summary>
        /// Static constructor subscribes to projectWindowItemOnGUI delegate.
        /// </summary>
        static SymLinksOnHierarchyItemGUI()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;

#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged -= OnHierarchyWindowChanged;
            EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
#else
            EditorApplication.hierarchyWindowChanged -= OnHierarchyWindowChanged;
            EditorApplication.hierarchyWindowChanged += OnHierarchyWindowChanged;
#endif

            _needToSetup = false;
            _settupedGameObject = 0;
        }

        public static void ResetSymLinksDatas()
        {
            _gameObjectsId.Clear();
            _gameObjectsList.Clear();
            _toolTipInfo.Clear();
            _gameObjectsHasBeenSetup.Clear();
            _parentListId.Clear();
            _gameObjectsIsInFrameWork.Clear();
            _pathSymLinksObjects.Clear();
            _settupedGameObject = 0;
        }

        /// <summary>
        /// when script reload, some variables need to be resetup
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            _needToSetup = true;
        }
        /// <summary>
        /// when hierarchy change, calculation need to be done again
        /// </summary>
        private static void OnHierarchyWindowChanged()
        {
            _needToSetup = true;
        }

        /// <summary>
        /// called at every frameGUI for every gaemObjects inside hierarchy.
        /// The complexe list & index stuff append because we have to be cost effective.
        /// </summary>
        /// <param name="instanceId">id of the gameObject</param>
        /// <param name="selectionRect">rect of display</param>
        private static void OnHierarchyItemGUI(int instanceId, Rect selectionRect)
        {
            try
            {
                if (!EditorPrefs.GetBool(UnityEssentialsPreferences.SHOW_INDICATOR_ON_HIERARCHY_WINDOW, true))
                {
                    return;
                }

                //the needToSetup append once. (not for every gameObject...)
                if (_needToSetup)
                {
                    ResetHasBeenSetupOfGameObjects();
                    _needToSetup = false;
                }

                //if this gameObejct has not been setup, setup it !
                int index = 0;
                bool isAlreadySetupped = _gameObjectsId.ContainIndex(instanceId, ref index);
                if (!isAlreadySetupped)
                {
                    UnityEngine.Object targetGameObject = EditorUtility.InstanceIDToObject(instanceId);
                    if (targetGameObject == null)
                    {
                        return;
                    }
                    _gameObjectsId.Add(instanceId);
                    _gameObjectsList.Add(targetGameObject);
                    _toolTipInfo.Add(SYMLINK_TOOLTIP);
                    _gameObjectsHasBeenSetup.Add(false);
                    _gameObjectsIsInFrameWork.Add(false);
                    _pathSymLinksObjects.Add("");
                    index = _gameObjectsId.Count - 1;
                    SetupGameObject(index);
                }
                else if (!_gameObjectsHasBeenSetup[index])
                {
                    SetupGameObject(index);
                }

                //finally, if gameObject is known to be in symlink folder, display mark.
                if (_gameObjectsIsInFrameWork[index])
                {
                    DisplayMarker(selectionRect, _toolTipInfo[index], UnityEssentialsPreferences.GetDefaultColor());
                }
                else if (EditorPrefs.GetBool(UnityEssentialsPreferences.SHOW_DOT_ON_PARENTS, true) && _parentListId.Contains(instanceId))
                {
                    DisplayPointMarker(selectionRect, SYMLINK_PARENT_TOOLTIP, UnityEssentialsPreferences.GetDefaultColor());
                }
            }
            catch { }
        }

        private static void ResetHasBeenSetupOfGameObjects()
        {
            _settupedGameObject = 0;
            for (int i = 0; i < _gameObjectsHasBeenSetup.Count; i++)
            {
                _gameObjectsHasBeenSetup[i] = false;
                _toolTipInfo[i] = SYMLINK_TOOLTIP;
            }
            _parentListId.Clear();
        }

        /// <summary>
        /// From an instance id, setup the info:
        /// HasBennSetup at true
        /// determine if gameObject is inside framework or not !
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="info"></param>
        private static void SetupGameObject(int index)
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (_settupedGameObject > MAX_SETUP_IN_ONE_FRAME)
            {
                _timerBetween2ChunkSetup.StartChrono(TIME_BETWEEN_2_CHUNK_SETUP);
                _settupedGameObject = 0;
                return;
            }
            if (_timerBetween2ChunkSetup.IsRunning())
            {
                return;
            }

            string toolTip = _toolTipInfo[index];
            string path = "";
            GameObject gameObjectRef = _gameObjectsList[index] as GameObject;
            bool prefab = DetermineIfGameObjectIsInSymLink.IsPrefabsAndInSymLink(gameObjectRef, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved, ref toolTip, ref path);
            bool component = DetermineIfGameObjectIsInSymLink.HasComponentInSymLink(gameObjectRef, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved, ref toolTip, ref path);
            bool assets = DetermineIfGameObjectIsInSymLink.HasSymLinkAssetInsideComponent(gameObjectRef, ref SymLinksOnProjectWindowItemGUI.AllSymLinksAssetPathSaved, ref toolTip, ref path);
            bool isSomethingInsideFrameWork = prefab || component || assets;

            _pathSymLinksObjects[index] = path;
            _toolTipInfo[index] = toolTip;
            _gameObjectsHasBeenSetup[index] = true;
            _gameObjectsIsInFrameWork[index] = isSomethingInsideFrameWork;

            if (isSomethingInsideFrameWork && gameObjectRef != null)
            {
                Transform parentRef = gameObjectRef.transform.parent;
                while (parentRef != null)
                {
                    _parentListId.Add(parentRef.gameObject.GetInstanceID());
                    parentRef = parentRef.parent;
                }
            }

            _settupedGameObject++;
        }

        /// <summary>
        /// display the marker at the given position
        /// </summary>
        /// <param name="selectionRect"></param>
        private static void DisplayMarker(Rect selectionRect, string toolTip, Color color)
        {
            Rect r = new Rect(selectionRect);
            r.x = r.width - 20;
            r.width = 18;
            ExtSymLinks.DisplayTinyMarker(selectionRect, toolTip, color);
        }

        /// <summary>
        /// display the marker at the given position
        /// </summary>
        /// <param name="selectionRect"></param>
        private static void DisplayPointMarker(Rect selectionRect, string toolTip, Color color)
        {
            Rect r = new Rect(selectionRect);
            r.x = r.width - 20;
            r.width = 18;
            ExtSymLinks.DisplayPointMarker(selectionRect, toolTip, color);
        }
    }
}
