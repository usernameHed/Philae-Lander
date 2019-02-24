using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastForward : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("time before die")]
    private float timeBeforeDie = 3f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("time before die")]
    private float dotMarginDiffNormal = 0.71f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] fastForwardLayer = new string[] { "Walkable/FastForward"};
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] orientedForwardLayer = new string[] { "Walkable/OrientedGravity" };
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] dontLayer = new string[] { "Walkable/Dont" };

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityController entityController = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityJump entityJump = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityGravity entityGravity = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityAttractor entityAttractor = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private GameObject ikillableObject = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private GroundCheck groundCheck = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private Rigidbody rb = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityAction entityAction = null;

    [FoldoutGroup("Debug"), SerializeField, Tooltip(""), ReadOnly]
    private bool fastForward = false;
    public bool IsInFastForward() => fastForward;
    [FoldoutGroup("Debug"), SerializeField, Tooltip(""), ReadOnly]
    private Transform lastHitPlatform;
    [FoldoutGroup("Debug"), SerializeField, Tooltip(""), ReadOnly]
    private List<FastForwardTrigger> fastForwardTriggers = new List<FastForwardTrigger>();


    [FoldoutGroup("Debug"), Tooltip("espace entre 2 sauvegarde de position ?"), SerializeField]
    private float timeDebugFlyAway = 0.3f;   //a-t-on un attract point de placé ?

    private FrequencyCoolDown timerDebugFlyAway = new FrequencyCoolDown();
    private FrequencyCoolDown coolDownBeforeDie = new FrequencyCoolDown();
    private Vector3 previousNormal = Vector3.zero;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.SceneLoaded, Init);
    }

    private void Init()
    {
        previousNormal = Vector3.zero;
    }

    public void JustJumped()
    {
        if (fastForward)
            SetInAir();
    }

    public bool WhereWeInFastForward()
    {
        return (fastForward);
    }

    public void OnGrounded()
    {
        //TODO: here reset fastForward ?
        ResetFlyAway();
    }

    private bool IsForwardLayer(int layer)
    {
        int isFastForward = ExtList.ContainSubStringInArray(fastForwardLayer, LayerMask.LayerToName(layer));
         if (isFastForward != -1)
            return (true);
        return (false);
    }
    private bool IsOrientedLayer(int layer)
    {
        int isOriented = ExtList.ContainSubStringInArray(orientedForwardLayer, LayerMask.LayerToName(layer));
        if (isOriented != -1)
            return (true);
        return (false);
    }
    private bool IsDontLayer(int layer)
    {
        int isForbidden = ExtList.ContainSubStringInArray(dontLayer, LayerMask.LayerToName(layer));
        if (isForbidden != -1)
            return (true);
        return (false);
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


        float dotNormal = ExtQuaternion.DotProduct(previousNormal, newNormal);

        //Debug.Log("dot Diff: " + dotNormal + " (max: " + dotMarginDiffNormal + ")");
        //here we are too much diff
        if (dotNormal < dotMarginDiffNormal)
        {
            return (false);
        }
        return (true);
    }

    
    public void EnterInZone(FastForwardTrigger fastForwardTrigger)
    {
        if (!fastForwardTriggers.Contains(fastForwardTrigger))
            fastForwardTriggers.Add(fastForwardTrigger);
    }

    public void LeanInZone(FastForwardTrigger fastForwardTrigger)
    {
        fastForwardTriggers.Remove(fastForwardTrigger);
    }

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
                SetInAir();

                entityGravity.SetOrientation(EntityGravity.OrientationPhysics.NORMALS);
                previousNormal = fastForwardTriggers[0].GetGravityDirection(rb.position).normalized;
                groundCheck.SetNewNormalFromOutside(previousNormal);
                //playerGravity.SetOrientation
            }
        }
    }

    public bool DoChangeOrientationManually(RaycastHit hitInfo, ref Vector3 newOrientation)
    {
        if (!IsOrientedLayer(hitInfo.transform.gameObject.layer))
            return (false);

        FastForwardOrientationLD fastForwardOrientationLD = hitInfo.transform.gameObject.GetComponentInAllParents<FastForwardOrientationLD>(99, true);
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

    public bool CanChangeNormal(RaycastHit hitInfo, Vector3 surfaceNormal)
    {
        int lastLayer = hitInfo.transform.gameObject.layer;

        Vector3 tmpNewGravity = Vector3.zero;
        bool changeManuallyGravity = DoChangeOrientationManually(hitInfo, ref tmpNewGravity);


        //here we are in forward layer
        if (IsForwardLayer(lastLayer) || IsOrientedLayer(lastLayer))
        {
            //here we just OnGrounded... we were flying ! so accepte the new normal then ?
            if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            {
                previousNormal = (!changeManuallyGravity) ? surfaceNormal : tmpNewGravity;// newNormal;
                //normalToReturn = previousNormal;
                fastForward = true; //say yes to fastForward
                lastHitPlatform = hitInfo.transform;
                //Debug.Log("On Ground reset !");
                return (true);
            }

            //here the previous was a fast forward too
            if (fastForward)
            {
                if (lastHitPlatform.GetInstanceID() != hitInfo.transform.GetInstanceID())
                {
                    //Debug.Log("update normal, we change forward");
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
                        //Debug.Log("update, we are on the same object, AND difference is negligible");
                        //here we leave forward layer, update and say yes to GROUNDCHECK
                        previousNormal = (!changeManuallyGravity) ? surfaceNormal : tmpNewGravity;// newNormal;
                        //normalToReturn = previousNormal;
                        return (true);
                    }
                    else
                    {
                        //Debug.Log("here same object, but surface differ too much... dont update !");
                        //DONT update previous normal
                        //DONT update normal in GROUNDCHECK
                        return (false);
                    }
                }
            }
            //here the previous was NOT a fastForward, we can update everything
            else
            {
                //Debug.Log("here the previous was NOT a fastForward, we can update everything");
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
                if (IsDiffNormalGood(surfaceNormal) && !IsDontLayer(lastLayer))
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

    /// <summary>
    /// 
    /// </summary>
    public void SetInAir()
    {
        Debug.Log("ok, we could be dead soon");
        WeSupposeWeAreDeadSoon();
        timerDebugFlyAway.Reset();
    }

    /// <summary>
    /// miracle, we survive !
    /// </summary>
    public void WeAreSavedYeah()
    {
        coolDownBeforeDie.Reset();
    }

    public bool IsCurrentlyWaitingForDeath()
    {
        return (coolDownBeforeDie.IsRunning());
    }
    /// <summary>
    /// we are falling down... no ending in perspective !
    /// </summary>
    public void WeSupposeWeAreDeadSoon()
    {
        coolDownBeforeDie.StartCoolDown(timeBeforeDie);
    }

    private void TryToKill()
    {
        if (coolDownBeforeDie.IsStartedAndOver())
        {
            ikillableObject.GetComponent<IKillable>().Kill();
        }
    }

    /// <summary>
    /// reset far away when we are on ground
    /// </summary>
    public void ResetFlyAway()
    {
        timerDebugFlyAway.Reset();
        WeAreSavedYeah();
    }

    /// <summary>
    /// active attractor if we are far away !
    /// </summary>
    private void DebugFlyAway()
    {
        if (IsCurrentlyWaitingForDeath())
        {
            return;
        }

        if (timerDebugFlyAway.IsStartedAndOver())
        {
            if (WhereWeInFastForward())
            {
                SetInAir();
                return;
            }

            Debug.LogError("ok on est dans le mal !");
            timerDebugFlyAway.Reset();
            entityAttractor.ActiveAttractor();
            return;
        }
        if (!entityJump.HasJumped && entityController.GetMoveState() == EntityController.MoveState.InAir
            && entityGravity.GetOrientationPhysics() == EntityGravity.OrientationPhysics.NORMALS && !timerDebugFlyAway.IsRunning())
        {
            Debug.Log("mettre le timer du mal");
            timerDebugFlyAway.StartCoolDown(timeDebugFlyAway);
        }
    }

    private void FixedUpdate()
    {
        SetNewDirectionFromOutside();
        DebugFlyAway();
    }

    private void Update()
    {
        TryToKill();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.SceneLoaded, Init);
    }
}
