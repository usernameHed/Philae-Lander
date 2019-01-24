using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    private float jumpHeight = 5f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    private bool stayHold = false;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    private bool canJumpInAir = true;

    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on jump"), SerializeField]
    private Vibration onJump;
    [FoldoutGroup("GamePlay"), Tooltip("vibration quand on se pose"), SerializeField]
    private Vibration onGrounded;

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

    private FrequencyCoolDown coolDownWhenJumped = new FrequencyCoolDown();


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

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * playerGravity.Gravity);
    }

    private void Jump(Vector3 dir, float boost = 1)
    {
        Vector3 jumpForce = dir * CalculateJumpVerticalSpeed() * boost;

        PlayerConnected.Instance.SetVibrationPlayer(playerController.idPlayer, onJump);

        rb.velocity += jumpForce;
    }

    private Vector3 CalculateDirectionJump()
    {
        return (playerGravity.GetMainAndOnlyGravity());
    }

    private bool CanJump()
    {
        //can't jump in air)
        if (!canJumpInAir && playerController.GetMoveState() == PlayerController.MoveState.InAir)
            return (false);

        if (hasJumped)
            return (false);

        //faux si on hold pas et quand a pas laché
        if (jumpStop)
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
            return;
        }

        if (/*playerController.GetMoveState() == PlayerController.MoveState.InAir
            || */hasJumped)
        {
            return;
        }

        if (playerInput.Jump && CanJump())
        {
            Vector3 jumpDirection = CalculateDirectionJump();
            coolDownWhenJumped.StartCoolDown(justJumpedTimer);

            Jump(jumpDirection);

            if (!stayHold)
                jumpStop = true;
            
            hasJumped = true;
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

        }
        //here, we just on grounded after a jump
        else
        {
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
