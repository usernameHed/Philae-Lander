using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Spline
{
    /// <summary>
    /// 
    /// </summary>
    public class BranchBetweenSplines : MonoBehaviour
    {
        [SerializeField] private SplineBase _splineA;
        [SerializeField] private SplineBase _splineB;
        [SerializeField] private SplineBase _generatedSpline; public SplineBase SplineBase { get { return (_generatedSpline); } }
        [SerializeField] private float _pathPositionA = 1;
        [SerializeField] private float _pathPositionB = 0;
        [SerializeField, Range(0f, 1f)] private float _tangentPercent = 0.5f;

        /// <summary>How to interpret the Path Position</summary>
        [SerializeField, Tooltip("How to interpret Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.")]
        protected SplineBase.PositionUnits _positionUnits = SplineBase.PositionUnits.Normalized;

        public static SplineBase GenerateBranch(SplineBase splineA, SplineBase splineB, float from, float to, SplineBase.PositionUnits positionUnits)
        {
            BranchBetweenSplines branch = GameObject.Instantiate(Resources.Load<BranchBetweenSplines>("BRANCH_SPLINE"));
            branch.Construct(splineA, splineB, from, to, positionUnits);
            return (branch.SplineBase);
        }

        public void Construct(SplineBase splineA, SplineBase splineB, float from, float to, SplineBase.PositionUnits positionUnits)
        {
            _splineA = splineA;
            _splineB = splineB;
            _pathPositionA = from;
            _pathPositionB = to;
            _positionUnits = positionUnits;
            ConstructSpline();
        }

        private Vector3 GetPosA()
        {
            Vector4 pos = _splineA.EvaluatePositionAtUnit(_pathPositionA, _positionUnits);
            return (pos);
        }
        private Vector4 GetPosB()
        {
            Vector4 pos = _splineB.EvaluatePositionAtUnit(_pathPositionB, _positionUnits);
            return (pos);
        }

        private float GetRollA()
        {
            Quaternion rotation = _splineA.EvaluateOrientationAtUnit(_pathPositionA, _positionUnits);
            float roll = rotation.eulerAngles.z;
            if (roll > 180)    //to rework better
            {
                roll -= 360;
            }
            return (roll);
        }

        private float GetRollB()
        {
            Quaternion rotation = _splineB.EvaluateOrientationAtUnit(_pathPositionB, _positionUnits);
            float roll = rotation.eulerAngles.z;
            if (roll > 180)    //to rework better
            {
                roll -= 360;
            }
            return (roll);
        }

        private Vector3 GetForwardA()
        {
            Quaternion rotation = _splineA.EvaluateOrientationAtUnit(_pathPositionA, _positionUnits);
            return (rotation * Vector3.forward);
        }

        private Vector3 GetForwardB()
        {
            Quaternion rotation = _splineB.EvaluateOrientationAtUnit(_pathPositionB, _positionUnits);
            return (rotation * Vector3.forward);
        }

        public void ConstructSpline()
        {
            if (_splineA == null || _splineB == null)
            {
                return;
            }

            Vector3 A = GetPosA();
            Vector3 B = GetPosB();

            Vector3 Aforward = GetForwardA();
            Vector3 Bforward = GetForwardB();

            Aforward *= (A - B).magnitude / 1 * _tangentPercent;
            Bforward *= (A - B).magnitude / 1 * _tangentPercent;
            _generatedSpline.CreateSpline(new Vector7(A, Aforward, GetRollA()), new Vector7(B, Bforward, GetRollB()));
        }
    }
}