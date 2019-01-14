using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Quantized.Game.Behavior
{
    /// <summary>
    /// Quantizedbit (Komorowski Sebastian)
    /// Group component methods.
    /// </summary>
	public class GroupComponentMethods
	{
		#region values
		private List<MethodInfo> actionsVoid = new List<MethodInfo>();
		private List<MethodInfo> actionsBool = new List<MethodInfo>();
		private List<MethodInfo> actionsEnum = new List<MethodInfo>();
		private List<PropertyInfo> actionProps = new List<PropertyInfo>();
		private string[] _allActions;
		#endregion

		#region properties
		public List<MethodInfo> allVoid
		{
			get
			{
				return actionsVoid;
			}
		}
		
		public List<MethodInfo> allBool
		{
			get
			{
				return actionsBool;
			}
		}
		
		public List<MethodInfo> allEnumerators
		{
			get
			{
				return actionsEnum;
			}
		}

		public List<PropertyInfo> allProperties
		{
			get
			{
				return actionProps;
			}
		}

		public string[] allActions
		{
			get
			{
				return _allActions;
			}
		}
		#endregion

		#region methods
		public GroupComponentMethods(System.Type type)
		{
			List<string> allActionsResult = new List<string>();
			System.Type current = type;

			while (current != null && (current != typeof(MonoBehaviour) && current != typeof(BehaviorAI)))
			{
				FindActions(current, allActionsResult);
                current = current.BaseType;
			}

			allActionsResult.Sort();
			_allActions = allActionsResult.ToArray();
		}

		private void FindActions(System.Type currentType, List<string> allActionsResult)
		{
			MethodInfo[] methods = currentType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			PropertyInfo[] props = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

			foreach (PropertyInfo i in props)
			{
				if (i.CanRead && i.PropertyType == typeof(bool))
				{
					actionProps.Add (i);
					allActionsResult.Add(i.Name);
				}
			}

			foreach (MethodInfo mi in methods)
			{
				if (mi.IsPublic && !mi.IsSpecialName && mi.GetParameters().Length < 1)
				{
					if (mi.ReturnType == typeof(void))
					{
						actionsVoid.Add(mi);
						allActionsResult.Add(mi.Name);
					}
					else if (mi.ReturnType == typeof(bool))
					{
						actionsBool.Add(mi);
						allActionsResult.Add(mi.Name);
					}
					else if (mi.ReturnType == typeof(IEnumerator<bool>))
					{
						actionsEnum.Add(mi);
						allActionsResult.Add(mi.Name);
					}
				}
			}
		}
		#endregion
	}
}