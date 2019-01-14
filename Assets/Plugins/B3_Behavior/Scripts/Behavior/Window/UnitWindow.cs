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
	/// Unit window.
	/// </summary>
	public abstract class UnitWindow
	{
		// TODO SEBA zablkokować buton usuwania gdy nie ma child
		// zablokowac element dodania gdy komponent obsługuje tylko jednego potomka

		#region def
        public static readonly float IMG_OPIONS_SIZE = 24f;
        public static readonly Vector2 IMG_OPIONS_SIZE_VEC = Vector2.one * IMG_OPIONS_SIZE;
        public static readonly Vector2 IMG_SIZE = new Vector2(64f, 64f);
		/// <summary>
		/// podstawowa wielkosc okna
		/// </summary>
		public static readonly Vector2 WINDOW_BEHAVIOR_UNIT_SIZE = new Vector2(150f, 70f);
		/// <summary>
		/// odstępy miedzy oknami
		/// </summary>
		public static readonly Vector2 WINDOW_FRAMES = new Vector2(10f, 30f);
		/// <summary>
		/// wielkość okna z przyciskiem add
		/// </summary>
		public static readonly Vector2 WINDOW_ADD_SIZE = new Vector2(50f, 50f);
        /// <summary>
        /// rozmiar samego przycisku add
        /// </summary>
		public static readonly float BUTTON_ADD_PASTE_SIZE = 40f;
        /// <summary>
        /// początkowy index okna z przyciskiem add
        /// </summary>
		public static readonly int ID_WINDOW_ADD = 100;
        /// <summary>
        /// początkowy index okienka clipboard
        /// </summary>
		public static readonly int ID_WINDOW_SHOW_CLIPBOARD = 90;
        /// <summary>
        /// początkowy index okien z elementami behavioryki
        /// </summary>
        public static readonly int ID_WINDOW_BEHAVIOR_UNITS = 1000;

		protected static readonly Color COLOR_WIN = new Color(1f, 1f, 1f, 0f);
        protected static readonly float USED_COLOR_OFF = 0.975f;
		#endregion

		#region values
        private static int windowsCount = ID_WINDOW_BEHAVIOR_UNITS;
		protected static bool isAddedUsed = false;

		protected BComponent unit;
		protected GUISkin skin;
		protected bool showAddsButons = false;
		protected BComponent unitToAdd = null;
		protected System.Threading.Thread animAddButtons;
		protected float widnowsAddSize = 0f;
		protected float currentVisibleScale = 1f;
		protected bool keepVisible = false;
        protected Texture imgAddNext;

        protected float _visibleScale = 0f;
		#endregion

		#region properties
		public static Vector2 offsetHorizontal
		{
			get 
			{
				return new Vector2(WINDOW_BEHAVIOR_UNIT_SIZE.x + WINDOW_FRAMES.x + additionalOffset.x, 0f);
			}
		}

		public static Vector2 offsetVertical
		{
			get 
			{
				return new Vector2(0f, WINDOW_BEHAVIOR_UNIT_SIZE.y + WINDOW_FRAMES.y + additionalOffset.y);
			}
		}

		public int idWindow
		{
			get;
			private set;
		}

		public Location locationArrang
		{
			get;
			set;
		}

		public Rect rect
		{
			get;
			private set;
		}

        public float visibleScale
        {
            get
            {
                return _visibleScale;
            }
            set
            {
                _visibleScale = value;

                if (unit != null && unit.childrenComponents != null)
                {
                    foreach (BComponent b in unit.childrenComponents)
                    {
                        if (b != null && b.window != null)
                        {
                            b.window.visibleScale = value;
                        }
                    }
                }
            }
        }

		protected static Vector2 additionalOffset 
		{
			get;
			set;
		}

        protected static Rect ImgOptionsUnitRect(Rect r)
        {
            return new Rect(r.center + new Vector2(-IMG_OPIONS_SIZE * 0.75f, -r.size.y + IMG_OPIONS_SIZE * 1.1f), IMG_OPIONS_SIZE_VEC);
        }
		#endregion

		#region methods - msg
		public void OnGUICalculate(Vector2 position) 
		{
			position = CheckOverlapCurrent(position);
			position = CheckOverlapChildren(position);
			this.rect = new Rect(position, WINDOW_BEHAVIOR_UNIT_SIZE);

			SetChildrenPosition(position);
			CenterBetweenTwoChildren(position);
		}

		public void OnGUI(Vector2 offsetPosition, System.Action repaintEvent, float unitVisible)
		{
			UnityEngine.GUI.color = COLOR_WIN;
			GUISkin backup = UnityEngine.GUI.skin;

			Rect currentRect = new Rect(rect.position + offsetPosition, rect.size);
			BComponent lastUnit = null;
			int idCount = 0;
			Rect r = new Rect();

			UnityEngine.GUI.skin = skin;
            UnityEngine.GUI.Window(idWindow, new Rect(currentRect.position, currentRect.size * 1.2f + Vector2.up * 15f), EvtWindow, GUIContent.none);
			UnityEngine.GUI.skin = backup;

            currentVisibleScale = keepVisible ? 1f : unitVisible;
            currentVisibleScale = Mathf.Clamp(currentVisibleScale, 0f, 1f) - visibleScale;

			if (unit.childrenComponents != null) 
			{

				foreach (BComponent ut in unit.childrenComponents) 
				{
					if (ut == null)
					{
						continue;
					}

					r = new Rect(ut.window.rect.position + offsetPosition, ut.window.rect.size);
                    UnitArrang.arrang.window.DrawBezier(currentRect, r, currentVisibleScale, ut.usedUnitColor, (ut is AAction ? 8f : 5f));
                    ut.usedUnitColor *= USED_COLOR_OFF;

					if (showAddsButons)
					{
						r.size = WINDOW_ADD_SIZE;
						if (lastUnit == null)
						{
							r.position = r.position + new Vector2(-WINDOW_ADD_SIZE.x * 0.5f, ut.window.rect.size.y * 0.5f - WINDOW_ADD_SIZE.y * 0.5f);
						}
						else
						{
							r.position = r.position + new Vector2(((lastUnit.window.rect.position.x + lastUnit.window.rect.size.x) - ut.window.rect.position.x) * 0.5f - WINDOW_ADD_SIZE.x * 0.5f, 
							                                      ut.window.rect.size.y * 0.5f - WINDOW_ADD_SIZE.y * 0.5f);
						}
						UnityEngine.GUI.Window(ID_WINDOW_ADD + idCount, r, EvtWindowAdd, GUIContent.none);
					}

					lastUnit = ut;
					idCount += 1;
				}
			}

			if (showAddsButons)
			{
				if (lastUnit != null)
				{
					r = new Rect(lastUnit.window.rect.position + offsetPosition, WINDOW_ADD_SIZE);
					r.position = r.position + new Vector2(lastUnit.window.rect.size.x - WINDOW_ADD_SIZE.x * 0.5f, lastUnit.window.rect.size.y * 0.5f - WINDOW_ADD_SIZE.y * 0.5f);
					UnityEngine.GUI.Window(ID_WINDOW_ADD + idCount, r, EvtWindowAdd, GUIContent.none);
				}
				else
				{
					r = new Rect(rect.position + offsetPosition + new Vector2(rect.size.x * 0.5f - WINDOW_ADD_SIZE.x * 0.5f, WINDOW_ADD_SIZE.y + WINDOW_FRAMES.y), WINDOW_ADD_SIZE);
					UnityEngine.GUI.Window(ID_WINDOW_ADD + idCount, r, EvtWindowAdd, GUIContent.none);
				}

				if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
				{
					HideAdd();
					UnitArrang.arrang.window.FadeOffUnits ();
					repaintEvent();
				}

				if (animAddButtons != null && animAddButtons.IsAlive)
				{
					repaintEvent();
				}
			}
		}
		#endregion

		#region methods abstract
		protected abstract void DrawContentWindow(Rect r);
		#endregion

		#region methods
		public UnitWindow(BComponent unit)
		{
			this.skin = Resources.Load<GUISkin>("Window");
			this.unit = unit;
			this.rect = new Rect(new Vector2(-1f, -1f), new Vector2(-1f, -1f));

			idWindow = windowsCount;
			windowsCount += 1;
			isAddedUsed = false;
            imgAddNext = Resources.Load<Texture>("add new");
            visibleScale = 0f;
		}

		public void WaitFadeOffFroUnits()
		{
            UnitArrang.arrang.window.FadeOffUnits(1f);
		}

		public void KeepVisible()
		{
			keepVisible = true;
            UnitArrang.arrang.window.WaitEventFadeOff(
                () =>
                {
                    keepVisible = false;
                });
		}
		#endregion

		#region methods - add paste
		protected void ShowPaste()
		{
            unitToAdd = unit.behaviorManager.clipboard;
            unit.behaviorManager.clipboard = unitToAdd.Copy();

			showAddsButons = true;
			isAddedUsed = true;

			StarAtnimAddPasteButtons();
		}
		
		protected void ShowAdd(BComponent u)
		{
			unitToAdd = u;
			showAddsButons = true;
			isAddedUsed = true;

			StarAtnimAddPasteButtons();
		}
		
		protected void ActionAdd(int windowID)
		{
			if (unitToAdd != null)
			{
				unit.AddChild(unitToAdd, windowID - ID_WINDOW_ADD);
				UnitArrang.arrang.window.SaveAsset(unit.behaviorManager.xmlDocAI, unit.behaviorManager.xmlAI);

                UnitArrang.arrang.window.OffsetMoveBg();
			} 

			HideAdd();
		}

		protected void HideAdd()
		{
			isAddedUsed = false;
			unitToAdd = null;
			showAddsButons = false;
			widnowsAddSize = 0f;
		}

		protected void StarAtnimAddPasteButtons()
		{
			widnowsAddSize = 0f;
            UnitArrang.arrang.window.AnimButtonsAdds(BUTTON_ADD_PASTE_SIZE,
                (currentSize) =>
                {
                    widnowsAddSize = currentSize;
                }
            );
		}
		#endregion

		#region methods - transform on unit arrang
		public bool CenterBetweenTwoChildren(Vector2 position)
		{
			if (unit.childrenComponents != null && unit.childrenComponents.Length > 0)
			{
				BComponent unitLeft = unit.childrenComponents[0];
				BComponent unitRight = unit.childrenComponents[unit.childrenComponents.Length - 1];

				if (unitLeft == null || unitRight == null)
				{
					return false;
				}

				Vector2 leftPosition = unitLeft.window.rect.position;
				Vector2 rightPosition = unitRight.window.rect.position;
				position = new Vector2(Vector2.Lerp(leftPosition, rightPosition, 0.5f).x, position.y);
				this.rect = new Rect(position, WINDOW_BEHAVIOR_UNIT_SIZE);

				return true;
			}
			else
			{
				return false;
			}
		}

		public void PutNextPositionRight(Vector2 position)
		{
			rect = new Rect(position + offsetHorizontal, WINDOW_BEHAVIOR_UNIT_SIZE);
		}

		public static Vector2 GetNextPositionRight(Vector2 position)
		{
            return position + offsetHorizontal;
		}

		public void PutNextPositionLeft(Vector2 position)
		{
			rect = new Rect(position - offsetHorizontal, WINDOW_BEHAVIOR_UNIT_SIZE);
		}

		public Vector2 OverlapsWindows(Vector2 currentPosition, Vector2 secondPosition)
		{
			if (Math.Abs(currentPosition.x - secondPosition.x) < (WINDOW_BEHAVIOR_UNIT_SIZE.x + WINDOW_FRAMES.x) || secondPosition.x >= currentPosition.x)
			{
				return secondPosition - currentPosition + offsetHorizontal;
			}
			return Vector2.zero;
		}

		public bool IsOverlapsWindows(Vector2 currentPosition)
		{
			Vector2 secondPosition = rect.position;
			if (Math.Abs(currentPosition.x - secondPosition.x) < ((WINDOW_BEHAVIOR_UNIT_SIZE.x + WINDOW_FRAMES.x) * 0.99f))
			{
				return true;
			}
			return false;
		}

		public void TransformCurrentAndChildren(Vector2 offset)
		{
			rect = new Rect(rect.position + offset, WINDOW_BEHAVIOR_UNIT_SIZE);
			if (unit.childrenComponents != null && unit.childrenComponents.Length > 0)
			{
				foreach (BComponent bc in unit.childrenComponents)
				{
					if (bc != null && bc.window != null)
					{
						bc.window.TransformCurrentAndChildren(offset);
					}
				}
			}
		}

		protected Vector2 GetChildOffsetPosition(int index)
		{
			Vector2 offset = offsetVertical;
			float step = offsetHorizontal.x;
			float childrenZoneWidth = ((float)unit.childrenComponents.Length - 1f) * step;
			offset.x = step * index - childrenZoneWidth * 0.5f;
			return offset;
		}

		protected void SetChildrenPosition(Vector2 position)
		{
			Vector2 childPosition;
			int countChild = 0;
			if (unit.childrenComponents != null && unit.childrenComponents.Length > 0)
			{
				foreach (BComponent ut in unit.childrenComponents) 
				{
					if (ut == null)
					{
						continue;
					}

					childPosition = position + GetChildOffsetPosition(countChild);
					ut.window.OnGUICalculate(childPosition);
					++countChild;
				}
			}
		}

		protected Vector2 CheckOverlapCurrent(Vector2 position)
		{
			Vector2 result = position;
			Location location = unit.locationArrang;
			if (location.coll > 0)
			{
				BComponent find = UnitArrang.arrang.Find(new Location(location.row, location.coll - 1));
				if (find != null)
				{
					Vector2 findPos = find.window.rect.position;
					result = position + OverlapsWindows(position, findPos);
				}
			}

			return result;
		}

		protected Vector2 CheckOverlapChildren(Vector2 position)
		{
			Vector2 result = position;
			if (unit.childrenComponents != null && unit.childrenComponents.Length > 0)
			{
				BComponent unitChild = unit.childrenComponents[0];
				if (unitChild != null)
				{
					Location location = unitChild.locationArrang;
					if (location.coll > 0)
					{
						BComponent find = UnitArrang.arrang.Find(new Location(location.row, location.coll - 1));
						if (find != null)
						{
							Vector2 unitChildPosition = position + GetChildOffsetPosition(0);
							Vector2 findPos = find.window.rect.position;
							result = position + OverlapsWindows(unitChildPosition, findPos);
						}
					}
				}
			}
			return result;
		}
		#endregion

		#region event - windows
		private void EvtWindow (int windowID) 
		{
			Vector2 size = new Vector2(WINDOW_BEHAVIOR_UNIT_SIZE.x * 0.5f, WINDOW_BEHAVIOR_UNIT_SIZE.x * 0.5f);
            Vector2 left = new Vector2(WINDOW_BEHAVIOR_UNIT_SIZE.x * 0.5f, 10f) - new Vector2(/*size.x*/IMG_SIZE.x * 0.5f, 0f);
            Rect r = new Rect(left, size);

			Color colBack = UnityEngine.GUI.color;
			Color colCurrent = Color.white;
			colCurrent.a = currentVisibleScale;

			UnityEngine.GUI.color = colCurrent;
			DrawContentWindow(r);

			UnityEngine.GUI.color = colBack;
		}

		private void EvtWindowAdd(int windowID)
		{
			Vector2 size = new Vector2(widnowsAddSize, widnowsAddSize);
			Vector2 offset = (WINDOW_ADD_SIZE - new Vector2 (BUTTON_ADD_PASTE_SIZE, BUTTON_ADD_PASTE_SIZE)) * 0.5f;
			Vector2 pos = new Vector2 (BUTTON_ADD_PASTE_SIZE - size.x, BUTTON_ADD_PASTE_SIZE - size.y) * 0.5f;

            Texture2D backNormal = UnityEngine.GUI.skin.button.normal.background;
            Texture2D backActive = UnityEngine.GUI.skin.button.onActive.background;
            Texture2D backFocus =  UnityEngine.GUI.skin.button.onFocused.background;
            Texture2D backHover = UnityEngine.GUI.skin.button.onHover.background;
            Texture2D backOnNormal = UnityEngine.GUI.skin.button.onNormal.background;

            UnityEngine.GUI.skin.button.normal.background = null;
            UnityEngine.GUI.skin.button.onActive.background = null;
            UnityEngine.GUI.skin.button.onFocused.background = null;
            UnityEngine.GUI.skin.button.onHover.background = null;
            UnityEngine.GUI.skin.button.onNormal.background = null;

            if (UnityEngine.GUI.Button(new Rect(pos + offset, size), imgAddNext))
			{
				ActionAdd(windowID);
			}

            UnityEngine.GUI.skin.button.normal.background = backNormal;
            UnityEngine.GUI.skin.button.onActive.background = backActive;
            UnityEngine.GUI.skin.button.onFocused.background = backFocus;
            UnityEngine.GUI.skin.button.onHover.background = backHover;
            UnityEngine.GUI.skin.button.onNormal.background = backOnNormal;
		}
		#endregion
	}
}