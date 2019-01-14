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

namespace Quantized.Game.Behavior.Components.Decorators
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Inverter.
	/// </summary>
    public class Inverter : BComponent
    {
		#region def
		public static readonly string NAME = "inverter";
		#endregion

		#region values
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
					_window = new Window.WinInverter(this);
				}
				return _window;
			}
		}
		#endregion

		#region methods - builds
		public static BComponent Build(XmlNode xmlDoc, BComponent parent, Behaviors behaviorManager)
		{
			return new Inverter(parent, behaviorManager, xmlDoc);
		}

		public Inverter(BComponent parent, Behaviors behaviorManager, XmlNode xmlDoc) 
        {
			this.behaviors = new BComponent[] { GetComponent(this, behaviorManager, xmlDoc) };
			this.parent = parent;
			this.behaviorManager = behaviorManager;
        }

		public Inverter()
		{
			behaviors = null;
			parent = null;
			behaviorManager = null;
		}

        public Inverter (Inverter unit, BComponent newParent = null)
		{
            behaviors = unit.CopyChildren(this);
            parent = newParent;
			behaviorManager = unit.behaviorManager;
		}

        public override BComponent Copy (BComponent newParent = null)
		{
            return new Inverter(this, newParent);
		}
		#endregion 

		#region methods - update
		public override ReturnResult BehaveResult (ReturnResult lastResult)
		{
            if (behaviors == null || behaviors.Length < 1)
            {
                return new ReturnResult(null, lastResult.result);
            }

			if (lastResult.result != null)
			{
				if (lastResult.result.Value) 
                {
					return new ReturnResult(null, false);
				} 
                else 
                {
					return new ReturnResult(null, true);
				}
			}

			return new ReturnResult(behaviors[0], null);
		}
		#endregion

		#region methods
		public override void AddChild (BComponent child, int index)
		{
            AddChildAndDeletePrevious(child);
		}

		public override string ToString ()
		{
			return string.Format ("[Inverter: {0}]", behaviors[0].ToString());
		}
		#endregion
    }
}
