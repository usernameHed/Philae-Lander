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
    private List<Button> listButton = new List<Button>();


    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.SceneLoaded, Init);
    }

    /// <summary>
    /// init Ekko scene
    /// </summary>
    private void Init()
    {

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
    }
}
