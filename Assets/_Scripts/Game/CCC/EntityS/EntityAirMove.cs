using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Player Move forward locally")]
public class EntityAirMove : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private bool canDoAirMove = true;
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float speedAirMoveForward = 5f;
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float speedAirMoveSide = 5f;
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float timeBeforeCanAirMove = 0.6f;
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float dotForward = 0.70f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float ratioWhenGravityAirMove = 0.7f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float limitAirMoveCalculation = 1500f;

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
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityJumpCalculation entityJumpCalculation;

    protected FrequencyCoolDown coolDownJump = new FrequencyCoolDown();
    protected float amountAdded = 0f;

    /// <summary>
    /// move with input
    /// </summary>
    /// <param name="direction"></param>
    public void MovePhysics(Vector3 direction)
    {
        amountAdded += (direction * entityAction.GetMagnitudeInput()).sqrMagnitude;
        Debug.Log("Amount added: " + amountAdded);

        UnityMovement.MoveByForcePushing_WithPhysics(rb, direction, entityAction.GetMagnitudeInput());
    }

    public void JustJumped()
    {
        coolDownJump.StartCoolDown(timeBeforeCanAirMove);
        amountAdded = 0f;
    }

    private float GetRatioAirMove()
    {
        if (entityGravityAttractorSwitch.IsInGravityAttractorMode())
        {
            //here more ratio if more gravity !
            float ratioAirMove = entityGravityAttractorSwitch.GetRatioGravity();
            //here reduce this ratio
            return (ratioAirMove * ratioWhenGravityAirMove);
        }
        return (1);
    }

    /// <summary>
    /// move in physics, according to input of player
    /// </summary>
    private void AirMovePlayer()
    {
        Vector3 dirMove = entityAction.GetRelativeDirection(speedAirMoveSide, speedAirMoveForward);

        float lastVelocity = entityJump.GetLastJumpForwardVelocity();
        float dotDirForward = ExtQuaternion.DotProduct(dirMove.normalized, entityController.GetFocusedForwardDirPlayer());

        //if forward, limit speed (if lastVelocity == 1, we shouln't move forward
        if (dotDirForward > dotForward)
        {
            float valueSubstract = Mathf.Abs(lastVelocity - dotDirForward);
            //Debug.Log("value Substract: " + valueSubstract);
            dirMove = dirMove * valueSubstract;
        }
        Debug.DrawRay(rb.position, dirMove, Color.yellow, 5f);

        MovePhysics(dirMove * GetRatioAirMove());
    }

    private bool CanDoAirMove()
    {
        //can we do air move on this object ?
        if (!canDoAirMove)
            return (false);
        //only airMove in... air !
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return (false);
        //start airMove after a little bit of time
        if (!coolDownJump.IsReady())
            return (false);
        //No air Move en Side Jump or other stuff
        if (!entityJumpCalculation.CanDoAirMove())
            return (false);
        //if we have done enought airMove in air, don't do more
        if (amountAdded > limitAirMoveCalculation)
            return (false);

        return (true);
    }

    /// <summary>
    /// handle move physics
    /// </summary>
    private void FixedUpdate()
    {
        if (CanDoAirMove())
        {
            AirMovePlayer();
        }
    }

    
}
