using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Geometry.MovableShape.Line.editor;
using UnityEssentials.Spline;

namespace UnityEssentials.Geometry.MovableShape.triangle.editor
{
    [CustomEditor(typeof(MovableTriangle), true)]
    public class MovableTriangleEditor : Editor
    {
        private MovableTriangle _triangle;

        private void OnEnable()
        {
            if (target is MovableTriangle)
            {
                _triangle = (MovableTriangle)target;
            }
        }
        
        public void ConstructSplineZone(SplineBase spline)
        {
            this.UpdateEditor();
            ExtSerializedProperties.SetObjectReferenceValueIfEmpty<SplineBase>(this.GetPropertie("_triangle"), spline.transform);
            this.ApplyModification();
        }
    }
}