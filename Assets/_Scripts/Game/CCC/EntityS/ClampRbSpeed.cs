using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampRbSpeed : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("rigidbody"), SerializeField]
    private float maxSpeed = 10f;

    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb = null;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityController entityController = null;

    private void ClampSpeed()
    {
        if (entityController.GetActualVelocity() > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    private void FixedUpdate()
    {
        ClampSpeed();
    }
}
