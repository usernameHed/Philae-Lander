using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float turnRate = 5f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerInput playerInput;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("dobject to rotate"), SerializeField]
    private Transform objectToRotate;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private GroundCheck groundCheck;
    

    private Quaternion targetRotation = Quaternion.identity;

    public Quaternion DirObject(Quaternion rotation, Vector3 dir, float turnRate)
    {
        float heading = Mathf.Atan2(-dir.x * turnRate * Time.deltaTime, dir.z * turnRate * Time.deltaTime);

        Quaternion _targetRotation = Quaternion.identity;

        float x = 0;
        float y = heading * -1 * Mathf.Rad2Deg;
        float z = 0;

        _targetRotation = Quaternion.Euler(x, y, z);
        rotation = Quaternion.RotateTowards(rotation, _targetRotation, turnRate * Time.deltaTime);
        return (rotation);
    }

    private void RotatePlayer()
    {
        Vector2 dirInput = playerInput.GetDirInput();
        Vector3 relativeDirection = rb.transform.right * dirInput.x + rb.transform.forward * dirInput.y;

        Debug.DrawRay(objectToRotate.position, relativeDirection, Color.cyan, 0.4f);
        objectToRotate.localRotation = DirObject(objectToRotate.localRotation, relativeDirection, turnRate);

        //Quaternion toRotation = Quaternion.Euler(relativeDirection);
        //objectToRotate.rotation = Quaternion.Slerp(objectToRotate.rotation, toRotation, Time.deltaTime * turnRate);
    }

    private void FixedUpdate()
    {
        if (!groundCheck.IsSafeGrounded())
        {
            return;
        }

        if (!playerInput.NotMoving())
        {
            RotatePlayer();
        }
    }
}
