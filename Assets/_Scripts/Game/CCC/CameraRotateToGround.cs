using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateToGround : RotateToGround
{
    [FoldoutGroup("Object"), Tooltip("speed of rotation to ground"), SerializeField]
    public CameraController cameraController;

    private void Start()
    {
        InstantRotate();
    }

    private void FixedUpdate()
    {
        RotateObject(cameraController.GetRotateSpeedRotate());
    }
}
