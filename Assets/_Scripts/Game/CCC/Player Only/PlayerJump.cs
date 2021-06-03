
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

public class PlayerJump : EntityJump
{
    [SerializeField, Tooltip("ref script")]
    private PlayerController playerController = null;

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
            DoTheJump(1, false);
        }
    }

    public void DoTheJump(float boostHeight, bool fromCode)
    {
        coolDownWhenJumped.StartCoolDown(justJumpedTimer);
        playerController.ChangeState(EntityController.MoveState.InAir);

        rb.ClearVelocity();

        //SoundManager.Instance.PlaySound(playerController.SFX_jump, true);
        //SoundManager.Instance.PlaySound(playerController.SFX_playerMove, false);
        playerController.animator.SetBool("isJUMP", true);

        base.DoJump(boostHeight);

        if (!stayHold && !fromCode)
            jumpStop = true;

        hasJumped = true;
        //Debug.Break();
    }


    public override void OnGrounded()
    {
        base.OnGrounded();

        playerController.animator.SetBool("isJUMP", false);
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
