using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGravity : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float gravity = 9.81f;
    public float Gravity { get { return (gravity); } }
    //[FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    //private float magicTrajectoryCorrectionRatio = 1f;

    [FoldoutGroup("Ground Gravity"), Tooltip("Add gravity when releasing jump button, and rigidbody is going UPward the planet"), SerializeField]
    private float groundAddGravity = 45f;
    [FoldoutGroup("Ground Gravity"), Tooltip("Down gravity when we are falling into the planet"), SerializeField]
    private float stickToFloorGravity = 6f;

    
    

    [FoldoutGroup("Air Gravity"), Tooltip("Add gravity when releasing jump button, and rigidbody is going UPward the planet"), SerializeField]
    private float rbUpAddGravity = 2.5f;
    [FoldoutGroup("Air Gravity"), Tooltip("Down gravity when we are falling into the planet"), SerializeField]
    private float rbDownAddGravity = 5f;
    [FoldoutGroup("Air Gravity"), Tooltip("default air gravity"), SerializeField]
    private float defaultGravityInAir = 2f;


    [FoldoutGroup("Switch"), SerializeField, Tooltip("MUST PRECEED AIR ATTRACTOR TIME !!")]
    private bool isGoingDown = false;
    public bool IsGoingDown()
    {
        return (isGoingDown);
    }

    private Vector3 mainAndOnlyGravity = Vector3.zero;

    public Vector3 GetMainAndOnlyGravity()
    {
        return (mainAndOnlyGravity);
    }

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityController entityController = null;
    [FoldoutGroup("Object"), Tooltip("ref"), SerializeField]
    private GroundCheck groundCheck = null;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAction entityAction = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityJump entityJump = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityNoGravity entityNoGravity = null;

    public void OnGrounded()
    {
        isGoingDown = false;
        Debug.Log("ici not down");
    }

    public void JustJumped()
    {
        isGoingDown = false;
        Debug.Log("ici not down");
    }

    public Vector3 CalculateGravity(Vector3 positionEntity)
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            mainAndOnlyGravity = entityGravityAttractorSwitch.GetDirGAGravity();
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
    private void ApplyGroundGravity()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            return;
        if (!entityJump.IsJumpCoolDebugDownReady())
            return;

        Vector3 gravityOrientation = GetMainAndOnlyGravity();

        //here, apply base gravity when we are InAir
        Vector3 forceBaseGravity = -gravityOrientation * gravity * (groundAddGravity - 1) * Time.fixedDeltaTime;
        //Debug.DrawRay(rb.transform.position, forceBaseGravity, Color.green, 5f);
        //Debug.Log("apply ground gravity");
        rb.velocity += forceBaseGravity;
    }

    /// <summary>
    /// apply a gravity force when Almost On ground
    /// It's happen when we have sudently a little gap, we have to
    /// stick to the floor as soon as possible !
    /// (exept when we just jumped !)
    /// </summary>
    private void ApplySuplementGravity()
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
        rb.velocity += orientationDown;
    }

    /// <summary>
    /// here we fall down toward a planet, apply gravity down
    /// </summary>
    private Vector3 AirAddGoingDown(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        //Debug.Log("ici down ?");
        Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, orientationDown, Color.blue, 5f);
        return (orientationDown);
    }
    /// <summary>
    /// here we are going up, and we release the jump button, apply gravity down until the highest point
    /// </summary>
    private Vector3 AirAddGoingUp(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        //Debug.LogWarning("TEMPORATY DESACTIVE UP JUMP");
        //return (Vector3.zero);

        Vector3 orientationDown = -gravityOrientation * gravity * (rbUpAddGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, orientationDown, Color.yellow, 5f);
        return (orientationDown);
        
        //return (Vector3.zero);
    }
    /// <summary>
    /// apply base air gravity
    /// </summary>
    public Vector3 AirBaseGravity(Vector3 gravityOrientation, Vector3 positionEntity, float boost = 1)
    {
        Vector3 forceBaseGravityInAir = -gravityOrientation * gravity * (defaultGravityInAir - 1) * boost * Time.fixedDeltaTime;
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

        float dotGravityRigidbody = ExtQuaternion.DotProduct(gravityOrientation, rbVelocity);
        //here we fall down toward a planet, apply gravity down

        finalGravity += AirBaseGravity(gravityOrientation, positionObject, entityGravityAttractorSwitch.GetAirRatioGravity()) * entityNoGravity.GetNoGravityRatio();

        if (dotGravityRigidbody < 0 || isGoingDown)
        {
            if (applyForceDown)
            {
                finalGravity += AirAddGoingDown(gravityOrientation, positionObject) * entityNoGravity.GetNoGravityRatio() * entityGravityAttractorSwitch.GetRatioGravityDown();

                //if first time we fall down, call ultimateTest !
                if (!isGoingDown)
                {
                    isGoingDown = true;
                    //Debug.Log("ici down");
                }

            }

        }

        if (!isGoingDown)
        {
            //here we are going up, and we release the jump button, apply gravity down until the highest point
            if (dotGravityRigidbody > 0 && !entityAction.Jump)
            {
                isGoingDown = false;
                //Debug.Log("ici not down");
                if (applyForceUp)
                    finalGravity += AirAddGoingUp(gravityOrientation, positionObject) * entityNoGravity.GetNoGravityRatio();
            }
            //here we are going up, continiue pressing the jump button, AND in gravityAttractor
            else if (dotGravityRigidbody > 0 && entityAction.Jump
                && entityGravityAttractorSwitch.CanApplyForceDown())
            {
                isGoingDown = false;
                //Debug.Log("ici not down");
                if (applyForceUp)
                    finalGravity += AirAddGoingUp(gravityOrientation, positionObject) * entityNoGravity.GetNoGravityRatio() * (entityGravityAttractorSwitch.GetAirRatioGravity() / 2);
            }
        }
        

        return (finalGravity);
    }

    private bool CanApplyForceUp()
    {
        if (entityNoGravity.WereWeInNoGravity())
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

        //if (currentOrientation != OrientationPhysics.ATTRACTOR)
        rb.velocity = FindAirGravity(rb.transform.position, rb.velocity,
            GetMainAndOnlyGravity(),
            CanApplyForceUp(),
            true);
    }

    private void FixedUpdate()
    {
        CalculateGravity(rb.transform.position);

        ApplyGroundGravity();
        ApplySuplementGravity();
        ApplyAirGravity();
        //ExtDrawGuizmos.DebugWireSphere(rb.transform.position, Color.red, 0.1f, 1f);
        //Debug.DrawRay(rb.transform.position, GetMainAndOnlyGravity(), Color.red, 5f);
    }
}
