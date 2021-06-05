
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;
using UnityEssentials.time;

namespace Philae.LD
{
    public class InitialVelocity : MonoBehaviour
    {
        [Tooltip("initial push"), SerializeField]
        private float initialSpeed = 3000f;
        [Tooltip("initial push"), SerializeField]
        private float initialAngularVelocity = 0f;
        [Tooltip("initial push"), SerializeField]
        private Transform dirObject = null;
        [Tooltip("initial push"), SerializeField]
        private bool randomDir = true;
        [Tooltip("initial push"), SerializeField]
        private float timeMinBeforeBump = 5f;
        [Tooltip("initial push"), SerializeField]
        private float timeMaxRandomToAdd = 5f;

        [Tooltip("opti fps"), SerializeField]
        private Rigidbody rb = null;

        private FrequencyCoolDown timeBeforeActive = new FrequencyCoolDown();

        private void Awake()
        {
            ApplyInitialVelocity();
        }


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
            if (timeBeforeActive.IsFinished())
            {
                ApplyInitialVelocity();
            }
        }
    }
}