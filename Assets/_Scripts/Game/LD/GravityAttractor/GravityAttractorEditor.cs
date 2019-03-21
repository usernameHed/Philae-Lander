using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode, RequireComponent(typeof(GravityAttractorLD))]
public class GravityAttractorEditor : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public int createMode = 0;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public bool mergeMode = false;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public bool alwaysShow = true;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform parentAlones = null;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform parentLines = null;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform parentTriangles = null;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform parentQuad = null;

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public Transform previewPoint;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public Transform parentTmpPoints;

    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private GravityAttractorLD gravityAttractor = null;
    public GravityAttractorLD GetGravityAttractor() => gravityAttractor;

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    public List<GameObject> tmpPointCreated = new List<GameObject>();

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    public List<GameObject> tmpForm = new List<GameObject>();

    [FoldoutGroup("Debug"), SerializeField, ReadOnly]
    public GameObject objectPreview;
    [FoldoutGroup("Debug"), SerializeField, ReadOnly]
    public GameObject triggerRef;

    [HideInInspector]
    public Vector3[] allModifiedPosGravityPoint;
    [HideInInspector]
    public Transform[] allPosGravityPoint;
    [HideInInspector]
    public Vector3[] savePosAll;

    [HideInInspector]
    public bool isUpdatedFirstTime = false;
    private const string parentGravityName = "GRAVITY ATTRACTOR EDITOR (autogenerate)";
    private const string alonesParent = "Alones (autogenerate)";
    private const string linesParent = "Lines (autogenerate)";
    private const string trianglesParent = "Triangles (autogenerate)";
    private const string quadsParent = "Quads (autogenerate)";
    private const string tmpPoints = "TmpPoints";
    private const string previewPointName = "+";
    private const string triggerAttractorName = "GRAVITY ATTRACTOR TRIGGER";
    private const string triggerAttractorChildName = "Trigger";
    private const string triggerAttractorLayer = "Walkable/Ground";
    [HideInInspector]
    public string layerPoint = "Point";

    private void OnEnable()
    {
        if (!gravityAttractor)
            gravityAttractor = GetComponent<GravityAttractorLD>();

        
    }

    /*
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
                GravityAttractorLD.GravityPoint newGP = gravityAttractor.gravityPoints[i];
                newGP.SetDefautIfFirstTimeCreated();
                gravityAttractor.gravityPoints[i] = newGP;
            }
        }
        for (int i = 0; i < gravityAttractor.gravityLines.Count; i++)
        {
            GravityAttractorLD.GravityLine newGL = gravityAttractor.gravityLines[i];
            newGL.SetDefautIfFirstTimeCreated();
            gravityAttractor.gravityLines[i] = newGL;
        }
        for (int i = 0; i < gravityAttractor.gravityTriangles.Count; i++)
        {
            GravityAttractorLD.GravityTriangle newGT = gravityAttractor.gravityTriangles[i];
            newGT.SetDefautIfFirstTimeCreated();
            gravityAttractor.gravityTriangles[i] = newGT;
        }
        for (int i = 0; i < gravityAttractor.gravityQuad.Count; i++)
        {
            GravityAttractorLD.GravityQuad newGQ = gravityAttractor.gravityQuad[i];
            
            newGQ.SetDefautIfFirstTimeCreated();
            gravityAttractor.gravityQuad[i] = newGQ;
        }

        SetAllPointToArray();
    }
    */

    /// <summary>
    /// for initialisation
    /// </summary>
    /// <returns></returns>
    public int GetLenghtOfAllPoints()
    {
        return (gravityAttractor.gravityPoints.Count
            + gravityAttractor.gravityLines.Count * 2
            + gravityAttractor.gravityTriangles.Count * 3
            + gravityAttractor.gravityQuad.Count * 4);
    }

    /// <summary>
    /// clean empty point
    /// </summary>
    public void CleanUpTmpPointForm()
    {
        for (int i = tmpForm.Count - 1; i >= 0; i--)
        {
            if (tmpForm[i] == null)
                tmpForm.RemoveAt(i);
        }
    }
    public void CleanUpPointCreated()
    {
        for (int i = tmpPointCreated.Count - 1; i >= 0; i--)
        {
            if (tmpPointCreated[i] == null)
                tmpPointCreated.RemoveAt(i);
        }
    }

    /// <summary>
    /// creat an unique point attractor
    /// </summary>
    private void CreatePoint(GameObject point)
    {
        GravityAttractorLD.GravityPoint newPoint = new GravityAttractorLD.GravityPoint();
        newPoint.SetDefautIfFirstTimeCreated();
        point.transform.SetParent(parentAlones);
        point.name = "P" + gravityAttractor.gravityPoints.Count.ToString();

        newPoint.ChangePoint(0, point.transform);


        gravityAttractor.gravityPoints.Add(newPoint);
    }
    /// <summary>
    /// creat an unique point attractor
    /// </summary>
    private void CreateLine(GameObject pointA, GameObject pointB)
    {
        GravityAttractorLD.GravityLine newLine = new GravityAttractorLD.GravityLine();
        newLine.SetDefautIfFirstTimeCreated();

        Transform newLineTransform = null;
        CreateTransform(ref newLineTransform, gravityAttractor.gravityLines.Count.ToString(), parentLines);
        pointA.transform.SetParent(newLineTransform);
        pointB.transform.SetParent(newLineTransform);

        pointA.name = "L" + gravityAttractor.gravityLines.Count.ToString() + " - 1";
        pointB.name = "L" + gravityAttractor.gravityLines.Count.ToString() + " - 2";

        newLine.ChangePoint(0, pointA.transform);
        newLine.ChangePoint(1, pointB.transform);
        gravityAttractor.gravityLines.Add(newLine);
    }

    /// <summary>
    /// creat an unique point attractor
    /// </summary>
    private void CreateTriangle(GameObject pointA, GameObject pointB, GameObject pointC)
    {
        GravityAttractorLD.GravityTriangle newTriangle = new GravityAttractorLD.GravityTriangle();
        newTriangle.SetDefautIfFirstTimeCreated();

        Transform newTriangleTransform = null;
        CreateTransform(ref newTriangleTransform, gravityAttractor.gravityTriangles.Count.ToString(), parentTriangles);
        pointA.transform.SetParent(newTriangleTransform);
        pointB.transform.SetParent(newTriangleTransform);
        pointC.transform.SetParent(newTriangleTransform);

        pointA.name = "T" + gravityAttractor.gravityTriangles.Count.ToString() + " - 1";
        pointB.name = "T" + gravityAttractor.gravityTriangles.Count.ToString() + " - 2";
        pointC.name = "T" + gravityAttractor.gravityTriangles.Count.ToString() + " - 3";

        newTriangle.ChangePoint(0, pointA.transform);
        newTriangle.ChangePoint(1, pointB.transform);
        newTriangle.ChangePoint(2, pointC.transform);
        gravityAttractor.gravityTriangles.Add(newTriangle);
    }

    /// <summary>
    /// creat an unique point attractor
    /// </summary>
    private void CreateQuad(GameObject pointA, GameObject pointB, GameObject pointC, GameObject pointD)
    {
        GravityAttractorLD.GravityQuad newQuad = new GravityAttractorLD.GravityQuad();
        newQuad.SetDefautIfFirstTimeCreated();

        Transform newQuadTransform = null;
        CreateTransform(ref newQuadTransform, gravityAttractor.gravityQuad.Count.ToString(), parentQuad);
        pointA.transform.SetParent(newQuadTransform);
        pointB.transform.SetParent(newQuadTransform);
        pointC.transform.SetParent(newQuadTransform);
        pointD.transform.SetParent(newQuadTransform);

        pointA.name = "Q" + gravityAttractor.gravityQuad.Count.ToString() + " - 1";
        pointB.name = "Q" + gravityAttractor.gravityQuad.Count.ToString() + " - 2";
        pointC.name = "Q" + gravityAttractor.gravityQuad.Count.ToString() + " - 3";
        pointC.name = "Q" + gravityAttractor.gravityQuad.Count.ToString() + " - 4";

        newQuad.ChangePoint(0, pointA.transform);
        newQuad.ChangePoint(1, pointB.transform);
        newQuad.ChangePoint(2, pointC.transform);
        newQuad.ChangePoint(3, pointD.transform);
        gravityAttractor.gravityQuad.Add(newQuad);
    }


    /// <summary>
    /// ok, we create a form Based on edit point !
    /// </summary>
    public void SetupAForm()
    {
        CleanUpTmpPointForm();
        //tmpForm
        if (tmpForm.Count == 0 || tmpForm.Count > 4)
            return;

        if (tmpForm.Count == 1)
        {
            CreatePoint(tmpForm[0]);
        }
        else if (tmpForm.Count == 2)
        {
            CreateLine(tmpForm[0], tmpForm[1]);
        }
        else if (tmpForm.Count == 3)
        {
            CreateTriangle(tmpForm[0], tmpForm[1], tmpForm[2]);
        }
        else if (tmpForm.Count == 4)
        {
            CreateQuad(tmpForm[0], tmpForm[1], tmpForm[2], tmpForm[3]);
        }

        

        gravityAttractor.SetupArrayPoints();
        
    }

    /// <summary>
    /// create trigger object
    /// </summary>
    [Button]
    public void CreateTrigger()
    {
        if (!gravityAttractor)
            gravityAttractor = GetComponent<GravityAttractorLD>();

        Transform trigger = transform.Find(triggerAttractorName);
        if (!trigger)
        {
            GameObject triggerAttractor = new GameObject(triggerAttractorName);
            GameObject childTrigger = new GameObject(triggerAttractorChildName);

            triggerAttractor.layer = LayerMask.NameToLayer(triggerAttractorLayer);
            childTrigger.layer = LayerMask.NameToLayer(triggerAttractorLayer);

            triggerAttractor.transform.SetParent(transform);
            triggerAttractor.transform.SetAsFirstSibling();
            triggerAttractor.transform.localPosition = Vector3.zero;
            triggerAttractor.transform.localRotation = Quaternion.identity;

            childTrigger.transform.SetParent(triggerAttractor.transform);
            childTrigger.transform.ResetLocalTransform();

            GravityAttractorTrigger gaTrigger = triggerAttractor.AddComponent<GravityAttractorTrigger>();
            gaTrigger.refGravityAttractor = gravityAttractor;

            Rigidbody rbTrigger = triggerAttractor.AddComponent<Rigidbody>();
            rbTrigger.isKinematic = true;

            SphereCollider sphere = childTrigger.AddComponent<SphereCollider>();
            sphere.radius = 5f;
            sphere.isTrigger = true;
            BoxCollider box = childTrigger.AddComponent<BoxCollider>();
            box.size = new Vector3(5f, 5f, 5f);
            box.isTrigger = true;
            box.enabled = false;

            triggerRef = childTrigger;
            Debug.Log("ici trigger ref: " + triggerRef);
        }
    }
    [Button]
    public void RemoveTrigger()
    {
        Transform trigger = transform.Find(triggerAttractorName);
        if (trigger)
            DestroyImmediate(trigger.gameObject);
    }

    private void CreateTransform(ref Transform linkedTransfrom, string nameNewTransform, Transform parentToSet)
    {
        if (!linkedTransfrom)
        {
            Transform newParent = parentToSet.transform.Find(nameNewTransform);
            if (!newParent)
            {
                GameObject newObjParent = new GameObject(nameNewTransform);
                newParent = newObjParent.transform;
            }
            newParent.SetParent(parentToSet);
            newParent.localPosition = Vector3.zero;
            linkedTransfrom = newParent;
        }
    }

    /// <summary>
    /// generate parenting of object
    /// </summary>
    [Button("GenerateParenting")]
    public void GenerateParenting()
    {
        if (!gravityAttractor)
            gravityAttractor = GetComponent<GravityAttractorLD>();

        if (!gravityAttractor)
        {
            Debug.Log("no gravity attractor ?");
            return;
        }
        isUpdatedFirstTime = true;

        CreateTransform(ref gravityAttractor.gravityAttractorEditor, parentGravityName, gravityAttractor.transform);
        gravityAttractor.gravityAttractorEditor.SetAsFirstSibling();

        CreateTransform(ref parentAlones, alonesParent, gravityAttractor.gravityAttractorEditor);
        CreateTransform(ref parentLines, linesParent, gravityAttractor.gravityAttractorEditor);
        CreateTransform(ref parentTriangles, trianglesParent, gravityAttractor.gravityAttractorEditor);
        CreateTransform(ref parentQuad, quadsParent, gravityAttractor.gravityAttractorEditor);
        
        GeneratePreview();
    }

    /// <summary>
    /// generate preview poitns for editor creation
    /// </summary>
    public void GeneratePreview()
    {
        CreateTransform(ref previewPoint, previewPointName, gravityAttractor.gravityAttractorEditor);
        previewPoint.gameObject.layer = LayerMask.NameToLayer("Lock");
        CreateTransform(ref parentTmpPoints, tmpPoints, gravityAttractor.gravityAttractorEditor);
    }

    public void RemoveParenting()
    {
        if (!gravityAttractor)
            gravityAttractor = GetComponent<GravityAttractorLD>();
        if (!gravityAttractor)
        {
            Debug.Log("no gravity attractor ?");
            return;
        }

        if (gravityAttractor.gravityAttractorEditor)
        {
            DestroyImmediate(gravityAttractor.gravityAttractorEditor.gameObject);
        }
    }

    /*
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
    

    public bool ContainThisTransform(Transform point, ref GravityAttractorLD.GravityPointType gravityPointType, ref int indexShape, ref int indexPoint)
    {
        for (int i = 0; i < gravityAttractor.gravityPoints.Count; i++)
        {
            if (gravityAttractor.gravityPoints[i].point.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.POINT;
                indexShape = i;
                indexPoint = 0;
                return (true);
            }                
        }
        for (int i = 0; i < gravityAttractor.gravityLines.Count; i++)
        {
            if (gravityAttractor.gravityLines[i].pointA.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.LINE;
                indexShape = i;
                indexPoint = 0;
                return (true);
            }
            if (gravityAttractor.gravityLines[i].pointB.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.LINE;
                indexShape = i;
                indexPoint = 1;
                return (true);
            }
        }
        for (int i = 0; i < gravityAttractor.gravityTriangles.Count; i++)
        {
            if (gravityAttractor.gravityTriangles[i].pointA.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.TRIANGLE;
                indexShape = i;
                indexPoint = 0;
                return (true);
            }
            if (gravityAttractor.gravityTriangles[i].pointB.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.TRIANGLE;
                indexShape = i;
                indexPoint = 1;
                return (true);
            }
            if (gravityAttractor.gravityTriangles[i].pointC.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.TRIANGLE;
                indexShape = i;
                indexPoint = 2;
                return (true);
            }
        }
        for (int i = 0; i < gravityAttractor.gravityQuad.Count; i++)
        {
            if (gravityAttractor.gravityQuad[i].pointA.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.QUAD;
                indexShape = i;
                indexPoint = 0;
                return (true);
            }
            if (gravityAttractor.gravityQuad[i].pointB.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.QUAD;
                indexShape = i;
                indexPoint = 1;
                return (true);
            }
            if (gravityAttractor.gravityQuad[i].pointC.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.QUAD;
                indexShape = i;
                indexPoint = 2;
                return (true);
            }
            if (gravityAttractor.gravityQuad[i].pointD.GetInstanceID() == point.GetInstanceID())
            {
                gravityPointType = GravityAttractorLD.GravityPointType.QUAD;
                indexShape = i;
                indexPoint = 3;
                return (true);
            }
        }
        return (false);
    }
    */

    /// <summary>
    /// display allpoint of arrays, with there option (plane, infinite, border...)
    /// </summary>
    private void DisplayPoint()
    {
        for (int i = 0; i < gravityAttractor.gravityPoints.Count; i++)
        {
            
            if (gravityAttractor.gravityPoints[i].point)
            {
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityPoints[i].point.position);
                if (gravityAttractor.gravityPoints[i].GetPointInfo().range > 0 && alwaysShow)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(gravityAttractor.gravityPoints[i].point.position, gravityAttractor.gravityPoints[i].GetPointInfo().range);
                }
                if (gravityAttractor.gravityPoints[i].GetPointInfo().maxRange > gravityAttractor.gravityPoints[i].GetPointInfo().range && alwaysShow)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(gravityAttractor.gravityPoints[i].point.position, gravityAttractor.gravityPoints[i].GetPointInfo().maxRange);
                }
            }
                
        }
        for (int i = 0; i < gravityAttractor.gravityLines.Count; i++)
        {
            if (gravityAttractor.gravityLines[i].pointA && gravityAttractor.gravityLines[i].pointB)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(gravityAttractor.gravityLines[i].pointA.position, gravityAttractor.gravityLines[i].pointB.position);
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityLines[i].pointA.position);
                ExtDrawGuizmos.DrawCross(gravityAttractor.gravityLines[i].pointB.position);

                Gizmos.color = Color.white;
                if (gravityAttractor.gravityLines[i].GetPointInfo().range > 0 && alwaysShow)
                {
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityLines[i].pointA.position, gravityAttractor.gravityLines[i].pointB.position, Color.white, gravityAttractor.gravityLines[i].GetPointInfo().range);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityLines[i].pointA.position, gravityAttractor.gravityLines[i].GetPointInfo().range);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityLines[i].pointB.position, gravityAttractor.gravityLines[i].GetPointInfo().range);
                }
                if (gravityAttractor.gravityLines[i].GetPointInfo().maxRange > gravityAttractor.gravityLines[i].GetPointInfo().range && alwaysShow)
                {
                    Gizmos.color = Color.red;
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityLines[i].pointA.position, gravityAttractor.gravityLines[i].pointB.position, Color.red, gravityAttractor.gravityLines[i].GetPointInfo().maxRange);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityLines[i].pointA.position, gravityAttractor.gravityLines[i].GetPointInfo().maxRange);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityLines[i].pointB.position, gravityAttractor.gravityLines[i].GetPointInfo().maxRange);
                }

                if (gravityAttractor.gravityLines[i].noGravityBorders)
                {
                    ExtDrawGuizmos.DrawArrow(gravityAttractor.gravityLines[i].pointA.position + (gravityAttractor.gravityLines[i].pointA.position - gravityAttractor.gravityLines[i].pointB.position).normalized * 2, (gravityAttractor.gravityLines[i].pointB.position - gravityAttractor.gravityLines[i].pointA.position).normalized * 2);
                    ExtDrawGuizmos.DrawArrow(gravityAttractor.gravityLines[i].pointB.position + (gravityAttractor.gravityLines[i].pointB.position - gravityAttractor.gravityLines[i].pointA.position).normalized * 2, (gravityAttractor.gravityLines[i].pointA.position - gravityAttractor.gravityLines[i].pointB.position).normalized * 2);
                }
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

                Gizmos.color = Color.white;
                if (gravityAttractor.gravityTriangles[i].GetPointInfo().range > 0 && alwaysShow)
                {
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityTriangles[i].pointA.position, gravityAttractor.gravityTriangles[i].pointB.position, Color.white, gravityAttractor.gravityTriangles[i].GetPointInfo().range);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityTriangles[i].pointB.position, gravityAttractor.gravityTriangles[i].pointC.position, Color.white, gravityAttractor.gravityTriangles[i].GetPointInfo().range);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityTriangles[i].pointC.position, gravityAttractor.gravityTriangles[i].pointA.position, Color.white, gravityAttractor.gravityTriangles[i].GetPointInfo().range);

                    Gizmos.DrawWireSphere(gravityAttractor.gravityTriangles[i].pointA.position, gravityAttractor.gravityTriangles[i].GetPointInfo().range);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityTriangles[i].pointB.position, gravityAttractor.gravityTriangles[i].GetPointInfo().range);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityTriangles[i].pointC.position, gravityAttractor.gravityTriangles[i].GetPointInfo().range);
                }
                if (gravityAttractor.gravityTriangles[i].GetPointInfo().maxRange > gravityAttractor.gravityTriangles[i].GetPointInfo().range && alwaysShow)
                {
                    Gizmos.color = Color.red;
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityTriangles[i].pointA.position, gravityAttractor.gravityTriangles[i].pointB.position, Color.red, gravityAttractor.gravityTriangles[i].GetPointInfo().maxRange);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityTriangles[i].pointB.position, gravityAttractor.gravityTriangles[i].pointC.position, Color.red, gravityAttractor.gravityTriangles[i].GetPointInfo().maxRange);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityTriangles[i].pointC.position, gravityAttractor.gravityTriangles[i].pointA.position, Color.red, gravityAttractor.gravityTriangles[i].GetPointInfo().maxRange);

                    Gizmos.DrawWireSphere(gravityAttractor.gravityTriangles[i].pointA.position, gravityAttractor.gravityTriangles[i].GetPointInfo().maxRange);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityTriangles[i].pointB.position, gravityAttractor.gravityTriangles[i].GetPointInfo().maxRange);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityTriangles[i].pointC.position, gravityAttractor.gravityTriangles[i].GetPointInfo().maxRange);
                }


                Vector3 middlePlane = ExtQuaternion.GetMiddleOfXPoint(new Vector3[] { gravityAttractor.gravityTriangles[i].pointA.position,
                    gravityAttractor.gravityTriangles[i].pointB.position,
                    gravityAttractor.gravityTriangles[i].pointC.position}, false);

                if (gravityAttractor.gravityTriangles[i].unidirectionnal)
                {
                    ExtTriangle triangleA = new ExtTriangle(gravityAttractor.gravityTriangles[i].pointA.position, gravityAttractor.gravityTriangles[i].pointB.position, gravityAttractor.gravityTriangles[i].pointC.position,
                            gravityAttractor.gravityTriangles[i].unidirectionnal,
                            gravityAttractor.gravityTriangles[i].inverseDirection,
                            gravityAttractor.gravityTriangles[i].infinitePlane,
                            gravityAttractor.gravityTriangles[i].noGravityBorders);
                    if (!gravityAttractor.gravityTriangles[i].inverseDirection)
                        ExtDrawGuizmos.DrawArrow(middlePlane - triangleA.TriNorm.normalized, triangleA.TriNorm.normalized);
                    else
                        ExtDrawGuizmos.DrawArrow(middlePlane + triangleA.TriNorm.normalized, -triangleA.TriNorm.normalized);
                }

                if (gravityAttractor.gravityTriangles[i].infinitePlane || gravityAttractor.gravityTriangles[i].noGravityBorders)
                {
                    int dirArrow = (gravityAttractor.gravityTriangles[i].infinitePlane) ? -1 : 1;

                    Vector3 middleLineA = ExtQuaternion.GetMiddleOfXPoint(new Vector3[] { gravityAttractor.gravityTriangles[i].pointA.position, gravityAttractor.gravityTriangles[i].pointB.position });
                    Vector3 dirMiddleToMiddleLineA = (middlePlane - middleLineA).normalized;
                    if (dirArrow == 1)
                        middleLineA = middleLineA - dirMiddleToMiddleLineA;
                    ExtDrawGuizmos.DrawArrow(middleLineA, dirMiddleToMiddleLineA * dirArrow);

                    Vector3 middleLineB = ExtQuaternion.GetMiddleOfXPoint(new Vector3[] { gravityAttractor.gravityTriangles[i].pointB.position, gravityAttractor.gravityTriangles[i].pointC.position });
                    Vector3 dirMiddleToMiddleLineB = (middlePlane - middleLineB).normalized;
                    if (dirArrow == 1)
                        middleLineB = middleLineB - dirMiddleToMiddleLineB;
                    ExtDrawGuizmos.DrawArrow(middleLineB, (middlePlane - middleLineB).normalized * dirArrow);

                    Vector3 middleLineC = ExtQuaternion.GetMiddleOfXPoint(new Vector3[] { gravityAttractor.gravityTriangles[i].pointC.position, gravityAttractor.gravityTriangles[i].pointA.position });
                    Vector3 dirMiddleToMiddleLineC = (middlePlane - middleLineC).normalized;
                    if (dirArrow == 1)
                        middleLineC = middleLineC - dirMiddleToMiddleLineC;
                    ExtDrawGuizmos.DrawArrow(middleLineC, (middlePlane - middleLineC).normalized * dirArrow);
                }
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

                Gizmos.color = Color.white;
                if (gravityAttractor.gravityQuad[i].GetPointInfo().range > 0 && alwaysShow)
                {
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityQuad[i].pointA.position, gravityAttractor.gravityQuad[i].pointB.position, Color.white, gravityAttractor.gravityQuad[i].GetPointInfo().range);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityQuad[i].pointB.position, gravityAttractor.gravityQuad[i].pointC.position, Color.white, gravityAttractor.gravityQuad[i].GetPointInfo().range);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityQuad[i].pointC.position, gravityAttractor.gravityQuad[i].pointD.position, Color.white, gravityAttractor.gravityQuad[i].GetPointInfo().range);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityQuad[i].pointD.position, gravityAttractor.gravityQuad[i].pointA.position, Color.white, gravityAttractor.gravityQuad[i].GetPointInfo().range);

                    Gizmos.DrawWireSphere(gravityAttractor.gravityQuad[i].pointA.position, gravityAttractor.gravityQuad[i].GetPointInfo().range);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityQuad[i].pointB.position, gravityAttractor.gravityQuad[i].GetPointInfo().range);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityQuad[i].pointC.position, gravityAttractor.gravityQuad[i].GetPointInfo().range);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityQuad[i].pointD.position, gravityAttractor.gravityQuad[i].GetPointInfo().range);
                }
                if (gravityAttractor.gravityQuad[i].GetPointInfo().maxRange > gravityAttractor.gravityQuad[i].GetPointInfo().range && alwaysShow)
                {
                    Gizmos.color = Color.red;
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityQuad[i].pointA.position, gravityAttractor.gravityQuad[i].pointB.position, Color.red, gravityAttractor.gravityQuad[i].GetPointInfo().maxRange);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityQuad[i].pointB.position, gravityAttractor.gravityQuad[i].pointC.position, Color.red, gravityAttractor.gravityQuad[i].GetPointInfo().maxRange);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityQuad[i].pointC.position, gravityAttractor.gravityQuad[i].pointD.position, Color.red, gravityAttractor.gravityQuad[i].GetPointInfo().maxRange);
                    ExtDrawGuizmos.DrawCylinder(gravityAttractor.gravityQuad[i].pointD.position, gravityAttractor.gravityQuad[i].pointA.position, Color.red, gravityAttractor.gravityQuad[i].GetPointInfo().maxRange);

                    Gizmos.DrawWireSphere(gravityAttractor.gravityQuad[i].pointA.position, gravityAttractor.gravityQuad[i].GetPointInfo().maxRange);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityQuad[i].pointB.position, gravityAttractor.gravityQuad[i].GetPointInfo().maxRange);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityQuad[i].pointC.position, gravityAttractor.gravityQuad[i].GetPointInfo().maxRange);
                    Gizmos.DrawWireSphere(gravityAttractor.gravityQuad[i].pointD.position, gravityAttractor.gravityQuad[i].GetPointInfo().maxRange);
                }

                Vector3 middlePlane = ExtQuaternion.GetMiddleOfXPoint(new Vector3[] { gravityAttractor.gravityQuad[i].pointA.position,
                    gravityAttractor.gravityQuad[i].pointB.position,
                    gravityAttractor.gravityQuad[i].pointC.position,
                    gravityAttractor.gravityQuad[i].pointD.position}, true);

                if (gravityAttractor.gravityQuad[i].unidirectionnal)
                {
                    ExtTriangle triangleA = new ExtTriangle(gravityAttractor.gravityQuad[i].pointA.position, gravityAttractor.gravityQuad[i].pointB.position, gravityAttractor.gravityQuad[i].pointC.position,
                            gravityAttractor.gravityQuad[i].unidirectionnal,
                            gravityAttractor.gravityQuad[i].inverseDirection,
                            gravityAttractor.gravityQuad[i].infinitePlane,
                            gravityAttractor.gravityQuad[i].noGravityBorders);
                    if (!gravityAttractor.gravityQuad[i].inverseDirection)
                        ExtDrawGuizmos.DrawArrow(middlePlane - triangleA.TriNorm.normalized, triangleA.TriNorm.normalized);
                    else
                        ExtDrawGuizmos.DrawArrow(middlePlane + triangleA.TriNorm.normalized, -triangleA.TriNorm.normalized);

                    ExtTriangle triangleB = new ExtTriangle(gravityAttractor.gravityQuad[i].pointC.position, gravityAttractor.gravityQuad[i].pointD.position, gravityAttractor.gravityQuad[i].pointA.position,
                            gravityAttractor.gravityQuad[i].unidirectionnal,
                            gravityAttractor.gravityQuad[i].inverseDirection,
                            gravityAttractor.gravityQuad[i].infinitePlane,
                            gravityAttractor.gravityQuad[i].noGravityBorders);
                    if (!gravityAttractor.gravityQuad[i].inverseDirection)
                        ExtDrawGuizmos.DrawArrow(middlePlane - triangleB.TriNorm.normalized, triangleB.TriNorm.normalized);
                    else
                        ExtDrawGuizmos.DrawArrow(middlePlane + triangleB.TriNorm.normalized, -triangleB.TriNorm.normalized);
                }

                if (gravityAttractor.gravityQuad[i].infinitePlane || gravityAttractor.gravityQuad[i].noGravityBorders)
                {
                    int dirArrow = (gravityAttractor.gravityQuad[i].infinitePlane) ? -1 : 1;

                    Vector3 middleLineA = ExtQuaternion.GetMiddleOfXPoint(new Vector3[] { gravityAttractor.gravityQuad[i].pointA.position, gravityAttractor.gravityQuad[i].pointB.position });
                    Vector3 dirMiddleToMiddleLineA = (middlePlane - middleLineA).normalized;
                    if (dirArrow == 1)
                        middleLineA = middleLineA - dirMiddleToMiddleLineA;
                    ExtDrawGuizmos.DrawArrow(middleLineA, dirMiddleToMiddleLineA * dirArrow);

                    Vector3 middleLineB = ExtQuaternion.GetMiddleOfXPoint(new Vector3[] { gravityAttractor.gravityQuad[i].pointB.position, gravityAttractor.gravityQuad[i].pointC.position });
                    Vector3 dirMiddleToMiddleLineB = (middlePlane - middleLineB).normalized;
                    if (dirArrow == 1)
                        middleLineB = middleLineB - dirMiddleToMiddleLineB;
                    ExtDrawGuizmos.DrawArrow(middleLineB, (middlePlane - middleLineB).normalized * dirArrow);

                    Vector3 middleLineC = ExtQuaternion.GetMiddleOfXPoint(new Vector3[] { gravityAttractor.gravityQuad[i].pointC.position, gravityAttractor.gravityQuad[i].pointD.position });
                    Vector3 dirMiddleToMiddleLineC = (middlePlane - middleLineC).normalized;
                    if (dirArrow == 1)
                        middleLineC = middleLineC - dirMiddleToMiddleLineC;
                    ExtDrawGuizmos.DrawArrow(middleLineC, (middlePlane - middleLineC).normalized * dirArrow);

                    Vector3 middleLineD = ExtQuaternion.GetMiddleOfXPoint(new Vector3[] { gravityAttractor.gravityQuad[i].pointD.position, gravityAttractor.gravityQuad[i].pointA.position });
                    Vector3 dirMiddleToMiddleLineD = (middlePlane - middleLineD).normalized;
                    if (dirArrow == 1)
                        middleLineD = middleLineD - dirMiddleToMiddleLineD;
                    ExtDrawGuizmos.DrawArrow(middleLineD, (middlePlane - middleLineD).normalized * dirArrow);
                }
            }
        }
    }

    /// <summary>
    /// a point has been removed, clean the one with bad point
    /// </summary>
    public void CleanTheSick()
    {
        for (int i = gravityAttractor.gravityPoints.Count - 1; i >= 0; i--)
        {
            if (gravityAttractor.gravityPoints[i].point == null)
            {
                gravityAttractor.gravityPoints.RemoveAt(i);
            }
        }
        for (int i = gravityAttractor.gravityLines.Count - 1; i >= 0; i--)
        {
            if (gravityAttractor.gravityLines[i].pointA == null || gravityAttractor.gravityLines[i].pointB == null)
            {
                gravityAttractor.gravityLines.RemoveAt(i);
            }
        }
        for (int i = gravityAttractor.gravityTriangles.Count - 1; i >= 0; i--)
        {
            if (gravityAttractor.gravityTriangles[i].pointA == null || gravityAttractor.gravityTriangles[i].pointB == null
                || gravityAttractor.gravityTriangles[i].pointC == null)
            {
                gravityAttractor.gravityTriangles.RemoveAt(i);
            }
        }
        for (int i = gravityAttractor.gravityQuad.Count - 1; i >= 0; i--)
        {
            if (gravityAttractor.gravityQuad[i].pointA == null || gravityAttractor.gravityQuad[i].pointB == null
                || gravityAttractor.gravityQuad[i].pointC == null || gravityAttractor.gravityQuad[i].pointD == null)
            {
                gravityAttractor.gravityQuad.RemoveAt(i);
            }
        }
    }
    /*
    public void ResetAfterUndo()
    {
        Debug.Log("undo/redo performed !");
        gravityAttractor.SetupArrayPoints();
        gravityAttractor.valueArrayChanged = true;
        SetupEditorPoint();
    }
    */

    

    private void OnDrawGizmos()
    {
        if (!gravityAttractor/* || !parentAlones || !parentLines || !parentTriangles || !parentQuad*/)
            return;

        DisplayPoint();
    }
}
