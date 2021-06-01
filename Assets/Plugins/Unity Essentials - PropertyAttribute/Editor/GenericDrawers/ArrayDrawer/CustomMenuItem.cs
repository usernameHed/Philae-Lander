using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.generic
{
    public abstract class CustomMenuItem
    {
        public abstract GUIContent Content { get; }
        public abstract bool CanBeAdded { get; internal set; }

        public void Call(SerializedProperty serializedArray, int elementPosition)
        {
            serializedArray.serializedObject.Update();
            Execute(serializedArray, elementPosition);
            serializedArray.serializedObject.ApplyModifiedProperties();
        }

        protected abstract void Execute(SerializedProperty serializedArray, int elementPosition);
    }

    public class DuplicateArrayElementMenuItem : CustomMenuItem
    {
        private readonly GUIContent _content = new GUIContent("Duplicate Array Element");
        public override GUIContent Content { get => _content; }
        public override bool CanBeAdded { get => true; internal set { } }

        protected override void Execute(SerializedProperty serializedArray, int elementPosition)
        {
            SerializedProperty propertyToCopy = serializedArray.GetArrayElementAtIndex(elementPosition);
            serializedArray.InsertArrayElementAtIndex(elementPosition);
            serializedArray.GetArrayElementAtIndex(elementPosition).objectReferenceValue = propertyToCopy.objectReferenceValue;
        }
    }

    public class DeleteArrayElementMenuItem : CustomMenuItem
    {
        private readonly GUIContent _content = new GUIContent("Delete Array Element");
        public override GUIContent Content { get => _content; }
        public override bool CanBeAdded { get => true; internal set { } }

        protected override void Execute(SerializedProperty serializedArray, int elementPosition)
        {            
            serializedArray.DeleteArrayElementAtIndex(elementPosition);            
        }
    }
}