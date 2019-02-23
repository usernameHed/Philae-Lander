using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBumpUp : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("rigidbody"), SerializeField]
    private float timeBeforeWeCanBumpUp = 0.2f;

    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityController entityController;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;

    [FoldoutGroup("Debug"), Tooltip("rigidbody"), SerializeField]
    private bool hasBumpedUp = false;

    private FrequencyCoolDown coolDownJump = new FrequencyCoolDown();

    private bool CanDoBumpUp()
    {
        //if we are not inAir
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return (false);
        //if we have already  bumped up
        if (hasBumpedUp)
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

    public void HereBumpUp(RaycastHit hitInfo)
    {
        if (!CanDoBumpUp())
            return;


        Vector3 currentDir = rb.velocity;
        Vector3 normal = hitInfo.normal;

        Debug.Log("here try to bump up, becasue we canot swithc up");
        hasBumpedUp = true;
    }
}
