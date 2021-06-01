using System;
using UnityEngine;
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
    [AddComponentMenu("Unity Essentials/Spline/Controller/Offset From Other Sticker")]
    public class ControllerStickOffset : ControllerStickBase
    {
        [SerializeField, NoNull] protected ControllerStick _targetStick; public ControllerStick TargetStick { get { return (_targetStick); } }
        [SerializeField] private float _offsetFromTarget; public float OffsetFromTarget { get { return (_offsetFromTarget); } }
        [SerializeField] private bool _forecastIfReachBeyond = false;

        private FrequencyChrono _lerpWhenClosingBranch = new FrequencyChrono();
        private float _timeToReachWantedPositionAfterClosingBranch = 0.2f;

        public override float PathPosition
        {
            get
            {
                if (_manageBranch)
                {
                    if (_otherStick == null)
                    {
                        _otherStick = _targetStick as SplineControllerMove;
                    }
                    ManageSplineChange();
                    if (_isOnDifferentSpline)
                    {
                        return (_lastPathPosition);
                    }
                }
                return (RightPathPosition());
            }
            set
            {
                _targetStick.PathPosition = value;
            }
        }

        public float PathPositionTarget { get { return (_targetStick.PathPosition); } }
        public override float ClampedPathPosition
        {
            get
            {
                if (_manageBranch)
                {
                    if (_otherStick == null)
                    {
                        _otherStick = _targetStick as SplineControllerMove;
                    }
                    ManageSplineChange();
                    if (_isOnDifferentSpline)
                    {
                        return (_lastPathPosition);
                    }
                }
                return (RightClampedPositon());
            }
            set
            {
                _targetStick.ClampedPathPosition = value;
            }
        }

        public override SplineBase.PositionUnits PositionUnits { get { return (_targetStick.PositionUnits); } set { _targetStick.PositionUnits = value; } }

        [SerializeField] private bool _manageBranch = true;


        private float _lastPathPosition;
        private bool _isOnDifferentSpline = false;
        private SplineControllerMove _otherStick;

        private void OnEnable()
        {
            _otherStick = _targetStick as SplineControllerMove;
        }

        protected float RightPathPosition()
        {
            float offset = _targetStick.PathPosition + _offsetFromTarget;
            float max = _targetStick.SplineBase.MaxUnit(_targetStick.PositionUnits);
            if (_loop)
            {
                if (offset >= max)
                {
                    offset %= max;
                }
                if (offset < 0)
                {
                    offset += max;
                }
            }
            return (offset);
        }

        protected float RightClampedPositon()
        {
            float offset = _targetStick.ClampedPathPosition + _offsetFromTarget;
            float max = _targetStick.SplineBase.MaxUnit(_targetStick.PositionUnits);
            if (_loop)
            {
                if (offset >= max)
                {
                    offset %= max;
                }
                if (offset < 0)
                {
                    offset += max;
                }
            }
            return (offset);
        }

        public override void ChangeSpline(SplineBase newSpline, float newPosition)
        {
            base.ChangeSpline(newSpline, newPosition);
            ManageSplineChange();
            if (_isOnDifferentSpline)
            {
                _lastPathPosition = newPosition;
            }
        }

        public void ChangeOffsetFromSpline(Vector2 offset)
        {
            _offsetFromSpline = offset;
        }

        private void ManageSplineChange()
        {
            if (_manageBranch)
            {
                if (_otherStick == null)
                {
                    return;
                }
                if (SplineBase != _otherStick.SplineBase)
                {
                    if (!_isOnDifferentSpline)
                    {
                        _isOnDifferentSpline = true;
                        _lerpWhenClosingBranch.Reset();
                    }
                }
                else
                {
                    if (_isOnDifferentSpline)
                    {
                        Debug.Log("lerp toward wanted position  ?");
                        _lerpWhenClosingBranch.StartChrono(_timeToReachWantedPositionAfterClosingBranch, false);
                    }
                    _isOnDifferentSpline = false;
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (_manageBranch && _otherStick == null)
            {
                _otherStick = _targetStick as SplineControllerMove;
            }

            ManageSplineChange();

            if (SplineBase != _otherStick.SplineBase)
            {
                _lastPathPosition += _otherStick.SpeedMove * Time.fixedDeltaTime;

                BasicPositionAndOffset(_lastPathPosition);
                BasicRotationAndOffset(_lastPathPosition);
            }
            //else if (_lerpWhenClosingBranch.IsRunning())
            //{
            //    CalculateOffset(out float offset, out float max);
            //    _lastPathPosition = Mathf.Lerp(_lastPathPosition, offset, _lerpWhenClosingBranch.GetCurrentPercentFromTheEnd());
            //}
            else
            {
                AttemptToStick();
            }
        }

        public virtual void AttemptToStick()
        {
            CalculateOffset(out float offset, out float max);

            UpdateRotation();
            UpdatePosition(offset, max);

            _lastPathPosition = offset;
        }

        public void UpdatePosition(float offset, float max)
        {
            BasicPositionAndOffset(offset);
            if (_forecastIfReachBeyond && !_loop)
            {
                if (offset >= max)
                {
                    _toMove.position += _toMove.forward * _targetStick.SplineBase.FromUnitDistanceToReal3DDistance(offset - max, _targetStick.PositionUnits);
                }
                else if (offset < 0)
                {
                    _toMove.position -= _toMove.forward * _targetStick.SplineBase.FromUnitDistanceToReal3DDistance(-offset, _targetStick.PositionUnits);
                }
            }
        }

        private void CalculateOffset(out float offset, out float max)
        {
            offset = _targetStick.PathPosition + _offsetFromTarget;
            max = _targetStick.SplineBase.MaxUnit(_targetStick.PositionUnits);
            if (_loop)
            {
                if (offset >= max)
                {
                    offset %= max;
                }
                if (offset < 0)
                {
                    offset += max;
                }
            }
        }

        public override void UpdateRotation()
        {
            if (_rotateAlongSpline)
            {
                CalculateOffset(out float offset, out float max);
                BasicRotationAndOffset(offset);
            }
            ApplyKeepUp();
        }
    }
}
