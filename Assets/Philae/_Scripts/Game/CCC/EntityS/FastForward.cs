
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;
using UnityEssentials.time;

public class FastForward : MonoBehaviour
{
    [SerializeField, Tooltip("time before die")]
    private float timeWithNoGravity = 3f;
    [SerializeField, Tooltip("time before die")]
    private float dotMarginDiffNormal = 0.71f;
    
    [SerializeField, Tooltip("ref")]
    private EntityController entityController = default;
    [SerializeField, Tooltip("ref")]
    private EntityJump entityJump = default;

    [SerializeField, Tooltip("ref")]
    private GroundCheck groundCheck = default;
    [SerializeField, Tooltip("ref")]
    private Rigidbody rb = default;
    [SerializeField, Tooltip("ref")]
    private EntityAction entityAction = default;
    [SerializeField, Tooltip("ref")]
    private BaseGravityAttractorSwitch baseGravityAttractorSwitch = default;

    [SerializeField, Tooltip(""), ReadOnly]
    private bool fastForward = false;
    public bool IsInFastForward() => fastForward;
    [SerializeField, Tooltip(""), ReadOnly]
    private Transform lastHitPlatform;
    [SerializeField, Tooltip(""), ReadOnly]
    private float timeBeforeSwitch = 0.1f;
    //[SerializeField, Tooltip(""), ReadOnly]
    //private List<FastForwardTrigger> fastForwardTriggers = new List<FastForwardTrigger>();
    
    private Vector3 previousNormal = Vector3.zero;
    private FrequencyCoolDown timeGoingForward = new FrequencyCoolDown();

    private FrequencyCoolDown timeBeforeBeiingForward = new FrequencyCoolDown();

    private void Init()
    {
        previousNormal = Vector3.zero;
    }

    public void JustJumped()
    {
        if (fastForward)
            SetFlyingForTheFirstTime();
    }

    public void SetFlyingForTheFirstTime()
    {
        Debug.Log("Set flying forward for the first time !");
        timeGoingForward.StartCoolDown(timeWithNoGravity);
        baseGravityAttractorSwitch.SetLastDirJump(previousNormal);
    }


    private void NoMoreForward()
    {
        if (IsInFastForward() && entityController.GetMoveState() == EntityController.MoveState.InAir
            && CanApplyGravity())
        {
            Debug.Log("here reset forward in air !");
            fastForward = false;
        }
    }

    public bool CanApplyGravity()
    {
        if (IsInFastForward() && timeGoingForward.IsRunning())
            return (false);
        return (true);
    }

    public void OnGrounded()
    {

    }

    /// <summary>
    /// return true if the difference between the previous normal and
    /// this one is less than 45°
    /// </summary>
    private bool IsDiffNormalGood(Vector3 newNormal)
    {
        //if we have no info on the previous normal, just say yes
        if (previousNormal == Vector3.zero)
        {
            return (true);
        }

        //Debug.DrawRay(rb.position, newNormal, Color.red, 5f);
        //Debug.DrawRay(rb.position, previousNormal * 0.5f, Color.yellow, 5f);


        float dotNormal = Vector3.Dot(previousNormal, newNormal);

        //Debug.Log("dot Diff: " + dotNormal + " (max: " + dotMarginDiffNormal + ")");
        //here we are too much diff
        if (dotNormal < dotMarginDiffNormal)
        {
            return (false);
        }
        return (true);
    }

    /*
    public void EnterInZone(FastForwardTrigger fastForwardTrigger)
    {
        if (!fastForwardTriggers.Contains(fastForwardTrigger))
            fastForwardTriggers.Add(fastForwardTrigger);
    }

    public void LeanInZone(FastForwardTrigger fastForwardTrigger)
    {
        fastForwardTriggers.Remove(fastForwardTrigger);
    }
    */

    /*
    /// <summary>
    /// say we want a certain direction gravity, and that it !
    /// </summary>
    public void SetNewDirectionFromOutside()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir && fastForwardTriggers.Count > 0)
        {
            if (fastForwardTriggers[0].IsAutomatic() || (!fastForwardTriggers[0].IsAutomatic() && entityAction.Jump && entityJump.IsJumpCoolDebugDownReady()))
            {
                Debug.Log("ici on est dans un trigger, l'activer si automatic, attendre l'input sinon");
                fastForward = true;
                
                previousNormal = fastForwardTriggers[0].GetGravityDirection(rb.position).normalized;
                groundCheck.SetNewNormalFromOutside(previousNormal);
                //playerGravity.SetOrientation
            }
        }
    }
    */

    /*
    public bool DoChangeOrientationManually(RaycastHit hitInfo, ref Vector3 newOrientation)
    {
        FastForwardOrientationLD fastForwardOrientationLD = hitInfo.transform.gameObject.GetComponentInParent<FastForwardOrientationLD>();
        if (fastForwardOrientationLD == null)
            return (false);

        if (!fastForwardOrientationLD.IsAutomatic() && !entityAction.Jump)
        {
            return (false);
        }

        newOrientation = fastForwardOrientationLD.GetGravityDirection(rb.position);
        Debug.DrawRay(rb.position, newOrientation, Color.black, 5f);
        return (true);
    }
    */

    public bool CanChangeNormal(RaycastHit hitInfo, Vector3 surfaceNormal)
    {
        int lastLayer = hitInfo.transform.gameObject.layer;

        //Debug.DrawRay(hitInfo.point, surfaceNormal, Color.black, 3f);

        Vector3 tmpNewGravity = Vector3.zero;
        bool changeManuallyGravity = false;// DoChangeOrientationManually(hitInfo, ref tmpNewGravity);


        //here we are in forward layer
        if (entityController.IsFastForwardLayer(lastLayer))
        {
            //here we just OnGrounded... we were flying ! so accepte the new normal then ?
            if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            {
                previousNormal = (!changeManuallyGravity) ? surfaceNormal : tmpNewGravity;// newNormal;
                //normalToReturn = previousNormal;
                fastForward = true; //say yes to fastForward
                lastHitPlatform = hitInfo.transform;
                Debug.Log("On Ground reset !");
                return (true);
            }

            //here the previous was a fast forward too
            if (fastForward)
            {
                if (lastHitPlatform.GetInstanceID() != hitInfo.transform.GetInstanceID())
                {
                    Debug.Log("update normal, we change forward");
                    //always update when we STAY in a fastForward
                    previousNormal = (!changeManuallyGravity) ? surfaceNormal : tmpNewGravity;// newNormal;
                    //normalToReturn = previousNormal;
                    lastHitPlatform = hitInfo.transform;
                    return (true);
                }
                else
                {
                    if (IsDiffNormalGood(surfaceNormal))
                    {
                        Debug.Log("update, we are on the same object, AND difference is negligible");
                        //here we leave forward layer, update and say yes to GROUNDCHECK
                        previousNormal = (!changeManuallyGravity) ? surfaceNormal : tmpNewGravity;// newNormal;
                        //normalToReturn = previousNormal;
                        return (true);
                    }
                    else
                    {
                        Debug.Log("here same object, but surface differ too much... dont update !");
                        //DONT update previous normal
                        //DONT update normal in GROUNDCHECK
                        return (false);
                    }
                }
            }
            //here the previous was NOT a fastForward, we can update everything
            else
            {
                Debug.Log("here the previous was NOT a fastForward, we can update everything");
                timeBeforeBeiingForward.StartCoolDown(timeBeforeSwitch);
                fastForward = true;
                lastHitPlatform = hitInfo.transform;
                previousNormal = surfaceNormal;// newNormal;

                //normalToReturn = previousNormal;
                return (true);
            }
        }
        //here we are not in forward layer
        else
        {
            timeBeforeBeiingForward.Reset();

            //here we just OnGrounded... we were flying ! so accepte the new normal then ?
            if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            {
                previousNormal = surfaceNormal;// newNormal;
                //normalToReturn = previousNormal;
                fastForward = false; //update fastForward
                lastHitPlatform = hitInfo.transform;
                //Debug.Log("On Ground reset !");
                return (true);
            }

            //if we were in fastforward before...
            if (fastForward)
            {
                //here we can update our normal, difference is negligable
                if (IsDiffNormalGood(surfaceNormal)/* && !IsDontLayer(lastLayer)*/)
                {
                    //here we leave forward layer, update and say yes to GROUNDCHECK
                    previousNormal = surfaceNormal;
                    //normalToReturn = previousNormal;
                    fastForward = false;

                    //Debug.Log("Update normal, difference is negligle");
                    return (true);
                }
                //here diff is too important
                else
                {
                    //DONT update previous normal
                    //DONT update normal in GROUNDCHECK
                    //Debug.Log("Dont update !");
                    return (false);
                }
                /*
                //DONT update previous normal
                //DONT update normal in GROUNDCHECK
                Debug.Log("Dont update !");
                //Debug.DrawRay(rb.position, previousNormal, Color.black, 5f);
                return (false);
                */
            }
            //here we were not in fastForward before.
            else
            {
                //nothing related to fastForward here !
                //Debug.Log("nothing related to fastForward here !");
                lastHitPlatform = hitInfo.transform;
                //normalToReturn = previousNormal;
                return (true);
            }
        }
    }

    public bool CanJump()
    {
        if (!IsInFastForward())
            return (true);
        /*
        FastForwardOrientationLD fastForwardOrientationLD = lastHitPlatform.gameObject.GetComponentInParent<FastForwardOrientationLD>();
        if (fastForwardOrientationLD)
        {
            return (false);
        }
        */
        return (true);
    }

    public bool SwithcingIsRunning()
    {
        if (timeBeforeBeiingForward.IsRunning())
            return (true);
        return (false);
    }
    

    private void FixedUpdate()
    {
        //SetNewDirectionFromOutside();
        NoMoreForward();
    }
}
