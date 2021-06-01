using UnityEngine;
using System;

namespace UnityEssentials.Spline
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Spline Type/Line")]
    public class SimpleLine : Spline
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            Vector3 delta = Waypoints[1].Position - Waypoints[0].Position;
            float deltaSquared = Vector3.Dot(delta, delta);
            float dist = Vector3.Dot(k - Waypoints[0].Position, delta);

            //k projection is outside the [_p1, _p2] interval, closest to _p1
            if (dist <= 0.0f)
            {
                return (Waypoints[0].Position);
            }
            //k projection is outside the [_p1, p2] interval, closest to _p2
            else if (dist >= deltaSquared)
            {
                return (Waypoints[1].Position);
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                dist = dist / deltaSquared;
                Vector3 pointOnLine = Waypoints[0].Position + dist * delta;
                return (pointOnLine);
            }
        }
    }
}
