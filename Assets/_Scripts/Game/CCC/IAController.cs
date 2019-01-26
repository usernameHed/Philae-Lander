using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class IAController : EntityController
{
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public IAInput iaInput;

    public enum State
    {
        WANDER,
        CHASE_PLAYER,
        GO_TO_WAYPOINT,
    }
    public bool IsWandering
    {
        get
        {
            return (interactionState == State.WANDER);
        }
    }
    public bool IsChasingPlayer
    {
        get
        {
            return (interactionState == State.CHASE_PLAYER);
        }
    }
    public bool IsGoingToWayPoint
    {
        get
        {
            return (interactionState == State.GO_TO_WAYPOINT);
        }
    }

    [FoldoutGroup("GamePlay"), Tooltip("movement speed when we are wandering"), SerializeField]
    private float speedMove = 1f;

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private State interactionState = State.WANDER;

    private void Awake()
    {
        base.Init();
    }

    private void Update()
    {
        if (!enabledScript)
            return;
    }

    private void FixedUpdate()
    {
        base.ChangeState(iaInput);
    }
}
