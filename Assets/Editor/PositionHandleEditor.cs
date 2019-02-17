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
        public GravityAttractor.GravityPointType gravityPointType;
        public int indexPoint;
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
        if (!(e.button == 2 && e.shift && mouseClic == MouseClicType.RELEASE))
            return;

        /*
        allGravityAttractor = GameObject.FindObjectsOfType<GravityAttractorEditor>();
        allPointToFusion.Clear();

        for (int k = 0; k < allGravityAttractor.Length; k++)
        {
            for (int i = 0; i < allGravityAttractor[k].GetAllGravityPoint().Count; i++)
            {
                Vector2 pos2dObject = view.camera.WorldToScreenPoint(allGravityAttractor[k].GetAllGravityPoint()[i].position);
                bool isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginFusion && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginFusion);
                if (isClose)
                {
                    InfoFusion infoFusion = new InfoFusion
                    {
                        point = allGravityAttractor[k].GetAllGravityPoint()[i],
                        indexScript = k,
                        indexInList = i,
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
        */
    }

    private void TryToReAssemble(InfoFusion infoFusionToDelete, InfoFusion infoFusionToKeep)
    {
        /*
        //find the type of 
        infoFusionToKeep = FindTransformToKeep();

        //change pointAlone in InfoFusion to delete / reorder

        //GravityAttractor.GravityPoint gravityPointToKeep = allGravityAttractor[infoFusionToKeep.indexScript].GetGravityAttractor().gravityPoints[infoFusionToKeep];
        //Transform objToKeep = allGravityAttractor[infoFusionToKeep.indexScript].

        int indexScript = infoFusionToDelete.indexScript;
        int indexPoint = infoFusionToDelete.indexInList;
        for (int i = 0; i < allGravityAttractor[indexScript].GetGravityAttractor().gravityPoints.Count; i++)
        {
            GravityAttractor.GravityPoint gravityPoint = allGravityAttractor[indexScript].GetGravityAttractor().gravityPoints[i];
            if (gravityPoint.point.GetInstanceID() == infoFusionToKeep.point.GetInstanceID())
            {

            }
        }
        */
    }

    /*
    private InfoFusion FindTransformToKeep(int indexScript, Transform toKeep)
    {
        
        for (int i = 0; i < allGravityAttractor[indexScript].GetGravityAttractor().gravityPoints.Count)
        {
            if (toKeep.GetInstanceID() == allGravityAttractor[indexScript].GetGravityAttractor().gravityPoints[i].point)
                return ();
        }
    }
    */

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