using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline.Extensions.Editor;

namespace UnityEssentials.Spline.PointsOnSplineExtensions.Editor
{
    [CustomEditor(typeof(TriggerActionPoint))]
    public class TriggerActionPointEditor : BaseEditor<TriggerActionPoint>
    {
        protected virtual void OnEnable()
        {
            //this.UpdateEditor();
            //ExtSerializedProperties.SetObjectReferenceValueIfEmpty<SplineBase>(this.GetPropertie("_spline"), _target.transform);
            //this.ApplyModification();
            Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((TriggerActionPoint)target), typeof(TriggerActionPoint), false);
            GUI.enabled = true;

            DrawRemainingPropertiesInInspector();
        }

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            return excluded;
        }
    }
}
