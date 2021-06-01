using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.SceneWorkflow;

public class ExempleLoadContext : MonoBehaviour
{
    [SerializeField]
    private ContextListerAsset _contextLister = default;
    [SerializeField]
    private SceneContextAsset _contextToLoad = default;

    public void Load()
    {
        _contextLister.LoadSceneFromLister(_contextToLoad, OnComplete);
    }

    public void OnComplete(SceneContextAsset loadedScenes)
    {
        Debug.Log("context " + loadedScenes.NameContext + " loaded!");
    }
}
