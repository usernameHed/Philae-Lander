using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Spline;
using UnityEssentials.Spline.Controller;

namespace UnityEssentials.Geometry.MovableShape.spline
{
    [Serializable]
    public class MovableSpline : MovableShape
    {
        [SerializeField] protected SplineSmooth _spline = default;
        [SerializeField, Tooltip("Controls how automatic calculation occurs.  A Follow target is necessary to use this feature.")]
        protected ControllerFocusTarget.AutoCalculate _auto = new ControllerFocusTarget.AutoCalculate(0, 2, 5);


        public override void Construct()
        {
            
        }

        public override void Actualize()
        {

        }

#if UNITY_EDITOR
        public override void Draw()
        {

        }
#endif

        public override bool IsInsideShape(Vector3 position)
        {
            Vector3 positionOnSpline = Vector3.zero;
            GetClosestPoint(position, ref positionOnSpline);
            bool isInsideShape = (position - positionOnSpline).magnitude < 0.0001f;
            return (isInsideShape);
        }

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 lossyScale)
        {
            
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            float pathPosition = _spline.FindClosestPoint(
                position,
                0,
                (Time.deltaTime < 0 || _auto.SearchRadius <= 0)
                    ? -1 : _auto.SearchRadius,
                _auto.SearchResolution);

            closestPoint = _spline.EvaluatePositionAtUnit(pathPosition, SplineBase.PositionUnits.PathUnits);
            return (true);
        }
    }
}