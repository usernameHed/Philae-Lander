
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMono<GameManager>
{
    [Tooltip("vibration of joystick active ?")]
    public bool enableVibration = true;
    

    [Tooltip("Enable lateral movement ?")]
    public Camera cameraMain;

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Application.targetFrameRate = 60;
        cameraMain = Camera.main;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();

        Debug.Log("Scene loaded: " + scene.name);
        EventManager.TriggerEvent(GameData.Event.SceneLoaded);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
