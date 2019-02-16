using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class GravityAttractorEditor : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public bool creatorMode = true;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform parentAlones;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform parentLines;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform parentTriangles;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform parentQuad;

    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private GravityAttractor gravityAttractor;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private Transform testPoint;

    [FoldoutGroup("Debug"), SerializeField, ReadOnly]
    private List<Transform> allGravityPoint = new List<Transform>();
    public List<Transform> GetAllGravityPoint() => allGravityPoint;

    private bool isUpdatedFirstTime = false;

    private void SetupEditorPoint()
    {
        if (!gravityAttractor.valueArrayChanged)
            return;
        gravityAttractor.valueArrayChanged = false;

        allGravityPoint.Clear();

        for (int i = 0; i < gravityAttractor.gravityPoints.Count; i++)
        {
            if (!gravityAttractor.gravityPoints[i].point)
            {
                GameObject newPoint = new GameObject("AlonePoint " + i);
                newPoint.transform.position = parentAlones.position;
                newPoint.transform.SetParent(parentAlones);
                GravityAttractor.GravityPoint newGP = gravityAttractor.gravityPoints[i];
                newGP.point = newPoint.transform;
                newGP.SetDefautIfFirstTimeCreated();
                gravityAttractor.gravityPoints[i] = newGP;
            }

            allGravityPoint.Add(gravityAttractor.gravityPoints[i].point);
        }
        for (int i = 0; i < gravityAttractor.gravityLines.Count; i++)
        {
            GravityAttractor.GravityLine newGL = gravityAttractor.gravityLines[i];

            if (!gravityAttractor.gravityLines[i].pointA)
            {
                GameObject newPoint = new GameObject("Line " + i + " - Point A");
                newPoint.transform.position = parentLines.transform.position;
                newPoint.transform.SetParent(parentLines.transform);
                newGL.pointA = newPoint.transform;
            }
            if (!gravityAttractor.gravityLines[i].pointB)
            {
                GameObject newPoint = new GameObject("Line " + i + " - Point B");
                newPoint.transform.position = parentLines.transform.position + new Vector3(5, 0, 0);
                newPoint.transform.SetParent(parentLines.transform);
                newGL.pointB = newPoint.transform;
            }
            newGL.SetDefautIfFirstTimeCreated();
            gravityAttractor.gravityLines[i] = newGL;


            allGravityPoint.Add(gravityAttractor.gravityLines[i].pointA);
            allGravityPoint.Add(gravityAttractor.gravityLines[i].pointB);
        }
        for (int i = 0; i < gravityAttractor.gravityTriangles.Count; i++)
        {
            GravityAttractor.GravityTriangle newGT = gravityAttractor.gravityTriangles[i];

            if (!gravityAttractor.gravityTriangles[i].pointA)
            {
                GameObject newPoint = new GameObject("Triangle " + i + " - Point A");
                newPoint.transform.position = parentTriangles.transform.position;
                newPoint.transform.SetParent(parentTriangles.transform);
                newGT.pointA = newPoint.transform;
            }
            if (!gravityAttractor.gravityTriangles[i].pointB)
            {
                GameObject newPoint = new GameObject("Triangle " + i + " - Point B");
                newPoint.transform.position = parentTriangles.transform.position + new Vector3(5, 0, 0);
                newPoint.transform.SetParent(parentTriangles.transform);
                newGT.pointB = newPoint.transform;
            }
            if (!gravityAttractor.gravityTriangles[i].pointC)
            {
                GameObject newPoint = new GameObject("Triangle " + i + " - Point C");
                newPoint.transform.position = parentTriangles.transform.position + new Vector3(5, 5, 0);
                newPoint.transform.SetParent(parentTriangles.transform);
                newGT.pointC = newPoint.transform;
            }
            newGT.SetDefautIfFirstTimeCreated();
            gravityAttractor.gravityTriangles[i] = newGT;


            allGravityPoint.Add(gravityAttractor.gravityTriangles[i].pointA);
            allGravityPoint.Add(gravityAttractor.gravityTriangles[i].pointB);
            allGravityPoint.Add(gravityAttractor.gravityTriangles[i].pointC);
        }
        for (int i = 0; i < gravityAttractor.gravityQuad.Count; i++)
        {
            GravityAttractor.GravityQuad newGQ = gravityAttractor.gravityQuad[i];

            if (!gravityAttractor.gravityQuad[i].pointA)
            {
                GameObject newPoint = new GameObject("Quad " + i + " - Point A");
                newPoint.transform.position = parentQuad.transform.position;
                newPoint.transform.SetParent(parentQuad.transform);
                newGQ.pointA = newPoint.transform;
            }
            if (!gravityAttractor.gravityQuad[i].pointB)
            {
                GameObject newPoint = new GameObject("Quad " + i + " - Point B");
                newPoint.transform.position = parentQuad.transform.position + new Vector3(5, 0, 0);
                newPoint.transform.SetParent(parentQuad.transform);
                newGQ.pointB = newPoint.transform;
            }
            if (!gravityAttractor.gravityQuad[i].pointC)
            {
                GameObject newPoint = new GameObject("Quad " + i + " - Point C");
                newPoint.transform.position = parentQuad.transform.position + new Vector3(5, 5, 0);
                newPoint.transform.SetParent(parentQuad.transform);
                newGQ.pointC = newPoint.transform;
            }
            if (!gravityAttractor.gravityQuad[i].pointD)
            {
                GameObject newPoint = new GameObject("Quad " + i + " - Point D");
                newPoint.transform.position = parentQuad.transform.position + new Vector3(0, 5, 0);
                newPoint.transform.SetParent(parentQuad.transform);
                newGQ.pointD = newPoint.transform;
            }
            newGQ.SetDefautIfFirstTimeCreated();
            gravityAttractor.gravityQuad[i] = newGQ;


            allGravityPoint.Add(gravityAttractor.gravityQuad[i].pointA);
            allGravityPoint.Add(gravityAttractor.gravityQuad[i].pointB);
            allGravityPoint.Add(gravityAttractor.gravityQuad[i].pointC);
            allGravityPoint.Add(gravityAttractor.gravityQuad[i].pointD);
        }

        CleanHoldEmptyChild();
    }

    private void CleanHoldEmptyChild()
    {
        foreach (Transform child in parentAlones)
        {
            if (!allGravityPoint.Contains(child))
                GameObject.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in parentLines)
        {
            if (!allGravityPoint.Contains(child))
                GameObject.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in parentTriangles)
        {
            if (!allGravityPoint.Contains(child))
                GameObject.DestroyImmediate(child.gameObject);
        }
        foreach (Transform child in parentQuad)
        {
            if (!allGravityPoint.Contains(child))
                GameObject.DestroyImmediate(child.gameObject);
        }
    }

    private void DisplayPoint()
    {
        for (int i = 0; i < gravityAttractor.gravityPoints.Count; i++)
        {
            Gizmos.color = Color.white;
            if (gravityAttractor.gravityPoints[i].point)
                DrawCross(gravityAttractor.gravityPoints[i].point.position);
        }
        for (int i = 0; i < gravityAttractor.gravityLines.Count; i++)
        {
            if (gravityAttractor.gravityLines[i].pointA && gravityAttractor.gravityLines[i].pointB)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(gravityAttractor.gravityLines[i].pointA.position, gravityAttractor.gravityLines[i].pointB.position);
                DrawCross(gravityAttractor.gravityLines[i].pointA.position);
                DrawCross(gravityAttractor.gravityLines[i].pointB.position);
            }
        }
        for (int i = 0; i < gravityAttractor.gravityTriangles.Count; i++)
        {
            if (gravityAttractor.gravityTriangles[i].pointA && gravityAttractor.gravityTriangles[i].pointB && gravityAttractor.gravityTriangles[i].pointC)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(gravityAttractor.gravityTriangles[i].pointA.position, gravityAttractor.gravityTriangles[i].pointB.position);
                Gizmos.DrawLine(gravityAttractor.gravityTriangles[i].pointB.position, gravityAttractor.gravityTriangles[i].pointC.position);
                Gizmos.DrawLine(gravityAttractor.gravityTriangles[i].pointC.position, gravityAttractor.gravityTriangles[i].pointA.position);
                DrawCross(gravityAttractor.gravityTriangles[i].pointA.position);
                DrawCross(gravityAttractor.gravityTriangles[i].pointB.position);
                DrawCross(gravityAttractor.gravityTriangles[i].pointC.position);
            }
        }
        for (int i = 0; i < gravityAttractor.gravityQuad.Count; i++)
        {
            if (gravityAttractor.gravityQuad[i].pointA && gravityAttractor.gravityQuad[i].pointB && gravityAttractor.gravityQuad[i].pointC && gravityAttractor.gravityQuad[i].pointD)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(gravityAttractor.gravityQuad[i].pointA.position, gravityAttractor.gravityQuad[i].pointB.position);
                Gizmos.DrawLine(gravityAttractor.gravityQuad[i].pointB.position, gravityAttractor.gravityQuad[i].pointC.position);
                Gizmos.DrawLine(gravityAttractor.gravityQuad[i].pointC.position, gravityAttractor.gravityQuad[i].pointD.position);
                Gizmos.DrawLine(gravityAttractor.gravityQuad[i].pointD.position, gravityAttractor.gravityQuad[i].pointA.position);
                DrawCross(gravityAttractor.gravityQuad[i].pointA.position);
                DrawCross(gravityAttractor.gravityQuad[i].pointB.position);
                DrawCross(gravityAttractor.gravityQuad[i].pointC.position);
                DrawCross(gravityAttractor.gravityQuad[i].pointD.position);
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

    public void ResetAfterUndo()
    {
        Debug.Log("undo/redo performed !");
        gravityAttractor.SetupArrayPoints();
        gravityAttractor.valueArrayChanged = true;
        SetupEditorPoint();
    }

    private void OnDrawGizmos()
    {
        if (!gravityAttractor || !parentAlones || !parentLines || !parentTriangles || !parentQuad)
            return;

        DisplayPoint();
        if (!Application.isPlaying && creatorMode)
        {
            if (!isUpdatedFirstTime)
            {
                gravityAttractor.SetupArrayPoints();
                isUpdatedFirstTime = true;
            }
            SetupEditorPoint();
            if (testPoint)
            {
                gravityAttractor.FindNearestPoint(testPoint.position);
            }                
        }

    }
}
