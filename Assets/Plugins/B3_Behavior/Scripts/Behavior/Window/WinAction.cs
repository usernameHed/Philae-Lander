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
	/// Window action.
	/// </summary>
	public class WinAction : UnitWindow 
	{
        #region def
        public static readonly float WINDOW_BEHAVIOR_UNIT_ACTION_HEIGHT = 20f;
        public static readonly float WINDOW_BEHAVIOR_UNIT_ACTION_WIDTH_SCALE = 0.95f;
        #endregion

		#region values
		protected AAction actionUnit;
		protected Texture imgOptions;
        protected Texture imgDelete;
        protected Texture imgActionType;
        protected string txtType = "[?]";
		#endregion

        #region properties
        protected static Rect rectForBox
        {
            get
            {
                return new Rect(2f, WINDOW_BEHAVIOR_UNIT_ACTION_HEIGHT, WINDOW_BEHAVIOR_UNIT_SIZE.x * 0.95f, WINDOW_BEHAVIOR_UNIT_ACTION_HEIGHT + 5f);
            }
        }
        #endregion

		#region methods
		public WinAction(BComponent unit) : base(unit)
		{
			actionUnit = unit as AAction;
			imgOptions = Resources.Load<Texture>("opcje");
            imgDelete = Resources.Load<Texture>("usun");

            if (unit is ActionBool)
            {
                txtType = "Bool";
                imgActionType = Resources.Load<Texture>("ActionBollean");
            }
            else if (unit is ActionEnumerator)
            {
                txtType = "Enumerator";
                imgActionType = Resources.Load<Texture>("ActionEnumerator");
            }
            else if (unit is ActionProperty)
            {
                txtType = "Property";
                imgActionType = Resources.Load<Texture>("ActionProperty");
            }
            else if (unit is ActionVoid)
            {
                txtType = "Void";
                imgActionType = Resources.Load<Texture>("ActionVoid");
            }
		}

		protected override void DrawContentWindow (Rect r)
		{
            string txt = !actionUnit.isError ? actionUnit.actionName : "Lack: " + actionUnit.messageError;
            string txtToolTip = string.Format(" {2}: {0} \n Component: {1} ", txt, actionUnit.componentName, txtType);

            Rect rTxt = rectForBox;
            txt = wrapText(txt, UnityEngine.GUI.skin.box, rTxt.size.x * 0.95f);

			if (!actionUnit.isError)
			{
                UnityEngine.GUI.Box(rTxt, new GUIContent(txt, txtToolTip));
			}
			else
			{
                UnityEngine.GUI.Box(rTxt, new GUIContent(txt, txtToolTip));
			}
            UnityEngine.GUI.skin.box.alignment = TextAnchor.MiddleCenter;

            if (!isAddedUsed && currentVisibleScale > 0.7f && !keepVisible)
			{
                Rect position = ImgOptionsUnitRect(r);
                Rect posRemove = new Rect(position.position - new Vector2(IMG_OPIONS_SIZE * 1.1f, 1f), position.size);
				if (UnityEngine.GUI.Button(posRemove, imgOptions))
				{
					UnitArrang.arrang.window.MenuActionMore(EventCallMoreAction);
				}

                posRemove = new Rect(position.position + new Vector2(IMG_OPIONS_SIZE * 1.1f, -1f), position.size);
                if (UnityEngine.GUI.Button(posRemove, imgDelete))
                {
                    UnitArrang.arrang.window.MenuActionDelete(EventCallDeleteActon, actionUnit.actionName);
                }

                posRemove = new Rect(position.position + new Vector2(-WINDOW_BEHAVIOR_UNIT_SIZE.x * 0.4f, WINDOW_BEHAVIOR_UNIT_SIZE.y - 30f), position.size * 1.5f);
                UnityEngine.GUI.Label(posRemove, imgActionType);
			}
		}

        protected string wrapText(string txt, GUIStyle style, float maxWidth)
        {
            char[] cTxt = txt.ToCharArray();
            System.Text.StringBuilder sb = new System.Text.StringBuilder(cTxt.Length);

            if (style.CalcSize(new GUIContent(txt)).x > maxWidth)
            {
                for (int i = 0; i < cTxt.Length; ++i)
                {
                    sb.Append(cTxt[i]);

                    if (style.CalcSize(new GUIContent(sb.ToString())).x > maxWidth)
                    {
                        int start = sb.Length - 5;
                        sb.Remove(start, sb.Length - start);
                        sb.Append("...");
                        return sb.ToString();
                    }
                }
            }

            return txt;
        }

		#endregion

		#region method - draw menu
		protected void EventCallMoreAction(object obj)
		{
			string id = obj as string;
			if (id == "cut")
			{
				unit.behaviorManager.clipboard = unit;
				unit.parent.RemoveChild(unit);
				UnitArrang.arrang.window.SaveAsset(unit.behaviorManager.xmlDocAI, unit.behaviorManager.xmlAI);
			}
			else if (id == "copy")
			{
				unit.behaviorManager.clipboard = unit.Copy();
			}
		}

        protected void EventCallDeleteActon(object obj)
        {
            UnitArrang.arrang.window.AnimateRemoveUnit(this,
                () =>
                {
                    actionUnit.parent.RemoveChild(actionUnit);
                    UnitArrang.arrang.window.SaveAsset(actionUnit.behaviorManager.xmlDocAI, actionUnit.behaviorManager.xmlAI);
                }
            );
        }
		#endregion
	}
}