using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float turnRate = 5f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private Transform mainReferenceObjectDirection;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerInput playerInput;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("dobject to rotate"), SerializeField]
    private Transform objectToRotate;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerController playerController;

    private void RotatePlayer()
    {
        // Form the direction we want to look towards
        Vector2 dirInput = playerInput.GetDirInput();
        Vector3 relativeDirection = mainReferenceObjectDirection.right * dirInput.x + mainReferenceObjectDirection.forward * dirInput.y;
        Debug.DrawRay(objectToRotate.position, relativeDirection, Color.cyan, 4f);

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
        if (playerController.GetMoveState() == PlayerController.MoveState.InAir)
        {
            return;
        }

        if (!playerInput.NotMoving())
        {
            RotatePlayer();
        }
    }
}
