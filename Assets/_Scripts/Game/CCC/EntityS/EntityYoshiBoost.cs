
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

public class EntityYoshiBoost : MonoBehaviour
{
    [Tooltip("vibration quand on jump"), SerializeField]
    private bool enableBoost = true;

    [Tooltip("vibration quand on jump"), SerializeField]
    private float timeBeforeJump = 0.4f;
    [Tooltip("vibration quand on jump"), SerializeField]
    private float boostTime = 1.5f;
    [Tooltip("vibration quand on jump"), SerializeField]
    private float timeBefore2Jump = 2f;
    [Space(10)]
    [Tooltip("vibration quand on jump"), SerializeField]
    private float speedMax = 4f;
    [Tooltip("vibration quand on jump"), SerializeField]
    private float speedMinDownWhenStart = 2f;



    [Tooltip("vibration quand on jump"), SerializeField]
    private float yoshiBoostGravity = 4f;
    public float GetYoshiBoost() => yoshiBoostGravity;

    [SerializeField, Tooltip("ref script")]
    private Rigidbody rbEntity = default;
    [SerializeField, Tooltip("ref script")]
    private EntityJump entityJump = default;
    [SerializeField, Tooltip("ref script")]
    private EntityAction entityAction = default;
    [SerializeField, Tooltip("ref script")]
    private PlayerController playerController = default;
    [SerializeField, Tooltip("ref script")]
    private EntityGravity entityGravity = default;
    [SerializeField, Tooltip("ref script")]
    private ClampRbSpeed clampRbSpeed = default;

    [SerializeField, Tooltip("ref script"), ReadOnly]
    private bool isBoosting = false;
    public bool AreWeBoosting() => isBoosting;

    private FrequencyCoolDown coolDownAfterJump = new FrequencyCoolDown();
    private FrequencyChrono chronoBoost = new FrequencyChrono();

    public bool CanDoBoost()
    {
        //we need to be inAIr !
        if (playerController.GetMoveState() != EntityController.MoveState.InAir)
            return (false);

        //if we just jump, dont
        if (coolDownAfterJump.IsRunning())
            return (false);

        //we need to release the jump button after jump before apply boost
        if (entityJump.IsJumpStoped())
            return (false);

        //we can active yoshi move only when going down
        if (!isBoosting && !entityGravity.IsGoingDown())
            return (false);

        return (true);
    }

    public void JustJumped()
    {
        coolDownAfterJump.StartCoolDown(timeBeforeJump);
    }

    public void OnGrounded()
    {
        coolDownAfterJump.Reset();
    }

    private void TryToBoost()
    {
        if (CanDoBoost() && entityAction.Jump)
            DoBoost();
        else if (isBoosting == true)
        {
            StopBoost();
        }
    }

    private void StopBoost()
    {
        isBoosting = false;
        Debug.Log("boosting over");
        coolDownAfterJump.StartCoolDown(timeBefore2Jump);
    }

    private void DoBoost()
    {
        if (!isBoosting)
        {
            Debug.Log("first time boost !");
            chronoBoost.StartCoolDown();
            isBoosting = true;
            clampRbSpeed.ReduceDecendingSpeedToAMin(speedMinDownWhenStart);
        }
        if (chronoBoost.GetTimer() > boostTime)
        {
            StopBoost();
            return;
        }

        clampRbSpeed.DoClamp(speedMax);
        ObjectsPooler.Instance.SpawnFromPool(GameData.PoolTag.Jump, rbEntity.position, rbEntity.rotation, ObjectsPooler.Instance.transform);
        //SoundManager.Instance.PlaySound(playerController.SFX_Boost);
    }

    private void FixedUpdate()
    {
        if (enableBoost)
            TryToBoost();
    }
}
