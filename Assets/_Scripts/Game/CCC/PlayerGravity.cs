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

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("")]
    private float timeBeforeActiveAttractorInAir = 0.5f;

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("raycast to ground layer")]
    private string[] layersRaycast;

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

    [FoldoutGroup("Air Attractor"), Tooltip("default air gravity"), SerializeField]
    private float gravityAttractor = 2f;
    //[FoldoutGroup("Air Gravity"), Tooltip("Down gravity when we are falling into the planet"), SerializeField]
    //private float rbDownAddGravity = 5f;
    [FoldoutGroup("Air Attractor"), Tooltip("default air gravity"), SerializeField]
    private float speedLerpAttractor = 5f;

    [FoldoutGroup("Debug"), Tooltip("default air gravity"), SerializeField]
    private OrientationPhysics currentOrientation = OrientationPhysics.OBJECT;

    private Vector3 mainAndOnlyGravity = Vector3.zero;
    private FrequencyCoolDown timerBeforeCreateAttractor = new FrequencyCoolDown();

    private Vector3 transformPoint = Vector3.zero;

    public Vector3 GetMainAndOnlyGravity()
    {
        return (mainAndOnlyGravity);
    }

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerController playerController;
    [FoldoutGroup("Object"), Tooltip("ref"), SerializeField]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerInput playerInput;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerJump playerJump;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script")]
    private Transform mainAttractObject;

    private float gravityAttractorLerp = 1f;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.OnGrounded, OnGrounded);
    }

    private void Awake()
    {
        ResearchInitialGround();
        CalculateGravity();
    }

    private void ResearchInitialGround()
    {
        RaycastHit hit;
        int raycastLayerMask = LayerMask.GetMask(layersRaycast);
        Vector3 dirDown = rb.transform.up * -1;
        Debug.DrawRay(rb.transform.position, dirDown, Color.magenta, 5f);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rb.transform.position, dirDown, out hit, Mathf.Infinity, raycastLayerMask))
        {
            mainAttractObject = hit.transform;
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.Log("No hit");
        }
    }

    /// <summary>
    /// called when jump
    /// </summary>
    public void CreateAttractor()
    {
        Debug.Log("create attractor !");
        transformPoint = rb.transform.position;
        timerBeforeCreateAttractor.StartCoolDown(timeBeforeActiveAttractorInAir);
    }

    private void ActiveAttractor()
    {
        Debug.Log("attractor activated !");
        currentOrientation = OrientationPhysics.ATTRACTOR;
        gravityAttractorLerp = 1;
    }

    private Vector3 GetDirAttractor()
    {
        Vector3 dirAttractor =  rb.transform.position - transformPoint;
        return (dirAttractor);
    }

    private void ChangeStateGravity()
    {
        //here player is on fly
        if (playerController.GetMoveState() == PlayerController.MoveState.InAir)
        {
            //Debug.Log("try to change gravity state");
            //here player is on fly, and we can create an attractor
            if (timerBeforeCreateAttractor.IsStartedAndOver() && currentOrientation == OrientationPhysics.NORMALS)
            {
                ActiveAttractor();
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
            Debug.Log("reset timer ??? we aree on ground wtf ??");
            timerBeforeCreateAttractor.Reset();
            currentOrientation = OrientationPhysics.NORMALS;
        }
    }

    private void CalculateGravity()
    {
        switch (currentOrientation)
        {
            case OrientationPhysics.OBJECT:
                Vector3 direction = rb.position - mainAttractObject.position;
                mainAndOnlyGravity = direction.normalized;
                break;
            case OrientationPhysics.NORMALS:
                mainAndOnlyGravity = groundCheck.GetDirLastNormal();
                break;
            case OrientationPhysics.ATTRACTOR:
                mainAndOnlyGravity = GetDirAttractor();
                break;
        }
    }

    public void OnGrounded()
    {
        //Debug.Log("reset, we are onground !");
    }

    /// <summary>
    /// apply gravity on ground
    /// </summary>
    private void ApplyGroundGravity()
    {
        if (playerController.GetMoveState() == PlayerController.MoveState.InAir)
            return;
        if (!playerJump.IsJumpCoolDebugDownReady())
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
        if (!playerJump.IsJumpCoolDebugDownReady())
            return;

        Vector3 gravityOrientation = GetMainAndOnlyGravity();
        //Debug.LogWarning("Apply gravity down down down !");

        Vector3 orientationDown = -gravityOrientation * gravity * (stickToFloorGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(rb.transform.position, orientationDown, Color.red, 5f);
        rb.velocity += orientationDown;
    }

    /// <summary>
    /// apply every gravity force in Air
    /// </summary>
    private void ApplyAirGravity()
    {
        if (playerController.GetMoveState() != PlayerController.MoveState.InAir)
            return;

        Vector3 gravityOrientation = GetMainAndOnlyGravity();
        float dotGravityRigidbody = ExtQuaternion.DotProduct(gravityOrientation, rb.velocity);
        //here we fall down toward a planet, apply gravity down
        if (dotGravityRigidbody < 0 && currentOrientation != OrientationPhysics.ATTRACTOR)
        {
            Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
            Debug.DrawRay(rb.transform.position, orientationDown, Color.blue, 5f);
            rb.velocity += orientationDown;

            Debug.Log("going down");
            //Debug.Break();
        }
        /*else if (dotGravityRigidbody < 0 && currentOrientation == OrientationPhysics.ATTRACTOR)
        {
            Debug.Log("going down and attractor");
            Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
            Debug.DrawRay(rb.transform.position, orientationDown, Color.blue, 5f);
            rb.velocity += orientationDown;
        }*/


        //here we are going up, and we release the jump button, apply gravity down until the highest point
        else if (dotGravityRigidbody > 0 && !playerInput.Jump)
        {
            Vector3 orientationUp = -gravityOrientation * gravity * (rbUpAddGravity - 1) * Time.fixedDeltaTime;
            Debug.DrawRay(rb.transform.position, orientationUp, Color.yellow, 5f);
            rb.velocity += orientationUp;
            //Debug.Log("going up");
        }
        //Debug.Log("air gravity");
        //here, apply base gravity when we are InAir

        if (currentOrientation != OrientationPhysics.ATTRACTOR)
        {
            Vector3 forceBaseGravityInAir = -gravityOrientation * gravity * (defaultGravityInAir - 1) * Time.fixedDeltaTime;
            Debug.DrawRay(rb.transform.position, forceBaseGravityInAir, Color.green, 5f);
            rb.velocity += forceBaseGravityInAir;
        }
        else
        {
            Debug.Log("attractor !!!");
            gravityAttractorLerp = Mathf.Lerp(gravityAttractorLerp, gravityAttractor, Time.fixedDeltaTime * speedLerpAttractor);

            Vector3 forceAttractor = -gravityOrientation * gravity * (gravityAttractorLerp - 1) * Time.fixedDeltaTime;
            Debug.DrawRay(rb.transform.position, forceAttractor, Color.white, 5f);
            ExtDrawGuizmos.DebugWireSphere(forceAttractor, Color.white, 1f, 5f);
            rb.velocity += forceAttractor;
        }
    }

    private void FixedUpdate()
    {
        ChangeStateGravity();
        CalculateGravity();

        ApplyGroundGravity();
        ApplySuplementGravity();
        ApplyAirGravity();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.OnGrounded, OnGrounded);
    }
}
