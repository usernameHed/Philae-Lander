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

    [FoldoutGroup("Jump Gravity"), SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastForNormalSwitch = 5f;


    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected bool stayHold = false;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected bool canJumpInAir = true;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityController entityController;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected Transform playerLocalyRotate;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityAction entityAction;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected PlayerGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityAttractor entityAttractor;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityContactSwitch entityContactSwitch;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityJumpCalculation entityJumpCalculation;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntitySwitch entitySwitch;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public GroundCheck groundCheck;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public FastForward fastForward;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected bool hasJumped = false;
    public bool HasJumped { get { return (hasJumped); } }
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected float justJumpedTimer = 0.1f;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    protected float justGroundTimer = 0.1f;
    [FoldoutGroup("Debug"), Tooltip("gravité du saut"), SerializeField]
    private float magicTrajectoryCorrection = 1.4f;

    protected FrequencyCoolDown coolDownWhenJumped = new FrequencyCoolDown();
    protected FrequencyCoolDown coolDownOnGround = new FrequencyCoolDown();

    protected bool jumpStop = false;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        jumpStop = false;
        hasJumped = false;
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

        if (!entityContactSwitch.IsCoolDownSwitchReady())
            return (false);

        int isForbidden = ExtList.ContainSubStringInArray(noJumpLayer, groundCheck.GetLastLayer());
        if (isForbidden != -1)
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
        Debug.Log("Grounded !");
        
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
        entityJumpCalculation.OnGrounded();
    }

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.

        //reduce height when max speed
        float jumpBoostHeight = jumpHeight / (1 + ((1 - ratioIncreaseHeightMove) * entityAction.GetMagnitudeInput()));

        if (entityContactSwitch.IsForwardForbiddenWall())
            jumpBoostHeight = jumpHeight;

        //Debug.Log("boost height: " + jumpBoostHeight);
        return Mathf.Sqrt(2 * jumpBoostHeight * playerGravity.Gravity);
    }

    private Vector3 AddJumpHeight(Vector3 normalizedDirJump, float boost = 1f)
    {
        float jumpSpeedCalculate = CalculateJumpVerticalSpeed() * boost;
        Vector3 jumpForce = normalizedDirJump * jumpSpeedCalculate;

        //Debug.DrawRay(rb.position, jumpForce, Color.red, 5f);
        return (jumpForce);
    }

    /// <summary>
    /// return the normalized jump dir()
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNormalizedJumpDir()
    {
        Vector3 normalizedNormalGravity = playerGravity.GetMainAndOnlyGravity();
        //Vector3 normalizedForwardPlayer = playerLocalyRotate.forward * entityAction.GetMagnitudeInput();
        Vector3 normalizedForwardPlayer = entityController.GetFocusedForwardDirPlayer() * entityAction.GetMagnitudeInput();

        if (entityContactSwitch.IsForwardForbiddenWall())
            normalizedForwardPlayer = Vector3.zero;
        //Debug.DrawRay(rb.position, normalizedNormalGravity, Color.yellow, 5f);
        //Debug.DrawRay(rb.position, normalizedForwardPlayer, Color.green, 5f);

        return (normalizedNormalGravity + normalizedForwardPlayer);
    }

    /// <summary>
    /// do a jump
    /// </summary>
    protected void DoJump()
    {
        Vector3 dirJump = GetNormalizedJumpDir();
        Vector3 orientedStrenghtJump = AddJumpHeight(dirJump);

        rb.velocity = orientedStrenghtJump;

        playerGravity.JustJumped();
        entitySwitch.JustJumped();
        fastForward.JustJumped();
        //JustJump();
        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.Jump, rb.transform.position, rb.transform.rotation, ObjectsPooler.Instance.transform);
        
        entityJumpCalculation.JumpCalculation(orientedStrenghtJump);
    }
}
