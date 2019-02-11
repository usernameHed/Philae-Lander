using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Player Move forward locally")]
public class PlayerAirMove : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float speedAirMoveForward = 5f;
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float speedAirMoveSide = 5f;
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float timeBeforeCanAirMove = 0.6f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityController entityController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityAction entityAction;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityContactSwitch entityContactSwitch;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntitySlide entitySlide;

    protected FrequencyCoolDown coolDownJump = new FrequencyCoolDown();

    /// <summary>
    /// move with input
    /// </summary>
    /// <param name="direction"></param>
    public void MovePhysics(Vector3 direction)
    {
        UnityMovement.MoveByForcePushing_WithPhysics(rb, direction, entityAction.GetMagnitudeInput());
    }

    public void JustJumped()
    {
        coolDownJump.StartCoolDown(timeBeforeCanAirMove);
    }

    /// <summary>
    /// move in physics, according to input of player
    /// </summary>
    private void AirMovePlayer()
    {
        //if we are in front of a NoSide Object, Slide, else, go forward
        Vector3 dirMove = (entityContactSwitch.IsForwardForbiddenWall())
            ? entitySlide.GetStraffDirection()
            : entityController.GetFocusedForwardDirPlayer();

        dirMove = entityAction.GetRelativeDirection(speedAirMoveSide, speedAirMoveForward);

        MovePhysics(dirMove);
    }

    /// <summary>
    /// handle move physics
    /// </summary>
    private void FixedUpdate()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir
            && coolDownJump.IsReady())
        {
            AirMovePlayer();
        }
    }

    
}
