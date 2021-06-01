using System;
using UnityEngine;
using UnityEssentials.Geometry.extensions;
using UnityEssentials.Geometry.PropertyAttribute.ReadOnly;

namespace UnityEssentials.Geometry.shape3d
{
    [Serializable]
    public struct ExtSphere
    {
        [SerializeField]
        private float _radius;
        public float Radius { get { return (_radius); } }
        [SerializeField, ReadOnly] private Vector3 _position;  public Vector3 Position { get { return (_position); } }
        [SerializeField, ReadOnly] private Vector3 _localScale; public Vector3 LocalScale { get { return (_localScale); } }

        [SerializeField, ReadOnly]
        private float _realRadius;
        public float RealRadius { get { return (_realRadius); } }

        public ExtSphere(Vector3 position, float radius) : this()
        {
            MoveSphape(position, Vector3.one, radius);
        }

        public ExtSphere(Vector3 position, Vector3 scale, float radius) : this()
        {
            MoveSphape(position, scale, radius);
        }

        public void Draw(Color color)
        {
            ExtDrawGuizmos.DebugWireSphere(Position, color, _realRadius);
        }

        public void DrawWithExtraRadius(Color color, float radius)
        {
            ExtDrawGuizmos.DebugWireSphere(Position, color, _realRadius + radius);
        }

        public void MoveSphape(Vector3 position, Vector3 scale, float radius)
        {
            _position = position;
            _localScale = scale;
            _radius = radius;
            _realRadius = radius * LocalScale.Maximum();
        }

        public void MoveSphape(Vector3 position, float radius)
        {
            _position = position;
            _radius = radius;
            _realRadius = radius * LocalScale.Maximum();
        }

        public void MoveSphape(Vector3 position, Vector3 scale)
        {
            _position = position;
            _localScale = scale;
            _realRadius = _radius * LocalScale.Maximum();
        }

        /// <summary>
        /// return true if the position is inside the sphape
        /// </summary>
        /// <param name="otherPosition">position to test</param>
        /// <returns>true if inside the shape</returns>
        public bool IsInsideShape(Vector3 otherPosition)
        {
            return (ExtVector3.Distance(otherPosition, Position) <= _realRadius);
        }

        public Vector3 GetClosestPoint(Vector3 k)
        {
            return (Position + ((k - Position).FastNormalized() * _realRadius));
        }
    }
}