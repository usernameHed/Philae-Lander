﻿using AiUnity.MultipleTags.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGravityAttractorSwitch : BaseGravityAttractorSwitch
{
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private float timeBeforeApplyForceDown = 0.4f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private float timeBeforeActiveAllAttractorAfterJumpCalculation = 2f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private float marginNegativeJumpHit = -0.1f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private bool calculateGroundPos = true;

    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityJumpCalculation entityJumpCalculation = default;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private FastForward fastForward = default;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityJump entityJump = default;


    private FrequencyCoolDown coolDownBeforeApplyForceDown = new FrequencyCoolDown();
    private FrequencyCoolDown coolDownBeforeAttract = new FrequencyCoolDown();


    private bool applyGalaxyForce = false;


    
    /// <summary>
    /// can we apply gravity force ?
    /// if we are going up (from jump), we can't
    /// if the timer is still running, we can't neiter
    /// </summary>
    /// <returns></returns>
    public override bool CanApplyGravityForce()
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return (true);

        if (fastForward && !fastForward.CanApplyGravity())
            return (false);

        if (entityController.GetMoveState() == EntityController.MoveState.InAir && !entityJump.HasJumped())
            return (true);

        if (baseGravity.IsGoingDownToGround())
            return (true);

        return (false);
    }
    private bool WeCanApplyGravityForceButCanWeApplyAll()
    {
        if (coolDownBeforeAttract.IsRunning())
            return (false);

        return (true);
    }

    /// <summary>
    /// gravity base apply on this attractor
    /// </summary>
    /// <returns></returns>
    public override float GetAirRatioGravity()
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return (1f);

        float normalRatio = (!CanApplyGravityForce()) ? 1f : pointInfo.gravityBaseRatio;
        return (normalRatio);
    }

    public bool CanApplyForceDown()
    {
        if (coolDownBeforeApplyForceDown.IsRunning())
            return (false);
        return (true);
    }

    public override void JustJumped()
    {
        coolDownBeforeAttract.Reset();
        coolDownBeforeApplyForceDown.StartCoolDown(timeBeforeApplyForceDown);
        applyGalaxyForce = false;
    }

    public override void OnGrounded()
    {
        coolDownBeforeAttract.Reset();
        coolDownBeforeApplyForceDown.Reset();
        applyGalaxyForce = false;
    }


    public override Vector3 GetGAGravityAtThisPoint(Vector3 posEntity)
    {
        return (GetAirSphereGravity(posEntity).sphereGravity);
    }

    /// <summary>
    /// calculate and set the gravity
    /// if justCalculate = true, do NOT set the gravity, but return it
    /// </summary>
    /// <param name="entityPosition"></param>
    /// <param name="justCalculate"></param>
    /// <returns></returns>
    protected override void CalculateGAGravity(Vector3 rbPos)
    {
        //Setup jump calculation when going down
        if (!applyGalaxyForce && baseGravity.IsGoingDownToGround())
        {
            //Debug.Log("ici going down ?");
            //here do a jumpCalculation
            applyGalaxyForce = true;

            if (entityJump.HasJumped() && entityJumpCalculation && entityJumpCalculation.UltimeTestBeforeAttractor())
            {
                //Debug.Log("here we have hit (and good angle), fall down with normal gravity jump");
                coolDownBeforeAttract.StartCoolDown(timeBeforeActiveAllAttractorAfterJumpCalculation);
            }
            else
            {
                //Debug.Log("here no hit, or not good angle");
            }
        }

        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
        {
            //Debug.Log("ici gravité terrestre ?");
            coolDownBeforeAttract.Reset();
            pointInfo.sphereGravity = groundCheck.GetDirLastNormal();
            pointInfo.pos = pointInfo.posRange = groundCheck.GetPointLastHit();
            wantedDirGravityOnGround = GetAirSphereGravity(rbPos).sphereGravity;
        }
        //here we can't apply, because we just jump (OR because we are falling and in the timer
        else if (!CanApplyGravityForce())
        {
            //Debug.Log("ici gravité last jump !");
            pointInfo.sphereGravity = wantedDirGravityOnGround = lastNormalJumpChoosen;
            //pointGroundHit = groundCheck.ResearchInitialGround(false);
            if (calculateGroundPos)
                pointInfo.pos = pointInfo.posRange = groundCheck.ResearchInitialGround(false);
        }
        else
        {
            //Debug.Log("ici in air gravity");
            pointInfo = GetAirSphereGravity(rbPos);
            wantedDirGravityOnGround = lastNormalJumpChoosen;
        }
    }

    /// <summary>
    /// get median of all attraction (3 max ?)
    /// </summary>
    public override GravityAttractorLD.PointInfo GetAirSphereGravity(Vector3 posEntity)
    {
        bool applyAllForce = WeCanApplyGravityForceButCanWeApplyAll();

        //prepare array
        GravityAttractorLD.PointInfo[] allPointInfo = new GravityAttractorLD.PointInfo[allGravityAttractor.Count];
        Vector3[] closestPost = ExtUtilityFunction.CreateNullVectorArray(allGravityAttractor.Count + 1);
        Vector3[] sphereDir = ExtUtilityFunction.CreateNullVectorArray(allGravityAttractor.Count + 1);

        //fill array with data from 
        for (int i = 0; i < allGravityAttractor.Count; i++)
        {
            GetClosestPointOfGA(posEntity, allGravityAttractor[i], ref allPointInfo[i]);

            sphereDir[i] = allPointInfo[i].sphereGravity;

            //correct pos depending on ratio ?
            closestPost[i] = allPointInfo[i].posRange;
            //closestPost[i] = allPointInfo[i].pos;

            //ExtDrawGuizmos.DebugWireSphere(allPointInfo[i].posRange, Color.blue, 1f);
            //ExtDrawGuizmos.DebugWireSphere(allPointInfo[i].pos, Color.green, 1f);
        }


        if (!applyAllForce)
        {
            //ici anuller les gravités avec un dot positif au jump            
            for (int i = 0; i < allGravityAttractor.Count; i++)
            {
                float dotGravity = ExtQuaternion.DotProduct(sphereDir[i], lastNormalJumpChoosen);
                //Debug.Log("dot: " + dotGravity);
                if (dotGravity > marginNegativeJumpHit)
                {
                    sphereDir[i] = closestPost[i] = ExtUtilityFunction.GetNullVector();
                }
            }
            //here create a fake gravity close enought (from the hit point);
            Vector3 pointHit = entityJumpCalculation.GetHitPoint();
            sphereDir[allGravityAttractor.Count] = lastNormalJumpChoosen;
            closestPost[allGravityAttractor.Count] = posEntity + lastNormalJumpChoosen.normalized * (posEntity - pointHit).magnitude;
            //Debug.DrawRay(posEntity, sphereDir[allGravityAttractor.Count], Color.black, 2f);
            //ExtDrawGuizmos.DebugWireSphere(closestPost[allGravityAttractor.Count], Color.black, 2f, 2f);
        }


        //setup the closest point, and his vector director
        int indexFound = -1;
        Vector3 close = ExtUtilityFunction.GetClosestPoint(posEntity, closestPost, ref indexFound);

        if (ExtUtilityFunction.IsNullVector(close))
        {
            Debug.LogWarning("null gravity !!");
            pointInfo.sphereGravity = lastNormalJumpChoosen;
            if (calculateGroundPos)
                pointInfo.pos = pointInfo.posRange = lastNormalJumpChoosen * 999;
            return (pointInfo);
        }

//////////////////////////////////////////////////// TMP
//GravityAttractorLD.PointInfo closestPointTmp = allPointInfo[indexFound];
//return (closestPointTmp);
//////////////////////////////////////////////////// TMP


        Vector3 closestVectorDir = close - posEntity;

        //the default force is this point
        float defaultForce = (closestVectorDir).sqrMagnitude;
        Debug.DrawRay(posEntity, closestVectorDir.normalized * defaultForce, Color.cyan);

        for (int i = 0; i < sphereDir.Length; i++)
        {
            if (i == indexFound)
                continue;

            if (ExtUtilityFunction.IsNullVector(sphereDir[i]) || ExtUtilityFunction.IsNullVector(closestPost[i]))
                continue;


            Vector3 currentVectorDir = closestPost[i] - posEntity;
            float magnitudeCurrentForce = (currentVectorDir).sqrMagnitude;

            //
            if (magnitudeCurrentForce > defaultForce * maxDistBasedOnHowManyTimeDefault)
            {
                //Debug.DrawRay(posEntity, currentVectorDir.normalized, Color.black);
                sphereDir[i] = ExtUtilityFunction.GetNullVector();
                continue;
            }

            //tesst dot product



            float currentForce = defaultForce / (magnitudeCurrentForce * ratioOtherDistance);
            sphereDir[i] *= Mathf.Clamp(currentForce, 0f, 1f);

            //Debug.DrawRay(posEntity, currentVectorDir.normalized * currentForce, Color.magenta);
        }

        Vector3 middleOfAllVec = ExtQuaternion.GetMiddleOfXVector(sphereDir);

        //here we found nothing exept the last jump !
        if (indexFound >= allPointInfo.Length)
        {
            GravityAttractorLD.PointInfo closestPointJump = pointInfo;
            closestPointJump.sphereGravity = middleOfAllVec;
            if (calculateGroundPos)
                closestPointJump.pos = closestPointJump.posRange = groundCheck.ResearchInitialGround(false);
            return (closestPointJump);
        }

        GravityAttractorLD.PointInfo closestPoint = allPointInfo[indexFound];
        closestPoint.sphereGravity = middleOfAllVec;
        //pointGroundHit = closestPoint.posRange;
        //Debug.Break();

        return (closestPoint);
    }

    private void FixedUpdate()
    {
        if (!calculateEveryFixedFrame)
        {
            if (frequencyTimer.Ready())
                CalculateGAGravity(rbEntity.transform.position);
        }
        else
        {
            CalculateGAGravity(rbEntity.transform.position);
        }
    }
}
