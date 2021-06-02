﻿
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;
using UnityEssentials.time;

public class IARabbitController : EntityController, IPooledObject, IKillable
{
    [Tooltip("movement speed when we are wandering"), SerializeField]
    private bool canForgetPlayer = false;
    [Tooltip("movement speed when we are wandering"), SerializeField]
    private float distForFlee = 5f;
    [Tooltip("movement speed when we are wandering"), SerializeField]
    private float distForGoBackToNormal = 10f;

    [Tooltip("ref script")]
    public IARabbitInput IARabbitInput;

    public enum State
    {
        WANDER,
        FLEE_PLAYER,
        //GO_TO_WAYPOINT,
    }
    public bool IsWandering
    {
        get
        {
            return (interactionState == State.WANDER);
        }
    }
    public bool IsFleeingPlayer
    {
        get
        {
            return (interactionState == State.FLEE_PLAYER);
        }
    }

    [Tooltip("movement speed when we are wandering"), SerializeField]
    private EntityJump entityJump = null;
    [Tooltip("movement speed when we are wandering"), SerializeField]
    private Vector2 iaScream = new Vector2(0, 50);

    [Tooltip(""), SerializeField/*, ReadOnly*/]
    private State interactionState = State.WANDER;

    private FrequencyCoolDown timerScream = new FrequencyCoolDown();


    [SerializeField]
    public PlayerController playerController;

    private void Awake()
    {
        base.Init();
    }

    private void Start()
    {
        StartTimerScream();
        if (!playerController)
            playerController = PhilaeManager.Instance.playerControllerRef;
    }

    public void DoWandering()
    {
        //Debug.Log("Wander");
        if (GetMoveState() != MoveState.InAir)
        {
            IARabbitInput.SetRandomInput();
            IARabbitInput.SetRandomJump();
        }
    }

    public bool IsTooFarFromPlayer()
    {
        if (!playerController)
            return (true);
        float dist = Vector3.SqrMagnitude(rb.transform.position - playerController.rb.position);
        if (dist > distForGoBackToNormal * distForGoBackToNormal)
        {
            return (true);
        }
        return (false);
    }

    public bool IsCloseToPlayer()
    {
        if (!playerController)
            return (false);

        float dist = Vector3.SqrMagnitude(rb.transform.position - playerController.rb.position);
        if (dist < distForFlee * distForFlee)
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

    public void SetFlee()
    {
        //Debug.Log("close !!!");
        interactionState = State.FLEE_PLAYER;
    }

    public void DoFlee()
    {
        //Debug.Log("Do chase");
        IARabbitInput.SetDirectionPlayerOut();
        IARabbitInput.SetRandomJump();
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
        if (canForgetPlayer)
        {
            Debug.Log("LOSE PLAYER");
            interactionState = State.WANDER;
        }
    }

    protected override void OnGrounded()
    {
        entityJump.OnGrounded();
        baseGravity.OnGrounded();
        baseGravityAttractorSwitch.OnGrounded();

        //.Instance.PlaySound(SFX_grounded);
    }

    private void StartTimerScream()
    {
        timerScream.StartCoolDown(ExtRandom.GetRandomNumber(iaScream.x, iaScream.y));
    }

    private void Update()
    {
        if (timerScream.IsFinished())
        {
            StartTimerScream();
            //SoundManager.Instance.PlaySound(SFX_Scream);
        }
    }

    private void FixedUpdate()
    {
        base.ChangeState();
    }

    public void OnObjectSpawn()
    {
        rb.transform.position = transform.position;
    }

    public void OnDesactivePool()
    {

    }

    public override void Kill()
    {
        Destroy(gameObject);
    }

    public override void GetHit(int amount, Vector3 posAttacker)
    {

    }

    private void OnDrawGizmos()
    {
        if (!rb)
            return;

        ExtDrawGuizmos.DebugWireSphere(rb.position, Color.red, distForFlee);
        ExtDrawGuizmos.DebugWireSphere(rb.position, Color.green, distForGoBackToNormal);
    }
}
