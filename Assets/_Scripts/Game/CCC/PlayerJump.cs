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
            entityAttractor.CreateAttractor();

            SoundManager.GetSingleton.playSound(GameData.Sounds.Player_Thruster.ToString());
            SoundManager.GetSingleton.playSound(GameData.Sounds.Player_Movement.ToString(), true);
            playerController.animator.SetBool("isJUMP", true);

            base.DoJump();
            JumpCalculation();
            Vibrate();

            if (!stayHold)
                jumpStop = true;
            
            hasJumped = true;
            //Debug.Break();
        }
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
