using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Spline.Extensions;

namespace UnityEssentials.Spline
{
    /// <summary>
    /// 
    /// </summary>
    public class SplineLister : SingletonMono<SplineLister>
    {
        protected SplineLister() { }

        [SerializeField] private List<SplineBase> _splineLister = new List<SplineBase>(5);
        public List<SplineBase> GetSplineBases() { return (_splineLister); }
        public int SplineCount { get { return (_splineLister.Count); } }

        public void AddSpline(SplineBase spline)
        {
            _splineLister.AddIfNotContain(spline);
        }

        public void RemoveSpline(SplineBase spline)
        {
            _splineLister.Remove(spline);
        }
    }
}