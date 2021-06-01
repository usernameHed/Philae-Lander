using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditorInternal;

namespace UnityEssentials.Spline.Extensions.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtReorderableList
    {
        public static void DeleteAt(this ReorderableList list, int index)
        {
            SerializedProperty element = list.serializedProperty;
            element.RemoveAt(index);
        }

        public static void Move(this ReorderableList list, int previousIndex, int nextIndex)
        {
            SerializedProperty element = list.serializedProperty;
            element.MoveArrayElement(previousIndex, nextIndex);
            element.serializedObject.ApplyModifiedProperties();
        }
    }
}
