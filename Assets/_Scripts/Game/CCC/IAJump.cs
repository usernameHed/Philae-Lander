using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAJump : EntityJump
{
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private IAController iaController;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.OnGrounded, OnGrounded);
    }

    private bool CanJump()
    {
        //can't jump in air
        if (!canJumpInAir && iaController.GetMoveState() == EntityController.MoveState.InAir)
            return (false);

        if (hasJumped)
            return (false);

        //faux si on hold pas et quand a pas laché
        if (jumpStop)
            return (false);

        //don't jump if we just grounded
        if (!coolDownOnGround.IsReady())
            return (false);

        return (true);
    }

    /// <summary>
    /// called when grounded (after a jump, or a fall !)
    /// </summary>
    public void OnGrounded()
    {
        Debug.Log("Grounded !");
        coolDownWhenJumped.Reset();
        //here, we just were falling, without jumping
        if (!hasJumped)
        {
            coolDownOnGround.StartCoolDown(justGroundTimer);
        }
        //here, we just on grounded after a jump
        else
        {
            //rb.ClearVelocity();
            coolDownOnGround.StartCoolDown(justGroundTimer);
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

            Debug.Log("jump !");
            
            rb.ClearVelocity();
            playerGravity.CreateAttractor();

            base.DoJump();

            if (!stayHold)
                jumpStop = true;
            
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

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.OnGrounded, OnGrounded);
    }
}
