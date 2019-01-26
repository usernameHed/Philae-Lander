using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Quantized.Game.Behavior
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Behavior AI.
	/// </summary>
	public class BehaviorAI : MonoBehaviour 
	{
		#region values
        [Tooltip("Xml file with behaviour tree")]
		public TextAsset xmlAI;
        [Tooltip("If TRUE then one update try used all possible transitions of components are performed. " +
            "On the other hand, when FALSE then udpate go to the action, and another update to the go to next action.")]
		public bool updateAllTree = false;
        [Tooltip("Delay Update for a specified time in seconds")]
        public float waitTime = 0f;

        private Behaviors _behavior;
		#endregion

        #region properties
        public Behaviors behaviour
        {
            get
            {
                return _behavior;
            }
        }
        #endregion

		#region methods - mono
		public void Start () 
		{
            MakeBehaviour();
		}
		
		public void Update () 
		{
			if (_behavior)
            {
				_behavior.Update();
			}
		}

        public void MakeBehaviour()
        {
            _behavior = MakeNewBehaviors();
        }

        public Behaviors MakeNewBehaviors(Window.UnitArrang arrang = null)
        {
            return new Behaviors(xmlAI, gameObject, waitTime, updateAllTree, arrang);
        }
		#endregion
	}
}