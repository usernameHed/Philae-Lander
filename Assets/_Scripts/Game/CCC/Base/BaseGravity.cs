
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGravity : UniqueGravity
{
    [Tooltip("default air gravity"), SerializeField]
    protected float defaultGravityOnGround = 5.5f;

    [SerializeField, Tooltip("ref script")]
    protected EntityController entityController = null;
    [Tooltip("ref"), SerializeField]
    protected GroundCheck groundCheck = null;
    
    [SerializeField, Tooltip("ref script")]
    protected BaseGravityAttractorSwitch baseGravityAttractorSwitch = null;
    [SerializeField, Tooltip("ref script")]
    protected EntityNoGravity entityNoGravity = null;

    public override Vector3 CalculateGravity(Vector3 positionEntity)
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            mainAndOnlyGravity = baseGravityAttractorSwitch.GravityDirection;
        }
        else
        {
            mainAndOnlyGravity = groundCheck.GetDirLastNormal();
        }
        return (mainAndOnlyGravity);
    }

    public override Vector3 GetPointGravityDown()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            return (uniqueGravityAttractorSwitch.GetPosRange());
        }
        else
        {
            return (groundCheck.GetPointLastHit());
        }        
    }

    /// <summary>
    /// apply gravity on ground
    /// </summary>
    protected void ApplyGroundGravity(float groundGravity)
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            return;

        Vector3 gravityOrientation = GetMainAndOnlyGravity();

        //here, apply base gravity when we are InAir
        Vector3 forceBaseGravity = -gravityOrientation * gravity * (groundGravity - 1) * Time.fixedDeltaTime;
        //Debug.DrawRay(rb.transform.position, forceBaseGravity, Color.green, 5f);
        //Debug.Log("apply ground gravity");
        rb.velocity += forceBaseGravity;
    }
    
}
