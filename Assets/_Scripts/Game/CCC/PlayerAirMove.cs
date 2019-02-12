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
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float dotForward = 0.70f;

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
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityJump entityJump;

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
        /*//if we are in front of a NoSide Object, Slide, else, go forward
        Vector3 dirMove = (entityContactSwitch.IsForwardForbiddenWall())
            ? entitySlide.GetStraffDirection()
            : entityController.GetFocusedForwardDirPlayer();
        */
        Vector3 dirMove = entityAction.GetRelativeDirection(speedAirMoveSide, speedAirMoveForward);

        /*Vector3 force = dirMove;
        // First find out what your modifier would be if the force
        // direction was in the direction of the current velocity
        float straightMultiplier = 1 - (rb.velocity.magnitude / maxSpeedAirMove);
        // This value will be 1 if the rigidbody is moving at 0,
        // and 0 if the rigidbody is moving at maxSpeed.
        // Then, find out what the dot product is between the force and the velocity
        float forceDot = Vector3.Dot(rb.velocity, force);
        // Now, smoothly interpolate between full power and modified power
        // depending on what direction the force is going!
        Vector3 modifiedForce = force * straightMultiplier;
        Vector3 correctForce = Vector3.Lerp(force, modifiedForce, forceDot);

        Debug.DrawRay(rb.position, dirMove, Color.red, 5f);
        Debug.DrawRay(rb.position, correctForce, Color.yellow, 5f);
        */
        float lastVelocity = entityJump.GetLastJumpForwardVelocity();
        float dotDirForward = ExtQuaternion.DotProduct(dirMove.normalized, entityController.GetFocusedForwardDirPlayer());
        //dotDirForward = ExtUtilityFunction.Remap(dotDirForward, )
        Debug.Log("dotDirForward: " + dotDirForward + "(initial jump forward: " + lastVelocity + ")");
        

        //if forward, limit speed (if lastVelocity == 1, we shouln't move forward
        if (dotDirForward > dotForward)
        {
            float valueSubstract = Mathf.Abs(lastVelocity - dotDirForward);
            Debug.Log("value Substract: " + valueSubstract);
            dirMove = dirMove * valueSubstract;
        }
        Debug.DrawRay(rb.position, dirMove, Color.yellow, 5f);

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
