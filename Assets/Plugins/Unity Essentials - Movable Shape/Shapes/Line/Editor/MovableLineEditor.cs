using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline;

namespace UnityEssentials.Geometry.MovableShape.Line.editor
{
    [CustomEditor(typeof(MovableLine), true)]
    public class MovableLineEditor : Editor
    {
        private MovableLine _line;

        private void OnEnable()
        {
            if (target is MovableLine)
            {
                _line = (MovableLine)target;
            }
        }
        
        public void ConstructSplineZone(SplineBase spline)
        {
            this.UpdateEditor();
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<SplineBase>(this.GetPropertie("_line"), spline.transform);
            this.ApplyModification();
        }
    }
}