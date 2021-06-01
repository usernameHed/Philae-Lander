using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape.editor
{
    public class TinyEditorWindowSceneView
    {
        public struct SaveEditorPref
        {
            public Rect Position;
            public bool Minimised;
        }

        private SaveEditorPref _saveEditorPreference = new SaveEditorPref();

        public string GetJsonOfSave()
        {
            _saveEditorPreference.Position = _windowMenu;
            _saveEditorPreference.Minimised = _isMinimised;
            string json = JsonUtility.ToJson(_saveEditorPreference);
            return (json);
        }

        public SaveEditorPref GetFromJsonDatasEditorPref(string json)
        {
            _saveEditorPreference = JsonUtility.FromJson<SaveEditorPref>(json);
            return (_saveEditorPreference);
        }

        private Rect _windowMenu;

        public delegate void ContentToDrawDelegate();
        private ContentToDrawDelegate _methodToCall;
        private Event _currentEvent;
        private float _saveHeight = 0;
        private bool _isMinimised = false;

        private string _keyEditorPref = "";
        public string NameEditorWindow = "";
        private int _id;

        private bool _draggable = false;
        public bool IsMinimisable = true;
        private Vector2 _lastPositionRight;
        private bool _snapRight;
        private bool _snapUp;
        public bool IsClosable = false;
        public EditorWindow _currentEditorWindow;
        private bool _lockToVisibleSizeX;    //if true, lock to the visible screen position, else, lock to the max size of the window
        private bool _lockToVisibleSizeY;    //if true, lock to the visible screen position, else, lock to the max size of the window

        private Rect _oldRect;
        private Vector2 _maxRect;  //determine the max value of size
        public bool IsClosed = false;

        public enum DEFAULT_POSITION
        {
            NONE = 0,

            UP_LEFT = 10,
            UP = 11,
            UP_RIGHT = 12,

            MIDDLE_LEFT = 20,
            MIDDLE = 21,
            MIDDLE_RIGHT = 22,

            DOWN_LEFT = 30,
            DOWN = 31,
            DOWN_RIGHT = 32,
        }

        /// <summary>
        /// 
        /// type:       UP_LEFT         UP          UP_RIGHT
        /// anchor:     (up left)       (up left)   (up right)
        /// 
        /// type:       MIDDLE_LEFT     MIDDLE      MIDDLE_RIGHT
        /// anchor:     (up left)       (up left)   (up right)
        /// 
        /// 
        /// type:       DOWN_LEFT       DOWN        DOWN_RIGHT
        /// anchor:     (down left)     (down left) (down right)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="snapUp"></param>
        /// <param name="snapRight"></param>
        /// <returns></returns>
        public Rect GetNiceRectFromEnum(DEFAULT_POSITION position, float defaultWidth, out bool snapUp, out bool snapRight)
        {
            Rect rectNav = new Rect(0, 0, 0, 0);
            snapUp = true;
            snapRight = false;

            float height = 50;

            switch (position)
            {
                case DEFAULT_POSITION.UP_LEFT:
                    {
                        snapUp = true;
                        snapRight = false;

                        float x = 10;
                        float y = 10;
                        rectNav = new Rect(x, y, defaultWidth, height);
                        return (rectNav);
                    }
                case DEFAULT_POSITION.UP:
                    {
                        snapUp = true;
                        snapRight = false;

                        float x = (EditorGUIUtility.currentViewWidth / 2) - (defaultWidth / 2);
                        float y = 10;
                        rectNav = new Rect(x, y, defaultWidth, height);
                        return (rectNav);
                    }

                case DEFAULT_POSITION.UP_RIGHT:
                    {
                        snapUp = true;
                        snapRight = true;

                        float marginFromRight = 100f;
                        float x = EditorGUIUtility.currentViewWidth - defaultWidth - marginFromRight;
                        float y = 10;
                        rectNav = new Rect(x, y, defaultWidth, height);
                        return (rectNav);
                    }

                case DEFAULT_POSITION.MIDDLE_LEFT:
                    {
                        snapUp = true;
                        snapRight = false;

                        float x = 10f;
                        float y = (_currentEditorWindow.position.height / 2) - (height / 2);
                        rectNav = new Rect(x, y, defaultWidth, height);
                        return (rectNav);
                    }

                case DEFAULT_POSITION.MIDDLE_RIGHT:
                    {
                        snapUp = true;
                        snapRight = true;

                        float marginFromRight = 10f;
                        float x = EditorGUIUtility.currentViewWidth - defaultWidth - marginFromRight;
                        float y = (_currentEditorWindow.position.height / 2) - (height / 2);
                        rectNav = new Rect(x, y, defaultWidth, height);
                        return (rectNav);
                    }


                case DEFAULT_POSITION.DOWN_LEFT:
                    {
                        snapUp = false;
                        snapRight = false;

                        float marginFromDown = 10f;
                        float x = 10;
                        float y = _currentEditorWindow.position.height - height - marginFromDown;
                        rectNav = new Rect(x, y, defaultWidth, height);
                        return (rectNav);
                    }
                case DEFAULT_POSITION.DOWN:
                    {
                        float yFromDown = 10;  //from bottom, go upper
                        float x = (EditorGUIUtility.currentViewWidth / 2) - (defaultWidth / 2);
                        float y = SceneView.currentDrawingSceneView.camera.pixelRect.size.y - height - yFromDown;
                        rectNav = new Rect(x, y, defaultWidth, height);
                        return (rectNav);
                    }
                case DEFAULT_POSITION.DOWN_RIGHT:
                    {
                        float yFromDown = 10;  //from bottom, go upper
                        float xFromRight = 10;
                        float x = EditorGUIUtility.currentViewWidth - defaultWidth - xFromRight;
                        float y = SceneView.currentDrawingSceneView.camera.pixelRect.size.y - height - yFromDown;
                        rectNav = new Rect(x, y, defaultWidth, height);
                        return (rectNav);
                    }


                default:
                case DEFAULT_POSITION.MIDDLE:
                    {
                        snapUp = true;
                        snapRight = false;

                        float x = (EditorGUIUtility.currentViewWidth / 2) - (defaultWidth / 2);
                        float y = (_currentEditorWindow.position.height / 2) - (height / 2);
                        rectNav = new Rect(x, y, defaultWidth, height);
                        return (rectNav);
                    }
            }
        }

        public void TinyInit(string keyEditorPref, string nameEditorWindow, DEFAULT_POSITION position, float defaultWidth = 150)
        {
            _currentEditorWindow = SceneView.currentDrawingSceneView;

            Rect rect = GetNiceRectFromEnum(position, defaultWidth, out bool snapUp, out bool snapRight);

            Init(
                    keyEditorPref: keyEditorPref,
                    nameEditorWindow: nameEditorWindow,
                    id: keyEditorPref.GetHashCode(),
                    windowMenu: rect,
                    currentEditorWindow: _currentEditorWindow,
                    currentEvent: Event.current,
                    draggable: true,
                    isMinimisable: false,
                    snapRight: snapRight,
                    snapUp: snapUp,
                    maxRect: new Vector2(0, 0),
                    applySetup: true,
                    closable: true,
                    lockToVisibleSizeX: true,
                    lockToVisibleSizeY: true);
        }

        /// <summary>
        /// need to be called in OnGUI
        /// </summary>
        public void Init(string keyEditorPref,
                        string nameEditorWindow,
                        int id,

                        Rect windowMenu,
                        EditorWindow currentEditorWindow,
                        Event currentEvent,

                        bool draggable,
                        bool isMinimisable,
                        bool snapRight,
                        bool snapUp,

                        Vector2 maxRect,
                        bool applySetup = true,
                        bool closable = false,
                        bool lockToVisibleSizeX = true,
                        bool lockToVisibleSizeY = true)
        {
            _maxRect = maxRect;
            _lockToVisibleSizeX = lockToVisibleSizeX;
            _lockToVisibleSizeY = lockToVisibleSizeY;
            _keyEditorPref = keyEditorPref;
            NameEditorWindow = nameEditorWindow;
            _id = id;
            _windowMenu = windowMenu;
            _currentEditorWindow = currentEditorWindow;
            _currentEvent = currentEvent;

            _draggable = draggable;
            IsMinimisable = isMinimisable;
            _snapRight = snapRight;
            _snapUp = snapUp;

            _isMinimised = false;
            _saveHeight = _windowMenu.height;
            IsClosable = closable;
            IsClosed = false;

            if (applySetup)
            {
                ApplySetup();
            }
        }

        public void SetNewPosition(Vector2 newPosition, Event currentEvent)
        {
            _currentEvent = currentEvent;
            _windowMenu.x = newPosition.x;
            _windowMenu.y = newPosition.y;
            EditorPrefs.SetString(_keyEditorPref, GetJsonOfSave());
        }

        public Rect GetPosition()
        {
            return (_windowMenu);
        }

        private void ApplySetup()
        {
            if (!string.IsNullOrEmpty(_keyEditorPref) && EditorPrefs.HasKey(_keyEditorPref))
            {
                ApplySaveSettings();
            }
            SavePositionFromRight(_snapRight, _snapUp);
        }

        /// <summary>
        /// apply the settings of the saved EditorPref
        /// </summary>
        private void ApplySaveSettings()
        {
            string keyRect = EditorPrefs.GetString(_keyEditorPref);
            _saveEditorPreference = GetFromJsonDatasEditorPref(keyRect);
            _windowMenu = new Rect(_saveEditorPreference.Position.x, _saveEditorPreference.Position.y, _windowMenu.width, _windowMenu.height);
            _isMinimised = _saveEditorPreference.Minimised;
            if (!IsMinimisable)
            {
                IsMinimisable = false;
            }
        }

        private bool HasChanged()
        {
            if (_saveEditorPreference.Position != _windowMenu
                || _saveEditorPreference.Minimised != _isMinimised)
            {
                return (true);
            }
            return (false);
        }

        private void SavePositionFromRight(bool snapRight, bool snapUp)
        {
            if (_currentEditorWindow == null)
            {
                _lastPositionRight = new Vector2(_windowMenu.x, _windowMenu.y);
                return;
            }

            float SnapWidth = (_lockToVisibleSizeX) ? EditorGUIUtility.currentViewWidth : _currentEditorWindow.position.height;
            float Snapheight = (_lockToVisibleSizeY) ? _currentEditorWindow.position.height : _currentEditorWindow.maxSize.y;

            _lastPositionRight.x = (snapRight) ? SnapWidth - _windowMenu.x : _windowMenu.x;
            _lastPositionRight.y = (snapUp) ? _windowMenu.y : Snapheight - _windowMenu.y;
        }

        private Vector2 GetNewRightPositiionFromOld(bool snapXRight, bool snapYUp)
        {
            if (_currentEditorWindow == null)
            {
                return (_lastPositionRight);
            }

            float SnapWidth = (_lockToVisibleSizeX) ? EditorGUIUtility.currentViewWidth : _currentEditorWindow.position.height;
            float Snapheight = (_lockToVisibleSizeY) ? _currentEditorWindow.position.height : _currentEditorWindow.maxSize.y;


            Vector2 pos;
            pos.x = (snapXRight) ? SnapWidth - _lastPositionRight.x : _lastPositionRight.x;
            pos.y = (snapYUp) ? _lastPositionRight.y : Snapheight - _lastPositionRight.y;
            return (pos);
        }

        /// <summary>
        /// called every frame, 
        /// </summary>
        /// <param name="action"></param>
        public Rect ShowEditorWindow(ContentToDrawDelegate action,
                                    EditorWindow currentEditorWindow,
                                    Event current)
        {

            _methodToCall = action;
            _currentEvent = current;
            _currentEditorWindow = currentEditorWindow;


            _windowMenu = ReworkPosition(_windowMenu, _snapRight, _snapUp);



            if (!IsMinimisable)
            {
                _isMinimised = false;
            }

            if (_isMinimised)
            {
                _windowMenu.height = 15f;
                _windowMenu = GUI.Window(_id, _windowMenu, NavigatorWindow, DisplayName(NameEditorWindow));
                SavePositionFromRight(_snapRight, _snapUp);
            }
            else
            {
                //here resize auto
                _windowMenu = GUILayout.Window(_id, _windowMenu, NavigatorWindow, DisplayName(NameEditorWindow), GUILayout.ExpandHeight(true));



                SavePositionFromRight(_snapRight, _snapUp);
            }

            if (!string.IsNullOrEmpty(_keyEditorPref) && HasChanged())
            {
                EditorPrefs.SetString(_keyEditorPref, GetJsonOfSave());
            }


            return (_windowMenu);
        }

        public bool IsMouseOver()
        {
            Vector2 mousePosition = Event.current.mousePosition;
            bool isMouseOver =
                    mousePosition.x >= _windowMenu.x
                && mousePosition.x <= _windowMenu.x + _windowMenu.width
                && mousePosition.y >= _windowMenu.y
                && mousePosition.y <= _windowMenu.y + _windowMenu.height;

            return (isMouseOver);
        }

        private string DisplayName(string nameUncut)
        {
            return (Truncate(nameUncut, 20, "..."));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minimise"></param>
        /// <param name="hard"></param>
        public void SetMinimise(bool minimise, bool hard)
        {
            if (!IsMinimisable)
            {
                if (!hard)
                {
                    return;
                }
                _isMinimised = minimise;
            }
            _isMinimised = minimise;
        }

        private void DisplayClosable()
        {
            float x = _windowMenu.width - 20;
            float y = 0;
            float width = 20f;
            float height = 15f;

            if (_maxRect.x > 0)
            {
                x = Mathf.Min(_maxRect.x, x);
            }
            if (_maxRect.y > 0)
            {
                y = Mathf.Min(_maxRect.y, y);
            }

            GUILayout.BeginArea(new Rect(x, y, width, height));
            if (GUILayout.Button(" x "))
            {
                IsClosed = true;
            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// called to display the minimsie button
        /// </summary>
        private void DisplayMinimise()
        {
            if (_isMinimised)
            {
                GUILayout.BeginArea(new Rect(0, 0, _windowMenu.width, 15));
                GUILayout.Label("   " + NameEditorWindow);
                GUILayout.EndArea();
            }


            float x = _windowMenu.width - 20;
            float y = 0;
            float width = 20f;
            float height = 15f;

            GUILayout.BeginArea(new Rect(x, y, width, height));
            bool minimise = GUILayout.Button(" - ");
            GUILayout.EndArea();

            if (minimise)
            {
                _isMinimised = (_isMinimised) ? false : true;
            }
        }

        private void DisplayDraggable()
        {
            GUILayout.BeginArea(new Rect(0, 0, 15, 15));
            GUILayout.Label("#");
            GUILayout.EndArea();
        }

        /// <summary>
        /// give restriction to position of the panel
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        private Rect ReworkPosition(Rect currentPosition, bool snapRight, bool snapUp)
        {
            if (_currentEditorWindow == null)
            {
                return (currentPosition);
            }


            if (_maxRect.x > 0)
            {
                currentPosition.width = Mathf.Min(_maxRect.x, currentPosition.width);
            }
            if (_maxRect.y > 0)
            {
                currentPosition.height = Mathf.Min(_maxRect.y, currentPosition.height);
            }


            Vector2 snapToRight = GetNewRightPositiionFromOld(snapRight, snapUp);
            currentPosition.x = snapToRight.x;
            currentPosition.y = snapToRight.y;


            float minX = 10;
            float minY = 20;
            float maxX = EditorGUIUtility.currentViewWidth - currentPosition.width - minX;
            float maxY = _currentEditorWindow.position.height - currentPosition.height - 5;

            if (!_lockToVisibleSizeX)
            {
                maxX = _currentEditorWindow.maxSize.x - currentPosition.width - minX;
            }
            if (!_lockToVisibleSizeY)
            {
                maxY = _currentEditorWindow.maxSize.y - currentPosition.height - 5;
            }

            if (currentPosition.x < minX)
            {
                currentPosition.x = minX;
                _snapRight = false;
            }
            if (currentPosition.x > maxX)
            {
                currentPosition.x = maxX;
                _snapRight = true;
            }

            if (currentPosition.y < minY)
            {
                currentPosition.y = minY;
                _snapUp = true;
            }
            //here the _sceneView.camera.pixelRect.size.y is incoherent when applying this event
            if (_currentEvent.type == EventType.Repaint)
            {
                //Debug.Log(_currentEvent.type);
                if (currentPosition.y > maxY)
                {
                    currentPosition.y = maxY;
                    _snapUp = false;
                }
            }

            return (currentPosition);
        }


        /// <summary>
        /// draw the editor window with everything in it
        /// </summary>
        /// <param name="id"></param>
        private void NavigatorWindow(int id)
        {
            if (IsClosable)
            {
                DisplayClosable();
            }
            else if (IsMinimisable)
            {
                DisplayMinimise();
            }

            if (_draggable)
            {
                DisplayDraggable();
                GUI.DragWindow(new Rect(0, 0, _windowMenu.width - 20, 20));  //allow to drag de window only at the top
            }

            //don't draw anything if it's minimised
            if (_isMinimised)
            {
                return;
            }
            _methodToCall();
            PreventClicGoThought(Event.current);
        }

        public void PreventClicGoThought(Event current)
        {
            if ((current.type == EventType.MouseDrag || current.type == EventType.MouseDown)
                && current.button == 0)
            {
                Use();
            }
        }

        #region extension
        /// <summary>
        /// truncate a string with a defined maxLenght
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Truncate(string value, int maxLength, string addAtEnd = "")
        {
            if (string.IsNullOrEmpty(value))
            {
                return (value);
            }
            if (value.Length <= maxLength)
            {
                return (value);
            }
            string truncated = value.Substring(0, maxLength);
            return (truncated + addAtEnd);
        }

        public static void Use()
        {
            if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
            {
                Event.current.Use();
            }
        }

        #endregion
    }
}