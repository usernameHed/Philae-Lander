using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[TypeInfoBox("[ILevelLocal] Manage Setup Scene behaviour")]
public class MenuManager : SingletonMono<MenuManager>
{
    [SerializeField]
    private List<Button> listButton = new List<Button>();

    private EventSystem m_EventSystem;
    private int buttonSelected = 0;
    private GameObject lastSelected;


    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.SceneLoaded, Init);
        m_EventSystem = EventSystem.current;
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
        lastSelected = m_EventSystem.currentSelectedGameObject;
        SelectButton(0);
    }

    public void SelectButton(int index)
    {
        buttonSelected = index;
    }

    private void InputGame()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (buttonSelected == 0)
            {
                SceneTransition.Instance.PlayNext();
            }
            else
            {
                SceneTransition.Instance.Previous();
            }
        }
        if (Input.GetButtonDown("Escape"))
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
