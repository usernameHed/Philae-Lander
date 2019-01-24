using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

[TypeInfoBox("Used to change scene with fading and loading async")]
[TypeInfoBox("Always have a Scene Global Manager, and the a LocalSceneManager to define witch scene we want to swap when play/exit the game")]
public class SceneManagerGlobal : SingletonMono<SceneManagerGlobal>
{
    protected SceneManagerGlobal() { } // guarantee this will be always a singleton only - can't use the constructor!

    private struct SceneCharging
    {
        public string scene;
        public AsyncOperation async;
        public bool isAdditive;
        public bool swapWhenFinishUpload;
        public string sceneToChargeAfterAdditive;
    }

    private List<SceneCharging> sceneCharging = new List<SceneCharging>();

    private bool closing = false;
    private bool newScene = false;

    public void ResetRetry()
    {
        StopCoroutine(Retry());
    }

    ///////////////////////////////////////////////////////////////////////////// gestion asyncrone
    /// <summary>
    /// Add scene to charge in background
    /// </summary>
    /// <param name="scene">scene name</param>
    /// <param name="swapWhenLoaded">Do we launch the scene when it's loaded ?</param>
    /// <param name="additive">Addidive or not ?</param>
    public void StartLoading(string scene, bool swapWhenLoaded = true, bool additive = false, bool fade = false, float speedFade = 1.0f, string sceneToChargeAfterAdditive = "")
    {
        ResetRetry();

        if ((additive && fade) || scene == "")
        {
            Debug.LogError("not possible");
            return;
        }

        SceneCharging sceneToCharge;
        ////////////////store scene to charge
        sceneToCharge.scene = scene;
        sceneToCharge.async = (!additive) ? SceneManager.LoadSceneAsync(scene)
                                         : SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        sceneToCharge.isAdditive = additive;
        sceneToCharge.swapWhenFinishUpload = swapWhenLoaded;
        sceneToCharge.async.allowSceneActivation = sceneToCharge.swapWhenFinishUpload;
        sceneToCharge.sceneToChargeAfterAdditive = sceneToChargeAfterAdditive;

        //add scene to load
        sceneCharging.Add(sceneToCharge);

        if (fade && sceneToCharge.swapWhenFinishUpload)
        {
            ActivateSceneWithFade(sceneCharging.Count - 1, speedFade);
            return;
        }

        if (sceneToCharge.swapWhenFinishUpload)
            StartCoroutine(SwapAfterLoad(sceneCharging.Count - 1));
    }

    /// <summary>
    /// here active an readyToGo scene (scene loaded in memory)
    /// </summary>
    public void ActivateScene(string scene, bool fade = false, float speedFade = 1f)
    {
        for (int i = 0; i < sceneCharging.Count; i++)
        {
            if (sceneCharging[i].scene == scene)
            {
                StartCoroutine(ActiveSceneWithFadeWait(i, speedFade));
                return;
            }
        }
        JumpToScene(scene, fade, speedFade);
    }


    /// <summary>
    /// Here active the right scene, when it's loaded
    /// </summary>
    /// <param name="index">index de la scène de la list</param>
    /// <param name="justActive">ici on est sur que la scène est chargé !</param>
    private void ActivateScene(int index, bool restartIfNotCharged = false, float time = 0.5f)
    {
        if (index < 0 || index >= sceneCharging.Count)
        {
            Debug.Log("ici on n'aurai pas du faire ça...");
            return;
        }

        newScene = true;
        sceneCharging[index].async.allowSceneActivation = true;
        string sceneToChageIfBug = sceneCharging[index].scene;
        string isNewScene = (sceneCharging[index].isAdditive) ? sceneCharging[index].sceneToChargeAfterAdditive : "";
        sceneCharging.RemoveAt(index);

        if (isNewScene != "")
            FindWitchOneToLoadAfterAdditive(isNewScene);


        StartCoroutine(Retry(sceneToChageIfBug));
    }
    private IEnumerator Retry(string sceneToChageIfBug = "")
    {
        yield return new WaitForSeconds(0.2f);
        if (newScene)
        {
            Debug.Log("retry here !!! we can't manage to relauch the scene !");
            JumpToScene(sceneToChageIfBug);
        }
    }

    //relauch activation
    private IEnumerator WaitForActivateScene(int index, float time) { yield return new WaitForSeconds(time); ActivateScene(index, true, time);  }


    private void FindWitchOneToLoadAfterAdditive(string sceneToChargeAfterAdditive)
    {
        EventManager.TriggerEvent(GameData.Event.AdditiveJustFinishLoad);

        Debug.Log("findAfterAdditive;.." + sceneToChargeAfterAdditive);
        SceneManagerLocal local = SceneManagerLocal.Instance;
        local.StartLoading(sceneToChargeAfterAdditive);
    }

    /// <summary>
    /// here lauch [index] scene when the scene is loaded
    /// </summary>
    private IEnumerator SwapAfterLoad(int index)
    {
        Debug.LogWarning("wait before switch... just wait");

        yield return sceneCharging[index].async;
        ActivateScene(index, true);
    }

    /// <summary>
    /// here fade, then active the scene we want
    /// </summary>
    private void ActivateSceneWithFade(int index, float speedFade)
    {
        StartCoroutine(ActiveSceneWithFadeWait(index, speedFade));
    }
    private IEnumerator ActiveSceneWithFadeWait(int index, float speedFade)
    {
        float fadeTime = gameObject.GetComponent<Fading>().BeginFade(1, speedFade);
        yield return new WaitForSeconds(fadeTime / 2);
        ActivateScene(index, true); //re-try to active if failure
    }

    /// <summary>
    /// Unload une scene précédement loadé !
    /// </summary>
    public void UnloadScene(int index)
    {
        SceneManager.UnloadSceneAsync(sceneCharging[index].scene);
        sceneCharging.RemoveAt(index);
    }
    /// <summary>
    /// est appelé si on doit annuler le chargement d'une scene !
    /// </summary>
    public void UnloadScene(string scene)
    {
        for (int i = 0; i < sceneCharging.Count; i++)
        {
            if (sceneCharging[i].scene == scene)
            {
                sceneCharging.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// jump à une scène
    /// </summary>
    [ContextMenu("JumpToScene")]
    public void JumpToScene(string scene, bool fade = false, float fadeSpeed = 1.5f)
    {
        if (!fade)
        {
            SceneManager.LoadScene(scene);
            return;
        }
        
        StartCoroutine(JumpToSceneWithFadeWait(scene, fadeSpeed));
    }
    private IEnumerator JumpToSceneWithFadeWait(string scene, float speed)
    {
        gameObject.GetComponent<Fading>().BeginFade(1, speed);
        yield return new WaitForSeconds(speed / 2);
        JumpToScene(scene);
    }

    /// <summary>
    /// ajoute une scène à celle courrante
    /// </summary>
    [ContextMenu("JumpAdditiveScene")]
    public void JumpAdditiveScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Exit with fade !
    /// </summary>
    public void QuitGame(bool fade = false, float speed = 1.5f)
    {
        if (!fade)
        {
            Quit();
            return;
        }
        StartCoroutine(QuitWithFade(speed));
    }
    private IEnumerator QuitWithFade(float speed)
    {
        gameObject.GetComponent<Fading>().BeginFade(1, speed);
        yield return new WaitForSeconds(speed / 2);
        Quit();
    }

    /// <summary>
    /// Called if we manualy close
    /// </summary>
    private void OnApplicationQuit()
    {
        if (closing)
            return;
        Quit();
    }

    /// <summary>
    /// Exit game (in play mode or in runtime)
    /// </summary>
    [ContextMenu("Quit")]
    private void Quit()
    {
        closing = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}