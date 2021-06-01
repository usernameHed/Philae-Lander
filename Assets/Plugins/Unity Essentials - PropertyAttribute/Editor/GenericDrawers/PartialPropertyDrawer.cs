using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.generic
{
    public abstract class PartialPropertyDrawer : PropertyDrawer
    {
        public abstract string[] PropertiesToOverride { get; }

        public abstract float GetSpecificPropertyHeight(SerializedProperty property, string propertyName);
        public abstract void DrawSpecificProperty(Rect position, SerializedProperty property, string propertyName);


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.hasChildren || !property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            float totalHeight = CalculateTotalPropertyHeight(property.Copy());
            return totalHeight;
        }

        private float CalculateTotalPropertyHeight(SerializedProperty property)
        {
            SerializedProperty endProperty = property.GetEndProperty(true);
            if (!property.NextVisible(true))
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            float totalHeight = GetHeaderHeight();
            do
            {
                float height;
                if (PropertiesToOverride.Contains(property.name))
                {
                    height = GetSpecificPropertyHeight(property, property.name);
                }
                else
                {
                    height = EditorGUI.GetPropertyHeight(property, null, true);
                }
                totalHeight += height + EditorGUIUtility.standardVerticalSpacing;
            }
            while (property.NextVisible(false) && /*property != endProperty*/!SerializedProperty.EqualContents(property, endProperty));

            return totalHeight;
        }

        protected virtual float GetHeaderHeight()
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
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
            SerializedProperty copy = property.Copy();
            float lastYPosition = DrawProperty(position, copy, label);
            return lastYPosition;
        }

        private float DrawProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.hasChildren)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property, label);
                return position.y + EditorGUIUtility.singleLineHeight;
            }

            Rect singleLineRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            singleLineRect = DrawHeader(property, singleLineRect, label);
            if (!CanDrawBody(property))
            {
                return singleLineRect.y;
            }
            singleLineRect.width = position.width;
            return DrawPropertyChildren(singleLineRect, property);
        }

        protected virtual Rect DrawHeader(SerializedProperty property, Rect singleLineRect, GUIContent label)
        {
            property.isExpanded = EditorGUI.Foldout(singleLineRect, property.isExpanded, label, true);
            singleLineRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return singleLineRect;
        }

        private bool CanDrawBody(SerializedProperty property)
        {
            return property.isExpanded;
        }

        protected virtual float DrawPropertyChildren(Rect position, SerializedProperty property)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                return DrawEachPropertyChildren(position, property);
            }
        }

        protected float DrawEachPropertyChildren(Rect position, SerializedProperty property)
        {
            SerializedProperty endProperty = property.GetEndProperty(true);
            if (!property.NextVisible(true))
            {
                return position.y;
            }

            float y = position.y;
            do
            {
                y += DrawProperty(position, property, y);
            }
            while (property.NextVisible(false) && /*property != endProperty*/!SerializedProperty.EqualContents(property, endProperty));
            return y;
        }

        private float DrawProperty(Rect position, SerializedProperty property, float y)
        {
            float height;
            if (PropertiesToOverride.Contains(property.name))
            {
                height = GetSpecificPropertyHeight(property.Copy(), property.name);
                DrawSpecificProperty(new Rect(position.x, y, position.width, height), property, property.name);
            }
            else
            {
                height = EditorGUI.GetPropertyHeight(property, new GUIContent(property.displayName), true);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), property, true);
            }
            return height + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}