using UnityEngine;
using UnityEssentials.ActionTrigger.entity;
using UnityEssentials.Geometry.MovableShape;

namespace UnityEssentials.ActionTrigger.Actions.example
{
    /// <summary>
    /// here this action search for the PlayerController IActionComponent of the Entity
    /// </summary>
    public class ExampleAction : Action
    {
        public override void OnEnter(Entity entity)
        {
            if (entity.HasIActionComponent<PlayerController>(out PlayerController player))
            {
                //here this entity of type PlayerController just enter
            }
        }

        public override void OnExit(Entity entity)
        {
            if (entity.HasIActionComponent<PlayerController>(out PlayerController player))
            {
                //here this entity of type PlayerController just exit
            }
        }

        public override void OnStay(Entity entity)
        {
            if (entity.HasIActionComponent<PlayerController>(out PlayerController player))
            {
                //here this entity of type PlayerController is currently inside
            }
        }
    }
}