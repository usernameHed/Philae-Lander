using UnityEngine;
using System;

namespace UnityEssentials.Spline.PointsOnSplineExtensions
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Points Lister/Rotation Points")]
    public class QuaternionPoints : PointsOnSplineExtension
    {
        [Serializable]
        public class QuaternionWaypoint : PointsOnSplineExtension.Waypoint
        {
            public Quaternion Rotation;

            public override void Trigger(object parameters)
            {
                base.Trigger(parameters);
                Debug.Log("Vector3 Point");
            }
        }

        public QuaternionWaypoint[] Waypoints = new QuaternionWaypoint[0];
        public override int WaypointsCount { get { return (Waypoints.Length); } }
        public override Waypoint GetWayPoint(int index) { return (Waypoints[index]); }

        public bool DisplayQuaternion = true;

        public override void SetWayPoint(Waypoint waypoint, int index)
        {
            Waypoints[index] = (QuaternionWaypoint)waypoint;
        }

        public Quaternion GetRotation(int index) { return (Waypoints[index].Rotation); }
    }
}