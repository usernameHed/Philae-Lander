using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class PlayerController : SingletonMono<PlayerController>
{
    protected PlayerController() { } // guarantee this will be always a singleton only - can't use the constructor!

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
    public PlayerGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private GroundCheck groundCheck;
    //[FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    //private PlayerJump playerJump;
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

    private Vector2 direction;              //save of direction player
    //private Vector3 dirOrientedAllControl;  //save of GetDirOrientedInputForMultipleControl

    private bool enabledScript = true;      //tell if this script should be active or not
    private float oldDrag;
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
        oldDrag = rb.drag;
        enabledScript = true;               //active this script at start
    }

    public void ChangeMainPlanet(Rigidbody rb)
    {
        playerGravity.ChangeMainAttractObject(rb.transform);
    }

    /// <summary>
    /// called when the game is over: desactive player
    /// </summary>
    public void GameOver()
    {
        enabledScript = false;
        PlayerConnected.Instance.SetVibrationPlayer(idPlayer, deathVibration);
    }

    /*
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
    */

    private void Update()
    {
        if (!enabledScript)
            return;
    }

    public void SetDragRb(float dragg)
    {
        if (rb.drag != dragg)
            rb.drag = dragg;
    }

    public void ChangeState(MoveState stateToChange)
    {
        moveState = MoveState.InAir;
        rb.drag = 0;
    }

    /// <summary>
    /// set state of player
    /// </summary>
    private void ChangeState()
    {
        if (moveState == MoveState.InAir && groundCheck.IsSafeGrounded())
        {
            EventManager.TriggerEvent(GameData.Event.OnGrounded);
        }

        if (groundCheck.IsFlying()/* || playerJump.IsJumpedAndNotReady()*/)
        {
            //IN AIR
            moveState = MoveState.InAir;
            SetDragRb(0);
            return;
        }

        if (rb.drag != oldDrag/* && playerJump.IsJumpCoolDebugDownReady()*/)
            SetDragRb(oldDrag);



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
