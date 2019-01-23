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
    [FoldoutGroup("Gravity"), Tooltip("Add gravity when releasing jump button, and rigidbody is going UPward the planet"), SerializeField]
    private float rbUpAddGravity = 2.5f;
    [FoldoutGroup("Gravity"), Tooltip("Down gravity when we are falling into the planet"), SerializeField]
    private float rbDownAddGravity = 2.5f;
    [FoldoutGroup("Gravity"), Tooltip("Down gravity when we are falling into the planet"), SerializeField]
    private float defaultGravityInAir = 1f;

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

        Vector3 currentVeocity = rb.velocity;
        //rb.velocity = new Vector3(currentVeocity.x * jumpForce.x,
        //    currentVeocity.y * jumpForce.y,
        //    currentVeocity.z * jumpForce.z);
        rb.velocity += jumpForce;

        //rb.AddForce(jumpForce);
        //rb.velocity = jumpForce;
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
            gravityApplyer.SetUseGravity(false);

            if (!stayHold)
                jumpStop = true;

            externalForces.JustJumped();
            hasJumped = true;
        }
    }

    /// <summary>
    /// when we jump, and just release...  take us down !
    /// </summary>
    private void ApplyDownForceWhenReleasing()
    {
        if (playerController.GetMoveState() != PlayerController.MoveState.InAir)
            return;

        Vector3 gravityOrientation = gravityApplyer.GetDirGravity();
        float dotGravityRigidbody = ExtQuaternion.DotProduct(gravityOrientation, rb.velocity);
        //here we fall down toward a planet, apply gravity down
        if (dotGravityRigidbody < 0)
        {
            Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
            Debug.DrawRay(rb.transform.position, orientationDown, Color.blue, 5f);
            rb.velocity += orientationDown;
            Debug.Log("going down");
            //Debug.Break();
        }
        //here we are going up, and we release the jump button, apply gravity down until the highest point
        else if (dotGravityRigidbody > 0 && !playerInput.Jump)
        {
            Vector3 orientationUp = -gravityOrientation * gravity * (rbUpAddGravity - 1) * Time.fixedDeltaTime;
            Debug.DrawRay(rb.transform.position, orientationUp, Color.yellow, 5f);
            rb.velocity += orientationUp;
            Debug.Log("going up");
        }
        //here, apply base gravity when we are InAir
        Vector3 forceBaseGravityInAir = -gravityOrientation * gravity * (defaultGravityInAir - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(rb.transform.position, forceBaseGravityInAir, Color.green, 5f);
        rb.velocity += forceBaseGravityInAir;

        /*//here we are going upward in the sky, but we need to slow down because we release the jump button
        if (rb.velocity.y > 0 && !playerInput.Jump)
        {
            Debug.Log("going up");
            rb.velocity += -CalculateDirectionJump() * gravity * (rbUpAddGravity - 1) * Time.fixedDeltaTime;
        }*/
    }

    /// <summary>
    /// called when grounded (after a jump, or a fall !)
    /// </summary>
    public void OnGrounded()
    {
        PlayerConnected.Instance.SetVibrationPlayer(playerController.idPlayer, onGrounded);
        Debug.Log("Grounded !");
        gravityApplyer.SetUseGravity(true);
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
        ApplyDownForceWhenReleasing();
    }
}
