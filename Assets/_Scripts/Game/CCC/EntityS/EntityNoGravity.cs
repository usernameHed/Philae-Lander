
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

public class EntityNoGravity : MonoBehaviour
{
    [SerializeField, Tooltip("")]
    private EntityController entityController = null;

    [SerializeField, Tooltip(""), ReadOnly]
    private bool wereWeInNoGravity = false;
    public bool WereWeInNoGravity() => wereWeInNoGravity;

    [SerializeField, Tooltip(""), ReadOnly]
    private float currentRatioGravity = 1f;
    public float GetNoGravityRatio() => currentRatioGravity;

    [SerializeField, Tooltip(""), ReadOnly]
    private List<NoGravityTrigger> zonesNoGravity = new List<NoGravityTrigger>();

    public void EnterInZone(NoGravityTrigger noGravityTrigger)
    {
        if (!zonesNoGravity.Contains(noGravityTrigger))
            zonesNoGravity.Add(noGravityTrigger);
        UpdateNoGravity();
    }

    public void LeanInZone(NoGravityTrigger noGravityTrigger)
    {
        zonesNoGravity.Remove(noGravityTrigger);
        UpdateNoGravity();
    }

    public void IsCollidingWhileNoGravity(Collision collision)
    {
        Debug.LogWarning("ici gravity warning");
    }

    /// <summary>
    /// is the current ratio fo the player the base ratio or more  ?
    /// </summary>
    public bool IsBaseOrMoreRatio()
    {
        return (currentRatioGravity >= 1);
    }

    /// <summary>
    /// only if there is at lease one noGravityRatio
    /// </summary>
    /// <returns></returns>
    private float CalculateMultipleRatio()
    {
        //reset ratio of nothing
        if (zonesNoGravity.Count == 0 && currentRatioGravity != 1)
        {
            return (1);
        }

        float actualRatioGravity = 0f;
        if (zonesNoGravity.Count > 0)
        {
            for (int i = 0; i < zonesNoGravity.Count; i++)
            {
                actualRatioGravity += zonesNoGravity[i].ratioGravity;
            }
            actualRatioGravity /= zonesNoGravity.Count;
        }
        return (actualRatioGravity);
    }

    private void UpdateNoGravity()
    {
        //get ratio of gravity
        currentRatioGravity = CalculateMultipleRatio();
    }

    public void OnGrounded()
    {
        wereWeInNoGravity = false;
    }

    private void Update()
    {
        if (currentRatioGravity < 1)
        {
            /*
            if (fastForward.IsInFastForward() && entityController.GetMoveState() == EntityController.MoveState.InAir)
            {
                fastForward.SetInAir();
            }
            */
            
        }
        if (currentRatioGravity == 0 && entityController.GetMoveState() == EntityController.MoveState.InAir
            && !wereWeInNoGravity)
        {
            wereWeInNoGravity = true;
        }
    }
}
