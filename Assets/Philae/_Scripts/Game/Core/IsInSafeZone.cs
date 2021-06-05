
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.time;

namespace Philae.Core
{
    public class IsInSafeZone : MonoBehaviour
    {
        [SerializeField, Tooltip("")]
        public bool isInQuieteZone = false;

        private FrequencyCoolDown timerStaySafe = new FrequencyCoolDown();
        private float timeSafe = 0.2f;

        public void SetSafe()
        {
            isInQuieteZone = true;
            timerStaySafe.StartCoolDown(timeSafe);
        }

        private void TryToResetSafe()
        {
            if (timerStaySafe.IsFinished())
            {
                isInQuieteZone = false;
            }
        }
        public bool IsSafe()
        {
            return (isInQuieteZone);
        }

        private void Update()
        {
            TryToResetSafe();
        }
    }
}