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

    [FoldoutGroup("Debug"), Tooltip("text debug to display")]
    public GameObject pausePanel;

    [FoldoutGroup("Debug"), Tooltip("text debug to display")]
    public TextMeshProUGUI text;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.SceneLoaded, Init);
        EventManager.StartListening(GameData.Event.SwitchKeyBoardOrGamePad, SwitchKeyBoardOrGamePad);        
    }

    /// <summary>
    /// init Ekko scene
    /// </summary>
    private void Init()
    {
        PauseGame(false);
        SwitchKeyBoardOrGamePad();
    }

    public void PauseGame(bool pause)
    {
        Time.timeScale = (pause) ? 0f : 1f;
        pausePanel.SetActive(pause);
    }

    /// <summary>
    /// swithc text input
    /// </summary>
    private void SwitchKeyBoardOrGamePad()
    {
        text.text = (PlayerConnected.Instance.keyboardActive) ? "keyboard" : "joystick";
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
        EventManager.StopListening(GameData.Event.SwitchKeyBoardOrGamePad, SwitchKeyBoardOrGamePad);
    }
}
