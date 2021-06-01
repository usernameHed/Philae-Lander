using UnityEngine;
using System;

namespace UnityEssentials.Spline.PointsOnSplineExtensions
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Points Lister/Vector Points")]
    public class VectorPoints : PointsOnSplineExtension
    {
        [Serializable]
        public class VectorWaypoint : PointsOnSplineExtension.Waypoint
        {
            public Vector3 Point;

            public override void Trigger(object parameters)
            {
                base.Trigger(parameters);
                Debug.Log("Vector3 Point");
            }
        }

        public VectorWaypoint[] Waypoints = new VectorWaypoint[0];
        public override int WaypointsCount { get { return (Waypoints.Length); } }
        public override Waypoint GetWayPoint(int index) { return (Waypoints[index]); }

        public bool DisplayVector3 = true;

        public override void SetWayPoint(Waypoint waypoint, int index)
        {
            Waypoints[index] = (VectorWaypoint)waypoint;
        }

        public Vector3 GetPoint(int index) { return (Waypoints[index].Point); }
    }
}