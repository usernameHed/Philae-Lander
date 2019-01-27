using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : EntityJump
{
    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onJump;
    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on se pose"), SerializeField]
    private Vibration onGrounded;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerController playerController;

    private bool CanJump()
    {
        //can't jump in air
        if (!canJumpInAir && playerController.GetMoveState() == EntityController.MoveState.InAir)
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

    private void JumpManager()
    {
        if (IsJumpCoolDebugDownReady() && hasJumped &&
            playerController.GetMoveState() != EntityController.MoveState.InAir)
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
            playerController.ChangeState(EntityController.MoveState.InAir);

            Debug.Log("jump !");
            
            rb.ClearVelocity();
            playerGravity.CreateAttractor();

            base.DoJump();
            Vibrate();

            if (!stayHold)
                jumpStop = true;
            
            hasJumped = true;
            //Debug.Break();
        }
    }

    public void OnGrounded()
    {
        //if (isPlayer)
        PlayerConnected.Instance.SetVibrationPlayer(playerController.idPlayer, onGrounded);
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

    /// <summary>
    /// do a jump
    /// </summary>
    private void Vibrate()
    {
        PlayerConnected.Instance.SetVibrationPlayer(playerController.idPlayer, onJump);
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
