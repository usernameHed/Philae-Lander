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
        [OnValueChanged("OnMaxRangeChange")]
        public float range;
        [OnValueChanged("OnMaxRangeChange")]
        public float maxRange;

        [HideInInspector]
        public Vector3 posRange;
        [HideInInspector]
        public Vector3 sphereGravity;
        [HideInInspector]
        public GravityAttractorLD refGA;

        public void OnMaxRangeChange()
        {
            if (maxRange < range)
                maxRange = range;
        }

        [Button]
        public void Init()
        {
            gravityBaseRatio = 0.5f;
            gravityDownRatio = 0.3f;
            range = 0f;
            maxRange = 0f;
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
            if (pointInfo.gravityBaseRatio != 0)
                return;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
        public void ChangePoint(int index, Transform newPoint)
        {
            if (index == 0 && newPoint != null)
            {
                point = newPoint;
            }
        }
    }

    [Serializable]
    public struct GravityLine
    {
        public Transform pointA;
        public Transform pointB;

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

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
        public void ChangePoint(int index, Transform newPoint)
        {
            if (index == 0 && newPoint != null)
            {
                pointA = newPoint;
            }
            else if (index == 1 && newPoint != null)
            {
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

        public bool unidirectionnal;    //n'est valide seulement dans un seul sens
        [EnableIf("unidirectionnal")]
        public bool inverseDirection;   //inverser la direction si on est en unidirectionnal
        public bool noGravityBorders;   //si on est pas dans le plan, mais sur les bords, retourner null

        [EnableIf("noGravityBorders")]
        [DisableIf("isQuad")]
        public bool calculateAB;
        [EnableIf("noGravityBorders")]
        [DisableIf("isQuad")]
        public bool calculateBC;
        [EnableIf("noGravityBorders")]
        [DisableIf("isQuad")]
        public bool calculateCA;
        [EnableIf("noGravityBorders")]
        [DisableIf("isQuad")]
        public bool calculateCorner;

        public bool isQuad;


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
            noGravityBorders = false;
            isQuad = false;

            calculateAB = false;
            calculateBC = false;
            calculateCA = false;
            calculateCorner = false;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
        public void ChangePoint(int index, Transform newPoint)
        {
            if (index == 0 && newPoint != null)
            {
                pointA = newPoint;
            }
            else if (index == 1 && newPoint != null)
            {
                pointB = newPoint;
            }
            else if (index == 2 && newPoint != null)
            {
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

        public bool unidirectionnal;    //n'est valide seulement dans un seul sens
        [EnableIf("unidirectionnal")]
        public bool inverseDirection;   //inverser la direction si on est en unidirectionnal
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
            noGravityBorders = false;

            pointInfo = new PointInfo();
            pointInfo.Init();
            Debug.Log("Default value changed");
        }
        public void ChangePoint(int index, Transform newPoint)
        {
            if (index == 0 && newPoint != null)
            {
                pointA = newPoint;
            }
            else if (index == 1 && newPoint != null)
            {
                pointB = newPoint;
            }
            else if (index == 2 && newPoint != null)
            {
                pointC = newPoint;
            }
            else if (index == 3 && newPoint != null)
            {
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
    public bool isMovingLD = false;
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityPoint> gravityPoints = new List<GravityPoint>();
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityLine> gravityLines = new List<GravityLine>();
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityTriangle> gravityTrianglesOrQuad = new List<GravityTriangle>();
    [FoldoutGroup("GamePlay"), OnValueChanged("SetupArrayPoints"), Tooltip(""), SerializeField]
    public List<GravityQuad> gravityTetra = new List<GravityQuad>();

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
    private Vector3[] allResultPosRange;
    private int indexFound = -1;

    private Vector3[] arrayPointsLinesCenterTmp;
    private Vector3[] arrayPointsTrianglesCenterTmp;
    private Vector3[] arrayPointsQuadsCenterTmp;

    private ExtTetra[] arrayExtTetra;
    private ExtTriangleOrQuad[] arrayExtTrianglesOrQuad;
    private ExtLine[] arrayExtLines;


    private void Start()
    {
        SetupArrayPoints();
    }

    [Button]
    public void CreateEditor()
    {
        if (!gameObject.HasComponent<GravityAttractorEditor>())
        {
            gameObject.AddComponent(typeof(GravityAttractorEditor));
        }            
    }

    [Button]
    public void SetupArrayPoints()
    {
        int indexAllResult = 0;
        
        arrayPoints = ExtUtilityFunction.CreateNullVectorArray(gravityPoints.Count);
        arrayPointsLines = ExtUtilityFunction.CreateNullVectorArray(gravityLines.Count);
        arrayPointsTriangles = ExtUtilityFunction.CreateNullVectorArray(gravityTrianglesOrQuad.Count);
        arrayPointsQuads = ExtUtilityFunction.CreateNullVectorArray(gravityTetra.Count);

        arrayPointsLinesCenterTmp = ExtUtilityFunction.CreateNullVectorArray(gravityLines.Count);
        arrayPointsTrianglesCenterTmp = ExtUtilityFunction.CreateNullVectorArray(gravityTrianglesOrQuad.Count);
        arrayPointsQuadsCenterTmp = ExtUtilityFunction.CreateNullVectorArray(gravityTetra.Count);

        indexAllResult += (gravityPoints.Count > 0) ? 1 : 0;
        indexAllResult += (gravityLines.Count > 0) ? 1 : 0;
        indexAllResult += (gravityTrianglesOrQuad.Count > 0) ? 1 : 0;
        indexAllResult += (gravityTetra.Count > 0) ? 1 : 0;

        allResult = new PointInfo[indexAllResult];
        allResultPos = ExtUtilityFunction.CreateNullVectorArray(indexAllResult);
        allResultPosRange = ExtUtilityFunction.CreateNullVectorArray(indexAllResult);

        valueArrayChanged = true;

        PrecalculAll();
    }

    [Button]
    public void PrecalculAll()
    {
        arrayExtLines = new ExtLine[gravityLines.Count];
        arrayExtTrianglesOrQuad = new ExtTriangleOrQuad[gravityTrianglesOrQuad.Count];
        arrayExtTetra = new ExtTetra[gravityTetra.Count];

        for (int i = 0; i < gravityLines.Count; i++)
        {
            //create a line
            arrayExtLines[i] = new ExtLine(gravityLines[i].pointA.position, gravityLines[i].pointB.position, gravityLines[i].noGravityBorders);
        }
        for (int i = 0; i < gravityTrianglesOrQuad.Count; i++)
        {
            //create a triangleOrQuad
            arrayExtTrianglesOrQuad[i] = new ExtTriangleOrQuad(gravityTrianglesOrQuad[i].pointA.position, gravityTrianglesOrQuad[i].pointB.position, gravityTrianglesOrQuad[i].pointC.position,
                gravityTrianglesOrQuad[i].unidirectionnal,
                gravityTrianglesOrQuad[i].inverseDirection,
                gravityTrianglesOrQuad[i].noGravityBorders,
                gravityTrianglesOrQuad[i].calculateAB,
                gravityTrianglesOrQuad[i].calculateBC,
                gravityTrianglesOrQuad[i].calculateCA,
                gravityTrianglesOrQuad[i].calculateCorner,

                gravityTrianglesOrQuad[i].isQuad);
        }
        for (int i = 0; i < gravityTetra.Count; i++)
        {
            //create a tetra
            arrayExtTetra[i] = new ExtTetra(gravityTetra[i].pointA.position, gravityTetra[i].pointB.position, gravityTetra[i].pointC.position, gravityTetra[i].pointD.position,
                gravityTetra[i].unidirectionnal,
                gravityTetra[i].inverseDirection,
                gravityTetra[i].noGravityBorders);
        }

    }

    

    [Button]
    public void RemoveEditor()
    {
        if (gameObject.HasComponent<GravityAttractorEditor>())
        {
            DestroyImmediate(gameObject.GetComponent<GravityAttractorEditor>());
        } 
    }

    private Vector3 GetRightPosWithRange(Vector3 posEntity, Vector3 posCenter, float range, float maxRange)
    {
        Vector3 posFound = posCenter;
        float lenghtCenterToPlayer = 0;

        if (range > 0)
        {
            Vector3 realPos = posCenter + (posEntity - posCenter).normalized * range;
            lenghtCenterToPlayer = (posEntity - posCenter).sqrMagnitude;
            float lenghtCenterToRangeMax = (realPos - posCenter).sqrMagnitude;
            if (lenghtCenterToRangeMax > lenghtCenterToPlayer)
            {
                realPos = posEntity;
            }

            posFound = realPos;
        }

        //if player is out of range, return null
        if (maxRange > range)
        {
            Vector3 realPos = posCenter + (posEntity - posCenter).normalized * maxRange;
            //calculate only if we havn't already calculate
            if (range == 0)
                lenghtCenterToPlayer = (posEntity - posCenter).sqrMagnitude;
            float lenghtCenterToRangeMax = (realPos - posCenter).sqrMagnitude;
            if (lenghtCenterToPlayer > lenghtCenterToRangeMax)
            {
                posFound = ExtUtilityFunction.GetNullVector();
            }

        }

        return (posFound);
    }

    private void GetClosestPointFromGravityPoints(Vector3 posEntity, ref Vector3 closestPoint, ref Vector3 closestRangePoint)
    {
        //ExtUtilityFunction.FillArrayWithWrongVector(ref arrayPoints);

        for (int i = 0; i < gravityPoints.Count; i++)
        {
            //if (!gravityPoints[i].point)
            //    continue;
            //arrayPoints[i] = gravityPoints[i].point.position;
            arrayPoints[i] = GetRightPosWithRange(posEntity, gravityPoints[i].point.position, gravityPoints[i].GetPointInfo().range, gravityPoints[i].GetPointInfo().maxRange);
        }
        //closestRangePoint = ExtUtilityFunction.GetClosestPoint(posEntity, arrayPoints, ref indexFound);
        //closestPoint = gravityPoints[indexFound].point.position;

        closestRangePoint = ExtUtilityFunction.GetClosestPoint(posEntity, arrayPoints, ref indexFound);
        closestPoint = gravityPoints[indexFound].point.position;
    }

    /// <summary>
    /// loop through all lines
    /// </summary>
    /// <param name="posEntity"></param>
    /// <returns></returns>
    private void GetClosestPointFromLines(Vector3 posEntity, ref Vector3 closestPoint, ref Vector3 closestRangePoint)
    {
        //ExtUtilityFunction.FillArrayWithWrongVector(ref arrayPointsLines);
        //ExtUtilityFunction.FillArrayWithWrongVector(ref arrayPointsLinesCenterTmp);

        for (int i = 0; i < gravityLines.Count; i++)
        {
            //if (!gravityLines[i].pointA || !gravityLines[i].pointB)
            //    continue;

            //ExtLine line = new ExtLine(gravityLines[i].pointA.position, gravityLines[i].pointB.position, gravityLines[i].noGravityBorders);
            arrayPointsLinesCenterTmp[i] = arrayExtLines[i].ClosestPointTo(posEntity);
            arrayPointsLines[i] = GetRightPosWithRange(posEntity, arrayPointsLinesCenterTmp[i], gravityLines[i].GetPointInfo().range, gravityLines[i].GetPointInfo().maxRange);
        }
        closestRangePoint = ExtUtilityFunction.GetClosestPoint(posEntity, arrayPointsLines, ref indexFound);
        if (indexFound != -1)
            closestPoint = arrayPointsLinesCenterTmp[indexFound];
    }

    /// <summary>
    /// loop through all triangles
    /// </summary>
    /// <param name="posEntity"></param>
    /// <returns></returns>
    private void GetClosestPointFromTrianglesOrQuad(Vector3 posEntity, ref Vector3 closestPoint, ref Vector3 closestRangePoint)
    {
        //ExtUtilityFunction.FillArrayWithWrongVector(ref arrayPointsTriangles);
        //ExtUtilityFunction.FillArrayWithWrongVector(ref arrayPointsTrianglesCenterTmp);

        for (int i = 0; i < gravityTrianglesOrQuad.Count; i++)
        {
            arrayPointsTrianglesCenterTmp[i] = arrayExtTrianglesOrQuad[i].ClosestPointTo(posEntity);
            arrayPointsTriangles[i] = GetRightPosWithRange(posEntity, arrayPointsTrianglesCenterTmp[i], gravityTrianglesOrQuad[i].GetPointInfo().range, gravityTrianglesOrQuad[i].GetPointInfo().maxRange);
        }
        closestRangePoint = ExtUtilityFunction.GetClosestPoint(posEntity, arrayPointsTriangles, ref indexFound);
        if (indexFound != -1)
            closestPoint = arrayPointsTrianglesCenterTmp[indexFound];
        //return (closestFound);
    }

    /// <summary>
    /// loop through all Quad
    /// </summary>
    private void GetClosestPointFromTetras(Vector3 posEntity, ref Vector3 closestPoint, ref Vector3 closestRangePoint)
    {
        //ExtUtilityFunction.FillArrayWithWrongVector(ref arrayPointsQuads);
        //ExtUtilityFunction.FillArrayWithWrongVector(ref arrayPointsQuadsCenterTmp);
        
        for (int i = 0; i < gravityTetra.Count; i++)
        {
            arrayPointsQuadsCenterTmp[i] = arrayExtTetra[i].ClosestPtPointRect(posEntity);
            arrayPointsQuads[i] = GetRightPosWithRange(posEntity, arrayPointsQuadsCenterTmp[i], gravityTetra[i].GetPointInfo().range, gravityTetra[i].GetPointInfo().maxRange);
        }
        //Vector3 closestFound = ExtUtilityFunction.GetClosestPoint(posEntity, arrayPointsQuads, ref indexFound);

        closestRangePoint = ExtUtilityFunction.GetClosestPoint(posEntity, arrayPointsQuads, ref indexFound);
        if (indexFound != -1)
            closestPoint = arrayPointsQuadsCenterTmp[indexFound];

        if (ExtUtilityFunction.IsNullVector(closestPoint))
            return;

        //Debug.Log("closest found: " + closestFound);
        indexFound = indexFound % gravityTetra.Count;

        

        //return (closestFound);
    }

    /// <summary>
    /// find the nearest point from a groupe of points, or lines, or triangles, or quad
    /// </summary>
    public PointInfo FindNearestPoint(Vector3 fromPoint)
    {
        if (allResult.Length == 0)
            return (pointInfo);

        int indexResult = 0;
        Vector3 closestPoint = ExtUtilityFunction.GetNullVector();
        Vector3 closestRangePoint = ExtUtilityFunction.GetNullVector();

        //ExtUtilityFunction.FillArrayWithWrongVector(ref allResultPos);
        //ExtUtilityFunction.FillArrayWithWrongVector(ref allResultPosRange);


        //if there are individual points, add to result
        if (arrayPoints.Length > 0 && gravityPoints.Count > 0 && gravityPoints.Count == arrayPoints.Length)
        {
            //Vector3 closestPoint = GetClosestPointFromGravityPoints(fromPoint);
            GetClosestPointFromGravityPoints(fromPoint, ref closestPoint, ref closestRangePoint);
            //Debug.DrawLine(fromPoint, closestPoint, Color.white);
            allResult[indexResult].pos = closestPoint;
            allResult[indexResult].posRange = closestRangePoint;
            allResult[indexResult].gravityBaseRatio = gravityPoints[indexFound].GetPointInfo().gravityBaseRatio;
            allResult[indexResult].gravityDownRatio = gravityPoints[indexFound].GetPointInfo().gravityDownRatio;
            allResult[indexResult].range = gravityPoints[indexFound].GetPointInfo().range;

            allResultPos[indexResult] = closestPoint;
            allResultPosRange[indexResult] = closestRangePoint;

            indexResult++;
        }

        //if there are lines, add to result
        if (arrayPointsLines.Length > 0 && gravityLines.Count > 0 && gravityLines.Count == arrayPointsLines.Length)
        {
            //Vector3 closestPointLines = GetClosestPointFromLines(fromPoint);
            GetClosestPointFromLines(fromPoint, ref closestPoint, ref closestRangePoint);

            //Debug.DrawLine(fromPoint, closestPointLines, Color.cyan);
            allResult[indexResult].pos = closestPoint;
            allResult[indexResult].posRange = closestRangePoint;
            if (indexFound != -1)
            {
                allResult[indexResult].gravityBaseRatio = gravityLines[indexFound].GetPointInfo().gravityBaseRatio;
                allResult[indexResult].gravityDownRatio = gravityLines[indexFound].GetPointInfo().gravityDownRatio;
                allResult[indexResult].range = gravityLines[indexFound].GetPointInfo().range;
            }
            allResultPos[indexResult] = closestPoint;
            allResultPosRange[indexResult] = closestRangePoint;

            indexResult++;
        }

        //if there are triangles, add to result
        if (arrayPointsTriangles.Length > 0 && gravityTrianglesOrQuad.Count > 0 && gravityTrianglesOrQuad.Count == arrayPointsTriangles.Length)
        {
            GetClosestPointFromTrianglesOrQuad(fromPoint, ref closestPoint, ref closestRangePoint);
            //Debug.DrawLine(fromPoint, closestPointTriangles, Color.green);
            allResult[indexResult].pos = closestPoint;
            allResult[indexResult].posRange = closestRangePoint;
            if (indexFound != -1)
            {
                allResult[indexResult].gravityBaseRatio = gravityTrianglesOrQuad[indexFound].GetPointInfo().gravityBaseRatio;
                allResult[indexResult].gravityDownRatio = gravityTrianglesOrQuad[indexFound].GetPointInfo().gravityDownRatio;
                allResult[indexResult].range = gravityTrianglesOrQuad[indexFound].GetPointInfo().range;
            }
            allResultPos[indexResult] = closestPoint;
            allResultPosRange[indexResult] = closestRangePoint;

            indexResult++;
        }

        //if there are Quads, add to result
        if (arrayPointsQuads.Length > 0 && gravityTetra.Count > 0 && gravityTetra.Count == arrayPointsQuads.Length)
        {
            GetClosestPointFromTetras(fromPoint, ref closestPoint, ref closestRangePoint);

            allResult[indexResult].pos = closestPoint;
            allResult[indexResult].posRange = closestRangePoint;
            if (indexFound != -1)
            {
                allResult[indexResult].gravityBaseRatio = gravityTetra[indexFound].GetPointInfo().gravityBaseRatio;
                allResult[indexResult].gravityDownRatio = gravityTetra[indexFound].GetPointInfo().gravityDownRatio;
                allResult[indexResult].range = gravityTetra[indexFound].GetPointInfo().range;
            }
            allResultPos[indexResult] = closestPoint;
            allResultPosRange[indexResult] = closestRangePoint;

            indexResult++;
        }


        if (indexResult > 0)
        {
            pointInfo.pos = ExtUtilityFunction.GetClosestPoint(fromPoint, allResultPos, ref indexFound);
            pointInfo.posRange = ExtUtilityFunction.GetClosestPoint(fromPoint, allResultPosRange, ref indexFound);

            if (indexFound != -1)
            {
                pointInfo.gravityBaseRatio = allResult[indexFound].gravityBaseRatio;
                pointInfo.gravityDownRatio = allResult[indexFound].gravityDownRatio;
                pointInfo.range = allResult[indexFound].range;
                //Debug.DrawLine(fromPoint, pointInfo.pos, Color.green, 5f);
                //ExtDrawGuizmos.DebugWireSphere(pointInfo.posRange, Color.red, 1f);
                //ExtDrawGuizmos.DebugWireSphere(pointInfo.pos, Color.red, 1f);
            }
        }
        pointInfo.refGA = this;
        return (pointInfo);
    }
}
