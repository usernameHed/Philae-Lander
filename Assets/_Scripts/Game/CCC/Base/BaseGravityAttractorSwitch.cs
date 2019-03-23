using AiUnity.MultipleTags.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGravityAttractorSwitch : UniqueGravityAttractorSwitch
{
    /*
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float marginDotGA = 0.71f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float marginNormalJumpInGA = 0.3f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float marginNegativeJumpHit = -0.1f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float timeBeforeApplyForceDown = 0.4f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float timeBeforeActiveAllAttractorAfterJumpCalculation = 2f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public int maxGravityApplied = 3;

    [FoldoutGroup("GamePlay"), Tooltip("More you have, less they attract !"), SerializeField]
    public float ratioOtherDistance = 1.3f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float maxDistBasedOnHowManyTimeDefault = 3f;
    */

    //[FoldoutGroup("Object"), Tooltip(""), SerializeField]
    //protected Rigidbody rbEntity = null;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    protected EntityController entityController;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    protected BaseGravity baseGravity;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    protected GroundCheck groundCheck;
    /*
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    protected GravityAttractorLD.PointInfo pointInfo = new GravityAttractorLD.PointInfo();
    
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    protected GravityAttractorLD.PointInfo tmpLastPointInfo = new GravityAttractorLD.PointInfo();

    [FoldoutGroup("Debug"), SerializeField, Tooltip(""), ReadOnly]
    protected List<GravityAttractorLD> allGravityAttractor = new List<GravityAttractorLD>();


    protected Vector3 lastNormalJumpChoosen = Vector3.up;
    */
    protected Vector3 wantedDirGravityOnGround = Vector3.zero;

    public Vector3 GetWantedGravityOnGround() => wantedDirGravityOnGround;
    //private bool isGoingDown = false;

    /*
public void EnterInZone(GravityAttractorLD refGravityAttractor)
{
    if (!allGravityAttractor.Contains(refGravityAttractor))
        allGravityAttractor.Add(refGravityAttractor);
}

public void LeanInZone(GravityAttractorLD refGravityAttractor)
{
    allGravityAttractor.Remove(refGravityAttractor);
}


public bool IsTheSamePointInfo(GravityAttractorLD.PointInfo tmpInfo)
{
    //if (!tmpInfo.refGA && !pointInfo.refGA)
    //    return (true);
    if (!tmpInfo.refGA || !pointInfo.refGA)
        return (false);
    return (tmpInfo.refGA.GetInstanceID() == pointInfo.refGA.GetInstanceID());
}
*/

    /// <summary>
    /// gravity base apply on this attractor
    /// </summary>
    /// <returns></returns>
    public override float GetAirRatioGravity()
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return (1f);

        float normalRatio = pointInfo.gravityBaseRatio;
        if (normalRatio == 0)
        {
            Debug.LogWarning("0 !");
        }
        return (normalRatio);
    }

    public virtual void JustJumped()
    {

    }

    public override void SetLastDirJump(Vector3 dirNormalChoosen)
    {
        base.SetLastDirJump(dirNormalChoosen);
        //lastNormalJumpChoosen = dirNormalChoosen;
        wantedDirGravityOnGround = dirNormalChoosen;
    }

    public virtual void OnGrounded()
    {

    }

    /// <summary>
    /// calculate and set the gravity
    /// if justCalculate = true, do NOT set the gravity, but return it
    /// </summary>
    /// <param name="entityPosition"></param>
    /// <param name="justCalculate"></param>
    /// <returns></returns>
    protected override void CalculateGAGravity()
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
        {
            pointInfo.sphereGravity = groundCheck.GetDirLastNormal();
            wantedDirGravityOnGround = GetAirSphereGravity(rbEntity.position).sphereGravity;
        }
        else
        {
            pointInfo = GetAirSphereGravity(rbEntity.position);
            wantedDirGravityOnGround = lastNormalJumpChoosen;
        }
    }
}
