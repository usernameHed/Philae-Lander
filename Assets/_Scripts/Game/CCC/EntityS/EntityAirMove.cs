
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAirMove : MonoBehaviour
{
    [Tooltip("speed move forward"), SerializeField]
    private bool canDoAirMove = true;
    [Tooltip("speed move forward"), SerializeField]
    private float speedAirMoveForward = 5f;
    [Tooltip("speed move forward"), SerializeField]
    private float speedAirMoveSide = 5f;
    [Tooltip("speed move forward"), SerializeField]
    private float timeBeforeCanAirMove = 0.6f;
    [Tooltip("speed move forward"), SerializeField]
    private float dotForward = 0.70f;
    [Tooltip(""), SerializeField]
    public float ratioWhenGravityAirMove = 0.7f;
    [Tooltip(""), SerializeField]
    public float limitAirCalculationSide = 30f;
    [Tooltip(""), SerializeField]
    public float limitAirCalculationForward = 60f;
    [Tooltip(""), SerializeField]
    public float velocityMaxAirMove = 7f;
    [Tooltip(""), SerializeField]
    public float speedDecreaseAddition = 3f;

    [Tooltip(""), SerializeField]
    public float dotInverse = -0.9f;
    [Tooltip(""), SerializeField]
    public float inverseRatioAcceleration = 1f;
    [Range(0, 1), Tooltip(""), SerializeField]
    public float minRatioTurn = 0.3f;

    [SerializeField, Tooltip("ref")]
    private Rigidbody rb = null;
    [SerializeField, Tooltip("ref")]
    private EntityController entityController = null;
    [SerializeField, Tooltip("ref")]
    private EntityAction entityAction = null;
    [SerializeField, Tooltip("ref")]
    private EntityJump entityJump = null;
    [SerializeField, Tooltip("ref")]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch = null;
    [SerializeField, Tooltip("ref")]
    private GroundForwardCheck groundForwardCheck = null;
    [SerializeField, Tooltip("ref")]
    private EntityMove entityMove = null;
    [SerializeField, Tooltip("ref")]
    private EntityRotate entityRotate = null;

    protected float amountAdded = 0f;

    protected FrequencyCoolDown coolDownJump = new FrequencyCoolDown();

    /// <summary>
    /// move with input
    /// </summary>
    /// <param name="direction"></param>
    public void MovePhysics(Vector3 direction, bool addAmount = true)
    {
        if (addAmount)
        {
            float toAdd = (direction * entityMove.GetMagnitudeAcceleration()).sqrMagnitude * entityMove.GetCurrentSpeedForwardClamped01() * Time.deltaTime;
            //Debug.Log("toAdd: " + toAdd);
            amountAdded += toAdd;
        }

        Debug.DrawRay(rb.position, direction, Color.green, 5f);
        //UnityMovement.MoveByForcePushing_WithPhysics(rb, direction, entityAction.GetMagnitudeInput());
        UnityMovement.MoveByForcePushing_WithPhysics(rb, direction, entityMove.GetMagnitudeAcceleration());
    }

    public void ResetAirMove()
    {
        amountAdded = 0f;
    }

    public void JustJumped()
    {
        coolDownJump.StartCoolDown(timeBeforeCanAirMove);
        ResetAirMove();
    }

    public void OnGrounded()
    {
        ResetAirMove();
    }

    private float GetRatioAirMove()
    {
        return (entityGravityAttractorSwitch.GetAirRatioGravity());
    }

    /// <summary>
    /// move in physics, according to input of player
    /// </summary>
    private void AirMovePlayer()
    {
        Vector3 dirMove = entityAction.GetRelativeDirection(speedAirMoveSide, speedAirMoveForward);

        float lastVelocity = entityJump.GetLastJumpForwardVelocity();
        //float dotDirForward = ExtQuaternion.DotProduct(dirMove.normalized, entityController.GetFocusedForwardDirPlayer());
        float dotDirForwardJump = ExtQuaternion.DotProduct(dirMove.normalized, entityJump.GetLastJumpForwardDirection());
        float dotDirAcceleration = ExtQuaternion.DotProduct(dirMove.normalized, entityController.GetActualDirForward().normalized);

        //if forward, limit speed (if lastVelocity == 1, we shouln't move forward
        if (dotDirForwardJump > dotForward)
        {
            if (amountAdded > limitAirCalculationForward * entityGravityAttractorSwitch.GetAirRatioGravity()
                && entityController.GetActualVelocity() > velocityMaxAirMove)
                return;
            //ne pas avancer si c'est un forbidden wall
            if (groundForwardCheck.IsForwardForbiddenWall())
                return;


            float valueSubstract = Mathf.Abs(lastVelocity - dotDirForwardJump);
            //Debug.Log("value Substract: " + valueSubstract);
            dirMove = dirMove * valueSubstract;
        }
        else
        {
            //if we have done enought airMove in air, don't do more
            if (amountAdded > limitAirCalculationSide * entityGravityAttractorSwitch.GetAirRatioGravity()
                && entityController.GetActualVelocity() > velocityMaxAirMove)
            {
                Debug.Log("stop airMove");
                return;
            }
                
            
            //dotInverse = -0.8, dot when input inverse: -0.95
            if (dotDirAcceleration < dotInverse)
            {
                float ratioAccel = (1 + (1 - dotDirAcceleration)) * inverseRatioAcceleration;
                Debug.Log("inverse ratio: " + ratioAccel);
                dirMove = dirMove * ratioAccel;

                MovePhysics(dirMove * GetRatioAirMove(), false);
                //entityRotate.DoAirRotate(Mathf.Max(minRatioTurn, 1 - lastVelocity));
                return;
            }
        }
        //Debug.DrawRay(rb.position, dirMove, Color.yellow, 5f);

        MovePhysics(dirMove * GetRatioAirMove());
        entityRotate.DoAirRotate(Mathf.Max(minRatioTurn, 1 - lastVelocity));
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

        return (true);
    }

    private void RemoveAdded()
    {
        float amountInput = (1 - entityMove.GetMagnitudeAcceleration());
        if (entityController.GetActualVelocity() < velocityMaxAirMove)
            amountInput = 1f;
        //entityController.GetActualVelocity() > velocityMaxAirMove
        amountAdded -= (speedDecreaseAddition * Time.deltaTime) * amountInput;
        if (amountAdded < 0)
            amountAdded = 0f;
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
        RemoveAdded();
    }
}
