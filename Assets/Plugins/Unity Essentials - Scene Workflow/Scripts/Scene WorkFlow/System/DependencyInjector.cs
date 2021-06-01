using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.SceneWorkflow
{
    /// <summary>
    ///     Generic dependency injector
    /// </summary>
    public class DependencyInjector
    {
        private Dictionary<Type, object> _mappingTypeToObject;

        public DependencyInjector()
        {
            _mappingTypeToObject = new Dictionary<Type, object>();
        }
            
        /// <summary>
        ///     Get a specific system by asking for a type
        /// </summary>
        /// <typeparam name="T">The wanted type</typeparam>
        /// <returns>The system linked to T, null if it doesn't exists</returns>
        public T Get<T>()
        {
            if (!_mappingTypeToObject.ContainsKey(typeof(T)))
            {
                Debug.LogError("Default T : " + typeof(T).FullName);
                return default(T);
            }

            return (T) _mappingTypeToObject[typeof(T)];
        }

        /// <summary>
        ///     Add a mapping from a specific type to
        ///     a specific object
        /// </summary>
        /// <typeparam name="T">The type that will be mapped to the system</typeparam>
        /// <param name="systemToMap">The system to map</param>
        public void Map<T>(T systemToMap)
        {
            _mappingTypeToObject.Add(typeof(T), systemToMap);
        }

        /// <summary>
        ///     Remove a mapping from a specific type to
        ///     a specific object
        /// </summary>
        /// <typeparam name="T">The type that will be removed from the system</typeparam>
        /// <returns>True if the type has been removed, false otherwise</returns>
        public bool Unmap<T>()
        {
            return _mappingTypeToObject.Remove(typeof(T));
        }
    }
}
