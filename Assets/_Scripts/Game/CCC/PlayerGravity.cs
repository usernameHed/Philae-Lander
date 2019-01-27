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
    [FoldoutGroup("Air Attractor"), Tooltip("default air gravity"), SerializeField]
    private float speedLerpAttractor = 5f;

    [FoldoutGroup("Air Attractor"), Tooltip("position de l'attractpoint"), SerializeField]
    public float lengthPositionAttractPoint = 1f;    //position de l'attract point par rapport à la dernier position / normal
    [FoldoutGroup("Air Attractor"), Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float sizeDistanceForSavePlayerPos = 0.5f;   //a-t-on un attract point de placé ?

    [FoldoutGroup("Debug"), Tooltip("default air gravity"), SerializeField]
    private OrientationPhysics currentOrientation = OrientationPhysics.OBJECT;
    [FoldoutGroup("Debug"), Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float differenceAngleNormalForUpdatePosition = 5f;   //a-t-on un attract point de placé ?

    private Vector3 mainAndOnlyGravity = Vector3.zero;
    private FrequencyCoolDown timerBeforeCreateAttractor = new FrequencyCoolDown();

    private Vector3 transformPointAttractor = Vector3.zero;

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
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Transform rbRotate;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script"), ReadOnly]
    private Transform mainAttractObject;
    public Transform GetMainAttractObject() { return (mainAttractObject); }
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script"), ReadOnly]
    private Vector3[] worldLastPosition = new Vector3[3];      //save la derniere position grounded...

    private bool isOnTransition = false;

    private float gravityAttractorLerp = 1f;
    private Vector3 worldPreviousNormal;    //et sa dernière normal accepté par le changement d'angle
    private Vector3 worldLastNormal;        //derniere normal enregistré, peut import le changement position/angle

    private void Awake()
    {
        
    }

    private void Start()
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

    private void WorldLastPositionSet(Vector3 newValue)
    {
        Vector3 next = Vector3.zero;
        for (int i = 0; i < worldLastPosition.Length - 1; i++)
        {
            if (i == 0)
            {
                next = worldLastPosition[0];
                worldLastPosition[0] = newValue;
            }
            else
            {
                Vector3 tmpValue = worldLastPosition[i];
                worldLastPosition[i] = next;
                next = tmpValue;
            }
        }
    }
    private Vector3 WorldLastPositionGetIndex(int index)
    {
        index = (index < 0) ? 0 : index;
        index = (index >= worldLastPosition.Length) ? worldLastPosition.Length - 1 : index;
        return (worldLastPosition[index]);
    }

    public void SaveLastPositionOnground()
    {
        if (entityController.GetMoveState() == PlayerController.MoveState.InAir)
            return;

        worldLastNormal = GetMainAndOnlyGravity();   //avoir toujours une normal à jour
        float distForSave = (WorldLastPositionGetIndex(0) - rb.transform.position).sqrMagnitude;

        //Debug.Log("dist save: " + distForSave);
        //si la distance entre les 2 point est trop grande, dans tout les cas, save la nouvelle position !
        if (distForSave > sizeDistanceForSavePlayerPos)
        {
            WorldLastPositionSet(rb.transform.position); //save la position onGround
            ExtDrawGuizmos.DebugWireSphere(WorldLastPositionGetIndex(0), Color.red, 0.5f, 1f);
        }
        //si la normal à changé, update la position + normal !
        else if (worldPreviousNormal != worldLastNormal)
        {
            //ici changement de position SEULEMENT si l'angle de la normal diffère de X
            float anglePreviousNormal = ExtQuaternion.GetAngleFromVector3(worldPreviousNormal, rbRotate.up);
            float angleNormalPlayer = ExtQuaternion.GetAngleFromVector3(worldLastNormal, rbRotate.up);
            //ici gérer les normal à zero ??
            float diff;
            if (ExtQuaternion.IsAngleCloseToOtherByAmount(anglePreviousNormal, angleNormalPlayer, differenceAngleNormalForUpdatePosition, out diff))
            {
                //Debug.Log("ici l'angle est trop proche, ducoup ne pas changer de position");

                //ni de normal ??
            }
            else
            {

                //ici change la normal, ET la position
                WorldLastPositionSet(rb.transform.position); //save la position onGround
                worldPreviousNormal = worldLastNormal;

                ExtDrawGuizmos.DebugWireSphere(WorldLastPositionGetIndex(0), Color.yellow, 0.5f, 1f);
                Debug.DrawRay(rb.transform.position, worldPreviousNormal, Color.yellow, 1f);
            }

            //coolDownUpdatePos.StartCoolDown();
        }
    }

    /// <summary>
    /// called when jump
    /// </summary>
    public void CreateAttractor()
    {
        Debug.Log("create attractor !");
        timerBeforeCreateAttractor.StartCoolDown(timeBeforeActiveAttractorInAir);

        transformPointAttractor = WorldLastPositionGetIndex(1) - worldLastNormal * lengthPositionAttractPoint;

        //ExtDrawGuizmos.DebugWireSphere(transformPointAttractor, Color.white, 1f, 1f);

        //ExtDrawGuizmos.DebugWireSphere(WorldLastPositionGetIndex(1), Color.red, 1f, 2f);          //ancienne pos
        //ExtDrawGuizmos.DebugWireSphere(transformPointAttractor, Color.blue, 1f, 2f);      //nouvel pos
        //Debug.DrawRay(WorldLastPositionGetIndex(0), worldLastNormal * 4, Color.red, 2f);      //last normal

        //Debug.Break();
    }

    private void ActiveAttractor()
    {
        Debug.Log("attractor activated !");
        currentOrientation = OrientationPhysics.ATTRACTOR;
        gravityAttractorLerp = 1;
    }

    private Vector3 GetDirAttractor()
    {
        Vector3 dirAttractor =  rb.transform.position - transformPointAttractor;
        return (dirAttractor);
    }

    private void ChangeStateGravity()
    {
        //here player is on fly
        if (entityController.GetMoveState() == PlayerController.MoveState.InAir)
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
            /*if (currentOrientation == OrientationPhysics.OBJECT)
            {
                PhilaeManager.Instance.cameraController.SetBaseCamera();
            }*/
            //Debug.Log("reset timer ??? we aree on ground wtf ??");
            timerBeforeCreateAttractor.Reset();
            currentOrientation = OrientationPhysics.NORMALS;
        }
    }

    public void ChangeMainAttractObject(Transform rbTransform)
    {
        if (rbTransform.GetInstanceID() != mainAttractObject.GetInstanceID()
            && (entityController.GetMoveState() == EntityController.MoveState.InAir)
            && !isOnTransition)
        {
            PhilaeManager.Instance.cameraController.SetChangePlanetCam();

            mainAttractObject = rbTransform;
            currentOrientation = OrientationPhysics.OBJECT;
            PhilaeManager.Instance.PlanetChange();

            entityController.SetKinematic(true);

            isOnTransition = true;
            Invoke("UnsetKinematic", PhilaeManager.Instance.cameraController.GetTimeKinematic());
        }
    }

    private void UnsetKinematic()
    {
        entityController.SetKinematic(false);
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

    /// <summary>
    /// apply gravity on ground
    /// </summary>
    private void ApplyGroundGravity()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            return;
        if (!entityJump.IsJumpCoolDebugDownReady())
            return;

        if (isOnTransition)
        {
            Debug.Log("stop transition !");
            isOnTransition = false;
        }

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
        //Debug.DrawRay(rb.transform.position, orientationDown, Color.red, 5f);
        rb.velocity += orientationDown;
    }

    /// <summary>
    /// apply every gravity force in Air
    /// </summary>
    private void ApplyAirGravity()
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return;

        Vector3 gravityOrientation = GetMainAndOnlyGravity();
        float dotGravityRigidbody = ExtQuaternion.DotProduct(gravityOrientation, rb.velocity);
        //here we fall down toward a planet, apply gravity down
        if (dotGravityRigidbody < 0 && currentOrientation != OrientationPhysics.ATTRACTOR)
        {
            Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
            //Debug.DrawRay(rb.transform.position, orientationDown, Color.blue, 5f);
            rb.velocity += orientationDown;

            //Debug.Log("going down");
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
        else if (dotGravityRigidbody > 0 && !entityAction.Jump)
        {
            Vector3 orientationUp = -gravityOrientation * gravity * (rbUpAddGravity - 1) * Time.fixedDeltaTime;
            //Debug.DrawRay(rb.transform.position, orientationUp, Color.yellow, 5f);
            rb.velocity += orientationUp;
            //Debug.Log("going up");
        }
        //Debug.Log("air gravity");
        //here, apply base gravity when we are InAir

        if (currentOrientation != OrientationPhysics.ATTRACTOR)
        {
            Vector3 forceBaseGravityInAir = -gravityOrientation * gravity * (defaultGravityInAir - 1) * Time.fixedDeltaTime;
            //Debug.DrawRay(rb.transform.position, forceBaseGravityInAir, Color.green, 5f);
            rb.velocity += forceBaseGravityInAir;
        }
        else
        {
            //Debug.Log("attractor !!!");
            gravityAttractorLerp = Mathf.Lerp(gravityAttractorLerp, gravityAttractor, Time.fixedDeltaTime * speedLerpAttractor);

            Vector3 forceAttractor = -gravityOrientation * gravity * (gravityAttractorLerp - 1) * Time.fixedDeltaTime;
            //Debug.DrawRay(rb.transform.position, forceAttractor, Color.white, 5f);
            //ExtDrawGuizmos.DebugWireSphere(forceAttractor, Color.white, 1f, 5f);
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

        SaveLastPositionOnground();
    }
}
