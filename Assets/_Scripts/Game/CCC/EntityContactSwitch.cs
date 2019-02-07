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
    public float dotMarginImpact = 0.86f;

    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityController entityController;

    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isForwardWall = false;
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isForbiddenForward = false;

    public bool IsForwardForbiddenWall()
    {
        return (isForwardWall && isForbiddenForward);
    }

    private void ForwardWallCheck()
    {
        RaycastHit hitInfo;

        if (Physics.SphereCast(rb.transform.position, sizeRadiusForward, entityController.GetFocusedForwardDirPlayer(), out hitInfo,
                               distForward, entityController.layerMask, QueryTriggerInteraction.Ignore))
        {
            ExtDrawGuizmos.DebugWireSphere(rb.transform.position + (entityController.GetFocusedForwardDirPlayer()) * (distForward), Color.yellow, sizeRadiusForward, 3f);
            Debug.DrawRay(rb.transform.position, (entityController.GetFocusedForwardDirPlayer()) * (distForward), Color.yellow, 5f);
            ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.red, 0.1f, 3f);

            Debug.Log("forward");
            isForwardWall = true;

            Vector3 forwardPlayer = entityController.GetFocusedForwardDirPlayer();
            Vector3 normalHit = hitInfo.normal;

            float dotImpact = ExtQuaternion.DotProduct(-forwardPlayer, normalHit);
            if (dotImpact < dotMarginImpact)
            {
                Debug.Log("forward too inclined");
                isForbiddenForward = true;
                return;
            }


            

            int isForbidden = ExtList.ContainSubStringInArray(walkForbiddenForwardUp, LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
            if (isForbidden != -1)
            {
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
                    groundCheck.SetForwardWall(hitInfo.normal);
                    isForbiddenForward = false;
                }
            }
        }
        else
        {
            isForwardWall = false;
            isForbiddenForward = false;
        }
    }

    private void FixedUpdate()
    {
        //set if the is a wall in front of us (we need flying info)
        ForwardWallCheck();
    }
}
