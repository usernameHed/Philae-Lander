using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.Geometry.Editor.Gui
{
    public static class ExtGUIFloatFields
    {
        public static int IntField(float valueToModify,
            UnityEngine.Object objToRecord,
            out bool valueHasChanged,
            string label = "",
            string toolTipText = "",
            float min = -9999,
            float max = 9999,
            bool createHorizontalScope = true,
            bool delayedFloatField = false,
            float defaultWidth = 40,
            params GUILayoutOption[] options)
        {
            float floatValue = FloatField(valueToModify, objToRecord, out valueHasChanged, label, toolTipText, min, max, createHorizontalScope, delayedFloatField, defaultWidth, options);
            return ((int)Mathf.Round(floatValue));
        }

        public static float FloatField(float valueToModify, string text = "")
        {
            return (FloatField(valueToModify, null, out bool valueHasChanged, text));
        }

        public static float FloatField(float valueToModify, UnityEngine.Object objToRecord, string text = "")
        {
            return (FloatField(valueToModify, objToRecord, out bool valueHasChanged, text));
        }

        public static float FloatField(float valueToModify, UnityEngine.Object objToRecord, string text, out bool valueHasChanged)
        {
            return (FloatField(valueToModify, objToRecord, out valueHasChanged, text));
        }

        /// <summary>
        /// draw a float field with a slider to move
        /// must be called in OnSceneGUI
        /// </summary>
        /// <param name="valueToModify"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float FloatField(float valueToModify,
            UnityEngine.Object objToRecord,
            out bool valueHasChanged,
            string label = "",
            string toolTipText = "",
            float min = -9999,
            float max = 9999,
            bool createHorizontalScope = true,
            bool delayedFloatField = false,
            float defaultWidth = 40,
            params GUILayoutOption[] options)
        {
            float oldValue = valueToModify;

            if (objToRecord != null)
            {
                Undo.RecordObject(objToRecord, "record " + objToRecord.name + " editor GUI change");
            }

            float newValue = valueToModify;

            if (createHorizontalScope)
            {
                using (HorizontalScope horizontalScopeSlider = new HorizontalScope(GUILayout.MaxWidth(50)))
                {
                    newValue = FloatFieldWithSlider(valueToModify, label, toolTipText, min, max, delayedFloatField, defaultWidth, options);
                }
            }
            else
            {
                newValue = FloatFieldWithSlider(valueToModify, label, toolTipText, min, max, delayedFloatField, defaultWidth, options);
            }

            valueHasChanged = oldValue != newValue;


            if (valueHasChanged && Event.current.control/* && Event.current.button == 0 && Event.current.GetTypeForControl(GUIUtility.GetControlID(FocusType.Passive)) == EventType.MouseDown*/)
            {
                newValue = Round(newValue, 1);
            }
            /*
            else if (valueHasChanged && Event.current.shift)
            {
                Debug.Log(Event.current);

                Debug.Log("shift");
                float diff = (oldValue - newValue) / 100;
                newValue = oldValue - diff;
            }
            */
            else if (valueHasChanged && Event.current.alt)
            {
                float diff = (oldValue - newValue) * 100;
                newValue = oldValue - diff;
            }




            if (valueHasChanged && objToRecord != null)
            {
                EditorUtility.SetDirty(objToRecord);
            }

            return (newValue);
        }

        public static float FloatFieldWithSlider(float value, string label = "", string toolTipText = "", float min = -9999, float max = 9999, bool delayedFloatField = false, float defaultWidth = 40, params GUILayoutOption[] options)
        {
            if (!string.IsNullOrEmpty(label))
            {
                GUIContent gUIContent = new GUIContent(label, toolTipText);
                GUILayout.Label(gUIContent, options);
            }
            else
            {
                GUIContent gUIContent = new GUIContent("", toolTipText);
                GUILayout.Label(gUIContent);
            }

            Rect newRect = GUILayoutUtility.GetLastRect();
            Rect posRect = new Rect(newRect.x + newRect.width - 2, newRect.y, defaultWidth, 15);
            value = MyFloatFieldInternal(posRect, newRect, value, EditorStyles.numberField);
            value = SetBetween(value, min, max);

            GUILayout.Label("", GUILayout.Width(30));

            return (value);
        }

        private static float MyFloatFieldInternal(Rect position, Rect dragHotZone, float value, GUIStyle style)
        {
            int controlID = GUIUtility.GetControlID("EditorTextField".GetHashCode(), FocusType.Keyboard, position);
            Type editorGUIType = typeof(EditorGUI);

            Type RecycledTextEditorType = Assembly.GetAssembly(editorGUIType).GetType("UnityEditor.EditorGUI+RecycledTextEditor");
            Type[] argumentTypes = new Type[] { RecycledTextEditorType, typeof(Rect), typeof(Rect), typeof(int), typeof(float), typeof(string), typeof(GUIStyle), typeof(bool) };
            MethodInfo doFloatFieldMethod = editorGUIType.GetMethod("DoFloatField", BindingFlags.NonPublic | BindingFlags.Static, null, argumentTypes, null);

            FieldInfo fieldInfo = editorGUIType.GetField("s_RecycledEditor", BindingFlags.NonPublic | BindingFlags.Static);
            object recycledEditor = fieldInfo.GetValue(null);

            object[] parameters = new object[] { recycledEditor, position, dragHotZone, controlID, value, "g7", style, true };

            return (float)doFloatFieldMethod.Invoke(null, parameters);
        }

        #region extensions

        public static float Round(float value, int digits)
        {
            float mult = Mathf.Pow(10.0f, (float)digits);
            return Mathf.Round(value * mult) / mult;
        }

        /// <summary>
        /// return the value clamped between the 2 value
        /// </summary>
        /// <param name="value1">must be less than value2</param>
        /// <param name="currentValue"></param>
        /// <param name="value2">must be more than value1</param>
        /// <returns></returns>
        public static float SetBetween(float currentValue, float value1, float value2)
        {
            if (value1 > value2)
            {
                Debug.LogError("value2 can be less than value1");
                return (0);
            }

            if (currentValue < value1)
            {
                currentValue = value1;
            }
            if (currentValue > value2)
            {
                currentValue = value2;
            }
            return (currentValue);
        }
        #endregion

        //end class
    }
    //end nameSpace
}