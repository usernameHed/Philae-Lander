using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Spline;

namespace UnityEssentials.Geometry.MovableShape.spline.editor
{
    [CustomEditor(typeof(MovableSpline), true)]
    public class MovableSplineEditor : Editor
    {
        private MovableSpline _zoneSpline;

        private void OnEnable()
        {
            if (target is MovableSpline)
            {
                _zoneSpline = (MovableSpline)target;
            }
        }

        public void ConstructSplineZone(SplineBase spline)
        {
            this.UpdateEditor();
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<SplineBase>(this.GetPropertie("_spline"), spline.transform);
            this.ApplyModification();
        }
    }
}