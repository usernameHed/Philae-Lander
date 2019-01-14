using UnityEngine;
using Rewired;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;


/// <summary>
/// effectue une vibration
/// </summary>
[Serializable]
public struct Vibration
{
    [FoldoutGroup("Vibration"), Tooltip("vibre le rotor droit"), SerializeField]
    public bool vibrateLeft;
    [FoldoutGroup("Vibration"), EnableIf("vibrateLeft"), Range(0, 1), Tooltip("force du rotor"), SerializeField]
    public float strenthLeft;
    [FoldoutGroup("Vibration"), EnableIf("vibrateLeft"), Range(0, 10), Tooltip("temps de vibration"), SerializeField]
    public float durationLeft;

    [FoldoutGroup("Vibration"), Tooltip("cooldown du jump"), SerializeField]
    public bool vibrateRight;
    [FoldoutGroup("Vibration"), EnableIf("vibrateRight"), Range(0, 1), Tooltip("cooldown du jump"), SerializeField]
    public float strenthRight;
    [FoldoutGroup("Vibration"), EnableIf("vibrateRight"), Range(0, 10), Tooltip("cooldown du jump"), SerializeField]
    public float durationRight;
}

/// <summary>
/// Gère la connexion / déconnexion des manettes
/// <summary>
[TypeInfoBox("Manage global input gamePad/keyboard and switch")]
public class PlayerConnected : SingletonMono<PlayerConnected>
{
    protected PlayerConnected() { } // guarantee this will be always a singleton only - can't use the constructor!

    #region variable

    [FoldoutGroup("GamePlay"), Tooltip("all Control For Reactivating GamePad")]
    public List<string> allControlGamePad = new List<string>();

    [FoldoutGroup("Debug"), Tooltip("Active les vibrations"), ReadOnly]
    public bool enabledVibration = true;
    [FoldoutGroup("Debug"), Tooltip("is keaybord defined as active ?"), ReadOnly]
    public bool keyboardActive = true;
    [FoldoutGroup("Debug"), Tooltip("show gamePad active"), ReadOnly]
    public bool[] playerArrayConnected;                      //tableau d'état des controller connecté
    [FoldoutGroup("Debug"), Tooltip("Active les vibrations")]
    public bool simulatePlayerOneifNoGamePad = false;   //Si aucune manette n'est connecté, active le player 1 avec le clavier !
    

    private short playerNumber = 4;                     //size fixe de joueurs (0 = clavier, 1-4 = manette)
    private Player[] playersRewired;                 //tableau des class player (rewired)
    private float timeToGo;
    private FrequencyCoolDown desactiveVibrationAtStart = new FrequencyCoolDown();
    private float timeDesactiveAtStart = 2f;

    #endregion

    #region  initialisation
    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.SceneLoaded, Init);
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        playerArrayConnected = new bool[playerNumber];                           //initialise 
        playersRewired = new Player[playerNumber];
        InitPlayerRewired();                                                //initialise les event rewired
        InitController();                                                   //initialise les controllers rewired   
    }

    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        enabledVibration = GameManager.Instance.enableVibration;
        desactiveVibrationAtStart.StartCoolDown(timeDesactiveAtStart);
        TestIfActiveKeyBoard();
    }

    private void TestIfActiveKeyBoard()
    {
        //if no gamepad, active keyboard
        if (NoPlayer())
        {
            SetActiveKeyBoard(true);    //here keybard/mouse is active
        }
        else
        {
            SetActiveKeyBoard(false);   //here gamePad are active
        }
    }

    /// <summary>
    /// active or not Mouse !
    /// </summary>
    private void SetActiveKeyBoard(bool active)
    {
        if (active == keyboardActive)
            return;

        keyboardActive = active;
        if (keyboardActive)
        {
            Debug.Log("keyboard actived");
            ActiveKeyboardForPlayer(true);
        }
        else
        {
            Debug.Log("gamePad actived");
            ActiveKeyboardForPlayer(false);
        }
    }

    /// <summary>
    /// initialise les players
    /// </summary>
    private void InitPlayerRewired()
    {
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;

        //parcourt les X players...
        for (int i = 0; i < playerNumber; i++)
        {
            playersRewired[i] = ReInput.players.GetPlayer(i);       //get la class rewired du player X
            playerArrayConnected[i] = false;                             //set son état à false par défault
        }

        SetKeyboardForPlayerOne();
    }

    /// <summary>
    /// défini le keyboard pour le joueur 1 SI il n'y a pas de manette;
    /// </summary>
    private void SetKeyboardForPlayerOne()
    {
        if (simulatePlayerOneifNoGamePad && NoPlayer())
            playerArrayConnected[0] = true;
    }

    /// <summary>
    /// initialise les players (manettes)
    /// </summary>
    private void InitController()
    {
        foreach (Player player in ReInput.players.GetPlayers(true))
        {
            foreach (Joystick j in player.controllers.Joysticks)
            {
                SetPlayerController(player.id, true);
                break;
            }
        }
    }
    #endregion

    #region core script

    /// <summary>
    /// actualise le player ID si il est connecté ou déconnecté
    /// </summary>
    /// <param name="id">id du joueur</param>
    /// <param name="isConnected">statue de connection du joystick</param>
    private void SetPlayerController(int id, bool isConnected)
    {
        playerArrayConnected[id] = isConnected;
    }

    private void UpdatePlayerController(int id, bool isConnected)
    {
        playerArrayConnected[id] = isConnected;
        TestIfActiveKeyBoard();
    }

    /// <summary>
    /// renvoi s'il n'y a aucun player de connecté
    /// </summary>
    /// <returns></returns>
    public bool NoPlayer()
    {
        for (int i = 0; i < playerArrayConnected.Length; i++)
        {
            if (playerArrayConnected[i])
                return (false);
        }
        return (true);
    }
    public int GetNbPlayer()
    {
        int nb = 0;
        for (int i = 0; i < playerArrayConnected.Length; i++)
        {
            if (playerArrayConnected[i])
                nb++;
        }
        return (nb);
    }

    /// <summary>
    /// get id of player
    /// </summary>
    /// <param name="id"></param>
    public Player GetPlayer(int id)
    {
        if (id == -1)
            return (ReInput.players.GetSystemPlayer());
        else if (id >= 0 && id < playerNumber)
            return (playersRewired[id]);
        Debug.LogError("problème d'id");
        return (null);
    }
    /// <summary>
    /// renvoi vrai si n'importe quel gamePad/joueur active
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool GetButtonDownFromAnyGamePad(string action)
    {
        for (int i = 0; i < playersRewired.Length; i++)
        {
            if (playersRewired[i].GetButtonDown(action))
                return (true);
        }
        return (false);
    }
    public bool GetButtonUpFromAnyGamePad(string action)
    {
        for (int i = 0; i < playersRewired.Length; i++)
        {
            if (playersRewired[i].GetButtonUp(action))
                return (true);
        }
        return (false);
    }

    /// <summary>
    /// set les vibrations du gamepad
    /// </summary>
    /// <param name="id">l'id du joueur</param>
    public void SetVibrationPlayer(int id, int motorIndex = 0, float motorLevel = 1.0f, float duration = 1.0f)
    {
        if (!enabledVibration)
            return;

        //if we are at start of the game, don't vibrate
        if (!desactiveVibrationAtStart.IsReady())
            return;

        GetPlayer(id).SetVibration(motorIndex, motorLevel, duration);
    }

    /// <summary>
    /// set les vibrations du gamepad
    /// </summary>
    /// <param name="id">l'id du joueur</param>
    public void SetVibrationPlayer(int id, Vibration vibration)
    {
        if (!enabledVibration)
            return;

        //if we are at start of the game, don't vibrate
        if (!desactiveVibrationAtStart.IsReady())
            return;

        if (vibration.vibrateLeft)
            GetPlayer(id).SetVibration(0, vibration.strenthLeft, vibration.durationLeft);
        if (vibration.vibrateRight)
            GetPlayer(id).SetVibration(1, vibration.strenthRight, vibration.durationRight);
    }

    #endregion

    #region unity fonction and ending

    /// <summary>
    /// a controller is connected
    /// </summary>
    /// <param name="args"></param>
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller was connected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        UpdatePlayerController(args.controllerId, true);

        EventManager.TriggerEvent(GameData.Event.GamePadConnectionChange, true, args.controllerId);
    }

    /// <summary>
    /// a controller is disconnected
    /// </summary>
    void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        UpdatePlayerController(args.controllerId, false);
        SetKeyboardForPlayerOne();

        EventManager.TriggerEvent(GameData.Event.GamePadConnectionChange, false, args.controllerId);
    }

    /// <summary>
    /// return true if any button is down from gamePAd or keyboard
    /// </summary>
    /// <returns></returns>
    private bool IsAnyButtonGamePadPressed()
    {
        for (int i = 0; i < playerNumber; i++)
        {
            for (int j = 0; j < allControlGamePad.Count; j++)
            {
                if (playersRewired[i].GetButtonUp(allControlGamePad[j]))
                    return (true);
            }
        }
        return (false);
    }
    private bool IsAnyButtonKeyBoardPressed()
    {
        for (int i = 0; i < GameData.keys.Length; i++)
        {
            KeyCode keycode = new KeyCode();
            keycode = ExtEnum.GetEnumValueFromString(GameData.keys[i], keycode);
            if (Input.GetKeyUp(keycode))
                return (true);
        }
        return (false);
    }

    /// <summary>
    /// switch keyboard
    /// </summary>
    /// <param name="active"></param>
    private void ActiveKeyboardForPlayer(bool active)
    {
        for (int i = 0; i < playerNumber; i++)
        {
            playersRewired[i].controllers.Keyboard.enabled = active;
        }
        EventManager.TriggerEvent(GameData.Event.SwitchKeyBoardOrGamePad);
    }

    /// <summary>
    /// active mouse
    /// </summary>
    private void MouseAndKeayboardInput()
    {
        //si on clique sur n'importe quel touche, ou le clic gauche de la souris, activer la souris
        //dans les menus...
        if (Input.GetMouseButtonUp(0) || IsAnyButtonKeyBoardPressed())
        {
            SetActiveKeyBoard(true);
        }
        else if (IsAnyButtonGamePadPressed())
        {
            SetActiveKeyBoard(false);
        }
    }

    private void Update()
    {
        MouseAndKeayboardInput();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.SceneLoaded, Init);
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        ReInput.ControllerConnectedEvent -= OnControllerConnected;
        ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
    }
    #endregion
}
