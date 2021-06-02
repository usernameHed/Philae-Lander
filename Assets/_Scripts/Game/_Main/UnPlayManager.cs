using UnityEngine;

using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

/// <summary>
/// ScoreManager Description
/// </summary>
[ExecuteInEditMode]
public class UnPlayManager : MonoBehaviour
{
    public PhilaeManager philaeManager;

    void Awake()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("Awake in edit mode");
            if (PlayerPrefs.GetInt("should spawn") == 1)
            {
                PlayerPrefs.SetInt("should spawn", 0);
                philaeManager.needRecalculate = true;
            }
        }
        else
        {
            //gameManager.HardReload();
            
            //StartCoroutine(waitBeforeLoad());
        }
    }

    void OnDestroy()
    {
        if (Application.isPlaying)
        {
            Debug.Log("Destroyed in play mode");
            PlayerPrefs.SetInt("should spawn", 1);
        }
    }
}
