using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor.EventEditor
{
    [InitializeOnLoad]
    public static class CallBackIsMovingHandle
    {
        public delegate void EventIsMovingGameObject(GameObject current);
        public static event EventIsMovingGameObject OnStart;
        public static event EventIsMovingGameObject OnDrag;
        public static event EventIsMovingGameObject OnEnd;

        private static GameObject _currentSelection;
        private static bool _isDragging = false;
        private static bool _hasStarted = false;
        public static bool IsDragging(GameObject current)
        {
            return (_currentSelection == current && _isDragging);
        }
        public static bool IsDraggingAny()
        {
            return (_isDragging);
        }

        static CallBackIsMovingHandle()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += DurringSceneGUI;
#else
            SceneView.onSceneGUIDelegate += DurringSceneGUI;
#endif
        }

        private static void DurringSceneGUI(SceneView sceneView)
        {
            if (Selection.objects.Length == 0
                || Selection.objects.Length > 1
                || Selection.activeGameObject == null)
            {
                _currentSelection = null;
                _isDragging = false;
                return;
            }

            //first time gameObject selected
            if (_currentSelection == null)
            {
                if (_hasStarted || _isDragging)
                {

                }
                _currentSelection = Selection.activeGameObject;
                _isDragging = false;
                _hasStarted = false;
                return;
            }

            if (Tools.current != Tool.Move && Tools.current != Tool.Transform)
            {
                _isDragging = false;
                _hasStarted = false;
                return;
            }

            int controlID = HandleUtility.nearestControl;
            HandleUtility.AddDefaultControl(controlID);

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDrag:
                    //first drag has to be same control as the nearestControl!
                    //other drag don't, if the first one where good, other will be good too
                    if (!_isDragging && GUIUtility.hotControl != controlID)
                    {
                        break;
                    }

                    if (!_hasStarted)
                    {
                        TriggerStart();
                    }

                    break;
            }
            bool hasPressEscape = Event.current.keyCode == KeyCode.Escape && Event.current.rawType == EventType.KeyDown;

            if (hasPressEscape && (_hasStarted || _isDragging))
            {
                TriggerEnd();
            }
            if (_hasStarted && Event.current.rawType == EventType.MouseDrag && Event.current.button == 0)
            {
                TriggerDrag();
            }
            else if (_isDragging && Event.current.rawType == EventType.MouseUp && Event.current.button == 0)
            {
                TriggerEnd();
            }
        }
        private static void TriggerStart()
        {
            OnStart?.Invoke(_currentSelection);
            _hasStarted = true;
            _isDragging = true;
        }

        private static void TriggerDrag()
        {
            OnDrag?.Invoke(_currentSelection);
            _isDragging = true;
        }

        private static void TriggerEnd()
        {
            OnEnd?.Invoke(_currentSelection);
            _isDragging = false;
            _hasStarted = false;
        }
    }
}