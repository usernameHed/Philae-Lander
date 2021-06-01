using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.generic
{
    public abstract class PartialEditor : Editor
    {
        public abstract string[] PropertiesToOverride { get; }

        public abstract void DrawSpecificProperty(SerializedProperty property, string propertyName);

        /// <summary>
        ///     Draw the GUI of the associated SerializedObject using the
        ///     custom draw methods for the properties you want
        ///     to override
        /// </summary>
        /// <param name="objectToDraw">The object to draw</param>
        public void DrawGUI(SerializedObject objectToDraw)
        {
            objectToDraw.UpdateIfRequiredOrScript();

            SerializedProperty property = objectToDraw.GetIterator();
            bool expanded = true;
            while (property.NextVisible(expanded))
            {
                using (new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
                {
                    DrawProperty(property);
                }
                expanded = false;
            }
            objectToDraw.ApplyModifiedProperties();
        }

        private void DrawProperty(SerializedProperty property)
        {
            if (PropertiesToOverride.Contains(property.name))
            {
                DrawSpecificProperty(property, property.name);
            }
            else
            {
                EditorGUILayout.PropertyField(property, true);
            }
        }
    }
}