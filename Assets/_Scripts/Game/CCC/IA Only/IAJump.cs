using Sirenix.OdinInspector;
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
            return (false);

        if (hasJumped)
            return (false);

        //don't jump if we just grounded
        if (!coolDownOnGround.IsReady())
            return (false);

        if (!entityContactSwitch.IsCoolDownSwitchReady())
            return (false);

        return (true);
    }

    /// <summary>
    /// called when grounded (after a jump, or a fall !)
    /// </summary>
    public override void OnGrounded()
    {
        base.OnGrounded();

        ExtLog.DebugLogIa("Grounded !", ExtLog.Log.IA);

        coolDownOnGround.StartCoolDown(justGroundTimer + ExtRandom.GetRandomNumber(0f, addRandomJump));
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
            SoundManager.Instance.PlaySound(GameData.Sounds.Ennemy_Jump_start.ToString() + rb.transform.GetInstanceID());


            rb.ClearVelocity();
            entityAttractor.CreateAttractor();

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
