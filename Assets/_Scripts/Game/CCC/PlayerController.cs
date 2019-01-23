using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingletonMono<PlayerController>
{
    public enum MoveState
    {
        Idle,
        InAir,
        Move,
    }

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private Vibration deathVibration;


    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    public Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public PlayerInput playerInput;
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public GravityApplyer gravityApplyer;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private PlayerJump playerJump;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Transform rotateObject;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("state move"), ReadOnly]
    private MoveState moveState = MoveState.Idle;
    public MoveState GetMoveState()
    {
        return moveState;
    }
    [FoldoutGroup("Debug", Order = 1), SerializeField, Tooltip("id player for input")]
    public int idPlayer = 0;
    [FoldoutGroup("Debug"), Tooltip("ref rigidbody"), SerializeField, ReadOnly]
    private Rigidbody currentPlanet;

    private Vector2 direction;              //save of direction player
    //private Vector3 dirOrientedAllControl;  //save of GetDirOrientedInputForMultipleControl

    private bool enabledScript = true;      //tell if this script should be active or not

    private bool planetSwitcher = false;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
    }

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// init player
    /// </summary>
    public void Init()
    {
        enabledScript = true;               //active this script at start
        currentPlanet = PlanetSwitcher.Instance.GetClosestRigidBody(rb.gameObject);

        gravityApplyer.SetPlanetList(currentPlanet);
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

    private void TryToChangePlanet()
    {
        if (groundCheck.IsFlying() && !planetSwitcher)
        {
            Rigidbody newPlanet = PlanetSwitcher.Instance.GetClosestRigidBody(rb.gameObject);
            if (newPlanet.GetInstanceID() != currentPlanet.GetInstanceID())
            {
                Debug.Log("Planet change !");
                currentPlanet = newPlanet;
                planetSwitcher = true;
                gravityApplyer.SetPlanetList(currentPlanet);
                //gravityApplyer.SetRatioGravity(0);
            }
        }
        else if (!groundCheck.IsFlying())
        {
            planetSwitcher = false;
        }
    }

    private void Update()
    {
        if (!enabledScript)
            return;

        InputPlayer();      //input player
        //TryToChangePlanet();
    }

    private void ChangeState()
    {
        if (moveState == MoveState.InAir && groundCheck.IsSafeGrounded())
        {
            playerJump.OnGrounded();
        }

        if (groundCheck.IsFlying())
        {
            moveState = MoveState.InAir;
            return;
        }

        if (!playerInput.NotMoving())
        {
            moveState = MoveState.Move;
        }
        else
        {
            moveState = MoveState.Idle;
        }
    }

    private void FixedUpdate()
    {
        ChangeState();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, GameOver);
    }
}
