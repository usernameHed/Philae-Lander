using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.SceneWorkflow.toolbarExtent
{
	[InitializeOnLoad]
	public static class ToolbarExtender
	{
		private const string UNITY_EDITOR_TOOLBAR_REFLECTION = "UnityEditor.Toolbar";
        private const string REPAINT_FUNCTION = "RepaintToolbar";
        private static int _toolCount;
		private static GUIStyle _commandStyle = null;

		public static readonly List<Action> LeftToolbarGUI = new List<Action>();
		public static readonly List<Action> RightToolbarGUI = new List<Action>();
		private static Rect _leftRect = new Rect();
		public static Rect GetLeftRect() { return (_leftRect); }
		private static Rect _rightRect = new Rect();
		public static Rect GetRightRect() { return (_rightRect); }

		static ToolbarExtender()
		{
			Type toolbarType = typeof(Editor).Assembly.GetType(UNITY_EDITOR_TOOLBAR_REFLECTION);
			
#if UNITY_2019_1_OR_NEWER
			string fieldName = "k_ToolCount";
#else
			string fieldName = "s_ShownToolIcons";
#endif
			
			FieldInfo toolIcons = toolbarType.GetField(fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

#if UNITY_2019_1_OR_NEWER
			_toolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 7;
#elif UNITY_2018_1_OR_NEWER
			_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 6;
#else
			_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 5;
#endif
	
			ToolbarCallback.OnToolbarGUI -= OnGUI;
			ToolbarCallback.OnToolbarGUI += OnGUI;
		}

		/// <summary>
		/// ask to repaint toolBar
		/// </summary>
		public static void Repaint()
        {
			Type toolbarType = typeof(Editor).Assembly.GetType(UNITY_EDITOR_TOOLBAR_REFLECTION);
			MethodInfo repaintMethod = toolbarType.GetMethod(REPAINT_FUNCTION, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			repaintMethod?.Invoke(new object(), new object[0]);
		}

		private static void OnGUI()
		{
			// Create two containers, left and right
			// Screen is whole toolbar

			if (_commandStyle == null)
			{
				_commandStyle = new GUIStyle("CommandLeft");
			}

			float screenWidth = EditorGUIUtility.currentViewWidth;
			
			RectOfficialCalculation(screenWidth, ref _leftRect, ref _rightRect);
			AdditionalRectCalculation(ref _leftRect, ref _rightRect);

			if (_leftRect.width > 0)
			{
				GUILayout.BeginArea(_leftRect);
				GUILayout.BeginHorizontal();
				foreach (Action handler in LeftToolbarGUI)
				{
					handler();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}

			if (_rightRect.width > 0)
			{
				GUILayout.BeginArea(_rightRect);
				GUILayout.BeginHorizontal();
				foreach (Action handler in RightToolbarGUI)
				{
					handler();
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}
		}
		/// <summary>
		/// Following calculations match code reflected from Toolbar.OldOnGUI()
		/// </summary>
		/// <param name="screenWidth"></param>
		/// <param name="leftRect"></param>
		/// <param name="rightRect"></param>
		private static void RectOfficialCalculation(float screenWidth, ref Rect leftRect, ref Rect rightRect)
		{
			float playButtonsPosition = (screenWidth - 100) / 2;

			leftRect = new Rect(0, 0, screenWidth, Screen.height);
			leftRect.xMin += 10; // Spacing left
			leftRect.xMin += 32 * _toolCount; // Tool buttons
			leftRect.xMin += 20; // Spacing between tools and pivot
			leftRect.xMin += 64 * 2; // Pivot buttons
			leftRect.xMax = playButtonsPosition;

			rightRect = new Rect(0, 0, screenWidth, Screen.height);
			rightRect.xMin = playButtonsPosition;
			rightRect.xMin += _commandStyle.fixedWidth * 3; // Play buttons
			rightRect.xMax = screenWidth;
			rightRect.xMax -= 10; // Spacing right
			rightRect.xMax -= 80; // Layout
			rightRect.xMax -= 10; // Spacing between layout and layers
			rightRect.xMax -= 80; // Layers
			rightRect.xMax -= 20; // Spacing between layers and account
			rightRect.xMax -= 80; // Account
			rightRect.xMax -= 10; // Spacing between account and cloud
			rightRect.xMax -= 32; // Cloud
			rightRect.xMax -= 10; // Spacing between cloud and collab
			rightRect.xMax -= 78; // Colab
		}

		/// <summary>
		/// adding personal preference to the rect calculated
		/// </summary>
		/// <param name="leftRect"></param>
		/// <param name="rightRect"></param>
		private static void AdditionalRectCalculation(ref Rect leftRect, ref Rect rightRect)
		{
			// Add spacing around existing controls
			leftRect.xMin += 20;
			leftRect.xMax -= 30;
			rightRect.xMin -= 10;
			rightRect.xMax += 5;

			// Add top and bottom margins
			leftRect.y = 2;
			leftRect.height = 27;
			rightRect.y = 2;
			rightRect.height = 27;
		}
	}
}
