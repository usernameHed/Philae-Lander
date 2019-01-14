using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace Quantized.Game.Behavior.Components.Actions
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// A action.
	/// </summary>
	public abstract class AAction : BComponent
	{
		#region def
		public static readonly string ATTRIBUTE_ACTION = "action";
		public static readonly string ATTRIBUTE_COMPONENT_NAME = "componentName";
		#endregion

		#region values
		protected Window.UnitWindow _window;
		#endregion

		#region properties
		public string actionName
		{
			get;
			set;
		}

		public string componentName
		{
			get;
			set;
		}

		public bool isError
		{
			get;
			protected set;
		}

		public string messageError
		{
			get;
			protected set;
		}

		public override Window.UnitWindow window
		{
			get 
			{
				if (_window == null)
				{
					_window = new Window.WinAction(this);
				}
				return _window;
			}
		}
		#endregion

		#region methods - find delegate
		public static T FindMethod<T>(string name, MonoBehaviour mono) where T : class
		{
			Type type = mono.GetType();
			MethodInfo methods = type.GetMethod(name);
			T result = null;
			try 
			{
				result = (T)(object)Delegate.CreateDelegate(typeof(T), mono, methods);
			} 
			catch (System.Exception e)
			{
				Debug.LogErrorFormat(" --- {0}; {1}; {2}", e.Message, mono.name, name);
			}

			return result;
		}

		public static T FindMethod<T>(MethodInfo methods, MonoBehaviour mono) where T : class
		{
			T result = null;
			try 
			{
				result = (T)(object)Delegate.CreateDelegate(typeof(T), mono, methods);
			} 
			catch (System.Exception e)
			{
				Debug.LogErrorFormat(" --- {0}; {1}", e.Message, mono.name);
			}
			
			return result;
		}

		public static T FindProperty<T>(string name, MonoBehaviour mono) where T : class
		{
			Type type = mono.GetType();
			PropertyInfo property = type.GetProperty(name);
            if (property == null)
            {
                Debug.LogErrorFormat(" --- property don't find: {0}", name);
                return null;
            }

			MethodInfo methods = property.GetGetMethod();
			T result = null;
			try 
			{
				result = (T)(object)Delegate.CreateDelegate(typeof(T), mono, methods);
			} 
			catch (System.Exception e)
			{
				Debug.LogErrorFormat(" --- {0}", e.Message);
			}

			return result;
		}

		public static T FindProperty<T>(PropertyInfo property, MonoBehaviour mono) where T : class
		{
			MethodInfo methods = property.GetGetMethod();
			T result = null;
			try 
			{
				result = (T)(object)Delegate.CreateDelegate(typeof(T), mono, methods);
			} 
			catch (System.Exception e)
			{
				Debug.LogErrorFormat(" --- {0}", e.Message);
			}
			
			return result;
		}
		#endregion

		#region methods - 
		protected static MonoBehaviour GetRightMono(string componentName, MonoBehaviour[] types)
		{
			foreach (MonoBehaviour t in types)
			{
				if (t.GetType().Name == componentName)
				{
					return t;
				}
			}
			return null;
		}

		public static void FindAttributes(XmlNode xmlDoc, out string actionName, out string componentName)
		{
			actionName = null;
			componentName = null;

			foreach (XmlAttribute a in xmlDoc.Attributes) 
			{
				if (a.Name == ATTRIBUTE_ACTION) 
				{
					actionName = a.Value;
				} 
				else if (a.Name == ATTRIBUTE_COMPONENT_NAME)
				{
					componentName = a.Value;
				}
			}
		}

		protected override XmlNode ToXmlNode (XmlDocument doc)
		{
			XmlElement result = doc.CreateElement(name);
			XmlAttribute attAction = doc.CreateAttribute(ATTRIBUTE_ACTION);
			XmlAttribute attComponent = doc.CreateAttribute(ATTRIBUTE_COMPONENT_NAME);

			attAction.Value = actionName;
			attComponent.Value = componentName;

			result.Attributes.Append(attComponent);
			result.Attributes.Append(attAction);
			return result;
		}
		#endregion
	}
}