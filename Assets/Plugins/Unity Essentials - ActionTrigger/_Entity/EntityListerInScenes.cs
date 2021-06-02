using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.ActionTrigger.Extensions;
using UnityEssentials.ActionTrigger.PropertyAttribute.ReadOnly;
using UnityEssentials.ActionTrigger.Trigger;
using UnityEssentials.SceneWorkflow;

namespace UnityEssentials.ActionTrigger.entity
{
    /// <summary>
    /// base entity that interact with trigger
    /// </summary>
    public class EntityListerInScenes : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        protected List<Entity> _entityList = new List<Entity>(TriggerZone.DEFAULT_MAX_ELEMENT_IN_ZONE);
        public int EntityListCount { get { return (_entityList.Count); } }
        public Entity GetEntity(int index) { return (_entityList[index]); }

        private void Awake()
        {
            Debug.Log("<color='green'>map entityLister</color>");
            DependencyInjectorSingleton.Instance.Map(this);
        }

        public void Add(Entity entity)
        {
            if (_entityList.AddIfNotContain(entity))
            {
                Debug.Log("<color='green'>entity " + entity.gameObject.name + " added to list</color>");
            }
        }

        public void Remove(Entity entity)
        {
            if (_entityList.Remove(entity))
            {
                Debug.Log("<color='orange'>entity " + entity.gameObject.name + " removed to list</color>");
            }
        }
    }
}