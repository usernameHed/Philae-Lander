using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.ActionTrigger.entity;
using UnityEssentials.ActionTrigger.Extensions;
using UnityEssentials.Geometry.MovableShape;
using UnityEssentials.SceneWorkflow;

namespace UnityEssentials.ActionTrigger.Trigger
{
    public class TriggerZoneNoPhysics : TriggerZone
    {
        [SerializeField] private MovableShape _shape = default;
        /// <summary>
        /// if > 0, add timeStep
        /// </summary>
        [SerializeField] private float _coolDownCalculation = 0.1f;

        private EntityListerInScenes _entityListerInScenes = default;
        private FrequencyCoolDown _coolDownCalculationTimer = new FrequencyCoolDown();

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            _shape.ShowZone = false;
        }

        private void Start()
        {
            _entityListerInScenes = DependencyInjectorSingleton.Instance.Get<EntityListerInScenes>();
        }

        private void FixedUpdate()
        {
            if (_coolDownCalculationTimer.IsNotRunning())
            {
                LoopThoughtEntities();
                _coolDownCalculationTimer.StartCoolDown(_coolDownCalculation);
            }
        }

        private void LoopThoughtEntities()
        {
            //here for all entity present in scene,
            //test if they are inside the zone
            for (int i = _entityListerInScenes.EntityListCount - 1; i >= 0; i--)
            {
                Entity entity = _entityListerInScenes.GetEntity(i);
                if (base.IsEntityInside(entity))
                {
                    AttemptToRemove(entity);
                    continue;
                }
                AttemptToAdd(entity);
            }
        }

        /// <summary>
        /// Attempt to add if it enter the zone
        /// </summary>
        /// <param name="entity">entity to test</param>
        /// <returns>true if added</returns>
        private bool AttemptToAdd(Entity entity)
        {
            bool isInside = _shape.IsInsideShape(entity.transform.position);
            if (isInside)
            {
                base.Add(entity);
                return (true);
            }
            return (false);
        }

        /// <summary>
        /// Attempt to remove the entity if it leave the zone
        /// </summary>
        /// <param name="entity">entity to test</param>
        /// <returns>true if removed</returns>
        private bool AttemptToRemove(Entity entity)
        {
            bool isInside = _shape.IsInsideShape(entity.transform.position);
            if (!isInside)
            {
                base.Remove(entity);
                return (true);
            }
            return (false);
        }
    }
}