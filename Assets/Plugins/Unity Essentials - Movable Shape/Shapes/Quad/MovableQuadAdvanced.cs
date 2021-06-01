using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.extensions;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableQuadAdvanced : MovableQuad
    {
        [SerializeField] private float _radiusMin = 0;
        [SerializeField] private float _radiusMax = 10;

#if UNITY_EDITOR
        [SerializeField] private bool _drawRadius = true;
#endif

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            bool canApplyGravity = _quad.GetClosestPoint(position, ref closestPoint);
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
            _quad.Draw(base.GetColor());
            if (!_drawRadius)
            {
                return;
            }
            if (_radiusMin > 0)
            {
                //4 demi capsule!
                //_quad.DrawWithExtraSize(Color.gray, new Vector3(_radiusMin, _radiusMin, _radiusMin));
            }
            if (_radiusMax > 0)
            {
                //_quad.DrawWithExtraSize(Color.red, new Vector3(_radiusMax, _radiusMax, _radiusMax));
            }
        }
#endif
    }
}