using UnityEngine;
using System;

namespace UnityEssentials.Spline.PointsOnSplineExtensions
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Points Lister/Basic Points")]
    public class BasicPoints : PointsOnSplineExtension
    {
        [Serializable]
        public class BasicWaypoint : PointsOnSplineExtension.Waypoint
        {
            
        }

        public BasicWaypoint[] Waypoints = new BasicWaypoint[0];
        public override int WaypointsCount { get { return (Waypoints.Length); } }
        public override Waypoint GetWayPoint(int index)
        {
            if (index < 0)
            {
                return (Waypoints[0]);
            }
            if (index >= Waypoints.Length)
            {
                return (Waypoints[Waypoints.Length - 1]);
            }
            return (Waypoints[index]);
        }

        public bool DisplayVector3 = true;

        public override void SetWayPoint(Waypoint waypoint, int index)
        {
            Waypoints[index] = (BasicWaypoint)waypoint;
        }
    }
}