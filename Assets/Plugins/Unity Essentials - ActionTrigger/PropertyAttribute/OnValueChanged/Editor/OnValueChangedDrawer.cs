using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEssentials.ActionTrigger.PropertyAttribute.Generic;

namespace UnityEssentials.ActionTrigger.PropertyAttribute.OnvalueChanged
{
    [CustomPropertyDrawer(typeof(OnValueChangedAttribute))]
    public class OnValueChangedDrawer : PartialPropertyDrawer
    {
        //private const string LEVEL_PROPERTY = "_levels";
        private string[] _propertiesToOverride = new string[] { /* LEVEL_PROPERTY */ };
        public override string[] PropertiesToOverride => _propertiesToOverride;

        public override void DrawSpecificProperty(Rect position, SerializedProperty property, string propertyName)
        {
            /*
            if (propertyName.Equals(LEVEL_PROPERTY))
            {
                DrawLevelsProperty(position, property);
            }
            else if (propertyName.Equals(BOSS_PROPERTY))
            {
                EditorGUI.PropertyField(position, property, new GUIContent($"Level {BattleDrawer.CurrentLevelDraw} - BOSS"));
                BattleDrawer.CurrentLevelDraw++;
            }
            else
            {
                EditorGUI.LabelField(position, new GUIContent("PROPERTY NOT IMPLEMENTED : " + propertyName));
            }
            */
        }

        public override float GetSpecificPropertyHeight(SerializedProperty property, string propertyName)
        {
            return (0f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            DrawGUI(position, property, label);

            if (EditorGUI.EndChangeCheck())
            {
                string methodName = (attribute as OnValueChangedAttribute).MethodName;
                MethodInfo method = property.serializedObject.targetObject.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                //MethodInfo method = property.GetParent().serializedObject.targetObject.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                object obj = property.serializedObject.targetObject;
                if (method == null)
                {
                    throw new System.Exception("Method \"" + methodName + "\" does not exist, considere using OnValueChanged(nameof(Method)) to avoid that.");
                }
                StaticCallBackMethodInfo.CallAfterDelay(method, obj, null);
            }
        }
    }
}