using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToGround : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private float speedRotate = 5f;

    [FoldoutGroup("Object"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private GravityApplyer gravityApplyer;
    [FoldoutGroup("Object"), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private GameObject rbObject;

    public Vector3 GetRotationOrientationDown()
    {
        if (groundCheck.IsFlying())
        {
            return (gravityApplyer.GetDirGravity());
        }
        return (groundCheck.GetDirLastNormal());
    }

    private void RotateObject()
    {
        
        Vector3 dirOrientation = GetRotationOrientationDown();

        Debug.DrawRay(rbObject.transform.position, dirOrientation * 5, Color.green, 0.3f);

        //UnityRotateExtensions.Rotate_DegreesPerSecond(rbObject, dirOrientation, speedRotate);
        Quaternion targetRotation = Quaternion.FromToRotation(rbObject.transform.up, dirOrientation) * rbObject.transform.rotation;
        rbObject.transform.rotation = Quaternion.RotateTowards(rbObject.transform.rotation, targetRotation, speedRotate);
    }

    private void FixedUpdate()
    {
        RotateObject();
    }
}
