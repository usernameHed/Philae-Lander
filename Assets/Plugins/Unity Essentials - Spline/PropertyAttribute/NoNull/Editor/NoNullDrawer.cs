using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Spline.PropertyAttribute.NoNull
{
    [CustomPropertyDrawer(typeof(NoNull))]
    public class NoNullDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect inRect, SerializedProperty inProp, GUIContent label)
        {
            EditorGUI.BeginProperty(inRect, label, inProp);
            Color oldColor = GUI.color;
            bool error = inProp.objectReferenceValue == null;
            if (error)
            {
                label.text = "[!] " + label.text;
                GUI.color = Color.red;
            }

            EditorGUI.PropertyField(inRect, inProp, label);
            GUI.color = oldColor;

            EditorGUI.EndProperty();
        }
    }
}