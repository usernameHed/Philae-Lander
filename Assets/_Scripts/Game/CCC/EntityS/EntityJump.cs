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
    protected Transform playerLocalyRotate;
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
    }

    public float GetLastJumpForwardVelocity()
    {
        return (lastVelocityJump);
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

        if (!fastForward.CanJump())
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
        float jumpBoostHeight = jumpHeight / (1 + ((1 - ratioIncreaseHeightMove) * entityAction.GetMagnitudeInput()));

        if (groundForwardCheck.IsForwardForbiddenWall())
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
    }

    /// <summary>
    /// return the normalized jump dir()
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNormalizedJumpDir()
    {
        bool isForbiddenForward = groundForwardCheck.IsForwardForbiddenWall();
        //bool isForbiddenForward = false;
        lastVelocityJump = (isForbiddenForward) ? 0 : entityAction.GetMagnitudeInput();

        Vector3 normalizedNormalGravity = baseGravity.GetMainAndOnlyGravity();
        //Vector3 normalizedNormalGravity = groundCheck.GetDirLastNormal();

        entityGravityAttractorSwitch.SetLastDirJump(normalizedNormalGravity);

        Vector3 normalizedForwardPlayer = (!isForbiddenForward)
            ? entityController.GetFocusedForwardDirPlayer(normalizedNormalGravity) * lastVelocityJump
            //? entityRotate.GetLastDesiredDirection().normalized * lastVelocityJump
            : Vector3.zero;


        Debug.DrawRay(rb.position, normalizedNormalGravity, Color.yellow, 5f);
        Debug.DrawRay(rb.position, normalizedForwardPlayer, Color.green, 5f);
        //Debug.Break();

        return (normalizedNormalGravity + normalizedForwardPlayer);
    }

    /// <summary>
    /// do a jump
    /// </summary>
    protected void DoJump(float boostHeight)
    {
        Vector3 dirJump = GetNormalizedJumpDir();
        Vector3 orientedStrenghtJump = AddJumpHeight(dirJump, boostHeight);

        rb.velocity = orientedStrenghtJump;

        baseGravity.JustJumped();
        if (playerAirMove)
            playerAirMove.JustJumped();
        entityGravityAttractorSwitch.JustJumped();
        entityBumpUp.JustJumped();
        if (entityYoshiBoost)
            entityYoshiBoost.JustJumped();
        if (fastForward)
            fastForward.JustJumped();
        //JustJump();
        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.Jump, rb.transform.position, rb.transform.rotation, ObjectsPooler.Instance.transform);
        
    }
}
