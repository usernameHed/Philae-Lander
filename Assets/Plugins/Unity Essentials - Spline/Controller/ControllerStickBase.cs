using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEssentials.Spline.ControllerExtensions.EventsTrigger;
using UnityEssentials.Spline.Extensions;
using UnityEssentials.Spline.PropertyAttribute.DrawIf;
using UnityEssentials.Spline.PropertyAttribute.NoNull;
using UnityEssentials.Spline.PropertyAttribute.ReadOnly;

namespace UnityEssentials.Spline.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ControllerStickBase : MonoBehaviour
    {
        [Header("Base Settings")]
        [SerializeField, NoNull] protected Transform _toMove;
        [SerializeField, NoNull] protected SplineBase _spline; public SplineBase SplineBase { get { return (_spline); } }

        public abstract float PathPosition { get; set; }
        public abstract float ClampedPathPosition { get; set; }
        public abstract SplineBase.PositionUnits PositionUnits { get; set; }

        [SerializeField] protected bool _loop = true; public bool Loop { get { return (_loop); } }
        [SerializeField] protected Vector2 _offsetFromSpline = Vector2.zero;
        [SerializeField] protected float _dampingTimePosition = 0.1f;
        [SerializeField] protected float _dampingTimeRotation = 0.1f;

        [Header("Rotate Settings"), Space(30)]
        /// <summary>
        /// if true, _toMove will face the direction of the spline
        /// </summary>
        [SerializeField] protected bool _rotateAlongSpline = true;
        public bool RotateAlongSpline { get { return (_rotateAlongSpline); } }
        [SerializeField, DrawIf(nameof(_rotateAlongSpline), true, ComparisonType.Equals)] protected Vector3 _offsetRotation = new Vector3(0, 0, 0);

        /// <summary>
        /// if true, _toMove will keep his UP rotation toward target up transform reference (if no transform given, toward vector up)
        /// </summary>
        [SerializeField] protected bool _keepUp = false;
        public bool KeepUp { get { return (_keepUp); } }
        /// <summary>
        /// up default reference
        /// </summary>
        [SerializeField, DrawIf(nameof(_keepUp), true, ComparisonType.Equals)] protected Vector3 _upReference = Vector3.up;
        /// <summary>
        /// if filled, use this as default up
        /// </summary>
        [SerializeField, DrawIf(nameof(_keepUp), true, ComparisonType.Equals)] protected Transform _upReferenceObject = default;
        /// <summary>
        /// if true, the up represent the direction current - target (useful for Mario galaxy like stuffs
        /// </summary>
        [SerializeField] protected bool _keepUpGalaxyOriented = false;


        public delegate void SplineChanged(SplineBase spline);
        public event SplineChanged SplineHasChanged;
        public delegate void Looped();
        public Looped OnLooped;
        protected Vector3 _refPositionVelocity = Vector3.zero;
        protected Quaternion _refRotationVelocity = Quaternion.identity;
        protected Quaternion _refRotationUpVelocity = Quaternion.identity;

        public virtual void ChangeSpline(SplineBase newSpline, float newPosition)
        {
            _spline = newSpline;
            SplineHasChanged?.Invoke(_spline);
        }
        //public static void GenerateAndSwitchBranch(ControllerStick stick, SplineBase splineA, SplineBase splineB, float from, float to, SplineBase.PositionUnits positionUnits)
        //{
        //    SplineBase generatedSpline = BranchBetweenSplines.GenerateBranch(splineA, splineB, from, to, positionUnits);
        //    stick.ChangeSpline(generatedSpline, 0);
        //}

        public void BasicPositionAndOffset(float offset)
        {
            Vector3 target = PositionWithoutOffsetFromSpline(offset)
                                            + _toMove.right * _offsetFromSpline.x + _toMove.up * _offsetFromSpline.y;

            _toMove.position = ExtVector3.OwnSmoothDamp(_toMove.position, target, ref _refPositionVelocity, _dampingTimePosition, Mathf.Infinity, Time.fixedDeltaTime);
        }

        public Vector3 PositionWithoutOffsetFromSpline(float offset)
        {
            return (SplineBase.EvaluatePositionAtUnit(offset, PositionUnits));
        }

        public virtual void UpdateRotation()
        {
            if (_rotateAlongSpline)
            {
                Quaternion target = _spline.EvaluateOrientationAtUnit(PathPosition, PositionUnits);
                target = target * Quaternion.Euler(_offsetRotation);
                _toMove.rotation = ExtRotation.OwnSmoothDamp(_toMove.rotation, target, ref _refRotationVelocity, _dampingTimeRotation);
            }
            ApplyKeepUp();
        }

        public void BasicRotationAndOffset(float offset)
        {
            Quaternion target = RotationWithoutOffsetFromSpline(offset);
            target = target * Quaternion.Euler(_offsetRotation);

            _toMove.rotation = ExtRotation.OwnSmoothDamp(_toMove.rotation, target, ref _refRotationVelocity, _dampingTimeRotation);
        }

        public Quaternion RotationWithoutOffsetFromSpline(float offset)
        {
            return (SplineBase.EvaluateOrientationAtUnit(offset, PositionUnits));
        }

        public virtual void ApplyKeepUp()
        {
            if (!_keepUp)
            {
                return;
            }
            Quaternion target;
            if (_upReferenceObject == null)
            {
                target = ExtRotation.TurretLookRotation(_toMove.rotation, _upReference);
            }
            else
            {
                if (_keepUpGalaxyOriented)
                {
                    target = ExtRotation.TurretLookRotation(_toMove.rotation, _toMove.position - _upReferenceObject.position);
                }
                else
                {
                    target = ExtRotation.TurretLookRotation(_toMove.rotation, _upReferenceObject.up);
                }
            }
            _toMove.rotation = ExtRotation.OwnSmoothDamp(_toMove.rotation, target, ref _refRotationUpVelocity, _dampingTimeRotation);
        }
    }
}
