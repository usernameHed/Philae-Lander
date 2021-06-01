using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.extensions;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableCapsuleAdvanced : MovableCapsule
    {
        [SerializeField] private float _radiusMin = 0;
        [SerializeField] private float _radiusMax = 10;

#if UNITY_EDITOR
        [SerializeField] private bool _drawRadius = true;
#endif

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            bool canApplyGravity = true;
            closestPoint = Capsule.GetClosestPoint(position);
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
            Capsule.Draw(base.GetColor());
            if (!_drawRadius)
            {
                return;
            }
            if (_radiusMin > 0)
            {
                Capsule.DrawWithExtraSize(Color.gray, new Vector3(_radiusMin * transform.lossyScale.Maximum(), _radiusMin * transform.lossyScale.Maximum(), _radiusMin * transform.lossyScale.Maximum()));
            }
            if (_radiusMax > 0)
            {
                Capsule.DrawWithExtraSize(Color.red, new Vector3(_radiusMax * transform.lossyScale.Maximum(), _radiusMax * transform.lossyScale.Maximum(), _radiusMax * transform.lossyScale.Maximum()));
            }
        }
#endif
    }
}