using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.generic
{
    public class BoxAroundScriptableObjectInspector : IDisposable
    {
        public BoxAroundScriptableObjectInspector(Rect position)
        {
            GUI.Box(new Rect(0, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing - 1, Screen.width, position.height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing), "");
            EditorGUI.indentLevel++;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel--;
        }
    }
}