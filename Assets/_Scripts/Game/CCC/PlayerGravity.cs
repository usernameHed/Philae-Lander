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

    public struct InfoJump
    {
        public bool didWeHit;
        public Vector3 pointHit;
        public Vector3 normalHit;
        public Transform objHit;
        public Vector3 dirUltimatePlotPoint;
        public Vector3 ultimatePlotPoint;

        public void SetDirLast(Vector3[] plots)
        {
            Vector3 penultimate = plots[plots.Length - 2];
            Vector3 ultimate = plots[plots.Length - 1];
            
            ultimatePlotPoint = ultimate;
            dirUltimatePlotPoint = ultimate - penultimate;
        }

        public void Clear()
        {
            didWeHit = false;
            pointHit = Vector3.zero;
            normalHit = Vector3.zero;
            objHit = null;
            dirUltimatePlotPoint = Vector3.zero;
            ultimatePlotPoint = Vector3.zero;
        }
    }

    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float gravity = 9.81f;
    public float Gravity { get { return (gravity); } }
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    private float magicTrajectoryCorrection = 1f;
    //[FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    //private float magicTrajectoryCorrectionRatio = 1f;


    

    [FoldoutGroup("Ground Gravity"), Tooltip("Add gravity when releasing jump button, and rigidbody is going UPward the planet"), SerializeField]
    private float groundAddGravity = 45f;
    [FoldoutGroup("Ground Gravity"), Tooltip("Down gravity when we are falling into the planet"), SerializeField]
    private float stickToFloorGravity = 6f;

    [FoldoutGroup("Jump Gravity"), SerializeField, Tooltip("raycast to ground layer")]
    private string[] layersRaycast;
    [FoldoutGroup("Jump Gravity"), SerializeField, Tooltip("raycast to ground layer")]
    private float distRaycastForNormalSwitch = 5f;
    [FoldoutGroup("Jump Gravity"), SerializeField, Tooltip("MUST PRECEED AIR ATTRACTOR TIME !!")]
    private float timeBeforeTestingNewNormalGravity = 0.4f;

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
    [FoldoutGroup("Air Attractor"), SerializeField, Tooltip("raycast to ground layer")]
    private float distSpherecastForLightAttractor = 5f;
    [FoldoutGroup("Air Attractor"), SerializeField, Tooltip("raycast to ground layer")]
    private float radiusSphereCastForLightAttractor = 0.3f;

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
    [FoldoutGroup("Debug"), Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float timeDebugFlyAway = 0.3f;   //a-t-on un attract point de placé ?

    private Vector3 mainAndOnlyGravity = Vector3.zero;
    private FrequencyCoolDown timerBeforeCreateAttractor = new FrequencyCoolDown();
    private FrequencyCoolDown timerBeforeTestingNewNormalGravity = new FrequencyCoolDown();
    private FrequencyCoolDown timerDebugFlyAway = new FrequencyCoolDown();
    private InfoJump infoJump = new InfoJump();
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
    private Vector3 mainAttractPoint;
    public Transform GetMainAttractObject() { return (mainAttractObject); }
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script"), ReadOnly]
    private Vector3[] worldLastPosition = new Vector3[3];      //save la derniere position grounded...

    private bool isOnTransition = false;
    private bool normalGravityTested = false;   //know if we are in the 0.5-0.8 sec between norma and attractor
    private bool dontApplyForceDownForThisRound = false;
    private bool applyStrongAttractor = true;
    private float gravityAttractorLerp = 1f;
    private Vector3 worldPreviousNormal;    //et sa dernière normal accepté par le changement d'angle
    private Vector3 worldLastNormal;        //derniere normal enregistré, peut import le changement position/angle
    private RaycastHit hitInfo;
    private int layerMask = Physics.AllLayers;

    private void Awake()
    {
        layerMask = LayerMask.GetMask(objOnSight);
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
            mainAttractPoint = mainAttractObject.position;
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

        Debug.DrawRay(rb.transform.position, groundCheck.GetDirLastNormal(), Color.black, 0.5f);
        

        ExtDrawGuizmos.DebugWireSphere(transformPointAttractor, Color.blue, 1f, 2f);      //nouvel pos
        Debug.DrawRay(WorldLastPositionGetIndex(0), worldLastNormal * 4, Color.red, 2f);      //last normal

        //Debug.Break();
    }


    public void OnGrounded()
    {
        timerDebugFlyAway.Reset();
        dontApplyForceDownForThisRound = false;
        normalGravityTested = false;
        timerBeforeCreateAttractor.Reset();
        timerBeforeTestingNewNormalGravity.Reset();

        CreateAttractor();
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

    /// <summary>
    /// calculate trajectory of entity
    /// </summary>
    public Vector3[] Plot(Rigidbody rigidbody, Vector3 pos, Vector3 velocity, int steps, bool noForceWhenUp, bool noForceWhenDown)
    {
        Vector3[] results = new Vector3[steps];

        float timestep = Time.fixedDeltaTime / magicTrajectoryCorrection;
        gravityAttractorLerp = 1f;
        
        float drag = 1f - timestep * rigidbody.drag;
        Vector3 moveStep = velocity * timestep;

        int i = -1;
        while (++i < steps)
        {
            CalculateGravity(pos);
            Vector3 gravityAccel = FindAirGravity(pos, moveStep, GetMainAndOnlyGravity(), noForceWhenUp, noForceWhenDown, true) * timestep;
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results[i] = pos;
            ExtDrawGuizmos.DebugWireSphere(pos, Color.white, 0.1f, 0.1f);
        }
        return (results);
    }

    /// <summary>
    /// do raycast along the Plots, and reutrn true if we hit
    /// </summary>
    /// <param name="infoPlot"></param>
    /// <returns></returns>
    public bool DoRaycast(Vector3[] infoPlot, int depth = 2)
    {
        Vector3 prevPos = rb.transform.position;

        for (int i = 0; i <= depth; i++)
        {
            Vector3 dirRaycast;
            Vector3 lastPoint;

            if (i == depth)
            {
                dirRaycast = infoJump.dirUltimatePlotPoint.normalized * distRaycastForNormalSwitch;
                lastPoint = infoJump.ultimatePlotPoint;
                //lastPoint pas utile de le mettre ?
            }
            else
            {
                int indexPoint = ((infoPlot.Length / depth) * (i + 1)) - 1;
                Debug.Log("index: " + indexPoint + "(max: " + infoPlot.Length);
                lastPoint = infoPlot[indexPoint];

                dirRaycast = lastPoint - prevPos;
            }

            Debug.DrawRay(prevPos, dirRaycast, Color.red, 5f);
            if (Physics.SphereCast(prevPos, 0.3f, dirRaycast, out hitInfo,
                                   dirRaycast.magnitude, layerMask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("find something ! keep going with normal gravity");
                infoJump.didWeHit = true;
                infoJump.normalHit = hitInfo.normal;
                infoJump.objHit = hitInfo.transform;
                infoJump.pointHit = hitInfo.point;
                Debug.DrawRay(hitInfo.point, infoJump.normalHit, Color.magenta, 5f);
                return (true);
            }
            prevPos = lastPoint;
        }
        return (false);
    }

    /// <summary>
    /// we just jump
    /// if we hit something in the plot and raycast: pass to OBJECT ORIENTED (a kind of swithc planet)
    /// </summary>
    public void JustJump()
    {
        //reset jump first test timer
        timerBeforeTestingNewNormalGravity.StartCoolDown(timeBeforeTestingNewNormalGravity);
        normalGravityTested = false;
        dontApplyForceDownForThisRound = false;

        //first create 30 plot of the normal jump
        Vector3[] plots = Plot(rb, rb.transform.position, rb.velocity, 30, false, true);
        infoJump.Clear();
        infoJump.SetDirLast(plots);

        bool hit = DoRaycast(plots);    //return true if we hit a wall in the first jump plot
        //if (!hit)
        //    hit = ForwardRaycastJump(plots);    //return true if we hit a wall in the long raycast after the 30 plots

        //here, we hit something at some point !
        if (hit)
        {
            Vector3 normalJump = GetMainAndOnlyGravity();
            Vector3 normalHit = infoJump.normalHit;

            float dotImpact = ExtQuaternion.DotProduct(normalJump, normalHit);
            if (dotImpact > 0)
            {
                Debug.Log("we hit way !");
                currentOrientation = OrientationPhysics.OBJECT;
                mainAttractObject = infoJump.objHit;
                mainAttractPoint = infoJump.pointHit;
            }
            else
            {
                Debug.Log("No way we climb That !, Obstacle to inclined");
            }

        }
        else
        {
            Debug.Log("no hit... fack");
        }
        //Debug.Break();
    }

    /// <summary>
    /// do an ultime test of PLOT / raycast, if we find something: switch to Object oriented
    /// else, we will pass soon on Attractor...
    /// </summary>
    private void UltimaTestBeforeAttractor()
    {
        if (normalGravityTested)
            return;
        normalGravityTested = true;

        Vector3 ultimate = infoJump.ultimatePlotPoint;
        Vector3 dirUltimate = infoJump.dirUltimatePlotPoint;

        //chose if we add force or not
        Debug.Log("ultimate raycast");

        infoJump.Clear();
        //create plot WITH force down
        Vector3[] plots = Plot(rb, rb.transform.position, dirUltimate.normalized * rb.velocity.magnitude, 16, false, true);

        infoJump.SetDirLast(plots);

        bool hit = DoRaycast(plots, 1);    //return true if we hit a wall in the first jump plot

        if (!hit)
        {
            infoJump.Clear();
            //create plot WITOUT force down
            plots = Plot(rb, rb.transform.position, dirUltimate.normalized * rb.velocity.magnitude, 30, false, false);
            infoJump.SetDirLast(plots);

            hit = DoRaycast(plots, 1);
            if (hit)
            {
                Debug.Log("hit the long run ! desactive force down for this one !");
                dontApplyForceDownForThisRound = true;
            }
        }

        if (hit)
        {
            currentOrientation = OrientationPhysics.OBJECT;
            mainAttractObject = infoJump.objHit;
            mainAttractPoint = infoJump.pointHit;
        }


        //raycast

        //currentOrientation = OrientationPhysics.OBJECT;
        //Debug.Break();
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

    private bool RaycastForward(Vector3 pos, Vector3 dir, float length, float radius)
    {
        Debug.DrawRay(pos, dir * length, Color.cyan, 5f);

        if (Physics.SphereCast(pos, radius, dir, out hitInfo,
                                length, layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("ATTRACTOR find something in stage 2 ! normal gravity !!");
            return (true);
        }
        return (false);
    }

    private void SetupAttractor()
    {
        dontApplyForceDownForThisRound = false;
        Vector3 lastPos = Plot(rb, rb.transform.position, rb.velocity, 15, true, true)[14];

        

        Vector3 dirRaycast = lastPos - rb.transform.position;
        Debug.DrawRay(rb.transform.position, dirRaycast, Color.red, 5f);
        if (Physics.SphereCast(rb.transform.position, 0.3f, dirRaycast, out hitInfo,
                                dirRaycast.magnitude, layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("ATTRACTOR find something ! keep going with normal gravity");
            applyStrongAttractor = false;
        }
        else
        {
            Vector3 dirNewRaycast = GetMainAndOnlyGravity() * -1;
            bool hit = RaycastForward(lastPos, dirNewRaycast, distSpherecastForLightAttractor, radiusSphereCastForLightAttractor);
            if (!hit)
            {
                Vector3 rightGravity = ExtQuaternion.CrossProduct(dirNewRaycast, -rbRotate.transform.right);
                Vector3 middleRaycastAndGravity = ExtQuaternion.GetMiddleOf2Vector(dirNewRaycast, rightGravity);
                
                hit = RaycastForward(lastPos, middleRaycastAndGravity, distSpherecastForLightAttractor, radiusSphereCastForLightAttractor);
                if (hit)
                {
                    //Debug.LogError("we made it !");
                }
            }
            applyStrongAttractor = !hit;
        }
                
        ActiveAttractor();
        //Debug.Break();
    }

    private void ChangeStateGravity()
    {
        //here player is on fly
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            //Debug.Log("try to change gravity state");
            //here player is on fly, and we can create an attractor

            if (timerBeforeTestingNewNormalGravity.IsStartedAndOver() && currentOrientation == OrientationPhysics.NORMALS)
            {
                UltimaTestBeforeAttractor();
            }
            else if (timerBeforeCreateAttractor.IsStartedAndOver() && currentOrientation == OrientationPhysics.NORMALS)
            {
                SetupAttractor();
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
        if (currentOrientation != OrientationPhysics.NORMALS)
        {
            timerDebugFlyAway.Reset();
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
            mainAttractPoint = rbTransform.position;
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
                Vector3 direction = positionEntity - mainAttractPoint;
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
        
        Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, orientationDown, Color.blue, 5f);
        return (orientationDown);
        
        //return (Vector3.zero);
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
    private Vector3 AirAttractor(Vector3 gravityOrientation, Vector3 positionEntity, bool applyStrongAttractor)
    {
        //gravityAttractorLerp = Mathf.Lerp(gravityAttractorLerp, gravityAttractorMinMax.x, Time.fixedDeltaTime * speedLerpAttractorMinMax.x);
        
        if (applyStrongAttractor)
        {
            gravityAttractorLerp = Mathf.Lerp(gravityAttractorLerp, gravityAttractorMinMax.x, Time.fixedDeltaTime * speedLerpAttractorMinMax.x);
        }
        else
        {
            gravityAttractorLerp = Mathf.Lerp(gravityAttractorLerp, gravityAttractorMinMax.y, Time.fixedDeltaTime * speedLerpAttractorMinMax.y);
        }

        //gravityAttractorLerp = defaultGravityInAir;

        Vector3 forceAttractor = -gravityOrientation * gravity * (gravityAttractorLerp - 1) * Time.fixedDeltaTime;

        Debug.DrawRay(positionEntity, forceAttractor, Color.white, 5f);
        return (forceAttractor);
    }

    /// <summary>
    /// return the right gravity
    /// </summary>
    /// <returns></returns>
    private Vector3 FindAirGravity(Vector3 positionObject, Vector3 rbVelocity, Vector3 gravityOrientation, bool noForceWhenUp, bool noForceWhenDown, bool applyStrongAttractor)
    {
        Vector3 finalGravity = rbVelocity;

        if (currentOrientation == OrientationPhysics.ATTRACTOR)
        {
            finalGravity += AirAttractor(gravityOrientation, positionObject, applyStrongAttractor);
        }
        else
        {
            float dotGravityRigidbody = ExtQuaternion.DotProduct(gravityOrientation, rbVelocity);
            //here we fall down toward a planet, apply gravity down
            if (dotGravityRigidbody < 0 && currentOrientation != OrientationPhysics.ATTRACTOR)
            {
                if (noForceWhenDown)
                    finalGravity += AirAddGoingDown(gravityOrientation, positionObject);
            }
            //here we are going up, and we release the jump button, apply gravity down until the highest point
            else if (dotGravityRigidbody > 0 && !entityAction.Jump && currentOrientation != OrientationPhysics.ATTRACTOR)
            {
                if (!noForceWhenUp)
                    finalGravity += AirAddGoingUp(gravityOrientation, positionObject);
            }
            //Debug.Log("air gravity");
            //here, apply base gravity when we are InAir

            finalGravity += AirBaseGravity(gravityOrientation, positionObject);

        }
        return (finalGravity);
    }

    private void DebugFlyAway()
    {
        if (timerDebugFlyAway.IsStartedAndOver())
        {
            Debug.LogError("ok on est dans le mal !");
            timerDebugFlyAway.Reset();
            ActiveAttractor();
            return;
        }
        if (!entityJump.HasJumped && entityController.GetMoveState() == EntityController.MoveState.InAir
            && currentOrientation == OrientationPhysics.NORMALS && !timerDebugFlyAway.IsRunning())
        {
            Debug.Log("mettre le timer du mal");
            timerDebugFlyAway.StartCoolDown(timeDebugFlyAway);
        }
    }

    /// <summary>
    /// apply every gravity force in Air
    /// </summary>
    private void ApplyAirGravity()
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return;

        //if (currentOrientation != OrientationPhysics.ATTRACTOR)
        rb.velocity = FindAirGravity(rb.transform.position, rb.velocity, GetMainAndOnlyGravity(), true, !dontApplyForceDownForThisRound, applyStrongAttractor);
    }

    private void FixedUpdate()
    {
        ChangeStateGravity();
        CalculateGravity(rb.transform.position);

        ApplyGroundGravity();
        ApplySuplementGravity();
        ApplyAirGravity();

        SaveLastPositionOnground();

        DebugFlyAway();

        //ExtDrawGuizmos.DebugWireSphere(rb.transform.position, Color.red, 0.1f, 5f);
    }
}
