using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

[TypeInfoBox("[ILevelLocal] Manage Setup Scene behaviour")]
public class MenuManager : SingletonMono<MenuManager>
{
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private List<Button> listButton;


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
        SwitchKeyBoardOrGamePad();
    }

    /// <summary>
    /// swithc text input
    /// </summary>
    private void SwitchKeyBoardOrGamePad()
    {
        text.text = (PlayerConnected.Instance.keyboardActive) ? "keyboard" : "joystick";
    }

    private void Start()
    {
        listButton[0].Select();
    }

    private void InputGame()
    {
        if (PlayerConnected.Instance.GetPlayer(0).GetButtonDown("Escape"))
        {
            Debug.Log("quit ?");
            //ScoreManager.Instance.Save();
            SceneTransition.Instance.Previous();
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
