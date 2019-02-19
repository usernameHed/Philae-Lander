﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class IAController : EntityController, IPooledObject, IKillable
{
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public IAInput iaInput;

    public enum State
    {
        WANDER,
        CHASE_PLAYER,
        //GO_TO_WAYPOINT,
    }
    public bool IsWandering
    {
        get
        {
            return (interactionState == State.WANDER);
        }
    }
    public bool IsChasingPlayer
    {
        get
        {
            return (interactionState == State.CHASE_PLAYER);
        }
    }
    /*public bool IsGoingToWayPoint
    {
        get
        {
            return (interactionState == State.GO_TO_WAYPOINT);
        }
    }*/
    [FoldoutGroup("GamePlay"), Tooltip("movement speed when we are wandering"), SerializeField]
    private bool canLosePlayer = false;
    [FoldoutGroup("GamePlay"), Tooltip("movement speed when we are wandering"), SerializeField]
    private float distForChase = 100f;
    [FoldoutGroup("GamePlay"), Tooltip("movement speed when we are wandering"), SerializeField]
    private float distForTriggerChangePlanet = 20f;
    [FoldoutGroup("GamePlay"), Tooltip("movement speed when we are wandering"), SerializeField]
    public Rigidbody rigidBodyRef;
    [FoldoutGroup("GamePlay"), Tooltip("movement speed when we are wandering"), SerializeField]
    public Transform objRotateRef;
    [FoldoutGroup("GamePlay"), Tooltip("movement speed when we are wandering"), SerializeField]
    private IAJump iAJump;
    [FoldoutGroup("GamePlay"), Tooltip("movement speed when we are wandering"), SerializeField]
    private IAInput iAInput;
    [FoldoutGroup("GamePlay"), MinMaxSlider(0, 50), Tooltip("movement speed when we are wandering"), SerializeField]
    private Vector2 iaScream;

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private State interactionState = State.WANDER;

    private FrequencyCoolDown timerScream = new FrequencyCoolDown();


    [SerializeField, ReadOnly]
    public PlayerController playerController;

    private void Awake()
    {
        base.Init();
    }

    private void Start()
    {
        StartTimerScream();
        playerController = PhilaeManager.Instance.playerControllerRef;
    }

    public void DoWandering()
    {
        //Debug.Log("Wander");
        iaInput.SetRandomInput();
        iaInput.SetRandomJump();
    }

    public bool IsNotCloseToPlayer()
    {
        return (!IsCloseToPlayer());
    }

    public bool IsCloseToPlayer()
    {
        float dist = Vector3.SqrMagnitude(rigidBodyRef.position - playerController.rb.position);
        if (dist < distForChase)
        {
            return (true);
        }
        return (false);
    }

    /*public bool IsNotOnSamePlanet()
    {
        return (!IsOnSamePlanet());
    }
    
    public bool IsOnSamePlanet()
    {
        if (playerController.playerGravity.GetMainAttractObject().GetInstanceID() == playerGravity.GetMainAttractObject().GetInstanceID())
        {
            return (true);
        }
        return (false);
    }*/

    public void SetChase()
    {
        //Debug.Log("close !!!");
        interactionState = State.CHASE_PLAYER;
    }

    public void DoChase()
    {
        //Debug.Log("Do chase");
        iaInput.SetDirectionPlayer();
        iaInput.SetRandomJump();
    }

    /*

    public bool IsCloseToTriggerOtherPlanet()
    {
        float dist = Vector3.SqrMagnitude(rigidBody.position - goToPointTarget.position);
        if (dist < distForTriggerChangePlanet)
        {
            return (true);
        }
        return (false);
    }*/

    public void LosePlayer()
    {
        if (canLosePlayer)
        {
            Debug.Log("LOSE PLAYER");
            interactionState = State.WANDER;
        }
    }

    /*
    public void ChangePlanet()
    {
        Debug.Log("try to change planet !");
        iAInput.SetJump();
    }*/

    private void OnGrounded()
    {
        iAJump.OnGrounded();
        playerGravity.OnGrounded();
        entityAttractor.OnGrounded();
        entitySwitch.OnGrounded();
        fastForward.OnGrounded();
        entityGravityAttractorSwitch.OnGrounded();

        SoundManager.GetSingleton.playSound(GameData.Sounds.Ennemy_Jump_End.ToString() + rb.transform.GetInstanceID());
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


        if (!iaInput.NotMoving())
        {
            moveState = MoveState.Move;
        }
        else
        {
            moveState = MoveState.Idle;
        }
    }

    private void StartTimerScream()
    {
        timerScream.StartCoolDown(ExtRandom.GetRandomNumber(iaScream.x, iaScream.y));
    }

    private void Update()
    {
        if (timerScream.IsStartedAndOver())
        {
            StartTimerScream();
            SoundManager.GetSingleton.playSound(GameData.Sounds.Ennemy_Scream.ToString() + rb.transform.GetInstanceID());
        }
    }

    private void FixedUpdate()
    {
        ChangeState();
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
