using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.SceneManagement;
using UnityEssentials.PropertyAttribute.readOnly;

public class PhilaeManager : SingletonMono<PhilaeManager>
{
    [Tooltip("text debug to display")]
    public LDManager ldManager;


    [Tooltip("defined if this scene have trnasition")]
    public CameraControllerOld cameraController;
    [Tooltip("defined if this scene have trnasition")]
    public bool releaseScene = false;
    [SerializeField, Tooltip("vibration of joystick active ?"), ReadOnly]
    private int switchPlanet = 0;
    [SerializeField, Tooltip("vibration of joystick active ?"), ReadOnly]
    private float waitUntilRestart = 1f;


    [Tooltip("text debug to display")]
    public GameObject pausePanel;
    [Tooltip("text debug to display")]
    public bool needRecalculate = false;

    [HideInInspector]
    public PlayerController playerControllerRef;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.SceneLoaded, Init);
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
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
            if (Input.GetButtonDown("Escape"))
            {
                Debug.Log("quit ?");
                SceneTransition.Instance.Quit();
            }
            if (Input.GetButtonDown("Restart"))
            {
                Debug.Log("Restart ?");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            if (Input.GetButtonDown("Escape")
                || Input.GetButtonDown("Start"))
            {
                Debug.Log("quit ?");
                SceneTransition.Instance.Previous();
            }
            if (Input.GetButtonDown("Restart"))
            {
                Debug.Log("Restart ?");
                //ScoreManager.Instance.Save();
                SceneTransition.Instance.PlayNext();
            }
        }
        /*if (PlayerConnected.Instance.GetPlayer(0).GetButton("Focus"))
        {
            Debug.Break();
        }
        */

    }

    private void PauseInspector()
    {

    }

    private void GameOver()
    {
        Invoke("EndGame", waitUntilRestart);
    }

    private void EndGame()
    {
        Debug.Log("Restart ?");
        //ScoreManager.Instance.Save();
        SceneTransition.Instance.PlayNext();
    }

    private void Update()
    {
        InputGame();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.SceneLoaded, Init);
        EventManager.StopListening(GameData.Event.GameOver, GameOver);
    }
}
