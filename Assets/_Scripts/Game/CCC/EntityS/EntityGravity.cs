using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGravity : BaseGravity
{
    //[FoldoutGroup("Ground Gravity"), Tooltip("Add gravity when releasing jump button, and rigidbody is going UPward the planet"), SerializeField]
    //private float groundAddGravity = 5.5f;
    [FoldoutGroup("Ground Gravity"), Tooltip("Down gravity when we are falling into the planet"), SerializeField]
    private float stickToFloorGravity = 6f;


    [FoldoutGroup("Air Gravity"), Tooltip("Add gravity when releasing jump button, and rigidbody is going UPward the planet"), SerializeField]
    private float rbUpAddGravity = 2f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAction entityAction = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityJump entityJump = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityYoshiBoost entityYoshiBoost = null;

    public override void OnGrounded()
    {
        isGoingDown = isGoingDownToGround = false;
    }

    public override void JustJumped()
    {
        isGoingDown = isGoingDownToGround = false;
    }

    private bool CanDoGroundGravity()
    {
        if (!entityJump.IsJumpCoolDebugDownReady())
            return (false);
        return (true);
    }

    /// <summary>
    /// apply a gravity force when Almost On ground
    /// It's happen when we have sudently a little gap, we have to
    /// stick to the floor as soon as possible !
    /// (exept when we just jumped !)
    /// </summary>
    private void ApplySuplementGroundGravity()
    {
        //here we are not almost grounded
        if (!groundCheck.IsAlmostGrounded())
            return;
        //here we just jumped ! don't add supplement force
        if (!entityJump.IsJumpCoolDebugDownReady())
            return;

        Vector3 gravityOrientation = GetMainAndOnlyGravity();
        //Debug.LogWarning("Apply gravity down down down !");

        Vector3 orientationDown = -gravityOrientation * gravity * (stickToFloorGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(rb.transform.position, orientationDown, Color.red, 5f);
        //Debug.Log("ici suplement gravity");
        rb.velocity += orientationDown;
    }

    /// <summary>
    /// here we are going up, and we release the jump button, apply gravity down until the highest point
    /// </summary>
    private Vector3 AirAddGoingUp(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        Vector3 orientationDown = -gravityOrientation * gravity * (rbUpAddGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, orientationDown, Color.yellow, 5f);
        return (orientationDown);
    }

    /// <summary>
    /// apply base air gravity
    /// </summary>
    public Vector3 AirBoostYoshiGravity(Vector3 gravityOrientation, Vector3 positionEntity, float boost = 1)
    {
        Vector3 forceBaseGravityInAir = gravityOrientation * gravity * (entityYoshiBoost.GetYoshiBoost() - 1) * boost * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, forceBaseGravityInAir, Color.green, 5f);
        return (forceBaseGravityInAir);
    }


    /// <summary>
    /// return the right gravity
    /// </summary>
    /// <returns></returns>
    public Vector3 FindAirGravity(Vector3 positionObject, Vector3 rbVelocity, Vector3 gravityOrientation,
        bool applyForceUp, bool applyForceDown)
    {
        Vector3 finalGravity = rbVelocity;
        float noGravityRatio = (entityNoGravity ? entityNoGravity.GetNoGravityRatio() : 1f);


        finalGravity += AirBaseGravity(gravityOrientation, positionObject, baseGravityAttractorSwitch.GetAirRatioGravity()) * noGravityRatio;

        
        if (isGoingDown && applyForceDown)
        {
            finalGravity += base.AirAddGoingDown(gravityOrientation, positionObject) * noGravityRatio * baseGravityAttractorSwitch.GetRatioGravityDown();
        }

        

        //here we are going up, and we release the jump button, apply gravity down until the highest point
        else if (!isGoingDown && !entityAction.Jump)
        {
            isGoingDown = false;
            if (applyForceUp)
                finalGravity += AirAddGoingUp(gravityOrientation, positionObject) * noGravityRatio * baseGravityAttractorSwitch.GetAirRatioGravity();
        }
        //here we are going up, continiue pressing the jump button, AND in gravityAttractor
        else if (!isGoingDown && entityAction.Jump
            && entityGravityAttractorSwitch.CanApplyForceDown())
        {
            isGoingDown = false;
            if (applyForceUp)
                finalGravity += AirAddGoingUp(gravityOrientation, positionObject) * noGravityRatio * (baseGravityAttractorSwitch.GetAirRatioGravity() / 2);
        }

        if (entityYoshiBoost && entityYoshiBoost.AreWeBoosting())
            finalGravity += AirBoostYoshiGravity(gravityOrientation, positionObject) * baseGravityAttractorSwitch.GetAirRatioGravity();
        
        //Debug.Log(finalGravity);
        
        return (finalGravity);
    }

    private bool CanApplyForceUp()
    {
        if (!entityController.isPlayer)
            return (false);

        if (entityNoGravity && entityNoGravity.WereWeInNoGravity())
            return (false);

        if (entityYoshiBoost && entityYoshiBoost.AreWeBoosting())
            return (false);

        return (true);
    }

    private bool CanApplyForceDown()
    {
        if (entityYoshiBoost && entityYoshiBoost.AreWeBoosting())
            return (false);

        return (true);
    }

    /// <summary>
    /// apply every gravity force in Air
    /// </summary>
    private void ApplyAirGravity()
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return;

        base.SetGoingDown();

        
        //if (currentOrientation != OrientationPhysics.ATTRACTOR)
        rb.velocity = FindAirGravity(rb.transform.position, rb.velocity,
            GetMainAndOnlyGravity(),
            CanApplyForceUp(),
            CanApplyForceDown());
    }

    private void FixedUpdate()
    {
        base.CalculateGravity(rb.transform.position);

        if (CanDoGroundGravity())
            base.ApplyGroundGravity(base.defaultGravityOnGround);
        ApplySuplementGroundGravity();
        ApplyAirGravity();
    }
}
