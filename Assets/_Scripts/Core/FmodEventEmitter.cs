using UnityEngine;
using System.Collections;

public class FmodEventEmitter : MonoBehaviour
{
    public string additionnalName = "";
    public Transform addIdOfObject;

    [SerializeField]
    private FMODUnity.StudioEventEmitter emitter;   //l'emitter attaché à l'objet


    void Start()
    {
        if (!emitter)
            emitter = gameObject.GetComponent<FMODUnity.StudioEventEmitter>();  //init l'emitter
        string addParent = (addIdOfObject) ? addIdOfObject.GetInstanceID().ToString() : "";
        if (emitter && emitter.Event != "")
            SoundManager.GetSingleton.AddKey(emitter.Event + additionnalName + addParent, this);
    }

    /// <summary>
    /// play l'emmiter
    /// </summary>
    public void Play()
    {
        if (!gameObject || !emitter)
            return;
        emitter.Play();
    }

    /// <summary>
    /// stop l'emmiter
    /// </summary>
    public void Stop()
    {
        if (!gameObject || !emitter)
            return;
        emitter.Stop();
    }

    /// <summary>
    /// change les paramettres de l'emmiter
    /// </summary>
    /// <param name="paramName"></param>
    /// <param name="value"></param>
    public void SetParameterValue(string paramName, float value)
    {
        if (!gameObject || !emitter)
            return;
        emitter.SetParameter(paramName, value);
    }

    private void OnDestroy()
    {
        string addParent = (addIdOfObject) ? addIdOfObject.GetInstanceID().ToString() : "";
        if (emitter && emitter.Event != "" && SoundManager.GetSingleton)
            SoundManager.GetSingleton.DeleteKey(emitter.Event + additionnalName + addParent, this);
    }
}
