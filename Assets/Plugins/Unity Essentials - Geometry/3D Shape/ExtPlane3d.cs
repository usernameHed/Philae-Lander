using UnityEssentials.Geometry.shape2d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.PropertyAttribute.ReadOnly;
using UnityEssentials.Geometry.extensions;
using UnityEssentials.Geometry.GravityOverride;

namespace UnityEssentials.Geometry.shape3d
{

    /// <summary>
    /// a 3D Plane
    /// </summary>
    [Serializable]
    public struct ExtPlane3d
    {
        [SerializeField, ReadOnly] private Vector3 _position;       public Vector3 Position { get { return (_position); } }
        [SerializeField, ReadOnly] private Quaternion _rotation;    public Quaternion Rotation { get { return (_rotation); } }
        
        [SerializeField] private ExtPlane _plane;
        public bool AllowBottom { get { return (_plane.AllowBottom); }}
        public Vector3 Normal { get { return (_plane.Normal); } }

        [SerializeField, ReadOnly] private Matrix4x4 _planeMatrix;

        public ExtPlane3d(Vector3 position, Quaternion rotation) : this()
        {
            _position = position;
            _rotation = rotation;

            UpdateMatrix();
        }

        public ExtPlane3d(Vector3 position, Vector3 normal) : this()
        {
            _position = position;
            _rotation = ExtRotation.QuaternionFromVectorDirector(normal, Vector3.up);
            _rotation = ExtRotation.RotateQuaternion(_rotation, new Vector3(-90, 0, 0));
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _planeMatrix = ExtMatrix.GetMatrixTRS(_position, _rotation, Vector3.one);
            _plane.MoveShape(_position, _planeMatrix.UpFast());
        }

        public void Draw(Color color, bool drawFace = false, bool drawPoints = false)
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DrawLocalPlane3d(this, color, drawFace, drawPoints, true);
#endif
        }

        public void MoveSphape(Vector3 position, Quaternion rotation)
        {
            _position = position;
            _rotation = rotation;
            UpdateMatrix();
        }

        /// <summary>
        /// return true if the position is inside the sphape
        ///     2 ------------- 3 
        ///   /               /   
        ///  1 ------------ 4     
        /// 
        /// </summary>
        public bool IsInsideShape(Vector3 k)
        {
            if (!_plane.AllowBottom && !_plane.IsAbove(k))
            {
                return (false);
            }
            return (true);
        }

        /// <summary>
        /// return closest point IF point is not bellow with the settings set to false
        /// </summary>
        /// <param name="k"></param>
        /// <param name="canApplyGravity"></param>
        /// <returns></returns>
        public bool GetClosestPoint(Vector3 k, ref Vector3 closestPoint)
        {
            closestPoint = Vector3.zero;
            if (!_plane.AllowBottom && !_plane.IsAbove(k))
            {
                return (false);
            }
            closestPoint = ExtPlane.ProjectPointInPlane(_plane, k);
            return (true);
        }
    }

}
