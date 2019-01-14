using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Quantized.Game.Behavior
{
    /// <summary>
    /// Quantizedbit (Komorowski Sebastian)
    /// Components manager.
    /// </summary>
    public class ComponentsManager
    {
        #region values
        private Dictionary<string, GroupComponentMethods> groups = new Dictionary<string, GroupComponentMethods>();
		private string[] _componentsNames;
        #endregion

        #region properties
		public string[] componentsNames
        {
            get
            {
				return _componentsNames;
            }
        }
        #endregion

        #region methods
        public ComponentsManager(GameObject go)
        {
            MonoBehaviour[] allComponents = go.GetComponents<MonoBehaviour>();
            System.Type current = null;

            foreach (MonoBehaviour m in allComponents)
            {
				if (m.GetType() == typeof(BehaviorAI))
				{
					continue;
				}

                current = m.GetType();
                groups.Add(current.Name, new GroupComponentMethods(current));
            }

			List<string> result = new List<string>(groups.Keys.ToList());
			result.Sort();
			_componentsNames = result.ToArray();
        }

		public GroupComponentMethods GetActionsFromComponent(string componentName)
		{
			GroupComponentMethods findType;
			if (groups.TryGetValue(componentName, out findType))
			{
				return findType;
			}

			return null;
		}
        #endregion
    }
}