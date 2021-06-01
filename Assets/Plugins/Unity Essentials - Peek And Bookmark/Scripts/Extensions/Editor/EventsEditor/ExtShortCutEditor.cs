using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.Peek.Extensions.Editor.EventEditor
{
    /// <summary>
    /// Use shortcut in editor
    /// </summary>
    public static class ExtShortCutEditor
    {
        public enum ModifierKey
        {
            NONE = 0,
            SHIFT = 1,
            CTRL = 2,
            ALT = 3
        }

        #region Use ShortCut
        public static bool IsTriggeringShortCut(Event current, string key1, KeyCode defaultKey1)
        {
            if (current == null)
            {
                return (false);
            }

            KeyCode keycode = ExtShortCutEditor.GetKey(key1, (int)defaultKey1);
            return (current.keyCode == keycode);
        }

        public static bool IsTriggeringShortCut1Modifier1Keys(Event current,
            string modifierKey, ModifierKey modifierDefaultKey,
            string key, KeyCode defaultKey)
        {
            if (current == null)
            {
                return (false);
            }

            ModifierKey modifier = ExtShortCutEditor.GetModifierKey(modifierKey, (int)modifierDefaultKey);
            KeyCode keycode = ExtShortCutEditor.GetKey(key, (int)defaultKey);

            return (ExtShortCutEditor.IsModifierEnumMatch(current, modifier) && current.keyCode == keycode);
        }

        public static void IsTriggeringShortCut1Modifier2Keys(Event current,
            out bool firstKey, out bool secondKey,

            string modifierKey, ModifierKey modifierDefaultKey,
            string key1, KeyCode defaultKey1,
            string key2, KeyCode defaultKey2)
        {
            if (current == null)
            {
                firstKey = false;
                secondKey = false;
                return;
            }

            ModifierKey modifier = ExtShortCutEditor.GetModifierKey(modifierKey, (int)modifierDefaultKey);
            KeyCode firstKeyCode = ExtShortCutEditor.GetKey(key1, (int)defaultKey1);
            KeyCode secondKeyCode = ExtShortCutEditor.GetKey(key2, (int)defaultKey2);

            firstKey = ExtShortCutEditor.IsModifierEnumMatch(current, modifier) && current.keyCode == firstKeyCode;
            secondKey = ExtShortCutEditor.IsModifierEnumMatch(current, modifier) && current.keyCode == secondKeyCode;
        }

        public static bool IsTriggeringShortCut2Modifier1Keys(Event current,
            string modifierKey1, ModifierKey modifierDefaultKey1,
            string modifierKey2, ModifierKey modifierDefaultKey2,
            string key1, KeyCode defaultKey1)
        {
            if (current == null)
            {
                return (false);
            }

            ModifierKey modifier1 = ExtShortCutEditor.GetModifierKey(modifierKey1, (int)modifierDefaultKey1);
            ModifierKey modifier2 = ExtShortCutEditor.GetModifierKey(modifierKey2, (int)modifierDefaultKey2);
            KeyCode keycode = ExtShortCutEditor.GetKey(key1, (int)defaultKey1);

            bool modifierMatch1 = ExtShortCutEditor.IsModifierEnumMatch(current, modifier1);
            bool modifierMatch2 = ExtShortCutEditor.IsModifierEnumMatch(current, modifier2);

            bool manageOneNone = (!modifierMatch1 && modifier1 == ModifierKey.NONE && modifierMatch2)
                || (!modifierMatch2 && modifier2 == ModifierKey.NONE && modifierMatch1);

            bool modifierOk = manageOneNone || (modifierMatch1 && modifierMatch2);
            return (modifierOk && current.keyCode == keycode);
        }
        #endregion

        #region Display ShortCuts
        public static void ShortCut(string description, string toolTip, string key, KeyCode defaultKey)
        {
            ShortCut(description, toolTip, "", ModifierKey.NONE, key, defaultKey);
        }

        public static void ShortCut(string description, string toolTip, string keyModifier, ModifierKey defaultModifier)
        {
            ShortCut(description, toolTip, keyModifier, defaultModifier, "", KeyCode.None);
        }

        public static void ShortCut(string description, string toolTip, string modifierKey, ModifierKey defaultModifierKey, string key, KeyCode defaultKey)
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                GUIStyle centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                centeredStyle.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label(new GUIContent(description, toolTip), centeredStyle);

                if (!string.IsNullOrEmpty(modifierKey))
                {
                    if (!EditorPrefs.HasKey(modifierKey))
                    {
                        EditorPrefs.SetInt(modifierKey, (int)defaultModifierKey);
                    }

                    ModifierKey currentModifier = (ModifierKey)EditorPrefs.GetInt(modifierKey);
                    EditorGUI.BeginChangeCheck();
                    ModifierKey enumModifierSelected = (ModifierKey)EditorGUILayout.EnumPopup(currentModifier, GUILayout.Width(60));
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetInt(modifierKey, (int)enumModifierSelected);
                    }
                }

                if (!string.IsNullOrEmpty(key))
                {
                    if (!EditorPrefs.HasKey(key))
                    {
                        EditorPrefs.SetInt(key, (int)defaultKey);
                    }
                    KeyCode currentKey = (KeyCode)EditorPrefs.GetInt(key);
                    EditorGUI.BeginChangeCheck();
                    KeyCode enumSelected = (KeyCode)EditorGUILayout.EnumPopup(currentKey, GUILayout.Width(100));
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetInt(key, (int)enumSelected);
                    }
                }
            }
        }

        public static void ShortCutOneModifier2Keys(string description, string toolTip, string modifierKey, ModifierKey defaultModifierKey, string key1, KeyCode defaultKey1, string key2, KeyCode defaultKey2)
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                GUIStyle centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                centeredStyle.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label(new GUIContent(description, toolTip), centeredStyle);

                if (!string.IsNullOrEmpty(modifierKey))
                {
                    if (!EditorPrefs.HasKey(modifierKey))
                    {
                        EditorPrefs.SetInt(modifierKey, (int)defaultModifierKey);
                    }

                    ModifierKey currentModifier = (ModifierKey)EditorPrefs.GetInt(modifierKey);
                    EditorGUI.BeginChangeCheck();
                    ModifierKey enumModifierSelected = (ModifierKey)EditorGUILayout.EnumPopup(currentModifier, GUILayout.Width(60));
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetInt(modifierKey, (int)enumModifierSelected);
                    }
                }

                if (!EditorPrefs.HasKey(key1))
                {
                    EditorPrefs.SetInt(key1, (int)defaultKey1);
                }
                KeyCode currentKey = (KeyCode)EditorPrefs.GetInt(key1);
                EditorGUI.BeginChangeCheck();
                KeyCode enumSelected = (KeyCode)EditorGUILayout.EnumPopup(currentKey, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(key1, (int)enumSelected);
                }

                if (!EditorPrefs.HasKey(key2))
                {
                    EditorPrefs.SetInt(key2, (int)defaultKey2);
                }
                KeyCode currentKey2 = (KeyCode)EditorPrefs.GetInt(key2);
                EditorGUI.BeginChangeCheck();
                KeyCode enumSelected2 = (KeyCode)EditorGUILayout.EnumPopup(currentKey2, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(key2, (int)enumSelected2);
                }
            }
        }

        public static void ShortCut2Modifier1Keys(string description, string toolTip,
            string modifierKey1, ModifierKey defaultModifierKey1,
            string modifierKey2, ModifierKey defaultModifierKey2,
            string key, KeyCode defaultKey)
        {
            using (HorizontalScope horizontalScope = new HorizontalScope())
            {
                GUIStyle centeredStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
                centeredStyle.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label(new GUIContent(description, toolTip), centeredStyle);

                if (!string.IsNullOrEmpty(modifierKey1))
                {
                    if (!EditorPrefs.HasKey(modifierKey1))
                    {
                        EditorPrefs.SetInt(modifierKey1, (int)defaultModifierKey1);
                    }

                    ModifierKey currentModifier = (ModifierKey)EditorPrefs.GetInt(modifierKey1);
                    EditorGUI.BeginChangeCheck();
                    ModifierKey enumModifierSelected = (ModifierKey)EditorGUILayout.EnumPopup(currentModifier, GUILayout.Width(60));
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetInt(modifierKey1, (int)enumModifierSelected);
                    }
                }

                if (!string.IsNullOrEmpty(modifierKey2))
                {
                    if (!EditorPrefs.HasKey(modifierKey2))
                    {
                        EditorPrefs.SetInt(modifierKey2, (int)defaultModifierKey2);
                    }

                    ModifierKey currentModifier = (ModifierKey)EditorPrefs.GetInt(modifierKey2);
                    EditorGUI.BeginChangeCheck();
                    ModifierKey enumModifierSelected = (ModifierKey)EditorGUILayout.EnumPopup(currentModifier, GUILayout.Width(60));
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetInt(modifierKey2, (int)enumModifierSelected);
                    }
                }

                if (!EditorPrefs.HasKey(key))
                {
                    EditorPrefs.SetInt(key, (int)defaultKey);
                }
                KeyCode currentKey = (KeyCode)EditorPrefs.GetInt(key);
                EditorGUI.BeginChangeCheck();
                KeyCode enumSelected = (KeyCode)EditorGUILayout.EnumPopup(currentKey, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(key, (int)enumSelected);
                }
            }
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
        public static void ManageEnumModifier(string key, ModifierKey type, string description, string toolTip, int defaultKey)
        {
            if (!EditorPrefs.HasKey(key))
            {
                EditorPrefs.SetInt(key, defaultKey);
            }
            ModifierKey current = (ModifierKey)EditorPrefs.GetInt(key);

            using (HorizontalScope horizontal = new HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent(description, toolTip), GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();
                ModifierKey enumSelected = (ModifierKey)EditorGUILayout.EnumPopup(current, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(key, (int)enumSelected);
                }
            }
        }
        public static ModifierKey GetModifierKey(string key, int defaultKey)
        {
            if (!EditorPrefs.HasKey(key))
            {
                EditorPrefs.SetInt(key, defaultKey);
            }
            ModifierKey current = (ModifierKey)EditorPrefs.GetInt(key);
            return (current);
        }

        public static void ManageEnumKey(string key, KeyCode type, string description, string toolTip, int defaultKey)
        {
            if (!EditorPrefs.HasKey(key))
            {
                EditorPrefs.SetInt(key, defaultKey);
            }
            KeyCode current = (KeyCode)EditorPrefs.GetInt(key);

            using (HorizontalScope horizontal = new HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent(description, toolTip), GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();
                KeyCode enumSelected = (KeyCode)EditorGUILayout.EnumPopup(current, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(key, (int)enumSelected);
                }
            }
        }

        public static KeyCode GetKey(string key, int defaultKey)
        {
            if (!EditorPrefs.HasKey(key))
            {
                EditorPrefs.SetInt(key, defaultKey);
            }
            KeyCode current = (KeyCode)EditorPrefs.GetInt(key);
            return (current);
        }

        #endregion

        //end of class
    }
}