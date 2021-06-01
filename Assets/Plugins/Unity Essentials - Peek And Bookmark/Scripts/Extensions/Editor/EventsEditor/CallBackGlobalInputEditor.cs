using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Peek.Extensions.Editor.EventEditor
{
    public class CallBackGlobalInputEditor
    {
        public delegate void EventOnGlobalKeyPress(Event current);
        public static event EventOnGlobalKeyPress OnGlobalKeyEvent;
        public static event EventOnGlobalKeyPress OnGlobalKeyUp;
        public static event EventOnGlobalKeyPress OnGlobalKeyPressUpDoubleClick;

        public delegate void EventOnGlobalKeyPressOverSceneView(Event current, SceneView sceneView);
        public static event EventOnGlobalKeyPressOverSceneView OnGlobalKeyEventOverSceneView;
        public static event EventOnGlobalKeyPressOverSceneView OnGlobalKeyPressUpOverSceneView;
        public static event EventOnGlobalKeyPressOverSceneView OnGlobalKeyPressUpDoubleClickOverSceneView;

        public delegate void EventOnGlobalNKeyPress(Event current, int count);
        public static event EventOnGlobalNKeyPress OnGlobalNKeyPressUp;

        public delegate void EventOnGlobalNKeyPressOverSceneView(Event current, SceneView sceneView, int count);
        public static event EventOnGlobalNKeyPressOverSceneView OnGlobalNKeyPressUpOverSceneView;


        private static float _timingDoubleClick = 0.3f;
        private static EditorChrono _timerDoubleClick = new EditorChrono();
        private static KeyCode _lastKeyCode;
        private static int _clickCount = 0;

        [InitializeOnLoadMethod]
        private static void EditorInit()
        {
            System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)info.GetValue(null);
            value += EditorGlobalKeyPress;
            info.SetValue(null, value);
        }


        private static void EditorGlobalKeyPress()
        {
            //here basic key event (can be keyDown, up, held...)
            OnGlobalKeyEvent?.Invoke(Event.current);
            if (EditorWindow.mouseOverWindow != null && EditorWindow.mouseOverWindow.GetType() == typeof(SceneView))
            {
                OnGlobalKeyEventOverSceneView?.Invoke(Event.current, (SceneView)EditorWindow.mouseOverWindow);
            }

            //fire special event on key up
            if (Event.current.type == EventType.KeyUp)
            {
                _clickCount++;
                if (_timerDoubleClick.IsRunning() && _lastKeyCode == Event.current.keyCode)
                {
                    //2 key press
                    if (_clickCount == 2)
                    {
                        OnGlobalKeyPressUpDoubleClick?.Invoke(Event.current);
                        if (EditorWindow.mouseOverWindow != null && EditorWindow.mouseOverWindow.GetType() == typeof(SceneView))
                        {
                            OnGlobalKeyPressUpDoubleClickOverSceneView?.Invoke(Event.current, (SceneView)EditorWindow.mouseOverWindow);
                        }
                    }
                }
                else
                {
                    //first key press
                    _clickCount = 1;
                    OnGlobalKeyUp?.Invoke(Event.current);
                    if (EditorWindow.mouseOverWindow != null && EditorWindow.mouseOverWindow.GetType() == typeof(SceneView))
                    {
                        OnGlobalKeyPressUpOverSceneView?.Invoke(Event.current, (SceneView)EditorWindow.mouseOverWindow);
                    }
                }
                //N click
                OnGlobalNKeyPressUp?.Invoke(Event.current, _clickCount);
                if (EditorWindow.mouseOverWindow != null && EditorWindow.mouseOverWindow.GetType() == typeof(SceneView))
                {
                    OnGlobalNKeyPressUpOverSceneView?.Invoke(Event.current, (SceneView)EditorWindow.mouseOverWindow, _clickCount);
                }

                _timerDoubleClick.StartChrono(_timingDoubleClick);
                _lastKeyCode = Event.current.keyCode;
            }
        }
    }
}