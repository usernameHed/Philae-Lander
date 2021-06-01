using UnityEditor;
using UnityEngine;

namespace UnityEssentials.SceneWorkflow.extension.propertyAttribute.generic
{
    /// <summary>
    ///     Custom property drawer for the planets.
    ///     
    ///     Rely on this code : https://gist.github.com/tomkail/ba4136e6aa990f4dc94e0d39ec6a058c from tomkail on github.
    /// </summary>
    /// <typeparam name="T">for which class the drawer is for</typeparam>
    public abstract class GenericScriptableObjectPropertyDrawer<T> : PropertyDrawer
    {
        private const string SCRIPT_VARIABLE_IN_SCRIPTABLES = "m_Script";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            ScriptableObject data = property.objectReferenceValue as ScriptableObject;
            if (data == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float totalHeight = CalculateScriptableObjectInspectorHeight(data);
            return totalHeight;
        }

        private float CalculateScriptableObjectInspectorHeight(ScriptableObject data)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            using (SerializedObject serializedObject = new SerializedObject(data))
            {
                SerializedProperty objectProperty = serializedObject.GetIterator();
                if (objectProperty.NextVisible(true))
                {
                    totalHeight += CalculateSubPropertiesHeight(serializedObject, objectProperty);
                }
                totalHeight += EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }

        private float CalculateSubPropertiesHeight(SerializedObject serializedObject, SerializedProperty objectProperty)
        {
            float subPropertiesHeight = 0;
            do
            {
                if (!objectProperty.name.Equals(SCRIPT_VARIABLE_IN_SCRIPTABLES))
                {
                    SerializedProperty subProp = serializedObject.FindProperty(objectProperty.name);
                    float height = EditorGUI.GetPropertyHeight(subProp, null, true) + EditorGUIUtility.standardVerticalSpacing;
                    subPropertiesHeight += height;
                }
            }
            while (objectProperty.NextVisible(false));

            return subPropertiesHeight;
        }

        /// <summary>
        ///     Draw all scriptable object content
        ///     and return the next Y position if you want
        ///     to draw under this
        /// </summary>
        /// <param name="position">The rect where we can draw</param>
        /// <param name="property">The property to draw</param>
        /// <param name="label">The label associated to the property</param>
        /// <returns>The next Y position if you want to draw under</returns>
        public float DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            if (property.objectReferenceValue == null)
            {
                Color previousColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                EditorGUI.ObjectField(new Rect(position.x, position.y, position.width - 60, EditorGUIUtility.singleLineHeight), property, typeof(T));
                GUI.backgroundColor = previousColor;
                EditorGUI.EndProperty();
                return position.y + EditorGUIUtility.singleLineHeight;
            }

            DrawFoldoutPropertyField(position, property);
            float lastYPosition = DrawScriptableObject(position, property);
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
            return lastYPosition;
        }

        private void DrawFoldoutPropertyField(Rect position, SerializedProperty property)
        {
            bool isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), property.isExpanded, property.displayName, true);
            if (isExpanded != property.isExpanded)
            {
                property.isExpanded = isExpanded;
            }
            EditorGUI.PropertyField(new Rect(EditorGUIUtility.labelWidth + 14, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), property, GUIContent.none, true);
        }

        private float DrawScriptableObject(Rect position, SerializedProperty property)
        {
            if (GUI.changed)
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            if (property.objectReferenceValue == null)
            {
                EditorGUIUtility.ExitGUI();
            }

            if (!property.isExpanded)
            {
                return position.y + EditorGUIUtility.singleLineHeight;
            }

            using (BoxAroundScriptableObjectInspector boxAround = new BoxAroundScriptableObjectInspector(position))
            {
                return DrawScriptableObjectInspector(position, property);
            }
        }

        private float DrawScriptableObjectInspector(Rect position, SerializedProperty property)
        {
            ScriptableObject data = (ScriptableObject)property.objectReferenceValue;
            float lastY = 0;
            using (SerializedObject serializedObject = new SerializedObject(data))
            {
                lastY = DrawAllPropertiesOfSerializedObject(position, serializedObject);
            }
            return lastY;
        }

        private float DrawAllPropertiesOfSerializedObject(Rect position, SerializedObject serializedObject)
        {
            SerializedProperty prop = serializedObject.GetIterator();
            if (!prop.NextVisible(true))
            {
                return 0;
            }

            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            do
            {
                y += DrawProperty(position, prop, y);
            }
            while (prop.NextVisible(false));
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }

            return y;
        }

        private float DrawProperty(Rect position, SerializedProperty property, float y)
        {
            if (property.name.Equals("m_Script"))
            {
                return 0;
            }
            float height = EditorGUI.GetPropertyHeight(property, new GUIContent(property.displayName), true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), property, true);
            return height + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}