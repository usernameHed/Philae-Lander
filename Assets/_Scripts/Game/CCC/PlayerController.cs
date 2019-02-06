﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class PlayerController : EntityController, IKillable
{
    public enum PhysicType
    {
        BASE,
        PLANET_CENTER,
        DIRECTION_CENTER,
    }

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private Vibration deathVibration;

    [FoldoutGroup("Object"), Tooltip("ref script")]
    public PlayerInput playerInput;
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public PlayerJump playerJump;
    
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public Transform renderPlayer;
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public Animator animator;


    [FoldoutGroup("Debug", Order = 1), SerializeField, Tooltip("id player for input")]
    public int idPlayer = 0;

    private bool isKilled = false;
    private bool isMoving = false;


    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
    }

    private void Awake()
    {
        base.Init();
        isKilled = false;
        PhilaeManager.Instance.InitPlayer(this);
    }

    

    /// <summary>
    /// called when the game is over: desactive player
    /// </summary>
    public void GameOver()
    {
        enabledScript = false;
        PlayerConnected.Instance.SetVibrationPlayer(idPlayer, deathVibration);
    }


    private void Update()
    {
        if (!enabledScript)
            return;
    }

    private void OnGrounded()
    {
        playerJump.OnGrounded();
        playerGravity.OnGrounded();
        entityAttractor.OnGrounded();
        entitySwitch.OnGrounded();

        if (PhilaeManager.Instance.cameraController.IsOnAttractorMode())
        {
            PhilaeManager.Instance.cameraController.SetBaseCamera();
        }

        SoundManager.GetSingleton.playSound(GameData.Sounds.Ennemy_Jump_End.ToString() + rb.transform.GetInstanceID());
    }

    /// <summary>
    /// set state of player
    /// </summary>
    private void ChangeState()
    {
        //Check if is flying
        if (groundCheck.IsFlying())
        {
            //IN AIR
            moveState = MoveState.InAir;
            SetDragRb(0);
            isMoving = false;
            return;
        }

        if (moveState == MoveState.InAir && groundCheck.IsSafeGrounded())
        {
            OnGrounded();
        }

        if (rb.drag != oldDrag)
            SetDragRb(oldDrag);


        if (!playerInput.NotMoving())
        {
            
            moveState = MoveState.Move;
            if (!isMoving)
            {
                SoundManager.GetSingleton.playSound(GameData.Sounds.Player_Movement.ToString());
                animator.SetBool("isMarche", true);
            }

            isMoving = true;
        }
        else
        {
            moveState = MoveState.Idle;
            if (isMoving)
            {
                SoundManager.GetSingleton.playSound(GameData.Sounds.Player_Movement.ToString(), true);
                SoundManager.GetSingleton.playSound(GameData.Sounds.Player_End_Movement.ToString());
                animator.SetBool("isMarche", false);
            }

            isMoving = false;
        }
    }

    private void FixedUpdate()
    {
        ChangeState();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, GameOver);
    }

    public void Kill()
    {
        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.Hit, rb.transform.position, rb.transform.rotation, ObjectsPooler.Instance.transform);
        //throw new System.NotImplementedException();
        PlayerConnected.Instance.SetVibrationPlayer(idPlayer, deathVibration);
        EventManager.TriggerEvent(GameData.Event.GameOver);
        renderPlayer.gameObject.SetActive(false);
        SoundManager.GetSingleton.playSound(GameData.Sounds.Player_Death.ToString());

        isKilled = true;
    }

    public void GetHit(int amount, Vector3 posAttacker)
    {
        //throw new System.NotImplementedException();
    }
}
