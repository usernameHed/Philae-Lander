using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    [Serializable]
    public struct GravityPoint
    {
        public Transform point;
        public float gravityRatio;
    }
    [Serializable]
    public struct GravityLine
    {
        public Transform pointA;
        public Transform pointB;
        public float gravityRatio;
    }
    [Serializable]
    public struct GravityTriangle
    {
        public Transform pointA;
        public Transform pointB;
        public Transform pointC;
        public float gravityRatio;
    }
    [Serializable]
    public struct GravityQuad
    {
        public Transform pointA;
        public Transform pointB;
        public Transform pointC;
        public Transform pointD;
        public float gravityRatio;
    }

    [FoldoutGroup("GamePlay"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private List<GravityPoint> gravityPoint = new List<GravityPoint>();
    [FoldoutGroup("GamePlay"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private List<GravityLine> gravityLines = new List<GravityLine>();
    [FoldoutGroup("GamePlay"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private List<GravityTriangle> gravityTriangles = new List<GravityTriangle>();
    [FoldoutGroup("GamePlay"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private List<GravityQuad> gravityQuad = new List<GravityQuad>();

    [FoldoutGroup("Test"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private Transform testPoint;
    [FoldoutGroup("Test"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private bool isTester = false;

    [FoldoutGroup("GamePlay"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private bool takeDistIntoAccount = true;

    [FoldoutGroup("Debug"), ReadOnly]
    public List<GravityPoint> lastListFound = new List<GravityPoint>();

    private Vector3[] arrayPoints;
    private Vector3[] arrayPointsLines;
    private Vector3[] arrayPointsTriangle;
    private Vector3[] arrayPointsQuad;

    [Button]
    private void SetupArrayPoints()
    {
        arrayPoints = new Vector3[gravityPoint.Count];
        for (int i = 0; i < gravityPoint.Count; i++)
        {
            if (gravityPoint[i].point)
                arrayPoints[i] = gravityPoint[i].point.position;
        }

        arrayPointsLines = new Vector3[gravityLines.Count];
        arrayPointsTriangle = new Vector3[gravityTriangles.Count];
        arrayPointsQuad = new Vector3[gravityQuad.Count];
    }

    public void SelectedGravityAttractor()
    {
        Debug.Log(gameObject.name + " selected !" + gameObject);
        lastListFound.Clear();
    }

    public void UnselectGravityAttractor()
    {
        Debug.Log(gameObject.name + " un-selected !" + gameObject);
        lastListFound.Clear();
    }

    /// <summary>
    /// get the closest gravity point
    /// </summary>
    /// <param name="entity"></param>
    public List<GravityPoint> GetPoint(Vector3 entity)
    {
        lastListFound.Clear();

        float sqrDist = 0;
        int indexGravityPoint = -1;
        for (int i = 0; i < gravityPoint.Count; i++)
        {
            float dist = (entity - gravityPoint[i].point.position).sqrMagnitude;
            if (i == 0)
            {
                indexGravityPoint = 0;
                sqrDist = dist;
            }
            else if (dist < sqrDist)
            {
                sqrDist = dist;
                indexGravityPoint = i;
            }
        }
        if (indexGravityPoint == -1)
            Debug.LogError("nothing found");

        lastListFound.Add(gravityPoint[indexGravityPoint]);

         /*
        for (int i = 0; i < gravityPoint.Count; i++)
        {
            float dist = (entity.position - gravityPoint[i].point.position).sqrMagnitude;
            if (takeDistIntoAccount)
            {
                if (dist < gravityPoint[i].dist * gravityPoint[i].dist)
                    lastListFound.Add(gravityPoint[i].point);
            }
            else
            {

            }
        }
        if (lastListFound.Count == 0)
            Debug.LogError("nothing found");
            */
        return (lastListFound);
    }

    public Vector3 GetClosestPointOnLineSegment(Vector3 A, Vector3 B, Vector3 P)
    {
        Vector3 AP = P - A;       //Vector from A to P   
        Vector3 AB = B - A;       //Vector from A to B  

        float magnitudeAB = AB.sqrMagnitude;     //Magnitude of AB vector (it's length squared)     
        float ABAPproduct = Vector3.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

        if (distance < 0)     //Check if P projection is over vectorAB     
        {
            return A;
        }
        else if (distance > 1)
        {
            return B;
        }
        else
        {
            return A + AB * distance;
        }
    }

    private Vector3 GetClosestPointFromLines(Transform posEntity)
    {
        for (int i = 0; i < gravityLines.Count; i++)
        {
            if (!gravityLines[i].pointA || !gravityLines[i].pointB)
                continue;

            arrayPointsLines[i] = GetClosestPointOnLineSegment(gravityLines[i].pointA.position, gravityLines[i].pointB.position, posEntity.position);
             

            Gizmos.color = Color.black;
            Gizmos.DrawLine(posEntity.position, arrayPointsLines[i]);
        }

        Vector3 closestPoint = ExtUtilityFunction.GetClosestPoint(testPoint.position, arrayPointsLines);

        if (closestPoint != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(testPoint.position, closestPoint);
        }

        return (Vector3.zero);
    }

    private void FindTestPoint()
    {
        if (!isTester || !testPoint)
            return;

        Vector3 closestPoint = ExtUtilityFunction.GetClosestPoint(testPoint.position, arrayPoints);

        if (closestPoint != Vector3.zero)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(testPoint.position, closestPoint);
        }

        Vector3 closestPointLine = GetClosestPointFromLines(testPoint);

        if (closestPointLine != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(testPoint.position, closestPointLine);
        }
        //Gizmos.color = Color.white;
    }

    private void DisplayPoint()
    {
        for (int i = 0; i < gravityPoint.Count; i++)
        {
            if (gravityPoint[i].point)
            {
                Gizmos.color = Color.white;
                if (lastListFound.Contains(gravityPoint[i]))
                    Gizmos.color = Color.red;

                //if (takeDistIntoAccount)
                //    Gizmos.DrawWireSphere(gravityPoint[i].point.position, gravityPoint[i].dist * gravityPoint[i].dist);

                DrawCross(gravityPoint[i].point.position);
            }
        }
        for (int i = 0; i < gravityLines.Count; i++)
        {
            if (gravityLines[i].pointA && gravityLines[i].pointB)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(gravityLines[i].pointA.position, gravityLines[i].pointB.position);
                DrawCross(gravityLines[i].pointA.position);
                DrawCross(gravityLines[i].pointB.position);
            }
        }
        for (int i = 0; i < gravityTriangles.Count; i++)
        {
            if (gravityTriangles[i].pointA && gravityTriangles[i].pointB && gravityTriangles[i].pointC)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(gravityTriangles[i].pointA.position, gravityTriangles[i].pointB.position);
                Gizmos.DrawLine(gravityTriangles[i].pointB.position, gravityTriangles[i].pointC.position);
                Gizmos.DrawLine(gravityTriangles[i].pointC.position, gravityTriangles[i].pointA.position);
                DrawCross(gravityTriangles[i].pointA.position);
                DrawCross(gravityTriangles[i].pointB.position);
                DrawCross(gravityTriangles[i].pointC.position);
            }
        }
        for (int i = 0; i < gravityQuad.Count; i++)
        {
            if (gravityQuad[i].pointA && gravityQuad[i].pointB && gravityQuad[i].pointC && gravityQuad[i].pointD)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(gravityQuad[i].pointA.position, gravityQuad[i].pointB.position);
                Gizmos.DrawLine(gravityQuad[i].pointB.position, gravityQuad[i].pointC.position);
                Gizmos.DrawLine(gravityQuad[i].pointC.position, gravityQuad[i].pointD.position);
                Gizmos.DrawLine(gravityQuad[i].pointD.position, gravityQuad[i].pointA.position);
                DrawCross(gravityQuad[i].pointA.position);
                DrawCross(gravityQuad[i].pointB.position);
                DrawCross(gravityQuad[i].pointC.position);
                DrawCross(gravityQuad[i].pointD.position);
            }
        }
    }

    private void DrawCross(Vector3 position)
    {
        Gizmos.DrawRay(new Ray(position, new Vector3(10, 10, 10)));
        Gizmos.DrawRay(new Ray(position, new Vector3(-10, -10, -10)));

        Gizmos.DrawRay(new Ray(position, new Vector3(-10, 10, 10)));
        Gizmos.DrawRay(new Ray(position, new Vector3(10, -10, -10)));

        Gizmos.DrawRay(new Ray(position, new Vector3(-10, 0, 10)));
        Gizmos.DrawRay(new Ray(position, new Vector3(10, 0, -10)));

        Gizmos.DrawRay(new Ray(position, new Vector3(10, 10, 0)));
        Gizmos.DrawRay(new Ray(position, new Vector3(-10, -10, 0)));

        Gizmos.DrawRay(new Ray(position, new Vector3(0, 10, 10)));
        Gizmos.DrawRay(new Ray(position, new Vector3(0, -10, -10)));

        Gizmos.DrawRay(new Ray(position, new Vector3(0, 0, 10)));
        Gizmos.DrawRay(new Ray(position, new Vector3(0, 0, -10)));

        Gizmos.DrawRay(new Ray(position, new Vector3(0, 10, 0)));
        Gizmos.DrawRay(new Ray(position, new Vector3(0, -10, 0)));
    }

    private void OnDrawGizmos()
    {
        DisplayPoint();
        if (!Application.isPlaying)
        {
            SetupArrayPoints();
            FindTestPoint();
        }
    }

    private void FixedUpdate()
    {
        //FindTestPoint();
    }
}
