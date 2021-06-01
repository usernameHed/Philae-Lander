using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor.EventEditor
{
    [InitializeOnLoad]
    public static class CallBackDuplicate
    {
        public delegate void EventOnDuplicate(GameObject duplicatedGameObject, GameObject source);
        public static event EventOnDuplicate OnDuplicate;
        public delegate void EventOnDuplicates(GameObject[] duplicatedGameObject, GameObject[] sources);
        public static event EventOnDuplicates OnDuplicates;

        private const string DUPLICATE_COMMAND = "Duplicate";
        private const string PAST_COMMAND = "Paste";
        private static bool _duplicateOrPasteExecuted = false;
        private static GameObject[] _sources;

        static CallBackDuplicate()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += DurringSceneGUI;
#else
            SceneView.onSceneGUIDelegate += DurringSceneGUI;
#endif
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            if (_duplicateOrPasteExecuted)
            {
                for (int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    GameObject source = FindSourceFromDuplicate(Selection.gameObjects[i], _sources);
                    OnDuplicate?.Invoke(Selection.gameObjects[i], source);
                }
                OnDuplicates?.Invoke(Selection.gameObjects, FindSourcesFromDuplicates(Selection.gameObjects, _sources));
                _duplicateOrPasteExecuted = false;
            }
        }

        private static GameObject FindSourceFromDuplicate(GameObject duplicate, GameObject[] sources)
        {
            for (int i = 0; i < sources.Length; i++)
            {
                if (duplicate.name.Contains(sources[i].name))
                {
                    return (sources[i]);
                }
            }
            if (sources.Length > 0)
            {
                return (sources[0]);
            }
            return (null);
        }

        private static GameObject[] FindSourcesFromDuplicates(GameObject[] duplicate, GameObject[] sources)
        {
            GameObject[] newSources = new GameObject[duplicate.Length];
            for (int i = 0; i < duplicate.Length; i++)
            {
                newSources[i] = FindSourceFromDuplicate(duplicate[i], sources);
            }
            return (newSources);
        }



        private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
        {
            if (_duplicateOrPasteExecuted)
            {
                return;
            }
            AttemptToValidateCallBack(Event.current);
        }

        private static void AttemptToValidateCallBack(Event current)
        {
            bool isCommandEvent = /*current.type == EventType.ValidateCommand
                || */current.type == EventType.ExecuteCommand;
            bool isValidCommandName = current.commandName == DUPLICATE_COMMAND || current.commandName == PAST_COMMAND;

            if (isCommandEvent && isValidCommandName)
            {
                _duplicateOrPasteExecuted = true;
                _sources = Selection.gameObjects;
            }
        }

        private static void DurringSceneGUI(SceneView sceneView)
        {
            if (_duplicateOrPasteExecuted)
            {
                return;
            }
            AttemptToValidateCallBack(Event.current);
        }
    }
}