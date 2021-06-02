using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAFollowerJump : EntityJump
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    private float addRandomJump = 4f;
    /// <summary>
    /// called when grounded (after a jump, or a fall !)
    /// </summary>
    public override void OnGrounded()
    {
        base.OnGrounded();

        Debug.Log("grounded !");

        coolDownOnGround.StartCoolDown(justGroundTimer + ExtRandom.GetRandomNumber(0f, addRandomJump));
    }

    private void JumpManager()
    {
        if (IsJumpCoolDebugDownReady() && hasJumped &&
            entityController.GetMoveState() != EntityController.MoveState.InAir)
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
        entityController.ChangeState(EntityController.MoveState.InAir);

        ExtLog.DebugLogIa("jump !", ExtLog.Log.IA);
        //SoundManager.Instance.PlaySound(entityController.SFX_jump);


        rb.ClearVelocity();

        
        base.DoJump(boostHeight);

        hasJumped = true;
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
