using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialVelocity : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private float initialSpeed = 3000f;
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private Transform dirObject;

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
    private Rigidbody rb;

    private void Awake ()
    {
        rb.velocity = (dirObject.position - rb.position) * initialSpeed;
    }
}
