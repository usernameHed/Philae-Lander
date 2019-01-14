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
	/// Action void.
	/// </summary>
	public class ActionVoid : AAction
	{
		#region const
		public static readonly string NAME = "actionVoid";
		#endregion
		
		#region value
		private System.Action action;
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
			System.Action action = null;
			string actionName;
			string componentName;
			bool isError = false;
			string messageError = "";

			FindAttributes(xmlDoc, out actionName, out componentName);
			if (actionName == null) 
			{
				isError = true;
				messageError = "ActionVoid: don't have action attribute";
				throw new NullReferenceException(messageError);
			}
			else
			{
				MonoBehaviour mono = GetRightMono(componentName, behaviorManager.components);
				if (mono != null)
				{
					action = FindMethod<System.Action>(actionName, mono);
				}
				else
				{
					isError = true;
					messageError = string.Format("ActionVoid: wrong component name [{1}] for menthod: [{0}]", actionName, componentName);
					Debug.LogErrorFormat(messageError);
				}
			}

			ActionVoid result = new ActionVoid(parent, action, actionName, componentName, behaviorManager);
			result.isError = isError;
			result.messageError = messageError;
			return result;
		}

		public ActionVoid(BComponent parent, System.Action action, string aName, string cName, Behaviors behaviorManager) 
		{
			this.action = action;
			this.actionName = aName;
			this.componentName = cName;
			this.parent = parent;
			this.behaviorManager = behaviorManager;
		}

		public ActionVoid(string aName, string cName, MethodInfo method, MonoBehaviour mono)
		{
			this.parent = null;
			this.actionName = aName;
			this.componentName = cName;
			this.action = FindMethod<System.Action>(method, mono);
		}

        public ActionVoid(ActionVoid actionVoid, BComponent newParent = null)
		{
            this.parent = newParent;
			this.actionName = actionVoid.actionName;
			this.componentName = actionVoid.componentName;
			this.action = actionVoid.action;
			this.behaviorManager = actionVoid.behaviorManager;
		}

        public override BComponent Copy (BComponent newParent = null)
		{
            return new ActionVoid(this, newParent);
		}
		#endregion

		#region methods - update
		public override ReturnResult BehaveResult (ReturnResult lastResult)
		{
			action();
			return new ReturnResult(null, true, true);
		}
		#endregion

		#region methods
		public override string ToString ()
		{
			return string.Format ("[ActionVoid: {0}]", action.ToString());
		}
		#endregion
	}
}


