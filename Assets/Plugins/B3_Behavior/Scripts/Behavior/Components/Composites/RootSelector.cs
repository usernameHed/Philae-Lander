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
	/// Root selector.
	/// </summary>
    public class RootSelector : BComponent
    {
		#region const
		public static readonly string NAME = "root";
		#endregion

		#region values
        public bool updateAllTree;

		private Window.UnitWindow _window;

		private LinkedList<ReturnResult> results = new LinkedList<ReturnResult>();
		private ReturnResult current = null;
		private ReturnResult lastRes = new ReturnResult(null, null);
		private ReturnResult nextRes = null;
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
					_window = new Window.WinRootSelector(this);
				}
				return _window;
			}
		}
		#endregion

		#region methods - builds
		public static BComponent Build(XmlNode xmlDoc, BComponent parent, Behaviors behaviorManager)
		{
			return new RootSelector(parent, behaviorManager, xmlDoc);
		}

		public RootSelector(BComponent parent, Behaviors behaviorManager, XmlNode xmlDoc)
        {
			this.behaviors = GetComponents(this, behaviorManager, xmlDoc);
			this.parent = parent;
			this.behaviorManager = behaviorManager;
		}
			
        public override BComponent Copy (BComponent newParent = null)
		{
			return this;
		}
		#endregion

		#region methods - update
		public override ReturnResult BehaveResult (ReturnResult lastResult)
		{
            usedUnitColor = 1f;

			if (current == null) 
			{
				current = new ReturnResult(behaviors[0], null);
				lastRes = new ReturnResult(null, null);
			} 

            bool isOut = false;

            do
            {
                nextRes = current.componentNext.BehaveResult(lastRes);
                current.componentNext.usedUnitColor = 1f;

                if (nextRes.result == null && nextRes.componentNext != null)
                {
                    results.AddLast(current);
                    lastRes = current;
                    current = nextRes;
                }
                else
                {
                    isOut = nextRes.isAction && !updateAllTree;

                    if (updateAllTree)
                    {
                        if (current.isUntilTrue && nextRes.result.Value)
                        {
                            isOut = true;
                        }
                        else if (current.isUntilFalse && !nextRes.result.Value)
                        {
                            isOut = true;
                        }
                    }

                    lastRes = nextRes;
                    current = null;
                    if (results.Count > 0)
                    {
                        current = results.Last.Value;
                        results.RemoveLast();
                    }
                }

                if (isOut)
                {
                    break;
                }
            }
            while (current != null);
                
			return null;
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
			return string.Format ("[RootSelector: {0}\n]", result);
		}

		public override void AddChild (BComponent child, int index)
		{
            AddChildAndDeletePrevious(child);
		}
		#endregion
    }
}
