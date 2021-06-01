using UnityEngine;
using UnityEngine.Events;

namespace UnityEssentials.Spline.ControllerExtensions
{
    /// <summary>
    /// Use shortcut in editor
    /// </summary>
    public static class ExtShortCut
    {
        public enum ModifierKey
        {
            NONE = 0,
            SHIFT = 1,
            CTRL = 2,
            ALT = 3
        }

        #region Use Shortcut
        public static bool IsTriggeringShortCut(Event current, KeyCode keycode)
        {
            if (current == null)
            {
                return (false);
            }
            return (current.keyCode == keycode);
        }

        public static bool IsTriggeringShortCut1Modifier1Keys(Event current,
            ModifierKey modifier,
            KeyCode keycode)
        {
            if (current == null)
            {
                return (false);
            }

            return (IsModifierEnumMatch(current, modifier) && current.keyCode == keycode);
        }

        public static void IsTriggeringShortCut1Modifier2Keys(Event current,
            out bool firstKey, out bool secondKey,
            ModifierKey modifier,
            KeyCode firstKeyCode,
            KeyCode secondKeyCode)
        {
            if (current == null)
            {
                firstKey = false;
                secondKey = false;
                return;
            }

            firstKey = IsModifierEnumMatch(current, modifier) && current.keyCode == firstKeyCode;
            secondKey = IsModifierEnumMatch(current, modifier) && current.keyCode == secondKeyCode;
        }

        public static bool IsTriggeringShortCut2Modifier1Keys(Event current,
            ModifierKey modifier1,
            ModifierKey modifier2,
            KeyCode keycode)
        {
            if (current == null)
            {
                return (false);
            }

            bool modifierMatch1 = IsModifierEnumMatch(current, modifier1);
            bool modifierMatch2 = IsModifierEnumMatch(current, modifier2);

            bool manageOneNone = (!modifierMatch1 && modifier1 == ModifierKey.NONE && modifierMatch2)
                || (!modifierMatch2 && modifier2 == ModifierKey.NONE && modifierMatch1);

            bool modifierOk = manageOneNone || (modifierMatch1 && modifierMatch2);
            return (modifierOk && current.keyCode == keycode);
        }
        #endregion

        #region Misc
        public static bool IsModifierEnumMatch(Event current, ModifierKey modifier)
        {
            return (current.shift && modifier == ModifierKey.SHIFT
                || current.alt && modifier == ModifierKey.ALT
                || current.control && modifier == ModifierKey.CTRL
                || (!current.shift && !current.alt && !current.control && modifier == ModifierKey.NONE));
        }
        #endregion
        //end of class
    }
}