using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEssentials.SceneWorkflow.extension;

namespace UnityEssentials.SceneWorkflow
{
    [CreateAssetMenu(fileName = "Context Lister", menuName = "UnityEssentials/Scene Workflow/Context Lister")]
    public class ContextListerAsset : ScriptableObject
    {
        [SerializeField]
        private List<SceneContextAsset> _contextList = new List<SceneContextAsset>();
        public void AddContext(SceneContextAsset scene)
        {
            _contextList.AddIfNotContain(scene);
        }
        public int CountSceneToLoad { get { return (_contextList.Count); } }
        public SceneContextAsset GetSceneAtIndex(int index) { return (_contextList[index]); }

        public int LastIndexUsed = -1;

        private List<AsyncOperation> _asyncOperations = new List<AsyncOperation>(20);
        public delegate void FuncToCallOnComplete(SceneContextAsset loadedScenes);
        private FuncToCallOnComplete _refFuncToCallOnComplete = null;

        private SceneContextAsset _lastSceneAssetUsed = null;
        private bool _isActivatingScene = false;

        /// <summary>
        /// Load a list of scene: ALL scene present in game
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScenesByIndex(int index, FuncToCallOnComplete funcToCallOnComplete, bool hardReload)
        {
            if (_isActivatingScene && !hardReload)
            {
                throw new System.Exception("can't load twice in a row...");
            }
            LastIndexUsed = index;
            LoadSceneFromLister(_contextList[index], funcToCallOnComplete);
        }

        public void LoadSceneFromLister(SceneContextAsset lister, FuncToCallOnComplete funcToCallOnComplete)
        {
            _isActivatingScene = true;
            _lastSceneAssetUsed = lister;
            _refFuncToCallOnComplete = funcToCallOnComplete;

            if (Application.isPlaying)
            {
                _asyncOperations.Clear();
                for (int i = 0; i < _lastSceneAssetUsed.SceneToLoad.Count; i++)
                {
                    if (i == 0)
                    {
                        SceneManager.LoadScene(_lastSceneAssetUsed.SceneToLoad[0].ScenePath, LoadSceneMode.Single);
                        if (_lastSceneAssetUsed.SceneToLoad.Count == 1)
                        {
                            CalledWhenAllSceneAreLoaded();
                            return;
                        }
                        else
                        {
                            ExtCoroutineWithoutMonoBehaviour.StartUniqueCoroutine(WaitForLoading());
                        }
                    }
                    else
                    {
                        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_lastSceneAssetUsed.SceneToLoad[i].ScenePath, LoadSceneMode.Additive);
                        asyncOperation.allowSceneActivation = false;
                        asyncOperation.completed += OnSecondarySceneCompleted;
                        _asyncOperations.Add(asyncOperation);
                    }
                }
            }
#if UNITY_EDITOR
            else
            {
                EditorUtility.DisplayProgressBar("Loading scene", "", 0f);
                for (int i = 0; i < _lastSceneAssetUsed.SceneToLoad.Count; i++)
                {
                    if (_lastSceneAssetUsed.SceneToLoad[i].ScenePath.IsNullOrEmpty())
                    {
                        continue;
                    }
                    EditorSceneManager.OpenScene(_lastSceneAssetUsed.SceneToLoad[i].ScenePath,
                        (i == 0) ? OpenSceneMode.Single : OpenSceneMode.Additive);
                    EditorUtility.DisplayProgressBar("Loading scene", "", i * 1 / _lastSceneAssetUsed.SceneToLoad.Count);
                }
                EditorUtility.ClearProgressBar();
                CalledWhenAllSceneAreLoaded();
            }
#endif
        }

        private void UnloadCurrentLoadingScenes()
        {
            if (_lastSceneAssetUsed == null)
            {
                return;
            }
            for (int i = 0; i < _lastSceneAssetUsed.SceneToLoad.Count; i++)
            {
                SceneManager.UnloadSceneAsync(_lastSceneAssetUsed.SceneToLoad[i].ScenePath);
            }
        }

        /// <summary>
        /// called when ONE scene is loaded (we need to wait for all others scene...)
        /// </summary>
        /// <param name="obj"></param>
        private void FirstSceneLoaded(AsyncOperation obj)
        {
            _asyncOperations.Remove(obj);
            if (_asyncOperations.Count == 0)
            {
                CalledWhenAllSceneAreLoaded();
            }
            else
            {
                ExtCoroutineWithoutMonoBehaviour.StartUniqueCoroutine(WaitForLoading());
            }
        }


        private IEnumerator WaitForLoading()
        {
            yield return new WaitUntil(() => AllAditiveSceneAreReady());
            ActiveAllScenes();
        }


        /// <summary>
        /// test if ALL scenes are loaded
        /// </summary>
        /// <returns></returns>
        private bool AllAditiveSceneAreReady()
        {
            for (int i = 0; i < _asyncOperations.Count; i++)
            {
                if (_asyncOperations[i].progress < 0.9f)
                {
                    return (false);
                }
            }
            return (true);
        }

        private void ActiveAllScenes()
        {
            for (int i = 0; i < _asyncOperations.Count; i++)
            {
                _asyncOperations[i].allowSceneActivation = true;
            }
        }

        private void OnSecondarySceneCompleted(AsyncOperation obj)
        {
            _asyncOperations.Remove(obj);
            if (_asyncOperations.Count == 0)
            {
                ExtCoroutineWithoutMonoBehaviour.CleanUp();
                CalledWhenAllSceneAreLoaded();
            }
        }

        public void CalledWhenAllSceneAreLoaded()
        {
            _refFuncToCallOnComplete?.Invoke(_lastSceneAssetUsed);
            _refFuncToCallOnComplete = null;
            _lastSceneAssetUsed = null;
            _isActivatingScene = false;
        }

        //end of class
    }

    //end of nameSpace
}