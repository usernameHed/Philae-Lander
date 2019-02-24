using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBumpUp : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("rigidbody"), SerializeField]
    private float timeBeforeWeCanBumpUp = 0.2f;
    [FoldoutGroup("GamePlay"), Tooltip("rigidbody"), SerializeField]
    private float dotMargin = 0.86f;

    [FoldoutGroup("GroundBump"), Tooltip("rigidbody"), SerializeField]
    private float upDistRaycast = 0.1f;
    [FoldoutGroup("GroundBump"), Tooltip("rigidbody"), SerializeField]
    private float lateralDistRaycast = 0.3f;
    [FoldoutGroup("GroundBump"), Tooltip("rigidbody"), SerializeField]
    private float distForwardRaycast = 1f;
    [FoldoutGroup("GroundBump"), Tooltip("rigidbody"), SerializeField]
    private float marginMagnitudeForBump = 0.5f;
    [FoldoutGroup("GroundBump"), Tooltip("rigidbody"), SerializeField]
    private float boostHeight = 0.5f;
    public float GetBoostHeight() => boostHeight;

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

    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityJumpCalculation entityJumpCalculation = null;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityAction entityAction = null;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityGravityAttractorSwitch entityGravityAttractorSwitch = null;

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    private bool hasBumpedUp = false;
    [FoldoutGroup("GroundBump"), Tooltip(""), SerializeField]
    private float timeBeforeResetBumpUpGround = 0.1f;

    [FoldoutGroup("Debug"), Tooltip("rigidbody"), SerializeField]
    private bool isBumpingGroundUp = false;
    public bool IsBumpingGroundUp() => isBumpingGroundUp;
    public void ResetBumpingGround() => isBumpingGroundUp = false;

    private FrequencyCoolDown coolDownJump = new FrequencyCoolDown();
    private FrequencyCoolDown coolDownBumpUpGround = new FrequencyCoolDown();


    private bool CanDoBumpUp()
    {
        //if we have already  bumped up
        if (hasBumpedUp)
            return (false);
        //do a bump only if we are going upwards !
        if (entityGravity.IsGoingDown())
            return (false);

        if (!entityJumpCalculation.CanDoBumpUp())
            return (false);

        return (true);
    }
    
    public void JustJumped()
    {
        coolDownJump.StartCoolDown(timeBeforeWeCanBumpUp);
        hasBumpedUp = false;
    }

    public void OnGrounded()
    {
        coolDownJump.Reset();
        hasBumpedUp = false;
    }

    private void BumpUpDone()
    {
        hasBumpedUp = true;
        entityAirMove.ResetAirMove();
        entityJump.ResetInitialJumpDir();
    }

    private void TryToGroundBump(Vector3 normal)
    {
        isBumpingGroundUp = false;

        RaycastHit hit;
        Vector3 origin = rb.position + entityGravity.GetMainAndOnlyGravity() * upDistRaycast;
        Vector3 originRight = origin + entityController.GetFocusedRightDirPlayer() * lateralDistRaycast;
        Vector3 originLeft = origin - entityController.GetFocusedRightDirPlayer() * lateralDistRaycast;
        Vector3 dirRaycast = entityController.GetFocusedForwardDirPlayer();

        if (entityAction.GetMagnitudeInput() < marginMagnitudeForBump)
            return;

        if (!Physics.Raycast(origin, dirRaycast, out hit, distForwardRaycast, entityController.layerMask)
            && !Physics.Raycast(originRight, dirRaycast, out hit, distForwardRaycast, entityController.layerMask)
            && !Physics.Raycast(originLeft, dirRaycast, out hit, distForwardRaycast, entityController.layerMask))
        {
            Debug.DrawRay(origin, dirRaycast, Color.magenta, 0.1f);
            Debug.DrawRay(originRight, dirRaycast, Color.magenta, 0.1f);
            Debug.DrawRay(originLeft, dirRaycast, Color.magenta, 0.1f);
            Debug.Log("Did No Hit");
            isBumpingGroundUp = true;
            coolDownBumpUpGround.StartCoolDown(timeBeforeResetBumpUpGround);
        }
    }
    

    private void TryToAirBump(Vector3 normal, Transform objHit)
    {
        Vector3 currentDirInverted = -rb.velocity.normalized;
        float dotVelocity = ExtQuaternion.DotProduct(currentDirInverted, normal);
        if (dotVelocity > dotMargin)
        {
            //TODO: here do not bump up si:
            //objHit.layer est une GA, ET normal pas bon
            if (entityGravityAttractorSwitch.IsAirAttractorLayer(objHit.gameObject.layer)
                && entityGravityAttractorSwitch.IsNormalOk(objHit, normal, false))
            {
                Debug.LogError("NOP raté, pas de air bump !");

                return;
            }

            //Debug.Log("here try to bump up ! dot: " + dotVelocity);
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

        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
        {
            TryToGroundBump(surfaceNormal);
        }
        else
        {
            TryToAirBump(surfaceNormal, hitInfo.transform);
        }        
    }

    private void FixedUpdate()
    {
        if (coolDownBumpUpGround.IsStartedAndOver())
            ResetBumpingGround();
    }
}
