using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// InputPlayer Description
/// </summary>
[TypeInfoBox("Player input")]
public class PlayerInput : EntityAction
{
    [FoldoutGroup("Debug"), Tooltip("input for moving Camera horizontally"), ReadOnly]
    public Vector2 cameraInput;

    [FoldoutGroup("Debug"), Tooltip("input for moving Camera horizontally"), ReadOnly]
    public Vector2 mouseInput;

    [FoldoutGroup("Debug"), Tooltip("input for moving Camera horizontally"), ReadOnly]
    public bool focus;
    [FoldoutGroup("Debug"), Tooltip("input for moving Camera horizontally"), ReadOnly]
    public bool focusUp;

    [FoldoutGroup("Debug"), Tooltip("input for dash"), ReadOnly]
    public bool dash;
    [FoldoutGroup("Debug"), Tooltip("input for dash"), ReadOnly]
    public bool dashUp;

    [FoldoutGroup("Object"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    protected PlayerController playerController;
    public PlayerController PlayerController { get { return (playerController); } }
    

    private bool enableScript = true;

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
    }

    /// <summary>
    /// retourne si le joueur se déplace ou pas
    /// </summary>
    /// <returns></returns>
    public bool NotMovingCamera(float margin = 0)
    {
        if (GetCameraInput().magnitude <= margin)
            return (true);
        return (false);
    }

    /// <summary>
    /// tout les input du jeu, à chaque update
    /// </summary>
    private void GetInput()
    {
        if (enableScript)
        {
            //all axis
            moveInput = new Vector2(PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetAxis("Move Horizontal"),
                PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetAxis("Move Vertical"));

            //all button
            Jump = PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetButton("Jump");
            JumpUp = PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetButtonUp("Jump");
            focus = PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetButton("Focus");
            focusUp = PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetButtonUp("Focus");

            dash = PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetButton("Dash");
            dashUp = PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetButtonUp("Dash");

            mouseInput = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y"));
        }

        cameraInput = new Vector2(
            PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetAxis("Move Horizontal Right"),
            PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetAxis("Move Vertical Right"));
    }

    /// <summary>
    /// return the input of the camera (mouse or right stick of gamePad ?)
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCameraInput()
    {
        return (cameraInput);
    }

    private void Update()
    {
        GetInput();
    }

    private void GameOver()
    {
        enableScript = false;
        this.enabled = false;
    }

    private void OnDisable()
    {
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
    }
}
