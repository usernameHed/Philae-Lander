using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractorLD : MonoBehaviour
{
    [Serializable]
    public struct PointInfo
    {
        [ReadOnly]
        public Vector3 pos;
        public float gravityBaseRatio;
        public float gravityDownRatio;
        public bool stickIfAlreadyGrounded;

        [Button]
        public void Init()
        {
            gravityBaseRatio = 0.5f;
            gravityDownRatio = 0.3f;
            stickIfAlreadyGrounded = false;
        }
    }

    [Serializable]
    public struct GravityPoint
    {
        [SerializeField]
        public Transform point;
        public float range;

        [SerializeField]
        private PointInfo pointInfo;
        public PointInfo GetPointInfo()
        {
            return (pointInfo);
        }
        public void SetDefautIfFirstTimeCreated()
        {
            if (pointInfo.gravityBaseRatio != 0)
                return;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
        public void ChangePoint(int index, Transform newPoint)
        {
            if (index == 0 && newPoint != null && point.GetInstanceID() != newPoint.GetInstanceID())
            {
                DestroyImmediate(point.gameObject);
                point = newPoint;
            }
        }
    }

    [Serializable]
    public struct GravityLine
    {
        public Transform pointA;
        public Transform pointB;
        public float range;

        [SerializeField]
        private PointInfo pointInfo;
        public PointInfo GetPointInfo()
        {
            return (pointInfo);
        }
        public void SetDefautIfFirstTimeCreated()
        {
            if (pointInfo.gravityBaseRatio != 0)
                return;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
        public void ChangePoint(int index, Transform newPoint)
        {
            if (index == 0 && newPoint != null && pointA.GetInstanceID() != newPoint.GetInstanceID())
            {
                if (pointA != null)
                    DestroyImmediate(pointA.gameObject);
                pointA = newPoint;
            }
            else if (index == 1 && newPoint != null && pointB.GetInstanceID() != newPoint.GetInstanceID())
            {
                if (pointB != null)
                    DestroyImmediate(pointB.gameObject);
                pointB = newPoint;
            }
        }
    }
    [Serializable]
    public struct GravityTriangle
    {
        public Transform pointA;
        public Transform pointB;
        public Transform pointC;
        public float range;

        public bool unidirectionnal;    //n'est valide seulement dans un seul sens
        [EnableIf("unidirectionnal")]
        public bool inverseDirection;   //inverser la direction si on est en unidirectionnal
        [DisableIf("noGravityBorders")]
        public bool infinitePlane;      //définir ce plan comme infini
        [DisableIf("infinitePlane")]
        public bool noGravityBorders;   //si on est pas dans le plan, mais sur les bords, retourner null

        [SerializeField]
        private PointInfo pointInfo;
        public PointInfo GetPointInfo()
        {
            return (pointInfo);
        }
        public void SetDefautIfFirstTimeCreated()
        {
            if (pointInfo.gravityBaseRatio != 0)
                return;

            unidirectionnal = false;
            inverseDirection = false;
            infinitePlane = false;
            noGravityBorders = false;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
        public void ChangePoint(int index, Transform newPoint)
        {
            if (index == 0 && newPoint != null && pointA.GetInstanceID() != newPoint.GetInstanceID())
            {
                if (pointA != null)
                    DestroyImmediate(pointA.gameObject);
                pointA = newPoint;
            }
            else if (index == 1 && newPoint != null && pointB.GetInstanceID() != newPoint.GetInstanceID())
            {
                if (pointB != null)
                    DestroyImmediate(pointB.gameObject);
                pointB = newPoint;
            }
            else if (index == 2 && newPoint != null && pointC.GetInstanceID() != newPoint.GetInstanceID())
            {
                if (pointC != null)
                    DestroyImmediate(pointC.gameObject);
                pointC = newPoint;
            }
        }
    }
    [Serializable]
    public struct GravityQuad
    {
        public Transform pointA;
        public Transform pointB;
        public Transform pointC;
        public Transform pointD;
        public float range;

        public bool unidirectionnal;    //n'est valide seulement dans un seul sens
        [EnableIf("unidirectionnal")]
        public bool inverseDirection;   //inverser la direction si on est en unidirectionnal
        [DisableIf("noGravityBorders")]
        public bool infinitePlane;      //définir ce plan comme infini
        [DisableIf("infinitePlane")]
        public bool noGravityBorders;   //si on est pas dans le plan, mais sur les bords, retourner null


        [SerializeField]
        private PointInfo pointInfo;
        public PointInfo GetPointInfo()
        {
            return (pointInfo);
        }
        public void SetDefautIfFirstTimeCreated()
        {
            if (pointInfo.gravityBaseRatio != 0)
                return;

            unidirectionnal = false;
            inverseDirection = false;
            infinitePlane = false;
            noGravityBorders = false;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
        public void ChangePoint(int index, Transform newPoint)
        {
            if (index == 0 && newPoint != null && pointA.GetInstanceID() != newPoint.GetInstanceID())
            {
                if (pointA != null)
                    DestroyImmediate(pointA.gameObject);
                pointA = newPoint;
            }
            else if (index == 1 && newPoint != null && pointB.GetInstanceID() != newPoint.GetInstanceID())
            {
                if (pointB != null)
                    DestroyImmediate(pointB.gameObject);
                pointB = newPoint;
            }
            else if (index == 2 && newPoint != null && pointC.GetInstanceID() != newPoint.GetInstanceID())
            {
                if (pointC != null)
                    DestroyImmediate(pointC.gameObject);
                pointC = newPoint;
            }
            else if (index == 3 && newPoint != null && pointD.GetInstanceID() != newPoint.GetInstanceID())
            {
                if (pointD != null)
                    DestroyImmediate(pointD.gameObject);
                pointD = newPoint;
            }
        }
    }

    public enum GravityPointType
    {
        POINT = 0,
        LINE = 1,
        TRIANGLE = 2,
        QUAD = 3,
    }

    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityPoint> gravityPoints = new List<GravityPoint>();
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityLine> gravityLines = new List<GravityLine>();
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityTriangle> gravityTriangles = new List<GravityTriangle>();
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityQuad> gravityQuad = new List<GravityQuad>();

    [FoldoutGroup("Object"), SerializeField]
    public Transform gravityAttractorEditor;

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
    public void CreateEditor()
    {
        if (!gameObject.HasComponent<GravityAttractorEditor>())
            gameObject.AddComponent(typeof(GravityAttractorEditor));
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

    [Button]
    public void RemoveEditor()
    {
        if (gameObject.HasComponent<GravityAttractorEditor>())
        {
            DestroyImmediate(gameObject.GetComponent<GravityAttractorEditor>());
        } 
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

            ExtTriangle triangle = new ExtTriangle(gravityTriangles[i].pointA.position, gravityTriangles[i].pointB.position, gravityTriangles[i].pointC.position,
                gravityTriangles[i].unidirectionnal,
                gravityTriangles[i].inverseDirection,
                gravityTriangles[i].infinitePlane,
                gravityTriangles[i].noGravityBorders);
            arrayPointsTriangles[i] = triangle.ClosestPointTo(posEntity);
        }
        Vector3 closestFound = ExtUtilityFunction.GetClosestPoint(posEntity, arrayPointsTriangles, ref indexFound);

        //if (ExtUtilityFunction.IsNullVector(closestFound))
        //    return (closestFound);

        return (closestFound);
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
            ExtTriangle triangleA = new ExtTriangle(gravityQuad[i].pointA.position, gravityQuad[i].pointB.position, gravityQuad[i].pointC.position,
                gravityQuad[i].unidirectionnal,
                gravityQuad[i].inverseDirection,
                gravityQuad[i].infinitePlane,
                gravityQuad[i].noGravityBorders);
            arrayPointsQuads[i] = triangleA.ClosestPointTo(posEntity);

            ExtTriangle triangleB = new ExtTriangle(gravityQuad[i].pointC.position, gravityQuad[i].pointD.position, gravityQuad[i].pointA.position,
                gravityQuad[i].unidirectionnal,
                gravityQuad[i].inverseDirection,
                gravityQuad[i].infinitePlane,
                gravityQuad[i].noGravityBorders);
            arrayPointsQuads[i + gravityQuad.Count] = triangleB.ClosestPointTo(posEntity);

        }
        Vector3 closestFound = ExtUtilityFunction.GetClosestPoint(posEntity, arrayPointsQuads, ref indexFound);

        if (ExtUtilityFunction.IsNullVector(closestFound))
            return (closestFound);

        //Debug.Log("closest found: " + closestFound);
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
            allResult[indexResult].gravityBaseRatio = gravityPoints[indexFound].GetPointInfo().gravityBaseRatio;
            allResult[indexResult].gravityDownRatio = gravityPoints[indexFound].GetPointInfo().gravityDownRatio;
            allResult[indexResult].stickIfAlreadyGrounded = gravityPoints[indexFound].GetPointInfo().stickIfAlreadyGrounded;


            allResultPos[indexResult] = closestPoint;
            indexResult++;
        }

        //if there are lines, add to result
        if (arrayPointsLines.Length > 0 && gravityLines.Count > 0 && gravityLines.Count == arrayPointsLines.Length)
        {
            Vector3 closestPointLines = GetClosestPointFromLines(fromPoint);
            //Debug.DrawLine(fromPoint, closestPointLines, Color.cyan);
            allResult[indexResult].pos = closestPointLines;
            allResult[indexResult].gravityBaseRatio = gravityLines[indexFound].GetPointInfo().gravityBaseRatio;
            allResult[indexResult].gravityDownRatio = gravityLines[indexFound].GetPointInfo().gravityDownRatio;
            allResult[indexResult].stickIfAlreadyGrounded = gravityLines[indexFound].GetPointInfo().stickIfAlreadyGrounded;


            allResultPos[indexResult] = closestPointLines;
            indexResult++;
        }

        //if there are triangles, add to result
        if (arrayPointsTriangles.Length > 0 && gravityTriangles.Count > 0 && gravityTriangles.Count == arrayPointsTriangles.Length)
        {
            Vector3 closestPointTriangles = GetClosestPointFromTriangles(fromPoint);
            //Debug.DrawLine(fromPoint, closestPointTriangles, Color.green);
            allResult[indexResult].pos = closestPointTriangles;
            if (indexFound != -1)
            {
                allResult[indexResult].gravityBaseRatio = gravityTriangles[indexFound].GetPointInfo().gravityBaseRatio;
                allResult[indexResult].gravityDownRatio = gravityTriangles[indexFound].GetPointInfo().gravityDownRatio;
                allResult[indexResult].stickIfAlreadyGrounded = gravityTriangles[indexFound].GetPointInfo().stickIfAlreadyGrounded;
            }
            allResultPos[indexResult] = closestPointTriangles;
            indexResult++;
        }

        //if there are Quads, add to result
        if (arrayPointsQuads.Length > 0 && gravityQuad.Count > 0 && gravityQuad.Count * 2 == arrayPointsQuads.Length)
        {
            Vector3 closestPointQuads = GetClosestPointFromQuads(fromPoint);

            allResult[indexResult].pos = closestPointQuads;
            if (indexFound != -1)
            {
                allResult[indexResult].gravityBaseRatio = gravityQuad[indexFound].GetPointInfo().gravityBaseRatio;
                allResult[indexResult].gravityDownRatio = gravityQuad[indexFound].GetPointInfo().gravityDownRatio;
                allResult[indexResult].stickIfAlreadyGrounded = gravityQuad[indexFound].GetPointInfo().stickIfAlreadyGrounded;
            }
            allResultPos[indexResult] = closestPointQuads;
            indexResult++;
        }


        if (indexResult > 0)
        {
            pointInfo.pos = ExtUtilityFunction.GetClosestPoint(fromPoint, allResultPos, ref indexFound);

            if (indexFound != -1)
            {
                pointInfo.gravityBaseRatio = allResult[indexFound].gravityBaseRatio;
                pointInfo.gravityDownRatio = allResult[indexFound].gravityDownRatio;
                pointInfo.stickIfAlreadyGrounded = allResult[indexFound].stickIfAlreadyGrounded;
                Debug.DrawLine(fromPoint, pointInfo.pos, Color.green, 5f);
            }
        }
        return (pointInfo);
    }
}
