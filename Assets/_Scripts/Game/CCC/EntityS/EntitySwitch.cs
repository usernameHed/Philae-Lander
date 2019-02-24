using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySwitch : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float radiusOverlap = 2f;
    [FoldoutGroup("Switch"), Tooltip("default air gravity"), SerializeField]
    private float speedRotateWhenSwitching = 30f;
    [FoldoutGroup("Switch"), Tooltip("marge de précision de la caméra sur sa cible"), SerializeField]
    private float timeBeforeResetBaseCamera = 0.4f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float timeBeforeStartOverlapping = 0.3f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private string[] layerSwitch = new string[] { "Walkable/Up"};
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("ref rigidbody")]
    private float dotRangeCeilling = 0.5f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityController entityController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityRotateToGround rotateToGround;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private GroundCheck groundCheck;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("")]
    private bool isOnTransition = false;


    private Collider[] results = new Collider[5];
    private FrequencyCoolDown coolDownOverlap = new FrequencyCoolDown();
    private int layermask;
    private static float radiusSphereCast = 0.3f;

    private void Awake()
    {
        layermask = LayerMask.GetMask(layerSwitch);
    }

    private void OverlapTest()
    {
        int number = Physics.OverlapSphereNonAlloc(entityController.rb.position, radiusOverlap, results, layermask, QueryTriggerInteraction.Ignore);
        //ExtDrawGuizmos.DebugWireSphere(entityController.rb.position, Color.blue, radiusOverlap, 0.1f);
        for (int i = 0; i < number; i++)
        {
            //if (groundCheck.GetLastPlatform() && results[i].transform.GetInstanceID() == groundCheck.GetLastPlatform().GetInstanceID())
            //    continue;

            Vector3 dirPlayerObject = results[i].transform.position - entityController.rb.position;
            Vector3 gravityOrientation = playerGravity.GetMainAndOnlyGravity();

            float dotPlayer = ExtQuaternion.DotProduct(dirPlayerObject, gravityOrientation);
            if (dotPlayer > dotRangeCeilling)
            {
                Debug.Log("cet objet est dans la direction oposé de l'autre !");
                //Vector3 closestPos = results[i].transform.G

                Debug.DrawRay(entityController.rb.position, gravityOrientation * radiusOverlap, Color.cyan, 5f);

                RaycastHit hitInfo;
                Vector3 posGravity = results[i].transform.position;
                Vector3 normalHit = -gravityOrientation;
                if (Physics.SphereCast(entityController.rb.position, 0.3f, gravityOrientation, out hitInfo,
                               radiusOverlap + 0.1f, layermask, QueryTriggerInteraction.Ignore))
                {
                    posGravity = hitInfo.point;
                    normalHit = hitInfo.normal;
                }
                ChangeMainAttractObject(results[i].transform, posGravity, normalHit);
             }
        }
    }

    private void ChangeMainAttractObject(Transform obj, Vector3 pointHit, Vector3 normalHit)
    {
        if (entityController.isPlayer)
        {
            PhilaeManager.Instance.cameraController.SetChangePlanetCam();
            PhilaeManager.Instance.PlanetChange();
        }

        playerGravity.SetObjectAttraction(obj, pointHit, normalHit);

        rotateToGround.SetNewTempSpeed(speedRotateWhenSwitching);

        //CalculateGravity(rb.transform.position);

        

        entityController.SetKinematic(true);
        ExtLog.DebugLogIa("change planete", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
        isOnTransition = true;
        Invoke("UnsetKinematic", timeBeforeResetBaseCamera);
        Debug.Break();
    }

    private void UnsetKinematic()
    {
        entityController.SetKinematic(false);
        PhilaeManager.Instance.cameraController.SetBaseCamera();
    }

    public void JustJumped()
    {
        coolDownOverlap.StartCoolDown(timeBeforeStartOverlapping);
    }

    /// <summary>
    /// called onGround, or when we active attractor ?
    /// </summary>
    public void OnGrounded()
    {
        coolDownOverlap.Reset();
        if (isOnTransition)
        {
            //ExtLog.DebugLogIa("stop transition !", (entityController.isPlayer) ? ExtLog.Log.BASE : ExtLog.Log.IA);
            Debug.Log("stop transition");
            isOnTransition = false;
        }
    }

    private void FixedUpdate()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir
             && coolDownOverlap.IsReady() && !isOnTransition)
        {
            OverlapTest();
        }            
    }
}
