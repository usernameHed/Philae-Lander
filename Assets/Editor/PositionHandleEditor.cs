using System.Collections;
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

    private static float marginOverPoint = 50f;
    private GravityAttractorEditor ldGravityAttractor;
    private Vector3 mouse2dPosition;

    private GameObject objSelected = null;
    private Vector3 savePos;
    private MouseClicType mouseClic = MouseClicType.NONE;
    //private bool haveToReset = false;

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

                TryToUnselect(e);

                mouseClic = MouseClicType.CLICK;


                break;
            case EventType.MouseUp:
                //GUIUtility.hotControl = controlID;
                //Debug.Log("Left CLick Up");
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
    }

    private IEnumerator WaitUntilUndo()
    {
        yield return new WaitForEndOfFrame();
        //Debug.Log("ici undo ???");
        ldGravityAttractor.ResetAfterUndo();
    }

    private void TryToUnselect(Event e)
    {
        if (e.button == 1 || e.button == 2 || e.keyCode == KeyCode.Escape)
        {
            objSelected = null;
        }
    }

    private void SearchNewHandler(SceneView view)
    {
        if (mouseClic == MouseClicType.CLICK || mouseClic == MouseClicType.HOLD)
        {
            //Debug.Log("don't search other...");
            return;
        }

        for (int i = 0; i < ldGravityAttractor.GetAllGravityPoint().Count; i++)
        {
            Vector2 pos2dObject = view.camera.WorldToScreenPoint(ldGravityAttractor.GetAllGravityPoint()[i].position);

            bool isClose = (Mathf.Abs(mouse2dPosition.x - pos2dObject.x) < marginOverPoint && Mathf.Abs(mouse2dPosition.y - pos2dObject.y) < marginOverPoint);
            //bool isClose = ExtUtilityFunction.IsClose(mouse2dPose.x, pos2dObject.x, marginOverPoint) && ExtUtilityFunction.IsClose(mouse2dPose.y, pos2dObject.y, marginOverPoint);
            //Debug.Log("pos mouse: " + mouse2dPosition + ", pos object: " + pos2dObject + ", close: " + isClose);

            if (isClose && objSelected != ldGravityAttractor.GetAllGravityPoint()[i].gameObject)
            {
                objSelected = ldGravityAttractor.GetAllGravityPoint()[i].gameObject;
                break;
            }
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