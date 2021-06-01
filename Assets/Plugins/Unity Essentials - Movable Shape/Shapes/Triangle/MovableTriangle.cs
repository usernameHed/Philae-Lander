using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Spline;

namespace UnityEssentials.Geometry.MovableShape.triangle
{
    [Serializable]
    public class MovableTriangle : MovableShape
    {
        [SerializeField] private Triangle _triangle = default;


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
            return (_triangle.IsInsideShape(position));
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            closestPoint = _triangle.GetClosestPoint(position);
            return (true);
        }
    }
}