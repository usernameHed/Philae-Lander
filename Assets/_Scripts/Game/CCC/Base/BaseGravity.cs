using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGravity : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    protected float gravity = 9.81f;
    public float Gravity { get { return (gravity); } }
    
    [FoldoutGroup("Air Gravity"), Tooltip("default air gravity"), SerializeField]
    protected float defaultGravityInAir = 2f;
    [FoldoutGroup("Air Gravity"), Tooltip("default air gravity"), SerializeField]
    protected float defaultGravityOnGround = 5.5f;
    [FoldoutGroup("Air Gravity"), Tooltip("Down gravity when we are falling into the planet"), SerializeField]
    protected float rbDownAddGravity = 3.5f;

    [FoldoutGroup("Switch"), SerializeField, Tooltip("down a partir du moment ou on est donw la premiere fois")]
    protected bool doWeSwitchBetweenBoth = true;
    [FoldoutGroup("Switch"), SerializeField, Tooltip("up or down selon la normal dot"), ReadOnly]
    protected bool isGoingDown = false;
    public bool IsGoingDown() => isGoingDown;
    [FoldoutGroup("Switch"), SerializeField, Tooltip("down a partir du moment ou on est donw la premiere fois"), ReadOnly]
    protected bool isGoingDownToGround = false;

    public bool IsGoingDownToGround()
    {
        return (isGoingDownToGround);
    }

    private Vector3 mainAndOnlyGravity = Vector3.zero;

    public Vector3 GetMainAndOnlyGravity()
    {
        return (mainAndOnlyGravity);
    }

    public virtual void OnGrounded()
    {
        isGoingDown = isGoingDownToGround = false;
    }

    public virtual void JustJumped()
    {
        isGoingDown = isGoingDownToGround = false;
    }

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityController entityController = null;
    [FoldoutGroup("Object"), Tooltip("ref"), SerializeField]
    protected GroundCheck groundCheck = null;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected Rigidbody rb = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected BaseGravityAttractorSwitch baseGravityAttractorSwitch = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    protected EntityNoGravity entityNoGravity = null;

    public Vector3 CalculateGravity(Vector3 positionEntity)
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            mainAndOnlyGravity = baseGravityAttractorSwitch.GetDirGAGravity();
        }
        else
        {
            mainAndOnlyGravity = groundCheck.GetDirLastNormal();
        }
        return (mainAndOnlyGravity);
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

    protected void SetGoingDown()
    {
        float dotGravityRigidbody = ExtQuaternion.DotProduct(GetMainAndOnlyGravity(), rb.velocity);
        if (dotGravityRigidbody < 0 || (isGoingDown && !doWeSwitchBetweenBoth))
        {
            //first time falling
            if (!isGoingDown)
            {
                isGoingDown = true;
            }
            if (!isGoingDownToGround)
            {
                isGoingDownToGround = true;
            }
        }
        else
        {
            isGoingDown = false;
        }
    }

    /// <summary>
    /// apply base air gravity
    /// </summary>
    protected Vector3 AirBaseGravity(Vector3 gravityOrientation, Vector3 positionEntity, float boost = 1)
    {
        Vector3 forceBaseGravityInAir = -gravityOrientation * gravity * (defaultGravityInAir - 1) * boost * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, forceBaseGravityInAir, Color.green, 5f);
        return (forceBaseGravityInAir);
    }

    /// <summary>
    /// here we fall down toward a planet, apply gravity down
    /// </summary>
    protected Vector3 AirAddGoingDown(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        //Debug.Log("ici down ?");
        Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, orientationDown, Color.blue, 5f);
        return (orientationDown);
    }
}
