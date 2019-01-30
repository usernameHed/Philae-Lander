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
    private float magicTrajectoryCorrection = 1f;
    //[FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    //private float magicTrajectoryCorrectionRatio = 1f;


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

    [FoldoutGroup("Air Attractor"), SerializeField, Tooltip("")]
    private float timeBeforeActiveAttractorInAir = 0.5f;
    [FoldoutGroup("Air Attractor"), Tooltip("default air gravity"), SerializeField]
    private Vector2 gravityAttractorMinMax = new Vector2(2f, 100f);
    [FoldoutGroup("Air Attractor"), Tooltip("default air gravity"), SerializeField]
    private Vector2 speedLerpAttractorMinMax = new Vector2(5f, 0.001f);
    [FoldoutGroup("Air Attractor"), Tooltip(""), SerializeField]
    private string[] objOnSight = new string[] {"Walkable/Floor"};

    [FoldoutGroup("Air Attractor"), SerializeField, Tooltip("ref script")]
    private float distAllowedForNormalGravity = 10f;


    [FoldoutGroup("Swtich"), Tooltip("min dist when we don't change planet !"), SerializeField]
    private float distMinForChange = 2f;   //a-t-on un attract point de placé ?


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
        FillAllPos();
        ResearchInitialGround();
        CalculateGravity(rb.transform.position);
    }

    private void FillAllPos()
    {
        for (int i = 0; i < worldLastPosition.Length; i++)
        {
            worldLastPosition[i] = rb.transform.position;
        }
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
            ExtLog.DebugLogIa("Did Hit", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
        }
        else
        {
            ExtLog.DebugLogIa("No hit", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
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
        ExtLog.DebugLogIa("create attractor !", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);

        timerBeforeCreateAttractor.StartCoolDown(timeBeforeActiveAttractorInAir);

        transformPointAttractor = WorldLastPositionGetIndex(1) - worldLastNormal * lengthPositionAttractPoint;

        ExtDrawGuizmos.DebugWireSphere(transformPointAttractor, Color.white, 1f, 1f);

        ExtDrawGuizmos.DebugWireSphere(WorldLastPositionGetIndex(1), Color.red, 1f, 2f);          //ancienne pos
        Debug.Log("ici create ?");
        ExtDrawGuizmos.DebugWireSphere(transformPointAttractor, Color.blue, 1f, 2f);      //nouvel pos
        Debug.DrawRay(WorldLastPositionGetIndex(0), worldLastNormal * 4, Color.red, 2f);      //last normal

        //Debug.Break();
    }

    /// <summary>
    /// get vector director of attractor for the new physics direction
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDirAttractor(Vector3 positionEntity)
    {
        Vector3 dirAttractor = positionEntity - transformPointAttractor;
        return (dirAttractor);
    }

    public Vector3[] Plot(Rigidbody rigidbody, Vector3 pos, Vector3 velocity, int steps)
    {
        Vector3[] results = new Vector3[steps];

        float timestep = Time.fixedDeltaTime / magicTrajectoryCorrection;
        gravityAttractorLerp = 1f;
        
        float drag = 1f - timestep * rigidbody.drag;
        Vector3 moveStep = velocity * timestep;
        /*Vector3 gravityAccel2 = FindAirGravity(pos, moveStep, GetMainAndOnlyGravity()) * timestep;// * timestep * timestep;
        moveStep += gravityAccel2;
        pos += moveStep;
        */
        for (int i = 0; i < steps; ++i)
        {
            CalculateGravity(pos);
            Vector3 gravityAccel = FindAirGravity(pos, moveStep, GetMainAndOnlyGravity()) * timestep;// * timestep * timestep;
            //Vector3 gravityAccel = AirAttractor(GetMainAndOnlyGravity(), pos) * timestep * timestep;
            //Vector3 forceBaseGravityInAir = -gravityOrientation * gravity * (defaultGravityInAir - 1) * Time.fixedDeltaTime;

            //gravityAccel += AirBaseGravity(GetMainAndOnlyGravity(), pos);

            moveStep += gravityAccel;
            //moveStep *= drag;
            pos += moveStep;
            //pos += GetMainAndOnlyGravity() * magicTrajectoryCorrectionRatio;
            results[i] = pos;
            ExtDrawGuizmos.DebugWireSphere(results[i], Color.white, 0.1f, 5f);
        }
        //Trajectory.DebugTrajectory(rigidbody, velocity, ForceMode.VelocityChange, Color.green, 3.0f, 3.0f);

        return results;
    }

    /// <summary>
    /// test if we create an attractor point, or if we can simply keep the actual gravity !
    /// or change it to the normal of the collision ??
    /// </summary>
    /// <returns></returns>
    private bool RaycastIfWeCanDoNormalGravity()
    {
        //Debug.Break();
        return (false);
    }

    /// <summary>
    /// create an attractor point for entity gravity !
    /// </summary>
    private void ActiveAttractor()
    {
        ExtLog.DebugLogIa("attractor activated !", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
        currentOrientation = OrientationPhysics.ATTRACTOR;

        //camera change only of this is have player
        if (entityController.isPlayer)
        {
            PhilaeManager.Instance.cameraController.SetAttractorCamera();
        }

        //RESET LERP !!! important !
        gravityAttractorLerp = 1;
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
                Vector3 lastPos = Plot(rb, rb.transform.position, rb.velocity, 15)[14];

                RaycastHit hitInfo;
                int layerMask = Physics.AllLayers;
                layerMask = LayerMask.GetMask(objOnSight);

                Vector3 dirRaycast = lastPos - rb.transform.position;
                Debug.DrawRay(rb.transform.position, dirRaycast, Color.red, 5f);
                if (Physics.SphereCast(rb.transform.position, 0.3f, dirRaycast, out hitInfo,
                                       dirRaycast.magnitude, layerMask, QueryTriggerInteraction.Ignore))
                {
                    Debug.Log("find something ! keep going with normal gravity");
                }
                else
                {
                    Vector3 dirNewRaycast = GetMainAndOnlyGravity() * -1;
                    float distRaycast = 5f;
                    Debug.DrawRay(lastPos, dirNewRaycast * distRaycast, Color.cyan, 5f);

                    if (Physics.SphereCast(lastPos, 0.3f, dirNewRaycast, out hitInfo,
                                           distRaycast, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        Debug.Log("find something in stage 2 ! normal gravity !!");
                    }
                }


                ActiveAttractor();
                //Plot(rb, rb.transform.position, rb.velocity, 30);
                gravityAttractorLerp = 1;
                Debug.Break();

                /*if (!RaycastIfWeCanDoNormalGravity())
                {
                    ActiveAttractor();
                }
                */
                    
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
            //Debug.Log("reset timer ??? we aree on ground wtf ??");
            timerBeforeCreateAttractor.Reset();
            currentOrientation = OrientationPhysics.NORMALS;
        }
    }

    public bool IsTooCloseToOtherPlanet(Transform rbTransform)
    {
        float dist = Vector3.SqrMagnitude(rb.position - rbTransform.position);
        ExtLog.DebugLogIa("dist from attractive planet: " + dist, (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
        if (dist < distMinForChange)
        {
            return (true);
        }
        return (false);
    }

    public void ChangeMainAttractObject(Transform rbTransform)
    {
        if (rbTransform.GetInstanceID() != mainAttractObject.GetInstanceID()
            && (entityController.GetMoveState() == EntityController.MoveState.InAir)
            && !isOnTransition && entityJump.IsJumpCoolDebugDownReady()
            && !IsTooCloseToOtherPlanet(rbTransform))
        {
            if (entityController.isPlayer)
            {
                PhilaeManager.Instance.cameraController.SetChangePlanetCam();
            }                

            mainAttractObject = rbTransform;
            currentOrientation = OrientationPhysics.OBJECT;
            PhilaeManager.Instance.PlanetChange();

            entityController.SetKinematic(true);
            ExtLog.DebugLogIa("change planete", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
            isOnTransition = true;
            Invoke("UnsetKinematic", PhilaeManager.Instance.cameraController.GetTimeKinematic());
        }
    }

    private void UnsetKinematic()
    {
        entityController.SetKinematic(false);
    }

    private void CalculateGravity(Vector3 positionEntity)
    {
        switch (currentOrientation)
        {
            case OrientationPhysics.OBJECT:
                Vector3 direction = positionEntity - mainAttractObject.position;
                mainAndOnlyGravity = direction.normalized;
                break;
            case OrientationPhysics.NORMALS:
                mainAndOnlyGravity = groundCheck.GetDirLastNormal();
                break;
            case OrientationPhysics.ATTRACTOR:
                mainAndOnlyGravity = GetDirAttractor(positionEntity);
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
            ExtLog.DebugLogIa("stop transition !", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
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
        Debug.DrawRay(rb.transform.position, orientationDown, Color.red, 5f);
        rb.velocity += orientationDown;
    }

    /// <summary>
    /// here we fall down toward a planet, apply gravity down
    /// </summary>
    private Vector3 AirAddGoingDown(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        Debug.Log("ici down ?");
        Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, orientationDown, Color.blue, 5f);
        return (orientationDown);
    }
    /// <summary>
    /// here we are going up, and we release the jump button, apply gravity down until the highest point
    /// </summary>
    private Vector3 AirAddGoingUp(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        Debug.Log("ici up");
        Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, orientationDown, Color.blue, 5f);
        return (orientationDown);
    }
    /// <summary>
    /// apply base air gravity
    /// </summary>
    private Vector3 AirBaseGravity(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        Vector3 forceBaseGravityInAir = -gravityOrientation * gravity * (defaultGravityInAir - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, forceBaseGravityInAir, Color.green, 5f);
        return (forceBaseGravityInAir);
    }
    /// <summary>
    /// apply attractor gravity
    /// </summary>
    private Vector3 AirAttractor(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        gravityAttractorLerp = Mathf.Lerp(gravityAttractorLerp, gravityAttractorMinMax.x, Time.fixedDeltaTime * speedLerpAttractorMinMax.x);
        /*
        if (useMin)
        {
            gravityAttractorLerp = Mathf.Lerp(gravityAttractorLerp, gravityAttractorMinMax.x, Time.fixedDeltaTime * speedLerpAttractorMinMax.x);
        }
        else
        {
            gravityAttractorLerp = Mathf.Lerp(gravityAttractorLerp, gravityAttractorMinMax.y, Time.fixedDeltaTime * speedLerpAttractorMinMax.y);
        }*/

        //gravityAttractorLerp = defaultGravityInAir;

        Vector3 forceAttractor = -gravityOrientation * gravity * (gravityAttractorLerp - 1) * Time.fixedDeltaTime;

        Debug.DrawRay(positionEntity, forceAttractor, Color.white, 5f);
        return (forceAttractor);
    }

    /// <summary>
    /// return the right gravity
    /// </summary>
    /// <returns></returns>
    private Vector3 FindAirGravity(Vector3 positionObject, Vector3 rbVelocity, Vector3 gravityOrientation)
    {
        Vector3 finalGravity = rbVelocity;

        if (currentOrientation == OrientationPhysics.ATTRACTOR)
        {
            finalGravity += AirAttractor(gravityOrientation, positionObject);
        }
        else
        {
            float dotGravityRigidbody = ExtQuaternion.DotProduct(gravityOrientation, rbVelocity);
            //here we fall down toward a planet, apply gravity down
            if (dotGravityRigidbody < 0 && currentOrientation != OrientationPhysics.ATTRACTOR)
            {
                finalGravity += AirAddGoingDown(gravityOrientation, positionObject);
            }
            //here we are going up, and we release the jump button, apply gravity down until the highest point
            else if (dotGravityRigidbody > 0 && !entityAction.Jump && currentOrientation != OrientationPhysics.ATTRACTOR)
            {
                finalGravity += AirAddGoingUp(gravityOrientation, positionObject);
            }
            //Debug.Log("air gravity");
            //here, apply base gravity when we are InAir

            if (currentOrientation != OrientationPhysics.ATTRACTOR)
            {
                finalGravity += AirBaseGravity(gravityOrientation, positionObject);
            }
        }
        /*

        float dotGravityRigidbody = ExtQuaternion.DotProduct(gravityOrientation, rbVelocity);
        //here we fall down toward a planet, apply gravity down
        if (dotGravityRigidbody < 0 && currentOrientation != OrientationPhysics.ATTRACTOR)
        {
            finalGravity += AirAddGoingDown(gravityOrientation, positionObject);
        }
        //here we are going up, and we release the jump button, apply gravity down until the highest point
        else if (dotGravityRigidbody > 0 && !entityAction.Jump && currentOrientation != OrientationPhysics.ATTRACTOR)
        {
            finalGravity += AirAddGoingUp(gravityOrientation, positionObject);
        }
        //Debug.Log("air gravity");
        //here, apply base gravity when we are InAir

        if (currentOrientation != OrientationPhysics.ATTRACTOR)
        {
            finalGravity += AirBaseGravity(gravityOrientation, positionObject);
        }
        //here apply attractor gravity
        else
        {
            finalGravity += AirAttractor(gravityOrientation, positionObject);
        }
        */
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
            rb.velocity = FindAirGravity(rb.transform.position, rb.velocity, GetMainAndOnlyGravity());
    }

    private void FixedUpdate()
    {
        ChangeStateGravity();
        CalculateGravity(rb.transform.position);

        ApplyGroundGravity();
        ApplySuplementGravity();
        ApplyAirGravity();

        SaveLastPositionOnground();

        ExtDrawGuizmos.DebugWireSphere(rb.transform.position, Color.red, 0.1f, 5f);
    }
}
