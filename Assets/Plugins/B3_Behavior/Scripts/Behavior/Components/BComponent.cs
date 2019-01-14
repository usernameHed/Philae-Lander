using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using UnityEngine;
using Quantized.Game.Behavior.Components.Actions;
using Quantized.Game.Behavior.Components.Composites;
using Quantized.Game.Behavior.Components.Decorators;

namespace Quantized.Game.Behavior.Components
{
    /// <summary>
    /// Quantizedbit (Komorowski Sebastian)
    /// BComponent.
    /// </summary>
    public abstract class BComponent
    {
		#region values
		protected BComponent[] behaviors;
        private float _usedUnitColor = 0f;
		#endregion

		#region properties
		public abstract string name
		{
			get;
		}

		public BComponent parent
		{
			get;
			protected set;
		}

		public BComponent[] childrenComponents
		{
			get 
			{
				return behaviors;
			}
		}

		public Location locationArrang
		{
			get;
			set;
		}

		public abstract Window.UnitWindow window
		{
			get;
		}

		public Behaviors behaviorManager
		{
			get;
			protected set;
		}

        public float usedUnitColor
        {
            get
            {
                return _usedUnitColor;
            }
            set
            {
                _usedUnitColor = Mathf.Clamp(value, 0f, 1f);
            }
        }
		#endregion

		#region methods - abstract
		public virtual ReturnResult BehaveResult(ReturnResult lastResult)
		{
			return null;
		}
		#endregion

		#region methods
		public static BComponent[] GetComponents(BComponent parent, Behaviors behaviorManager, XmlNode xmlDoc) 
		{
			List<BComponent> components = new List<BComponent>();
			BComponent current = null;
			foreach (XmlNode x in xmlDoc) 
			{
				current = Behaviors.BuildComponent(x, parent, behaviorManager);
				if (current != null) 
				{
					components.Add(current);
				}
			}
			return components.ToArray();
		}
		
		public static BComponent GetComponent(BComponent parent, Behaviors behaviorManager, XmlNode xmlDoc) 
		{
			BComponent current = null;
			foreach (XmlNode x in xmlDoc) 
			{
				current = Behaviors.BuildComponent(x, parent, behaviorManager);
				if (current != null) 
				{
					return current;
				}
			}
			return null;
		}

        protected void KeepVisible()
		{
            window.KeepVisible();

            if (behaviors != null)
			{
				for (int i = 0; i < behaviors.Length; ++i) 
				{
                    behaviors[i].KeepVisible();
				}
			}
		}
		#endregion

		#region methods - add, compy and remove
		public virtual void AddChild(BComponent child, int index)
		{
			List<BComponent> result;

			if (behaviors == null)
			{
				result = new List<BComponent>();
			}
			else
			{
				result = new List<BComponent>(behaviors);
			}

			child.parent = this;
			child.behaviorManager = behaviorManager;
            child.KeepVisible();

			result.Insert(index, child);
			behaviors = result.ToArray();

			window.WaitFadeOffFroUnits ();
		}


        protected void AddChildAndDeletePrevious(BComponent unit)
		{
			unit.parent = this;
			unit.behaviorManager = behaviorManager;
            unit.KeepVisible();

			behaviors = new BComponent[] { unit };
			window.WaitFadeOffFroUnits ();
		}

		public virtual void RemoveChild(BComponent child)
		{
			if (behaviors != null)
			{
				List<BComponent> result = new List<BComponent>(behaviors);
				if (result.Remove(child))
				{
					behaviors = result.ToArray();
				}
			}
		}

		public virtual void RemoveAllChildren()
		{
			behaviors = null;
		}

        public virtual BComponent[] CopyChildren(BComponent newParent)
		{
			if (behaviors != null)
			{
				BComponent[] result = new BComponent[behaviors.Length];
				for (int i = 0; i < behaviors.Length; ++i)
				{
					if (behaviors[i] != null)
					{
                        result[i] = behaviors[i].Copy(newParent);
					}
				}
				return result;
			}
			return null;
		}

        public abstract BComponent Copy(BComponent newParent = null);
		#endregion

		#region methods - 
		public virtual XmlDocument ToXml()
		{
			XmlDocument result = new XmlDocument();
			XmlNode current = ToXmlNode(result);
			result.AppendChild(current);

			if (behaviors != null && behaviors.Length > 0)
			{
				foreach (BComponent c in behaviors)
				{
					if (c != null)
					{
						c.MakeXml(current);
					}
				}
			}

			return result;
		}

		public virtual StringWriter ToXmlString()
		{
			XmlDocument doc = ToXml();
			StringWriter result = new StringWriter();
			XmlWriterSettings settings = new XmlWriterSettings
												{
													Indent = true,
													IndentChars = "  ",
													NewLineChars = "\r\n",
													NewLineHandling = NewLineHandling.Replace
												};

			XmlWriter writer = XmlWriter.Create(result, settings);

			doc.Save(writer);
			return result;
		}

		protected virtual XmlNode ToXmlNode (XmlDocument doc)
		{
			return doc.CreateElement(name);
		}

		protected virtual void MakeXml(XmlNode xml)
		{
			XmlNode current = ToXmlNode(xml.OwnerDocument);
			xml.AppendChild(current);

			if (behaviors != null && behaviors.Length > 0)
			{
				foreach (BComponent c in behaviors)
				{
					if (c != null) 
					{
						c.MakeXml(current);
					}
				}
			}
		}
		#endregion
    }
}
