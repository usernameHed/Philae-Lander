using UnityEngine;
using System;

namespace UnityEssentials.Spline
{
    /// <summary>Defines a world-space path, consisting of an array of waypoints,
    /// each of which has position, tangent, and roll settings.  Bezier interpolation
    /// is performed between the waypoints, to get a smooth and continuous path.</summary>
    [AddComponentMenu("Unity Essentials/Spline/Spline Type/Spline")]
    public class Spline : SplineBase
    {
        /// <summary>A waypoint along the path</summary>
        [Serializable] public struct Waypoint
        {
            /// <summary>Position in path-local space</summary>
            [Tooltip("Position in path-local space")]
            public Vector3 Position;

            /// <summary>Offset from the position, which defines the tangent of the curve at the waypoint.  
            /// The length of the tangent encodes the strength of the bezier handle.  
            /// The same handle is used symmetrically on both sides of the waypoint, to ensure smoothness.</summary>
            [Tooltip("Offset from the position, which defines the tangent of the curve at the waypoint.  The length of the tangent encodes the strength of the bezier handle.  The same handle is used symmetrically on both sides of the waypoint, to ensure smoothness.")]
            public Vector3 Tangent;

            /// <summary>Defines the roll of the path at this waypoint.  
            /// The other orientation axes are inferred from the tangent and world up.</summary>
            [Tooltip("Defines the roll of the path at this waypoint.  The other orientation axes are inferred from the tangent and world up.")]
            public float Roll;

            internal static Waypoint FromVector7(Vector7 v)
            {
                Waypoint wp = new Waypoint();
                wp.Position = new Vector3(v[0], v[1], v[2]);
                wp.Tangent = new Vector3(v[3], v[4], v[5]);
                wp.Roll = v[6];
                return wp;
            }

            internal static bool AreEqual(Waypoint a, Waypoint b)
            {
                bool positionEqual = ExtVector3.AlmostZero(a.Position - b.Position, 0.1f);
                bool tangentEqual = ExtVector3.AlmostZero(a.Tangent - b.Tangent, 0.1f);
                bool rollEqual = ExtVector3.AlmostZero(a.Roll - b.Roll);
                return (positionEqual && tangentEqual && rollEqual);
            }

            internal static bool AreEqual(Vector7 a, Waypoint b)
            {
                return (new Vector3(a.a, a.b, a.c) == b.Position && new Vector3(a.d, a.e, a.f) == b.Tangent && a.g == b.Roll);
            }
        }

        /// <summary>If checked, then the path ends are joined to form a continuous loop</summary>
        [Tooltip("If checked, then the path ends are joined to form a continuous loop.")]
        public bool m_Looped;
        /// <summary>True if the path ends are joined to form a continuous loop</summary>
        public override bool Looped { get { return m_Looped; } }


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
        
        /// <summary>When calculating the distance cache, sample the path this many 
        /// times between points</summary>
        public override int DistanceCacheSampleStepsPerSegment { get { return Resolution; } }

        public override void CreateSpline(params Vector7[] positions)
        {
            if (Waypoints.Length != positions.Length)
            {
                Waypoints = new Waypoint[positions.Length];
                for (int i = 0; i < Waypoints.Length; i++)
                {
                    Waypoints[i] = Waypoint.FromVector7(positions[i]);
                }
                InvalidateDistanceCache();
            }
            else
            {
                bool hasChanged = false;
                for (int i = 0; i < Waypoints.Length; i++)
                {
                    if (Waypoint.AreEqual(positions[i], Waypoints[i]))
                    {
                        continue;
                    }
                    Waypoints[i].Position = new Vector3(positions[i].a, positions[i].b, positions[i].c);
                    Waypoints[i].Tangent = new Vector3(positions[i].d, positions[i].e, positions[i].f);
                    Waypoints[i].Roll = positions[i].g;
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
            Spline A = splineA as Spline;
            Spline B = splineB as Spline;

            if (A == null || B == null)
            {
                return (false);
            }
            for (int i = 0; i < A.Waypoints.Length; i++)
            {
                if (!Waypoint.AreEqual(A.Waypoints[i], B.Waypoints[i]))
                {
                    Debug.Log("not equal waypoints " + i);
                    return (false);
                }
            }
            return (true);
        }



        /// <summary>Returns normalized position</summary>
        float GetBoundingIndices(float pos, out int indexA, out int indexB)
        {
            pos = StandardizePos(pos);
            int rounded = Mathf.RoundToInt(pos);
            if (Mathf.Abs(pos - rounded) < Mathf.Epsilon)
                indexA = indexB = (rounded == Waypoints.Length) ? 0 : rounded;
            else
            {
                indexA = Mathf.FloorToInt(pos);
                if (indexA >= Waypoints.Length)
                {
                    pos -= MaxPos;
                    indexA = 0;
                }
                indexB = Mathf.CeilToInt(pos);
                if (indexB >= Waypoints.Length)
                    indexB = 0;
            }
            return pos;
        }

        /// <summary>Get a worldspace position of a point along the path</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <returns>World-space position of the point along at path at pos</returns>
        public override Vector3 EvaluatePosition(float pos)
        {
            Vector3 result = new Vector3();
            if (Waypoints.Length == 0)
                result = transform.position;
            else
            {
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB);
                if (indexA == indexB)
                    result = Waypoints[indexA].Position;
                else
                {
                    // interpolate
                    Waypoint wpA = Waypoints[indexA];
                    Waypoint wpB = Waypoints[indexB];
                    result = ExtSpline.Bezier3(pos - indexA,
                        Waypoints[indexA].Position, wpA.Position + wpA.Tangent,
                        wpB.Position - wpB.Tangent, wpB.Position);
                }
            }
            return transform.TransformPoint(result);
        }

        /// <summary>Get the tangent of the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <returns>World-space direction of the path tangent.
        /// Length of the vector represents the tangent strength</returns>
        public override Vector3 EvaluateTangent(float pos)
        {
            Vector3 result = new Vector3();
            if (Waypoints.Length == 0)
                result = transform.rotation * Vector3.forward;
            else
            {
                int indexA, indexB;
                pos = GetBoundingIndices(pos, out indexA, out indexB);
                if (indexA == indexB)
                    result = Waypoints[indexA].Tangent;
                else
                {
                    Waypoint wpA = Waypoints[indexA];
                    Waypoint wpB = Waypoints[indexB];
                    result = ExtSpline.BezierTangent3(pos - indexA,
                        Waypoints[indexA].Position, wpA.Position + wpA.Tangent,
                        wpB.Position - wpB.Tangent, wpB.Position);
                }
            }
            return transform.TransformDirection(result);
        }

        /// <summary>Get the orientation the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
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
                    float rollA = Waypoints[indexA].Roll;
                    float rollB = Waypoints[indexB].Roll;
                    if (indexB == 0)
                    {
                        // Special handling at the wraparound - cancel the spins
                        rollA = rollA % 360;
                        rollB = rollB % 360;
                    }
                    roll = Mathf.Lerp(rollA, rollB, pos - indexA);
                }

                Vector3 fwd = EvaluateTangent(pos);
                if (!fwd.AlmostZero())
                {
                    Vector3 up = transform.rotation * Vector3.up;
                    Quaternion q = Quaternion.LookRotation(fwd, up);
                    result = q * Quaternion.AngleAxis(roll, Vector3.forward);
                }
            }
            return result;
        }

        private void OnValidate() { InvalidateDistanceCache(); }

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
