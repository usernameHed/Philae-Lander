﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToGround : MonoBehaviour
{
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    private SmoothNormals smoothNormals;
    [FoldoutGroup("Object"), Tooltip("ref object to rotate"), SerializeField]
    private GameObject rbObject;

    protected void InstantRotate()
    {
        Vector3 dirOrientation = smoothNormals.GetSmoothedNormal();

        rbObject.transform.rotation = Quaternion.FromToRotation(rbObject.transform.up, dirOrientation) * rbObject.transform.rotation;
    }

    protected void RotateObject(float speedRotate)
    {
        Vector3 dirOrientation = smoothNormals.GetSmoothedNormal();

        //Debug.DrawRay(rbObject.transform.position, dirOrientation * 5, Color.green, 0.3f);

        //UnityRotateExtensions.Rotate_DegreesPerSecond(rbObject, dirOrientation, speedRotate);
        Quaternion targetRotation = Quaternion.FromToRotation(rbObject.transform.up, dirOrientation) * rbObject.transform.rotation;
        rbObject.transform.rotation = Quaternion.RotateTowards(rbObject.transform.rotation, targetRotation, speedRotate * Time.deltaTime);
    }
}
