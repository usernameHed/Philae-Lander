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
	/// Window random selector.
	/// </summary>
	public class WinRandomSelector : WinComposit 
	{
		#region methods
		public WinRandomSelector(BComponent unit) : base(unit)
		{
			this.img = Resources.Load<Texture>("ico_random");
		}

        protected override void DrawContentWindow(Rect r)
        {
            RandomSelector.HoldTime hold = ((RandomSelector)unit).holdTime;
            string txtToolTip = string.Format("Hold random choice\nTime min:{0}; max:{1};", hold.min, hold.max);
            Rect rr = new Rect(r.position, IMG_SIZE);

            UnityEngine.GUI.DrawTexture(rr, img);
            UnityEngine.GUI.Label(rr, new GUIContent("", txtToolTip));

            DrawMenu(r);        
        }

        protected override void DrawMenu(Rect r)
        {
            if (!isAddedUsed && currentVisibleScale > 0.7f && !keepVisible)
            {
                Rect position = ImgOptionsUnitRect(r);

                Rect posOptions = new Rect(position.position - new Vector2(IMG_OPIONS_SIZE * 1.1f, 1f), position.size);
                if (UnityEngine.GUI.Button(posOptions, imgOptions))
                {
                    UnitArrang.arrang.window.MenuMoreComponentRandom(EventOptionsComponentRandom, unit);
                }
                Rect posAdd = new Rect(position.position + new Vector2(0f, r.yMax - IMG_OPIONS_SIZE * 0.9f), position.size);
                if (UnityEngine.GUI.Button(posAdd, imgAdd))
                {
                    UnitArrang.arrang.window.MenuComponentAdd(EventAddComponent);
                }
                Rect posRemove =  new Rect(position.position + new Vector2(IMG_OPIONS_SIZE * 1.1f, -1f), position.size);
                if (UnityEngine.GUI.Button(posRemove, imgRemove))
                {
                    UnitArrang.arrang.window.MenuComponentDelete(EventDeleteComponent, unit.name);
                }
            }
        }
        #endregion

        #region methods - evet
        protected virtual void EventOptionsComponentRandom(object obj)
        {
            string name = (string)obj;

            if (name == "Copy-Unit")
            {
                unit.behaviorManager.clipboard = unit.Copy();
            }
            else if (name == "Cut-Unit")
            {
                unit.behaviorManager.clipboard = unit.Copy();
                unit.parent.RemoveChild(unit);
                UnitArrang.arrang.window.SaveAsset(unit.behaviorManager.xmlDocAI, unit.behaviorManager.xmlAI);
            }
            else if (name == "Hold")
            {
                UnitArrang.arrang.window.SaveAsset(unit.behaviorManager.xmlDocAI, unit.behaviorManager.xmlAI);
            }
        }
        #endregion
	}
}