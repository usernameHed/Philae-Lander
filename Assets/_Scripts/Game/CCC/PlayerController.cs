using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class PlayerController : EntityController
{
    public enum PhysicType
    {
        BASE,
        PLANET_CENTER,
        DIRECTION_CENTER,
    }

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private Vibration deathVibration;

    [FoldoutGroup("Object"), Tooltip("ref script")]
    public PlayerInput playerInput;
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public PlayerJump playerJump;

    [FoldoutGroup("Debug", Order = 1), SerializeField, Tooltip("id player for input")]
    public int idPlayer = 0;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
    }

    private void Awake()
    {
        base.Init();
        PhilaeManager.Instance.InitPlayer(this);
    }

    

    /// <summary>
    /// called when the game is over: desactive player
    /// </summary>
    public void GameOver()
    {
        enabledScript = false;
        PlayerConnected.Instance.SetVibrationPlayer(idPlayer, deathVibration);
    }


    private void Update()
    {
        if (!enabledScript)
            return;
    }

    /// <summary>
    /// set state of player
    /// </summary>
    private void ChangeState()
    {
        if (moveState == MoveState.InAir && groundCheck.IsSafeGrounded())
        {
            playerJump.OnGrounded();
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
