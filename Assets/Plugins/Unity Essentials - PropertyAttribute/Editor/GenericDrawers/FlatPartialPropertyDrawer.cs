using UnityEditor;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.generic
{
    public abstract class FlatPartialPropertyDrawer : PartialPropertyDrawer
    {
        protected override float GetHeaderHeight()
        {
            return 0f;
        }

        protected override Rect DrawHeader(SerializedProperty property, Rect singleLineRect, GUIContent label)
        {            
            //we expand the property and we don't draw it
            property.isExpanded = true;
            return singleLineRect;
        }

        protected override float DrawPropertyChildren(Rect position, SerializedProperty property)
        {
            return DrawEachPropertyChildren(position, property);
        }
    }
}