#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEssentials.SceneWorkflow.PropertyAttribute.SceneReference
{
    /// <summary>
    /// EDITOR PART of the SceneReference
    /// </summary>
    public partial class SceneReference : ISerializationCallbackReceiver
    {
        // What we use in editor to select the scene
        [SerializeField] private UnityEngine.Object sceneAsset = null;

        // Called to prepare this data for serialization. Stubbed out when not in editor.
        public void OnBeforeSerialize()
        {
            HandleBeforeSerialize();
        }

        // Called to set up data for deserialization. Stubbed out when not in editor.
        public void OnAfterDeserialize()
        {
            // We sadly cannot touch assetdatabase during serialization, so defer by a bit.
            EditorApplication.update += HandleAfterDeserialize;
        }

        bool IsValidSceneAsset
        {
            get
            {
                if (sceneAsset == null)
                    return false;
                return sceneAsset.GetType().Equals(typeof(SceneAsset));
            }
        }

        /// <summary>
        /// rename an asset, and return the name
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static string RenameAsset(string oldPath, string newName, bool refreshAsset)
        {
            if (Directory.Exists(oldPath))
            {
                throw new System.Exception("a file must be given, not a directory");
            }

            string pathWhitoutName = Path.GetDirectoryName(oldPath);
            pathWhitoutName = ReformatPathForUnity(pathWhitoutName);
            string extension = Path.GetExtension(oldPath);
            string newWantedName = ReformatPathForUnity(newName);

            AssetDatabase.RenameAsset(oldPath, newWantedName);
            if (refreshAsset)
            {
                AssetDatabase.Refresh();
            }
            string newPath = pathWhitoutName + "/" + newWantedName + extension;
            Debug.Log("renamed to: " + newPath);
            return (newPath);
        }

        /// <summary>
        /// get the first scene found in a directory, as ExtSceneReference
        /// </summary>
        /// <param name="directory">where to search</param>
        /// <returns>first found scenes</returns>
        public static SceneReference GetSceneInDirectories(string directory = "Assets/")
        {
            List<SceneReference> scenes = GetScenesInDirectories(directory);
            if (scenes.Count == 0)
            {
                return (null);
            }
            return (scenes[0]);
        }

        /// <summary>
        /// get the first scene found in a directory, as ExtSceneReference
        /// </summary>
        /// <param name="directory">where to search</param>
        /// <returns>first found scenes</returns>
        public static SceneReference GetSceneFromPath(string relativePath)
        {
            SceneReference extSceneReference = new SceneReference();
            extSceneReference.ScenePath = relativePath;
            return (extSceneReference);
        }

        /// <summary>
        /// return a Scene Reference to a ExtSceneReference reference
        /// </summary>
        /// <param name="directory">where to search</param>
        /// <returns>first found scenes</returns>
        public static SceneReference GetSceneFromPath(Scene scene)
        {
            SceneReference extSceneReference = new SceneReference();
            extSceneReference.ScenePath = scene.path;
            return (extSceneReference);
        }

        /// <summary>
        /// return a Scene Reference to a ExtSceneReference reference
        /// </summary>
        /// <param name="directory">where to search</param>
        /// <returns>first found scenes</returns>
        public static SceneReference[] GetSceneFromPath(Scene[] scenes)
        {
            SceneReference[] _sceneRefs = new SceneReference[scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                SceneReference extSceneReference = new SceneReference();
                extSceneReference.ScenePath = scenes[i].path;
                _sceneRefs[i] = extSceneReference;
            }
            return (_sceneRefs);
        }

        /// <summary>
        /// return a Scene Reference to a ExtSceneReference reference
        /// </summary>
        /// <param name="directory">where to search</param>
        /// <returns>first found scenes</returns>
        public static SceneReference[] GetAllActiveScene()
        {
            SceneReference[] _sceneRefs = new SceneReference[EditorSceneManager.sceneCount];
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                SceneReference extSceneReference = new SceneReference();
                extSceneReference.ScenePath = EditorSceneManager.GetSceneAt(i).path;
                _sceneRefs[i] = extSceneReference;
            }
            return (_sceneRefs);
        }

        /// <summary>
        /// get a lsit of all scene found in a directory, as ExtSceneReference
        /// </summary>
        /// <param name="directory">where to search</param>
        /// <returns>list of found scenes</returns>
        public static List<SceneReference> GetScenesInDirectories(string directory = "Assets/")
        {
            List<UnityEngine.Object> scenes = GetAssetsByGenericType<UnityEngine.Object>(directory, "*.unity");
            List<SceneReference> scenesReferences = new List<SceneReference>();
            for (int i = 0; i < scenes.Count; i++)
            {
                SceneReference extSceneReference = new SceneReference();
                extSceneReference.ScenePath = AssetDatabase.GetAssetPath(scenes[i]);
                scenesReferences.Add(extSceneReference);
            }
            return (scenesReferences);
        }

        /// <summary>
        /// find all asset of type T
        /// </summary>
        /// <typeparam name="T">Type to find</typeparam>
        /// <returns>all asset found with type T, return an list<Object>, you have to cast them to use them, see GetAssetsByRaceScriptableObjects for an exemple</returns>
        public static List<T> GetAssetsByGenericType<T>(string directory = "Assets/", string searchExtension = "*.asset") where T : UnityEngine.Object
        {
            List<T> AllAssetFound = new List<T>();

            DirectoryInfo dir = new DirectoryInfo(directory);
            FileInfo[] infoAssets = dir.GetFiles(searchExtension, SearchOption.AllDirectories);

            for (int i = 0; i < infoAssets.Length; i++)
            {
                string relativePath = ConvertAbsoluteToRelativePath(infoAssets[i].FullName);

                System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(relativePath);
                if (assetType == null)
                {
                    continue;
                }
                T asset = (T)AssetDatabase.LoadAssetAtPath(relativePath, typeof(T));
                if (asset)
                {
                    AllAssetFound.Add(asset);
                }
            }
            return (AllAssetFound);
        }

        public static string ConvertAbsoluteToRelativePath(string absolutePath)
        {
            string gameDataFolder = Application.dataPath;
            absolutePath = ReformatPathForUnity(absolutePath);

            if (absolutePath.StartsWith(gameDataFolder))
            {
                string relativepath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
                return (relativepath);
            }
            return (absolutePath);
        }

        public UnityEngine.Object GetSceneAssetAsObject
        {
            get
            {
                UnityEngine.Object sceneRef = AssetDatabase.LoadAssetAtPath(ScenePath, typeof(Scene));
                return (sceneRef);
            }
        }

        private SceneAsset GetSceneAssetFromPath()
        {
            if (string.IsNullOrEmpty(scenePath))
                return null;
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        }

        private string GetScenePathFromAsset()
        {
            if (sceneAsset == null)
                return string.Empty;
            return AssetDatabase.GetAssetPath(sceneAsset);
        }

        private void HandleBeforeSerialize()
        {
            // Asset is invalid but have Path to try and recover from
            if (IsValidSceneAsset == false && string.IsNullOrEmpty(scenePath) == false)
            {
                sceneAsset = GetSceneAssetFromPath();
                if (sceneAsset == null)
                    scenePath = string.Empty;

                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
            // Asset takes precendence and overwrites Path
            else
            {
                scenePath = GetScenePathFromAsset();
            }
        }

        private void HandleAfterDeserialize()
        {
            EditorApplication.update -= HandleAfterDeserialize;
            // Asset is valid, don't do anything - Path will always be set based on it when it matters
            if (IsValidSceneAsset)
                return;

            // Asset is invalid but have path to try and recover from
            if (string.IsNullOrEmpty(scenePath) == false)
            {
                sceneAsset = GetSceneAssetFromPath();
                // No asset found, path was invalid. Make sure we don't carry over the old invalid path
                if (sceneAsset == null)
                    scenePath = string.Empty;

                if (Application.isPlaying == false)
                    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }
}
#endif