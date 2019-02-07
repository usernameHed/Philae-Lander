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

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    public bool isPlayer = false;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] walkablePlatform = new string[] { "Walkable/Floor" };

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    public Rigidbody rb;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    public Transform rbRotateObject;

    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    public PlayerGravity playerGravity;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref")]
    protected GroundCheck groundCheck;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    protected EntityAttractor entityAttractor;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    protected EntitySwitch entitySwitch;
    [FoldoutGroup("Object"), Tooltip("ref script"), SerializeField]
    protected EntityAction entityAction;

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

    protected bool enabledScript = true;      //tell if this script should be active or not
    protected float oldDrag;
    protected bool planetSwitcher = false;

    /// <summary>
    /// init player
    /// </summary>
    public void Init()
    {
        layerMask = LayerMask.GetMask(walkablePlatform);
        oldDrag = rb.drag;
        enabledScript = true;               //active this script at start
    }

    /// <summary>
    /// return the forward dir we want for the player
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFocusedForwardDirPlayer()
    {
        Vector3 realNormal = playerGravity.GetMainAndOnlyGravity();
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
        Vector3 realNormal = playerGravity.GetMainAndOnlyGravity();
        Vector3 rightNormal = -ExtQuaternion.CrossProduct(realNormal, rbRotateObject.transform.forward);
        return (rightNormal);
        //return (rbRotateObject.transform.forward);
    }

    public void SetKinematic(bool isKinematc)
    {
        rb.isKinematic = isKinematc;
    }

    /*
    public void ChangeMainPlanet(Rigidbody rbOther)
    {
        Debug.LogWarning("no managed anymore!");
        playerGravity.ChangeMainAttractObject(rbOther.transform);//, rbOther.transform.position, (rb.transform.position - rbOther.transform.position).normalized);
    }
    */

    
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
}
