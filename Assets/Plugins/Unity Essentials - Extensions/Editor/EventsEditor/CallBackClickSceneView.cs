using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor.EventEditor
{
    [InitializeOnLoad]
    public static class CallBackClickSceneView
    {
        public class HitSceneView
        {
            public GameObject Target;
            public Vector3 PointHit;
            public Vector3 Normal;
            public Ray Ray;
            public RaycastHit Hit;
            public RaycastHit2D Hit2d;

            public void Copy(HitSceneView other)
            {
                Target = other.Target;
                PointHit = other.PointHit;
                Normal = other.Normal;
                Ray = other.Ray;
                Hit = other.Hit;
                Hit2d = other.Hit2d;
            }
        }

        public delegate void ClickOnGameObject(HitSceneView hit);
        public static event ClickOnGameObject OnClick;
        public static event ClickOnGameObject OnDoubleClick;

        public delegate void ClickNTimeOnGameObject(HitSceneView hit, int count);
        public static event ClickNTimeOnGameObject OnClickNTime;


        private static HitSceneView _hover = new HitSceneView();
        private static bool _isOnClick = false;
        private static HitSceneView _hit = new HitSceneView();
        private static float _timingDoubleClick = 0.2f;
        private static EditorChrono _timerDoubleClick = new EditorChrono();
        private static int _clickCount = 0;
        private static GameObject _lastClicked;

        static CallBackClickSceneView()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += DuringSceneGUI;
#else
            SceneView.onSceneGUIDelegate += DuringSceneGUI;
#endif
        }

        private static void DuringSceneGUI(SceneView sceneView)
        {
            if (!_isOnClick && Event.current.type == EventType.MouseDown)
            {
                SetCurrentOverObject(_hover);
                if (_hover.Target != null)
                {
                    _hit.Copy(_hover);
                    _isOnClick = true;
                }
            }
            else if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseMove) && _isOnClick)
            {
                _isOnClick = false;
            }
            else if (Event.current.type == EventType.Used && _isOnClick)
            {
                if (Selection.activeGameObject == _hit.Target)
                {
                    Clicked();
                }
                _isOnClick = false;
            }
        }

        public static void Clicked()
        {
            _clickCount++;
            if (_timerDoubleClick.IsRunning() && _lastClicked == _hit.Target)
            {
                //2 clic
                if (_clickCount == 2)
                {
                    OnDoubleClick?.Invoke(_hit);
                }
            }
            else
            {
                //first click
                _clickCount = 1;
                OnClick?.Invoke(_hit);
            }
            //N click
            OnClickNTime?.Invoke(_hit, _clickCount);

            _timerDoubleClick.StartChrono(_timingDoubleClick);
            _lastClicked = _hit.Target;
        }

        /// <summary>
        /// Raycast from mouse Position in scene view and save object hit
        /// </summary>
        /// <param name="newHit">return the info of the hit</param>
        /// <param name="setNullIfNot">if true, set back the last hitObject to null</param>
        public static void SetCurrentOverObject(HitSceneView newHit, bool setNullIfNot = true)
        {
            if (Event.current == null)
            {
                return;
            }
            Vector2 mousePos = Event.current.mousePosition;
            if (mousePos.x < 0 || mousePos.x >= Screen.width || mousePos.y < 0 || mousePos.y >= Screen.height)
            {
                return;
            }

            RaycastHit _saveRaycastHit;
            Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePos);
            newHit.Ray = worldRay;
            //first a test only for point
            if (Physics.Raycast(worldRay, out _saveRaycastHit, Mathf.Infinity))
            {
                if (_saveRaycastHit.collider.gameObject != null)
                {
                    newHit.Target = _saveRaycastHit.collider.gameObject;
                    newHit.PointHit = _saveRaycastHit.point;
                    newHit.Normal = _saveRaycastHit.normal;
                    newHit.Hit = _saveRaycastHit;
                }
            }
            else
            {
                RaycastHit2D raycast2d = Physics2D.Raycast(worldRay.GetPoint(0), worldRay.direction);
                if (raycast2d.collider != null)
                {
                    newHit.Target = raycast2d.collider.gameObject;
                    newHit.PointHit = raycast2d.point;
                    newHit.Normal = raycast2d.normal;
                    newHit.Hit2d = raycast2d;
                }
                else
                {

                    if (setNullIfNot)
                    {
                        newHit.Target = null;
                        newHit.PointHit = Vector3.zero;
                    }
                }
            }
        }
    }
}