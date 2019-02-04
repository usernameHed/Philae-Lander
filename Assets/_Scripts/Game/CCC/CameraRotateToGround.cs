using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateToGround : RotateToGround
{
    [FoldoutGroup("Object"), Tooltip("speed of rotation to ground"), SerializeField]
    public CameraController cameraController;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    private SmoothNormals smoothNormals;

    private void Start()
    {
        InstantRotate(smoothNormals.GetSmoothedNormalCamera());
    }

    private void FixedUpdate()
    {
        RotateObject(cameraController.GetRotateSpeedRotate(), smoothNormals.GetSmoothedNormalCamera());
    }
}
