using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : EntityJump
{
    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onJump = new Vibration();
    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on se pose"), SerializeField]
    private Vibration onGrounded = new Vibration();

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
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

        Debug.Log("jump !");

        rb.ClearVelocity();
        entityAttractor.CreateAttractor();

        SoundManager.Instance.PlaySound(GameData.Sounds.Player_Thruster.ToString());
        SoundManager.Instance.PlaySound(GameData.Sounds.Player_Movement.ToString(), false);
        playerController.animator.SetBool("isJUMP", true);

        base.DoJump(boostHeight);
        Vibrate();

        if (!stayHold && !fromCode)
            jumpStop = true;

        hasJumped = true;
        //Debug.Break();
    }


    public override void OnGrounded()
    {
        base.OnGrounded();

        //if (isPlayer)
        PlayerConnected.Instance.SetVibrationPlayer(playerController.idPlayer, onGrounded);
        playerController.animator.SetBool("isJUMP", false);
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
