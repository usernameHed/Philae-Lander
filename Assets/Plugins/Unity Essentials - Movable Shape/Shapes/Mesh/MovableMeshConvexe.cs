using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableMeshConvexe : MovableShape
    {
        public override void Construct()
        {

        }

        public override void Actualize()
        {
            Move(transform.position,
                transform.rotation,
                transform.localScale);
        }

#if UNITY_EDITOR
        public override void Draw()
        {

        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 localScale)
        {

        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (false);
        }
        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            return (false);
        }
    }
}