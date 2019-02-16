using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(GravityAttractorEditor)), CanEditMultipleObjects]
public class PositionHandleEditor : Editor
{
    private static float marginOverPoint = 0.2f;

    protected virtual void OnSceneGUI()
    {
        GravityAttractorEditor ldGravityAttractor = (GravityAttractorEditor)target;

        if (!ldGravityAttractor.creatorMode)
        {
            //Tools.current = Tool.Transform;
            return;
        }
        Tools.current = Tool.View;

        Vector3[] allPos = new Vector3[ldGravityAttractor.GetAllGravityPoint().Count];
        EditorGUI.BeginChangeCheck();

        var view = SceneView.currentDrawingSceneView;
        Vector2 mouse2dPose = Event.current.mousePosition;
        mouse2dPose.x = ExtUtilityFunction.Remap(mouse2dPose.x, 0, view.camera.pixelRect.width, 0f, 1f);
        mouse2dPose.y = ExtUtilityFunction.Remap(mouse2dPose.y, 0, view.camera.pixelRect.height, 0f, 1f);

        for (int i = 0; i < ldGravityAttractor.GetAllGravityPoint().Count; i++)
        {
            Vector2 pos2dObject = view.camera.WorldToViewportPoint(ldGravityAttractor.GetAllGravityPoint()[i].position);

            //Debug.Log("pos mouse: " + mouse2dPose + ", pos object: " + pos2dObject);

            //Vector3 viewPos = Ma.WorldToViewportPoint(target.position);
            //if (ExtUtilityFunction.IsClose(mouse2dPose.x, pos2dObject.x, marginOverPoint) && ExtUtilityFunction.IsClose(mouse2dPose.y, pos2dObject.y, marginOverPoint))
                allPos[i] = Handles.PositionHandle(ldGravityAttractor.GetAllGravityPoint()[i].position, Quaternion.identity);
            //Undo.RecordObject(example.GetAllGravityPoint()[i].gameObject, "Move Custom Handle on: " + example.GetAllGravityPoint()[i].gameObject.name);

            //example.GetAllGravityPoint()[i].position = newTargetPosition;
        }
        if (EditorGUI.EndChangeCheck())
        {
            for (int i = 0; i < ldGravityAttractor.GetAllGravityPoint().Count; i++)
            {
                Vector2 pos2dObject = view.camera.WorldToViewportPoint(ldGravityAttractor.GetAllGravityPoint()[i].position);

                //if (ExtUtilityFunction.IsClose(mouse2dPose.x, pos2dObject.x, marginOverPoint) && ExtUtilityFunction.IsClose(mouse2dPose.y, pos2dObject.y, marginOverPoint))
                    ldGravityAttractor.GetAllGravityPoint()[i].position = allPos[i];
            }
        }
    }
}