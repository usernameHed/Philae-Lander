using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityJump : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected float jumpHeight = 3f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("increase the height jump when we move faster")]
    protected float ratioIncreaseHeightMove = 0.5f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("")]
    protected bool inputJump = true;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected float ratioBoostWhenJumpingAgainstWall = 1.3f;

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] noJumpLayer = new string[] { "Walkable/FastForward", "Walkable/Dont" };

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected bool stayHold = false;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected bool canJumpInAir = true;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected bool doGravityAttractorJump = true;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityController entityController;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityAction entityAction;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityRotate entityRotate;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected BaseGravity baseGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected GroundForwardCheck groundForwardCheck;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public GroundCheck groundCheck;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityAirMove playerAirMove;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityGravityAttractorSwitch entityGravityAttractorSwitch;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityBumpUp entityBumpUp;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityYoshiBoost entityYoshiBoost;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public FastForward fastForward;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityMove entityMove;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected bool hasJumped = false;
    public bool HasJumped() => hasJumped;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected float justJumpedTimer = 0.1f;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected float justGroundTimer = 0.1f;

    protected FrequencyCoolDown coolDownWhenJumped = new FrequencyCoolDown();
    protected FrequencyCoolDown coolDownOnGround = new FrequencyCoolDown();
    private float lastVelocityJump = 0f;
    private Vector3 lastDirForwardJump = Vector3.zero;

    protected bool jumpStop = false;
    public bool IsJumpStoped() => jumpStop;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        jumpStop = false;
        hasJumped = false;
        lastVelocityJump = 0;
        lastDirForwardJump = Vector3.zero;
    }

    public float GetLastJumpForwardVelocity()
    {
        return (lastVelocityJump);
    }

    public Vector3 GetLastJumpForwardDirection()
    {
        return (lastDirForwardJump);
    }

    protected bool CanJump()
    {
        //can't jump in air
        if (!canJumpInAir && entityController.GetMoveState() == EntityController.MoveState.InAir)
            return (false);

        if (hasJumped)
            return (false);

        //faux si on hold pas et quand a pas laché
        if (jumpStop)
            return (false);

        //don't jump if we just grounded
        if (!coolDownOnGround.IsReady())
            return (false);

        //if (!entityContactSwitch.IsCoolDownSwitchReady())
        //    return (false);

        int isForbidden = ExtList.ContainSubStringInArray(noJumpLayer, groundCheck.GetLastLayer());
        if (isForbidden != -1)
            return (false);

        if (fastForward && !fastForward.CanJump())
            return (false);

        return (true);
    }

    public bool IsJumpCoolDebugDownReady()
    {
        return (coolDownWhenJumped.IsReady());
    }

    public bool IsJumpedAndNotReady()
    {
        return (hasJumped && !IsJumpCoolDebugDownReady());
    }

    public virtual void OnGrounded()
    {
        //Debug.Log("Grounded !");
        lastVelocityJump = 0f;
        lastDirForwardJump = Vector3.zero;

        coolDownWhenJumped.Reset();
        //here, we just were falling, without jumping
        if (!hasJumped)
        {
            coolDownOnGround.StartCoolDown(justGroundTimer);
        }
        //here, we just on grounded after a jump
        else
        {
            //rb.ClearVelocity();
            coolDownOnGround.StartCoolDown(justGroundTimer);
            hasJumped = false;
        }
    }

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.

        //reduce height when max speed
        float jumpBoostHeight = jumpHeight / (1 + ((1 - ratioIncreaseHeightMove) * entityMove.GetMagnitudeAcceleration()));

        if (groundForwardCheck && groundForwardCheck.IsForwardForbiddenWall())
            jumpBoostHeight = jumpHeight;

        //Debug.Log("boost height: " + jumpBoostHeight);
        return Mathf.Sqrt(2 * jumpBoostHeight * baseGravity.Gravity);
    }

    private Vector3 AddJumpHeight(Vector3 normalizedDirJump, float boost = 1f)
    {
        float jumpSpeedCalculate = CalculateJumpVerticalSpeed() * boost;
        Vector3 jumpForce = normalizedDirJump * jumpSpeedCalculate;

        //Debug.DrawRay(rb.position, jumpForce, Color.red, 5f);
        return (jumpForce);
    }

    /// <summary>
    /// called by entityBumpUp
    /// </summary>
    public void ResetInitialJumpDir()
    {
        lastVelocityJump = 0f;
        lastDirForwardJump = Vector3.zero;
    }

    /// <summary>
    /// return the normalized jump dir()
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNormalizedJumpDir(float boostForward = 1)
    {
        bool isForbiddenForward = false;
        if (groundForwardCheck)
            isForbiddenForward = groundForwardCheck.IsForwardForbiddenWall();
        //bool isForbiddenForward = false;
        lastVelocityJump = (isForbiddenForward) ? 0 : entityMove.GetMagnitudeAcceleration();

        Debug.Log("velocityJump: " + lastVelocityJump);

        Vector3 normalizedNormalGravity = baseGravity.GetMainAndOnlyGravity();
        //Vector3 normalizedNormalGravity = groundCheck.GetDirLastNormal();

        entityGravityAttractorSwitch.SetLastDirJump(normalizedNormalGravity);

        Vector3 normalizedForwardPlayer = Vector3.zero;
        if (!isForbiddenForward)
        {
            if (inputJump)
                normalizedForwardPlayer = entityRotate.GetLastDesiredDirection().normalized * lastVelocityJump;
            else
                normalizedForwardPlayer = entityController.GetFocusedForwardDirPlayer(normalizedNormalGravity) * lastVelocityJump;
        }
        else
        {
            normalizedNormalGravity *= ratioBoostWhenJumpingAgainstWall;
        }

        lastDirForwardJump = normalizedForwardPlayer;

        Debug.DrawRay(rb.position, normalizedNormalGravity, Color.yellow, 5f);
        Debug.DrawRay(rb.position, normalizedForwardPlayer * boostForward, Color.green, 5f);
        //Debug.Break();

        return (normalizedNormalGravity + normalizedForwardPlayer * boostForward);
    }

    /// <summary>
    /// do a jump
    /// </summary>
    protected void DoJump(float boostHeight, float boostForward = 1f)
    {
        Vector3 dirJump = GetNormalizedJumpDir(boostForward);
        Vector3 orientedStrenghtJump = AddJumpHeight(dirJump, boostHeight);

        rb.velocity = orientedStrenghtJump;

        baseGravity.JustJumped();
        if (playerAirMove)
            playerAirMove.JustJumped();
        entityGravityAttractorSwitch.JustJumped();
        if (entityBumpUp)
            entityBumpUp.JustJumped();
        if (entityYoshiBoost)
            entityYoshiBoost.JustJumped();
        if (fastForward)
            fastForward.JustJumped();
        //JustJump();
        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.Jump, rb.transform.position, rb.transform.rotation, ObjectsPooler.Instance.transform);
        
    }
}
