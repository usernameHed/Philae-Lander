using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// </summary>
//[RequireComponent(typeof(CircleCollider2D))]
public class SoundManager : SerializedMonoBehaviour                                   //commentaire
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    
    [SerializeField]
    public Dictionary<string, FmodEventEmitter> soundsEmitter = new Dictionary<string, FmodEventEmitter>();

    //public FmodEventEmitter musicEmitterScript;

    //[FMODUnity.EventRef]
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get { return instance; }
    }

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>

    private void SetSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Awake()                                                    //initialisation referencce
    {
        SetSingleton();                                                  //set le script en unique ?
    }
    #endregion

    #region core script

    /// <summary>
    /// appelé lorsque la state de la musique a changé
    /// </summary>
    private void StateMusicChanged(string musicName, string stateName, int musicState)
    {
        PlaySound(GetEmitter(musicName), stateName, musicState);
    }

    /// <summary>
    /// ajoute une key dans la liste
    /// </summary>
    public void AddKey(string key, FmodEventEmitter value)
    {
        foreach (KeyValuePair<string, FmodEventEmitter> sound in soundsEmitter)
        {
            if (key == sound.Key)
            {
                soundsEmitter[sound.Key] = value;
                return;
            }
        }
        soundsEmitter.Add(key, value);
    }

    /// <summary>
    /// ajoute une key dans la liste
    /// </summary>
    public void DeleteKey(string key, FmodEventEmitter value)
    {
        foreach (KeyValuePair<string, FmodEventEmitter> sound in soundsEmitter)
        {
            if (key == sound.Key)
            {
                soundsEmitter.Remove(key);
                return;
            }
        }
        //Debug.Log("key sound not found");
    }

    private FmodEventEmitter GetEmitter(string soundTag)
    {
        foreach (KeyValuePair<string, FmodEventEmitter> sound in soundsEmitter)
        {
            if (soundTag == sound.Key)
            {
                return (sound.Value);
            }
        }
        return (null);
    }

    /// <summary>
    /// joue un son de menu (sans emmiter)
    /// </summary>
    public void PlaySound(string soundTag, bool play = true)
    {
        if (soundTag == null || soundTag == "")
            return;

        if (!soundTag.Contains("event:/"))
            soundTag = "event:/SFX/" + soundTag;
        PlaySound(GetEmitter(soundTag), play);
        //FMODUnity.RuntimeManager.PlayOneShot("2D sound");   //methode 1 
    }

    /// <summary>
    /// ici play l'emitter (ou le stop)
    /// </summary>
    /// <param name="emitterScript"></param>
    public void PlaySound(FmodEventEmitter emitterScript, bool play = true)
    {

        if (!emitterScript)
        {
            Debug.LogWarning("Emmiter SOund not found !!");
            return;
        }

        if (play)
            emitterScript.Play();
        else
            emitterScript.Stop();
    }

    /// <summary>
    /// ici change le paramettre de l'emitter
    /// </summary>
    /// <param name="emitterScript"></param>
    public void PlaySound(FmodEventEmitter emitterScript, string paramName, float value)
    {
        emitterScript.SetParameterValue(paramName, value);
    }

    #endregion
}
