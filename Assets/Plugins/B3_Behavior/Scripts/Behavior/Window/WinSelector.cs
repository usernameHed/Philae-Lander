using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;
using Quantized.Game.Behavior.Components;
using Quantized.Game.Behavior.Components.Composites;
using Quantized.Game.Behavior.Components.Actions;
using Quantized.Game.Behavior.Components.Decorators;

namespace Quantized.Game.Behavior.Window
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Window selector.
	/// </summary>
	public class WinSelector : WinComposit 
	{
		#region methods
		public WinSelector(BComponent unit) : base(unit)
		{
			this.img = Resources.Load<Texture>("ico_quest");
		}
		#endregion
	}
}