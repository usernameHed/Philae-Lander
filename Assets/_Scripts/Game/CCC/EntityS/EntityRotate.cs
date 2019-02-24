using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Rotate localy the player")]
public class EntityRotate : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float turnRate = 5f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAction entityAction = null;
    [FoldoutGroup("Object"), Tooltip("dobject to rotate"), SerializeField]
    private Transform objectToRotate = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityController entityController = null;

    private void RotatePlayer()
    {
        // Form the direction we want to look towards
        Vector3 relativeDirection = entityAction.GetRelativeDirection();
        //Debug.DrawRay(objectToRotate.position, relativeDirection, Color.white, 3f);

        // Preserve our current up direction
        // (or you could calculate this as the direction away from the planet's center)
        Vector3 up = objectToRotate.up;

        // Form a rotation facing the desired direction while keeping our
        // local up vector exactly matching the current up direction.
        Quaternion desiredOrientation = ExtQuaternion.TurretLookRotation(relativeDirection, up);

        // Move toward that rotation at a controlled, even speed regardless of framerate.
        objectToRotate.rotation = Quaternion.RotateTowards(
                                objectToRotate.rotation,
                                desiredOrientation,
                                turnRate * Time.deltaTime);

    }

    private void FixedUpdate()
    {
        if (entityController.GetMoveState() == PlayerController.MoveState.InAir)
        {
            return;
        }

        if (!entityAction.NotMoving())
        {
            RotatePlayer();
        }
    }
}
