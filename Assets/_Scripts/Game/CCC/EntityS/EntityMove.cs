using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Player Move forward locally")]
public class EntityMove : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private float speedMove = 5f;
    [FoldoutGroup("GamePlay"), Tooltip("speed move forward"), SerializeField]
    private AnimationCurve easeAcceleration;
    [FoldoutGroup("GamePlay"), Tooltip("base speed"), SerializeField]
    private float minAcceleration = 10f;

    [FoldoutGroup("GamePlay"), Range(0f, 1f), Tooltip("when we have a little input, set it to this value"), SerializeField]
    private float minInput = 0.15f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private Rigidbody rb = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityController entityController = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityAction entityAction = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntityJump entityJump = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private GroundForwardCheck groundForwardCheck = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    private EntitySlide entitySlide = null;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref")]
    private float currentSpeedMove = 0f;

    private void OnEnable()
    {
        currentSpeedMove = minAcceleration;
    }

    /// <summary>
    /// return true if input is going forward for a relativly long time
    /// </summary>
    /// <returns></returns>
    public bool IsInputRelativeFullSpeed(float ratio = 0.5f)
    {
        //if current speed is more than 50% of the speed, then we are at "full speed"
        if (currentSpeedMove > speedMove * ratio)
        {
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// return a value from 0 to 1, representing the "player input", with rb acceleration
    /// </summary>
    /// <returns></returns>
    public float GetMagnitudeAcceleration()
    {
        float playerInput = entityAction.GetMagnitudeInput();

        //TODO: a super lerp;
        //min: minAcceleration
        //max: speedMove
        //current: currentSpeedMove
        float remapCurrentSpeed = ExtUtilityFunction.Remap(currentSpeedMove, minAcceleration, speedMove, 0, 1);


        return (remapCurrentSpeed * playerInput);
    }

    public float GetCurrentSpeedClamped01()
    {
        float currentVelocity = entityController.GetActualVelocity();
        float velocityRemapped = ExtUtilityFunction.Remap(currentVelocity, 0, 10, 0, 1);

        //Debug.Log("velocity remapped: " + velocityRemapped + "(actual: " + currentVelocity + ")");
        return (Mathf.Clamp01(velocityRemapped));
    }
    public float GetCurrentSpeedForwardClamped01()
    {
        float currentVelocity = entityController.GetActualAccelerationForward().magnitude;
        float velocityRemapped = ExtUtilityFunction.Remap(currentVelocity, 0, 10, 0, 1);

        //Debug.Log("velocity forward not mapped: " + currentVelocity);
        return (Mathf.Clamp01(velocityRemapped));
    }

    private void ChangeLerp()
    {
        if (entityAction.NotMoving())
        {
            currentSpeedMove = Mathf.Lerp(currentSpeedMove, minAcceleration, easeAcceleration.Evaluate(Time.deltaTime));
        }
        else
        {
            currentSpeedMove = Mathf.Lerp(currentSpeedMove, speedMove, easeAcceleration.Evaluate(Time.deltaTime));
        }

        
    }

    /// <summary>
    /// move with input
    /// </summary>
    /// <param name="direction"></param>
    public void MovePhysics(Vector3 direction)
    {
       
        UnityMovement.MoveByForcePushing_WithPhysics(rb, direction, currentSpeedMove * entityAction.GetMagnitudeInput(minInput, 1f));
    }

    /// <summary>
    /// move in physics, according to input of player
    /// </summary>
    private void MovePlayer()
    {
        Vector3 dirMove = Vector3.zero;

        if (groundForwardCheck && groundForwardCheck.IsForwardForbiddenWall())
        {
            //Debug.Log("move Straff");
            dirMove = entitySlide.GetStraffDirection();
        }
        else
        {
            //Debug.Log("move forward");
            dirMove = entityController.GetFocusedForwardDirPlayer();
        }
        //Debug.DrawRay(rb.position, dirMove * 5, Color.blue);
        MovePhysics(dirMove);
    }

    /// <summary>
    /// handle move physics
    /// </summary>
    private void FixedUpdate()
    {
        ChangeLerp();

        //GetCurrentSpeedClamped01();

        if (entityController.GetMoveState() == EntityController.MoveState.Move
            && entityJump.IsJumpCoolDebugDownReady())
        {
            MovePlayer();
        }
    }

    
}
