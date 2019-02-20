using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("check if player in the ground")]
public class GroundCheck : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Range(0f, 2f), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private float groundCheckDistance = 0.2f;
    [FoldoutGroup("GamePlay"), Range(0f, 2f), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private float stickToFloorDist = 0.6f;
    [FoldoutGroup("GamePlay"), Range(0f, 2f), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private float stickToCeillingDist = 0.6f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float sizeRadiusRayCast = 0.5f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] dontLayer = new string[] { "Walkable/Dont" };

    [FoldoutGroup("Object"), SerializeField]
    private SphereCollider sphereCollider;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityGravity playerGravity;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityJump entityJump;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityController entityController;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private FastForward fastForward;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch;

    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityNoGravity entityNoGravity;

    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isGrounded = false;
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isAlmostGrounded = false;
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isFlying = true;
    
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private string currentFloorLayer;
    [FoldoutGroup("Debug"), Tooltip("reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)"), SerializeField]
    public float collRayCastMargin = 0.1f;
    [FoldoutGroup("Debug"), SerializeField, ReadOnly]
    private Transform lastPlatform = null;
    public Transform GetLastPlatform() { return (lastPlatform); }

    private float radius;
    private Vector3 dirNormal = Vector3.zero;
    private Vector3 dirSurfaceNormal = Vector3.zero;
    

    private void Awake()
    {
        isGrounded = isAlmostGrounded = false;
        isFlying = true;
        radius = sphereCollider.radius;
    }

    public string GetLastLayer()
    {
        return (currentFloorLayer);
    }

    public bool IsSafeGrounded()
    {
        return (!isFlying);
    }
    public bool IsFlying()
    {
        return (isFlying);
    }
    public bool IsAlmostGrounded()
    {
        return (isAlmostGrounded && !isGrounded && !isFlying);
    }
    
    public Vector3 GetDirLastNormal()
    {
        return (dirNormal);
    }
    public Vector3 GetDirLastSurfaceNormal()
    {
        return (dirSurfaceNormal);
    }

    public void SetForwardWall(RaycastHit hitInfo)
    {
        dirNormal = hitInfo.normal;
        SetCurrentPlatform(hitInfo.transform);
        isGrounded = true;
        isFlying = false;
    }
    public void SetBackwardWall(RaycastHit hitInfo)
    {
        dirNormal = hitInfo.normal;
        SetCurrentPlatform(hitInfo.transform);
        isGrounded = true;
        isFlying = false;
    }

    private bool IsInDontLayer(RaycastHit hitInfo)
    {
        int isForbidden = ExtList.ContainSubStringInArray(dontLayer, LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
        if (isForbidden != -1)
            return (true);
        return (false);
    }

    private bool CanChangeNormal(RaycastHit hitInfo)
    {
        if (!fastForward.CanChangeNormal(hitInfo, dirSurfaceNormal))
            return (false);

        return (true);
    }

    private void SetCurrentPlatform(Transform platform)
    {
        lastPlatform = platform;
        currentFloorLayer = LayerMask.LayerToName(platform.gameObject.layer);
    }

    /// <summary>
    /// Set isGrounded
    /// sphere cast down just beyond the bottom of the capsule to see/
    /// if the capsule is colliding round the bottom
    /// </summary>
    private void GroundChecking(float magnitudeToCheck, ref bool groundValue, Vector3 dirRay)
    {
        if (entityJump.IsJumpedAndNotReady())
        {
            return;
        }
        //isGrounded = false;
        //return;

        RaycastHit hitInfo;

        //Vector3 dirRaycast = playerGravity.GetMainAndOnlyGravity() * (radius + magnitudeToCheck);
        //Debug.DrawRay(rb.transform.position, dirRaycast * -1, Color.blue, 0.1f);
        if (Physics.SphereCast(rb.transform.position, sizeRadiusRayCast, dirRay, out hitInfo,
                               magnitudeToCheck, entityController.layerMask, QueryTriggerInteraction.Ignore))
        {
            //TODO: here if gravityAttractorLayer, qu'on est dans un SphereAirMove mode, et que la normal
            // n'est pas bonne, ne pas être considéré comme grounded !
            

            //try to set 
            if (!lastPlatform || hitInfo.collider.transform.GetInstanceID() != lastPlatform.GetInstanceID())
            {
                Debug.Log("try to change ??");
                entityGravityAttractorSwitch.TryToSetNewGravityAttractor(hitInfo.collider.transform);
            }
            else if (lastPlatform && entityGravityAttractorSwitch.IsInGravityAttractorMode()
                && !entityGravityAttractorSwitch.IsAirAttractorLayer(hitInfo.collider.gameObject.layer))
            {
                Debug.LogWarning("HEre we detect we are in gravityMode, AND on a different layer platform... force change !");
                entityGravityAttractorSwitch.TryToSetNewGravityAttractor(hitInfo.collider.transform);
            }
                

            if (IsInDontLayer(hitInfo))
            {
                Debug.Log("continiue flying... we are in dont zone");
                return;
            }

            if (!entityGravityAttractorSwitch.IsNormalAcceptedIfWeAreInGA(hitInfo.transform, hitInfo.normal))
            {
                Debug.Log("here sphereAirMove tell us we are in a bad normal, continiue to fall");
                return;
            }
            

            SetCurrentPlatform(hitInfo.collider.transform);

            groundValue = true;

            dirSurfaceNormal = ExtUtilityFunction.GetSurfaceNormal(rb.transform.position,
                playerGravity.GetMainAndOnlyGravity() * -0.01f,
                groundCheckDistance,
                sizeRadiusRayCast,
                hitInfo.point,
                collRayCastMargin,
                entityController.layerMask);

            if (CanChangeNormal(hitInfo))
            {
                dirNormal = hitInfo.normal;
            }
        }
        else
        {
            groundValue = false;
            dirNormal = playerGravity.GetMainAndOnlyGravity() * 1;
        }
    }
    
    /// <summary>
    /// set the drag, and stick to ground if needed
    /// </summary>
    private void SetDragAndStick()
    {
        if (isGrounded)
        {
            isAlmostGrounded = false;
        }
        else
        {
            GroundChecking(stickToFloorDist, ref isAlmostGrounded, playerGravity.GetMainAndOnlyGravity() * -0.01f);
            if (!isAlmostGrounded && !entityNoGravity.IsBaseOrMoreRatio())
            {
                GroundChecking(stickToCeillingDist, ref isGrounded, playerGravity.GetMainAndOnlyGravity() * 0.01f);
            }

        }
    }

    /// <summary>
    /// set if we are flying or not !
    /// </summary>
    private void SetFlying()
    {
        if (entityJump.IsJumpedAndNotReady())
        {
            isGrounded = false;
            isAlmostGrounded = false;
        }

        if (isGrounded || isAlmostGrounded)
        {
            isFlying = false;
            return;
        }
        isFlying = true;
    }

    private void FixedUpdate()
    {
        GroundChecking(groundCheckDistance, ref isGrounded, playerGravity.GetMainAndOnlyGravity() * -0.01f);           //set whenever or not we are grounded
        SetDragAndStick();          //set, depending on the grounded, the drag, and stick or not to the floor
        SetFlying();                //set if we fly or not !
    }
}
