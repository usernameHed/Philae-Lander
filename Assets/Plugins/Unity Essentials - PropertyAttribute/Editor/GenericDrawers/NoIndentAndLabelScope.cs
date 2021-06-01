using System;
using UnityEditor;

namespace UnityEssentials.PropertyAttribute.generic
{
    public class NoIndentAndLabelScope : IDisposable
    {
        private int _initialIndentLevel;

        public NoIndentAndLabelScope() : this(0.01f)
        {
        }

        public NoIndentAndLabelScope(float labelWidth)
        {
            _initialIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = _initialIndentLevel;
            EditorGUIUtility.labelWidth = 0;
        }
    }
}