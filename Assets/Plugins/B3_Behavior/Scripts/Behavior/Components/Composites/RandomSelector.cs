using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using UnityEngine;
using Quantized.Game.Behavior.Components;
using Quantized.Game.Behavior.Components.Composites;
using Quantized.Game.Behavior.Components.Actions;
using Quantized.Game.Behavior.Components.Decorators;

namespace Quantized.Game.Behavior.Components.Composites
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Random selector.
	/// </summary>
    public class RandomSelector : BComponent
    {
		#region def
		public static readonly string NAME = "randomSelector";
        public static readonly string ATTRIBUTE_TIME_MIN = "timeMin";
        public static readonly string ATTRIBUTE_TIME_MAX = "timeMax";
        private static readonly float TIME_MIN = 0.2f;
        private static readonly float TIME_MAX = 1f;

        public struct HoldTime
        {
            public float min;
            public float max;

            public HoldTime(float min, float max)
            {
                this.min = min;
                this.max = max;
            }
        }
		#endregion

		#region values
        private float timeMin;
        private float timeMax;

		private float currentTime;
		private int currentIndex;
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
					_window = new Window.WinRandomSelector(this);
				}
				return _window;
			}
		}

        public HoldTime holdTime
        {
            get
            {
                HoldTime result;
                result.min = timeMin;
                result.max = timeMax;
                return result;
            }
            set
            {
                timeMin = value.min;
                timeMax = value.max;
            }
        }

		protected int randomIndex
		{
			get 
			{
                return RandomRange(0, (behaviors != null && behaviors.Length > 0 ? behaviors.Length - 1 : 0));
			}
		}
		
        // od 0.2 do 1
		protected float randomTime
		{
			get 
			{
                return 4f * RandomRange(timeMin, timeMax) + Time.time;
			}
		}
		#endregion

		#region methods - builds
		public static BComponent Build(XmlNode xmlDoc, BComponent parent, Behaviors behaviorManager)
		{
			return new RandomSelector(parent, behaviorManager, xmlDoc);
		}

		public RandomSelector(BComponent parent, Behaviors behaviorManager, XmlNode xmlDoc) 
        {
			this.parent = parent;
			this.behaviors = GetComponents(this, behaviorManager, xmlDoc);
			this.behaviorManager = behaviorManager;
			currentIndex = 0;
			currentTime = 0f;

            FindAttributesTime(xmlDoc, out timeMin, out timeMax);
            FirstSeed();
		}

		public RandomSelector()
		{
			parent = null;
			behaviors = null;
			currentIndex = 0;
			currentTime = 0f;
			behaviorManager = null;
            timeMin = TIME_MIN;
            timeMax = TIME_MAX;

            FirstSeed();
		}

        public RandomSelector(RandomSelector random, BComponent newParent = null)
		{
            parent = newParent;
			currentIndex = 0;
			currentTime = 0f;
            behaviors = random.CopyChildren(this);
			behaviorManager = random.behaviorManager;
            timeMin = random.timeMin;
            timeMax = random.timeMax;

            FirstSeed();
		}

        public override BComponent Copy (BComponent newParent = null)
		{
            return new RandomSelector(this, newParent);
		}
		#endregion

		#region methods - update
		public override ReturnResult BehaveResult (ReturnResult lastResult)
		{
			if (Time.time > currentTime) 
			{
				currentIndex = randomIndex;
				currentTime = randomTime;
			}

            if (behaviors != null && behaviors.Length > 0)
            {
                return new ReturnResult(behaviors[currentIndex], lastResult.result);
            }
            else
            {
                return new ReturnResult(null, lastResult.result);
            }
		}
		#endregion

        #region methods - random
        private static void FirstSeed()
        {
            UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks + (int)(UnityEngine.Random.value * 100f);
        }

        public static float RandomRange(float from, float to)
        {
            float distance = Mathf.Abs(to - from) * 2f;
            float value = distance * UnityEngine.Random.value;
            float result = from + value * 0.5f;
            return Mathf.Clamp(result, from, to);
        }

        public static int RandomRange(int from, int to)
        {
            return Mathf.RoundToInt(RandomRange((float)from, (float)to));
        }
        #endregion

		#region methods
        public void FindAttributesTime(XmlNode xmlDoc, out float timeMin, out float timeMax)
        {
            float val = 0;
            timeMin = TIME_MIN;
            timeMax = TIME_MAX;

            foreach (XmlAttribute a in xmlDoc.Attributes) 
            {
                if (a.Name == ATTRIBUTE_TIME_MIN) 
                {
                    if (float.TryParse(a.Value, out val))
                    {
                        timeMin = val;
                    }
                } 
                else if (a.Name == ATTRIBUTE_TIME_MAX)
                {
                    if (float.TryParse(a.Value, out val))
                    {
                        timeMax = val;
                    }
                }
            }
        }

		protected override XmlNode ToXmlNode (XmlDocument doc)
		{
            XmlElement result = doc.CreateElement(name);
            XmlAttribute attTimeMin = doc.CreateAttribute(ATTRIBUTE_TIME_MIN);
            XmlAttribute attTimeMax = doc.CreateAttribute(ATTRIBUTE_TIME_MAX);

            attTimeMin.Value = timeMin.ToString();
            attTimeMax.Value = timeMax.ToString();

            result.Attributes.Append(attTimeMin);
            result.Attributes.Append(attTimeMax);
            return result;
		}

		public override string ToString ()
		{
			string result = "\n";
			foreach (BComponent c in behaviors) {
				result = result + "\n" + c.ToString();
			}
			return string.Format ("[RandomSelector: {0}\n]", result);
		}
		#endregion
    }
}
