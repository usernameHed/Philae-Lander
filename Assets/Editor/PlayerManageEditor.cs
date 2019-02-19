using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[CustomEditor(typeof(PlayerEditor))]
public class PlayerManageEditor : Editor
{
    private PlayerEditor playerEditor;

    private void OnEnable()
    {
        EditorApplication.update += OwnUpdate;
        playerEditor = (PlayerEditor)target;
    }

    private void OwnUpdate()
    {
        
        if (Application.IsPlaying(playerEditor.rb))
        {
            if (playerEditor.playerInput.dashUp)
            {
                FocusOnSelection();
            }
            else if (playerEditor.playerInput.focusUp)
            {
                playerEditor.playerInput.focusUp = false;
                FocusOnSelection();
                Debug.Break();
            }
        }
    }

    private void FocusOnSelection()
    {
        SceneView.lastActiveSceneView.LookAt(playerEditor.rb.position);

        SceneViewCameraFunction.ViewportPanZoomIn();
    }

    private void OnDisable()
    {
        EditorApplication.update -= OwnUpdate;
    }
}