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
	/// Window root selector.
	/// </summary>
	public class WinRootSelector : WinComposit
	{
		#region methods
		public WinRootSelector(BComponent unit) : base(unit)
		{
            this.img = Resources.Load<Texture>("root");
		}

		protected override void DrawContentWindow (Rect r)
		{
            UnityEngine.GUI.DrawTexture(new Rect(r.position, IMG_SIZE), img);

			if (!isAddedUsed)
			{
                Rect position = ImgOptionsUnitRect(r);
                Rect posAdd = new Rect(position.position + new Vector2(0f, r.yMax - IMG_OPIONS_SIZE * 0.9f), position.size);

                if (unit.childrenComponents == null || unit.childrenComponents.Length < 1)
                {
                    if (UnityEngine.GUI.Button(posAdd, imgAdd))
                    {
                        UnitArrang.arrang.window.MenuComponentAdd(EventAddComponent);
                    }
                }
			}
		}
		#endregion
	}
}