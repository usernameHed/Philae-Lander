using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PositionHandleChilds))]
public class PositionHandleChildsEditor : Editor
{
    private PositionHandleChilds _positionHandle;
    private Tool LastTool = Tool.None;

    private void OnEnable()
    {
        _positionHandle = (PositionHandleChilds)target;
        LastTool = Tools.current;
        Tools.current = Tool.None;
    }

    private void OnSceneGUI()
    {
        if (Event.current.alt)
            return;

        for (int i = 0; i < _positionHandle._allChildToMove.Count; i++)
        {
            Transform child = _positionHandle._allChildToMove[i];
            if (!child)
                continue;

            if (child.gameObject.activeInHierarchy)
            {
                Undo.RecordObject(child, "handle camPoint move");
                child.position = Handles.PositionHandle(child.position, child.rotation);
            }
                
        }
    }

    private void OnDisable()
    {
        Tools.current = LastTool;
    }
}
