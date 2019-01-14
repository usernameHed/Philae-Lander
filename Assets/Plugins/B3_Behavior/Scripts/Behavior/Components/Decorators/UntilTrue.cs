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
	/// Quantizedbit (Sebastian Komorowski)
	/// Until true.
	/// </summary>
	public class UntilTrue : BComponent 
	{
		#region const
		public static readonly string NAME = "untilTrue";
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
					_window = new Window.WinUntilTrue(this);
				}
				return _window;
			}
		}
		#endregion

		#region methods - builds
		public static BComponent Build(XmlNode xmlDoc, BComponent parent, Behaviors behaviorManager)
		{
			return new UntilTrue(parent, behaviorManager, xmlDoc);
		}

		public UntilTrue(BComponent parent, Behaviors behaviorManager, XmlNode xmlDoc)
		{
			this.behaviors = new BComponent[] { GetComponent(this, behaviorManager, xmlDoc) };
			this.parent = parent;
			this.behaviorManager = behaviorManager;
		}

		public UntilTrue()
		{
			behaviors = null;
			parent = null;
			behaviorManager = null;
		}

        public UntilTrue(UntilTrue unit, BComponent newParent = null)
		{
            behaviors = unit.CopyChildren(this);
            parent = newParent;
			behaviorManager = unit.behaviorManager;
		}

        public override BComponent Copy (BComponent newParent = null)
		{
            return new UntilTrue(this, newParent);
		}
		#endregion

		#region methods - update
		public override ReturnResult BehaveResult (ReturnResult lastResult)
		{
            ReturnResult res;
			if (lastResult.result != null && !lastResult.result.Value)
			{
                res = new ReturnResult(null, true);
                res.isUntilTrue = true;
                return res;
			} 

            if (behaviors != null && behaviors.Length > 0)
            {
                res = new ReturnResult(behaviors[0], null);
            }
            else
            {
                res = new ReturnResult(null, lastResult.result);
            }
            res.isUntilTrue = true;
            return res;
		}
		#endregion

		#region methods
		public override void AddChild (BComponent child, int index)
		{
            AddChildAndDeletePrevious(child);
		}

		public override string ToString ()
		{
			return string.Format ("[UntilTrue: {0}]", behaviors[0].ToString());
		}
		#endregion
	}
}