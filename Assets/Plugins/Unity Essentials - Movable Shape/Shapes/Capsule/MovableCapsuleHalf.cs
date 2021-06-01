using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableCapsuleHalf : MovableShape
    {
        public ExtHalfCapsule CapsuleHalf;

        public override void Construct()
        {
            CapsuleHalf = new ExtHalfCapsule(transform.position,
                transform.rotation,
                transform.lossyScale,
                0.5f,
                2f);
        }

        public override void Actualize()
        {
            Move(transform.position,
                transform.rotation,
                transform.lossyScale);
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            CapsuleHalf.Draw(base.GetColor());
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 lossyScale)
        {
            CapsuleHalf.MoveSphape(newPosition, rotation, lossyScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (CapsuleHalf.IsInsideShape(position));
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            closestPoint = CapsuleHalf.GetClosestPoint(position);
            return (true);
        }
    }
}