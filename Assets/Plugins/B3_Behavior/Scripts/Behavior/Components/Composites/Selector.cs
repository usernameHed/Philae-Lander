using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using Quantized.Game.Behavior.Components;
using Quantized.Game.Behavior.Components.Composites;
using Quantized.Game.Behavior.Components.Actions;
using Quantized.Game.Behavior.Components.Decorators;

namespace Quantized.Game.Behavior.Components.Composites
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Selector.
	/// </summary>
    public class Selector : BComponent
    {
		#region const
		public static readonly string NAME = "selector";
		#endregion

		#region values
		private int index = 0;
		private Window.UnitWindow _window;
		#endregion

		#region properties
		public override string name
		{
			get 
			{
				return NAME;
			}
		}

		public override Window.UnitWindow window 
		{
			get 
			{
				if (_window == null)
				{
					_window = new Window.WinSelector(this);
				}
				return _window;
			}
		}
		#endregion

		#region methods - builds
		public static BComponent Build(XmlNode xmlDoc, BComponent parent, Behaviors behaviorManager)
		{
			return new Selector(parent, behaviorManager, xmlDoc);
		}

		public Selector(BComponent parent, Behaviors behaviorManager, XmlNode xmlDoc)
        {
			this.behaviors = GetComponents(this, behaviorManager, xmlDoc);
			this.parent = parent;
			this.behaviorManager = behaviorManager;
			index = 0;
		}

		public Selector()
		{
			this.parent = null;
			behaviors = null;
			index = 0;
			behaviorManager = null;
		}

        public Selector(Selector select, BComponent newParent = null)
		{
            this.parent = newParent;
            behaviors = select.CopyChildren(this);
			index = 0;
			this.behaviorManager = select.behaviorManager;
		}

        public override BComponent Copy (BComponent newParent = null)
		{
            return new Selector(this, newParent);
		}
		#endregion

		#region methods - update
		public override ReturnResult BehaveResult (ReturnResult lastResult)
		{
            if (behaviors == null || behaviors.Length < 1)
            {
                return new ReturnResult(null, lastResult.result);
            }

			if (lastResult.result != null && lastResult.result.Value)
			{
				index = 0;
				return new ReturnResult(null, true);
			}

			if (index >= behaviors.Length)
			{
				index = 0;
				return new ReturnResult(null, false);
			}
			else 
			{
				return new ReturnResult(behaviors[index++], null);
			}
		}
		#endregion

		#region methods
		public override string ToString ()
		{
			string result = "\n";
			foreach (BComponent c in behaviors) 
			{
				result = result + "\n" + c.ToString();
			}
			return string.Format ("[Selector: {0} \n]", result);
		}
		#endregion
    }
}
