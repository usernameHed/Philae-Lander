﻿using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.extensions;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableDiscAdvanced : MovableDisc
    {
        [SerializeField] private float _radiusMax = 10;

#if UNITY_EDITOR
        [SerializeField] private bool _drawRadius = true;
#endif

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            bool canApplyGravity = Disc.GetClosestPoint(position, ref closestPoint);
            if (canApplyGravity)
            {
                closestPoint = ExtMovableShapeAdvanced.GetRightPosWithRange(position, closestPoint, 0, _radiusMax * transform.lossyScale.Maximum(), out bool outOfRange);
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
            Disc.Draw(base.GetColor());
            if (!_drawRadius)
            {
                return;
            }
            if (_radiusMax > 0)
            {
                ExtDrawGuizmos.DebugWireSphere(Disc.Position, Color.red, _radiusMax * transform.lossyScale.Maximum());
            }
        }
#endif
    }
}