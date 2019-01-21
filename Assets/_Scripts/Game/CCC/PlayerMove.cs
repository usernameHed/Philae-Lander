using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Player Move and rotation")]
public class PlayerMove : MonoBehaviour
{
    public enum MoveState
    {
        Idle,
        InAir,
        Move,
    }

    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float speedMove = 5f;
    
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Transform objectRotateLocal;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private PlayerInput playerInput;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private GroundCheck groundCheck;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("state move"), ReadOnly]
    public MoveState moveState = MoveState.Idle;

    private bool enabledScript = true;      //tell if this script should be active or not
    
    private void Start()
    {
        Init();
    }

    public MoveState GetMoveState()
    {
        return moveState;
    }

    /// <summary>
    /// init player
    /// </summary>
    public void Init()
    {
        enabledScript = true;               //active this script at start
    }

    /// <summary>
    /// move with input
    /// </summary>
    /// <param name="direction"></param>
    public void MovePhysics(Vector3 direction)
    {
        UnityMovement.MoveByForcePushing_WithPhysics(rb, direction, speedMove);
    }

    /// <summary>
    /// move in physics, according to input of player
    /// </summary>
    private void MovePlayer()
    {
        MovePhysics(objectRotateLocal.forward);
    }

    /// <summary>
    /// handle move physics
    /// </summary>
    private void FixedUpdate()
    {
        if (!enabledScript)
            return;

        if (!groundCheck.IsSafeGrounded())
        {
            moveState = MoveState.InAir;
            return;
        }

        //apply force if input
        if (!playerInput.NotMoving())
        {
            moveState = MoveState.Move;
            MovePlayer();
        }            
        else
        {
            moveState = MoveState.Idle;
        }
    }

    
}
