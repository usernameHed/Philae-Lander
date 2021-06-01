using System;
using UnityEngine;

namespace UnityEssentials.Spline.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Controller/Focus Target"), ExecuteInEditMode]
    public class ControllerFocusTarget : ControllerStick
    {
        [Serializable]
        public struct AutoCalculate
        {
            /// <summary>Offset, in current position units, from the closest point on the path to the follow target.</summary>
            [Tooltip("Offset, in current position units, from the closest point on the path to the follow target")]
            public float PositionOffset;

            /// <summary>Search up to how many waypoints on either side of the current position.  Use 0 for Entire path</summary>
            [Tooltip("Search up to how many waypoints on either side of the current position.  Use 0 for Entire path.")]
            public int SearchRadius;

            /// <summary>We search between waypoints by dividing the segment into this many straight pieces.
            /// The higher the number, the more accurate the result, but performance is
            /// proportionally slower for higher numbers</summary>
            [Tooltip("We search between waypoints by dividing the segment into this many straight pieces.  The higher the number, the more accurate the result, but performance is proportionally slower for higher numbers")]
            public int SearchResolution;

            /// <summary>Constructor with specific field values</summary>
            public AutoCalculate(float positionOffset, int searchRadius, int stepsPerSegment)
            {
                PositionOffset = positionOffset;
                SearchRadius = searchRadius;
                SearchResolution = stepsPerSegment;
            }
        };

        [SerializeField] protected Transform _target;
        [SerializeField] protected bool _lookAtTarget = true;
        [SerializeField] protected Vector3 _up = Vector3.up;

        /// <summary>Controls how automatic calculation occurs</summary>
        [SerializeField, Tooltip("Controls how automatic calculation occurs.  A Follow target is necessary to use this feature.")]
        protected AutoCalculate _auto = new AutoCalculate(0, 2, 5);


#if UNITY_EDITOR
        /// <summary>
        /// call this only in editor to stick this object to the spline.
        /// It's used here because there is no way of having an editor
        /// for an unselected gameObject... sorry for that
        /// </summary>
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }
            AttemptToFocus();
        }
#endif

        protected override void FixedUpdate()
        {
            AttemptToFocus();
        }

        private void AttemptToFocus()
        {
            if (_toMove == null || _target == null)
            {
                this.enabled = false;
                return;
            }
            AutoCalculatePositionBasedOnTarget();
            Stick();
        }

        public virtual void AutoCalculatePositionBasedOnTarget()
        {
            float prevPos = _spline.ToNativePathUnits(_pathPosition, _positionUnits);
            // This works in path units
            _pathPosition = _spline.FindClosestPoint(
                _target.position,
                Mathf.FloorToInt(prevPos),
                (Time.deltaTime < 0 || _auto.SearchRadius <= 0)
                    ? -1 : _auto.SearchRadius,
                _auto.SearchResolution);
            _pathPosition = _spline.FromPathNativeUnits(_pathPosition, _positionUnits);

            // Apply the path position offset
            _pathPosition += _auto.PositionOffset;
        }

        public override void Stick()
        {
            _toMove.position = _spline.EvaluatePositionAtUnit(_pathPosition, _positionUnits);
            if (_lookAtTarget)
            {
                _toMove.LookAt(_target, _up);
            }
            else if (_rotateAlongSpline)
            {
                _toMove.rotation = _spline.EvaluateOrientationAtUnit(_pathPosition, _positionUnits);
            }
        }
    }
}
