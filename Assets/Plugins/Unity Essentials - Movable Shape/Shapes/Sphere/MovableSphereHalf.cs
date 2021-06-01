using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableSphereHalf : MovableShape
    {
        public ExtHalfSphere SphereHalf;

        public override void Construct()
        {
            SphereHalf = new ExtHalfSphere(transform.position,
                transform.rotation,
                transform.localScale,
                0.5f);
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
            SphereHalf.Draw(base.GetColor());
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 localScale)
        {
            SphereHalf.MoveSphape(newPosition, rotation, localScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (SphereHalf.IsInsideShape(position));
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            closestPoint = SphereHalf.GetClosestPoint(position);
            return (true);
        }
    }
}