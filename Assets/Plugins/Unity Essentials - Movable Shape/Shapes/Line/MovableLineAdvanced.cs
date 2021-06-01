using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.extensions;

namespace UnityEssentials.Geometry.MovableShape.Line
{
    public class MovableLineAdvanced : MovableLine
    {
        [SerializeField] private float _radiusMin = 0;
        [SerializeField] private float _radiusMax = 10;

#if UNITY_EDITOR
        [SerializeField] private bool _drawRadius = true;
#endif

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            bool canApplyGravity = true;
            closestPoint = _line.GetClosestPoint(position);
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
            if (!_drawRadius || _line == null)
            {
                return;
            }
            if (_radiusMin > 0)
            {
                _line.DrawRadius(_radiusMin * transform.lossyScale.Maximum(), Color.gray);
            }
            if (_radiusMax > 0)
            {
                _line.DrawRadius(_radiusMax * transform.lossyScale.Maximum(), Color.red);
            }
        }
#endif
    }
}