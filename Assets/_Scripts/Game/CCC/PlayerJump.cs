using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    private float jumpHeight = 1f;

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    private bool stayHold = false;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    private bool canJumpInAir = true;

    /*[FoldoutGroup("Slow"), SerializeField, Tooltip("ref script")]
    private float jumpHeightWhenSlow = 2f;
    [FoldoutGroup("Slow"), SerializeField, Tooltip("ref script")]
    private float speedSlow = 0.4f;
    */

    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onJump;
    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on se pose"), SerializeField]
    private Vibration onGrounded;

    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Transform playerLocalyRotate;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerInput playerInput;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerController playerController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerGravity playerGravity;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    private bool hasJumped = false;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    private float justJumpedTimer = 0.1f;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    private float justGroundTimer = 0.1f;

    private FrequencyCoolDown coolDownWhenJumped = new FrequencyCoolDown();
    private FrequencyCoolDown coolDownOnGround = new FrequencyCoolDown();


    private bool jumpStop = false;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.OnGrounded, OnGrounded);
    }

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

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.

        //reduce height when max speed
        float jumpBoostHeight = jumpHeight / (1 + (1 * playerInput.GetMagnitudeInput()));
        Debug.Log("boost height: " + jumpBoostHeight);
        return Mathf.Sqrt(2 * jumpBoostHeight * playerGravity.Gravity);
    }

    private Vector3 AddJumpHeight(Vector3 normalizedDirJump, float boost = 1f)
    {
        float jumpSpeedCalculate = CalculateJumpVerticalSpeed() * boost;
        Vector3 jumpForce = normalizedDirJump * jumpSpeedCalculate;

        Debug.DrawRay(rb.position, jumpForce, Color.red, 5f);
        return (jumpForce);
    }

    /// <summary>
    /// return the normalized jump dir()
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNormalizedJumpDir()
    {
        Vector3 normalizedNormalGravity = playerGravity.GetMainAndOnlyGravity();
        Vector3 normalizedForwardPlayer = playerLocalyRotate.forward * playerInput.GetMagnitudeInput();

        Debug.DrawRay(rb.position, normalizedNormalGravity, Color.yellow, 5f);
        Debug.DrawRay(rb.position, normalizedForwardPlayer, Color.green, 5f);

        return (normalizedNormalGravity + normalizedForwardPlayer);
    }

    /// <summary>
    /// do a jump
    /// </summary>
    private void DoJump()
    {
        Vector3 dirJump = GetNormalizedJumpDir();
        Vector3 orientedStrenghtJump = AddJumpHeight(dirJump);

        rb.velocity = orientedStrenghtJump;
    }

    private bool CanJump()
    {
        //can't jump in air
        if (!canJumpInAir && playerController.GetMoveState() == PlayerController.MoveState.InAir)
            return (false);

        if (hasJumped)
            return (false);

        //faux si on hold pas et quand a pas laché
        if (jumpStop)
            return (false);

        //don't jump if we just grounded
        if (!coolDownOnGround.IsReady())
            return (false);

        return (true);
    }

    private void JumpManager()
    {
        if (IsJumpCoolDebugDownReady() && hasJumped &&
            playerController.GetMoveState() != PlayerController.MoveState.InAir)
        {
            hasJumped = false;
            Debug.LogError("Unjump... error ??");
            coolDownOnGround.Reset();
            return;
        }

        if (hasJumped)
        {
            return;
        }

        if (playerInput.Jump && CanJump())
        {
            coolDownWhenJumped.StartCoolDown(justJumpedTimer);
            playerController.ChangeState(PlayerController.MoveState.InAir);

            Debug.Log("jump !");
            
            rb.ClearVelocity();
            PlayerConnected.Instance.SetVibrationPlayer(playerController.idPlayer, onJump);
            playerGravity.CreateAttractor();
            DoJump();

            if (!stayHold)
                jumpStop = true;
            
            hasJumped = true;
            //Debug.Break();
        }
    }


    /// <summary>
    /// called when grounded (after a jump, or a fall !)
    /// </summary>
    public void OnGrounded()
    {
        PlayerConnected.Instance.SetVibrationPlayer(playerController.idPlayer, onGrounded);
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

    private void Update()
    {
        //on lache, on autorise le saut encore
        if (playerInput.JumpUp)
            jumpStop = false;
    }

    private void FixedUpdate()
    {
        JumpManager();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.OnGrounded, OnGrounded);
    }
}
