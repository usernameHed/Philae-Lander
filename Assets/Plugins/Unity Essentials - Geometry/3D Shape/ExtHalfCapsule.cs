using UnityEssentials.Geometry.shape2d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.extensions;
using UnityEssentials.Geometry.GravityOverride;
using UnityEssentials.Geometry.PropertyAttribute.ReadOnly;

namespace UnityEssentials.Geometry.shape3d
{
    /// <summary>
    /// Capsule 
    ///       _3_
    ///    /       \
    ///   /  -----  \
    ///  |/         \|
    ///  |\    1    /|
    ///  | --_____-- |
    ///  |           |
    ///  |           |
    ///  |           |
    ///  |           |
    ///  |           |
    ///  |           |
    ///  \     2     /
    ///    --_____-- 
    ///    
    /// </summary>
    [Serializable]
    public struct ExtHalfCapsule
    {
        #region Capsule Serialized Variables cached
        [SerializeField, ReadOnly] private Vector3 _position;
        public Vector3 Position { get { return (_position); } }
        [SerializeField, ReadOnly] private Quaternion _rotation;
        public Quaternion Rotation { get { return (_rotation); } }
        [SerializeField, ReadOnly] private Vector3 _localScale;
        public Vector3 LocalScale { get { return (_localScale); } }

        [SerializeField, ReadOnly] private ExtSphere _topSphere;
        [SerializeField, ReadOnly] private ExtCircle _bottomCircle;

        [SerializeField] private float _radius;
        public float Radius { get { return (_radius); } }
        private float _radiusSquared;
        public float RadiusSquared { get { return (_radiusSquared); } }
        [SerializeField] private float _lenght;
        public float Lenght { get { return (_lenght); } }
        [SerializeField, ReadOnly] private float _lenghtSquared;
        public float LenghtSquared { get { return (_lenghtSquared); } }
        [SerializeField, ReadOnly] private float _realRadius;
        [SerializeField, ReadOnly] private float _realSquaredRadius;
        public float RealRadius { get { return (_realRadius); } }
        [SerializeField, ReadOnly] private Matrix4x4 _capsuleMatrix;
        [SerializeField, ReadOnly] private Vector3 _p1;
        public Vector3 P1 { get { return (_p1); } }
        [SerializeField, ReadOnly] private Vector3 _p2;
        public Vector3 P2 { get { return (_p2); } }
        [SerializeField, ReadOnly] private Vector3 _delta;
        [SerializeField, ReadOnly] private float _deltaSquared;
        #endregion



        public ExtHalfCapsule(Vector3 position,
            Quaternion rotation,
            Vector3 localScale,
            float radius,
            float lenght) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _radius = radius;
            _lenght = lenght;
            _lenghtSquared = _lenght * _lenght;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;

            UpdateMatrix();            
        }

        public ExtHalfCapsule(Vector3 p1, Vector3 p2, float radius = 0.25f) : this()
        {
            _position = ExtVector3.GetMeanOfXPoints(p1, p2);
            _rotation = ExtQuaternion.QuaternionFromLine(p1, p2, Vector3.up);
            _rotation = ExtRotation.RotateQuaternion(_rotation, new Vector3(90, 0, 0));

            _localScale = new Vector3(1, 1, 1);

            _capsuleMatrix = Matrix4x4.TRS(_position, _rotation, _localScale * 1);

            //why radius a 0.25, and lenght * 0.8 ?? I don't know,
            //it's there to match the first constructor(position, rotation, scale)
            _radius = radius;
            _lenght = ExtVector3.Distance(p2, p1) * 0.8f;

            _lenghtSquared = _lenght * _lenght;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;

            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _capsuleMatrix = Matrix4x4.TRS(_position, _rotation, _localScale * _radius);
            Vector3 size = new Vector3(0, _lenght / 2, 0);
            _p1 = _capsuleMatrix.MultiplyPoint3x4(Vector3.zero - ((-size)));
            _p2 = _capsuleMatrix.MultiplyPoint3x4(Vector3.zero + ((-size)));
            _delta = _p2 - _p1;
            _deltaSquared = Vector3.Dot(_delta, _delta);

            _topSphere.MoveSphape(_p1, _realRadius);
            _bottomCircle.MoveSphape(_p2, _capsuleMatrix.DownFast(), _realRadius);
        }

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            ExtDrawGuizmos.DebugHalfCapsuleFromInsidePoint(_p1, _p2, color, _realRadius);
        }

        public void DrawWithExtraSize(Color color, Vector3 extraSize)
        {
            if (extraSize.Maximum() <= 0f)
            {
                return;
            }

            Matrix4x4 cylinderMatrix = Matrix4x4.TRS(_position, _rotation, (_localScale + extraSize) * _radius);
            Vector3 size = new Vector3(0, _lenght / 2, 0);
            Vector3 p1 = cylinderMatrix.MultiplyPoint3x4(Vector3.zero + ((-size)));
            Vector3 p2 = cylinderMatrix.MultiplyPoint3x4(Vector3.zero - ((-size)));
            float realRadius = _radius * MaxXY(_localScale + extraSize);

            ExtDrawGuizmos.DebugHalfCapsuleFromInsidePoint(p2, p1, color, _realRadius);
        }
#endif

        private float MaxXY(Vector3 size)
        {
            return (Mathf.Max(size.x, size.z));
        }


        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale, float radius, float lenght)
        {
            _radius = radius;
            _lenght = lenght;
            _lenghtSquared = _lenght * _lenght;
            MoveSphape(position, rotation, localScale);
        }

        public void ChangeRadius(float radius)
        {
            _radius = radius;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public void ChangeLenght(float lenght)
        {
            _lenght = lenght;
            _lenghtSquared = _lenght * _lenght;
            UpdateMatrix();
        }

        /// <summary>
        /// return true if the position is inside the sphape
        /// </summary>
        public bool IsInsideShape(Vector3 pointToTest)
        {
            bool isInsideTopSphere = _topSphere.IsInsideShape(pointToTest);
            if (isInsideTopSphere)
            {
                return (true);
            }
            return (IsInsideTrunk(pointToTest));
        }

        private bool IsInsideTrunk(Vector3 k)
        {
            Vector3 pDir = k - _p1;
            float dot = Vector3.Dot(_delta, pDir);

            if (dot < 0f || dot > _deltaSquared)
            {
                return (false);
            }

            float dsq = pDir.x * pDir.x + pDir.y * pDir.y + pDir.z * pDir.z - dot * dot / _deltaSquared;

            if (dsq > _realSquaredRadius)
            {
                return (false);
            }
            else
            {
                return (true);
            }
        }


        
        /// <summary>
        /// Return the closest point on the surface of the capsule
        ///   
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            float dist = Vector3.Dot(k - _p1, _delta);

            //k projection is outside the [_p1, _p2] interval, closest to _p1
            if (dist <= 0.0f)
            {
                return (_topSphere.GetClosestPoint(k));
            }
            //k projection is outside the [_p1, p2] interval, closest to _p2
            else if (dist >= _deltaSquared)
            {
                Vector3 closestPoint = Vector3.zero;
                bool canApplyGravity = _bottomCircle.GetClosestPointOnDisc(k, ref closestPoint);
                if (!canApplyGravity)
                {
                    throw new Exception("can't be possible, see AllowBottom for more information");
                }
                return (closestPoint);
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                dist = dist / _deltaSquared;
                Vector3 pointOnLine = _p1 + dist * _delta;
                Vector3 pointOnSurfaceLine = pointOnLine + ((k - pointOnLine).FastNormalized() * _realRadius);
                return (pointOnSurfaceLine);
            }
        }

        public float GetDistanceFromPoint(Vector3 k)
        {
            return (GetClosestPoint(k).magnitude);
        }

        public bool GetClosestPointIfWeCan(Vector3 k, GravityOverrideLineTopDown gravityOverride, ref Vector3 closestPoint)
        {
            closestPoint = Vector3.zero;
            if (!gravityOverride.CanApplyGravity)
            {
                return (false);
            }

            float dist = Vector3.Dot(k - _p1, _delta);

            //k projection is outside the [_p1, _p2] interval, closest to _p1
            if (dist <= 0.0f)
            {
                if (!gravityOverride.Top)
                {
                    return (false);
                }
                closestPoint = _topSphere.GetClosestPoint(k);
                return (true);
            }
            //k projection is outside the [_p1, p2] interval, closest to _p2
            else if (dist >= _deltaSquared)
            {
                if (!gravityOverride.Bottom)
                {
                    return (false);
                }
                return (_bottomCircle.GetClosestPointOnDisc(k, ref closestPoint));
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                if (!gravityOverride.Trunk)
                {
                    return (false);
                }

                dist = dist / _deltaSquared;
                Vector3 pointOnLine = _p1 + dist * _delta;
                closestPoint = pointOnLine + ((k - pointOnLine).FastNormalized() * _realRadius);
                return (true);
            }
        }
        

        //end class
    }
    //end nameSpace
}