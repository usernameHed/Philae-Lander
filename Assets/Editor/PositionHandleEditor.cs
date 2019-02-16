using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(GravityAttractorEditor)), CanEditMultipleObjects]
public class PositionHandleEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        GravityAttractorEditor example = (GravityAttractorEditor)target;

        if (!example.creatorMode)
        {
            //Tools.current = Tool.Transform;
            return;
        }
        Tools.current = Tool.View;

        Vector3[] allPos = new Vector3[example.GetAllGravityPoint().Count];
        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < example.GetAllGravityPoint().Count; i++)
        {
            allPos[i] = Handles.PositionHandle(example.GetAllGravityPoint()[i].position, Quaternion.identity);
            //Undo.RecordObject(example.GetAllGravityPoint()[i].gameObject, "Move Custom Handle on: " + example.GetAllGravityPoint()[i].gameObject.name);

            //example.GetAllGravityPoint()[i].position = newTargetPosition;
        }
        if (EditorGUI.EndChangeCheck())
        {
            for (int i = 0; i < example.GetAllGravityPoint().Count; i++)
            {
                example.GetAllGravityPoint()[i].position = allPos[i];
            }
        }
    }
}