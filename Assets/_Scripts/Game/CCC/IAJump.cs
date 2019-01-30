﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAJump : EntityJump
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    private float addRandomJump = 4f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private IAController iaController;

    private bool CanJump()
    {
        //can't jump in air
        if (!canJumpInAir && iaController.GetMoveState() == EntityController.MoveState.InAir)
        {
            //Debug.Log("ici");
            return (false);
        }
            

        if (hasJumped)
        {
            //Debug.Log("ou la");
            return (false);
        }

        //don't jump if we just grounded
        if (!coolDownOnGround.IsReady())
        {
            //Debug.Log("ou encore la");
            return (false);
        }
            

        return (true);
    }

    /// <summary>
    /// called when grounded (after a jump, or a fall !)
    /// </summary>
    public void OnGrounded()
    {
        ExtLog.DebugLogIa("Grounded !", ExtLog.Log.IA);
        coolDownWhenJumped.Reset();

        coolDownOnGround.StartCoolDown(justGroundTimer + ExtRandom.GetRandomNumber(0f, addRandomJump));
        //here, we just were falling, without jumping
        if (!hasJumped)
        {
            
        }
        //here, we just on grounded after a jump
        else
        {
            hasJumped = false;
        }
    }

    private void JumpManager()
    {
        if (IsJumpCoolDebugDownReady() && hasJumped &&
            iaController.GetMoveState() != EntityController.MoveState.InAir)
        {
            hasJumped = false;
            Debug.LogError("Unjump... error ??");
            coolDownOnGround.Reset();
            return;
        }

        if (hasJumped)
        {
            return;
        }

        if (entityAction.Jump && CanJump())
        {
            coolDownWhenJumped.StartCoolDown(justJumpedTimer);
            iaController.ChangeState(EntityController.MoveState.InAir);

            ExtLog.DebugLogIa("jump !", ExtLog.Log.IA);
            SoundManager.GetSingleton.playSound(GameData.Sounds.Ennemy_Jump_start.ToString() + rb.transform.GetInstanceID());


            rb.ClearVelocity();
            playerGravity.CreateAttractor();

            base.DoJump();
            
            hasJumped = true;
            //Debug.Break();
        }
    }

    private void Update()
    {
        //on lache, on autorise le saut encore
        if (entityAction.JumpUp)
            jumpStop = false;
    }

    private void FixedUpdate()
    {
        JumpManager();
    }
}
