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

    [FoldoutGroup("Object"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    protected PlayerController playerController;
    public PlayerController PlayerController { get { return (playerController); } }

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
        //all axis
        moveInput = new Vector2(PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetAxis("Move Horizontal"),
            PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetAxis("Move Vertical"));

        mouseInput = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y"));

        cameraInput = new Vector2(
            PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetAxis("Move Horizontal Right"),
            PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetAxis("Move Vertical Right"));

        //all button
        Jump = PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetButton("Jump");
        JumpUp = PlayerConnected.Instance.GetPlayer(playerController.idPlayer).GetButtonUp("Jump");
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
}
