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
    [AddComponentMenu("Unity Essentials/Spline/Points Lister/Component Points")]
    public class ComponentPoints : PointsOnSplineExtension
    {
        [Serializable]
        public class ComponentWaypoint : PointsOnSplineExtension.Waypoint
        {
            public Component Reference;

            public override void Trigger(object parameters)
            {
                Debug.Log("Component Point");
                base.Trigger(parameters);
            }
        }

        public ComponentWaypoint[] Waypoints = new ComponentWaypoint[0];
        public override int WaypointsCount { get { return (Waypoints.Length); } }
        public override Waypoint GetWayPoint(int index) { return (Waypoints[index]); }

        [Header("Components settings")]
        [SerializeField, Tooltip("When you create a point, duplicate the current selected one")] private bool _duplicateCurrentSelectedOnCreation = true;
        [SerializeField, Tooltip("When moving point, move also the Target Reference")] private bool _holdShiftToMoveReference = false;
        [SerializeField, Tooltip("When removing a point, remove also the Target Reference")] private bool _removeReferenceWhenRemovingPoint = true;
        [SerializeField, Tooltip("Show the target")] private bool _showTarget = true; public bool ShowTarget { get { return (_showTarget); } }
        [SerializeField, Tooltip("Show Distance on Scene View")] private bool _showDistance = true;
        [SerializeField, Tooltip("Show a default handle for the Target when selecting a point")] private bool _showHandleTarget = false;


        public override void SetWayPoint(Waypoint waypoint, int index)
        {
            Waypoints[index] = (ComponentWaypoint)waypoint;
        }

        public virtual Component GetReference(int index)
        {
            if (index < 0 || index >= Waypoints.Length)
            {
                return (null);
            }
            return (Waypoints[index].Reference);
        }
    }
}