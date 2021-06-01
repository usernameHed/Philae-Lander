using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableSphereHalfAdvanced : MovableSphereHalf
    {
        [SerializeField] private float _radiusMin = 0;
        [SerializeField] private float _radiusMax = 10;

#if UNITY_EDITOR
        [SerializeField] private bool _drawRadius = true;
#endif

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            bool canApplyGravity = true;
            closestPoint = SphereHalf.GetClosestPoint(position);
            if (canApplyGravity)
            {
                closestPoint = ExtMovableShapeAdvanced.GetRightPosWithRange(position, closestPoint, _radiusMin, _radiusMax, out bool outOfRange);
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
            SphereHalf.Draw(base.GetColor());
            if (!_drawRadius)
            {
                return;
            }
            if (_radiusMin > 0)
            {
                SphereHalf.DrawWithExtraRadius(Color.gray, _radiusMin);
            }
            if (_radiusMax > 0)
            {
                SphereHalf.DrawWithExtraRadius(Color.red, _radiusMax);
            }
        }
#endif
    }
}