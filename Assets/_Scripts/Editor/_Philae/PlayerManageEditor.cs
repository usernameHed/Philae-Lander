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
    private GravityAttractorLD.PointInfo point;

    private void OnEnable()
    {
        EditorApplication.update += OwnUpdate;
        playerEditor = (PlayerEditor)target;
    }

    private void OwnUpdate()
    {
        if (Application.isPlaying)
        {
            /*
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
            */
        }
    }

    private void OnSceneGUI()
    {
        if (!Application.isPlaying)
        {
            PhilaeManager phiTmp = ExtUtilityEditor.GetScript<PhilaeManager>();
            point = ExtGetGravityAtPoints.GetAirSphereGravityStatic(playerEditor.rb.transform.position, phiTmp.ldManager.allGravityAttractorLd);

            Debug.DrawLine(playerEditor.rb.transform.position, point.posRange, Color.cyan);
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