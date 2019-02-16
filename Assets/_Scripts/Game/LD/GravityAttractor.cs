using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    [Serializable]
    public struct PointInfo
    {
        [ReadOnly]
        public Vector3 pos;
        public float gravityRatio;
        public bool noGravity;

        public void Init()
        {
            gravityRatio = 1f;
            noGravity = false;
        }
    }

    [Serializable]
    public struct GravityPoint
    {
        [SerializeField]
        public Transform point;

        [SerializeField]
        private PointInfo pointInfo;
        public PointInfo GetPointInfo()
        {
            return (pointInfo);
        }
        public void SetDefautIfFirstTimeCreated()
        {
            if (pointInfo.gravityRatio != 0)
                return;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
    }

    [Serializable]
    public struct GravityLine
    {
        public Transform pointA;
        public Transform pointB;

        [SerializeField]
        private PointInfo pointInfo;
        public PointInfo GetPointInfo()
        {
            return (pointInfo);
        }
        public void SetDefautIfFirstTimeCreated()
        {
            if (pointInfo.gravityRatio != 0)
                return;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
    }
    [Serializable]
    public struct GravityTriangle
    {
        public Transform pointA;
        public Transform pointB;
        public Transform pointC;

        [SerializeField]
        private PointInfo pointInfo;
        public PointInfo GetPointInfo()
        {
            return (pointInfo);
        }
        public void SetDefautIfFirstTimeCreated()
        {
            if (pointInfo.gravityRatio != 0)
                return;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
    }
    [Serializable]
    public struct GravityQuad
    {
        public Transform pointA;
        public Transform pointB;
        public Transform pointC;
        public Transform pointD;

        [SerializeField]
        private PointInfo pointInfo;
        public PointInfo GetPointInfo()
        {
            return (pointInfo);
        }
        public void SetDefautIfFirstTimeCreated()
        {
            if (pointInfo.gravityRatio != 0)
                return;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
    }

    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityPoint> gravityPoints = new List<GravityPoint>();
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityLine> gravityLines = new List<GravityLine>();
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityTriangle> gravityTriangles = new List<GravityTriangle>();
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityQuad> gravityQuad = new List<GravityQuad>();


    [FoldoutGroup("Debug"), SerializeField, ReadOnly]
    private PointInfo pointInfo = new PointInfo();
    [FoldoutGroup("Debug"), SerializeField, ReadOnly]
    public bool valueArrayChanged = false;


    private Vector3[] arrayPoints;
    private Vector3[] arrayPointsLines;
    private Vector3[] arrayPointsTriangles;
    private Vector3[] arrayPointsQuads;
    private PointInfo[] allResult;
    private Vector3[] allResultPos;
    private int indexFound = -1;

    

    private void Start()
    {
        SetupArrayPoints();
    }

    [Button]
    public void SetupArrayPoints()
    {
        int indexAllResult = 0;
        
        arrayPoints = new Vector3[gravityPoints.Count];
        arrayPointsLines = new Vector3[gravityLines.Count];
        arrayPointsTriangles = new Vector3[gravityTriangles.Count];
        arrayPointsQuads = new Vector3[gravityQuad.Count * 2];   //quad are actualy just 2 triangle

        indexAllResult += (gravityPoints.Count > 0) ? 1 : 0;
        indexAllResult += (gravityLines.Count > 0) ? 1 : 0;
        indexAllResult += (gravityTriangles.Count > 0) ? 1 : 0;
        indexAllResult += (gravityQuad.Count > 0) ? 1 : 0;

        allResult = new PointInfo[indexAllResult];
        allResultPos = new Vector3[indexAllResult];

        valueArrayChanged = true;
    }

    public void SelectedGravityAttractor()
    {
        Debug.Log(gameObject.name + " selected !" + gameObject);
        //lastListFound.Clear();
    }

    public void UnselectGravityAttractor()
    {
        Debug.Log(gameObject.name + " un-selected !" + gameObject);
        //lastListFound.Clear();
    }
    
    private Vector3 GetClosestPointFromGravityPoints(Vector3 posEntity)
    {
        for (int i = 0; i < gravityPoints.Count; i++)
        {
            if (!gravityPoints[i].point)
                continue;
            arrayPoints[i] = gravityPoints[i].point.position;
        }
        return (ExtUtilityFunction.GetClosestPoint(posEntity, arrayPoints, ref indexFound));
    }

    /// <summary>
    /// loop through all lines
    /// </summary>
    /// <param name="posEntity"></param>
    /// <returns></returns>
    private Vector3 GetClosestPointFromLines(Vector3 posEntity)
    {
        for (int i = 0; i < gravityLines.Count; i++)
        {
            if (!gravityLines[i].pointA || !gravityLines[i].pointB)
                continue;

            ExtLine line = new ExtLine(gravityLines[i].pointA.position, gravityLines[i].pointB.position);
            arrayPointsLines[i] = line.ClosestPointTo(posEntity);
        }

        return (ExtUtilityFunction.GetClosestPoint(posEntity, arrayPointsLines, ref indexFound));
    }

    /// <summary>
    /// loop through all triangles
    /// </summary>
    /// <param name="posEntity"></param>
    /// <returns></returns>
    private Vector3 GetClosestPointFromTriangles(Vector3 posEntity)
    {
        for (int i = 0; i < gravityTriangles.Count; i++)
        {
            if (!gravityTriangles[i].pointA || !gravityTriangles[i].pointB || !gravityTriangles[i].pointC)
                continue;

            ExtTriangle triangle = new ExtTriangle(gravityTriangles[i].pointA.position, gravityTriangles[i].pointB.position, gravityTriangles[i].pointC.position);
            arrayPointsTriangles[i] = triangle.ClosestPointTo(posEntity);
        }
        
        return (ExtUtilityFunction.GetClosestPoint(posEntity, arrayPointsTriangles, ref indexFound));
    }

    /// <summary>
    /// loop through all Quad
    /// </summary>
    private Vector3 GetClosestPointFromQuads(Vector3 posEntity)
    {
        for (int i = 0; i < gravityQuad.Count; i++)
        {
            if (!gravityQuad[i].pointA || !gravityQuad[i].pointB || !gravityQuad[i].pointC || !gravityQuad[i].pointD)
                continue;

            //create 2 triangle.
            ExtTriangle triangleA = new ExtTriangle(gravityQuad[i].pointA.position, gravityQuad[i].pointB.position, gravityQuad[i].pointC.position);
            arrayPointsQuads[i] = triangleA.ClosestPointTo(posEntity);
            ExtTriangle triangleB = new ExtTriangle(gravityQuad[i].pointC.position, gravityQuad[i].pointD.position, gravityQuad[i].pointA.position);
            arrayPointsQuads[i + gravityQuad.Count] = triangleB.ClosestPointTo(posEntity);
        }
        Vector3 closestFound = ExtUtilityFunction.GetClosestPoint(posEntity, arrayPointsQuads, ref indexFound);
        indexFound = indexFound % gravityQuad.Count;
        return (closestFound);
    }

    /// <summary>
    /// find the nearest point from a groupe of points, or lines, or triangles, or quad
    /// </summary>
    public PointInfo FindNearestPoint(Vector3 fromPoint)
    {
        if (allResult.Length == 0)
            return (pointInfo);

        int indexResult = 0;

        //if there are individual points, add to result
        if (arrayPoints.Length > 0 && gravityPoints.Count > 0 && gravityPoints.Count == arrayPoints.Length)
        {
            Vector3 closestPoint = GetClosestPointFromGravityPoints(fromPoint);
            //Debug.DrawLine(fromPoint, closestPoint, Color.white);
            allResult[indexResult].pos = closestPoint;
            allResult[indexResult].gravityRatio = gravityPoints[indexFound].GetPointInfo().gravityRatio;
            allResult[indexResult].noGravity = gravityPoints[indexFound].GetPointInfo().noGravity;


            allResultPos[indexResult] = closestPoint;
            indexResult++;
        }

        //if there are lines, add to result
        if (arrayPointsLines.Length > 0 && gravityLines.Count > 0 && gravityLines.Count == arrayPointsLines.Length)
        {
            Vector3 closestPointLines = GetClosestPointFromLines(fromPoint);
            //Debug.DrawLine(fromPoint, closestPointLines, Color.cyan);
            allResult[indexResult].pos = closestPointLines;
            allResult[indexResult].gravityRatio = gravityLines[indexFound].GetPointInfo().gravityRatio;
            allResult[indexResult].noGravity = gravityLines[indexFound].GetPointInfo().noGravity;


            allResultPos[indexResult] = closestPointLines;
            indexResult++;
        }

        //if there are triangles, add to result
        if (arrayPointsTriangles.Length > 0 && gravityTriangles.Count > 0 && gravityTriangles.Count == arrayPointsTriangles.Length)
        {
            Vector3 closestPointTriangles = GetClosestPointFromTriangles(fromPoint);
            //Debug.DrawLine(fromPoint, closestPointTriangles, Color.green);
            allResult[indexResult].pos = closestPointTriangles;
            allResult[indexResult].gravityRatio = gravityTriangles[indexFound].GetPointInfo().gravityRatio;
            allResult[indexResult].noGravity = gravityTriangles[indexFound].GetPointInfo().noGravity;

            allResultPos[indexResult] = closestPointTriangles;
            indexResult++;
        }

        //if there are Quads, add to result
        if (arrayPointsQuads.Length > 0 && gravityQuad.Count > 0 && gravityQuad.Count == arrayPointsQuads.Length)
        {
            Vector3 closestPointQuads = GetClosestPointFromQuads(fromPoint);
            //Debug.DrawLine(fromPoint, closestPointQuads, Color.red);
            allResult[indexResult].pos = closestPointQuads;
            allResult[indexResult].gravityRatio = gravityQuad[indexFound].GetPointInfo().gravityRatio;
            allResult[indexResult].noGravity = gravityQuad[indexFound].GetPointInfo().noGravity;

            allResultPos[indexResult] = closestPointQuads;
            indexResult++;
        }


        if (indexResult > 0)
        {
            pointInfo.pos = ExtUtilityFunction.GetClosestPoint(fromPoint, allResultPos, ref indexFound);
            pointInfo.gravityRatio = allResult[indexFound].gravityRatio;
            pointInfo.noGravity = allResult[indexFound].noGravity;
            Debug.DrawLine(fromPoint, pointInfo.pos, Color.red);
        }
        return (pointInfo);
    }
}
