using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEssentials.Peek.ToolbarExtender;

namespace UnityEssentials.Peek
{
    [InitializeOnLoad]
    public static class BookMarkSceneEvent
    {
        static BookMarkSceneEvent()
        {
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += SceneOpenedCallback;
            SceneManager.sceneLoaded += SceneOpenedPlayModeCallback;
        }

        private static void SceneOpenedPlayModeCallback(Scene scene, LoadSceneMode mode)
        {
            PeekLogic.UpdateSceneGameObjectFromSceneLoad(scene);
        }

        private static void SceneOpenedCallback(Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            PeekLogic.UpdateSceneGameObjectFromSceneLoad(scene);
        }
    }
}