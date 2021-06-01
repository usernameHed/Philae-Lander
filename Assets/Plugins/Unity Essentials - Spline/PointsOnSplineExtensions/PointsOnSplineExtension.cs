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
    public abstract class PointsOnSplineExtension : MonoBehaviour
    {
        [Serializable]
        public abstract class Waypoint
        {
            public string Description;
            public float PathPosition;
            public Vector3 PreviousPosition;
            [SerializeField] private int _index; public int Index { get { return (_index); } }
            [SerializeField] private PointsOnSplineExtension _pointsListerReference;
            public PointsOnSplineExtension PointLister { get { return (_pointsListerReference); } }

            public string GetDescription()
            {
                if (string.IsNullOrWhiteSpace(Description))
                {
                    return (_index.ToString());
                }
                return (Description);
            }

            public float GetPathPosition(SplineBase.PositionUnits unitOfController)
            {
                float convertedUnit = _pointsListerReference.SplineBase.ConvertPathUnit(PathPosition, _pointsListerReference.PositionUnits, unitOfController);
                return (convertedUnit);
            }

            public virtual void Trigger(object parameters)
            {
                //Debug.Log("WayPoint " + _index + ", at " + PathPosition + " triggered!");
            }
        }
        [SerializeField, NoNull] private SplineBase _spline;    public SplineBase SplineBase { get { return (_spline); } }    public bool IsSplineLinked { get { return (_spline != null); } }
        [SerializeField, Tooltip("How to interpret Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.")]
        private SplineBase.PositionUnits _positionUnits = SplineBase.PositionUnits.Distance;    public SplineBase.PositionUnits PositionUnits { get { return (_positionUnits); } }

        public abstract int WaypointsCount { get; }
        public abstract Waypoint GetWayPoint(int index);
        public abstract void SetWayPoint(Waypoint waypoint, int index);

#if UNITY_EDITOR
        [SerializeField, HideInInspector] private string _description = "Points on splines";    public string Description { get { return (_description); } }
        [SerializeField, Tooltip("Color of waypoints")] private Color _colorWayPoints = Color.white;  public Color ColorWayPoint { get { return (_colorWayPoints); } }
        [SerializeField, Tooltip("Show Gizmos (waypoints, line and everything)")] private bool _showWayPoints = true;    public bool ShowWayPoints { get { return (_showWayPoints); } }
        [SerializeField, Tooltip("Show gizmos even when not selected")] private bool _showWayPointsWhenUnselected = true; public bool ShowWayPointsWhenUnselected { get { return (_showWayPointsWhenUnselected); } }
        [SerializeField, Tooltip("When you rename a point (right click > rename on the button), decide if you display the name also in the sceneView")] private bool _showFullNameOnSceneView = true; public bool ShowFullNameOnSceneView { get { return (_showFullNameOnSceneView); } }
        [SerializeField, Tooltip("When creating new point, set it automaticly close to the scene view camera")] private bool _addPointClosestToCamera = true; public bool AddPointClosestToCamera { get { return (_addPointClosestToCamera); } }
        [SerializeField, Tooltip("If you set this target, you will be able to move your point close to this target with the SetPositionButton")] private Transform _targetToTrack = default;    public Vector3 TargetPosition { get { return (_targetToTrack != null ? _targetToTrack.position : Vector3.zero); } }
        [SerializeField, HideInInspector] private bool _foldout = false;
        [SerializeField, HideInInspector] private int _lastIndexSelected = -1; public int LastSelected { get { return (_lastIndexSelected); } }
        [SerializeField] private bool _anchorWhenChangingSpline = false; public bool AnchorWhenChingingSpline { get { return (_anchorWhenChangingSpline); } }
#endif

        public Vector3 GetPositionFromPoint(int index)
        {
            return (_spline.EvaluatePositionAtUnit(GetWayPoint(index).PathPosition, _positionUnits));
        }
        
        public Vector3 GetPositionFromPoint(float pathPosition, SplineBase.PositionUnits unit)
        {
            return (_spline.EvaluatePositionAtUnit(pathPosition, unit));
        }

        public Quaternion GetRotationFromPoint(int index)
        {
            return (_spline.EvaluateOrientationAtUnit(GetWayPoint(index).PathPosition, _positionUnits));
        }

        public Quaternion GetRotationFromPoint(float pathPosition, SplineBase.PositionUnits unit)
        {
            return (_spline.EvaluateOrientationAtUnit(pathPosition, unit));
        }

        public float ConvertWorldPositionToPathPosition(Vector3 worldPosition)
        {
            return (_spline.ConvertWorldPositionToPathPosition(worldPosition, _positionUnits));
        }

        public void RecalculateWayPoint(int index)
        {
            float closestPositionOnSpline = _spline.FindClosestPoint(GetPositionFromPoint(index), 0, -1, 10);
            float currentUnits = _spline.FromPathNativeUnits(closestPositionOnSpline, _positionUnits);
            GetWayPoint(index).PathPosition = currentUnits;
        }

        public Waypoint GetClosestWaypointBidirectionnal(ControllerStick controller)
        {
            if (WaypointsCount == 0)
            {
                return (null);
            }
            Waypoint closest = GetWayPoint(0);
            float dist = DistFromWayPoint(closest, controller);
            for (int i = 1; i < WaypointsCount; i++)
            {
                Waypoint waypointToTest = GetWayPoint(i);
                float distToTest = DistFromWayPoint(waypointToTest, controller);
                if (distToTest < dist)
                {
                    closest = waypointToTest;
                    dist = distToTest;
                }
            }
            return (closest);
        }

        /// <summary>
        /// taking into account that points are ordered by pathPosition!
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public Waypoint GetClosestWaypointLeft(ControllerStick controller)
        {
            if (WaypointsCount == 0)
            {
                return (null);
            }
            Waypoint previous = null;
            for (int i = 0; i < WaypointsCount; i++)
            {
                Waypoint waypointToTest = GetWayPoint(i);
                if (waypointToTest.GetPathPosition(controller.PositionUnits) > controller.PathPosition)
                {
                    break;
                }
                previous = waypointToTest;
            }
            return (previous);
        }

        /// <summary>
        /// taking into account that points are ordered by pathPosition!
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public Waypoint GetClosestWaypointRight(ControllerStick controller)
        {
            if (WaypointsCount == 0)
            {
                return (null);
            }
            Waypoint previous = null;
            for (int i = WaypointsCount - 1; i >= 0; i--)
            {
                Waypoint waypointToTest = GetWayPoint(i);
                if (waypointToTest.GetPathPosition(controller.PositionUnits) < controller.PathPosition)
                {
                    break;
                }
                previous = waypointToTest;
            }
            return (previous);
        }

        public static float DistFromWayPoint(Waypoint waypoint, ControllerStick controller)
        {
            if (waypoint == null || controller == null)
            {
                return (0);
            }
            return (Mathf.Abs(waypoint.GetPathPosition(controller.PositionUnits) - controller.PathPosition));
        }
    }
}
