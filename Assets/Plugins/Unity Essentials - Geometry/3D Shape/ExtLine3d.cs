using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.extensions;
using UnityEssentials.Geometry.GravityOverride;

namespace UnityEssentials.Geometry.shape2d
{
    /// <summary>
    /// 
    ///     1
    ///     |
    ///     |
    ///     |
    ///     |
    ///     2
    /// 
    /// a 3D line, with 2 points
    /// </summary>
    [Serializable]
    public struct ExtLine3d
    {
        [SerializeField]
        private Vector3 _position;
        public Vector3 Position { get { return (_position); } }
        [SerializeField]
        private Quaternion _rotation;
        public Quaternion Rotation { get { return (_rotation); } }
        [SerializeField]
        private Vector3 _localScale;
        public Vector3 LocalScale { get { return (_localScale); } }

        public Vector3 P1 { get { return (_line.P1); } }
        public Vector3 P2 { get { return (_line.P2); } }

        [SerializeField]
        private ExtLine _lineLocalPosition;

        [SerializeField]
        private ExtLine _line;
        [SerializeField]
        private Matrix4x4 _linesMatrix;

        public ExtLine3d(Vector3 position, Quaternion rotation, Vector3 localScale) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;

            _lineLocalPosition = new ExtLine(new Vector3(0, 0, 0.4f), new Vector3(0, 0, -0.4f));
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _linesMatrix = Matrix4x4.TRS(_position, _rotation, _localScale);
            UpdateGlobalLineFromLocalOnes();
        }

        private void UpdateGlobalLineFromLocalOnes()
        {
            _line.MoveShape(
                _linesMatrix.MultiplyPoint3x4(_lineLocalPosition.P1),
                _linesMatrix.MultiplyPoint3x4(_lineLocalPosition.P2));
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            UpdateMatrix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="nullIfOutside"></param>
        /// <returns></returns>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            return (_line.GetClosestPoint(k));
        }

        public bool GetClosestPointIfWeCan(Vector3 k, out Vector3 closestPoint, GravityOverrideLineTopDown gravityOverride)
        {
            return (_line.GetClosestPointIfWeCan(k, out closestPoint, gravityOverride));
        }

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            Debug.DrawLine(_line.P1, _line.P2, color);
        }

        public void DrawWithOffset(Color color, Vector3 offset)
        {
            Debug.DrawLine(_line.P1 + offset, _line.P2 + offset, color);
        }

        public void DrawWithExtraSize(Color color, float offset, float size = 0.1f)
        {
            ExtDrawGuizmos.DebugCapsuleFromInsidePoint(_line.P1, _line.P2, color, offset);
            ExtDrawGuizmos.DebugSphere(_line.P1, color, size);
            ExtDrawGuizmos.DebugSphere(_line.P2, color, size);

        }
#endif

    }

}
