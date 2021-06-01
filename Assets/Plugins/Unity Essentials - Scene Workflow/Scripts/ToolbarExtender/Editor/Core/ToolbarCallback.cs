using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

namespace UnityEssentials.SceneWorkflow.toolbarExtent
{
	public static class ToolbarCallback
	{
		private static Type _toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
		private static Type _guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
		static Type _iWindowBackendType = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");
		static PropertyInfo _windowBackend = _guiViewType.GetProperty("windowBackend",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		static PropertyInfo _viewVisualTree = _iWindowBackendType.GetProperty("visualTree",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
		static PropertyInfo _viewVisualTree = _guiViewType.GetProperty("visualTree",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
		private static FieldInfo _imguiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		private static ScriptableObject _currentToolbar;

		/// <summary>
		/// Callback for toolbar OnGUI method.
		/// </summary>
		public static Action OnToolbarGUI;

		static ToolbarCallback()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;
		}

		private static void OnUpdate()
		{
			if (MustGetToolbar())
			{
				//Find the first toolBar
				UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(_toolbarType);
				_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

				if (_currentToolbar != null)
				{
					// Get it's visual tree
#if UNITY_2020_1_OR_NEWER
					var windowBackend = _windowBackend.GetValue(_currentToolbar);
					VisualElement visualTree = (VisualElement) _viewVisualTree.GetValue(windowBackend, null);
#else
					VisualElement visualTree = (VisualElement)_viewVisualTree.GetValue(_currentToolbar, null);
#endif

					// Get first child which 'happens' to be toolbar IMGUIContainer
					IMGUIContainer container = (IMGUIContainer)visualTree[0];


					// (Re)attach handler
					Action handler = (Action)_imguiContainerOnGui.GetValue(container);
					handler -= OnGUI;
					handler += OnGUI;
					_imguiContainerOnGui.SetValue(container, handler);
				}
			}
		}

		/// <summary>
		/// toolbar is a ScriptableObject and gets deleted when layout changes
		/// </summary>
		/// <returns></returns>
		private static bool MustGetToolbar()
		{
			return _currentToolbar == null && !EditorApplication.isCompiling;
		}

		private static void OnGUI()
		{
			OnToolbarGUI?.Invoke();
		}
	}
}
