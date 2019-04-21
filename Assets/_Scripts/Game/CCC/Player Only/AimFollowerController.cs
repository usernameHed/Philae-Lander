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
    public UniqueGravityAttractorSwitch _playerUniqueGravityAttractor;
    [FoldoutGroup("Object"), SerializeField]
    public EntityRaycastForward _entityRaycastForward;

    [FoldoutGroup("Object"), SerializeField]
    public Rigidbody rbPlanetOriented;
    [FoldoutGroup("Object"), SerializeField]
    public Transform toRotateTowardAim;

    private void Start()
    {
        if (!playerController)
            playerController = PhilaeManager.Instance.playerControllerRef;
    }

    private void FixedUpdate()
    {
        //rotate to gravity player
        GravityAttractorLD.PointInfo point = ExtGetGravityAtPoints.GetAirSphereGravityStatic(rbPlanetOriented.transform.position, _playerUniqueGravityAttractor.allGravityAttractor);
        RotateToGround.InstantRotateObject(rbPlanetOriented.transform.position - point.posRange, rbPlanetOriented.transform);

        Vector3 toAim = _entityRaycastForward.GetLastPos() - rbPlanetOriented.transform.position;
        toRotateTowardAim.rotation = ExtQuaternion.TurretLookRotation(toAim, rbPlanetOriented.transform.up);

        //DoRotate(GetLastDesiredRotation(entityAction.GetRelativeDirection(), objectToRotate.up), turnRate);
        UnityMovement.MoveTowards_WithPhysics(rbPlanetOriented, toRotateTowardAim.forward, speedMove);
    }
}
