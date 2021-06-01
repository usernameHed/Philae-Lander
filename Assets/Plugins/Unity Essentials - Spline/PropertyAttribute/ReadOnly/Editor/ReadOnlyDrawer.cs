using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Spline.PropertyAttribute.ReadOnly
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // On sauvegarde l'état précédent
            var previousGuiState = GUI.enabled;

            // On désactive la propriété
            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label);

            // On restaure l'état précédent
            GUI.enabled = previousGuiState;
        }
    }
}