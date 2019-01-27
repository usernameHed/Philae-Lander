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

    public int MusicState
    {
        get
        {
            return (musicState);
        }
        set
        {
            if (musicState != value)
            {
                musicState = value;
                stateMusicChanged();
            }
        }
    }

    private int musicState = 0;
    //[FMODUnity.EventRef]
    private static SoundManager instance;
    public static SoundManager GetSingleton
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
    private void stateMusicChanged()
    {
        playSound(GetEmitter("BackgroundMusic"), "Checkpoint", musicState);
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
    public void playSound(string soundTag, bool stop = false)
    {
        if (soundTag == null || soundTag == "")
            return;

        if (!soundTag.Contains("event:/"))
            soundTag = "event:/SFX/" + soundTag;
        playSound(GetEmitter(soundTag), stop);
        //FMODUnity.RuntimeManager.PlayOneShot("2D sound");   //methode 1 
    }

    /// <summary>
    /// ici play l'emitter (ou le stop)
    /// </summary>
    /// <param name="emitterScript"></param>
    public void playSound(FmodEventEmitter emitterScript, bool stop = false)
    {

        if (!emitterScript)
        {
            Debug.LogWarning("Emmiter SOund not found !!");
            return;
        }

        if (!stop)
            emitterScript.Play();
        else
            emitterScript.Stop();
    }

    /// <summary>
    /// ici change le paramettre de l'emitter
    /// </summary>
    /// <param name="emitterScript"></param>
    public void playSound(FmodEventEmitter emitterScript, string paramName, float value)
    {
        emitterScript.SetParameterValue(paramName, value);
    }

    #endregion
}
