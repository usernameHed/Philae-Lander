using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.ActionTrigger.entity;
using UnityEssentials.ActionTrigger.Extensions;
using UnityEssentials.ActionTrigger.Trigger;
using UnityEssentials.Geometry.MovableShape;

namespace UnityEssentials.ActionTrigger.Actions.example
{
    /// <summary>
    /// here this action search for the Life IActionComponent of the Entity, and assign it with a class
    /// </summary>
    public class ActionKeepTrackOfEntiesOverTime : Action
    {
        [Serializable]
        public struct DamageOverTime
        {
            public float HitAmount;
        }

        private Dictionary<Life, DamageOverTime> _damageOverTimePerEntity = new Dictionary<Life, DamageOverTime>(TriggerZone.DEFAULT_MAX_ELEMENT_IN_ZONE);

        public override void OnEnter(Entity entity)
        {
            if (entity.HasIActionComponent<Life>(out Life life))
            {
                //here this entity of type Life just enter
                //add entity if not already present
                if (!_damageOverTimePerEntity.TryGetValue(life, out DamageOverTime damage))
                {
                    DamageOverTime damageOverTime = new DamageOverTime();
                    _damageOverTimePerEntity.Add(life, damageOverTime);
                }
            }
        }

        public override void OnExit(Entity entity)
        {
            if (entity.HasIActionComponent<Life>(out Life life))
            {
                //here this entity of type Life just exit
                _damageOverTimePerEntity.Remove(life);
            }
        }

        public override void OnStay(Entity entity)
        {
            if (entity.HasIActionComponent<Life>(out Life life))
            {
                //here this entity of type Life is currently inside
                bool exist = _damageOverTimePerEntity.TryGetValue(life, out DamageOverTime damage);
                if (!exist)
                {
                    return;
                }
                //here you have the life component using the damage datas
            }
        }
    }
}