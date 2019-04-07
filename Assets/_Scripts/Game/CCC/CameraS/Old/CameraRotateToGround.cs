using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateToGround : RotateToGround
{
    [FoldoutGroup("Object"), Tooltip("speed of rotation to ground"), SerializeField]
    public CameraControllerOld cameraController = null;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    private SmoothNormals smoothNormals = null;

    private void Start()
    {
        InstantRotate(smoothNormals.GetSmoothedNormalCamera());
    }

    private void FixedUpdate()
    {
        RotateObject(cameraController.GetRotateSpeedRotate(), smoothNormals.GetSmoothedNormalCamera());
    }
}
