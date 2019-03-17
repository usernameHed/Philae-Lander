using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGravity : BaseGravity
{
    public override void OnGrounded()
    {
        isGoingDown = isGoingDownToGround = false;
    }

    public override void JustJumped()
    {
        isGoingDown = isGoingDownToGround = false;
    }

    /// <summary>
    /// return the right gravity
    /// </summary>
    /// <returns></returns>
    public Vector3 FindAirGravity(Vector3 positionObject, Vector3 rbVelocity, Vector3 gravityOrientation,
        bool applyForceUp, bool applyForceDown)
    {
        Vector3 finalGravity = rbVelocity;

        finalGravity += base.AirBaseGravity(gravityOrientation, positionObject, baseGravityAttractorSwitch.GetAirRatioGravity()) * entityNoGravity.GetNoGravityRatio();

        if (isGoingDown && applyForceDown)
        {
            finalGravity += base.AirAddGoingDown(gravityOrientation, positionObject) * entityNoGravity.GetNoGravityRatio() * baseGravityAttractorSwitch.GetRatioGravityDown();
        }

        return (finalGravity);
    }

    private bool CanApplyForceUp()
    {
        if (!entityController.isPlayer)
            return (false);

        if (entityNoGravity.WereWeInNoGravity())
            return (false);

        return (true);
    }

    private bool CanApplyForceDown()
    {
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

        base.ApplyGroundGravity(base.defaultGravityOnGround);
        ApplyAirGravity();
    }
}
