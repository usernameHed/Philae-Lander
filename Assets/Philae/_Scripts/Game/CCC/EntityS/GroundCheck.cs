
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;
using UnityEssentials.PropertyAttribute.readOnly;
using UnityEssentials.time;

public class GroundCheck : MonoBehaviour
{
    [Range(0f, 2f), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private float groundCheckDistance = 0.2f;
    [Range(0f, 2f), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private float stickToFloorDist = 0.6f;
    [Range(0f, 2f), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private float stickToCeillingDist = 0.6f;
    [Tooltip(""), SerializeField]
    public float sizeRadiusRayCast = 0.5f;
    [Tooltip(""), SerializeField]
    public float timeInAirBeforeNotStick = 0.3f;
    [Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private bool calculateSurfaceNormal = true;

    [SerializeField]
    private SphereCollider sphereCollider = null;
    [Tooltip(""), SerializeField]
    private Rigidbody rb = null;
    [Tooltip(""), SerializeField]
    private BaseGravity baseGravity = null;
    [Tooltip(""), SerializeField]
    private EntityJump entityJump = null;
    [Tooltip(""), SerializeField]
    private EntityController entityController = null;
    [Tooltip(""), SerializeField]
    private BaseGravityAttractorSwitch baseGravityAttractorSwitch = null;
    [Tooltip(""), SerializeField]
    private EntityNoGravity entityNoGravity = null;
    [Tooltip(""), SerializeField]
    private FastForward fastForward = null;

    [ReadOnly, SerializeField]
    private bool isGrounded = false;
    [ReadOnly, SerializeField]
    private bool isAlmostGrounded = false;
    [ReadOnly, SerializeField]
    private bool isFlying = true;
    
    [ReadOnly, SerializeField]
    private string currentFloorLayer;
    [Tooltip("reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)"), SerializeField]
    public float collRayCastMargin = 0.1f;
    [SerializeField, ReadOnly]
    private Transform lastPlatform = null;
    public Transform GetLastPlatform() { return (lastPlatform); }

    private float radius;
    [SerializeField, ReadOnly]
    private Vector3 dirNormal = Vector3.zero;
    private Vector3 dirSurfaceNormal = Vector3.zero;

    private FrequencyCoolDown coolDownForStick = new FrequencyCoolDown();
    private Vector3 pointHit;

    private void Awake()
    {
        isGrounded = isAlmostGrounded = false;
        isFlying = true;
        radius = sphereCollider.radius;
        coolDownForStick.Reset();

        ResearchInitialGround(true);
    }

    public Vector3 ResearchInitialGround(bool setGAGravity = true)
    {
        RaycastHit hit;
        //int raycastLayerMask = LayerMask.GetMask(entityController.walkablePlatform);
        Vector3 dirDown = rb.transform.up * -1;
        Debug.DrawRay(rb.transform.position, dirDown, Color.magenta, 5f);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(rb.transform.position, dirDown, out hit, Mathf.Infinity, entityController.layerMask))
        {
            if (setGAGravity)
            {
                dirNormal = hit.normal;
                Debug.Log("Did Hit");
                baseGravityAttractorSwitch.SetDefaultGAgravity(hit.point, dirNormal);
            }
            return (hit.point);
        }
        else
        {
            if (setGAGravity)
            {
                dirNormal = rb.transform.up;
                Debug.Log("Did NO Hit");
                baseGravityAttractorSwitch.SetDefaultGAgravity(rb.position - dirNormal * 9999, dirNormal);
            }
            return (rb.position - rb.transform.up * 9999);
        }
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
    public Vector3 GetPointLastHit()
    {
        return (pointHit);
    }

    public void SetForwardWall(RaycastHit hitInfo)
    {
        dirNormal = hitInfo.normal;
        pointHit = hitInfo.point;
        isGrounded = true;
        isFlying = false;
        SetCurrentPlatform(hitInfo.transform);
    }

    public void SetNewNormalFromOutside(Vector3 newGravity)
    {
        dirNormal = newGravity;
    }

    private bool CanChangeNormal(RaycastHit hitInfo)
    {
        if (fastForward)
        {
            if (!fastForward.CanChangeNormal(hitInfo, dirSurfaceNormal))
                return (false);
        }

        return (true);
    }

    private bool SetCurrentPlatform(Transform platform)
    {
        if (lastPlatform != platform)
        {
            lastPlatform = platform;
            currentFloorLayer = LayerMask.LayerToName(platform.gameObject.layer);
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// Set isGrounded
    /// sphere cast down just beyond the bottom of the capsule to see/
    /// if the capsule is colliding round the bottom
    /// </summary>
    private void GroundChecking(float magnitudeToCheck, ref bool groundValue, Vector3 dirRay)
    {
        if (entityJump && entityJump.IsJumpedAndNotReady())
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
            //si on est sur un mur Galaxy...
            if (entityController.IsMarioGalaxyPlatform(LayerMask.LayerToName(hitInfo.collider.gameObject.layer)))
                //&& !entityGravityAttractorSwitch.IsNormalIsOkWithCurrentGravity(hitInfo.normal, entityGravityAttractorSwitch.GetDirGAGravity()))
            {
                //si on était en l'air, test la gravité par rapport à la vrai gravité !
                if (isFlying && !baseGravityAttractorSwitch.IsNormalIsOkWithCurrentGravity(hitInfo.normal, -baseGravityAttractorSwitch.GravityDirection))
                {
                    Debug.Log("here sphereAirMove tell us we are in a bad normal, (we were inAir before) continiue to fall");
                    groundValue = false;
                    return;
                }
                else if (!isFlying && !baseGravityAttractorSwitch.IsNormalIsOkWithCurrentGravity(hitInfo.normal, baseGravityAttractorSwitch.GetWantedGravityOnGround()))
                {
                    Debug.Log("here sphereAirMove tell us we are in a bad normal, (we were onGround Before, first time fly ?) continiue to fall");
                    groundValue = false;
                    return;
                }



            }

            if (SetCurrentPlatform(hitInfo.collider.transform))
            {
                //Debug.Log("test de fastForward ?");
            }

            groundValue = true;

            if (calculateSurfaceNormal)
            {
                dirSurfaceNormal = ExtVector3.GetSurfaceNormal(rb.transform.position,
                    baseGravity.GetMainAndOnlyGravity() * -0.01f,
                    groundCheckDistance,
                    sizeRadiusRayCast,
                    hitInfo.point,
                    collRayCastMargin,
                    entityController.layerMask);
            }
            else
            {
                dirSurfaceNormal = hitInfo.normal;
            }


            bool previous = fastForward && fastForward.IsInFastForward() && !fastForward.SwithcingIsRunning();
            if (CanChangeNormal(hitInfo))
            {
                dirNormal = hitInfo.normal;
                pointHit = hitInfo.point;

                if (fastForward && previous && fastForward.IsInFastForward())
                {
                    Debug.LogWarning("mmm ici ???");
                    dirNormal = dirSurfaceNormal;
                }


                Vector3 tmpOrientedGravity = dirNormal;
                if (fastForward && fastForward.DoChangeOrientationManually(hitInfo, ref tmpOrientedGravity))
                {
                    Debug.LogWarning("ici fast forwaard");
                    dirNormal = tmpOrientedGravity.normalized;
                }
            }
        }
        else
        {
            groundValue = false;
            //dirNormal = baseGravity.GetMainAndOnlyGravity() * 1;
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
            coolDownForStick.Reset();
        }
        else
        {
            //le coolDown inAir n'a pas commencé, OU a commencé, et n'est pas fini
            if (!coolDownForStick.IsStarted || coolDownForStick.IsRunning())
            {
                //Debug.Log("TRY TO STICK");
                GroundChecking(stickToFloorDist, ref isAlmostGrounded, baseGravity.GetMainAndOnlyGravity() * -0.01f);
                
            }
            else
            {
                isAlmostGrounded = false;
                //Debug.Log("DONT STICK");
            }

            if (!isGrounded && !isAlmostGrounded && (entityNoGravity && !entityNoGravity.IsBaseOrMoreRatio()) )
            {
                GroundChecking(stickToCeillingDist, ref isGrounded, baseGravity.GetMainAndOnlyGravity() * 0.01f);
            }
        }
    }

    /// <summary>
    /// set if we are flying or not !
    /// </summary>
    private void SetFlying()
    {
        if (entityJump && entityJump.IsJumpedAndNotReady())
        {
            isGrounded = false;
            isAlmostGrounded = false;
        }

        if (isGrounded || isAlmostGrounded)
        {
            isFlying = false;
            return;
        }

        if (!isFlying)
        {
            //Debug.Log("Set flying for the first time !");
            if (fastForward && fastForward.IsInFastForward())
            {
                fastForward.SetFlyingForTheFirstTime();
            }
            coolDownForStick.StartCoolDown(timeInAirBeforeNotStick);
            isFlying = true;
        }
    }

    private void FixedUpdate()
    {
        GroundChecking(groundCheckDistance, ref isGrounded, baseGravity.GetMainAndOnlyGravity() * -0.01f);           //set whenever or not we are grounded
        SetDragAndStick();          //set, depending on the grounded, the drag, and stick or not to the floor
        SetFlying();                //set if we fly or not !
    }
}
