using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(FastForwardOrientationLD), true)]
public class HandleFastForwardEditor : Editor
{
    private FastForwardOrientationLD fastForwardOrientation;
    private Vector3 savePos;

    private void OnEnable()
    {
        EditorApplication.update += OwnUpdate;
        fastForwardOrientation = (FastForwardOrientationLD)target;

        if (!fastForwardOrientation.referenceObjecct)
            fastForwardOrientation.referenceObjecct = fastForwardOrientation.gameObject.transform;

        CreateTargetIfMissing();
        //fastForwardOrientation.ResetDirection();
    }

    private void CreateTargetIfMissing()
    {
        if (!fastForwardOrientation.objectToTarget)
        {
            GameObject newPoint = new GameObject("Handle (autogenerate)");
            newPoint.transform.SetParent(fastForwardOrientation.referenceObjecct);
            newPoint.transform.localPosition = Vector3.zero;
            fastForwardOrientation.objectToTarget = newPoint.transform;
        }
    }

    private void OwnUpdate()
    {
        CreateTargetIfMissing();
    }

    private void HandlerDirection()
    {
        Undo.RecordObject(fastForwardOrientation.objectToTarget.transform, "position handler object");
        EditorGUI.BeginChangeCheck();
        savePos = Handles.PositionHandle(fastForwardOrientation.GetPosition(), Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            //fastForwardOrientation.directionGravity = savePos;
            fastForwardOrientation.objectToTarget.position = savePos;
        }
    }

    protected virtual void OnSceneGUI()
    {
        if (!fastForwardOrientation.createMode || !fastForwardOrientation.referenceObjecct)
        {
            //Tools.current = Tool.Transform;
            return;
        }
        Tools.current = Tool.View;

        HandlerDirection();
    }

    private void OnDisable()
    {
        EditorApplication.update -= OwnUpdate;
    }
}