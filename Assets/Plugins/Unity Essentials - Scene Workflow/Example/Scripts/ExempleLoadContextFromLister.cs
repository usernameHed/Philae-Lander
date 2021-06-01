using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.SceneWorkflow;

public class ExempleLoadContextFromLister : MonoBehaviour
{
    [SerializeField]
    private ContextListerAsset _contextLister = default;
    [SerializeField]
    private int _index = 0;

    public void Load()
    {
        _contextLister.LoadScenesByIndex(_index, OnComplete, false);
    }

    public void OnComplete(SceneContextAsset loadedScenes)
    {
        Debug.Log("context " + loadedScenes.NameContext + " loaded!");
    }
}
