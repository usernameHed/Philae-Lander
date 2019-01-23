using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float gravity = 9.81f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref script")]
    private float jumpHeight = 8f;
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
    private GravityApplyer gravityApplyer;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private ExternalForces externalForces;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    private bool hasJumped = false;

    private bool jumpStop = false;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        jumpStop = false;
        hasJumped = false;
    }

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    private void Jump(Vector3 dir, float boost = 1)
    {
        Vector3 jumpForce = dir * CalculateJumpVerticalSpeed() * boost;

        PlayerConnected.Instance.SetVibrationPlayer(playerController.idPlayer, onJump);

        //rb.AddForce(jumpForce);
        rb.velocity = jumpForce;
    }

    private Vector3 CalculateDirectionJump()
    {
        return (gravityApplyer.GetDirGravity());
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
        if (playerController.GetMoveState() == PlayerController.MoveState.InAir
            || hasJumped)
        {
            return;
        }

        if (playerInput.Jump && CanJump())
        {
            Vector3 jumpDirection = CalculateDirectionJump();
            Jump(jumpDirection);

            if (!stayHold)
                jumpStop = true;

            externalForces.JustJumped();
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
}
