using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBumpUp : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("rigidbody"), SerializeField]
    private float dotMargin = 0.86f;

    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityController entityController = null;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityGravity entityGravity = null;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityAirMove entityAirMove = null;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private Rigidbody rb = null;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityJump entityJump = null;

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    private bool hasBumpedUp = false;

    private bool CanDoBumpUp()
    {
        //if we have already  bumped up
        if (hasBumpedUp)
            return (false);
        //do a bump only if we are going upwards !
        if (entityGravity.IsGoingDownToGround())
            return (false);

        return (true);
    }
    
    public void JustJumped()
    {
        hasBumpedUp = false;
    }

    public void OnGrounded()
    {
        hasBumpedUp = false;
    }

    private void BumpUpDone()
    {
        hasBumpedUp = true;
        if (entityAirMove)
            entityAirMove.ResetAirMove();
        entityJump.ResetInitialJumpDir();
    }

    private void TryToAirBump(Vector3 normal, Transform objHit)
    {
        Vector3 currentDirInverted = -rb.velocity.normalized;
        float dotVelocity = ExtQuaternion.DotProduct(currentDirInverted, normal);

        Debug.DrawRay(rb.position, normal, Color.blue, 5f);
        Debug.DrawRay(rb.position, currentDirInverted, Color.cyan, 5f);

        //Debug.Log("here try to bump up ! dot: " + dotVelocity);
        if (dotVelocity > dotMargin)
        {
            Debug.Log("bump up !");
            Vector3 upJump = entityGravity.GetMainAndOnlyGravity();
            rb.velocity = upJump * rb.velocity.magnitude;

            //Debug.Break();
            BumpUpDone();
        }
    }

    public void HereBumpUp(RaycastHit hitInfo, Vector3 surfaceNormal)
    {
        if (!CanDoBumpUp())
            return;

        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            TryToAirBump(surfaceNormal, hitInfo.transform);
        }        
    }
}
