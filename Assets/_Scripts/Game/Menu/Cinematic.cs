using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Video;

[TypeInfoBox("[ILevelLocal] Manage Setup Scene behaviour")]
public class Cinematic : SingletonMono<Cinematic>
{
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private VideoPlayer video;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.SceneLoaded, Init);
        EventManager.StartListening(GameData.Event.SwitchKeyBoardOrGamePad, SwitchKeyBoardOrGamePad);
        video.loopPointReached += EndReached;
    }

    /// <summary>
    /// init Ekko scene
    /// </summary>
    private void Init()
    {
        SwitchKeyBoardOrGamePad();
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
        /*if (PlayerConnected.Instance.GetPlayer(0).GetButtonDown("Escape"))
        {
            Debug.Log("quit ?");
            //ScoreManager.Instance.Save();
            SceneTransition.Instance.Previous();
        }*/
    }

    private void Update()
    {
        InputGame();
    }

    private void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        SceneTransition.Instance.PlayNext();
    }

    private void OnDisable()
    {
        video.loopPointReached -= EndReached;
        EventManager.StopListening(GameData.Event.SceneLoaded, Init);
        EventManager.StopListening(GameData.Event.SwitchKeyBoardOrGamePad, SwitchKeyBoardOrGamePad);
    }
}
