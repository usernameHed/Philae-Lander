
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IARabbitJump : EntityJump
{
    [SerializeField, Tooltip("ref script")]
    private float addRandomJump = 4f;
    [SerializeField, Tooltip("ref script")]
    private float ratioMoveForward = 1.5f;

    [SerializeField, Tooltip("ref script")]
    private IARabbitController iaRabbitController = null;

    /// <summary>
    /// called when grounded (after a jump, or a fall !)
    /// </summary>
    public override void OnGrounded()
    {
        base.OnGrounded();


        coolDownOnGround.StartCoolDown(justGroundTimer + ExtRandom.GetRandomNumber(0f, addRandomJump));
    }

    private void JumpManager()
    {
        if (IsJumpCoolDebugDownReady() && hasJumped &&
            iaRabbitController.GetMoveState() != EntityController.MoveState.InAir)
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
            DoTheJump(1);
        }
    }

    public void DoTheJump(float boostHeight)
    {
        coolDownWhenJumped.StartCoolDown(justJumpedTimer);
        iaRabbitController.ChangeState(EntityController.MoveState.InAir);

        //SoundManager.Instance.PlaySound(iaRabbitController.SFX_jump);


        rb.ClearVelocity();

        base.DoJump(boostHeight, ratioMoveForward);

        hasJumped = true;
        //Debug.Break();
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
