using UnityEngine;
using UnityEditor;

namespace UnityEssentials.PropertyAttribute.animationCurve
{
    [CustomPropertyDrawer(typeof(CurveAttribute))]
    public class CurveDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CurveAttribute curve = attribute as CurveAttribute;
            if (property.propertyType == SerializedPropertyType.AnimationCurve)
            {
                float currentXMin = GetVariableValue(position, property, curve.XMinVariable, curve.XMinValue, out bool canDrawXMin);
                if (!canDrawXMin)
                {
                    return;
                }
                float currentXMax = GetVariableValue(position, property, curve.XMaxVariable, curve.XMaxValue, out bool canDrawXMax);
                if (!canDrawXMax)
                {
                    return;
                }
                float currentYMin = GetVariableValue(position, property, curve.YMinVariable, curve.YMinValue, out bool canDrawYMin);
                if (!canDrawYMin)
                {
                    return;
                }
                float currentYMax = GetVariableValue(position, property, curve.YMaxVariable, curve.YMaxValue, out bool canDrawYMax);
                if (!canDrawYMax)
                {
                    return;
                }
                float xMax = currentXMax - currentXMin;
                float yMax = currentYMax - currentYMin;
                EditorGUI.CurveField(position, property, Color.cyan, new Rect(currentXMin, currentYMin, xMax, yMax));
            }
        }

        private float GetVariableValue(Rect position, SerializedProperty property, string rangeVariable, float defaultValue, out bool canDraw)
        {
            canDraw = true;
            if (!string.IsNullOrEmpty(rangeVariable))
            {
                SerializedProperty max = property.GetParent().FindPropertyRelative(rangeVariable);
                if (max == null)
                {
                    EditorGUI.HelpBox(position, "variable " + rangeVariable + " doesn't exist !", MessageType.Error);
                    canDraw = false;
                    return (0);
                }
                else if (max.propertyType != SerializedPropertyType.Float)
                {
                    EditorGUI.HelpBox(position, "variable " + rangeVariable + " is not a float !", MessageType.Error);
                    canDraw = false;
                    return (0);
                }
                else
                {
                    return (max.floatValue);
                }
            }
            return (defaultValue);
        }
    }
}