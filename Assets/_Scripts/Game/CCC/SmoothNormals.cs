using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothNormals : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private float smoothSpeed = 5f;

    [FoldoutGroup("Object"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private PlayerGravity playerGravity;
    [FoldoutGroup("Object"), Tooltip("player object"), SerializeField]
    private GameObject rbObject;
    [FoldoutGroup("Debug"), Tooltip("Smoothed normals"), SerializeField, ReadOnly]
    private Vector3 smoothedNormal;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerController playerController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private GroundCheck groundCheck;

    private void Start()
    {
        smoothedNormal = GetRotationOrientationDown();
    }

    public Vector3 GetSmoothedNormal()
    {
        return (smoothedNormal);
    }

    private Vector3 GetRotationOrientationDown()
    {
        if (playerController.GetMoveState() == PlayerController.MoveState.InAir)
        {
            return (playerGravity.GetMainAndOnlyGravity());
        }
        return (groundCheck.GetDirLastNormal());
    }

    private void CalculateSmoothNormal()
    {
        Vector3 actualNormal = GetRotationOrientationDown();
        //Debug.DrawRay(rbObject.transform.position, actualNormal * 2, Color.magenta, 5f);

        smoothedNormal = Vector3.Lerp(smoothedNormal, actualNormal, Time.deltaTime * smoothSpeed);


        //Debug.DrawRay(rbObject.transform.position, smoothedNormal, Color.yellow, 5f);
    }

    private void FixedUpdate()
    {
        CalculateSmoothNormal();
    }
}
