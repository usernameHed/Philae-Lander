using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace UnityEssentials.SceneWorkflow.PropertyAttribute.SceneReference
{
    /// <summary>
    /// Serialize Scene Asset References.
    /// </summary>
    [System.Serializable]
    public partial class SceneReference
    {
        // This should only ever be set during serialization/deserialization!
        [SerializeField]
        private string scenePath = string.Empty;

        public string SceneName
        {
            get
            {
                string scenePath = ScenePath;
                string sceneName = Path.GetFileName(scenePath);
                return (sceneName);
            }
#if UNITY_EDITOR
            set
            {
                ScenePath = RenameAsset(ScenePath, value, true);
            }
#endif
        }

        public static void LoadSceneAsync(SceneReference sceneToLoad, UnityAction toCall)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad.SceneNameWithoutExtention, LoadSceneMode.Additive);
                asyncOperation.completed += _ =>
                {
                    toCall?.Invoke();
                };
#if UNITY_EDITOR
            }
            else
            {
                EditorSceneManager.OpenScene(sceneToLoad.ScenePath, OpenSceneMode.Additive);
                toCall?.Invoke();
            }
#endif
        }

        public string SceneNameWithoutExtention
        {
            get
            {
                string sceneName = SceneName;
                string sceneNamewithoutExtention = Path.GetFileNameWithoutExtension(sceneName);
                return (sceneNamewithoutExtention);
            }
        }

        public Scene GetSceneAsset
        {
            get
            {
                //Scene sceneRef = AssetDatabase.LoadAssetAtPath(ScenePath, typeof(Scene));
                Scene sceneRef = SceneManager.GetSceneByPath(ScenePath);
                return (sceneRef);
            }
        }

        // Use this when you want to actually have the scene path
        public string ScenePath
        {
            get
            {
#if UNITY_EDITOR
                // In editor we always use the asset's path
                return GetScenePathFromAsset();
#else
            // At runtime we rely on the stored path value which we assume was serialized correctly at build time.
            // See OnBeforeSerialize and OnAfterDeserialize
            return scenePath;
#endif
            }
            set
            {
                scenePath = value;
#if UNITY_EDITOR
                sceneAsset = GetSceneAssetFromPath();
#endif
            }
        }

        public string SceneDirectory
        {
            get
            {
                string scenePath = ScenePath;
                string directory = GetDirectoryFromCompletPath(scenePath);
                return (directory);
            }
        }

        /// <summary>
        /// from Assets/some/path/assetName.asset
        /// to: Assets/some/path/
        /// </summary>
        /// <returns></returns>
        public static string GetDirectoryFromCompletPath(string completPath)
        {
            string nameFile = Path.GetDirectoryName(completPath);
            return (ReformatPathForUnity(nameFile));
        }

        /// <summary>
        /// change a path from
        /// Assets\path\of\file
        /// to
        /// Assets/path/of/file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReformatPathForUnity(string path, char characterReplacer = '-')
        {
            string formattedPath = path.Replace('\\', '/');
            formattedPath = formattedPath.Replace('|', characterReplacer);
            return (formattedPath);
        }

        public static implicit operator string(SceneReference sceneReference)
        {
            return sceneReference.ScenePath;
        }
    }
}