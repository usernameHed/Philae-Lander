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
	/// Action bool.
	/// </summary>
	public class ActionBool : AAction
    {
		#region const
		public static readonly string NAME = "actionBool";
		#endregion

		#region value
		private System.Func<bool> action;
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

		#region methods - builds
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
				messageError = string.Format("ActionBool: don't have action attribute [{0}]", actionName);
				throw new NullReferenceException(messageError);
			}
			else
			{
				MonoBehaviour mono = GetRightMono(componentName, behaviorManager.components);
				if (mono != null)
				{
					action = FindMethod<System.Func<bool>>(actionName, mono);
				}
				else 
				{
					isError = true;
					messageError = string.Format("ActionBool: wrong component name [{1}] for menthod: [{0}]", actionName, componentName);
					Debug.LogError(messageError);
				}
			}

			ActionBool result = new ActionBool(parent, action, actionName, componentName, behaviorManager);
			result.isError = isError;
			result.messageError = messageError;
			return result;
		}

		public ActionBool(BComponent parent, System.Func<bool> action, string aName, string cName, Behaviors behaviorManager) 
		{
			this.action = action;
			this.actionName = aName;
			this.componentName = cName;
			this.parent = parent;
			this.behaviorManager = behaviorManager;
		}

		public ActionBool(string aName, string cName, MethodInfo method, MonoBehaviour mono)
		{
			this.parent = null;
			this.actionName = aName;
			this.componentName = cName;
			this.action = FindMethod<System.Func<bool>>(method, mono);
		}

        public ActionBool(ActionBool actionBool, BComponent newParent = null)
		{
            this.parent = newParent;
			this.actionName = actionBool.actionName;
			this.componentName = actionBool.componentName;
			this.action = actionBool.action;
			this.behaviorManager = actionBool.behaviorManager;
		}

        public override BComponent Copy (BComponent newParent = null)
		{
            return new ActionBool(this, newParent);
		}
		#endregion

		#region methods - update
		public override ReturnResult BehaveResult (ReturnResult lastResult)
		{
			return  new ReturnResult(null, action(), true);
		}
		#endregion

		#region methods
		public override string ToString ()
		{
			return string.Format ("[ActionBool: {0}]", action.ToString());
		}
		#endregion
    }
}


