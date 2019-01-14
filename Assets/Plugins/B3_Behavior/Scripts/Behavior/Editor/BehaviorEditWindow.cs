using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.Threading;
using UnityEditor.AnimatedValues;
using Quantized.Game.Behavior;
using Quantized.Game.Behavior.Window;
using Quantized.Game.Behavior.Components;
using Quantized.Game.Behavior.Edit;

namespace Quantized.Game.Behavior
{
	/// <summary>
	/// Quantizedbit (Komorowski Sebastian)
	/// Behavior edit window.
	/// </summary>
	public class BehaviorEditWindow : EditorWindow, IEditWindow
	{
		#region def 
		public static readonly float BEZIER_OFFSET = 10f * 2.5f;
        public static readonly Color COLOR_LINE = new Color(0.4f, 0.4f, 0.4f, 0.7f);
        public static readonly float BEZIER_DOT = 4f;
        public static readonly uint TIME_TO_CHECK_FILE = 15;
		#endregion

		#region values
		private static BehaviorEditWindow window;

		private Vector2 windowScrollPos = new Vector2();
		private BehaviorAI currentBehaviour;
        private Texture imgClipboard;
        private Texture bgColor;

        private SomeEvent someEvent = new SomeEvent();

        private AnimFloat animClipboard = new AnimFloat(-40f);
        private AnimFloat animAllHide = new AnimFloat(1f);
        private AnimFloat animShowAddButtons;
        private AnimFloat animDelete = new AnimFloat(0f);

        private AnimVector3 animOffsetBg = new AnimVector3(Vector3.zero);
        private Vector3 lastAnimOffsetBg = Vector3.zero;
        private Vector2 mousePositon;

        private float fadeOffUnitsWaitTime;

        private uint lastTimeCheck = (uint)0;
		#endregion

		#region properties
		public ComponentsManager components
		{
			get;
			protected set;
		}

		private static BehaviorAI selectedBehaviour
		{
			get 
			{
				GameObject[] allSelected = Selection.gameObjects;
				BehaviorAI current = null;

				foreach (GameObject g in allSelected) 
				{
					current = g.GetComponent<BehaviorAI>();
					if (current != null) 
					{
						break;
					}
				}

				return current;
			}
		}
		#endregion

		#region methods - msg
		public void OnProjectChange()
		{
			SetGraph();
		}

		public void OnFocus()
		{
			SetGraph();
		}

        public void OnLostFocus()
        {
            currentBehaviour = null;
            SetGraph();
        }

		public void OnSelectionChange()
		{
			SetGraph();
		}

        public void OnHierarchyChange()
        {
            SetGraph();
        }

        public void OnDestroy()
        {
            currentBehaviour = null;
        }

		public void OnGUI () 
		{
			if (currentBehaviour == null)
			{
				return;
			}

			Rect position = new Rect(0f, 0f, this.position.size.x, this.position.size.y);
			Rect viewRect = new Rect(0f, 0f, UnitArrang.arrang != null ? UnitArrang.arrang.areaSize.x : 0f, UnitArrang.arrang != null ? UnitArrang.arrang.areaSize.y : 0f);

            DrawBgColor(viewRect);
            DrawClipboardBox();

			UnityEngine.GUI.BeginScrollView(position, windowScrollPos, viewRect, GUIStyle.none, GUIStyle.none);
            if (UnitArrang.arrang != null)
            {
                BeginWindows();
				UnitArrang.arrang.OnGUI(Repaint, animAllHide.value);
                EndWindows();
            }
			UnityEngine.GUI.EndScrollView();

			if (Event.current.type == EventType.MouseDrag && Event.current.button == 2 && UnitArrang.arrang != null)
			{
				Repaint();
				windowScrollPos = windowScrollPos - Event.current.delta;
			}

            Vector3 offset = CurrentOffsetMoveBg();
            if (UnitArrang.arrang != null)
            {
                windowScrollPos.x = Mathf.Clamp(windowScrollPos.x + offset.x, 0f, UnitArrang.arrang.areaSize.x - position.size.x);
                windowScrollPos.y = Mathf.Clamp(windowScrollPos.y + offset.y, 0f, UnitArrang.arrang.areaSize.y - position.size.y);
            }
            else
            {
                windowScrollPos.x = 0f;
                windowScrollPos.y = 0f;
            }

            mousePositon = Event.current.mousePosition;
		}
		#endregion

		#region methods
		public static void Init () 
		{
			if (window == null)
			{
				window = EditorWindow.GetWindow<BehaviorEditWindow>("B3 Behavior");
                window.imgClipboard = Resources.Load<Texture>("clipboard");
                window.bgColor = Resources.Load<Texture>("bg");
                window.CheckTime();
				window.Show();
			}
		}

        public void Update()
        {
            someEvent.Update();

            if ((uint)EditorApplication.timeSinceStartup > lastTimeCheck)
            {
                CheckTime();
            }
        }

		public void MakeArrangGraph()
		{
            if (currentBehaviour == null || currentBehaviour.behaviour == null)
            {
                return;
            }

            UnitArrang.arrang = new UnitArrang(currentBehaviour.behaviour.rootComponent, this);
			UnitArrang.arrang.OnGUICalculate();
            Repaint();
		}

		public void SaveAsset(XmlDocument xml, TextAsset txt)
		{
			xml.Save(AssetDatabase.GetAssetPath(txt));
            AssetDatabase.Refresh();
			MakeArrangGraph();
            CheckTime();
		}

        public string GetAssetTextPath(TextAsset txt)
        {
            return AssetDatabase.GetAssetPath(txt);
        }

        public void AnimButtonsAdds(float size, System.Action<float> refresh)
        {
            animShowAddButtons = new AnimFloat(0f);
            animShowAddButtons.target = size;
            animShowAddButtons.speed = 3.5f;
            animShowAddButtons.valueChanged.AddListener(Repaint);
            animShowAddButtons.valueChanged.AddListener(
                () =>
                {
                    if (refresh != null)
                    {
                        refresh(animShowAddButtons.value);
                    }
                }
            );

            FadeOnUnits();
        }

        public void AddSomeEvent(System.Func<bool> waitUtilTrue, System.Action action)
        {
            someEvent.AddEvent(waitUtilTrue, action);
        }

        public void AnimateRemoveUnit(UnitWindow removeUnit, System.Action endAnimEvent)
        {
            animDelete = new AnimFloat(0f);
            animDelete.target = 1f;
            animDelete.speed = 1.5f;
            animDelete.valueChanged.AddListener(Repaint);
            animDelete.valueChanged.AddListener(
                () =>
                {
                    removeUnit.visibleScale = animDelete.value;
                }
            );

            someEvent.AddEvent(
                () => 
                {
                    return animDelete.value > 0.95f;
                },
                () =>
                {
                    if (endAnimEvent != null)
                    {
                        endAnimEvent();
                    }
                }
            );
        }

        public void OffsetMoveBg()
        {
            float offsetSize = UnitWindow.WINDOW_FRAMES.y * 1.5f;
            float offsetSizeFrame = offsetSize * 3f;
            Vector2 offset = Vector2.zero;

            if (mousePositon.y >= (position.size.y - offsetSizeFrame))
            {
                offset.y = offsetSize;
            }
            if (position.size.x > offsetSizeFrame)
            {
                if (mousePositon.x <= offsetSizeFrame)
                {
                    offset.x = -offsetSize;
                }
                else if (mousePositon.x >= (position.size.x - offsetSizeFrame))
                {
                    offset.x = offsetSize;
                }
            }

            animOffsetBg = new AnimVector3(Vector3.zero);
            animOffsetBg.speed = 0.5f;
            animOffsetBg.target = offset;
            animOffsetBg.valueChanged.AddListener(Repaint);

            lastAnimOffsetBg = animOffsetBg.value;
        }

        private Vector3 CurrentOffsetMoveBg()
        {
            Vector3 result = animOffsetBg.value - lastAnimOffsetBg;
            lastAnimOffsetBg = animOffsetBg.value;
            return result;
        }

		private void SetGraph()
		{
            BehaviorAI selected = selectedBehaviour;

            if (selected != null)
            {
                if (currentBehaviour == null || selected != currentBehaviour || selected.behaviour == null)
                {
                    selected.MakeBehaviour();
                    components = new ComponentsManager(selected.gameObject);
                }

                currentBehaviour = selected;
                MakeArrangGraph();
            }
		}
		#endregion 	

		#region methods - fade
        public void FadeOnUnits ()
		{
			animAllHide = new AnimFloat(1f, Repaint);
			animAllHide.target = 0.3f;
			animAllHide.speed = 1f;
		}

        public void FadeOffUnits (float time = 0f)
		{
            fadeOffUnitsWaitTime = Time.realtimeSinceStartup + time;
            someEvent.AddEvent(
                () =>
                {
                    return Time.realtimeSinceStartup > fadeOffUnitsWaitTime;
                },
                () =>
                {
                    animAllHide = new AnimFloat(0.3f, Repaint);
                    animAllHide.target = 2.5f;
                    animAllHide.speed = 0.35f;
                }
            );
		}

        public void WaitEventFadeOff(System.Action action)
        {
            someEvent.AddEvent(
                () =>
                {
                    return animAllHide.value > 0.97f;
                },
                action
            );
        }
		#endregion

		#region methods - draw
        public void DrawBezier(Rect currentRect, Rect r, float visibleScale, float usedColorScale, float offseUp)
		{
            Color crrentColor = Color.Lerp(COLOR_LINE, Color.red, usedColorScale);
			crrentColor.a = visibleScale;

            Vector2 up = new Vector2(r.center.x, r.center.y - currentRect.size.y * 0.38f + offseUp);
            Vector2 down = new Vector2(currentRect.center.x, currentRect.center.y + currentRect.size.y * 0.5f);

            UnityEditor.Handles.DrawSolidRectangleWithOutline(new Rect(up - Vector2.one * (BEZIER_DOT * 0.5f), Vector2.one * BEZIER_DOT), crrentColor, crrentColor);
            UnityEditor.Handles.DrawSolidRectangleWithOutline(new Rect(down - Vector2.one * (BEZIER_DOT * 0.5f), Vector2.one * BEZIER_DOT), crrentColor, crrentColor);

			Handles.BeginGUI();
            Handles.DrawBezier(down,
                               up,
			                   new Vector2(currentRect.center.x, currentRect.yMax + BEZIER_OFFSET), 
			                   new Vector2(r.center.x, r.yMin - BEZIER_OFFSET), 
							   crrentColor, 
			                   null,
			                   4f);
			Handles.EndGUI();

            if (usedColorScale > 0f)
            {
                Repaint();
            }
		}

        protected void DrawClipboardBox()
        {
            if (currentBehaviour != null && currentBehaviour.behaviour != null && currentBehaviour.behaviour.clipboard != null)
            {
                if (!animClipboard.isAnimating && animClipboard.value <= -40f) 
                {
                    animClipboard = new AnimFloat(-40f, Repaint);
                    animClipboard.target = 15f;
                    animClipboard.speed = 2.5f;
                } 
            }
            UnityEngine.GUI.Box(new Rect(new Vector2(10f, animClipboard.value), Vector2.one * 20f), imgClipboard);
        }

        protected void DrawBgColor(Rect rect)
        {
            Rect bgRect = new Rect();
            bgRect.width = position.width > rect.width ? position.width : rect.width;
            bgRect.height = position.height > rect.height ? position.height : rect.height;
            UnityEngine.GUI.DrawTexture(bgRect, bgColor);
        }
		#endregion

        #region methods
        protected void CheckTime()
        {
            lastTimeCheck = (uint)EditorApplication.timeSinceStartup + TIME_TO_CHECK_FILE;
        }
        #endregion

		#region methods - menu
		public void MenuActionMore(System.Action<object> evt)
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Copy"), false, (obj) => { evt(obj); }, "copy");
			menu.AddItem(new GUIContent("Cut"), false, (obj) => { evt(obj); }, "cut");
			menu.ShowAsContext();
		}

        public void MenuActionDelete(System.Action<object> evt, string actionName)
        {
            if (EditorUtility.DisplayDialog(
                "Delete Behavior Action", 
                string.Format("Are you sure you want to delete action:  {0}", actionName), 
                "Yes", 
                "No"))
            {
                evt("delete");
            }
        }

		public void MenuMoreComponent(System.Action<object> evt)
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Copy"), false, (obj) => { evt(obj); }, "Copy-Unit");
			menu.AddItem(new GUIContent("Cut"), false, (obj) => { evt(obj); }, "Cut-Unit");
			menu.ShowAsContext();
		}

        public void MenuMoreComponentRandom(System.Action<object> evt, BComponent unit)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy"), false, (obj) => { evt(obj); }, "Copy-Unit");
            menu.AddItem(new GUIContent("Cut"), false, (obj) => { evt(obj); }, "Cut-Unit");

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Hold Time on Random Selection"), 
                false, 
                (obj) => 
                { 
                    DialogInputHoldRandom.Show(unit, evt);
                }, 
                "Hold"
            );

            menu.ShowAsContext();
        }

        public void MenuComponentDelete(System.Action<object> evt, string name)
		{
            if (EditorUtility.DisplayDialog(
                "Delete Behavior Action", 
                string.Format("Are you sure you want to delete component and its children:  {0}", name), 
                "Yes", 
                "No"))
            {
                evt("delete");
            }
		}

		public void MenuComponentAdd(System.Action<object> evt)
		{
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent(string.Format("Add/{0}", Components.Composites.Selector.NAME)), false, (obj) => { evt(obj); }, Components.Composites.Selector.NAME);
			menu.AddItem(new GUIContent(string.Format("Add/{0}", Components.Composites.Sequence.NAME)), false, (obj) => { evt(obj); }, Components.Composites.Sequence.NAME);
			menu.AddItem(new GUIContent(string.Format("Add/{0}", Components.Composites.RandomSelector.NAME)), false, (obj) => { evt(obj); }, Components.Composites.RandomSelector.NAME);
			menu.AddItem(new GUIContent(string.Format("Add/{0}", Components.Decorators.Inverter.NAME)), false, (obj) => { evt(obj); }, Components.Decorators.Inverter.NAME);
			menu.AddItem(new GUIContent(string.Format("Add/{0}", Components.Decorators.UntilFalse.NAME)), false, (obj) => { evt(obj); }, Components.Decorators.UntilFalse.NAME);
			menu.AddItem(new GUIContent(string.Format("Add/{0}", Components.Decorators.UntilTrue.NAME)), false, (obj) => { evt(obj); }, Components.Decorators.UntilTrue.NAME);
			menu.AddSeparator("Add/");
			
			string[] allComponentNames = components.componentsNames;
			foreach (string currentGropuName in allComponentNames)
			{
				GroupComponentMethods currentGroup = components.GetActionsFromComponent(currentGropuName);
				string[] allActions = currentGroup.allActions;
				foreach (string currentActionName in allActions)
				{
					menu.AddItem(new GUIContent(string.Format("Add/Actions/{0}/{1}", currentGropuName, currentActionName)), 
					             false, 
					             (obj) => { evt(obj); }, 
					             string.Format("Add-Actions:{0}|{1}", currentGropuName, currentActionName));
				}
			}
			menu.AddSeparator("");
			
            if (currentBehaviour != null && currentBehaviour.behaviour != null && currentBehaviour.behaviour.clipboard != null)
            {
                menu.AddItem(new GUIContent("Paste"), 
                    false, 
                    (obj) =>
                    {
                        evt(obj);
                    }, 
                    "Paste");
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste"));
            }
			menu.ShowAsContext();
		}
		#endregion
	}
}