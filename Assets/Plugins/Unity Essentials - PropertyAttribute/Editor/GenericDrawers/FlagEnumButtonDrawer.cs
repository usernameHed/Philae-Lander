using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.generic
{
    public abstract class FlagEnumButtonDrawer<T> : PropertyDrawer where T : Enum
    {
        protected virtual int ButtonsPerRow { get => 2; }
        protected abstract T NoneButton { get; }

        private int _noneButtonPosition;
        private int _enumValueReset;

        private Array _enumValues;
        private bool[] _buttonPressed;
        private int _buttonIntValue;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _enumValues = Enum.GetValues(typeof(T));
            int numberOfRows = 1 + (_enumValues.Length / ButtonsPerRow);
            return numberOfRows * base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _enumValues = Enum.GetValues(typeof(T));
            _noneButtonPosition = Array.IndexOf(_enumValues, NoneButton);
            _enumValueReset = (int)_enumValues.GetValue(_noneButtonPosition);
            _buttonIntValue = _enumValueReset;

            _buttonPressed = new bool[_enumValues.Length];
            float buttonWidth = (position.width - EditorGUIUtility.labelWidth) / ButtonsPerRow;
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);
            float yUnderLabel = labelRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            Rect buttonsRect = new Rect(labelRect.x, yUnderLabel,
                buttonWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();
            DrawAllButtons(position, property, buttonWidth, buttonsRect);
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = _buttonIntValue;
            }
        }

        private void DrawAllButtons(Rect position, SerializedProperty property, float buttonWidth, Rect buttonsRect)
        {
            DrawAllButtonsExceptNoneOne(property, buttonsRect);
            AddNoneButton(position, buttonWidth);
            if (_buttonPressed[_noneButtonPosition])
            {
                _buttonIntValue = _enumValueReset;
            }
        }

        private void DrawAllButtonsExceptNoneOne(SerializedProperty property, Rect buttonsRect)
        {
            int buttonDrawn = 0;
            for (int i = 0; i < _enumValues.Length; i++)
            {
                bool isNoneButton = i == _noneButtonPosition;
                if (!isNoneButton)
                {
                    AddFlagButton(buttonsRect, property, i, buttonDrawn);
                    buttonDrawn++;
                }
            }
        }

        private void AddFlagButton(Rect buttonRect, SerializedProperty property, int buttonNumber, int buttonDrawn)
        {
            int enumValue = (int)_enumValues.GetValue(buttonNumber);
            bool isButtonPressed = (property.intValue & enumValue) == enumValue;
            _buttonPressed[buttonNumber] = isButtonPressed;
            float height = buttonRect.y + EditorGUIUtility.singleLineHeight * (buttonDrawn / ButtonsPerRow);
            float xButton = buttonRect.x + EditorGUIUtility.labelWidth + buttonRect.width * (buttonDrawn % ButtonsPerRow);
            Rect buttonPos = new Rect(xButton, height, buttonRect.width, buttonRect.height);
            _buttonPressed[buttonNumber] = GUI.Toggle(buttonPos, _buttonPressed[buttonNumber], _enumValues.GetValue(buttonNumber).ToString(), "Button");
            if (_buttonPressed[buttonNumber])
            {
                _buttonIntValue += enumValue;
            }
        }

        private void AddNoneButton(Rect position, float buttonWidth)
        {
            bool isButtonResetPressed = _buttonIntValue == _enumValueReset;
            _buttonPressed[_noneButtonPosition] = isButtonResetPressed;
            Rect buttonPosReset = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, buttonWidth, EditorGUIUtility.singleLineHeight);
            _buttonPressed[_noneButtonPosition] = GUI.Toggle(buttonPosReset, _buttonPressed[_noneButtonPosition], NoneButton.ToString(), "Button");
        }
    }
}