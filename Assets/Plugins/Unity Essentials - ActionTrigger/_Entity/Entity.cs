using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.ActionTrigger.PropertyAttribute.ReadOnly;
using UnityEssentials.ActionTrigger.Trigger;
using UnityEssentials.SceneWorkflow;

namespace UnityEssentials.ActionTrigger.entity
{
    /// <summary>
    /// entity that interact with trigger
    /// </summary>
    public sealed class Entity : MonoBehaviour
    {
        private IActionComponents[] _actonComponents;
        private EntityListerInScenes _lister;

        private void OnEnable()
        {
            _actonComponents = gameObject.GetComponentsInChildren<IActionComponents>();
            if (DependencyInjectorSingleton.Instance != null)
            {
                _lister = DependencyInjectorSingleton.Instance.Get<EntityListerInScenes>();
                _lister.Add(this);
            }
        }

        private void OnDisable()
        {
            if (_lister != null)
            {
                _lister.Remove(this);
            }
        }

        public bool HasIActionComponent<T>(out T finalComponent)
        {
            finalComponent = default(T);
            for (int i = 0; i < _actonComponents.Length; i++)
            {
                if (_actonComponents[i] is T component)
                {
                    finalComponent = component;
                    return (true);
                }
            }
            return (false);
        }
    }
}