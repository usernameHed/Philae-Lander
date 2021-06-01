using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Spline.Controller;
using UnityEssentials.Spline.PointsOnSplineExtensions;
using UnityEssentials.Spline.PropertyAttribute.NoNull;

namespace UnityEssentials.Spline.ControllerExtensions.EventsTrigger
{
    /// <summary>
    ///
    /// </summary>
    public abstract class ControllerEvents : MonoBehaviour
    {
        [SerializeField, NoNull] protected ControllerStickBase _controller = default;
        public ControllerStickBase Controller { get { return (_controller); } }
        public SplineBase Spline { get { return (_controller != null ? _controller.SplineBase : null); } }
        public PointsOnSplineExtension[] ListToCheck = new PointsOnSplineExtension[0];
        [SerializeField] protected float _maxDistanceToActive = 0.2f;

        protected virtual void OnEnable()
        {
            _controller.SplineHasChanged += SplineChanged;
        }

        protected virtual void OnDisable()
        {
            _controller.SplineHasChanged -= SplineChanged;
        }

        public virtual void SplineChanged(SplineBase newSpline)
        {
            ListToCheck = Spline.transform.GetComponentsInChildren<PointsOnSplineExtension>();
        }
    }
}
