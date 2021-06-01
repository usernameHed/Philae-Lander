using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Spline.Controller;
using UnityEssentials.Spline.PointsOnSplineExtensions;
using UnityEssentials.Spline.PropertyAttribute.NoNull;

namespace UnityEssentials.Spline.ControllerExtensions.EventsTrigger
{
    /// <summary>
    /// This script will run though all points inside the given lister, and trigger them when we cross them.
    /// We can only trigger a point once, as we cross the spline from A to B.
    /// If you want to reset it (when for exemple, you teleport, or loop), call ResetControllerEvents
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Events/Trigger Points")]
    public class ControllerEventsTrigger : ControllerEvents
    {
        [SerializeField] private bool _resetOnLoop = true;

        private int[] _indexChecked = new int[0];
        private float _previousPathPosition = -1;

        private List<PointsOnSplineExtension.Waypoint> _toTrigger = new List<PointsOnSplineExtension.Waypoint>(10);

        private void Awake()
        {
            SetupIndexArray();

            //prevent previous point to all trigger simultaniously at awake
            UpdateAllWayPointCrossed(0, _controller.PathPosition, _controller.PositionUnits);
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _controller.OnLooped += OnLooped;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _controller.OnLooped -= OnLooped;
        }

        private void OnLooped()
        {
            Calculate();
            if (_resetOnLoop && _controller.SplineBase.Looped)
            {
                ResetControllerEvents();
            }
        }

        public override void SplineChanged(SplineBase newSpline)
        {
            base.SplineChanged(newSpline);
            ResetControllerEvents();
            _previousPathPosition = _controller.ClampedPathPosition;
        }

        /// <summary>
        /// call this when you want to be able to re-check again previous point on spline
        /// </summary>
        public void ResetControllerEvents()
        {
            SetupIndexArray();
        }

        /// <summary>
        /// important to start at -1, because we start the loop at index + 1 (0 at start)
        /// </summary>
        private void SetupIndexArray()
        {
            _indexChecked = new int[ListToCheck.Length];
            for (int i = 0; i < _indexChecked.Length; i++)
            {
                _indexChecked[i] = -1;
            }
        }

        private void Update()
        {
            Calculate();
        }

        private void Calculate()
        {
            if (_controller == null)
            {
                return;
            }

            if (UpdateAllWayPointCrossed(_previousPathPosition, _controller.ClampedPathPosition, _controller.PositionUnits))
            {
                for (int i = 0; i < _toTrigger.Count; i++)
                {
                    float diff = _controller.ClampedPathPosition - _toTrigger[i].GetPathPosition(_controller.PositionUnits);
                    Debug.Log("current player: " + _controller.ClampedPathPosition);
                    Debug.Log("trigger in COntroller Unit: " + _toTrigger[i].GetPathPosition(_controller.PositionUnits));
                    _toTrigger[i].Trigger(new object[3] { this, diff, _toTrigger[i].GetPathPosition(_controller.PositionUnits) });
                }
            }
            _previousPathPosition = _controller.ClampedPathPosition;
        }

        public bool UpdateAllWayPointCrossed(float previous, float current, SplineBase.PositionUnits positionUnits)
        {
            _toTrigger.Clear();

            for (int i = 0; i < ListToCheck.Length; i++)
            {
                if (ListToCheck[i] == null)
                {
                    continue;
                }
                if (!ListToCheck[i].gameObject.activeInHierarchy)
                {
                    continue;
                }

                int startIndex = _indexChecked[i] + 1;
                for (int k = startIndex; k < ListToCheck[i].WaypointsCount; k++)
                {
                    float wayPoint = ListToCheck[i].GetWayPoint(k).GetPathPosition(positionUnits);
                    if (wayPoint > previous && wayPoint <= current && Mathf.Abs(wayPoint - current) < _maxDistanceToActive)
                    {
                        _toTrigger.Add(ListToCheck[i].GetWayPoint(k));
                        _indexChecked[i] = k;
                    }
                    else if (wayPoint > current)
                    {
                        break;
                    }
                }
            }
            return (_toTrigger.Count > 0);
        }

        public PointsOnSplineExtension.Waypoint GetClosestWaypointBidirectionnal(ControllerStick controller, out float dist, out PointsOnSplineExtension lister)
        {
            dist = 0;
            lister = null;
            if (ListToCheck.Length == 0)
            {
                return (null);
            }
            PointsOnSplineExtension.Waypoint closest = ListToCheck[0].GetClosestWaypointBidirectionnal(controller);
            dist = PointsOnSplineExtension.DistFromWayPoint(closest, controller);
            lister = ListToCheck[0];
            for (int i = 1; i < ListToCheck.Length; i++)
            {
                PointsOnSplineExtension.Waypoint waypointToTest = ListToCheck[i].GetClosestWaypointBidirectionnal(controller);
                float distToTest = PointsOnSplineExtension.DistFromWayPoint(waypointToTest, controller);
                if (distToTest < dist)
                {
                    closest = waypointToTest;
                    dist = distToTest;
                    lister = ListToCheck[i];
                }
            }
            return (closest);
        }

        public PointsOnSplineExtension.Waypoint GetClosestWaypointLeft(ControllerStick controller, out float dist, out PointsOnSplineExtension lister)
        {
            dist = 0;
            lister = null;
            if (ListToCheck.Length == 0)
            {
                return (null);
            }
            PointsOnSplineExtension.Waypoint closest = null;
            dist = 0;
            lister = null;
            bool foundOne = false;

            for (int i = 0; i < ListToCheck.Length; i++)
            {
                if (ListToCheck[i] == null)
                {
                    continue;
                }
                PointsOnSplineExtension.Waypoint waypointToTest = ListToCheck[i].GetClosestWaypointLeft(controller);
                if (waypointToTest == null)
                {
                    continue;
                }

                if (!foundOne)
                {
                    closest = waypointToTest;
                    dist = PointsOnSplineExtension.DistFromWayPoint(waypointToTest, controller);
                    lister = ListToCheck[i];
                    foundOne = true;
                }
                else
                {
                    float distToTest = PointsOnSplineExtension.DistFromWayPoint(waypointToTest, controller);
                    if (distToTest < dist)
                    {
                        closest = waypointToTest;
                        dist = distToTest;
                        lister = ListToCheck[i];
                    }
                }
            }
            return (closest);
        }

        public PointsOnSplineExtension.Waypoint GetClosestWaypointRight(ControllerStick controller, out float dist, out PointsOnSplineExtension lister)
        {
            dist = 0;
            lister = null;
            if (ListToCheck.Length == 0)
            {
                return (null);
            }
            PointsOnSplineExtension.Waypoint closest = null;
            dist = 0;
            lister = null;
            bool foundOne = false;

            for (int i = 0; i < ListToCheck.Length; i++)
            {
                if (ListToCheck[i] == null || !ListToCheck[i].gameObject.activeInHierarchy)
                {
                    continue;
                }
                PointsOnSplineExtension.Waypoint waypointToTest = ListToCheck[i].GetClosestWaypointRight(controller);
                if (waypointToTest == null)
                {
                    continue;
                }

                if (!foundOne)
                {
                    closest = waypointToTest;
                    dist = PointsOnSplineExtension.DistFromWayPoint(waypointToTest, controller);
                    lister = ListToCheck[i];
                    foundOne = true;
                }
                else
                {
                    float distToTest = PointsOnSplineExtension.DistFromWayPoint(waypointToTest, controller);
                    if (distToTest < dist)
                    {
                        closest = waypointToTest;
                        dist = distToTest;
                        lister = ListToCheck[i];
                    }
                }
            }
            return (closest);
        }
    }
}
