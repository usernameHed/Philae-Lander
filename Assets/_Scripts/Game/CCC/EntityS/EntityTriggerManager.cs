using Sirenix.OdinInspector;
using UnityEngine;

public class EntityTriggerManager : MonoBehaviour
{
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public PlayerController playerController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public IAController iAController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityNoGravity entityNoGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public EntityGravityAttractorSwitch entityGravityAttractorSwitch;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    public FastForward fastForward;
    //[FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    //public EntityController entityController;

    public void OwnTriggerEnter(Collider other)
    {

    }

    public void OwnTriggerStay(Collider other)
    {

    }

    public void OwnTriggerExit(Collider other)
    {

    }

    public void OwnCollisionEnter(Collision collision)
    {

    }

    public void OwnCollisionStay(Collision collision)
    {
        /*if (entityController.GetMoveState() == EntityController.MoveState.InAir
            && !entityNoGravity.IsBaseOrMoreRatio())
        {
            entityNoGravity.IsCollidingWhileNoGravity(collision);
        }
        */
    }

    public void OwnCollisionExit(Collision collision)
    {

    }

    public void Kill()
    {
        if (playerController)
            playerController.Kill();
        if (iAController)
            iAController.Kill();
    }
}
