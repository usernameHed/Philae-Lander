using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Quantized.Game.Behavior.Components.Actions
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Action property.
	/// </summary>
	public class ActionProperty : AAction 
	{
		#region const
		public static readonly string NAME = "actionProperty";
		#endregion
		
		#region value
		private System.Func<bool> action = null;
		#endregion

		#region properties
		public override string name
		{
			get 
			{
				return NAME;
			}
		}
		#endregion

		#region methods - build
		public static BComponent Build(XmlNode xmlDoc, BComponent parent, Behaviors behaviorManager)
		{
			System.Func<bool> action = null;
			string actionName;
			string componentName;
			bool isError = false;
			string messageError = "";

			FindAttributes(xmlDoc, out actionName, out componentName);
			if (actionName == null) 
			{
				isError = true;
				messageError = "ActionProperty: don't have action attribute";
				throw new NullReferenceException(messageError);
			}
			else
			{
				MonoBehaviour mono = GetRightMono(componentName, behaviorManager.components);
				if (mono != null)
				{
					action = FindProperty<System.Func<bool>>(actionName, mono);

                    if (action == null)
                    {
                        isError = true;
                        messageError = string.Format("ActionProperty: I can't find property [{0}]", actionName);
                    }
				}
				else
				{
					isError = true;
					messageError = string.Format("ActionProperty: wrong component name [{1}] for menthod: [{0}]", actionName, componentName);
					Debug.LogError(messageError);
				}
			}

			ActionProperty result = new ActionProperty(parent, action, actionName, componentName, behaviorManager);
			result.isError = isError;
			result.messageError = messageError;
			return result;
		}

		public ActionProperty(BComponent parent, System.Func<bool> action, string aName, string cName, Behaviors behaviorManager) 
		{
			this.action = action;
			this.actionName = aName;
			this.componentName = cName;
			this.parent = parent;
			this.behaviorManager = behaviorManager;
		}

		public ActionProperty(string aName, string cName, PropertyInfo property, MonoBehaviour mono)
		{
			this.parent = null;
			this.actionName = aName;
			this.componentName = cName;
			this.action = FindProperty<System.Func<bool>>(property, mono);
		}

        public ActionProperty(ActionProperty actionProp, BComponent newParent = null)
		{
            this.parent = newParent;
			this.actionName = actionProp.actionName;
			this.componentName = actionProp.componentName;
			this.action = actionProp.action;
			this.behaviorManager = actionProp.behaviorManager;
		}

        public override BComponent Copy (BComponent newParent = null)
		{
            return new ActionProperty(this, newParent);
		}
		#endregion

		#region methods - update
		public override ReturnResult BehaveResult (ReturnResult lastResult)
		{
			return action() ? new ReturnResult(null, true, true) : new ReturnResult(null, false, true);
		}
		#endregion

		#region methods
		public override string ToString ()
		{
			return string.Format ("[ActionPropBoll: {0}]", action.ToString());
		}
		#endregion
	}
}