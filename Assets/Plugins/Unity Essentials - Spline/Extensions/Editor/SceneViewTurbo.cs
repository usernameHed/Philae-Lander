using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Spline.Extensions.Editor
{
    /// <summary>
    /// Update the sceneView every Update()
    /// </summary>
    [InitializeOnLoad]
    public static class SceneViewTurbo
    {
        /// <summary>
        /// subscribe to EditorApplication.update to be able to call EditorApplication.QueuePlayerLoopUpdate();
        /// We need to do it because unity doens't update when no even are triggered in the scene.
        /// 
        /// Then, Start the timer, this timer will increase every frame (in play or in editor mode), like the normal Time
        /// </summary>
        static SceneViewTurbo()
        {
            EditorApplication.update += EditorUpdate;
        }

        /// <summary>
        /// called every editorUpdate, tell unity to execute the Update() method
        /// even if no event are triggered in the scene
        /// in the scene.
        /// 
        /// called every frame in play and in editor, thanks to EditorApplication.QueuePlayerLoopUpdate();
        /// add to the current time, then save the current time for later.
        /// </summary>
        private static void EditorUpdate()
        {
            if (!Application.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
    }
}