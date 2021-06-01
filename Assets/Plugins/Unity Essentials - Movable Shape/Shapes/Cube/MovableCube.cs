using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovableCube : MovableShape
    {
        public ExtCube Cube;

        public override void Construct()
        {
            Cube = new ExtCube(transform.position,
                transform.rotation,
                transform.lossyScale);
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
            Cube.Draw(base.GetColor(), false, false);
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 lossyScale)
        {
            Cube.MoveSphape(newPosition, rotation, lossyScale);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (Cube.IsInsideShape(position));
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            closestPoint = Cube.GetClosestPoint(position);
            return (true);
        }
    }
}