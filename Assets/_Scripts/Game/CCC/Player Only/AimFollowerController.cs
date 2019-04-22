using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class AimFollowerController : MonoBehaviour
{
    [FoldoutGroup("Aim"), SerializeField]
    public FrequencyEase easeAccelerate = new FrequencyEase();
    //[FoldoutGroup("Aim"), SerializeField]
    //public FrequencyEase easeDecelerate = new FrequencyEase();
    [FoldoutGroup("Aim"), SerializeField]
    public float speedUpAndDown = 300f;

    [FoldoutGroup("Misc"), SerializeField]
    public float marginNotMoving = 0.3f;
    [FoldoutGroup("Misc"), SerializeField]
    public float factorCloseToBeGrounded = 0.6f;
    [FoldoutGroup("Misc"), SerializeField]
    public float marginDotUpAndDown = 0.1f;

    [FoldoutGroup("Object"), SerializeField]
    public PlayerController playerController;
    [FoldoutGroup("Object"), SerializeField]
    public EntityAction _entityAction;
    [FoldoutGroup("Object"), SerializeField]
    public UniqueGravityAttractorSwitch _playerUniqueGravityAttractor;
    [FoldoutGroup("Object"), SerializeField]
    public EntityRaycastForward _entityRaycastForward;

    [FoldoutGroup("Object"), SerializeField]
    public Rigidbody rbPlanetOriented;
    [FoldoutGroup("Object"), SerializeField]
    public Transform toRotateTowardAim;

    private float currentSpeed = 0f;
    private float currentVelocity = 0f;

    private void Start()
    {
        if (!playerController)
            playerController = PhilaeManager.Instance.playerControllerRef;
    }

    private void FixedUpdate()
    {
        if (!_entityAction.IsMoving(marginNotMoving))
        {
            //ease off
            easeAccelerate.BackToTime();
            currentSpeed = easeAccelerate.EvaluateWithoutDeltaTime();
            return;
        }

        //rotate to gravity player
        GravityAttractorLD.PointInfo point = ExtGetGravityAtPoints.GetAirSphereGravityStatic(rbPlanetOriented.transform.position, _playerUniqueGravityAttractor.allGravityAttractor);
        RotateToGround.InstantRotateObject(rbPlanetOriented.transform.position - point.posRange, rbPlanetOriented.transform);

        //aim at player
        Vector3 toAim = _entityRaycastForward.GetLastPos() - rbPlanetOriented.transform.position;
        toRotateTowardAim.rotation = ExtQuaternion.TurretLookRotation(toAim, rbPlanetOriented.transform.up);

        //ease on
        easeAccelerate.StartOrContinue();
        currentSpeed = easeAccelerate.EvaluateWithoutDeltaTime();


        Vector3 finalDir = toRotateTowardAim.forward * currentSpeed;

        Vector3 forward = toRotateTowardAim.forward;
        Vector3 up = toRotateTowardAim.up;
        Vector3 toGoal = (_entityRaycastForward.GetLastPos() - rbPlanetOriented.transform.position).FastNormalized();

        Debug.DrawRay(toRotateTowardAim.position, forward, Color.blue);
        Debug.DrawRay(toRotateTowardAim.position, toGoal, Color.red);
        Debug.DrawRay(toRotateTowardAim.position, up, Color.yellow);
        float dot = ExtQuaternion.DotProduct(toGoal, up);
        if (dot > marginDotUpAndDown)
        {
            finalDir += toRotateTowardAim.up * speedUpAndDown * dot;
        }
        else if (dot < -marginDotUpAndDown)
        {
            
            //here try to go down
            bool grounded = ExtGameObject.IsGrounded(rbPlanetOriented.gameObject, up, factorCloseToBeGrounded, playerController.layerMask, QueryTriggerInteraction.Ignore);
            if (!grounded)
            {
                finalDir += -toRotateTowardAim.up * speedUpAndDown * Mathf.Abs(dot);
            }
        }

        //apply movement
        rbPlanetOriented.MovePosition(rbPlanetOriented.position + playerController.rb.velocity.magnitude * finalDir * Time.deltaTime);
    }
}
