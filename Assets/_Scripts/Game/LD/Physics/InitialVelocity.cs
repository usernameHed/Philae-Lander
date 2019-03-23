using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialVelocity : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private float initialSpeed = 3000f;
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private float initialAngularVelocity = 0f;
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private Transform dirObject = null;
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private bool randomDir = true;
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private float timeMinBeforeBump = 5f;
    [FoldoutGroup("GamePlay"), Tooltip("initial push"), SerializeField]
    private float timeMaxRandomToAdd = 5f;

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
    private Rigidbody rb = null;

    private FrequencyCoolDown timeBeforeActive = new FrequencyCoolDown();

    private void Awake ()
    {
        ApplyInitialVelocity();
    }

    [Button]
    private void ApplyInitialVelocity()
    {
        if (randomDir)
        {
            rb.velocity = (ExtRandom.GetRandomInsideUnitSphere(1)).normalized * initialSpeed;
        }
        else
        {
            rb.velocity = (dirObject.position - rb.position).normalized * initialSpeed;
        }
        rb.angularVelocity = new Vector3(ExtRandom.GetRandomNumber(-1f, 1f) * initialAngularVelocity,
            ExtRandom.GetRandomNumber(-1f, 1f) * initialAngularVelocity,
            ExtRandom.GetRandomNumber(-1f, 1f) * initialAngularVelocity);

        timeBeforeActive.StartCoolDown(timeMinBeforeBump + ExtRandom.GetRandomNumber(0, timeMaxRandomToAdd));
    }

    private void Update()
    {
        if (timeBeforeActive.IsStartedAndOver())
        {
            ApplyInitialVelocity();
        }
    }
}
