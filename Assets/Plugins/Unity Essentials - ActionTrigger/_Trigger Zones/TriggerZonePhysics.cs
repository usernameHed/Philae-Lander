using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.ActionTrigger.entity;
using UnityEssentials.ActionTrigger.Extensions;

namespace UnityEssentials.ActionTrigger.Trigger
{
    public class TriggerZonePhysics : TriggerZone
    {
        private void OnTriggerEnter(Collider other)
        {
            Entity entity = other.gameObject.GetComponentInParent<Entity>();
            if (entity == null)
            {
                return;
            }
            base.AddIfNotContain(entity);
        }

        private void OnTriggerExit(Collider other)
        {
            Entity entity = other.gameObject.GetComponentInParent<Entity>();
            if (entity == null)
            {
                return;
            }
            base.Remove(entity);
        }
    }
}