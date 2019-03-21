﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class CoinController : EntityController, IPooledObject, IKillable
{
    [FoldoutGroup("GamePlay"), OnValueChanged("SetKinematic"), SerializeField, Tooltip("ref script")]
    private bool isKinematic = false;

    private void Awake()
    {
        base.Init();
        SetKinematic();
    }

    private void SetKinematic()
    {
        rb.isKinematic = isKinematic;
    }

    private void OnGrounded()
    {
        baseGravity.OnGrounded();
        baseGravityAttractorSwitch.OnGrounded();
        entityNoGravity.OnGrounded();
        if (entityBumpUp)
            entityBumpUp.OnGrounded();

        SoundManager.Instance.PlaySound(GameData.Sounds.Ennemy_Jump_End.ToString() + rb.transform.GetInstanceID());
    }

    /// <summary>
    /// set state of player
    /// </summary>
    private void ChangeState()
    {
        if (moveState == MoveState.InAir && groundCheck.IsSafeGrounded())
        {
            OnGrounded();
        }

        if (groundCheck.IsFlying()/* || playerJump.IsJumpedAndNotReady()*/)
        {
            //IN AIR
            moveState = MoveState.InAir;
            SetDragRb(0);
            return;
        }

        if (rb.drag != oldDrag/* && playerJump.IsJumpCoolDebugDownReady()*/)
            SetDragRb(oldDrag);


        moveState = MoveState.Idle;
    }
    
    private void FixedUpdate()
    {
        ChangeState();
        actualVelocity = rb.velocity.magnitude;
    }

    public void OnObjectSpawn()
    {
        rb.transform.position = transform.position;
        //throw new System.NotImplementedException();
    }

    public void OnDesactivePool()
    {
        //throw new System.NotImplementedException();
    }

    public void Kill()
    {
        Destroy(gameObject);
        //throw new System.NotImplementedException();
    }

    public void GetHit(int amount, Vector3 posAttacker)
    {
        //throw new System.NotImplementedException();
    }
}
