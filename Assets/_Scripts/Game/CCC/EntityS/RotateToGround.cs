﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToGround : MonoBehaviour
{
    [Tooltip("ref object to rotate"), SerializeField]
    private GameObject rbObject = null;
    [Tooltip("ref object to rotate"), SerializeField]
    protected bool instantRotation = false;

    protected void InstantRotate(Vector3 dirSmoothedNormal)
    {
        Vector3 dirOrientation = dirSmoothedNormal;

        rbObject.transform.rotation = Quaternion.FromToRotation(rbObject.transform.up, dirOrientation) * rbObject.transform.rotation;
    }

    public static void InstantRotateObject(Vector3 dirOrientation, Transform objToRotate)
    {
        objToRotate.rotation = Quaternion.FromToRotation(objToRotate.transform.up, dirOrientation) * objToRotate.transform.rotation;
    }

    protected void RotateObject(float speedRotate, Vector3 dirSmoothedNormal)
    {
        Vector3 dirOrientation = dirSmoothedNormal;

        //Debug.DrawRay(rbObject.transform.position, dirOrientation * 5, Color.green, 0.3f);

        //UnityRotateExtensions.Rotate_DegreesPerSecond(rbObject, dirOrientation, speedRotate);
        Quaternion targetRotation = Quaternion.FromToRotation(rbObject.transform.up, dirOrientation) * rbObject.transform.rotation;
        rbObject.transform.rotation = Quaternion.RotateTowards(rbObject.transform.rotation, targetRotation, speedRotate * Time.deltaTime);
    }
}
