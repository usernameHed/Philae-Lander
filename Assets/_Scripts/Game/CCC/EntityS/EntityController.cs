using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class EntityController : MonoBehaviour, IKillable
{
    public enum MoveState
    {
        Idle,
        InAir,
        Move,
    }

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    public bool isPlayer = false;
    
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] allWalkablePlatform = new string[] { "Walkable/Ground", "Walkable/Stick", "Walkable/Dont", "Walkable/FastForward" };

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] marioGalaxyPlatform = new string[] { "Walkable/Ground" };

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] stickPlatform = new string[] { "Walkable/Stick" };

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] walkForbiddenForwardUp = new string[] { "Walkable/Dont" };

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] fastForwardPlatform = new string[] { "Walkable/FastForward" };

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    public Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    public Transform rbRotateObject;

    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    public BaseGravity baseGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    protected GroundCheck groundCheck;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    protected EntityAction entityAction;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    protected BaseGravityAttractorSwitch baseGravityAttractorSwitch;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    protected EntityNoGravity entityNoGravity;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    protected EntityBumpUp entityBumpUp;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected EntityAirMove entityAirMove;

    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected FastForward fastForward;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected ClampRbSpeed clampRbSpeed;

    [FoldoutGroup("Sound"), SerializeField, Tooltip("ref script")]
    public FmodEventEmitter SFX_jump;
    [FoldoutGroup("Sound"), SerializeField, Tooltip("ref script")]
    public FmodEventEmitter SFX_grounded;
    [FoldoutGroup("Sound"), SerializeField, Tooltip("ref script")]
    public FmodEventEmitter SFX_Scream;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("state move"), ReadOnly]
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
        Quaternion forwardDir = ExtQuaternion.TurretLookRotation(GetActualVelocityVector(), baseGravity.GetMainAndOnlyGravity());
        return (forwardDir * Vector3.forward);
    }
    public Vector3 GetActualAccelerationForward()
    {
        Vector3 dirForward = GetActualDirForward();
        Vector3 dirVelocity = rb.velocity;

        Vector3 projected = ExtQuaternion.GetProjectionOfAOnB(dirVelocity, dirForward, baseGravity.GetMainAndOnlyGravity());

        //Debug.DrawRay(rb.transform.position, dirForward, Color.blue);
        //Debug.DrawRay(rb.transform.position, dirVelocity, Color.red);
        //Debug.DrawRay(rb.transform.position, projected, Color.green);
        return (projected);
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
        Vector3 forwardNormal = -ExtQuaternion.CrossProduct(realNormal, rbRotateObject.transform.right);
        return (forwardNormal);
    }

    /// <summary>
    /// return the forward dir we want for the player
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFocusedForwardDirPlayer(Vector3 realNormalGravity)
    {
        Vector3 realNormal = realNormalGravity;
        Vector3 forwardNormal = -ExtQuaternion.CrossProduct(realNormal, rbRotateObject.transform.right);
        return (forwardNormal);
    }

    public bool IsLookingTowardTheInput(float marginDot = 0.3f)
    {
        Vector3 relativeInput = entityAction.GetRelativeDirection().normalized;
        Vector3 forwardFocusPlayer = GetFocusedForwardDirPlayer();
        //Debug.DrawRay(rb.position, relativeInput, Color.yellow);
        //Debug.DrawRay(rb.position, forwardFocusPlayer, Color.blue);

        float dotDiffPlayerInput = ExtQuaternion.DotProduct(relativeInput, forwardFocusPlayer);
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
        Vector3 rightNormal = -ExtQuaternion.CrossProduct(realNormal, rbRotateObject.transform.forward);
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


        if (!entityAction.NotMoving())
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
