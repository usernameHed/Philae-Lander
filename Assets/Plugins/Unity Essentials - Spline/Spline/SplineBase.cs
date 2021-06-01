using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UnityEssentials.Spline
{
    /// <summary>
    /// Abstract base class for a world-space path
    /// </summary>
    [ExecuteInEditMode]
    public abstract class SplineBase : MonoBehaviour
    {
        /// <summary>Path samples per waypoint</summary>
        [Tooltip("Path samples per waypoint.  This is used for calculating path distances.")]
        [Range(1, 100)]
        public int Resolution = 20;
        [SerializeField] private bool _readOnly = false; public bool ReadOnly { get { return (_readOnly); } }
        [SerializeField] private bool _addToSplineLister = true;

        /// <summary>This class holds the settings that control how the path
        /// will appear in the editor scene view.  The path is not visible in the game view</summary>

        [Serializable] public class Appearance
        {
            [Tooltip("The color of the path itself when it is active in the editor")]
            public Color PathColor = Color.green;
            [Tooltip("The color of the path itself when it is inactive in the editor")]
            public Color InactivePathColor = Color.gray;
            [Tooltip("The width of the railroad-tracks that are drawn to represent the path")]
            [Range(0f, 10f)]
            public float Width = 0.1f;
        }
        /// <summary>The settings that control how the path
        /// will appear in the editor scene view.</summary>
        [Tooltip("The settings that control how the path will appear in the editor scene view.")]
        public Appearance Appearances = new Appearance();

        [Range(0, 10)] public float GridSize = 1f;

        public event UnityAction OnInvalidateCache;
        public event UnityAction OnSplineTransformMove;

        /// <summary>number of waypoint of the spline</summary>
        public abstract int WaypointsCount { get; }

        /// <summary>The minimum value for the path position</summary>
        public abstract float MinPos { get; }

        /// <summary>The maximum value for the path position</summary>
        public abstract float MaxPos { get; }

        /// <summary>True if the path ends are joined to form a continuous loop</summary>
        public abstract bool Looped { get; }

        /// <summary>Get a standardized path position, taking spins into account if looped</summary>
        /// <param name="pos">Position along the path</param>
        /// <returns>Standardized position, between MinPos and MaxPos</returns>
        public virtual float StandardizePos(float pos)
        {
            if (MaxPos == 0)
                return 0;
            if (Looped)
            {
                pos = pos % MaxPos;
                if (pos < 0)
                    pos += MaxPos;
                return pos > MaxPos - Mathf.Epsilon ? 0 : pos;
            }
            return Mathf.Clamp(pos, 0, MaxPos);
        }

        /// <summary>Get a worldspace position of a point along the path</summary>
        /// <param name="pos">Postion along the path.  Need not be standardized.</param>
        /// <returns>World-space position of the point along at path at pos</returns>
        public abstract Vector3 EvaluatePosition(float pos);

        /// <summary>Get the tangent of the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be standardized.</param>
        /// <returns>World-space direction of the path tangent.
        /// Length of the vector represents the tangent strength</returns>
        public abstract Vector3 EvaluateTangent(float pos);

        /// <summary>Get the orientation the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be standardized.</param>
        /// <returns>World-space orientation of the path</returns>
        public abstract Quaternion EvaluateOrientation(float pos);

        public virtual void CreateSplineSmooth(params Vector4[] positions) { }
        public virtual void CreateSpline(params Vector7[] positions) { }

        public abstract bool AreSplinesSimilare(SplineBase splineA, SplineBase splineB);

        public virtual void OnEnable()
        {
            if (_addToSplineLister && SplineLister.Instance != null)
            {
                SplineLister.Instance.AddSpline(this);
            }
        }

        public virtual void OnDisable()
        {
            if (_addToSplineLister && SplineLister.Instance != null)
            {
                SplineLister.Instance.RemoveSpline(this);
            }
        }

        /// <summary>Find the closest point on the path to a given worldspace target point.</summary>
        /// <remarks>Performance could be improved by checking the bounding polygon of each segment,
        /// and only entering the best segment(s)</remarks>
        /// <param name="p">Worldspace target that we want to approach</param>
        /// <param name="startSegment">In what segment of the path to start the search.
        /// A Segment is a section of path between 2 waypoints.</param>
        /// <param name="searchRadius">How many segments on either side of the startSegment
        /// to search.  -1 means no limit, i.e. search the entire path</param>
        /// <param name="stepsPerSegment">We search a segment by dividing it into this many
        /// straight pieces.  The higher the number, the more accurate the result, but performance
        /// is proportionally slower for higher numbers</param>
        /// <returns>The position along the path that is closest to the target point.
        /// The value is in Path Units, not Distance units.</returns>
        public virtual float FindClosestPoint(
            Vector3 p, int startSegment = 0, int searchRadius = -1, int stepsPerSegment = 10, PositionUnits units = PositionUnits.PathUnits)
        {
            float start = MinPos;
            float end = MaxPos;
            if (searchRadius >= 0)
            {
                int r = Mathf.FloorToInt(Mathf.Min(searchRadius, (end - start) / 2f));
                start = startSegment - r;
                end = startSegment + r + 1;
                if (!Looped)
                {
                    start = Mathf.Max(start, MinPos);
                    end = Mathf.Max(end, MaxPos);
                }
            }
            stepsPerSegment = Mathf.RoundToInt(Mathf.Clamp(stepsPerSegment, 1f, 100f));
            float stepSize = 1f / stepsPerSegment;
            float bestPos = startSegment;
            float bestDistance = float.MaxValue;
            int iterations = (stepsPerSegment == 1) ? 1 : 3;
            for (int i = 0; i < iterations; ++i)
            {
                Vector3 v0 = EvaluatePosition(start);
                for (float f = start + stepSize; f <= end; f += stepSize)
                {
                    Vector3 v = EvaluatePosition(f);
                    float t = p.ClosestPointOnSegment(v0, v);
                    float d = Vector3.SqrMagnitude(p - Vector3.Lerp(v0, v, t));
                    if (d < bestDistance)
                    {
                        bestDistance = d;
                        bestPos = f - (1 - t) * stepSize;
                    }
                    v0 = v;
                }
                start = bestPos - stepSize;
                end = bestPos + stepSize;
                stepSize /= stepsPerSegment;
            }
            if (units == PositionUnits.PathUnits)
            {
                return (bestPos);
            }
            return ConvertPathUnit(bestPos, PositionUnits.PathUnits, units);
        }

        public virtual float FindClosestPointFromRay(Ray ray, int maxSteps = 30, float sizeSteps = 1, PositionUnits units = PositionUnits.PathUnits)
        {
            Vector3 closestPointOnRay = ray.origin;
            float closestPoint = FindClosestPoint(closestPointOnRay, 0, -1, 10);
            float closestDist = Vector3.Distance(closestPointOnRay, EvaluatePosition(closestPoint));

            for (int i = 1; i < maxSteps; i++)
            {
                Vector3 pointOnRay = ray.origin + ray.direction * (i * sizeSteps);
                float pointOnSpline = FindClosestPoint(pointOnRay, 0, -1, 10);
                Vector3 vectorOnSpline = EvaluatePosition(pointOnSpline);
                float dist = Vector3.Distance(pointOnRay, vectorOnSpline);
                if (dist < closestDist)
                {
                    closestPoint = pointOnSpline;
                    closestDist = dist;
                }
            }
            if (units == PositionUnits.PathUnits)
            {
                return (closestPoint);
            }
            return ConvertPathUnit(closestPoint, PositionUnits.PathUnits, units);
        }

        public static float GetClosestPointFromCurrentBidirectionnal(float currentPosition, float pointA, float pointB, out bool chooseA)
        {
            float A = Mathf.Abs(currentPosition - pointA);
            float B = Mathf.Abs(currentPosition - pointB);

            chooseA = A <= B;
            return (chooseA ? pointA : pointB);
        }

        public static float DistanceToLine(Ray ray, Vector3 point)
        {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }

        public static Vector3 NearestPointOnRay(Ray ray, Vector3 point)
        {
            Vector3 delta = point - ray.origin;
            float distance = Mathf.Max(0, Vector3.Dot(delta, ray.direction));
            return ray.origin + ray.direction * distance;
        }



        /// <summary>How to interpret the Path Position</summary>
        public enum PositionUnits
        {
            /// <summary>Use PathPosition units, where 0 is first waypoint, 1 is second waypoint, etc</summary>
            PathUnits,
            /// <summary>Use Distance Along Path.  Path will be sampled according to its Resolution
            /// setting, and a distance lookup table will be cached internally</summary>
            Distance,
            /// <summary>Normalized units, where 0 is the start of the path, and 1 is the end.
            /// Path will be sampled according to its Resolution
            /// setting, and a distance lookup table will be cached internally</summary>
            Normalized
        }

        /// <summary>Get the minimum value, for the given unit type</summary>
        /// <param name="units">The unit type</param>
        /// <returns>The minimum allowable value for this path</returns>
        public float MinUnit(PositionUnits units)
        {
            if (units == PositionUnits.Normalized)
                return 0;
            return units == PositionUnits.Distance ? 0 : MinPos;
        }

        /// <summary>Get the maximum value, for the given unit type</summary>
        /// <param name="units">The unit type</param>
        /// <returns>The maximum allowable value for this path</returns>
        public float MaxUnit(PositionUnits units)
        {
            if (units == PositionUnits.Normalized)
                return 1;
            return units == PositionUnits.Distance ? PathLength : MaxPos;
        }

        public float FromUnitDistanceToReal3DDistance(float amount, PositionUnits units)
        {
            amount = ConvertPathUnit(amount, units, PositionUnits.Distance);
            return (amount);
        }

        /// <summary>Standardize the unit, so that it lies between MinUmit and MaxUnit</summary>
        /// <param name="pos">The value to be standardized</param>
        /// <param name="units">The unit type</param>
        /// <returns>The standardized value of pos, between MinUnit and MaxUnit</returns>
        public virtual float StandardizeUnit(float pos, PositionUnits units)
        {
            if (units == PositionUnits.PathUnits)
                return StandardizePos(pos);
            if (units == PositionUnits.Distance)
                return StandardizePathDistance(pos);
            float len = PathLength;
            if (len < Mathf.Epsilon)
                return 0;
            return StandardizePathDistance(pos * len) / len;
        }

        /// <summary>Get a worldspace position of a point along the path</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <param name="units">The unit to use when interpreting the value of pos.</param>
        /// <returns>World-space position of the point along at path at pos</returns>
        public Vector3 EvaluatePositionAtUnit(float pos, PositionUnits units)
        {
            return EvaluatePosition(ToNativePathUnits(pos, units));
        }

        /// <summary>Get the tangent of the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <param name="units">The unit to use when interpreting the value of pos.</param>
        /// <returns>World-space direction of the path tangent.
        /// Length of the vector represents the tangent strength</returns>
        public Vector3 EvaluateTangentAtUnit(float pos, PositionUnits units)
        {
            return EvaluateTangent(ToNativePathUnits(pos, units));
        }

        /// <summary>Get the orientation the curve at a point along the path.</summary>
        /// <param name="pos">Postion along the path.  Need not be normalized.</param>
        /// <param name="units">The unit to use when interpreting the value of pos.</param>
        /// <returns>World-space orientation of the path</returns>
        public Quaternion EvaluateOrientationAtUnit(float pos, PositionUnits units)
        {
            return EvaluateOrientation(ToNativePathUnits(pos, units));
        }

        public float ConvertWorldPositionToPathPosition(Vector3 worldPosition, PositionUnits units)
        {
            float position = FindClosestPoint(worldPosition, 0, -1, 10);
            float currentUnits = FromPathNativeUnits(position, units);
            return (currentUnits);
        }

        /// <summary>When calculating the distance cache, sample the path this many
        /// times between points</summary>
        public abstract int DistanceCacheSampleStepsPerSegment { get; }

        /// <summary>Call this if the path changes in such a way as to affect distances
        /// or other cached path elements</summary>
        public virtual void InvalidateDistanceCache()
        {
            m_DistanceToPos = null;
            m_PosToDistance = null;
            m_CachedSampleSteps = 0;
            m_PathLength = 0;
            OnInvalidateCache?.Invoke();
        }

        /// <summary>See whether the distance cache is valid.  If it's not valid,
        /// then any call to GetPathLength() or ToNativePathUnits() will
        /// trigger a potentially costly regeneration of the path distance cache</summary>
        /// <returns>Whether the cache is valid</returns>
        public bool DistanceCacheIsValid()
        {
            return (MaxPos == MinPos)
                || (m_DistanceToPos != null && m_PosToDistance != null
                    && m_CachedSampleSteps == DistanceCacheSampleStepsPerSegment
                    && m_CachedSampleSteps > 0);
        }

        /// <summary>Get the length of the path in distance units.
        /// If the distance cache is not valid, then calling this will
        /// trigger a potentially costly regeneration of the path distance cache</summary>
        /// <returns>The length of the path in distance units, when sampled at this rate</returns>
        public float PathLength
        {
            get
            {
                if (DistanceCacheSampleStepsPerSegment < 1)
                    return 0;
                if (!DistanceCacheIsValid())
                    ResamplePath(DistanceCacheSampleStepsPerSegment);
                return m_PathLength;
            }
        }

        /// <summary>Standardize a distance along the path based on the path length.
        /// If the distance cache is not valid, then calling this will
        /// trigger a potentially costly regeneration of the path distance cache</summary>
        /// <param name="distance">The distance to standardize</param>
        /// <returns>The standardized distance, ranging from 0 to path length</returns>
        public float StandardizePathDistance(float distance)
        {
            float length = PathLength;
            if (length < Vector3.kEpsilon)
                return 0;
            if (Looped)
            {
                distance = distance % length;
                if (distance < 0)
                    distance += length;
            }
            return Mathf.Clamp(distance, 0, length);
        }

        /// <summary>Get the path position (in native path units) corresponding to the psovided
        /// value, in the units indicated.
        /// If the distance cache is not valid, then calling this will
        /// trigger a potentially costly regeneration of the path distance cache</summary>
        /// <param name="pos">The value to convert from</param>
        /// <param name="units">The units in which pos is expressed</param>
        /// <returns>The length of the path in native units, when sampled at this rate</returns>
        public float ToNativePathUnits(float pos, PositionUnits units)
        {
            if (units == PositionUnits.PathUnits)
                return pos;
            if (DistanceCacheSampleStepsPerSegment < 1 || PathLength < Mathf.Epsilon)
                return MinPos;
            if (units == PositionUnits.Normalized)
                pos *= PathLength;
            pos = StandardizePathDistance(pos);
            float d = pos / m_cachedDistanceStepSize;
            int i = Mathf.FloorToInt(d);
            if (i >= m_DistanceToPos.Length-1)
                return MaxPos;
            float t = d - (float)i;
            return MinPos + Mathf.Lerp(m_DistanceToPos[i], m_DistanceToPos[i+1], t);
        }

        /// <summary>Get the path position (in path units) corresponding to this distance along the path.
        /// If the distance cache is not valid, then calling this will
        /// trigger a potentially costly regeneration of the path distance cache</summary>
        /// <param name="pos">The value to convert from, in native units</param>
        /// <param name="units">The units to convert toexpressed</param>
        /// <returns>The length of the path in distance units, when sampled at this rate</returns>
        public float FromPathNativeUnits(float pos, PositionUnits units)
        {
            if (units == PositionUnits.PathUnits)
                return pos;
            float length = PathLength;
            if (DistanceCacheSampleStepsPerSegment < 1 || length < Mathf.Epsilon)
                return 0;
            pos = StandardizePos(pos);
            float d = pos / m_cachedPosStepSize;
            int i = Mathf.FloorToInt(d);
            if (i >= m_PosToDistance.Length-1)
                pos = m_PathLength;
            else
            {
                float t = d - (float)i;
                pos = Mathf.Lerp(m_PosToDistance[i], m_PosToDistance[i+1], t);
            }
            if (units == PositionUnits.Normalized)
                pos /= length;
            return pos;
        }

        public float ConvertPathUnit(float currentPosition, PositionUnits currentUnit, PositionUnits wantedUnit)
        {
            Vector3 position = EvaluatePositionAtUnit(currentPosition, currentUnit);
            float prevPos = ToNativePathUnits(currentPosition, currentUnit);
            float pathPosition = FindClosestPoint(position, Mathf.FloorToInt(prevPos), -1, 10);
            pathPosition = FromPathNativeUnits(pathPosition, wantedUnit);
            return (pathPosition);
        }

        private float[] m_DistanceToPos;
        private float[] m_PosToDistance;
        private int m_CachedSampleSteps;
        private float m_PathLength;
        private float m_cachedPosStepSize;
        private float m_cachedDistanceStepSize;

        private void ResamplePath(int stepsPerSegment)
        {
            InvalidateDistanceCache();

            float minPos = MinPos;
            float maxPos = MaxPos;
            float stepSize = 1f / Mathf.Max(1, stepsPerSegment);

            // Sample the positions
            int numKeys = Mathf.RoundToInt((maxPos - minPos) / stepSize) + 1;
            m_PosToDistance = new float[numKeys];
            m_CachedSampleSteps = stepsPerSegment;
            m_cachedPosStepSize = stepSize;

            Vector3 p0 = EvaluatePosition(0);
            m_PosToDistance[0] = 0;
            float pos = minPos;
            for (int i = 1; i < numKeys; ++i)
            {
                pos += stepSize;
                Vector3 p = EvaluatePosition(pos);
                float d = Vector3.Distance(p0, p);
                m_PathLength += d;
                p0 = p;
                m_PosToDistance[i] = m_PathLength;
            }

            // Resample the distances
            m_DistanceToPos = new float[numKeys];
            m_DistanceToPos[0] = 0;
            if (numKeys > 1)
            {
                stepSize = m_PathLength / (numKeys - 1);
                m_cachedDistanceStepSize = stepSize;
                float distance = 0;
                int posIndex = 1;
                for (int i = 1; i < numKeys; ++i)
                {
                    distance += stepSize;
                    float d = m_PosToDistance[posIndex];
                    while (d < distance && posIndex < numKeys-1)
                         d = m_PosToDistance[++posIndex];
                    float d0 =  m_PosToDistance[posIndex-1];
                    float delta = d - d0;
                    float t = (distance - d0) / delta;
                    m_DistanceToPos[i] = m_cachedPosStepSize * (t + posIndex - 1);
                }
            }
        }

#if UNITY_EDITOR
        public abstract void DrawRadius(float radius, Color color);
#endif

        protected virtual void Update()
        {
            if (transform.hasChanged)
            {
                OnSplineTransformMove?.Invoke();
                transform.hasChanged = false;
            }
        }

        public Vector3 AttemptToApplyGrid(Vector3 position, float defaultWhenNoControl = 0)
        {
            if (GridSize != 0 && (Event.current.control || Event.current.alt || Event.current.shift))
            {
                if (Event.current.control && !Event.current.alt)
                {
                    position = new Vector3(
                        RoundToGrid(position.x, GridSize),
                        RoundToGrid(position.y, GridSize),
                        RoundToGrid(position.z, GridSize));
                }
                else if (Event.current.alt && !Event.current.control)
                {
                    position = new Vector3(
                        RoundToGrid(position.x, GridSize * 2),
                        RoundToGrid(position.y, GridSize * 2),
                        RoundToGrid(position.z, GridSize * 2));
                }
                else if (Event.current.alt && Event.current.control)
                {
                    position = new Vector3(
                        RoundToGrid(position.x, GridSize * 8),
                        RoundToGrid(position.y, GridSize * 8),
                        RoundToGrid(position.z, GridSize * 8));
                }
            }
            else if (defaultWhenNoControl != 0)
            {
                position = new Vector3(
                       RoundToGrid(position.x, defaultWhenNoControl),
                       RoundToGrid(position.y, defaultWhenNoControl),
                       RoundToGrid(position.z, defaultWhenNoControl));
            }
            return (position);
        }

        public float AttemptToApplyGrid(float position, float defaultWhenNoControl = 0)
        {
            if (GridSize != 0 && (Event.current.control || Event.current.alt || Event.current.shift))
            {
                if (Event.current.control && !Event.current.alt)
                {
                    position = RoundToGrid(position, GridSize);
                }
                else if (Event.current.alt && !Event.current.control)
                {
                    position = RoundToGrid(position, GridSize * 2);
                }
                else if (Event.current.alt && Event.current.control)
                {
                    position = RoundToGrid(position, GridSize * 8);
                }
            }
            else if (defaultWhenNoControl != 0)
            {
                position = RoundToGrid(position, defaultWhenNoControl);
            }
            return (position);
        }

        private float RoundToGrid(float input, float snap)
        {
            return snap * Mathf.Round((input / snap));
        }
    }
}
