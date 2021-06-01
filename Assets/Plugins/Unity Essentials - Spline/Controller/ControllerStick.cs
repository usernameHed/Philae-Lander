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
    [AddComponentMenu("Unity Essentials/Spline/Controller/Stick")]
    public class ControllerStick : ControllerStickBase
    {
        /// <summary>The position along the path at which the camera will be placed.
        /// This can be animated directly, or set automatically by the Auto feature
        /// to get as close as possible to the Follow target.</summary>
        [SerializeField, Tooltip("The position along the path at which the camera will be placed.  This can be animated directly, or set automatically by the Auto feature to get as close as possible to the Follow target.  The value is interpreted according to the Position Units setting.")]
        protected float _pathPosition;
        public override float PathPosition { get { return (_pathPosition); } set { _pathPosition = value; } }
        public override float ClampedPathPosition { get { return (_clampedPathPosition); } set { _clampedPathPosition = value; } }

        private float _clampedPathPosition;

        /// <summary>How to interpret the Path Position</summary>
        [SerializeField, Tooltip("How to interpret Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.")]
        protected SplineBase.PositionUnits _positionUnits = SplineBase.PositionUnits.Distance;

        public override SplineBase.PositionUnits PositionUnits { get { return (_positionUnits); } set { _positionUnits = value; } }

        /// <summary>
        /// if true, controller will stay grounded to the last closest position, even if spline is moved / changed.
        /// </summary>
        [SerializeField] protected bool _anchorOnSplineChange = false; public bool AnchorOnSplineChange { get { return (_anchorOnSplineChange); } }
        //[SerializeField] protected bool _anchorOnSplineMoved = false; public bool AnchorOnSplineMoved { get { return (_anchorOnSplineMoved); } }


        [SerializeField, ReadOnly] protected Vector3 _lastPosition; public Vector3 LastPosition { get { return (_lastPosition); } }

        private bool _hasInvalidate;

        private void Awake()
        {
            if (_toMove == null)
            {
                return;
            }
            _lastPosition = _toMove.position;
        }

        private void OnEnable()
        {
            _clampedPathPosition = _pathPosition;
        }

        private void OnDisable()
        {

        }

        public override void ChangeSpline(SplineBase newSpline, float newPosition)
        {
            _pathPosition = newPosition;
            _clampedPathPosition = _pathPosition;
            base.ChangeSpline(newSpline, newPosition);
        }

        public void SetPercent(float percent)
        {
            _positionUnits = SplineBase.PositionUnits.Normalized;
            _pathPosition = percent;
            _clampedPathPosition = _pathPosition;
        }

        public void ChangeUnit(SplineBase.PositionUnits newUnit)
        {
            _positionUnits = newUnit;
            float closestPositionOnSpline = _spline.FindClosestPoint(transform.position, 0, -1, 10);
            _pathPosition = _spline.FromPathNativeUnits(closestPositionOnSpline, PositionUnits);
            _clampedPathPosition = _pathPosition;
        }

        /// <summary>
        /// set the path position, assuming we know the path type
        /// </summary>
        /// <param name="path"></param>
        public void SetPathPosition(float path)
        {
            _pathPosition = ModulateAmount(path);
            _clampedPathPosition = _pathPosition;
        }

        public virtual void Move(float amount)
        {
            _pathPosition += amount;
            _clampedPathPosition += amount;

            if (_clampedPathPosition >= _spline.MaxUnit(PositionUnits))
            {
                OnLooped?.Invoke();
            }
            _clampedPathPosition %= _spline.MaxUnit(PositionUnits);
            _pathPosition = ModulateAmount(_pathPosition);
        }

        public float ModulateAmount(float pos)
        {
            if (!_loop || _spline.Looped)
            {
                return (pos);
            }
            switch (_positionUnits)
            {
                case SplineBase.PositionUnits.Distance:
                    pos %= _spline.PathLength;
                    return (pos);
                case SplineBase.PositionUnits.Normalized:
                    pos %= 1f;
                    return (pos);
                case SplineBase.PositionUnits.PathUnits:
                default:
                    pos %= _spline.WaypointsCount;
                    return (pos);
            }
        }

        public void UpdatePositionAndRotation()
        {
            UpdateRotation();
            UpdatePosition();
        }

        public virtual void UpdatePosition()
        {
            Vector3 target = _spline.EvaluatePositionAtUnit(_pathPosition, _positionUnits)
                + _toMove.right * _offsetFromSpline.x + _toMove.up * _offsetFromSpline.y;

            _toMove.position = ExtVector3.OwnSmoothDamp(_toMove.position, target, ref _refPositionVelocity, _dampingTimePosition, Mathf.Infinity, Time.fixedDeltaTime);
        }

        protected virtual void FixedUpdate()
        {
            AttemptToStick();
        }

        public void AttemptToStick()
        {
            if (_toMove == null || _spline == null)
            {
                this.enabled = false;
                return;
            }
            Stick();
        }

        public virtual void Stick()
        {
            UpdatePosition();
            _lastPosition = _toMove.position;

            UpdateRotation();
        }
    }
}
