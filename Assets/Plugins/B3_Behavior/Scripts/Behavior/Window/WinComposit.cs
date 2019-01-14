using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using UnityEngine;
using Quantized.Game.Behavior;
using Quantized.Game.Behavior.Components;
using Quantized.Game.Behavior.Components.Actions;
using Quantized.Game.Behavior.Components.Composites;
using Quantized.Game.Behavior.Components.Decorators;

namespace Quantized.Game.Behavior.Window
{
    /// <summary>
    /// Quantizedbit (Komorowski Sebastian)
    /// Window composit.
    /// </summary>
	public abstract class WinComposit : UnitWindow 
	{


		#region values
		protected Texture img;
		protected Texture imgAdd;
		protected Texture imgRemove;
		protected Texture imgOptions;
		protected float timeRepaint = 0f;
		#endregion

		#region methods
		public WinComposit(BComponent unit) : base(unit)
		{
			imgAdd = Resources.Load<Texture>("add");
			imgRemove = Resources.Load<Texture>("usun");
			imgOptions = Resources.Load<Texture>("opcje");
		}

		protected override void DrawContentWindow (Rect r)
		{
            UnityEngine.GUI.DrawTexture(new Rect(r.position, IMG_SIZE), img);

            DrawMenu(r);
		}

        protected virtual void DrawMenu(Rect r)
        {
            if (!isAddedUsed && currentVisibleScale > 0.7f && !keepVisible)
            {
                Rect position = ImgOptionsUnitRect(r);

                Rect posOptions = new Rect(position.position - new Vector2(IMG_OPIONS_SIZE * 1.1f, 1f), position.size);
                if (UnityEngine.GUI.Button(posOptions, imgOptions))
                {
                    UnitArrang.arrang.window.MenuMoreComponent(EventOptionsComponent);
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

        protected virtual void CreateAction(string componentName, string actionName)
		{
			GroupComponentMethods gcm = UnitArrang.arrang.window.components.GetActionsFromComponent(componentName);
			if (gcm != null)
			{
				PropertyInfo pi = gcm.allProperties.Find(x => x.Name == actionName);
				if (pi != null)
				{
					ShowAdd(new ActionProperty(actionName, componentName, pi, unit.behaviorManager.FindComponent(componentName)));
					return;
				}

				MethodInfo mVoid = gcm.allVoid.Find( x => x.Name == actionName);
				if (mVoid != null)
				{
					ShowAdd(new ActionVoid(actionName, componentName, mVoid, unit.behaviorManager.FindComponent(componentName)));
					return;
				}

				MethodInfo mBool = gcm.allBool.Find( x => x.Name == actionName);
				if (mBool != null)
				{
					ShowAdd(new ActionBool(actionName, componentName, mBool, unit.behaviorManager.FindComponent(componentName)));
					return;
				}

				MethodInfo mEnum = gcm.allEnumerators.Find( x => x.Name == actionName);
				if (mEnum != null)
				{
					ShowAdd(new ActionEnumerator(actionName, componentName, mEnum, unit.behaviorManager.FindComponent(componentName)));
				}
			}
		}
		#endregion

		#region methods - events
		protected void EventAddComponent(object obj)
		{
			string name = (string)obj;

			if (name == Selector.NAME)
			{
				ShowAdd(new Selector());
			}
			else if (name == Sequence.NAME)
			{
				ShowAdd(new Sequence());
			}
			else if (name == RandomSelector.NAME)
			{
				ShowAdd(new RandomSelector());
			}
			else if (name == Inverter.NAME)
			{
				ShowAdd(new Inverter());
			}
			else if (name == UntilFalse.NAME)
			{
				ShowAdd(new UntilFalse());
			}
			else if (name == UntilTrue.NAME)
			{
				ShowAdd(new UntilTrue());
			}
            else if (name == "Paste")
            {
                ShowPaste();
            }
			else if (name.Contains("Add-Actions:"))
			{
				string[] actionFirst = name.Split(':');
				if (actionFirst != null && actionFirst.Length == 2)
				{
					string[] componentAction = actionFirst[1].Split('|');
					if (componentAction != null && componentAction.Length == 2)
					{
						CreateAction(componentAction[0], componentAction[1]);
					}
				}
			}
		}

        protected void EventDeleteComponent(object obj)
        {
            UnitArrang.arrang.window.AnimateRemoveUnit(this, 
                () =>
                {
                    unit.parent.RemoveChild(unit);
                    UnitArrang.arrang.window.SaveAsset(unit.behaviorManager.xmlDocAI, unit.behaviorManager.xmlAI);
                }
            );
        }

        protected virtual void EventOptionsComponent(object obj)
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
        }
		#endregion
	}
}