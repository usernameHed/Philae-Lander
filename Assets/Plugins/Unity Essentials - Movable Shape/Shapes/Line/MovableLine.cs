using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Spline;

namespace UnityEssentials.Geometry.MovableShape.Line
{
    [Serializable]
    public class MovableLine : MovableShape
    {
        [SerializeField] protected SimpleLine _line = default;


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

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 lossyScale)
        {
            
        }

        public override bool IsInsideShape(Vector3 position)
        {
            Vector3 closestPoint = _line.GetClosestPoint(position);
            return (IsClose(closestPoint, position, 0.001f));
        }

        /// <summary>
        /// test if a Vector3 is close to another Vector3 (due to floating point inprecision)
        /// compares the square of the distance to the square of the range as this
        /// avoids calculating a square root which is much slower than squaring the range
        /// </summary>
        /// <param name="val"></param>
        /// <param name="about"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private static bool IsClose(Vector3 val, Vector3 about, float range)
        {
            float close = (val - about).sqrMagnitude;
            return (close < range * range);
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            closestPoint = _line.GetClosestPoint(position);
            return (true);
        }
    }
}