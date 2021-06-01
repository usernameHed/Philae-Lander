using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableDonut : MovableShape
    {
        public ExtDonut Donut;

        public override void Construct()
        {
            Donut = new ExtDonut(transform.position,
                transform.rotation,
                transform.lossyScale,
                0.5f,
                0.125f);
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
            Donut.Draw(base.GetColor());
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 lossyScale)
        {
            Donut.MoveSphape(newPosition, rotation, lossyScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (Donut.IsInsideShape(position));
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            closestPoint = Donut.GetClosestPoint(position);
            return (true);
        }
    }
}