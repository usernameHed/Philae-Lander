﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGravityAttractorSwitch : UniqueGravityAttractorSwitch
{
    [Tooltip(""), SerializeField]
    protected EntityController entityController;
    [Tooltip(""), SerializeField]
    protected BaseGravity baseGravity;
    [Tooltip(""), SerializeField]
    protected GroundCheck groundCheck;
    
    protected Vector3 wantedDirGravityOnGround = Vector3.zero;

    public Vector3 GetWantedGravityOnGround() => wantedDirGravityOnGround;
    
    /// <summary>
    /// gravity base apply on this attractor
    /// </summary>
    /// <returns></returns>
    public override float GetAirRatioGravity()
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return (1f);

        float normalRatio = pointInfo.gravityBaseRatio;
        return (normalRatio);
    }

    public virtual void JustJumped()
    {

    }

    public override void SetLastDirJump(Vector3 dirNormalChoosen)
    {
        base.SetLastDirJump(dirNormalChoosen);
        //lastNormalJumpChoosen = dirNormalChoosen;
        wantedDirGravityOnGround = dirNormalChoosen;
    }

    public virtual void OnGrounded()
    {

    }

    /// <summary>
    /// calculate and set the gravity
    /// if justCalculate = true, do NOT set the gravity, but return it
    /// </summary>
    /// <param name="entityPosition"></param>
    /// <param name="justCalculate"></param>
    /// <returns></returns>
    protected override void CalculateGAGravity(Vector3 rbPos)
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
        {
            pointInfo.sphereGravity = groundCheck.GetDirLastNormal();
            wantedDirGravityOnGround = GetAirSphereGravity(rbPos).sphereGravity;
        }
        else
        {
            pointInfo = GetAirSphereGravity(rbPos);
            wantedDirGravityOnGround = lastNormalJumpChoosen;
        }
    }
}
