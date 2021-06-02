
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.CameraMarioGalaxy;

public class PlayerController : EntityController, IKillable
{
    [Tooltip("ref script")]
    public PlayerInput playerInput = null;
    [Tooltip("ref script")]
    public PlayerJump playerJump = null;
    [Tooltip("ref script")]
    public Transform renderPlayer = null;
    [Tooltip("ref script")]
    public Animator animator = null;
    [SerializeField, Tooltip("ref script")]
    public EntityYoshiBoost entityYoshiBoost;
    [SerializeField, Tooltip("ref script")]
    public GroundAdvancedCheck groundAdvancedCheck;



    [SerializeField, Tooltip("id player for input")]
    public int idPlayer = 0;

    
    private bool isMoving = false;


    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
    }

    private void Awake()
    {
        base.Init();
        PhilaeManager.Instance.InitPlayer(this);
        groundAdvancedCheck.GetObjProjector().SetActive(true);
    }

    

    /// <summary>
    /// called when the game is over: desactive player
    /// </summary>
    public void GameOver()
    {

    }

    protected override void OnGrounded()
    {
        playerJump.OnGrounded();
        baseGravity.OnGrounded();
        baseGravityAttractorSwitch.OnGrounded();
        entityNoGravity.OnGrounded();
        entityBumpUp.OnGrounded();
        entityAirMove.OnGrounded();
        entityYoshiBoost.OnGrounded();
        fastForward.OnGrounded();

        //SoundManager.Instance.PlaySound(SFX_grounded);
    }

    /// <summary>
    /// set state of player
    /// </summary>
    private new void ChangeState()
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


        if (playerInput.IsMoving())
        {
            
            moveState = MoveState.Move;
            if (!isMoving)
            {
                //SoundManager.Instance.PlaySound(SFX_playerMove);
                animator.SetBool("isMarche", true);
            }

            isMoving = true;
        }
        else
        {
            moveState = MoveState.Idle;
            if (isMoving)
            {
                //SoundManager.Instance.PlaySound(SFX_playerMove, false);
                //SoundManager.Instance.PlaySound(SFX_playerEndMove);
                animator.SetBool("isMarche", false);
            }

            isMoving = false;
        }
    }

    private void FixedUpdate()
    {
        ChangeState();
        ExtMaths.LinearAcceleration(out actualAccelerationVector, rb.position, 4);
        actualAcceleration = actualAccelerationVector.magnitude;
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, GameOver);
    }

    public override void Kill()
    {
        if (isKilled)
            return;

        rb.isKinematic = true;

        groundAdvancedCheck.GetObjProjector().SetActive(false);

        //SoundManager.Instance.PlaySound(SFX_playerMove, false);

        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.Hit, rb.transform.position, rb.transform.rotation, ObjectsPooler.Instance.transform);
        EventManager.TriggerEvent(GameData.Event.GameOver);
        renderPlayer.gameObject.SetActive(false);
        //SoundManager.Instance.PlaySound(SFX_playerDeath);

        isKilled = true;
    }

    public override void GetHit(int amount, Vector3 posAttacker)
    {
        //throw new System.NotImplementedException();
    }
}
