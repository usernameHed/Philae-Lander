using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEssentials.Extensions.Editor
{
    [InitializeOnLoad]
    public static class CallBackOnSceneLoad
    {
        public delegate void OnSceneLoad(List<Transform> transformsInScene);
        public static event OnSceneLoad OnSceneLoaded;

        private static List<Transform> _gameObjectsList = new List<Transform>(300);

        static CallBackOnSceneLoad()
        {
            UnityEditor.SceneManagement.EditorSceneManager.sceneOpened += SceneOpenedCallback;
            SceneManager.sceneLoaded += SceneOpenedPlayModeCallback;
        }

        private static void SceneOpenedPlayModeCallback(Scene scene, LoadSceneMode mode)
        {
            _gameObjectsList.Clear();
            AppendAllTransformInSceneToList(scene);
        }

        private static void SceneOpenedCallback(Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            _gameObjectsList.Clear();
            AppendAllTransformInSceneToList(scene);
            OnSceneLoaded?.Invoke(_gameObjectsList);
        }

        /// <summary>
        /// Append all Transform found in this scene into _gameObjectsList list
        /// </summary>
        /// <param name="scene"></param>
        public static void AppendAllTransformInSceneToList(Scene scene)
        {
            if (!scene.IsValid())
            {
                return;
            }
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                Transform[] allChilds = roots[i].GetComponentsInChildren<Transform>(true);
                _gameObjectsList.Append<Transform>(allChilds.ToList());
            }
        }

        /// <summary>
        /// from a current list, append a second list at the end of the first list
        /// </summary>
        /// <typeparam name="T">type of content in the lists</typeparam>
        /// <param name="currentList">list where we append stuffs</param>
        /// <param name="listToAppends">list to append to the other</param>
        public static void Append<T>(this IList<T> currentList, IList<T> listToAppends)
        {
            if (listToAppends == null)
            {
                return;
            }
            for (int i = 0; i < listToAppends.Count; i++)
            {
                currentList.Add(listToAppends[i]);
            }
        }
    }
}