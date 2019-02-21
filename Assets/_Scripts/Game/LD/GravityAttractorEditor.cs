using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode, RequireComponent(typeof(GravityAttractor))]
public class GravityAttractorEditor : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public bool creatorMode = true;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public bool alwaysShow = false;
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
    public GravityAttractor GetGravityAttractor() => gravityAttractor;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private Transform testPoint;
    
    [HideInInspector]
    public Vector3[] allModifiedPosGravityPoint;
    [HideInInspector]
    public Transform[] allPosGravityPoint;
    [HideInInspector]
    public Vector3[] savePosAll;

    private bool isUpdatedFirstTime = false;
    private const string parentGravityName = "GRAVITY ATTRACTOR EDITOR (autogenerate)";
    private const string alonesParent = "Alones (autogenerate)";
    private const string linesParent = "Lines (autogenerate)";
    private const string trianglesParent = "Triangles (autogenerate)";
    private const string quadsParent = "Quads (autogenerate)";

    private void SetupEditorPoint()
    {
        if (!gravityAttractor.valueArrayChanged)
            return;
        gravityAttractor.valueArrayChanged = false;

        //allGravityPoint.Clear();

        for (int i = 0; i < gravityAttractor.gravityPoints.Count; i++)
        {
            if (!gravityAttractor.gravityPoints[i].point)
            {
                Transform newPoint = parentAlones.Find("AlonePoint " + i);
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("AlonePoint " + i);
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentAlones.position;
                }
                newPoint.SetParent(parentAlones);
                GravityAttractor.GravityPoint newGP = gravityAttractor.gravityPoints[i];
                newGP.point = newPoint;
                newGP.SetDefautIfFirstTimeCreated();
                gravityAttractor.gravityPoints[i] = newGP;
            }

            //allGravityPoint.Add(gravityAttractor.gravityPoints[i].point);
        }
        for (int i = 0; i < gravityAttractor.gravityLines.Count; i++)
        {
            GravityAttractor.GravityLine newGL = gravityAttractor.gravityLines[i];

            if (!gravityAttractor.gravityLines[i].pointA)
            {
                Transform newPoint = parentLines.Find("Line " + i + " - Point A");
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("Line " + i + " - Point A");
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentLines.position;
                }
                
                newPoint.SetParent(parentLines.transform);
                newGL.pointA = newPoint;
            }
            if (!gravityAttractor.gravityLines[i].pointB)
            {
                Transform newPoint = parentLines.Find("Line " + i + " - Point B");
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("Line " + i + " - Point B");
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentLines.position + new Vector3(5, 0, 0);
                }
                
                newPoint.transform.SetParent(parentLines.transform);
                newGL.pointB = newPoint.transform;
            }
            newGL.SetDefautIfFirstTimeCreated();
            gravityAttractor.gravityLines[i] = newGL;


            //allGravityPoint.Add(gravityAttractor.gravityLines[i].pointA);
            //allGravityPoint.Add(gravityAttractor.gravityLines[i].pointB);
        }
        for (int i = 0; i < gravityAttractor.gravityTriangles.Count; i++)
        {
            GravityAttractor.GravityTriangle newGT = gravityAttractor.gravityTriangles[i];

            if (!gravityAttractor.gravityTriangles[i].pointA)
            {
                Transform newPoint = parentTriangles.Find("Triangle " + i + " - Point A");
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("Triangle " + i + " - Point A");
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentTriangles.position;
                }
                
                newPoint.transform.SetParent(parentTriangles.transform);
                newGT.pointA = newPoint.transform;
            }
            if (!gravityAttractor.gravityTriangles[i].pointB)
            {
                Transform newPoint = parentTriangles.Find("Triangle " + i + " - Point B");
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("Triangle " + i + " - Point B");
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentTriangles.position + new Vector3(5, 0, 0);
                }
                newPoint.transform.SetParent(parentTriangles.transform);
                newGT.pointB = newPoint.transform;
            }
            if (!gravityAttractor.gravityTriangles[i].pointC)
            {
                Transform newPoint = parentTriangles.Find("Triangle " + i + " - Point C");
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("Triangle " + i + " - Point C");
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentTriangles.position + new Vector3(5, 0, 5);
                }

                newPoint.transform.SetParent(parentTriangles.transform);
                newGT.pointC = newPoint.transform;
            }
            newGT.SetDefautIfFirstTimeCreated();
            gravityAttractor.gravityTriangles[i] = newGT;


            //allGravityPoint.Add(gravityAttractor.gravityTriangles[i].pointA);
            //allGravityPoint.Add(gravityAttractor.gravityTriangles[i].pointB);
            //allGravityPoint.Add(gravityAttractor.gravityTriangles[i].pointC);
        }
        for (int i = 0; i < gravityAttractor.gravityQuad.Count; i++)
        {
            GravityAttractor.GravityQuad newGQ = gravityAttractor.gravityQuad[i];

            if (!gravityAttractor.gravityQuad[i].pointA)
            {
                Transform newPoint = parentQuad.Find("Quad " + i + " - Point A");
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("Quad " + i + " - Point A");
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentQuad.position;
                }

                newPoint.transform.SetParent(parentQuad.transform);
                newGQ.pointA = newPoint.transform;
            }
            if (!gravityAttractor.gravityQuad[i].pointB)
            {
                Transform newPoint = parentQuad.Find("Quad " + i + " - Point B");
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("Quad " + i + " - Point B");
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentQuad.position + new Vector3(5, 0, 0);
                }

                newPoint.transform.SetParent(parentQuad.transform);
                newGQ.pointB = newPoint.transform;
            }
            if (!gravityAttractor.gravityQuad[i].pointC)
            {
                Transform newPoint = parentQuad.Find("Quad " + i + " - Point C");
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("Quad " + i + " - Point C");
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentQuad.position + new Vector3(5, 0, 5);
                }

                newPoint.transform.SetParent(parentQuad.transform);
                newGQ.pointC = newPoint.transform;
            }
            if (!gravityAttractor.gravityQuad[i].pointD)
            {
                Transform newPoint = parentQuad.Find("Quad " + i + " - Point D");
                if (!newPoint)
                {
                    GameObject newObjPoint = new GameObject("Quad " + i + " - Point D");
                    newPoint = newObjPoint.transform;
                    newPoint.position = parentQuad.position + new Vector3(0, 0, 5);
                }

                newPoint.transform.SetParent(parentQuad.transform);
                newGQ.pointD = newPoint.transform;
            }
            newGQ.SetDefautIfFirstTimeCreated();
            gravityAttractor.gravityQuad[i] = newGQ;
            
        }

        SetAllPointToArray();

        CleanHoldEmptyChild();
    }


    private void CleanHoldEmptyChild()
    {
        Debug.Log("do a clean in ALL gravityAttractor in the same time ??");
        /*
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
        */
    }


    public int GetLenghtOfAllPoints()
    {
        return (gravityAttractor.gravityPoints.Count
            + gravityAttractor.gravityLines.Count * 2
            + gravityAttractor.gravityTriangles.Count * 3
            + gravityAttractor.gravityQuad.Count * 4);
    }

    private void OnEnable()
    {
        GenerateParenting();
    }

    [Button("GenerateParenting")]
    public void GenerateParenting()
    {
        if (!gravityAttractor)
            gravityAttractor = GetComponent<GravityAttractor>();

        if (!gravityAttractor)
        {
            Debug.Log("no gravity attractor ?");
            return;
        }

        if (!gravityAttractor.gravityAttractorEditor)
        {
            Transform newParent = transform.Find(parentGravityName);
            if (!newParent)
            {
                GameObject newObjParent = new GameObject(parentGravityName);
                newParent = newObjParent.transform;
            }
                
            newParent.SetParent(gravityAttractor.transform);
            newParent.localPosition = Vector3.zero;
            newParent.SetAsFirstSibling();
            gravityAttractor.gravityAttractorEditor = newParent;
        }

        if (!parentAlones)
        {
            Transform newParent = gravityAttractor.gravityAttractorEditor.transform.Find(alonesParent);
            if (!newParent)
            {
                GameObject newObjParent = new GameObject(alonesParent);
                newParent = newObjParent.transform;
            }
            newParent.SetParent(gravityAttractor.gravityAttractorEditor);
            newParent.localPosition = Vector3.zero;
            parentAlones = newParent;
        }
        if (!parentLines)
        {
            Transform newParent = gravityAttractor.gravityAttractorEditor.transform.Find(linesParent);
            if (!newParent)
            {
                GameObject newObjParent = new GameObject(linesParent);
                newParent = newObjParent.transform;
            }
            newParent.SetParent(gravityAttractor.gravityAttractorEditor);
            newParent.localPosition = Vector3.zero;
            parentLines = newParent;
        }
        if (!parentTriangles)
        {
            Transform newParent = gravityAttractor.gravityAttractorEditor.transform.Find(trianglesParent);
            if (!newParent)
            {
                GameObject newObjParent = new GameObject(trianglesParent);
                newParent = newObjParent.transform;
            }
            newParent.SetParent(gravityAttractor.gravityAttractorEditor);
            newParent.localPosition = Vector3.zero;
            parentTriangles = newParent;
        }
        if (!parentQuad)
        {
            Transform newParent = gravityAttractor.gravityAttractorEditor.transform.Find(quadsParent);
            if (!newParent)
            {
                GameObject newObjParent = new GameObject(quadsParent);
                newParent = newObjParent.transform;
            }
            newParent.SetParent(gravityAttractor.gravityAttractorEditor);
            newParent.localPosition = Vector3.zero;
            parentQuad = newParent;
        }
    }

    /// <summary>
    /// return all point as one bug array
    /// </summary>
    public void SetAllPointToArray()
    {
        allModifiedPosGravityPoint = new Vector3[GetLenghtOfAllPoints()];
        allPosGravityPoint = new Transform[GetLenghtOfAllPoints()];
        savePosAll = new Vector3[GetLenghtOfAllPoints()];

        int indexAll = 0;
        for (int i = 0; i < gravityAttractor.gravityPoints.Count; i++)
        {
            allPosGravityPoint[indexAll] = gravityAttractor.gravityPoints[i].point;
            indexAll++;
        }
        for (int i = 0; i < gravityAttractor.gravityLines.Count; i++)
        {
            allPosGravityPoint[indexAll] = gravityAttractor.gravityLines[i].pointA;
            indexAll++;
            allPosGravityPoint[indexAll] = gravityAttractor.gravityLines[i].pointB;
            indexAll++;
        }
        for (int i = 0; i < gravityAttractor.gravityTriangles.Count; i++)
        {
            allPosGravityPoint[indexAll] = gravityAttractor.gravityTriangles[i].pointA;
            indexAll++;
            allPosGravityPoint[indexAll] = gravityAttractor.gravityTriangles[i].pointB;
            indexAll++;
            allPosGravityPoint[indexAll] = gravityAttractor.gravityTriangles[i].pointC;
            indexAll++;
        }
        for (int i = 0; i < gravityAttractor.gravityQuad.Count; i++)
        {
            allPosGravityPoint[indexAll] = gravityAttractor.gravityQuad[i].pointA;
            indexAll++;
            allPosGravityPoint[indexAll] = gravityAttractor.gravityQuad[i].pointB;
            indexAll++;
            allPosGravityPoint[indexAll] = gravityAttractor.gravityQuad[i].pointC;
            indexAll++;
            allPosGravityPoint[indexAll] = gravityAttractor.gravityQuad[i].pointD;
            indexAll++;
        }
    }

    public bool ContainThisTransform(Transform point, ref GravityAttractor.GravityPointType gravityPointType, ref int indexShape, ref int indexPoint)
    {
        for (int i = 0; i < gravityAttractor.gravityPoints.Count; i++)
        {
            if (gravityAttractor.gravityPoints[i].point.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.POINT;
                indexShape = i;
                indexPoint = 0;
                return (true);
            }                
        }
        for (int i = 0; i < gravityAttractor.gravityLines.Count; i++)
        {
            if (gravityAttractor.gravityLines[i].pointA.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.LINE;
                indexShape = i;
                indexPoint = 0;
                return (true);
            }
            if (gravityAttractor.gravityLines[i].pointB.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.LINE;
                indexShape = i;
                indexPoint = 1;
                return (true);
            }
        }
        for (int i = 0; i < gravityAttractor.gravityTriangles.Count; i++)
        {
            if (gravityAttractor.gravityTriangles[i].pointA.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.TRIANGLE;
                indexShape = i;
                indexPoint = 0;
                return (true);
            }
            if (gravityAttractor.gravityTriangles[i].pointB.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.TRIANGLE;
                indexShape = i;
                indexPoint = 1;
                return (true);
            }
            if (gravityAttractor.gravityTriangles[i].pointC.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.TRIANGLE;
                indexShape = i;
                indexPoint = 2;
                return (true);
            }
        }
        for (int i = 0; i < gravityAttractor.gravityQuad.Count; i++)
        {
            if (gravityAttractor.gravityQuad[i].pointA.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.QUAD;
                indexShape = i;
                indexPoint = 0;
                return (true);
            }
            if (gravityAttractor.gravityQuad[i].pointB.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.QUAD;
                indexShape = i;
                indexPoint = 1;
                return (true);
            }
            if (gravityAttractor.gravityQuad[i].pointC.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.QUAD;
                indexShape = i;
                indexPoint = 2;
                return (true);
            }
            if (gravityAttractor.gravityQuad[i].pointD.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractor.GravityPointType.QUAD;
                indexShape = i;
                indexPoint = 3;
                return (true);
            }
        }
        return (false);
    }

    private void DisplayPoint()
    {
        for (int i = 0; i < gravityAttractor.gravityPoints.Count; i++)
        {
            Gizmos.color = Color.white;
            if (gravityAttractor.gravityPoints[i].point)
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityPoints[i].point.position);
        }
        for (int i = 0; i < gravityAttractor.gravityLines.Count; i++)
        {
            if (gravityAttractor.gravityLines[i].pointA && gravityAttractor.gravityLines[i].pointB)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(gravityAttractor.gravityLines[i].pointA.position, gravityAttractor.gravityLines[i].pointB.position);
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityLines[i].pointA.position);
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityLines[i].pointB.position);
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
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityTriangles[i].pointA.position);
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityTriangles[i].pointB.position);
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityTriangles[i].pointC.position);
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
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityQuad[i].pointA.position);
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityQuad[i].pointB.position);
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityQuad[i].pointC.position);
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityQuad[i].pointD.position);
            }
        }
    }

    public void CleanTheSick()
    {
        for (int i = gravityAttractor.gravityPoints.Count - 1; i >= 0; i--)
        {
            /*if (gravityAttractor.gravityPoints[i].point.parent.name != alonesParent)
            {
                Debug.Log("remove point: " + i);
                gravityAttractor.gravityPoints.RemoveAt(i);
            }
            else */for (int k = i - 1; k >= 0; k--)
            {
                Debug.Log("i: " + i + ", k: " + k);
                if (gravityAttractor.gravityPoints[k].point.GetInstanceID() == gravityAttractor.gravityPoints[i].point.GetInstanceID())
                {
                    gravityAttractor.gravityPoints.RemoveAt(i);
                    i--;
                }
            }
        }
        

        for (int i = gravityAttractor.gravityLines.Count - 1; i >= 0; i--)
        {
            if (gravityAttractor.gravityLines[i].pointA.GetInstanceID() == gravityAttractor.gravityLines[i].pointB.GetInstanceID())
            {
                gravityAttractor.gravityLines.RemoveAt(i);
            }
        }
        for (int i = gravityAttractor.gravityTriangles.Count - 1; i >= 0; i--)
        {
            //here just change triangle into a point ! there are all the same
            if (gravityAttractor.gravityTriangles[i].pointA.GetInstanceID() == gravityAttractor.gravityTriangles[i].pointB.GetInstanceID()
                && gravityAttractor.gravityTriangles[i].pointA.GetInstanceID() == gravityAttractor.gravityTriangles[i].pointC.GetInstanceID())
            {
                gravityAttractor.gravityTriangles.RemoveAt(i);
            }
            else if (gravityAttractor.gravityTriangles[i].pointA.GetInstanceID() == gravityAttractor.gravityTriangles[i].pointB.GetInstanceID())
                gravityAttractor.gravityTriangles.RemoveAt(i);
            else if (gravityAttractor.gravityTriangles[i].pointA.GetInstanceID() == gravityAttractor.gravityTriangles[i].pointC.GetInstanceID())
                gravityAttractor.gravityTriangles.RemoveAt(i);
            else if (gravityAttractor.gravityTriangles[i].pointB.GetInstanceID() == gravityAttractor.gravityTriangles[i].pointC.GetInstanceID())
                gravityAttractor.gravityTriangles.RemoveAt(i);
        }
        for (int i = gravityAttractor.gravityQuad.Count - 1; i >= 0; i--)
        {
            if (gravityAttractor.gravityQuad[i].pointA.GetInstanceID() == gravityAttractor.gravityQuad[i].pointB.GetInstanceID()
                || gravityAttractor.gravityQuad[i].pointA.GetInstanceID() == gravityAttractor.gravityQuad[i].pointC.GetInstanceID()
                || gravityAttractor.gravityQuad[i].pointA.GetInstanceID() == gravityAttractor.gravityQuad[i].pointD.GetInstanceID())
            {
                gravityAttractor.gravityQuad.RemoveAt(i);
            }
            else if (gravityAttractor.gravityQuad[i].pointB.GetInstanceID() == gravityAttractor.gravityQuad[i].pointC.GetInstanceID()
                || gravityAttractor.gravityQuad[i].pointB.GetInstanceID() == gravityAttractor.gravityQuad[i].pointD.GetInstanceID())
            {
                gravityAttractor.gravityQuad.RemoveAt(i);
            }
            else if (gravityAttractor.gravityQuad[i].pointC.GetInstanceID() == gravityAttractor.gravityQuad[i].pointD.GetInstanceID())
            {
                gravityAttractor.gravityQuad.RemoveAt(i);
            }
        }
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
