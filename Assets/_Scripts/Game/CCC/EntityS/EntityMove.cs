using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Player Move forward locally")]
public class EntityMove : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float speedMove = 5f;

    [FoldoutGroup("GamePlay"), Range(0f, 1f), Tooltip("when we have a little input, set it to this value"), SerializeField]
    private float minInput = 0.15f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private Rigidbody rb = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityController entityController = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityAction entityAction = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityJump entityJump = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private GroundForwardCheck groundForwardCheck = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntitySlide entitySlide = null;
    /// <summary>
    /// move with input
    /// </summary>
    /// <param name="direction"></param>
    public void MovePhysics(Vector3 direction)
    {
        UnityMovement.MoveByForcePushing_WithPhysics(rb, direction, speedMove * entityAction.GetMagnitudeInput(minInput, 1f));
    }

    /// <summary>
    /// move in physics, according to input of player
    /// </summary>
    private void MovePlayer()
    {
        //if we are in front of a NoSide Object, Slide, else, go forward
        Vector3 dirMove = (groundForwardCheck.IsForwardForbiddenWall())
            ? entitySlide.GetStraffDirection()
            : entityController.GetFocusedForwardDirPlayer();

        //Vector3 dirMove = entityController.GetFocusedForwardDirPlayer();

        MovePhysics(dirMove);
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
