using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PositionHandleChilds))]
public class PositionHandleChildsEditor : OdinEditor
{
    private PositionHandleChilds _positionHandle;
    private Tool LastTool = Tool.None;

    private new void OnEnable()
    {
        _positionHandle = (PositionHandleChilds)target;
        LastTool = Tools.current;
        Tools.current = Tool.None;
    }

    private void OnSceneGUI()
    {
        if (Event.current.alt)
            return;

        for (int i = 0; i < _positionHandle.allChildToMove.Count; i++)
        {
            Transform child = _positionHandle.allChildToMove[i];
            if (!child)
                continue;

            if (child.gameObject.activeInHierarchy)
                child.position = Handles.PositionHandle(child.position, child.rotation);
        }
    }

    private new void OnDisable()
    {
        Tools.current = LastTool;
    }
}
