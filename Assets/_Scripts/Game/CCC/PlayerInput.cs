using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// InputPlayer Description
/// </summary>
[TypeInfoBox("Player input")]
public class PlayerInput : MonoBehaviour
{
    [FoldoutGroup("Debug"), Tooltip("input for moving player horizontally"), ReadOnly]
    public Vector2 moveInput;

    [FoldoutGroup("Debug"), Tooltip("input for moving Camera horizontally"), ReadOnly]
    public Vector2 cameraInput;

    [FoldoutGroup("Debug"), Tooltip("input for moving Camera horizontally"), ReadOnly]
    public Vector2 mouseInput;


    [FoldoutGroup("Debug"), Tooltip(""), ReadOnly]
    public bool Jump;
    [FoldoutGroup("Debug"), Tooltip(""), ReadOnly]
    public bool JumpUp;


    [FoldoutGroup("Object"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    private PlayerController playerController;
    public PlayerController PlayerController { get { return (playerController); } }

    /// <summary>
    /// get la direction de l'input
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDirInput(bool digital = false)
    {
        float x = GetMoveInput().x;
        float y = GetMoveInput().y;

        if (digital)
        {
            x = (x > 0f) ? 1 : x;
            x = (x < 0f) ? -1 : x;
            y = (y > 0f) ? 1 : y;
            y = (y < 0f) ? -1 : y;
        }

        Vector2 dirInputPlayer = new Vector2(x, y);
        return (dirInputPlayer);
    }

    public float GetMagnitudeInput()
    {
        return (Mathf.Clamp01(GetDirInput().magnitude));
    }

    /// <summary>
    /// get simple input direction (no dependency on AutoCam)
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDirInputRelativeToPlayer()
    {
        Vector2 dirInput = GetDirInput();
        Vector3 relativeDirection = playerController.rb.transform.right * dirInput.x + playerController.rb.transform.forward * dirInput.y;
        Debug.DrawRay(transform.position, relativeDirection, Color.cyan, 3f);
        return (relativeDirection);
    }

    /// <summary>
    /// retourne si le joueur se déplace ou pas
    /// </summary>
    /// <returns></returns>
    public bool NotMoving(float margin = 0)
    {
        if (GetMoveInput().magnitude <= margin)
            return (true);
        return (false);
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

    /// <summary>
    /// get move input (keyboard or gamepad)
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMoveInput()
    {
        return (moveInput);
    }

    private void Update()
    {
        GetInput();
    }
}
