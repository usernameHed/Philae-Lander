using UnityEngine;
using System;
using UnityEssentials.Spline.PropertyAttribute.NoNull;
using System.Collections.Generic;
using UnityEssentials.Spline.Controller;

namespace UnityEssentials.Spline.PointsOnSplineExtensions
{
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Unity Essentials/Spline/Points Lister/Chunk Linker")]
    [RequireComponent(typeof(PointsOnSplineExtension))]
    public class ChunkLinker : MonoBehaviour
    {
        [Serializable]
        public class Chunk
        {
            public string Description = "Description";
            public int A;
            public int B;
            public Color Color = Color.cyan;
            public TriggerActionChunk ActionChunk;

            public float PositionA(PointsOnSplineExtension points)
            {
                return (points == null ? 0 : points.GetWayPoint(A).PathPosition);
            }
            public float PositionB(PointsOnSplineExtension points)
            {
                return (points == null ? 0 : points.GetWayPoint(B).PathPosition);
            }

            /// <summary>
            /// if samePosition, that mean the entire spline is a triggerStay
            /// </summary>
            /// <returns></returns>
            public bool SamePosition()
            {
                return (A == B);
            }

            public bool IsControllerInsideChunk(ControllerStickBase controller, PointsOnSplineExtension points)
            {
                if (SamePosition())
                {
                    return (true);
                }

                //current data
                SplineBase.PositionUnits unitPointsOnSpline = points.PositionUnits;
                SplineBase.PositionUnits unitController = controller.PositionUnits;
                float currentPosition = controller.PathPosition;



                //3 points with same units (units of the chunks points, default by Distance)
                currentPosition = controller.SplineBase.ConvertPathUnit(currentPosition, unitController, unitPointsOnSpline);
                float a = PositionA(points);
                float b = PositionB(points);

                //return true if we are inside a chunk
                if (b < a)
                {
                    return (currentPosition <= b || currentPosition >= a);
                }
                else
                {
                    return (currentPosition >= a && currentPosition <= b);
                }
            }
        }

        [SerializeField, NoNull] private PointsOnSplineExtension _points = default;
        [SerializeField] private Chunk[] _chunks;

        public virtual TriggerActionChunk GetReference(int index)
        {
            if (index < 0 || index >= _chunks.Length)
            {
                return (null);
            }
            return (_chunks[index].ActionChunk);
        }
        [SerializeField, Tooltip("Show Gizmos (waypoints, line and everything)")] private bool _showWayPoints = true; public bool ShowWayPoints { get { return (_showWayPoints); } }
        [SerializeField, Tooltip("Show gizmos even when not selected")] private bool _showWayPointsWhenUnselected = true; public bool ShowWayPointsWhenUnselected { get { return (_showWayPointsWhenUnselected); } }
        [SerializeField, Tooltip("When you create a chunk, duplicate the current selected one")] private bool _duplicateCurrentSelectedOnCreation = true;
        [SerializeField, Tooltip("When removing a chunk, remove also the Target Reference")] private bool _removeReferenceWhenRemovingPoint = true;
        [SerializeField, HideInInspector] private bool _foldout = true;


        public PointsOnSplineExtension Points { get { return (_points); } }

        public SplineBase SplineBase { get { return (_points == null ? null : _points.SplineBase); } }

        public int ChunkCount { get { return (_chunks.Length); } }
        public Chunk GetChunk(int index) { return (_chunks[index]); }
        public SplineBase.PositionUnits PositionUnits { get { return (_points.PositionUnits); } }
        [SerializeField] private float _offsetChunks = 0.6f; public float OffsetChunks { get { return (_offsetChunks); } }
    }
}