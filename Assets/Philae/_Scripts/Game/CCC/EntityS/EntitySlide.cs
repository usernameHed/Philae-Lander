
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public class EntitySlide : MonoBehaviour
{
    [SerializeField, Tooltip("ref rigidbody")]
    private float dotMarginNiceSlope = 0.3f;
    [SerializeField, Tooltip("ref rigidbody")]
    private Vector2 minMaxMagnitude = new Vector2(0f, 0.7f);
    [SerializeField, Tooltip("ref rigidbody")]
    private float minMagnitudeSlideWhenCastRightOrLeft = 0.4f;

    [SerializeField, Tooltip("ref rigidbody")]
    private EntityGravity playerGravity = null;
    [SerializeField, Tooltip("ref rigidbody")]
    private EntityController entityController = null;
    [SerializeField, Tooltip("ref rigidbody")]
    private EntityAction entityAction = null;
    [SerializeField, Tooltip("ref rigidbody")]
    private Rigidbody rb = null;
    [SerializeField, Tooltip("ref rigidbody")]
    private EntityRotate entityRotate = null;
    [SerializeField, Tooltip("ref rigidbody")]
    private GroundForwardCheck groundForwardCheck = default;

    private Vector3 playerStraff = Vector3.zero;

    public Vector3 GetStraffDirection()
    {
        //Debug.Log("straff required for moving: " + playerStraff);
        return (playerStraff);
    }

    /// <summary>
    /// calculate the normal direction, based on the hit forward
    /// </summary>
    public void CalculateStraffDirection(Vector3 normalHit)
    {
        Vector3 playerDir = entityController.GetFocusedForwardDirPlayer();
        //Debug.Log("ici calcul straff, normal hit: " + normalHit);
        
        //Debug.DrawRay(rb.transform.position, playerDir, Color.red, 5f);

        Vector3 upPlayer = playerGravity.GetMainAndOnlyGravity();
        float dotWrongSide = Vector3.Dot(upPlayer, normalHit);

        //here the slope is nice for normal forward ?
        if (1 - dotWrongSide < dotMarginNiceSlope)
        {
            //Debug.Log("nice slope, do nothing: dot: " + dotWrongSide + "(max: " + dotMarginNiceSlope + ")");
            playerStraff = entityController.GetFocusedForwardDirPlayer();
        }
        else
        {
            //Debug.Log("can straff !");
            Vector3 relativeDirPlayer = entityAction.GetRelativeDirection();
            float dotRight = 0f;
            float dotLeft = 0f;
            int rightOrLeft = ExtVector3.IsRightOrLeft(normalHit, upPlayer, relativeDirPlayer, ref dotRight, ref dotLeft);

            Vector3 right = Vector3.Cross(normalHit, upPlayer);

            if (rightOrLeft == 1)
            {
                playerStraff = ExtVector3.GetProjectionOfAOnB(relativeDirPlayer, right, upPlayer, minMaxMagnitude.x, minMaxMagnitude.y);// right * (dotRight);
                if (groundForwardCheck.IsAdvancedForwardCastRightOrLeft())
                {
                    //Debug.LogWarning("zero ! warning, slide if not both " + playerStraff);
                    if (playerStraff.magnitude < minMagnitudeSlideWhenCastRightOrLeft)
                    {
                        playerStraff = playerStraff.normalized * minMagnitudeSlideWhenCastRightOrLeft;
                    }
                    //playerStraff = right;
                }

                //Debug.DrawRay(rb.position, playerStraff, Color.magenta, 0.1f);
                //Debug.Log("ok right");
            }
            else if (rightOrLeft == -1)
            {
                playerStraff = ExtVector3.GetProjectionOfAOnB(relativeDirPlayer, - right, upPlayer, minMaxMagnitude.x, minMaxMagnitude.y);//-right * dotLeft;
                if (groundForwardCheck.IsAdvancedForwardCastRightOrLeft())
                {
                    //Debug.LogWarning("zero ! warning, slide if not both" + playerStraff);
                    if (playerStraff.magnitude < minMagnitudeSlideWhenCastRightOrLeft)
                    {
                        playerStraff = playerStraff.normalized * minMagnitudeSlideWhenCastRightOrLeft;
                    }
                    //playerStraff = -right;
                }

                //Debug.DrawRay(rb.position, playerStraff, Color.magenta, 0.1f);
                //Debug.Log("ok left");
            }
            else
            {
                
                if (entityAction.GetDirInput().x < 0 && (entityRotate.InputFromCameraOrientation == ExtRotation.OrientationRotation.FORWARD_AND_LEFT
                    || entityRotate.InputFromCameraOrientation == ExtRotation.OrientationRotation.FORWARD_AND_RIGHT))
                {
                    //Debug.Log("left ?");
                    playerStraff = -right;
                    return;
                }
                else if (entityAction.GetDirInput().x < 0 && (entityRotate.InputFromCameraOrientation == ExtRotation.OrientationRotation.BEHIND_AND_LEFT
                    || entityRotate.InputFromCameraOrientation == ExtRotation.OrientationRotation.BEHIND_AND_RIGHT))
                {
                    //Debug.Log("right ?");
                    playerStraff = right;
                    return;
                }
                else if (entityAction.GetDirInput().x > 0 && (entityRotate.InputFromCameraOrientation == ExtRotation.OrientationRotation.FORWARD_AND_LEFT
                    || entityRotate.InputFromCameraOrientation == ExtRotation.OrientationRotation.FORWARD_AND_RIGHT))
                {
                    //Debug.Log("right ?");
                    playerStraff = right;
                    return;
                }
                else if (entityAction.GetDirInput().x > 0 && (entityRotate.InputFromCameraOrientation == ExtRotation.OrientationRotation.BEHIND_AND_LEFT
                    || entityRotate.InputFromCameraOrientation == ExtRotation.OrientationRotation.BEHIND_AND_RIGHT))
                {
                    //Debug.Log("left ?");
                    playerStraff = -right;
                    return;
                }
                //Debug.LogError("forward ???");
                playerStraff = Vector3.zero;

            }
        }
        
    }
}
