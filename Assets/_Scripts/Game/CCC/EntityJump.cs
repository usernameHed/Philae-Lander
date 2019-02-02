﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityJump : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    protected float jumpHeight = 3f;
    [FoldoutGroup("GamePlay"), Range(0f, 0.5f), SerializeField, Tooltip("increase the height jump when we move faster")]
    protected float ratioIncreaseHeightMove = 0.5f;
    [FoldoutGroup("Jump Gravity"), SerializeField, Tooltip("MUST PRECEED AIR ATTRACTOR TIME !!")]
    private float timeFeforeCalculateAgainJump = 0.5f;
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
    protected GroundCheck groundCheck;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityJumpCalculation entityJumpCalculation;

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
    protected FrequencyCoolDown coolDowwnBeforeCalculateAgain = new FrequencyCoolDown();
    public bool IsReadyToTestCalculation() { return (coolDowwnBeforeCalculateAgain.IsStartedAndOver()); }
    private bool normalGravityTested = false;   //know if we are in the 0.5-0.8 sec between norma and attractor

    private InfoJump infoJump = new InfoJump();
    protected bool jumpStop = false;
    private RaycastHit hitInfo;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        jumpStop = false;
        hasJumped = false;
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
        normalGravityTested = false;
        coolDowwnBeforeCalculateAgain.Reset();
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
    }

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.

        //reduce height when max speed
        float jumpBoostHeight = jumpHeight / (1 + ((1 - ratioIncreaseHeightMove) * entityAction.GetMagnitudeInput()));

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
        Vector3 normalizedForwardPlayer = playerLocalyRotate.forward * entityAction.GetMagnitudeInput();

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
        //playerGravity.JustJump();
        //JustJump();

        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.Jump, rb.transform.position, rb.transform.rotation, ObjectsPooler.Instance.transform);

        entityJumpCalculation.JumpCalculation();
    }
}
