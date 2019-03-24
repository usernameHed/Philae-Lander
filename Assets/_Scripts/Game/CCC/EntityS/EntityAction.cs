using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// InputPlayer Description
/// </summary>
[TypeInfoBox("Player input")]
public class EntityAction : MonoBehaviour
{
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private Transform mainReferenceObjectDirection = null;

    [FoldoutGroup("Debug"), Tooltip("input for moving player horizontally"), ReadOnly]
    public Vector2 moveInput = Vector2.zero;


    [FoldoutGroup("Debug"), Tooltip(""), ReadOnly]
    public bool Jump;
    [FoldoutGroup("Debug"), Tooltip(""), ReadOnly]
    public bool JumpUp;

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

    /// <summary>
    /// get the relative direction 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRelativeDirection(float xBoost = 1, float yBoost = 1)
    {
        Vector3 dirInput = GetDirInput();
        Vector3 relativeDirection = mainReferenceObjectDirection.right * dirInput.x * xBoost + mainReferenceObjectDirection.forward * dirInput.y * yBoost;
        return (relativeDirection);
    }

    public Vector3 GetMainReferenceForwardDirection()
    {
        Vector3 relativeDirection = mainReferenceObjectDirection.forward;
        return (relativeDirection);
    }
    public Vector3 GetMainReferenceUpDirection()
    {
        Vector3 relativeDirection = mainReferenceObjectDirection.up;
        return (relativeDirection);
    }


    public float GetMagnitudeInput()
    {
        return (Mathf.Clamp01(GetDirInput().magnitude));
    }

    public float GetMagnitudeInput(float clampMin = 0.2f, float clampMax = 1f)
    {
        return (Mathf.Clamp(GetDirInput().magnitude, clampMin, clampMax));
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
    /// get move input (keyboard or gamepad)
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMoveInput()
    {
        return (moveInput);
    }
}
