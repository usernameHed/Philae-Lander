
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

public class IAFollowerController : EntityController, IPooledObject, IKillable
{
    [Tooltip("movement speed when we are wandering"), SerializeField]
    private readonly bool canLosePlayer = false;
    [Tooltip("movement speed when we are wandering"), SerializeField]
    private readonly float distForChase = 5f;
    [Tooltip("movement speed when we are wandering"), SerializeField]
    private readonly float distForLosePlayer = 10f;

    [Tooltip("ref script")]
    public IAFollowerInput followerInput;

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
    


    //[Tooltip("movement speed when we are wandering"), SerializeField]
    //public Rigidbody rigidBodyRef = null;
    //[Tooltip("movement speed when we are wandering"), SerializeField]
    //public Transform rigidbodyRef = null;
    [Tooltip("movement speed when we are wandering"), SerializeField]
    private IAFollowerJump iAJump = null;
    [Tooltip("movement speed when we are wandering"), SerializeField]
    private Vector2 iaScream = new Vector2(0, 50);

    [Tooltip(""), SerializeField, ReadOnly]
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
        followerInput.SetRandomInput();
        followerInput.SetRandomJump();
    }

    public bool IsTooFarFromPlayer()
    {
        if (!playerController)
            return (true);

        float dist = Vector3.SqrMagnitude(rb.transform.position - playerController.rb.position);
        if (dist > distForLosePlayer * distForLosePlayer)
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
        if (dist < distForChase * distForChase)
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
        followerInput.SetDirectionPlayer();
        followerInput.SetRandomJump();
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

    protected override void OnGrounded()
    {
        Debug.Log("grounded !");

        iAJump.OnGrounded();
        baseGravity.OnGrounded();
        baseGravityAttractorSwitch.OnGrounded();

        //SoundManager.Instance.PlaySound(SFX_grounded);
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
        ExtDrawGuizmos.DebugWireSphere(rb.position, Color.red, distForChase);
        ExtDrawGuizmos.DebugWireSphere(rb.position, Color.green, distForLosePlayer);
    }
}
