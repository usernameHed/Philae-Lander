using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEssentials.Spline.PropertyAttribute.NoNull;
using UnityEssentials.Spline.Controller;

namespace UnityEssentials.Spline.PointsOnSplineExtensions
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Points Lister/Action Points")]
    public class ActionPoints : ComponentPoints
    {
        [Serializable]
        public class ActionPointsWaypoint : ComponentWaypoint
        {
            public TriggerActionPoint ReferenceAction;

            public override void Trigger(object parameters)
            {
                if (ReferenceAction != null)
                {
                    ReferenceAction.Trigger(this, parameters);
                }
            }
        }

        public ActionPointsWaypoint[] WaypointsAction = new ActionPointsWaypoint[0];
        public override int WaypointsCount { get { return (WaypointsAction.Length); } }
        public override Waypoint GetWayPoint(int index) { return (WaypointsAction[index]); }

        public override void SetWayPoint(Waypoint waypoint, int index)
        {
            WaypointsAction[index] = (ActionPointsWaypoint)waypoint;
        }

        public override Component GetReference(int index)
        {
            if (index < 0 || index >= WaypointsAction.Length)
            {
                return (null);
            }
            return (WaypointsAction[index].ReferenceAction);
        }
    }
}