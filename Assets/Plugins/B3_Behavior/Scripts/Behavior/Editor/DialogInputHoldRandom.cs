using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Quantized.Game.Behavior;
using Quantized.Game.Behavior.Window;
using Quantized.Game.Behavior.Components;
using Quantized.Game.Behavior.Edit;

namespace Quantized.Game.Behavior
{
    /// <summary>
    /// Quantizedbit [Sebastian Komorowski]
    /// Dialog input hold random.
    /// </summary>
    public class DialogInputHoldRandom : EditorWindow 
    {
        #region values
        private Components.Composites.RandomSelector _unitRandom;
        private UnityEngine.GUISkin skin;
        private System.Action<object> evt;
        private float currentTimeMin;
        private float currentTimeMax;
        #endregion

        public Components.Composites.RandomSelector unitRandom
        {
            set 
            {
                _unitRandom = value;
                if (_unitRandom != null)
                {
                    Components.Composites.RandomSelector.HoldTime hold = _unitRandom.holdTime;
                    currentTimeMin = hold.min;
                    currentTimeMax = hold.max;
                }
            }
        }

        #region methods - msg
        public void OnGUI ()
        {
            GUISkin backSkin = UnityEngine.GUI.skin;
            UnityEngine.GUI.skin = skin;

            EditorGUI.indentLevel += 1;
            EditorGUILayout.LabelField("Hold Time");
            EditorGUI.indentLevel += 1;

            currentTimeMin = EditorGUILayout.FloatField("Min: ", currentTimeMin);
            currentTimeMax = EditorGUILayout.FloatField("Max: ", currentTimeMax);

            if (currentTimeMin > currentTimeMax)
            {
                float swap = currentTimeMax;
                currentTimeMax = currentTimeMin;
                currentTimeMin = swap;
            }
            if (currentTimeMin < 0f)
            {
                currentTimeMin = 0f;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(150));
                if (GUILayout.Button("   OK   "))
                {
                    _unitRandom.holdTime = new Components.Composites.RandomSelector.HoldTime(currentTimeMin, currentTimeMax);
                    evt("Hold");
                    Close();
                }
                EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(5));
                if (GUILayout.Button(" Cancel "))
                {
                    Close();
                }
            }

            EditorGUI.indentLevel -= 2;
            UnityEngine.GUI.skin = backSkin;
        }

        public void OnLostFocus()
        {
            Close();
        }
        #endregion

        #region methods
        public static DialogInputHoldRandom Show(BComponent unit, System.Action<object> evt)
        {
            DialogInputHoldRandom result = EditorWindow.GetWindowWithRect<DialogInputHoldRandom>(
                new Rect(0f, 0f, 300f, 100f), 
                true, 
                "Set new time values on random selected");
            result.unitRandom = unit as Components.Composites.RandomSelector;
            result.skin = Resources.Load<GUISkin>("dialog");
            result.evt = evt;
            result.ShowPopup();
            return result;
        }
        #endregion
    }
}