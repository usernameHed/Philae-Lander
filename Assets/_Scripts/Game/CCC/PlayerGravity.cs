using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    public enum OrientationPhysics
    {
        OBJECT,
        NORMALS,
        ATTRACTOR,
        GRAVITY_ATTRACTOR
    }

    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float gravity = 9.81f;
    public float Gravity { get { return (gravity); } }
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float magicTrajectoryCorrection = 1.4f;
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

    //[FoldoutGroup("Swtich"), Tooltip("min dist when we don't change planet !"), SerializeField]
    //private float distMinForChange = 2f;   //a-t-on un attract point de placé ?

    [FoldoutGroup("Debug"), Tooltip("default air gravity"), SerializeField]
    private OrientationPhysics currentOrientation = OrientationPhysics.OBJECT;
    public OrientationPhysics GetOrientationPhysics() { return (currentOrientation); }
    public void SetOrientation(OrientationPhysics orientation)
    {
        currentOrientation = orientation;
    }

    private Vector3 mainAndOnlyGravity = Vector3.zero;

    public Vector3 GetMainAndOnlyGravity()
    {
        return (mainAndOnlyGravity);
    }

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityController entityController;
    [FoldoutGroup("Object"), Tooltip("ref"), SerializeField]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAction entityAction;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityJump entityJump;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAttractor entityAttractor;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private FastForward fastForward;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityJumpCalculation entityJumpCalculation;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script"), ReadOnly]
    private Transform mainAttractObject;
    private Vector3 mainAttractPoint;
    private Vector3 mainAttractNormal;
    public Transform GetMainAttractObject() { return (mainAttractObject); }

    private void Start()
    {
        ResearchInitialGround();
        CalculateGravity(rb.transform.position);
    }

    public void SetObjectAttraction(Transform target)
    {
        SetObjectAttraction(target, target.position, Vector3.zero);
    }
    public void SetObjectAttraction(Transform target, Vector3 point, Vector3 normalHit)
    {
        SetOrientation(OrientationPhysics.OBJECT);
        mainAttractObject = target;
        mainAttractPoint = point;
        mainAttractNormal = normalHit;
    }

    private void ResearchInitialGround()
    {
        RaycastHit hit;
        //int raycastLayerMask = LayerMask.GetMask(entityController.walkablePlatform);
        Vector3 dirDown = rb.transform.up * -1;
        Debug.DrawRay(rb.transform.position, dirDown, Color.magenta, 5f);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rb.transform.position, dirDown, out hit, Mathf.Infinity, entityController.layerMask))
        {
            mainAttractObject = hit.transform;
            mainAttractPoint = hit.point;
            mainAttractNormal = hit.normal;
            ExtLog.DebugLogIa("Did Hit", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
        }
        else
        {
            ExtLog.DebugLogIa("No hit", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
        }
    }

    public void OnGrounded()
    {
        isGoingDown = false;
    }

    public void JustJumped()
    {
        isGoingDown = false;
        if (entityGravityAttractorSwitch.IsInGravityAttractorMode())
        {
            SetOrientation(OrientationPhysics.GRAVITY_ATTRACTOR);
        }
    }

    private void ChangeStateGravity()
    {
        //here player is on fly
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            //Debug.Log("try to change gravity state");
            //here player is on fly, and we can create an attractor
            if (entityGravityAttractorSwitch.IsInGravityAttractorMode())
            {
                SetOrientation(OrientationPhysics.GRAVITY_ATTRACTOR);
            }
            else if (entityAttractor.CanCreateAttractor() && currentOrientation == OrientationPhysics.NORMALS)
            {
                //bool aLittleMoreTime = entityJumpCalculation.UltimeTestBeforeAttractor();
                entityAttractor.ActiveAttractor();
            }
            //here currently attractor attractive
            else if (currentOrientation == OrientationPhysics.ATTRACTOR)
            {

            }
            //here on fly but still attracted by the last normal
            else if (currentOrientation != OrientationPhysics.OBJECT)
            {
                SetOrientation(OrientationPhysics.NORMALS);
            }
            //here attracted by an object
            else if (entityAttractor.CanCreateLateAttractor() && currentOrientation == OrientationPhysics.OBJECT)
            {
                entityAttractor.ActiveAttractor();
            }
        }
        //here on ground
        else
        {
            currentOrientation = OrientationPhysics.NORMALS;
        }
        if (currentOrientation != OrientationPhysics.NORMALS)
        {
            fastForward.ResetFlyAway();
        }
    }

    public Vector3 CalculateGravity(Vector3 positionEntity)
    {
        switch (currentOrientation)
        {
            case OrientationPhysics.OBJECT:
                if (entityJumpCalculation.CanApplyNormalizedObjectGravity())
                {
                    mainAndOnlyGravity = mainAttractNormal;
                    //Debug.Log("go To Object (attract Normal)");
                }
                else
                {
                    Vector3 direction = positionEntity - mainAttractPoint;
                    mainAndOnlyGravity = direction.normalized;
                }
                break;
            case OrientationPhysics.NORMALS:
                mainAndOnlyGravity = groundCheck.GetDirLastNormal();
                break;
            case OrientationPhysics.ATTRACTOR:
                mainAndOnlyGravity = entityAttractor.GetDirAttractor(positionEntity);
                break;
            case OrientationPhysics.GRAVITY_ATTRACTOR:
                entityGravityAttractorSwitch.CalculateSphereGravity(positionEntity);
                mainAndOnlyGravity = entityGravityAttractorSwitch.GetDirGAGravity();
                break;
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
        bool applyForceUp, bool applyForceDown, bool testForUltimate)
    {
        Vector3 finalGravity = rbVelocity;

        if (currentOrientation == OrientationPhysics.ATTRACTOR)
        {
            finalGravity += entityAttractor.AirAttractor(gravityOrientation, positionObject);
        }
        else
        {
            float dotGravityRigidbody = ExtQuaternion.DotProduct(gravityOrientation, rbVelocity);
            //here we fall down toward a planet, apply gravity down


            finalGravity += AirBaseGravity(gravityOrientation, positionObject, entityGravityAttractorSwitch.GetRatioGravity());

            if (dotGravityRigidbody < 0)
            {
                if (applyForceDown)
                {
                    finalGravity += AirAddGoingDown(gravityOrientation, positionObject);

                    //if first time we fall down, call ultimateTest !
                    if (testForUltimate && !isGoingDown)
                    {
                        entityJumpCalculation.UltimeTestBeforeAttractor();
                        isGoingDown = true;
                    }

                }

            }
            //here we are going up, and we release the jump button, apply gravity down until the highest point
            else if (dotGravityRigidbody > 0 && !entityAction.Jump)
            {
                isGoingDown = false;
                if (applyForceUp)
                    finalGravity += AirAddGoingUp(gravityOrientation, positionObject);
            }
            //Debug.Log("air gravity");
            //here, apply base gravity when we are InAir

        }
        return (finalGravity);
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
            entityJumpCalculation.GetSpecialAirGravity(),
            entityJumpCalculation.CanApplyForceUp(),
            entityJumpCalculation.CanApplyForceDown(),
            true);
    }

    private void FixedUpdate()
    {
        ChangeStateGravity();
        CalculateGravity(rb.transform.position);

        ApplyGroundGravity();
        ApplySuplementGravity();
        ApplyAirGravity();
        //ExtDrawGuizmos.DebugWireSphere(rb.transform.position, Color.red, 0.1f, 1f);
        //Debug.DrawRay(rb.transform.position, GetMainAndOnlyGravity(), Color.red, 5f);
    }
}
