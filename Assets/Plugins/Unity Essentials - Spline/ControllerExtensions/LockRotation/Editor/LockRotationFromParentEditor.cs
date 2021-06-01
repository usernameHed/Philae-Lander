using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline.Extensions;
using UnityEssentials.Spline.Extensions.Editor;

namespace UnityEssentials.Spline.ControllerExtensions
{
    [CustomEditor(typeof(LockRotationFromParent))]
    public class LockRotationFromParentEditor : BaseEditor<LockRotationFromParent>
    {
        public override void OnInspectorGUI()
        {
            BeginInspector();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((LockRotationFromParent)target), typeof(LockRotationFromParent), false);
            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            // Ordinary properties
            DrawRemainingPropertiesInInspector();

            if (GUILayout.Button("Reset local Rotation"))
            {
                ResetRotation();
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (_target.OverrideRotationUpGlobal)
                {
                    RotateUp();
                }
                else
                {
                    ResetRotation();
                }
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void RotateUp()
        {
            Undo.RecordObject(_target.ToLock, "Rotate Up");
            _target.ToLock.rotation = ExtRotation.TurretLookRotation(this.GetPropertie("_saveRotation").quaternionValue * Vector3.forward, Vector3.up);
        }

        private void ResetRotation()
        {
            Undo.RecordObject(_target.ToLock, "Reset Transform");
            _target.ToLock.localRotation = Quaternion.identity;
            this.GetPropertie("_saveRotation").SetValue<Quaternion>(_target.ToLock.rotation);
        }
    }
}