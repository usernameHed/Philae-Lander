using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Spline.PointsOnSplineExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public class TriggerActionPoint : MonoBehaviour
    {
        public delegate void OnTriggerPoints(ActionPoints.ActionPointsWaypoint action, object parameters);
        public event OnTriggerPoints OnTriggerPoint;

        public void Trigger(ActionPoints.ActionPointsWaypoint action, object parameters)
        {
            //Debug.Log("trigge point " + action.Index + " of " + action.PointLister.Description + " at: " + action.PathPosition, action.ReferenceAction);
            OnTriggerPoint?.Invoke(action, parameters);
#if UNITY_EDITOR
            //ExtDrawGuizmos.DebugWireSphere(transform.position, action.PointLister.ColorWayPoint, 0.5f, 0.5f);
            ExtDrawGuizmos.DebugWireSphere(action.PointLister.SplineBase.EvaluatePositionAtUnit(action.PathPosition, action.PointLister.PositionUnits), action.PointLister.ColorWayPoint, 1, 0.5f);
#endif
        }
    }
}