using UnityEssentials.Geometry.shape3d;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.PropertyAttribute.ReadOnly;
using UnityEssentials.Geometry.shape2d;

namespace UnityEssentials.Geometry.MovableShape
{
    public class MovablePlane : MovableShape
    {
        [SerializeField, ReadOnly]
        private ExtPlane3d _plane3d;

        public override void Construct()
        {
            _plane3d = new ExtPlane3d(transform.position,
                transform.rotation);
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
            _plane3d.Draw(base.GetColor());
        }
#endif

        public override void Move(Vector3 newPosition, Quaternion rotation, Vector3 lossyScale)
        {
            _plane3d.MoveSphape(newPosition, rotation);
        }

        public override bool IsInsideShape(Vector3 position)
        {
            return (_plane3d.IsInsideShape(position));
        }

        public override bool GetClosestPoint(Vector3 position, ref Vector3 closestPoint)
        {
            return (_plane3d.GetClosestPoint(position, ref closestPoint));
        }
    }
}