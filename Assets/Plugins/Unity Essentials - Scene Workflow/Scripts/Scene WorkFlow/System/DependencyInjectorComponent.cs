using UnityEngine;

namespace UnityEssentials.SceneWorkflow
{
    /// <summary>
    ///     Generic dependency injector using a unity component
    /// </summary>
    public class DependencyInjectorComponent : MonoBehaviour
    {
        private DependencyInjector _injector;

        protected virtual void Awake()
        {
            _injector = new DependencyInjector();
        }

        /// <summary>
        ///     Get a specific system by asking for a type
        /// </summary>
        /// <typeparam name="T">The wanted type</typeparam>
        /// <returns>The system linked to T, null if it doesn't exists</returns>
        public T Get<T>()
        {
            return _injector.Get<T>();
        }

        /// <summary>
        ///     Add a mapping from a specific type to
        ///     a specific object
        /// </summary>
        /// <typeparam name="T">The type that will be mapped to the system</typeparam>
        /// <param name="systemToMap">The system to map</param>
        public void Map<T>(T systemToMap)
        {
            _injector.Map<T>(systemToMap);
        }

        /// <summary>
        ///     Remove a mapping from a specific type to
        ///     a specific object
        /// </summary>
        /// <typeparam name="T">The type that will be removed from the system</typeparam>
        /// <returns>true if it succeed to remove it, false otherwise</returns>
        public bool Unmap<T>()
        {
            return _injector.Unmap<T>();
        }
    }
}
