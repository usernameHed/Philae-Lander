using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.ActionTrigger.entity;
using UnityEssentials.ActionTrigger.Extensions;
using UnityEssentials.ActionTrigger.Trigger;
using UnityEssentials.CrossSceneReference;

namespace UnityEssentials.ActionTrigger.Actions
{
    public abstract class Action : GuidComponent
    {
        private List<Entity> _entityToApplyAction = new List<Entity>(TriggerZone.DEFAULT_MAX_ELEMENT_IN_ZONE);

        public virtual void DoActionOnEnter(Entity entity)
        {
            if (entity == null)
            {
                return;
            }
            _entityToApplyAction.AddIfNotContain(entity);
            //Debug.Log("ENTER ACTION " + gameObject.name + " on " + entity.gameObject, gameObject);
            OnEnter(entity);
        }

        public virtual void DoActionOnExit(Entity entity)
        {
            if (entity == null)
            {
                return;
            }
            _entityToApplyAction.Remove(entity);
            //Debug.Log("LEAVE ACTION " + gameObject.name + " on " + entity.gameObject, gameObject);
            OnExit(entity);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _entityToApplyAction.Count; i++)
            {
                if (_entityToApplyAction[i] == null)
                {
                    continue;
                }
                OnStay(_entityToApplyAction[i]);
            }
        }

        public abstract void OnEnter(Entity entity);
        public abstract void OnExit(Entity entity);
        public abstract void OnStay(Entity entity);
    }
}