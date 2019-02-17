using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(GravityAttractorEditor)), CanEditMultipleObjects]
public class PositionHandleEditor : Editor
{
    public enum MouseClicType
    {
        CLICK,
        HOLD,
        RELEASE,
        NONE,
    }

    private struct InfoFusion
    {
        public Transform point;
        public int indexScript;
        public int indexShape;
        public GravityAttractor.GravityPointType gravityPointType;
        public int indexPointInShape;
    }

    private static float marginFusion = 50f;
    private static float marginOverPoint = 50f;
    private GravityAttractorEditor ldGravityAttractor;
    private Vector3 mouse2dPosition;

    private GameObject objSelected = null;
    private Vector3 savePos;
    private MouseClicType mouseClic = MouseClicType.NONE;
    private int indexFound = -1;
    private List<InfoFusion> allPointToFusion = new List<InfoFusion>();
    private GravityAttractorEditor[] allGravityAttractor;

    private void OnEnable()
    {
        EditorApplication.update += OwnUpdate;
        ldGravityAttractor = (GravityAttractorEditor)target;
    }

    private void OwnUpdate()
    {
        //enumeratorReset.MoveNext();
        /*if (haveToReset)
        {
            haveToReset = false;
            ldGravityAttractor.ResetAfterUndo();
        }*/
    }

    private void MouseInputs(SceneView view)
    {
        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                //GUIUtility.hotControl = controlID;
                //Debug.Log("Left Click");

                mouseClic = MouseClicType.CLICK;


                break;
            case EventType.MouseUp:
                //GUIUtility.hotControl = controlID;
                mouseClic = MouseClicType.RELEASE;
                //e.Use();
                break;

            case EventType.MouseDrag:
                mouseClic = MouseClicType.HOLD;
                break;

        }

        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z)
        {
            Undo.PerformUndo();
            EditorCoroutineUtility.StartCoroutine(WaitUntilUndo(), ldGravityAttractor.gameObject);
            e.Use();
        }
        else if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Y)
        {
            Undo.PerformRedo();
            EditorCoroutineUtility.StartCoroutine(WaitUntilUndo(), ldGravityAttractor.gameObject);
            e.Use();
        }
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.F)
        {
            FocusOnSelection();
            e.Use();
        }

        TryToFusion(e, view);
        TryToUnselect(e);        
    }

    private void FocusOnSelection()
    {
        if (objSelected)
            SceneView.lastActiveSceneView.LookAt(objSelected.transform.position);
    }

    private IEnumerator WaitUntilUndo()
    {
        yield return new WaitForEndOfFrame();
        //Debug.Log("ici undo ???");
        ldGravityAttractor.ResetAfterUndo();
    }

    private void TryToUnselect(Event e)
    {
        if (e.button == 1 || e.button == 2 || e.keyCode == KeyCode.Escape || e.alt || e.control)
        {
            objSelected = null;
        }
    }

    private void TryToFusion(Event e, SceneView view)
    {
        if (!(e.button == 1 && e.shift && mouseClic == MouseClicType.RELEASE))
            return;

        
        allGravityAttractor = GameObject.FindObjectsOfType<GravityAttractorEditor>();
        allPointToFusion.Clear();

        for (int k = 0; k < allGravityAttractor.Length; k++)
        {
            for (int i = 0; i < allGravityAttractor[k].GetGravityAttractor().gravityPoints.Count; i++)
            {
                Vector2 pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityPoints[i].point.position);
                bool isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityPoints[i].point,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.POINT,
                        indexPointInShape = 0,
                    };
                    allPointToFusion.Add(infoFusion);
                }
            }
            for (int i = 0; i < allGravityAttractor[k].GetGravityAttractor().gravityLines.Count; i++)
            {
                Vector2 pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityLines[i].pointA.position);
                bool isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityLines[i].pointA,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.LINE,
                        indexPointInShape = 0,
                    };
                    allPointToFusion.Add(infoFusion);
                }
                pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityLines[i].pointB.position);
                isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityLines[i].pointA,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.LINE,
                        indexPointInShape = 1,
                    };
                    allPointToFusion.Add(infoFusion);
                }
            }
            for (int i = 0; i < allGravityAttractor[k].GetGravityAttractor().gravityTriangles.Count; i++)
            {
                Vector2 pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityTriangles[i].pointA.position);
                bool isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityTriangles[i].pointA,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.TRIANGLE,
                        indexPointInShape = 0,
                    };
                    allPointToFusion.Add(infoFusion);
                }
                pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityTriangles[i].pointB.position);
                isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityTriangles[i].pointA,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.TRIANGLE,
                        indexPointInShape = 1,
                    };
                    allPointToFusion.Add(infoFusion);
                }
                pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityTriangles[i].pointC.position);
                isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityTriangles[i].pointA,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.TRIANGLE,
                        indexPointInShape = 2,
                    };
                    allPointToFusion.Add(infoFusion);
                }
            }
            for (int i = 0; i < allGravityAttractor[k].GetGravityAttractor().gravityQuad.Count; i++)
            {
                Vector2 pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityQuad[i].pointA.position);
                bool isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityQuad[i].pointA,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.QUAD,
                        indexPointInShape = 0,
                    };
                    allPointToFusion.Add(infoFusion);
                }
                pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityQuad[i].pointB.position);
                isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityQuad[i].pointA,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.QUAD,
                        indexPointInShape = 1,
                    };
                    allPointToFusion.Add(infoFusion);
                }
                pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityQuad[i].pointC.position);
                isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityQuad[i].pointA,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.QUAD,
                        indexPointInShape = 2,
                    };
                    allPointToFusion.Add(infoFusion);
                }
                pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetGravityAttractor().gravityQuad[i].pointD.position);
                isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetGravityAttractor().gravityQuad[i].pointA,
                        indexScript = k,
                        indexShape = i,
                        gravityPointType = GravityAttractor.GravityPointType.QUAD,
                        indexPointInShape = 3,
                    };
                    allPointToFusion.Add(infoFusion);
                }
            }
        }
        if (allPointToFusion.Count <= 1)
            return;
        for (int i = 1; i < allPointToFusion.Count; i++)
        {
            TryToReAssemble(allPointToFusion[i], allPointToFusion[0]);  //assemble current with the first
        }
        for (int i = 0; i < allPointToFusion.Count; i++)
        {
            allGravityAttractor[allPointToFusion[i].indexScript].ResetAfterUndo();
        }
        Debug.Log("ok fusion !");
    }

    private void TryToReAssemble(InfoFusion infoFusionToDelete, InfoFusion infoFusionToKeep)
    {
        if (infoFusionToDelete.gravityPointType == GravityAttractor.GravityPointType.POINT)
        {
            GravityAttractor.GravityPoint gravityPointToDelete = allGravityAttractor[infoFusionToDelete.indexScript].GetGravityAttractor().gravityPoints[infoFusionToDelete.indexShape];
            gravityPointToDelete.ChangePoint(infoFusionToDelete.indexPointInShape, infoFusionToKeep.point);
        }
        else if (infoFusionToDelete.gravityPointType == GravityAttractor.GravityPointType.LINE)
        {
            GravityAttractor.GravityLine gravityLineToDelete = allGravityAttractor[infoFusionToDelete.indexScript].GetGravityAttractor().gravityLines[infoFusionToDelete.indexShape];
            gravityLineToDelete.ChangePoint(infoFusionToDelete.indexPointInShape, infoFusionToKeep.point);
        }
        else if (infoFusionToDelete.gravityPointType == GravityAttractor.GravityPointType.TRIANGLE)
        {
            GravityAttractor.GravityTriangle gravityTriangleToDelete = allGravityAttractor[infoFusionToDelete.indexScript].GetGravityAttractor().gravityTriangles[infoFusionToDelete.indexShape];
            gravityTriangleToDelete.ChangePoint(infoFusionToDelete.indexPointInShape, infoFusionToKeep.point);
        }
        else if (infoFusionToDelete.gravityPointType == GravityAttractor.GravityPointType.QUAD)
        {
            GravityAttractor.GravityQuad gravityQuadToDelete = allGravityAttractor[infoFusionToDelete.indexScript].GetGravityAttractor().gravityQuad[infoFusionToDelete.indexShape];
            gravityQuadToDelete.ChangePoint(infoFusionToDelete.indexPointInShape, infoFusionToKeep.point);
        }
    }


    public void SetAllGravityPointArray(SceneView view)
    {
        if (ldGravityAttractor.GetLenghtOfAllPoints() == 0)
        {
            //Debug.LogError("not mathing size !");
            return;// (null);
        }
        //Vector3[] allPoints = ldGravityAttractor.GetAllPointToArray();

        for (int i = 0; i < ldGravityAttractor.allPosGravityPoint.Length; i++)
        {
            if (!ldGravityAttractor.allPosGravityPoint[i])
                continue;
            Vector2 pos2dObject = view.camera.WorldToScreenPoint(ldGravityAttractor.allPosGravityPoint[i].position);
            ldGravityAttractor.allModifiedPosGravityPoint[i] = pos2dObject;
        }
        //return (ldGravityAttractor.allModifiedPosGravityPoint);
    }

    private void SearchNewHandler(SceneView view)
    {
        if (mouseClic == MouseClicType.CLICK || mouseClic == MouseClicType.HOLD)
        {
            //Debug.Log("don't search other...");
            return;
        }

        if (ldGravityAttractor.allModifiedPosGravityPoint == null)
            return;

        if (ldGravityAttractor.allModifiedPosGravityPoint.Length == 0)
            return;

        SetAllGravityPointArray(view);
        Vector3 closestPoint = ExtUtilityFunction.GetClosestPoint(mouse2dPosition, ldGravityAttractor.allModifiedPosGravityPoint, ref indexFound);

        if (ldGravityAttractor.allModifiedPosGravityPoint == null)
            return;
        if (ldGravityAttractor.allPosGravityPoint[indexFound] == null)
            return;

        if (indexFound >= 0 && indexFound < ldGravityAttractor.allModifiedPosGravityPoint.Length
            && objSelected != ldGravityAttractor.allPosGravityPoint[indexFound].gameObject)
        {
            objSelected = ldGravityAttractor.allPosGravityPoint[indexFound].gameObject;
        }
    }

    private void HandlerManager()
    {
        if (!objSelected || !objSelected.transform)
            return;

        Undo.RecordObject(objSelected.transform, "Move Handler");
        EditorGUI.BeginChangeCheck();

        if (objSelected && objSelected.transform)
            savePos = Handles.PositionHandle(objSelected.transform.position, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            objSelected.transform.position = savePos;
        }
    }

    protected virtual void OnSceneGUI()
    {
        if (!ldGravityAttractor.creatorMode)
        {
            //Tools.current = Tool.Transform;
            return;
        }
        Tools.current = Tool.View;

        //Vector3[] allPos = new Vector3[ldGravityAttractor.GetAllGravityPoint().Count];


        SceneView view = SceneView.currentDrawingSceneView;
        mouse2dPosition = Event.current.mousePosition;
        mouse2dPosition.y = view.camera.pixelRect.height - mouse2dPosition.y;

        MouseInputs(view);
        SearchNewHandler(view);
        HandlerManager();
    }

    private void OnDisable()
    {
        EditorApplication.update -= OwnUpdate;
    }
}