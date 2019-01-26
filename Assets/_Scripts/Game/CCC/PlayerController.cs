using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class PlayerController : EntityController
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private Vibration deathVibration;

    [FoldoutGroup("Object"), Tooltip("ref script")]
    public PlayerInput playerInput;

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

    private void FixedUpdate()
    {
        base.ChangeState(playerInput);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, GameOver);
    }
}
