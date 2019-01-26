using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using System;
using UnityEngine.SceneManagement;

public class PhilaeManager : SingletonMono<PhilaeManager>
{
    [FoldoutGroup("Debug"), Tooltip("defined if this scene have trnasition")]
    public CameraController cameraController;
    [FoldoutGroup("Debug"), Tooltip("defined if this scene have trnasition")]
    public bool releaseScene = false;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("vibration of joystick active ?"), ReadOnly]
    private int switchPlanet = 0;

    [FoldoutGroup("Debug"), Tooltip("text debug to display")]
    public GameObject pausePanel;

    [HideInInspector]
    public PlayerController playerControllerRef;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.SceneLoaded, Init);     
    }

    public void InitPlayer(PlayerController pc)
    {
        playerControllerRef = pc;
    }

    /// <summary>
    /// init Ekko scene
    /// </summary>
    private void Init()
    {
        PauseGame(false);
    }

    public void PlanetChange()
    {
        switchPlanet++;
        if (switchPlanet == 1)
        {
            Debug.Log("change to Music theme");
            SoundManager.GetSingleton.playSound("event:/Music/" + GameData.Sounds.Music_Theme.ToString());
        }
    }

    public void PauseGame(bool pause)
    {
        Time.timeScale = (pause) ? 0f : 1f;
        pausePanel.SetActive(pause);
    }

    private void InputGame()
    {
        if (!releaseScene)
        {
            if (PlayerConnected.Instance.GetPlayer(0).GetButtonDoublePressDown("Escape"))
            {
                Debug.Log("quit ?");
                SceneTransition.Instance.Quit();
            }
            if (PlayerConnected.Instance.GetPlayer(0).GetButtonDoublePressDown("Restart"))
            {
                Debug.Log("Restart ?");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            if (PlayerConnected.Instance.GetPlayer(0).GetButtonUp("Escape"))
            {
                Debug.Log("quit ?");
                SceneTransition.Instance.Previous();
            }
            if (PlayerConnected.Instance.GetPlayer(0).GetButtonUp("Restart"))
            {
                Debug.Log("Restart ?");
                //ScoreManager.Instance.Save();
                SceneTransition.Instance.PlayNext();
            }
        }

    }

    private void Update()
    {
        InputGame();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.SceneLoaded, Init);
    }
}
