using UnityEngine;
using UnityEssentials.Extensions;
using UnityEssentials.PropertyAttribute.readOnly;

/// <summary>
/// InputPlayer Description
/// </summary>
public class EntityAction : MonoBehaviour
{
    [SerializeField, Tooltip("ref script")]
    protected Transform mainReferenceObjectDirection = null;

    [Tooltip("input for moving player horizontally"), ReadOnly]
    public Vector2 moveInput = Vector2.zero;


    [Tooltip(""), ReadOnly]
    public bool Jump;
    [Tooltip(""), ReadOnly]
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

    /// <summary>
    /// get the relative direction 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRelativeDirectionWithNoDamping(Vector3 mainGravity, Vector3 debugPos, float xBoost = 1, float yBoost = 1)
    {
        
        //Vector3 noDampingForward = -Vector3.Cross(mainGravity, mainReferenceObjectDirection.right);
        Vector3 relativeDirection = GetRelativeDirection();
        Vector3 noDampingInputDir = ExtRotation.TurretLookRotationVector(relativeDirection, mainGravity);


        Debug.DrawRay(debugPos, relativeDirection * 10, Color.black);
        Debug.DrawRay(debugPos, mainGravity * 10, Color.red);
        Debug.DrawRay(debugPos, noDampingInputDir * 10, Color.green);

        return (noDampingInputDir);
    }

    /// <summary>
    /// get the relative direction 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRelativeDirectionRight(float xBoost = 1, float yBoost = 1)
    {
        Vector3 dirDirection = GetRelativeDirection(xBoost, yBoost);
        Vector3 up = mainReferenceObjectDirection.up;

        Vector3 relativeRightDirection = Vector3.Cross(dirDirection, up);
        return (relativeRightDirection);
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
    public bool IsMoving(float margin = 0)
    {
        if (GetMoveInput().magnitude <= margin)
            return (false);
        return (true);
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
