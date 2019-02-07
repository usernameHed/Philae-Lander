﻿using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// InputPlayer Description
/// </summary>
[TypeInfoBox("Player input")]
public class EntityAction : MonoBehaviour
{
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private Transform mainReferenceObjectDirection;

    [FoldoutGroup("Debug"), Tooltip("input for moving player horizontally"), ReadOnly]
    public Vector2 moveInput;


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
    public Vector3 GetRelativeDirection()
    {
        Vector3 dirInput = GetDirInput();
        Vector3 relativeDirection = mainReferenceObjectDirection.right * dirInput.x + mainReferenceObjectDirection.forward * dirInput.y;
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
