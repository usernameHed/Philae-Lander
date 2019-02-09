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
    public string[] fastForwardLayer = new string[] { "Walkable/FastForward" };

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityController entityController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityJump entityJump;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private PlayerGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private EntityAttractor entityAttractor;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private GameObject ikillableObject;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private GroundCheck groundCheck;

    [FoldoutGroup("Debug"), SerializeField, Tooltip(""), ReadOnly]
    private bool fastForward = false;

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

    private bool IsForwardLayer(string layer)
    {
        return (IsForwardLayer(LayerMask.NameToLayer(layer)));
    }
    private bool IsForwardLayer(int layer)
    {
        int isForbidden = ExtList.ContainSubStringInArray(fastForwardLayer, LayerMask.LayerToName(layer));
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
        float dotNormal = ExtQuaternion.DotProduct(previousNormal, newNormal);

        Debug.Log("dot Diff: " + dotNormal + " (max: " + dotMarginDiffNormal + ")");
        //here we are too much diff
        if (dotNormal < dotMarginDiffNormal)
        {
            return (false);
        }
        return (true);
    }

    public bool CanChangeNormal(RaycastHit hitInfo)
    {
        int lastLayer = hitInfo.transform.gameObject.layer;
        Vector3 newNormal = hitInfo.normal;

        //here we are in forward layer
        if (IsForwardLayer(lastLayer))
        {
            //here we just OnGrounded... we were flying ! so accepte the new normal then ?
            if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            {
                previousNormal = newNormal;
                fastForward = true; //say yes to fastForward
                return (true);
            }

            //here the previous was a fast forward too
            if (fastForward)
            {
                //here we can update our normal, difference is negligable
                if (IsDiffNormalGood(newNormal))
                {
                    previousNormal = newNormal;
                    return (true);
                }
                //here we have too much difference
                else
                {
                    //DONT update previous normal
                    //DONT update normal in GROUNDCHECK
                    return (false);
                }
            }
            //here the previous was NOT a fastForward, we can update everything
            else
            {
                fastForward = true;
                previousNormal = newNormal;
                return (true);
            }
        }
        //here we are not in forward layer
        else
        {
            //here we just OnGrounded... we were flying ! so accepte the new normal then ?
            if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            {
                previousNormal = newNormal;
                fastForward = false; //update fastForward
                return (true);
            }

            //if we were in fastforward before...
            if (fastForward)
            {

                //here we can update our normal, difference is negligable
                if (IsDiffNormalGood(newNormal))
                {
                    //here we leave forward layer, update and say yes to GROUNDCHECK
                    previousNormal = newNormal;
                    fastForward = false;

                    Debug.Log("Update normal, difference is negligle");
                    return (true);
                }
                //here diff is too important
                else
                {
                    //DONT update previous normal
                    //DONT update normal in GROUNDCHECK
                    Debug.Log("Dont update !");
                    return (false);
                }
                
            }
            //here we were not in fastForward before.
            else
            {
                //nothing related to fastForward here !
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
            && playerGravity.GetOrientationPhysics() == PlayerGravity.OrientationPhysics.NORMALS && !timerDebugFlyAway.IsRunning())
        {
            Debug.Log("mettre le timer du mal");
            timerDebugFlyAway.StartCoolDown(timeDebugFlyAway);
        }
    }

    private void FixedUpdate()
    {
        //ChangeStageForward();
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
