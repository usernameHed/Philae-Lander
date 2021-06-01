using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableSphere : MovableShape
    {
        public ExtSphere Sphere;

        public override void Construct()
        {
            Sphere = new ExtSphere(transform.position, 0.5f);
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
            Sphere.Draw(base.GetColor());
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 lossyScale)
        {
            Sphere.MoveSphape(newPosition, lossyScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (Sphere.IsInsideShape(position));
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            closestPoint = Sphere.GetClosestPoint(position);
            return (true);
        }
    }
}