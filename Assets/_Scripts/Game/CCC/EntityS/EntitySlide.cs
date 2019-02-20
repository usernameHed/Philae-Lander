using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Say player can slide, or climb !")]
public class EntitySlide : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float dotMarginNiceSlope = 0.3f;
    [FoldoutGroup("GamePlay"), MinMaxSlider(0f, 1f), SerializeField, Tooltip("ref rigidbody")]
    private Vector2 minMaxMagnitude = new Vector2(0f, 0.7f);

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityController entityController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityAction entityAction;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Rigidbody rb;

    [FoldoutGroup("Debug"), ReadOnly, Tooltip("main Straff direction")]
    private Vector3 playerStraff = Vector3.zero;

    public Vector3 GetStraffDirection()
    {
        Debug.Log("straff required for moving");
        return (playerStraff);
    }

    /*/// <summary>
    /// take an angle in parameter.
    /// if the angle is close to 0, or 180, we touch floor or ceilling, reutrn false;
    /// </summary>
    private bool IsGoodAngleForSliding(Vector3 angleVector)
    {
        float closeToFloor = ExtQuaternion.GetAngleFromVector3(angleVector, Vector3.up);
        float closeToCeilling = ExtQuaternion.GetAngleFromVector3(angleVector, -Vector3.up);
        
        if (ExtQuaternion.IsAngleCloseToOtherByAmount(angleFloor, closeToFloor, marginForSlideFloor))
        {
            //Debug.DrawRay(transform.position, angleVector, Color.red, 3f);
            return (false);
        }            

            
        return (true);
    }*/

    /// <summary>
    /// calculate the normal direction, based on the hit forward
    /// </summary>
    public void CalculateStraffDirection(Vector3 normalHit)
    {
        Vector3 playerDir = entityController.GetFocusedForwardDirPlayer();

        //Debug.DrawRay(rb.transform.position, normalHit, Color.magenta, 5f);
        //Debug.DrawRay(rb.transform.position, playerDir, Color.red, 5f);

        Vector3 upPlayer = playerGravity.GetMainAndOnlyGravity();
        float dotWrongSide = ExtQuaternion.DotProduct(upPlayer, normalHit);

        //here the slope is nice for normal forward ?
        if (1 - dotWrongSide < dotMarginNiceSlope)
        {
            Debug.Log("nice slope, do nothing: dot: " + dotWrongSide + "(max: " + dotMarginNiceSlope + ")");
            playerStraff = entityController.GetFocusedForwardDirPlayer();
        }
        else
        {
            Debug.Log("can straff !");
            Vector3 relativeDirPlayer = entityAction.GetRelativeDirection();
            float dotRight = 0f;
            float dotLeft = 0f;
            int rightOrLeft = ExtQuaternion.IsRightOrLeft(normalHit, upPlayer, relativeDirPlayer, rb.position, ref dotRight, ref dotLeft);

            Vector3 right = ExtQuaternion.CrossProduct(normalHit, upPlayer);

            if (rightOrLeft == 1)
            {
                playerStraff = ExtQuaternion.GetProjectionOfAOnB(relativeDirPlayer, right, upPlayer, minMaxMagnitude.x, minMaxMagnitude.y);// right * (dotRight);
                Debug.DrawRay(rb.position, playerStraff, Color.magenta, 0.1f);
            }
            else if (rightOrLeft == -1)
            {
                playerStraff = ExtQuaternion.GetProjectionOfAOnB(relativeDirPlayer, - right, upPlayer, minMaxMagnitude.x, minMaxMagnitude.y);//-right * dotLeft;
                Debug.DrawRay(rb.position, playerStraff, Color.magenta, 0.1f);
            }
            else
            {
                Debug.LogError("forward ???");
                playerStraff = Vector3.zero;
            }
        }


    }
}
