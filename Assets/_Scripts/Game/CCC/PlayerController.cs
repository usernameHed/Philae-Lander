using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : SingletonMono<PlayerController>
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private Vibration deathVibration;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    public Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public PlayerInput playerInput;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Transform rotateObject;

    [FoldoutGroup("Debug", Order = 1), SerializeField, Tooltip("id player for input")]
    public int idPlayer = 0;

    private Vector2 direction;              //save of direction player
    //private Vector3 dirOrientedAllControl;  //save of GetDirOrientedInputForMultipleControl

    private bool enabledScript = true;      //tell if this script should be active or not

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
    }

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// init player
    /// </summary>
    public void Init()
    {
        enabledScript = true;               //active this script at start
    }

    /// <summary>
    /// called when the game is over: desactive player
    /// </summary>
    public void GameOver()
    {
        enabledScript = false;
        PlayerConnected.Instance.SetVibrationPlayer(idPlayer, deathVibration);
    }

    /// <summary>
    /// input called every update
    /// </summary>
    private void InputPlayer()
    {
        //direction = new Vector2(playerInput.GetMoveInput().x * 1, playerInput.GetMoveInput().y * 1);
        //dirOrientedAllControl = playerInput.GetDirOrientedInputForMultipleControl();
    }

    private void Update()
    {
        if (!enabledScript)
            return;

        InputPlayer();      //input player
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, GameOver);
    }
}
