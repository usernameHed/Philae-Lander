﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityContactSwitch : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField]
    private bool inAirForwardWall = true;

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] walkForbiddenForwardUp = new string[] { "Walkable/NoSide", "Walkable/Up" };
    [FoldoutGroup("Forward"), Range(0f, 2f), Tooltip("dist to check forward player"), SerializeField]
    private float distForward = 0.6f;
    [FoldoutGroup("Forward"), Tooltip(""), SerializeField]
    public float sizeRadiusForward = 0.3f;
    [FoldoutGroup("Forward"), Range(0f, 1f), Tooltip(""), SerializeField]
    public float dotMarginImpact = 0.3f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), Tooltip(""), SerializeField]
    public float timeBetween2TestForward = 0.8f;

    [FoldoutGroup("Backward"), Range(0f, 2f), Tooltip("dist to check forward player"), SerializeField]
    private float distBackward = 1f;

    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityController entityController;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityGravity playerGravity;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityAction entityAction;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntitySlide entitySlide;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityJumpCalculation entityJumpCalculation;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityJump entityJump;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch;

    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isForwardWall = false;
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isForbiddenForward = false;
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isBackwardWall = false;
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isForbiddenBackward = false;


    private FrequencyCoolDown coolDownForward = new FrequencyCoolDown();

    public bool IsForwardForbiddenWall()
    {
        return (isForwardWall && isForbiddenForward);
    }

    public bool IsCoolDownSwitchReady()
    {
        return (coolDownForward.IsReady());
    }

    private void ForwardWallCheck()
    {
        RaycastHit hitInfo;

        ResetContact();

        //do nothing if not moving
        if (entityAction.NotMoving())
            return;
        //do nothing if input and forward player are not equal
        if (!entityController.IsLookingTowardTheInput(dotMarginImpact))
            return;            

        if (Physics.SphereCast(rb.transform.position, sizeRadiusForward, entityController.GetFocusedForwardDirPlayer(), out hitInfo,
                               distForward, entityController.layerMask, QueryTriggerInteraction.Ignore))
        {
            if (entityGravityAttractorSwitch.IsAirAttractorLayer(hitInfo.transform.gameObject.layer)
                && !entityGravityAttractorSwitch.IsNormalOk(hitInfo.transform, hitInfo.normal))
            {
                Debug.LogWarning("here sphereAirMove tell us we are in a bad normal, do NOT do forward");
                isForwardWall = true;
                isForbiddenForward = true;
                return;
            }

            //ExtDrawGuizmos.DebugWireSphere(rb.transform.position + (entityController.GetFocusedForwardDirPlayer()) * (distForward), Color.yellow, sizeRadiusForward, 0.1f);
            //Debug.DrawRay(rb.transform.position, (entityController.GetFocusedForwardDirPlayer()) * (distForward), Color.yellow, 5f);
            ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.red, 0.1f, 0.1f);

            isForwardWall = true;

            Vector3 normalHit = hitInfo.normal;
            Vector3 upPlayer = playerGravity.GetMainAndOnlyGravity();
            entitySlide.CalculateStraffDirection(normalHit);    //calculate SLIDE

            float dotWrongSide = ExtQuaternion.DotProduct(upPlayer, normalHit);
            if (dotWrongSide < -dotMarginImpact)
            {
                Debug.Log("forward too inclined, dotImpact: " + dotWrongSide + "( max: " + dotMarginImpact + ")");
                isForbiddenForward = true;
                return;
            }

            int isForbidden = ExtList.ContainSubStringInArray(walkForbiddenForwardUp, LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
            if (isForbidden != -1)
            {
                //here we are in front of a forbidden wall !!
                isForbiddenForward = true;
            }
            else
            {
                if (groundCheck.IsFlying() && !inAirForwardWall)
                {
                    isForbiddenForward = true;
                }
                else
                {
                    //HERE FORWARD, DO SWITCH !!
                    coolDownForward.StartCoolDown(timeBetween2TestForward);
                    //Debug.Log("forward");
                    groundCheck.SetForwardWall(hitInfo);
                    
                    isForbiddenForward = false;
                }
            }
        }
        else
        {
            ResetContact();
        }
    }

    /// <summary>
    /// backward raycast
    /// </summary>
    private void BackwardWallCheck()
    {
        RaycastHit hitInfo;

        ResetBackwardContact();

        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return;
        if (entityJumpCalculation.GetJumpType() != InfoJump.JumpType.TO_SIDE)
            return;
        if (entityJump.IsJumpedAndNotReady())
            return;

        if (Physics.SphereCast(rb.transform.position, sizeRadiusForward, -entityController.GetFocusedForwardDirPlayer(), out hitInfo,
                               distBackward, entityController.layerMask, QueryTriggerInteraction.Ignore))
        {
            //ExtDrawGuizmos.DebugWireSphere(rb.transform.position + (entityController.GetFocusedForwardDirPlayer()) * (distForward), Color.yellow, sizeRadiusForward, 0.1f);
            //Debug.DrawRay(rb.transform.position, (entityController.GetFocusedForwardDirPlayer()) * (distForward), Color.yellow, 5f);
            ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.yellow, 0.1f, 0.1f);

            isBackwardWall = true;

            Vector3 normalHit = hitInfo.normal;
            Vector3 upPlayer = playerGravity.GetMainAndOnlyGravity();

            float dotWrongSide = ExtQuaternion.DotProduct(upPlayer, normalHit);
            if (dotWrongSide < -dotMarginImpact)
            {
                Debug.LogWarning("backward too inclined, dotImpact: " + dotWrongSide + "( max: " + dotMarginImpact + ")");
                isForbiddenBackward = true;
                //Debug.Break();
                return;
            }

            int isForbidden = ExtList.ContainSubStringInArray(walkForbiddenForwardUp, LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
            if (isForbidden != -1)
            {
                //here we are in front of a forbidden wall !!
                isForbiddenBackward = true;
            }
            else
            {
                //HERE BACKWARD, DO SWITCH !!

                Debug.Log("BACKWARD");
                groundCheck.SetBackwardWall(hitInfo);

                isForbiddenBackward = false;
                //Debug.Break();
            }
        }
        else
        {
            ResetBackwardContact();
        }
    }

    private void ResetContact()
    {
        isForwardWall = false;
        isForbiddenForward = false;
    }

    private void ResetBackwardContact()
    {
        isBackwardWall = false;
        isForbiddenBackward = false;
    }

    private void FixedUpdate()
    {
        //set if the is a wall in front of us (we need flying info)
        ForwardWallCheck();
        //BackwardWallCheck();
    }
}
