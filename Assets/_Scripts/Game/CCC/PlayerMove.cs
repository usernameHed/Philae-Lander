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
    private EntityController entityController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityAction entityAction;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityJump entityJump;

    /// <summary>
    /// move with input
    /// </summary>
    /// <param name="direction"></param>
    public void MovePhysics(Vector3 direction)
    {
        UnityMovement.MoveByForcePushing_WithPhysics(rb, direction, speedMove * entityAction.GetMagnitudeInput());
    }

    /// <summary>
    /// move in physics, according to input of player
    /// </summary>
    private void MovePlayer()
    {
        //MovePhysics(objectRotateLocal.forward);
        MovePhysics(entityController.GetFocusedForwardDirPlayer());
    }

    /// <summary>
    /// handle move physics
    /// </summary>
    private void FixedUpdate()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.Move
            && entityJump.IsJumpCoolDebugDownReady())
        {
            MovePlayer();
        }
    }

    
}
