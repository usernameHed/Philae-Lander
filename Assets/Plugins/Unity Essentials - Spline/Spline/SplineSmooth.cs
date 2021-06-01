using UnityEngine;
using System;

namespace UnityEssentials.Spline
{
    /// <summary>Defines a world-space path, consisting of an array of waypoints,
    /// each of which has position and roll settings.  Bezier interpolation
    /// is performed between the waypoints, to get a smooth and continuous path.
    /// The path will pass through all waypoints, and (unlike Spline) first 
    /// and second order continuity is guaranteed</summary>
    [AddComponentMenu("Unity Essentials/Spline/Spline Type/Spline Smooth")]
    public class SplineSmooth : SplineBase
    {
        /// <summary>If checked, then the path ends are joined to form a continuous loop</summary>
        [Tooltip("If checked, then the path ends are joined to form a continuous loop.")]
        public bool m_Looped;
        /// <summary>True if the path ends are joined to form a continuous loop</summary>
        public override bool Looped { get { return m_Looped; } }

        /// <summary>A waypoint along the path</summary>
        [Serializable] public struct Waypoint
        {
            /// <summary>Position in path-local space</summary>
            [Tooltip("Position in path-local space")]
            public Vector3 Position;

            /// <summary>Defines the roll of the path at this waypoint.  
            /// The other orientation axes are inferred from the tangent and world up.</summary>
            [Tooltip("Defines the roll of the path at this waypoint.  The other orientation axes are inferred from the tangent and world up.")]
            public float Roll;

            /// <summary>Representation as Vector4</summary>
            internal Vector4 AsVector4
            {
                get { return new Vector4(Position.x, Position.y, Position.z, Roll); }
            }

            internal static Waypoint FromVector4(Vector4 v)
            {
                Waypoint wp = new Waypoint();
                wp.Position = new Vector3(v[0], v[1], v[2]);
                wp.Roll = v[3];
                return wp;
            }

            internal static bool AreEqual(Waypoint a, Waypoint b)
            {
                bool positionEqual = ExtVector3.AlmostZero(a.Position - b.Position, 0.1f);
                bool rollEqual = ExtVector3.AlmostZero(a.Roll - b.Roll);
                return (positionEqual && rollEqual);
            }

            public Waypoint(Vector3 wp)
            {
                Position = wp;
                Roll = 0;
            }
        }

        /// <summary>The waypoints that define the path.
        /// They will be interpolated using a bezier curve</summary>
        [Tooltip("The waypoints that define the path.  They will be interpolated using a bezier curve.")]
        public Waypoint[] Waypoints = new Waypoint[0];

        /// <summary>number of waypoint of the spline</summary>
        public override int WaypointsCount { get { return (Waypoints.Length); } }

        /// <summary>The minimum value for the path position</summary>
        public override float MinPos { get { return 0; } }

        /// <summary>The maximum value for the path position</summary>
        public override float MaxPos
        {
            get
            {
                int count = Waypoints.Length - 1;
                if (count < 1)
                    return 0;
                return m_Looped ? count + 1 : count;
            }
        }

        protected Waypoint[] m_ControlPoints1;
        protected Waypoint[] m_ControlPoints2;
        bool m_IsLoopedCache;

        public override void CreateSplineSmooth(params Vector4[] positions)
        {
            if (Waypoints.Length != positions.Length)
            {
                Waypoints = new Waypoint[positions.Length];
                for (int i = 0; i < Waypoints.Length; i++)
                {
                    Waypoints[i] = Waypoint.FromVector4(positions[i]);
                }
                InvalidateDistanceCache();
            }
            else
            {
                bool hasChanged = false;
                for (int i = 0; i < Waypoints.Length; i++)
                {
                    if (Waypoints[i].Position == (Vector3)positions[i]
                        && Waypoints[i].Roll == positions[i].w)
                    {
                        continue;
                    }
                    Waypoints[i].Position = positions[i];
                    Waypoints[i].Roll = positions[i].w;
                    hasChanged = true;
                }
                if (hasChanged)
                {
                    InvalidateDistanceCache();
                }
            }
        }

        public override bool AreSplinesSimilare(SplineBase splineA, SplineBase splineB)
        {
            SplineSmooth A = splineA as SplineSmooth;
            SplineSmooth B = splineB as SplineSmooth;

            if (A == null || B == null)
            {
                return (false);
            }

            for (int i = 0; i < A.Waypoints.Length; i++)
            {
                if (!Waypoint.AreEqual(A.Waypoints[i], B.Waypoints[i]))
                {
                    return (false);
                }
            }
            return (true);
        }

        /// <summary>When calculating the distance cache, sample the path this many 
        /// times between points</summary>
        public override int DistanceCacheSampleStepsPerSegment { get { return Resolution; } }

        private void OnValidate() { InvalidateDistanceCache(); }

        /// <summary>Call this if the path changes in such a way as to affect distances
        /// or other cached path elements</summary>
        public override void InvalidateDistanceCache()
        {
            base.InvalidateDistanceCache();
            m_ControlPoints1 = null;
            m_ControlPoints2 = null;
        }

        protected void UpdateControlPoints()
        {
            int numPoints = (Waypoints == null) ? 0 : Waypoints.Length;
            if (numPoints > 1 
                && (Looped != m_IsLoopedCache
                    || m_ControlPoints1 == null || m_ControlPoints1.Length != numPoints
                    || m_ControlPoints2 == null || m_ControlPoints2.Length != numPoints))
            {
                Vector4[] p1 = new Vector4[numPoints];
                Vector4[] p2 = new Vector4[numPoints];
                Vector4[] K = new Vector4[numPoints];
                for (int i = 0; i < numPoints; ++i)
                    K[i] = Waypoints[i].AsVector4;
                if (Looped)
                    ExtSpline.ComputeSmoothControlPointsLooped(ref K, ref p1, ref p2);
                else
                    ExtSpline.ComputeSmoothControlPoints(ref K, ref p1, ref p2);

                m_ControlPoints1 = new Waypoint[numPoints];
                m_ControlPoints2 = new Waypoint[numPoints];
                for (int i = 0; i < numPoints; ++i)
                {
                    m_ControlPoints1[i] = Waypoint.FromVector4(p1[i]);
                    m_ControlPoints2[i] = Waypoint.FromVector4(p2[i]);
                }
                m_IsLoopedCache = Looped;
            }
        }

        /// <summary>Returns standardized position</summary>
        protected float GetBoundingIndices(float pos, out int indexA, out int indexB)
        {
            pos = StandardizePos(pos);
            int numWaypoints = Waypoints.Length;
            if (numWaypoints < 2)
                indexA = indexB = 0;
            else
            {
                indexA = Mathf.FloorToInt(pos);
                if (indexA >= numWaypoints)
                {
                    // Only true if looped
                    pos -= MaxPos;
                    indexA = 0;
                }
                indexB = indexA + 1;
                if (indexB == numWaypoints)
                {
                    if (Looped)
                        indexB = 0;
                    else 
                    {
                        --indexB;
                        --indexA;
                    }
                }
            }
            return pos;
        }

        /// <summary>Get a worldspace position of a point along the path</summary>
        /// <param name="pos">Position along the path.  Need not be normalized.</param>
        /// <returns>World-space position of the point along at path at pos</returns>
        public override Vector3 EvaluatePosition(float pos)
        {
            Vector3 result = Vector3.zero;
            if (Waypoints.Length > 0)
            {
                UpdateControlPoints();
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB);
                if (indexA == indexB)
                    result = Waypoints[indexA].Position;
                else
                    result = ExtSpline.Bezier3(pos - indexA, 
                        Waypoints[indexA].Position, m_ControlPoints1[indexA].Position,
                        m_ControlPoints2[indexA].Position, Waypoints[indexB].Position);
            }
            return transform.TransformPoint(result);
        }

        /// <summary>Get the tangent of the curve at a point along the path.</summary>
        /// <param name="pos">Position along the path.  Need not be normalized.</param>
        /// <returns>World-space direction of the path tangent.
        /// Length of the vector represents the tangent strength</returns>
        public override Vector3 EvaluateTangent(float pos)
        {
            Vector3 result = transform.rotation * Vector3.forward;
            if (Waypoints.Length > 1)
            {
                UpdateControlPoints();
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB);
                if (!Looped && indexA == Waypoints.Length - 1)
                    --indexA;
                result = ExtSpline.BezierTangent3(pos - indexA,
                    Waypoints[indexA].Position, m_ControlPoints1[indexA].Position,
                    m_ControlPoints2[indexA].Position, Waypoints[indexB].Position);
            }
            return transform.TransformDirection(result);
        }

        /// <summary>Get the orientation the curve at a point along the path.</summary>
        /// <param name="pos">Position along the path.  Need not be normalized.</param>
        /// <returns>World-space orientation of the path, as defined by tangent, up, and roll.</returns>
        public override Quaternion EvaluateOrientation(float pos)
        {
            Quaternion result = transform.rotation;
            if (Waypoints.Length > 0)
            {
                float roll = 0;
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB);
                if (indexA == indexB)
                    roll = Waypoints[indexA].Roll;
                else
                {
                    UpdateControlPoints();
                    roll = ExtSpline.Bezier1(pos - indexA,
                        Waypoints[indexA].Roll, m_ControlPoints1[indexA].Roll,
                        m_ControlPoints2[indexA].Roll, Waypoints[indexB].Roll);
                }

                Vector3 fwd = EvaluateTangent(pos);
                if (!fwd.AlmostZero())
                {
                    Vector3 up = transform.rotation * Vector3.up;
                    Quaternion q = Quaternion.LookRotation(fwd, up);
                    result = q * Quaternion.AngleAxis(roll, Vector3.forward);
                }
            }
            return (result);
        }

#if UNITY_EDITOR
        public override void DrawRadius(float radius, Color color)
        {
            if (Waypoints.Length == 0)
            {
                return;
            }
            else if (Waypoints.Length == 1)
            {
                Vector3 positionWayPoint = EvaluatePositionAtUnit(0, PositionUnits.PathUnits);
                ExtDrawGuizmos.DebugWireSphere(positionWayPoint, color, radius);
            }
            else if (Waypoints.Length == 2)
            {
                Vector3 positionWayPoint1 = EvaluatePositionAtUnit(0, PositionUnits.PathUnits);
                Vector3 positionWayPoint2 = EvaluatePositionAtUnit(1, PositionUnits.PathUnits);
                ExtDrawGuizmos.DebugCapsuleFromInsidePoint(positionWayPoint1, positionWayPoint2, color, radius);
            }
            else
            {
                for (int i = 0; i < Waypoints.Length - 1; i++)
                {
                    Vector3 positionWayPoint1 = EvaluatePositionAtUnit(i, PositionUnits.PathUnits);
                    Vector3 positionWayPoint2 = EvaluatePositionAtUnit(i + 1, PositionUnits.PathUnits);
                    Quaternion rotation1 = EvaluateOrientationAtUnit(i, PositionUnits.PathUnits);
                    Quaternion rotation2 = EvaluateOrientationAtUnit(i + 1, PositionUnits.PathUnits);
                    ExtDrawGuizmos.DebugCircle(positionWayPoint1, rotation1 * Vector3.forward, color, radius);
                    ExtDrawGuizmos.DebugCircle(positionWayPoint2, rotation2 * Vector3.forward, color, radius);
                    if (i == 0)
                    {
                        ExtDrawGuizmos.DrawHalfWireSphere(positionWayPoint1, rotation1 * -Vector3.forward, color, radius);
                    }
                    else if (i + 2 >= Waypoints.Length)
                    {
                        ExtDrawGuizmos.DrawHalfWireSphere(positionWayPoint2, rotation2 * Vector3.forward, color, radius);
                    }
                }
            }
        }
#endif
    }
}
