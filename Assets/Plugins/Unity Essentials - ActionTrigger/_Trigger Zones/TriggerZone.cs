using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.ActionTrigger.Actions;
using UnityEssentials.ActionTrigger.entity;
using UnityEssentials.ActionTrigger.Extensions;
using UnityEssentials.ActionTrigger.PropertyAttribute.ReadOnly;
using UnityEssentials.CrossSceneReference;

namespace UnityEssentials.ActionTrigger.Trigger
{
    public abstract class TriggerZone : MonoBehaviour
    {
        public const int DEFAULT_MAX_ELEMENT_IN_ZONE = 100;

        [SerializeField]
        private List<GuidReference> _actions = new List<GuidReference>();
        public int ActionCount { get { return (_actions.Count); } }
        public GameObject GetActionByIndex(int index) { return (_actions[index].gameObject); }
        public void SetActionByIndex(GuidReference reference, int index) { _actions[index] = reference; }

#if UNITY_EDITOR
        [Header("EditorOnly")]
        public bool ShowLinksToActions = false;
        public bool AutomaticlySetListFromSceneActions = false;
#endif

        private List<Entity> _entityInside = new List<Entity>(TriggerZone.DEFAULT_MAX_ELEMENT_IN_ZONE);

        /// <summary>
        /// Layer of detection (it's automatic for physics zone, but not for non physics zone)
        /// </summary>
        protected int _currentLayer;

        protected virtual void Awake()
        {
            _currentLayer = gameObject.layer;
        }

        protected void AddIfNotContain(Entity entity)
        {
            if (_entityInside.AddIfNotContain(entity))
            {
                Debug.Log("<color='green'>Entity " + entity.gameObject.name + " enter " + gameObject.name + "</color>", entity.gameObject);
                CallActionsOnEnter(entity);
            }
        }

        protected void Add(Entity entity)
        {
            _entityInside.Add(entity);
            Debug.Log("<color='green'>Entity " + entity.gameObject.name + " enter " + gameObject.name + "</color>", entity.gameObject);
            CallActionsOnEnter(entity);
        }

        private void CallActionsOnEnter(Entity entity)
        {
            for (int i = 0; i < _actions.Count; i++)
            {
                Action action = _actions[i].gameObject.GetComponent<Action>();
                if (action)
                {
                    action.DoActionOnEnter(entity);
                }
            }
        }

        protected void Remove(Entity entity)
        {
            if (_entityInside.Remove(entity))
            {
                Debug.Log("<color='orange'>Entity " + entity.gameObject.name + " removed from zone " + gameObject.name + "</color>", entity.gameObject);
                CallActionsOnExit(entity);
            }
        }

        private void CallActionsOnExit(Entity entity)
        {
            for (int i = 0; i < _actions.Count; i++)
            {
                Action action = _actions[i].gameObject.GetComponent<Action>();
                if (action)
                {
                    action.DoActionOnExit(entity);
                }
            }
        }

        protected bool IsEntityInside(Entity entity)
        {
            return (_entityInside.Contains(entity));
        }
    }
}