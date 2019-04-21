using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class AimFollowerController : MonoBehaviour
{
    [FoldoutGroup("Aim"), SerializeField]
    public float speedMove = 50f;

    [FoldoutGroup("Object"), SerializeField]
    public PlayerController playerController;
    [FoldoutGroup("Object"), SerializeField]
    public EntityRaycastForward _entityRaycastForward;
    [FoldoutGroup("Object"), SerializeField]
    public Rigidbody rb;

    private void Start()
    {
        if (!playerController)
            playerController = PhilaeManager.Instance.playerControllerRef;
    }

    private Quaternion GetLastDesiredRotation(Vector3 dirRelativeInput, Vector3 up)
    {
        //Vector3 relativeDirection = entityAction.GetRelativeDirection().normalized;
        // Preserve our current up direction
        // (or you could calculate this as the direction away from the planet's center)
        //Vector3 up = objectToRotate.up;
        //Vector3 up = baseGravity.GetMainAndOnlyGravity();

        // Form a rotation facing the desired direction while keeping our
        // local up vector exactly matching the current up direction.
        return (ExtQuaternion.TurretLookRotation(dirRelativeInput, up));
    }

    private void FixedUpdate()
    {
        //DoRotate(GetLastDesiredRotation(entityAction.GetRelativeDirection(), objectToRotate.up), turnRate);
        //UnityMovement.MoveTowards_WithPhysics(rb, _entityAction.GetMainReferenceForwardDirection(), speedMove * Time.deltaTime);
    }
}
