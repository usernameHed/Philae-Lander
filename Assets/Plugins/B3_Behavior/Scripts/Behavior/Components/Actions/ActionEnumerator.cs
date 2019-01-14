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
	/// Action enumerator.
	/// </summary>
	public class ActionEnumerator : AAction
	{
		#region const
		public static readonly string NAME = "actionEnumerator";
		#endregion
		
		#region value
		private System.Func<IEnumerator<bool>> action;
		private IEnumerator<bool> enumerator = null;
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
			System.Func<IEnumerator<bool>> action = null;
			string actionName;
			string componentName;
			bool isError = false;
			string messageError = "";

			FindAttributes(xmlDoc, out actionName, out componentName);

			if (actionName == null) 
			{
				isError = true;
				messageError = "ActionEnumerator: don't have action attribute";
				throw new NullReferenceException(messageError);
			}
			else 
			{
				MonoBehaviour mono = GetRightMono(componentName, behaviorManager.components);
				if (mono != null)
				{
					action = FindMethod<System.Func<IEnumerator<bool>>>(actionName, mono);
				}
				else
				{
					isError = true;
					messageError = string.Format("ActionEnumerator: wrong component name [{1}] for menthod: [{0}]", actionName, componentName);
					Debug.LogError(messageError);
				}
			}
			
			ActionEnumerator result = new ActionEnumerator(parent, action, actionName, componentName, behaviorManager);
			result.isError = isError;
			result.messageError = messageError;
			return result;
		}

		public ActionEnumerator(BComponent parent, System.Func<IEnumerator<bool>> action, string aName, string cName, Behaviors behaviorManager) 
		{
			this.parent = parent;
			this.action = action;
			this.actionName = aName;
			this.componentName = cName;
			this.behaviorManager = behaviorManager;
			if (action != null)
			{
				enumerator = action();
			}
		}

		public ActionEnumerator(string aName, string cName, MethodInfo method, MonoBehaviour mono)
		{
			this.parent = null;
			this.actionName = aName;
			this.componentName = cName;
			this.action = FindMethod<System.Func<IEnumerator<bool>>>(method, mono);
			this.enumerator = this.action();
		}

        public ActionEnumerator(ActionEnumerator actionEnum, BComponent newParent = null)
		{
            this.parent = newParent;
			this.actionName = actionEnum.actionName;
			this.componentName = actionEnum.componentName;
			this.action = actionEnum.action;
			this.enumerator = this.action();
			this.behaviorManager = actionEnum.behaviorManager;
		}

        public override BComponent Copy (BComponent newParent = null)
		{
            return new ActionEnumerator(this, newParent);
		}
		#endregion

		#region methods - update
		public override ReturnResult BehaveResult (ReturnResult lastResult)
		{
			if (enumerator.MoveNext()) {
				return enumerator.Current ? new ReturnResult(null, true, true) : new ReturnResult(null, false, true);
			} else {
				enumerator = action();
				return new ReturnResult(null, true, true);
			}
		}
		#endregion

		#region methods
		public override string ToString ()
		{
			return string.Format ("[ActionEnumerator: {0}]", action.ToString());
		}
		#endregion
	}
}