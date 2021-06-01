using System;
using UnityEngine;
using UnityEssentials.Geometry.extensions;
using UnityEssentials.Geometry.shape2d;
using UnityEssentials.Geometry.PropertyAttribute.ReadOnly;

namespace UnityEssentials.Geometry.shape3d
{
    [Serializable]
    public struct ExtHalfSphere
    {
        [SerializeField]
        private float _radius;
        public float Radius { get { return (_radius); } }
        [SerializeField, ReadOnly] private Vector3 _position;  public Vector3 Position { get { return (_position); } }
        [SerializeField, ReadOnly] private Quaternion _rotation; public Quaternion Rotation { get { return (_rotation); } }
        [SerializeField, ReadOnly] private Vector3 _localScale; public Vector3 LocalScale { get { return (_localScale); } }

        [SerializeField, ReadOnly] private float _realRadius;        public float RealRadius { get { return (_realRadius); } }
        [SerializeField, ReadOnly] private ExtCircle _circle;

        public ExtHalfSphere(Vector3 position, float radius) : this()
        {
            MoveSphape(position, Quaternion.identity, Vector3.one, radius);
        }

        public ExtHalfSphere(Vector3 position, Quaternion rotation, Vector3 scale, float radius) : this()
        {
            MoveSphape(position, rotation, scale, radius);
        }

        public void Draw(Color color)
        {
            ExtDrawGuizmos.DrawHalfWireSphere(Position, Rotation * Vector3.up, color, _realRadius);
        }

        public void DrawWithExtraRadius(Color color, float radius)
        {
            ExtDrawGuizmos.DrawHalfWireSphere(Position, Rotation * Vector3.up, color, _realRadius + radius);
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 scale, float radius)
        {
            _position = position;
            _rotation = rotation;
            _localScale = scale;
            _radius = radius;
            _realRadius = radius * LocalScale.Maximum();
            _circle.MoveSphape(_position, _rotation * Vector3.up, _realRadius);
        }

        public void MoveSphape(Vector3 position, float radius)
        {
            _position = position;
            _radius = radius;
            _realRadius = radius * LocalScale.Maximum();
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = scale;
            _realRadius = _radius * LocalScale.Maximum();
            _circle.MoveSphape(_position, _rotation * Vector3.up, _realRadius);
        }

        /// <summary>
        /// return true if the position is inside the sphape
        /// </summary>
        /// <param name="otherPosition">position to test</param>
        /// <returns>true if inside the shape</returns>
        public bool IsInsideShape(Vector3 otherPosition)
        {
            if (_circle.IsAbove(otherPosition))
            {
                return (ExtVector3.Distance(otherPosition, Position) <= _realRadius);
            }
            return (false);
        }

        public Vector3 GetClosestPoint(Vector3 k)
        {
            if (_circle.IsAbove(k))
            {
                return (Position + ((k - Position).FastNormalized() * _realRadius));
            }
            Vector3 closestPoint = Vector3.zero;
            _circle.GetClosestPointOnDisc(k, ref closestPoint);
            return (closestPoint);
        }
    }
}