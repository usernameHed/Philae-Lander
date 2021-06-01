using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableCapsule : MovableShape
    {
        public ExtCapsule Capsule;

        public override void Construct()
        {
            Capsule = new ExtCapsule(transform.position,
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
            Capsule.Draw(base.GetColor());
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 lossyScale)
        {
            Capsule.MoveSphape(newPosition, rotation, lossyScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (Capsule.IsInsideShape(position));
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            closestPoint = Capsule.GetClosestPoint(position);
            return (true);
        }
    }
}