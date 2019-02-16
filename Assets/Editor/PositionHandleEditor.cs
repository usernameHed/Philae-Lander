using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PositionHandle)), CanEditMultipleObjects]
public class PositionHandleEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        PositionHandle example = (PositionHandle)target;

        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(example.targetPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(example, "Change Look At Target Position");
            example.targetPosition = newTargetPosition;
            example.Update();
        }
    }
}