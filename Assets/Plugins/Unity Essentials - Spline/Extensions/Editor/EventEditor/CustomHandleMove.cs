using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Spline.Extensions.Editor.EventEditor
{
    /// <summary>
    /// usage: 
    /// Vector3 position;
    /// CustomHandleMove.DragHandleResult handleResult;
    /// 
    /// float size = HandleUtility.GetHandleSize(position) * 0.1f;
    /// 
    /// position = CustomHandleMove.DragHandle(position, size * 3, Handles.SphereCap, Color.green, out handleResult, gridSize);
    /// if (EditorGUI.EndChangeCheck())
    /// {
    ///     //here changed
    /// }
    /// 
    /// switch (handleResult)
    /// {
    ///     case CustomHandleMove.DragHandleResult.OnLeftPress:
    ///         break;
    /// 
    ///     case CustomHandleMove.DragHandleResult.OnLeftRelease:
    ///         break;
    /// }
    /// </summary>
    public class CustomHandleMove
    {
        // internal state for DragHandle()
        static int s_DragHandleHash = "DragHandleHash".GetHashCode();
        static Vector2 s_DragHandleMouseStart;
        static Vector2 s_DragHandleMouseCurrent;
        static Vector3 s_DragHandleWorldStart;
        static float s_DragHandleClickTime = 0;
        static int s_DragHandleClickID;
        static float s_DragHandleDoubleClickInterval = 0.5f;
        static bool s_DragHandleHasMoved;

        // externally accessible to get the ID of the most resently processed DragHandle
        public static int lastDragHandleID;

        public enum DragHandleResult
        {
            none = 0,

            OnLeftPress,
            OnLeftClick,
            OnLeftDoubleClick,
            OnLeftDrag,
            OnLeftRelease,

            OnRightPress,
            OnRightClick,
            OnRightDoubleClick,
            OnRightDrag,
            OnRightRelease,
        };

        public static Vector3 DragHandle(Vector3 position, float handleSize, Color colorSelected, out DragHandleResult result, Quaternion rotation, float snap = 0)
        {
            int id = GUIUtility.GetControlID(s_DragHandleHash, FocusType.Passive);
            lastDragHandleID = id;

            Vector3 screenPosition = Handles.matrix.MultiplyPoint(position);
            Matrix4x4 cachedMatrix = Handles.matrix;

            result = DragHandleResult.none;

            switch (Event.current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (HandleUtility.nearestControl == id && (Event.current.button == 0 || Event.current.button == 1))
                    {
                        GUIUtility.hotControl = id;
                        s_DragHandleMouseCurrent = s_DragHandleMouseStart = Event.current.mousePosition;
                        s_DragHandleWorldStart = position;
                        s_DragHandleHasMoved = false;

                        Event.current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);

                        if (Event.current.button == 0)
                            result = DragHandleResult.OnLeftPress;
                        else if (Event.current.button == 1)
                            result = DragHandleResult.OnRightPress;
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id && (Event.current.button == 0 || Event.current.button == 1))
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);

                        if (Event.current.button == 0)
                            result = DragHandleResult.OnLeftRelease;
                        else if (Event.current.button == 1)
                            result = DragHandleResult.OnRightRelease;

                        if (Event.current.mousePosition == s_DragHandleMouseStart)
                        {
                            bool doubleClick = (s_DragHandleClickID == id) &&
                                (Time.realtimeSinceStartup - s_DragHandleClickTime < s_DragHandleDoubleClickInterval);

                            s_DragHandleClickID = id;
                            s_DragHandleClickTime = Time.realtimeSinceStartup;

                            if (Event.current.button == 0)
                                result = doubleClick ? DragHandleResult.OnLeftDoubleClick : DragHandleResult.OnLeftClick;
                            else if (Event.current.button == 1)
                                result = doubleClick ? DragHandleResult.OnRightDoubleClick : DragHandleResult.OnRightClick;
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_DragHandleMouseCurrent += new Vector2(Event.current.delta.x, -Event.current.delta.y);
                        Vector3 position2 = Camera.current.WorldToScreenPoint(Handles.matrix.MultiplyPoint(s_DragHandleWorldStart))
                            + (Vector3)(s_DragHandleMouseCurrent - s_DragHandleMouseStart);
                        position = Handles.matrix.inverse.MultiplyPoint(Camera.current.ScreenToWorldPoint(position2));

                        if (Camera.current.transform.forward == Vector3.forward || Camera.current.transform.forward == -Vector3.forward)
                            position.z = s_DragHandleWorldStart.z;
                        if (Camera.current.transform.forward == Vector3.up || Camera.current.transform.forward == -Vector3.up)
                            position.y = s_DragHandleWorldStart.y;
                        if (Camera.current.transform.forward == Vector3.right || Camera.current.transform.forward == -Vector3.right)
                            position.x = s_DragHandleWorldStart.x;

                        if (Event.current.button == 0)
                            result = DragHandleResult.OnLeftDrag;
                        else if (Event.current.button == 1)
                            result = DragHandleResult.OnRightDrag;

                        s_DragHandleHasMoved = true;

                        GUI.changed = true;
                        Event.current.Use();
                    }
                    break;

                case EventType.Repaint:
                    Color currentColour = Handles.color;
                    if (id == GUIUtility.hotControl && s_DragHandleHasMoved)
                        Handles.color = colorSelected;

                    Handles.matrix = Matrix4x4.identity;
                    Handles.CircleHandleCap(id, screenPosition, rotation, handleSize, EventType.Repaint);
                    Handles.matrix = cachedMatrix;

                    Handles.color = currentColour;
                    break;

                case EventType.Layout:
                    Handles.matrix = Matrix4x4.identity;
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(screenPosition, handleSize));
                    Handles.matrix = cachedMatrix;
                    break;
            }

            
            return AttemptToApplyGrid(position, snap);
        }

        public static Vector3 AttemptToApplyGrid(Vector3 position, float gridSize, float defaultWhenNoControl = 0)
        {
            if (gridSize != 0 && (Event.current.control || Event.current.alt || Event.current.shift))
            {
                if (Event.current.control && !Event.current.alt)
                {
                    position = new Vector3(
                        RoundToGrid(position.x, gridSize),
                        RoundToGrid(position.y, gridSize),
                        RoundToGrid(position.z, gridSize));
                }
                else if (Event.current.alt && !Event.current.control)
                {
                    position = new Vector3(
                        RoundToGrid(position.x, gridSize * 2),
                        RoundToGrid(position.y, gridSize * 2),
                        RoundToGrid(position.z, gridSize * 2));
                }
                else if (Event.current.alt && Event.current.control)
                {
                    position = new Vector3(
                        RoundToGrid(position.x, gridSize * 8),
                        RoundToGrid(position.y, gridSize * 8),
                        RoundToGrid(position.z, gridSize * 8));
                }
            }
            else if (defaultWhenNoControl != 0)
            {
                position = new Vector3(
                       RoundToGrid(position.x, defaultWhenNoControl),
                       RoundToGrid(position.y, defaultWhenNoControl),
                       RoundToGrid(position.z, defaultWhenNoControl));
            }
            return (position);
        }

        public static float AttemptToApplyGrid(float position, float gridSize, float defaultWhenNoControl = 0)
        {
            if (gridSize != 0 && (Event.current.control || Event.current.alt || Event.current.shift))
            {
                if (Event.current.control && !Event.current.alt)
                {
                    position = RoundToGrid(position, gridSize);
                }
                else if (Event.current.alt && !Event.current.control)
                {
                    position = RoundToGrid(position, gridSize * 2);
                }
                else if (Event.current.alt && Event.current.control)
                {
                    position = RoundToGrid(position, gridSize * 8);
                }
            }
            else if (defaultWhenNoControl != 0)
            {
                position = RoundToGrid(position, defaultWhenNoControl);
            }
            return (position);
        }

        private static float RoundToGrid(float input, float snap)
        {
            return snap * Mathf.Round((input / snap));
        }
    }
}