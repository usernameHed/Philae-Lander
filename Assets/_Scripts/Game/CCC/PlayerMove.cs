using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Player Move forward locally")]
public class PlayerMove : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float speedMove = 5f;
    
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Transform objectRotateLocal;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private PlayerController playerController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private PlayerInput playerInput;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private PlayerJump playerJump;


    private bool enabledScript = true;      //tell if this script should be active or not
    
    private void Start()
    {
        Init();
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
        UnityMovement.MoveByForcePushing_WithPhysics(rb, direction, speedMove * playerInput.GetMagnitudeInput());
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
        if (playerController.GetMoveState() == PlayerController.MoveState.Move
            && playerJump.IsJumpCoolDebugDownReady())
        {
            MovePlayer();
        }
    }

    
}
