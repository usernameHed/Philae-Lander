using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothNormals : UniqueSmoothNormals
{
    [FoldoutGroup("GamePlay"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private float smoothSpeedCamera = 2f;

    [FoldoutGroup("Debug"), Tooltip("Smoothed normals"), SerializeField, ReadOnly]
    private Vector3 smoothedNormalCamera;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityController entityController = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private GroundCheck groundCheck = null;

    private void Start()
    {
        smoothedNormalCamera = GetRotationOrientationDown();
        smoothedNormalPlayer = GetRotationOrientationDown();
    }

    public Vector3 GetSmoothedNormalCamera()
    {
        return (smoothedNormalCamera);
    }

    protected override Vector3 GetRotationOrientationDown()
    {
        if (!entityController && !groundCheck)
        {
            return (uniqueGravity.GetMainAndOnlyGravity());
        }

        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            return (uniqueGravity.GetMainAndOnlyGravity());
        }
        return (groundCheck.GetDirLastNormal());
    }

    protected override void CalculateSmoothNormal()
    {
        Vector3 actualNormal = GetRotationOrientationDown();
        smoothedNormalPlayer = Vector3.Lerp(smoothedNormalPlayer, actualNormal, Time.deltaTime * smoothSpeedPlayer);
        smoothedNormalCamera = Vector3.Lerp(smoothedNormalCamera, actualNormal, Time.deltaTime * smoothSpeedCamera);
    }

    private void FixedUpdate()
    {
        CalculateSmoothNormal();
    }
}
