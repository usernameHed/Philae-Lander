using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialVelocity : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private float initialSpeed = 3000f;
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private float initialAngularVelocity = 30f;
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private Transform dirObject = null;

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
    private Rigidbody rb = null;

    private void Awake ()
    {
        rb.velocity = (dirObject.position - rb.position) * initialSpeed;
        rb.angularVelocity = new Vector3(ExtRandom.GetRandomNumber(-1f, 1f) * initialAngularVelocity,
            ExtRandom.GetRandomNumber(-1f, 1f) * initialAngularVelocity,
            ExtRandom.GetRandomNumber(-1f, 1f) * initialAngularVelocity);
    }
}
