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

namespace Quantized.Game.Behavior
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Behaviors.
	/// </summary>
    public class Behaviors
    {
		#region values
        private RootSelector root;
        private float waitTime;
        private float currentTime;
		#endregion

		#region operators
		public static implicit operator bool(Behaviors b)
		{
			return (object)b != null;
		}
		#endregion

		#region properties
		public BComponent rootComponent
		{
			get
			{
				return root;
			}
		}

		public TextAsset xmlAI
		{
			get;
			private set;
		}

		public XmlDocument xmlDocAI
		{
			get 
			{
				return root.ToXml();
			}
		}
		
		public MonoBehaviour[] components
		{
			get;
			private set;
		}

		public BComponent clipboard
		{
			get;
			set;
		}
		#endregion

		#region methods - static
		public static BComponent BuildComponent(XmlNode xmlDoc, BComponent parent, Behaviors behavior)
		{
			string name = xmlDoc.Name;
			if (ActionBool.NAME == name) 
			{
				return ActionBool.Build(xmlDoc, parent, behavior);
			} 
			else if (ActionProperty.NAME == name) 
			{
				return ActionProperty.Build(xmlDoc, parent, behavior);
			} 
			else if (ActionVoid.NAME == name) 
			{
				return ActionVoid.Build(xmlDoc, parent, behavior);
			} 
			else if (ActionEnumerator.NAME == name) 
			{
				return ActionEnumerator.Build(xmlDoc, parent, behavior);
			} 
			else if (RandomSelector.NAME == name) 
			{
				return RandomSelector.Build(xmlDoc, parent, behavior);
			} 
			else if (RootSelector.NAME == name) 
			{
				return RootSelector.Build(xmlDoc, parent, behavior);
			} 
			else if (Selector.NAME == name) 
			{
				return Selector.Build(xmlDoc, parent, behavior);
			} 
			else if (Sequence.NAME == name) 
			{
				return Sequence.Build(xmlDoc, parent, behavior);
			} 
			else if (Inverter.NAME == name) 
			{
				return Inverter.Build(xmlDoc, parent, behavior);
			} 
			else if (UntilTrue.NAME == name) 
			{
				return UntilTrue.Build(xmlDoc, parent, behavior);
			} 
			else if (UntilFalse.NAME == name) 
			{
				return UntilFalse.Build(xmlDoc, parent, behavior);
			} 
			else 
			{
                Debug.LogErrorFormat("I did not find the item: {0}", name);
				return null;
			}
		}
		#endregion

		#region methods
        public Behaviors(TextAsset txt, GameObject obj, float waitTime, bool updateAllTree, Window.UnitArrang arrang = null)
        {
			if (txt == null || txt.text == null || txt.text.Length < 1)
			{
				Debug.LogWarningFormat("No xml tree [{0}]", obj.name);
				return;
			}

			XmlDocument xmlDoc = new XmlDocument();
            if (arrang != null)
            {
                string path = arrang.window.GetAssetTextPath(txt);
                using (TextReader tr = new StreamReader(path))
                {
                     xmlDoc.Load(tr);
                }
            }
            else
            {
                using (StringReader sr = new StringReader(txt.text))
                {
                    xmlDoc.Load(sr);
                }
            }

			XmlNodeList xmlNodes = xmlDoc.SelectNodes("root");
			BComponent root = null;

			components = FindComponents(obj);
			xmlAI = txt;

			foreach (XmlNode xmlNode in xmlNodes) 
			{
				root = BuildComponent(xmlNode, null, this);
			}

			this.root = root as RootSelector;
			if (this.root == null) 
			{
				throw new NullReferenceException(string.Format("Behaviors: xml is invalid - has no root element [{0}]", txt.name));
			}
            this.root.updateAllTree = updateAllTree;

            this.waitTime = waitTime;
        }

		public void Update()
        {
            if (Time.time > currentTime) 
            {
                currentTime = Time.time + waitTime;
				root.BehaveResult(null);
			}
        }
		#endregion

		#region methods - finds
		public MonoBehaviour FindComponent(string name)
		{
			foreach (MonoBehaviour result in components)
			{
				if (result.GetType().Name == name)
				{
					return result;
				}
			}
			return null;
		}

		private MonoBehaviour[] FindComponents(GameObject obj)
		{
			MonoBehaviour[] allComponents = obj.GetComponents<MonoBehaviour>();
			List<MonoBehaviour> result = new List<MonoBehaviour>();
			System.Type current = null;
			
			foreach (MonoBehaviour m in allComponents)
			{
				current = m.GetType();
				if (current == typeof(BehaviorAI))
				{
					continue;
				}
				result.Add(m);
			}

			return result.ToArray();
		}
		#endregion
    }
}
