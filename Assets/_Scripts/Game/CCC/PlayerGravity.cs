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

    [FoldoutGroup("Switch"), Tooltip("default air gravity"), SerializeField]
    private float speedRotateWhenSwitching = 30f;
    [FoldoutGroup("Switch"), Tooltip("marge de précision de la caméra sur sa cible"), SerializeField]
    private float timeBeforeResetBaseCamera = 0.4f;

    //[FoldoutGroup("Swtich"), Tooltip("min dist when we don't change planet !"), SerializeField]
    //private float distMinForChange = 2f;   //a-t-on un attract point de placé ?

    [FoldoutGroup("Debug"), Tooltip("default air gravity"), SerializeField]
    private OrientationPhysics currentOrientation = OrientationPhysics.OBJECT;
    public OrientationPhysics GetOrientationPhysics() { return (currentOrientation); }

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
    private EntityJumpCalculation entityJumpCalculation;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityRotateToGround rotateToGround;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script"), ReadOnly]
    private Transform mainAttractObject;
    private Vector3 mainAttractPoint;
    private Vector3 mainAttractNormal;
    public Transform GetMainAttractObject() { return (mainAttractObject); }

    [FoldoutGroup("Debug"), SerializeField, Tooltip("")]
    private bool isOnTransition = false;


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
        currentOrientation = OrientationPhysics.OBJECT;
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
        if (isOnTransition)
        {
            //ExtLog.DebugLogIa("stop transition !", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
            Debug.Log("stop transition");
            isOnTransition = false;
        }
    }

    /*
    /// <summary>
    /// calculate trajectory of entity
    /// </summary>
    public Vector3[] Plot(Rigidbody rigidbody, Vector3 pos, Vector3 velocity, int steps, bool noForceWhenUp, bool noForceWhenDown)
    {
        Vector3[] results = new Vector3[steps];

        float timestep = Time.fixedDeltaTime / magicTrajectoryCorrection;
        //gravityAttractorLerp = 1f;
        
        float drag = 1f - timestep * rigidbody.drag;
        Vector3 moveStep = velocity * timestep;

        int i = -1;
        while (++i < steps)
        {
            CalculateGravity(pos);
            Vector3 gravityAccel = FindAirGravity(pos, moveStep, GetMainAndOnlyGravity(), noForceWhenUp, noForceWhenDown) * timestep;
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
            ExtDrawGuizmos.DebugWireSphere(pos, Color.white, 0.1f, 0.1f);
        }
        return (results);
    }
    */

    private void ChangeStateGravity()
    {
        //here player is on fly
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            //Debug.Log("try to change gravity state");
            //here player is on fly, and we can create an attractor

            if (entityJump.IsReadyToTestCalculation() && currentOrientation == OrientationPhysics.NORMALS)
            {
                entityJump.entityJumpCalculation.UltimaTestBeforeAttractor();
            }
            else if (entityAttractor.CanCreateAttractor() && currentOrientation == OrientationPhysics.NORMALS)
            {
                currentOrientation = OrientationPhysics.ATTRACTOR;
                entityAttractor.SetupAttractor();
            }
            //here currently attractor attractive
            else if (currentOrientation == OrientationPhysics.ATTRACTOR)
            {

            }
            //here on fly but still attracted by the last normal
            else if (currentOrientation != OrientationPhysics.OBJECT)
            {
                currentOrientation = OrientationPhysics.NORMALS;
            }
            //here attracted by an object
            else
            {
                currentOrientation = OrientationPhysics.OBJECT;
            }
        }
        //here on ground
        else
        {
            currentOrientation = OrientationPhysics.NORMALS;
        }
        if (currentOrientation != OrientationPhysics.NORMALS)
        {
            entityAttractor.ResetFlyAway();
        }
    }
    
    public void ChangeMainAttractObject(Transform obj, Vector3 pointHit, Vector3 normalHit)
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir
            && !isOnTransition)
        {
            if (entityController.isPlayer)
            {
                PhilaeManager.Instance.cameraController.SetChangePlanetCam();
            }

            SetObjectAttraction(obj, pointHit, normalHit);

            rotateToGround.SetNewTempSpeed(speedRotateWhenSwitching);

            //CalculateGravity(rb.transform.position);

            PhilaeManager.Instance.PlanetChange();

            entityController.SetKinematic(true);
            ExtLog.DebugLogIa("change planete", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
            isOnTransition = true;
            Invoke("UnsetKinematic", timeBeforeResetBaseCamera);
        }
    }
    
    public void ChangeMainAttractObject(Transform rbTransform)
    {
        if (rbTransform.GetInstanceID() != mainAttractObject.GetInstanceID()
            && (entityController.GetMoveState() == EntityController.MoveState.InAir)
            && !isOnTransition && entityJump.IsJumpCoolDebugDownReady())
        {
            if (entityController.isPlayer)
            {
                PhilaeManager.Instance.cameraController.SetChangePlanetCam();
            }

            SetObjectAttraction(rbTransform, rbTransform.position, rb.position - rbTransform.position);
            PhilaeManager.Instance.PlanetChange();

            entityController.SetKinematic(true);
            ExtLog.DebugLogIa("change planete", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
            isOnTransition = true;
            Invoke("UnsetKinematic", timeBeforeResetBaseCamera);
        }
    }

    private void UnsetKinematic()
    {
        entityController.SetKinematic(false);
        PhilaeManager.Instance.cameraController.SetBaseCamera();
    }

    public Vector3 CalculateGravity(Vector3 positionEntity)
    {
        switch (currentOrientation)
        {
            case OrientationPhysics.OBJECT:
                if (entityJumpCalculation.CanApplyNormalizedObjectGravity())
                {
                    mainAndOnlyGravity = mainAttractNormal;
                    Debug.Log("go To Object (attract Normal)");
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
        Debug.LogWarning("TEMPORATY DESACTIVE UP JUMP");
        return (Vector3.zero);

        Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, orientationDown, Color.blue, 5f);
        return (orientationDown);
        
        //return (Vector3.zero);
    }
    /// <summary>
    /// apply base air gravity
    /// </summary>
    public Vector3 AirBaseGravity(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        Vector3 forceBaseGravityInAir = -gravityOrientation * gravity * (defaultGravityInAir - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, forceBaseGravityInAir, Color.green, 5f);
        return (forceBaseGravityInAir);
    }
    

    /// <summary>
    /// return the right gravity
    /// </summary>
    /// <returns></returns>
    public Vector3 FindAirGravity(Vector3 positionObject, Vector3 rbVelocity, Vector3 gravityOrientation, bool applyForceUp, bool applyForceDown)
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
            if (dotGravityRigidbody < 0)
            {
                if (applyForceDown)
                    finalGravity += AirAddGoingDown(gravityOrientation, positionObject);
            }
            //here we are going up, and we release the jump button, apply gravity down until the highest point
            else if (dotGravityRigidbody > 0 && !entityAction.Jump)
            {
                if (applyForceUp)
                    finalGravity += AirAddGoingUp(gravityOrientation, positionObject);
            }
            //Debug.Log("air gravity");
            //here, apply base gravity when we are InAir

            finalGravity += AirBaseGravity(gravityOrientation, positionObject);
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
        rb.velocity = FindAirGravity(rb.transform.position, rb.velocity, GetMainAndOnlyGravity(), true, entityJumpCalculation.CanApplyForceDown());
    }

    private void FixedUpdate()
    {
        ChangeStateGravity();
        CalculateGravity(rb.transform.position);

        ApplyGroundGravity();
        ApplySuplementGravity();
        ApplyAirGravity();
        //ExtDrawGuizmos.DebugWireSphere(rb.transform.position, Color.red, 0.1f, 0.1f);
        //Debug.DrawRay(rb.transform.position, GetMainAndOnlyGravity(), Color.red, 5f);
    }
}
