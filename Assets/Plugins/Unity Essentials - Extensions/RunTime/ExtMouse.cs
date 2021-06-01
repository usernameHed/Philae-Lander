using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Extensions
{
    public static class ExtMouse
    {
        //private static bool _wasMouseDown;
        //private static Vector2 _clickDownPosition;

        #region mouse event editor
        public enum Modifier
        {
            NONE = 0,
            CONTROL = 10,
            SHIFT = 20,
            ALT = 30,
        }

        public enum ButtonType
        {
            NONE = 0,
            LEFT = 10,
            RIGHT = 20,
            MIDDLE = 30,
        }

        private static List<Modifier> SetupModifiers(Event current)
        {
            List<Modifier> modifiers = new List<Modifier>();
            if (current.control)
            {
                modifiers.Add(Modifier.CONTROL);
            }
            if (current.alt)
            {
                modifiers.Add(Modifier.ALT);
            }
            if (current.shift)
            {
                modifiers.Add(Modifier.SHIFT);
            }
            return (modifiers);
        }

        /// <summary>
        /// warning: current.button return 0 if nothing is pressed !! don't use alone
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private static ButtonType GetButtonType(Event current)
        {
            if (current.button == 0)
            {
                return (ButtonType.LEFT);
            }
            else if (current.button == 1)
            {
                return (ButtonType.RIGHT);
            }
            else if (current.button == 2)
            {
                return (ButtonType.MIDDLE);
            }
            return (ButtonType.NONE);
        }

        public static bool IsScrollingUp(Event current, ref float delta)
        {
            delta = 0;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            EventType eventType = current.GetTypeForControl(controlId);
            if (eventType == EventType.ScrollWheel)
            {
                delta = current.delta.y;
                return (delta > 0);
            }
            return (false);
        }

        public static bool IsScrollingDown(Event current, ref float delta)
        {
            delta = 0;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            EventType eventType = current.GetTypeForControl(controlId);
            if (eventType == EventType.ScrollWheel)
            {
                delta = current.delta.y;
                return (delta < 0);
            }
            return (false);
        }

        public static bool IsLeftMouseDown(Event current)
        {
            EventType eventType = EventType.Repaint;
            List<Modifier> modifiers = new List<Modifier>();
            ButtonType buttonType = ButtonType.LEFT;
            bool clicked = IsMouseClicked(current, ref eventType, ref modifiers, ref buttonType);
            return (clicked && eventType == EventType.MouseDown && buttonType == ButtonType.LEFT);
        }

        /// <summary>
        /// return true if the left mouse button is Down
        /// </summary>
        /// <param name="current">Event.current (only in a OnGUI() loop)</param>
        /// <returns>true if mouse is Down</returns>
        public static bool IsMouseClicked(Event current, ref EventType eventType, ref List<Modifier> modifiers, ref ButtonType buttonType)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            modifiers = new List<Modifier>();
            eventType = current.GetTypeForControl(controlId);

            buttonType = GetButtonType(current);

            if (eventType == EventType.MouseUp)
            {
                eventType = EventType.MouseUp;
                modifiers = SetupModifiers(current);
                return (true);
            }

            if (eventType == EventType.MouseDown)
            {
                eventType = EventType.MouseDown;
                modifiers = SetupModifiers(current);
                return (true);
            }

            return (false);
        }

        /// <summary>
        /// DOESN'T WORK IN ONE SHOOT, need to be called at least twice,
        /// once on mouseDown, and once on mouseUp
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public static bool IsClickOnSceneView(Event current, ref bool wasMouseDown, ref Vector2 clickDownPosition)
        {
            if (ExtMouse.IsLeftMouseDown(current))
            {
                clickDownPosition = current.mousePosition;
                wasMouseDown = true;
                return (false);
            }
            if (current.type == EventType.MouseUp && !Event.current.shift && !Event.current.alt)
            {
                wasMouseDown = false;
                return (true);
            }

            if (!wasMouseDown)
            {
                return (false);
            }
            if (current.type == EventType.Used && current.mousePosition == clickDownPosition && current.delta == Vector2.zero)
            {
                wasMouseDown = false;
                return (true);
            }
            return (false);
        }

        #endregion
    }
}