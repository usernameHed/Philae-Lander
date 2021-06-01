using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.extensions;

namespace UnityEssentials.Geometry.MovableShape.spline
{
    public class MovableSplineAdvanced : MovableSpline
    {
        [SerializeField] private float _radiusMin = 0;
        [SerializeField] private float _radiusMax = 10;

#if UNITY_EDITOR
        [SerializeField] private bool _drawRadius = true;
#endif

        public override bool IsInsideShape(Vector3 position)
        {
            Vector3 positionOnSpline = Vector3.zero;
            GetClosestPoint(position, ref positionOnSpline);
            bool isInsideShape = (position - positionOnSpline).magnitude < _radiusMin * transform.lossyScale.Maximum() + 0.0001f;
            return (isInsideShape);
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            bool canApplyGravity = base.GetClosestPoint(position, ref closestPoint);
            if (canApplyGravity)
            {
                closestPoint = ExtMovableShapeAdvanced.GetRightPosWithRange(position, closestPoint, _radiusMin * transform.lossyScale.Maximum(), _radiusMax * transform.lossyScale.Maximum(), out bool outOfRange);
                if (outOfRange)
                {
                    canApplyGravity = false;
                }
            }
            return (canApplyGravity);
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            if (!_drawRadius || _spline == null)
            {
                return;
            }
            if (_radiusMin > 0)
            {
                _spline.DrawRadius(_radiusMin * transform.lossyScale.Maximum(), Color.gray);
            }
            if (_radiusMax > 0)
            {
                _spline.DrawRadius(_radiusMax * transform.lossyScale.Maximum(), Color.red);
            }
        }
#endif
    }
}