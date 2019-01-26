using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class EntityController : MonoBehaviour
{
    public enum MoveState
    {
        Idle,
        InAir,
        Move,
    }

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    protected Rigidbody rb;

    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    protected PlayerGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    protected GroundCheck groundCheck;
    //[FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    //private PlayerJump playerJump;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    protected Transform rotateObject;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("state move"), ReadOnly]
    protected MoveState moveState = MoveState.Idle;
    public MoveState GetMoveState()
    {
        return moveState;
    }

    protected Vector2 direction;              //save of direction player
    //private Vector3 dirOrientedAllControl;  //save of GetDirOrientedInputForMultipleControl

    protected bool enabledScript = true;      //tell if this script should be active or not
    protected float oldDrag;
    protected bool planetSwitcher = false;

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
        Debug.Log("playerGravity: " + playerGravity);
        playerGravity.ChangeMainAttractObject(rb.transform);
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
    protected void ChangeState(EntityAction entityAction)
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


        if (!entityAction.NotMoving())
        {
            moveState = MoveState.Move;
        }
        else
        {
            moveState = MoveState.Idle;
        }
    }
}
