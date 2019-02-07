using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttractor : MonoBehaviour
{
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private PlayerGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityController entityController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityJump entityJump;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntitySwitch entitySwitch;

    [FoldoutGroup("Air Attractor"), SerializeField, Tooltip("")]
    private float timeBeforeActiveAttractorInAir = 0.8f;
    [FoldoutGroup("Air Attractor"), SerializeField, Tooltip("")]
    private float timeBeforeActiveLateAttractor = 1.4f;

    [FoldoutGroup("Air Attractor"), Tooltip("default air gravity"), SerializeField]
    private float gravityAttractor = 2f;
    [FoldoutGroup("Air Attractor"), Tooltip("default air gravity"), SerializeField]
    private float speedLerpAttractor = 5f;

    [FoldoutGroup("Air Attractor"), SerializeField, Tooltip("ref script")]
    private float distAllowedForNormalGravity = 10f;
    [FoldoutGroup("Air Attractor"), SerializeField, Tooltip("raycast to ground layer")]
    private float distSpherecastForLightAttractor = 5f;
    [FoldoutGroup("Air Attractor"), SerializeField, Tooltip("raycast to ground layer")]
    private float radiusSphereCastForLightAttractor = 0.3f;

    [FoldoutGroup("Air Attractor"), Tooltip("position de l'attractpoint"), SerializeField]
    public float lengthPositionAttractPoint = 1f;    //position de l'attract point par rapport à la dernier position / normal
    [FoldoutGroup("Air Attractor"), Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float sizeDistanceForSavePlayerPos = 0.5f;   //a-t-on un attract point de placé ?

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref script"), ReadOnly]
    private Vector3[] worldLastPosition = new Vector3[3];      //save la derniere position grounded...
    [FoldoutGroup("Debug"), Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float differenceAngleNormalForUpdatePosition = 5f;   //a-t-on un attract point de placé ?
    [FoldoutGroup("Debug"), Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float timeDebugFlyAway = 0.3f;   //a-t-on un attract point de placé ?


    private FrequencyCoolDown timerBeforeCreateAttractor = new FrequencyCoolDown();
    private FrequencyCoolDown timerBeforeCreateLateAttractor = new FrequencyCoolDown();
    private Vector3 worldPreviousNormal;    //et sa dernière normal accepté par le changement d'angle
    private Vector3 worldLastNormal;        //derniere normal enregistré, peut import le changement position/angle
    private Vector3 transformPointAttractor = Vector3.zero;
    private float gravityAttractorLerp = 1f;
    private FrequencyCoolDown timerDebugFlyAway = new FrequencyCoolDown();

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        FillAllPos();
    }

    /// <summary>
    /// fill the array at start
    /// </summary>
    private void FillAllPos()
    {
        for (int i = 0; i < worldLastPosition.Length; i++)
        {
            worldLastPosition[i] = entityController.rb.transform.position;
        }
    }

    /// <summary>
    /// Set the last position
    /// decalle other positions down
    /// </summary>
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

    /// <summary>
    /// save the last position on ground
    /// </summary>
    public void SaveLastPositionOnground()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            return;

        worldLastNormal = playerGravity.GetMainAndOnlyGravity();   //avoir toujours une normal à jour
        float distForSave = (WorldLastPositionGetIndex(0) - entityController.rb.transform.position).sqrMagnitude;

        //Debug.Log("dist save: " + distForSave);
        //si la distance entre les 2 point est trop grande, dans tout les cas, save la nouvelle position !
        if (distForSave > sizeDistanceForSavePlayerPos)
        {
            WorldLastPositionSet(entityController.rb.transform.position); //save la position onGround
            ExtDrawGuizmos.DebugWireSphere(WorldLastPositionGetIndex(0), Color.red, 0.5f, 1f);
        }
        //si la normal à changé, update la position + normal !
        else if (worldPreviousNormal != worldLastNormal)
        {
            //ici changement de position SEULEMENT si l'angle de la normal diffère de X
            float anglePreviousNormal = ExtQuaternion.GetAngleFromVector3(worldPreviousNormal, entityController.rbRotateObject.up);
            float angleNormalPlayer = ExtQuaternion.GetAngleFromVector3(worldLastNormal, entityController.rbRotateObject.up);
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
                WorldLastPositionSet(entityController.rb.transform.position); //save la position onGround
                worldPreviousNormal = worldLastNormal;

                ExtDrawGuizmos.DebugWireSphere(WorldLastPositionGetIndex(0), Color.yellow, 0.5f, 1f);
                Debug.DrawRay(entityController.rb.transform.position, worldPreviousNormal, Color.yellow, 1f);
            }
        }
    }

    public void OnGrounded()
    {
        timerBeforeCreateAttractor.Reset();
        timerBeforeCreateLateAttractor.Reset();
        ResetFlyAway();
        CreateAttractor();
    }

    public bool CanCreateAttractor()
    {
        return (timerBeforeCreateAttractor.IsStartedAndOver());
    }
    public bool CanCreateLateAttractor()
    {
        return (timerBeforeCreateLateAttractor.IsStartedAndOver());
    }

    /// <summary>
    /// get vector director of attractor for the new physics direction
    /// </summary>
    /// <returns></returns>
    public Vector3 GetDirAttractor(Vector3 positionEntity)
    {
        Vector3 dirAttractor = positionEntity - transformPointAttractor;
        return (dirAttractor);
    }

    /// <summary>
    /// called when jump
    /// </summary>
    public void CreateAttractor()
    {
        ExtLog.DebugLogIa("create attractor !", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
        timerBeforeCreateAttractor.StartCoolDown(timeBeforeActiveAttractorInAir);
        timerBeforeCreateLateAttractor.StartCoolDown(timeBeforeActiveLateAttractor);

        transformPointAttractor = WorldLastPositionGetIndex(1) - worldLastNormal * lengthPositionAttractPoint;

        //ExtDrawGuizmos.DebugWireSphere(transformPointAttractor, Color.white, 1f, 1f);

        //ExtDrawGuizmos.DebugWireSphere(WorldLastPositionGetIndex(1), Color.red, 1f, 2f);          //ancienne pos
        Debug.Log("ici create ?");

        //Debug.DrawRay(entityController.rb.transform.position, groundCheck.GetDirLastNormal(), Color.black, 0.5f);


        //ExtDrawGuizmos.DebugWireSphere(transformPointAttractor, Color.blue, 0.5f, 2f);      //nouvel pos
        //Debug.DrawRay(WorldLastPositionGetIndex(0), worldLastNormal * 4, Color.red, 2f);      //last normal

        //Debug.Break();
    }

    /// <summary>
    /// create an attractor point for entity gravity !
    /// </summary>
    public void ActiveAttractor()
    {
        playerGravity.SetOrientation(PlayerGravity.OrientationPhysics.ATTRACTOR);

        ExtLog.DebugLogIa("attractor activated !", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);

        //camera change only of this is have player
        if (entityController.isPlayer)
        {
            PhilaeManager.Instance.cameraController.SetAttractorCamera();
        }

        //RESET LERP !!! important !
        gravityAttractorLerp = 1;
    }

    /// <summary>
    /// apply attractor gravity
    /// </summary>
    public Vector3 AirAttractor(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        gravityAttractorLerp = Mathf.Lerp(gravityAttractorLerp, gravityAttractor, Time.fixedDeltaTime * speedLerpAttractor);

        Vector3 forceAttractor = -gravityOrientation * playerGravity.Gravity * (gravityAttractorLerp - 1) * Time.fixedDeltaTime;

        Debug.DrawRay(positionEntity, forceAttractor, Color.white, 5f);
        return (forceAttractor);
    }

    /// <summary>
    /// active attractor if we are far away !
    /// </summary>
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
            && playerGravity.GetOrientationPhysics() == PlayerGravity.OrientationPhysics.NORMALS && !timerDebugFlyAway.IsRunning())
        {
            Debug.Log("mettre le timer du mal");
            timerDebugFlyAway.StartCoolDown(timeDebugFlyAway);
        }
    }

    /// <summary>
    /// reset far away when we are on ground
    /// </summary>
    public void ResetFlyAway()
    {
        timerDebugFlyAway.Reset();
    }

    private void FixedUpdate()
    {
        SaveLastPositionOnground();
        DebugFlyAway();
    }
}
