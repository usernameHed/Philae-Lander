﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothNormals : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private float smoothSpeedCamera = 2f;
    [FoldoutGroup("GamePlay"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private float smoothSpeedPlayer = 2f;


    [FoldoutGroup("Object"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private PlayerGravity playerGravity;
    [FoldoutGroup("Object"), Tooltip("player object"), SerializeField]
    private GameObject rbObject;
    [FoldoutGroup("Debug"), Tooltip("Smoothed normals"), SerializeField, ReadOnly]
    private Vector3 smoothedNormalCamera;
    [FoldoutGroup("Debug"), Tooltip("Smoothed normals"), SerializeField, ReadOnly]
    private Vector3 smoothedNormalPlayer;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityController entityController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private GroundCheck groundCheck;

    private void Start()
    {
        smoothedNormalCamera = GetRotationOrientationDown();
        smoothedNormalPlayer = GetRotationOrientationDown();
    }

    public Vector3 GetSmoothedNormalCamera()
    {
        return (smoothedNormalCamera);
    }
    public Vector3 GetSmoothedNormalPlayer()
    {
        return (smoothedNormalPlayer);
    }


    private Vector3 GetRotationOrientationDown()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            return (playerGravity.GetMainAndOnlyGravity());
        }
        return (groundCheck.GetDirLastNormal());
    }

    private void CalculateSmoothNormal()
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
