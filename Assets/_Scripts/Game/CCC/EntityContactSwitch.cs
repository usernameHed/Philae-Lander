using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityContactSwitch : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField]
    private bool inAirForwardWall = true;

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] walkForbiddenForwardUp = new string[] { "Walkable/NoSide", "Walkable/Up" };
    [FoldoutGroup("GamePlay"), Range(0f, 2f), Tooltip("dist to check forward player"), SerializeField]
    private float distForward = 0.6f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float sizeRadiusForward = 0.3f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), Tooltip(""), SerializeField]
    public float dotMarginImpact = 0.3f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), Tooltip(""), SerializeField]
    public float timeBetween2TestForward = 0.8f;

    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityController entityController;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private PlayerGravity playerGravity;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityAction entityAction;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntitySlide entitySlide;

    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isForwardWall = false;
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isForbiddenForward = false;


    private FrequencyCoolDown coolDownForward = new FrequencyCoolDown();

    public bool IsForwardForbiddenWall()
    {
        return (isForwardWall && isForbiddenForward);
    }

    public bool IsCoolDownSwitchReady()
    {
        return (coolDownForward.IsReady());
    }

    private void ForwardWallCheck()
    {
        RaycastHit hitInfo;

        ResetContact();

        //do nothing if not moving
        if (entityAction.NotMoving())
            return;
        //do nothing if input and forward player are not equal
        if (!entityController.IsLookingTowardTheInput(dotMarginImpact))
            return;            

        if (Physics.SphereCast(rb.transform.position, sizeRadiusForward, entityController.GetFocusedForwardDirPlayer(), out hitInfo,
                               distForward, entityController.layerMask, QueryTriggerInteraction.Ignore))
        {
            //ExtDrawGuizmos.DebugWireSphere(rb.transform.position + (entityController.GetFocusedForwardDirPlayer()) * (distForward), Color.yellow, sizeRadiusForward, 0.1f);
            //Debug.DrawRay(rb.transform.position, (entityController.GetFocusedForwardDirPlayer()) * (distForward), Color.yellow, 5f);
            ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.red, 0.1f, 0.1f);

            isForwardWall = true;

            Vector3 normalHit = hitInfo.normal;
            Vector3 upPlayer = playerGravity.GetMainAndOnlyGravity();
            entitySlide.CalculateStraffDirection(normalHit);    //calculate SLIDE

            float dotWrongSide = ExtQuaternion.DotProduct(upPlayer, normalHit);
            if (dotWrongSide < -dotMarginImpact)
            {
                Debug.Log("forward too inclined, dotImpact: " + dotWrongSide + "( max: " + dotMarginImpact + ")");
                isForbiddenForward = true;
                return;
            }

            int isForbidden = ExtList.ContainSubStringInArray(walkForbiddenForwardUp, LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
            if (isForbidden != -1)
            {
                //here we are in front of a forbidden wall !!
                isForbiddenForward = true;
            }
            else
            {
                if (groundCheck.IsFlying() && !inAirForwardWall)
                {
                    isForbiddenForward = true;
                }
                else
                {
                    //HERE FORWARD, DO SWITCH !!
                    coolDownForward.StartCoolDown(timeBetween2TestForward);
                    //Debug.Log("forward");
                    groundCheck.SetForwardWall(hitInfo);
                    
                    isForbiddenForward = false;
                }
            }
        }
        else
        {
            ResetContact();
        }
    }

    private void ResetContact()
    {
        isForwardWall = false;
        isForbiddenForward = false;
    }

    private void FixedUpdate()
    {
        //set if the is a wall in front of us (we need flying info)
        ForwardWallCheck();
    }
}
