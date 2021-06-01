using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor.EventEditor
{
    [InitializeOnLoad]
    public static class CallBackClickEditor
    {
        public delegate void EventGameObjectHierarchyClick(GameObject current);
        public static event EventGameObjectHierarchyClick OnGameObjectHierarchySimpleClick;
        public static event EventGameObjectHierarchyClick OnGameObjectHierarchyDoubleClick;

        public delegate void EventGameObjectHierarchyClickCount(GameObject current, int count);
        public static event EventGameObjectHierarchyClickCount OnGameObjectHierarchyNClick;

        private static float _timingDoubleClick = 0.3f;
        private static EditorChrono _timerDoubleClick = new EditorChrono();
        private static GameObject _lastClicked;
        private static int _clickCount = 0;

        static CallBackClickEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyGUI;
        }

        private static void HierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null)
            {
                return;
            }
            if (Event.current.button == 0 && Event.current.type == EventType.MouseUp && selectionRect.Contains(Event.current.mousePosition))
            {
                _clickCount++;
                if (_timerDoubleClick.IsRunning() && _lastClicked == gameObject)
                {
                    //2 clic
                    if (_clickCount == 2)
                    {
                        //Debug.Log("trigger double click");
                        OnGameObjectHierarchyDoubleClick?.Invoke(gameObject);
                    }
                }
                else
                {
                    //first click
                    _clickCount = 1;
                    //Debug.Log("trigger simple click");
                    OnGameObjectHierarchySimpleClick?.Invoke(gameObject);
                }
                //N click
                //Debug.Log("trigger n click: " + _clickCount);
                OnGameObjectHierarchyNClick?.Invoke(gameObject, _clickCount);

                _timerDoubleClick.StartChrono(_timingDoubleClick);
                _lastClicked = gameObject;
            }
        }
    }
}