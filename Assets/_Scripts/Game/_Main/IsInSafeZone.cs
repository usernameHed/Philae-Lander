
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (timerStaySafe.IsStartedAndOver())
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
