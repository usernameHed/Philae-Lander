using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace UnityEssentials.PropertyAttribute.generic
{
    public class CustomArrayDrawer<T>
    {
        private SerializedProperty _serializedArray;
        private List<CustomMenuItem> _menuItems = new List<CustomMenuItem>(4);

        public float DrawArray(Rect propertyRect, SerializedProperty serializedProperty)
        {
            _serializedArray = serializedProperty;
            propertyRect.height = EditorGUIUtility.singleLineHeight;
            serializedProperty.isExpanded = EditorGUI.Foldout(propertyRect, _serializedArray.isExpanded, _serializedArray.displayName, true);
            propertyRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (!serializedProperty.isExpanded)
            {
                return propertyRect.y;
            }
            propertyRect = DrawSizeAndElements(propertyRect);
            return propertyRect.y;
        }

        private Rect DrawSizeAndElements(Rect propertyRect)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                _serializedArray.arraySize = EditorGUI.DelayedIntField(propertyRect, "Size", _serializedArray.arraySize);
                propertyRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                propertyRect = DrawElements(propertyRect);
            }
            return propertyRect;
        }

        private Rect DrawElements(Rect propertyRect)
        {
            Event e = Event.current;
            bool isRightClicking = e.type == EventType.MouseDown && e.button == 1;
            for (int i = 0; i < _serializedArray.arraySize; i++)
            {
                SerializedProperty serializedcardInfo = _serializedArray.GetArrayElementAtIndex(i);
                serializedcardInfo.objectReferenceValue = EditorGUI.ObjectField(propertyRect, "Element " + i, serializedcardInfo.objectReferenceValue, typeof(T), false);
                if(isRightClicking && propertyRect.Contains(e.mousePosition))
                {
                    BuildContextualMenu(_serializedArray, i);
                }
                propertyRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            return propertyRect;
        }

        private void BuildContextualMenu(SerializedProperty serializedArray, int elementPosition)
        {
            GenericMenu context = new GenericMenu();
            foreach (CustomMenuItem item in _menuItems)
            {
                if (item.CanBeAdded)
                {
                    context.AddItem(item.Content, false, () => item.Call(serializedArray, elementPosition));
                }
            }
            context.ShowAsContext();
        }

        public void AddContextMenuToElements(CustomMenuItem item) {
            _menuItems.Add(item);
        }

        public void ClearMenuItems()
        {
            _menuItems.Clear();
        }
    }
}
