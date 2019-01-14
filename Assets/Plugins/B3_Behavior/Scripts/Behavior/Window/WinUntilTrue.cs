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
	/// Window until true.
	/// </summary>
    public class WinUntilTrue : WinCompositOneChild
	{
		#region methods
		public WinUntilTrue(BComponent unit) : base(unit)
		{
			this.img = Resources.Load<Texture>("ico_repeat");
		}

        protected override void DrawContentWindow(Rect r)
        {
            string txtToolTip = "Execute until TRUE";
            Rect rr = new Rect(r.position, IMG_SIZE);

            UnityEngine.GUI.DrawTexture(rr, img);
            UnityEngine.GUI.Label(rr, new GUIContent("", txtToolTip));

            DrawMenu(r);        
        }
		#endregion
	}
}