
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;
using UnityEssentials.PropertyAttribute.readOnly;

public class EntityController : MonoBehaviour, IKillable
{
    public enum MoveState
    {
        Idle,
        InAir,
        Move,
    }

    [SerializeField, Tooltip("ref rigidbody")]
    public bool isPlayer = false;
    
    [Tooltip(""), SerializeField]
    public string[] allWalkablePlatform = new string[] { "Walkable/Ground", "Walkable/Stick", "Walkable/Dont", "Walkable/FastForward" };

    [Tooltip(""), SerializeField]
    public string[] marioGalaxyPlatform = new string[] { "Walkable/Ground" };

    [Tooltip(""), SerializeField]
    public string[] stickPlatform = new string[] { "Walkable/Stick" };

    [Tooltip(""), SerializeField]
    public string[] walkForbiddenForwardUp = new string[] { "Walkable/Dont" };

    [Tooltip(""), SerializeField]
    public string[] fastForwardPlatform = new string[] { "Walkable/FastForward" };

    [SerializeField, Tooltip("ref rigidbody")]
    public Rigidbody rb;
    [SerializeField, Tooltip("ref rigidbody")]
    public Transform rbRotateObject;

    [Tooltip("ref script"), SerializeField]
    public BaseGravity baseGravity;
    [SerializeField, Tooltip("ref")]
    protected GroundCheck groundCheck;
    [Tooltip("ref script"), SerializeField]
    protected EntityAction entityAction;
    [Tooltip("ref script"), SerializeField]
    protected BaseGravityAttractorSwitch baseGravityAttractorSwitch;
    [Tooltip("ref script"), SerializeField]
    protected EntityNoGravity entityNoGravity;
    [Tooltip("ref script"), SerializeField]
    protected EntityBumpUp entityBumpUp;
    [Tooltip("rigidbody"), SerializeField]
    protected EntityAirMove entityAirMove;

    [Tooltip("rigidbody"), SerializeField]
    protected FastForward fastForward;
    [Tooltip("rigidbody"), SerializeField]
    protected ClampRbSpeed clampRbSpeed;

    [SerializeField, Tooltip("state move"), ReadOnly]
    protected MoveState moveState = MoveState.Idle;
    public MoveState GetMoveState()
    {
        return moveState;
    }

    [HideInInspector]
    public int layerMask = Physics.AllLayers;
    protected Vector2 direction;              //save of direction player
    //private Vector3 dirOrientedAllControl;  //save of GetDirOrientedInputForMultipleControl
    protected float oldDrag;
    protected bool planetSwitcher = false;
    protected bool isKilled = false;
    
    protected float actualAcceleration = 0f;
    public float GetActualAcceleration() => actualAcceleration;
    protected Vector3 actualAccelerationVector = Vector3.zero;
    
    public float GetActualVelocity() => clampRbSpeed.GetActualVelocity();
    public Vector3 GetActualVelocityVector() => rb.velocity;
    public Vector3 GetActualAccelerationDir() => actualAccelerationVector;
    //here get the acceleration forward depending on the gravity
    public Vector3 GetActualDirForward()
    {
        Quaternion forwardDir = ExtRotation.TurretLookRotation(GetActualVelocityVector(), baseGravity.GetMainAndOnlyGravity());
        return (forwardDir * Vector3.forward);
    }
    public Vector3 GetActualAccelerationForward()
    {
        Vector3 dirForward = GetActualDirForward();
        Vector3 dirVelocity = rb.velocity;

        Vector3 projected = ExtVector3.GetProjectionOfAOnB(dirVelocity, dirForward, baseGravity.GetMainAndOnlyGravity());

        //Debug.DrawRay(rb.transform.position, dirForward, Color.blue);
        //Debug.DrawRay(rb.transform.position, dirVelocity, Color.red);
        //Debug.DrawRay(rb.transform.position, projected, Color.green);
        return (projected);
    }

    public int GetLayerMastAllWalkable()
    {
        layerMask = LayerMask.GetMask(allWalkablePlatform);
        return (layerMask);
    }


    /// <summary>
    /// init player
    /// </summary>
    public void Init()
    {
        isKilled = false;
        layerMask = LayerMask.GetMask(allWalkablePlatform);
        oldDrag = rb.drag;
    }

    

    public bool IsWalkablePlatform(string layer)
    {
        int isForbidden = ExtList.ContainSubStringInArray(allWalkablePlatform, layer);
        if (isForbidden != -1)
            return (true);
        return (false);
    }
    public bool IsMarioGalaxyPlatform(string layer)
    {
        int isForbidden = ExtList.ContainSubStringInArray(marioGalaxyPlatform, layer);
        if (isForbidden != -1)
            return (true);
        return (false);
    }
    public bool IsStickPlatform(string layer)
    {
        int isForbidden = ExtList.ContainSubStringInArray(stickPlatform, layer);
        if (isForbidden != -1)
            return (true);
        return (false);
    }
    public bool IsForbidenLayerSwitch(string layer)
    {
        int isForbidden = ExtList.ContainSubStringInArray(walkForbiddenForwardUp, layer);
        if (isForbidden != -1)
        {
            //here we are in front of a forbidden wall !!
            return (true);
        }
        return (false);
    }
    public bool IsFastForwardLayer(int layer)
    {
        int isFastForward = ExtList.ContainSubStringInArray(fastForwardPlatform, LayerMask.LayerToName(layer));
        if (isFastForward != -1)
            return (true);
        return (false);
    }

    /// <summary>
    /// return the forward dir we want for the player
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFocusedForwardDirPlayer()
    {
        Vector3 realNormal = baseGravity.GetMainAndOnlyGravity();
        Vector3 forwardNormal = -Vector3.Cross(realNormal, rbRotateObject.transform.right);
        return (forwardNormal);
    }

    /// <summary>
    /// return the forward dir we want for the player
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFocusedForwardDirPlayer(Vector3 realNormalGravity)
    {
        Vector3 realNormal = realNormalGravity;
        Vector3 forwardNormal = -Vector3.Cross(realNormal, rbRotateObject.transform.right);
        return (forwardNormal);
    }

    public bool IsLookingTowardTheInput(float marginDot = 0.3f)
    {
        Vector3 relativeInput = entityAction.GetRelativeDirection().normalized;
        Vector3 forwardFocusPlayer = GetFocusedForwardDirPlayer();
        //Debug.DrawRay(rb.position, relativeInput, Color.yellow);
        //Debug.DrawRay(rb.position, forwardFocusPlayer, Color.blue);

        float dotDiffPlayerInput = Vector3.Dot(relativeInput, forwardFocusPlayer);
        if (1 - dotDiffPlayerInput < marginDot)
            return (true);
        return (false);
    }

    /// <summary>
    /// return the forward dir we want for the player
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFocusedRightDirPlayer()
    {
        Vector3 realNormal = baseGravity.GetMainAndOnlyGravity();
        Vector3 rightNormal = -Vector3.Cross(realNormal, rbRotateObject.transform.forward);
        return (rightNormal);
        //return (rbRotateObject.transform.forward);
    }

    public void SetKinematic(bool isKinematc)
    {
        rb.isKinematic = isKinematc;
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

    protected virtual void OnGrounded()
    {
        baseGravity.OnGrounded();
        baseGravityAttractorSwitch.OnGrounded();
    }

    /// <summary>
    /// set state of player
    /// </summary>
    protected void ChangeState()
    {
        if (moveState == MoveState.InAir && groundCheck.IsSafeGrounded())
        {
            OnGrounded();
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


        if (entityAction.IsMoving())
        {
            moveState = MoveState.Move;
        }
        else
        {
            moveState = MoveState.Idle;
        }
    }

    public virtual void Kill()
    {
        
    }

    public virtual void GetHit(int amount, Vector3 posAttacker)
    {
        
    }
}
