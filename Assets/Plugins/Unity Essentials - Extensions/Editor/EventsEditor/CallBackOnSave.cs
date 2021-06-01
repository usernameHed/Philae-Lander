using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace UnityEssentials.Extensions.Editor.EventEditor
{
	[InitializeOnLoad]
	public class CallBackOnSave : UnityEditor.AssetModificationProcessor, IPreprocessBuildWithReport, IProcessSceneWithReport
	{
		/// <summary>
		/// Occurs on Scenes/Assets Save
		/// </summary>
		public static Action OnSave;

		/// <summary>
		/// Occurs on first frame in Playmode
		/// </summary>
		public static Action EnteredEditMode;
		public static Action EnteredPlayMode;
		public static Action ExitingEditMode;
		public static Action ExitingPlayMode;

		public static Action OnPreBuild;
		public static Action OnPreBuildScene;



		static CallBackOnSave()
		{
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}


		/// <summary>
		/// On Editor Save
		/// </summary>
		private static string[] OnWillSaveAssets(string[] paths)
		{
			// Prefab creation enforces SaveAsset and this may cause unwanted dir cleanup
			if (paths.Length == 1 && (paths[0] == null || paths[0].EndsWith(".prefab")))
            {
				return (paths);
			}
			OnSave?.Invoke();
			return paths;
		}

		/// <summary>
		/// On Before Playmode
		/// </summary>
		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			switch (state)
            {
				case PlayModeStateChange.EnteredEditMode:
					EnteredEditMode?.Invoke();
					break;
				case PlayModeStateChange.EnteredPlayMode:
					EnteredPlayMode?.Invoke();
					break;
				case PlayModeStateChange.ExitingEditMode:
					ExitingEditMode?.Invoke();
					break;
				case PlayModeStateChange.ExitingPlayMode:
					ExitingPlayMode?.Invoke();
					break;
			}
		}

		/// <summary>
		/// Before Build
		/// </summary>
		public void OnPreprocessBuild(BuildReport report)
		{
			OnPreBuild?.Invoke();
		}

        public void OnProcessScene(Scene scene, BuildReport report)
        {
			OnPreBuildScene?.Invoke();
		}

        public int callbackOrder
		{
			get { return 0; }
		}
	}
}