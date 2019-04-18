using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class AimFollowerController : MonoBehaviour
{
    [FoldoutGroup("Aim"), SerializeField]
    public float speedMove = 50f;

    [FoldoutGroup("Object"), SerializeField]
    public PlayerController playerController;
    [FoldoutGroup("Object"), SerializeField]
    public EntityAction _entityAction;
    [FoldoutGroup("Object"), SerializeField]
    public EntityRaycastForward _entityRaycastForward;
    [FoldoutGroup("Object"), SerializeField]
    public AimFollowerInput _aimFollowerInput;
    [FoldoutGroup("Object"), SerializeField]
    public Rigidbody rb;

    private void Start()
    {
        if (!playerController)
            playerController = PhilaeManager.Instance.playerControllerRef;
    }


    private void FixedUpdate()
    {
        _aimFollowerInput.SetDirectionAim();
        UnityMovement.MoveTowards_WithPhysics(rb, _entityAction.GetMainReferenceForwardDirection(), speedMove * Time.deltaTime);
    }
}
