

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Attractor;
using UnityEssentials.Extensions;
using UnityEssentials.time;

public class EntityGravityAttractorSwitch : BaseGravityAttractorSwitch
{
    [Tooltip(""), SerializeField]
    private float timeBeforeApplyForceDown = 0.4f;
    [Tooltip(""), SerializeField]
    private float timeBeforeActiveAllAttractorAfterJumpCalculation = 2f;
    [Tooltip(""), SerializeField]
    private float marginNegativeJumpHit = -0.1f;
    [Tooltip(""), SerializeField]
    private bool calculateGroundPos = true;

    [Tooltip(""), SerializeField]
    private EntityJumpCalculation entityJumpCalculation = default;
    [Tooltip(""), SerializeField]
    private FastForward fastForward = default;
    [Tooltip(""), SerializeField]
    private EntityJump entityJump = default;

    [SerializeField]
    private ExtGravitonCalculation _additionnalGravityCalculation = new ExtGravitonCalculation();


    private FrequencyCoolDown coolDownBeforeApplyForceDown = new FrequencyCoolDown();
    private FrequencyCoolDown coolDownBeforeAttract = new FrequencyCoolDown();


    private bool applyGalaxyForce = false;

    /// <summary>
    /// using the current attractor that attract the graviton, calculate
    /// gravity for another point
    /// </summary>
    /// <returns></returns>
    public Vector3 GetGravityAtAnyGivenPointUsingCurrentAttractorInList(Vector3 point)
    {
        _additionnalGravityCalculation.SetupGravityFields(_graviton.AttractorApplyingForce, point);
        _additionnalGravityCalculation.CalculateGravityFields();

        bool applyAllForce = WeCanApplyGravityForceButCanWeApplyAll();
        if (!applyAllForce)
        {
            _additionnalGravityCalculation.RemoveAttractorFromOneDirection(lastNormalJumpChoosen, marginNegativeJumpHit);
        }
        return (_additionnalGravityCalculation.CalculateForces(_graviton.Mass));
    }
    
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

        if (entityController.GetMoveState() == EntityController.MoveState.InAir && (entityJump && !entityJump.HasJumped()))
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

        float normalRatio = (!CanApplyGravityForce()) ? 1f : gravityBaseRatio;
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
        Debug.Log("JUST JUMPED!");
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


    //public override Vector3 GetGAGravityAtThisPoint(Vector3 posEntity)
    //{
    //    return (GetAirSphereGravity(posEntity).sphereGravity);
    //}

    /// <summary>
    /// calculate and set the gravity
    /// if justCalculate = true, do NOT set the gravity, but return it
    /// </summary>
    /// <param name="entityPosition"></param>
    /// <param name="justCalculate"></param>
    /// <returns></returns>
    protected override void CalculateGAGravity()
    {
        //Setup jump calculation when going down
        if (!applyGalaxyForce && baseGravity.IsGoingDownToGround())
        {
            Debug.Log("ici going down ?");
            //here do a jumpCalculation
            applyGalaxyForce = true;

            if (entityJump.HasJumped() && entityJumpCalculation && entityJumpCalculation.UltimeTestBeforeAttractor())
            {
                Debug.Log("here we have hit (and good angle), fall down with normal gravity jump");
                coolDownBeforeAttract.StartCoolDown(timeBeforeActiveAllAttractorAfterJumpCalculation);
            }
            else
            {
                Debug.Log("here no hit, or not good angle");
            }
        }

        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
        {
            Debug.Log("ici gravité terrestre ?");
            coolDownBeforeAttract.Reset();

            CustomCalculationWithJumpIntoConsideration();
            wantedDirGravityOnGround = -GravityDirection;

            GravityDirection = groundCheck.GetDirLastNormal();
            OverrideContactPointOfClosestAttractor(groundCheck.GetPointLastHit());
        }
        //here we can't apply, because we just jump (OR because we are falling and in the timer
        else if (!CanApplyGravityForce())
        {
            Debug.Log("ici gravité last jump !");
            wantedDirGravityOnGround = lastNormalJumpChoosen;
            GravityDirection = -lastNormalJumpChoosen;
            //pointGroundHit = groundCheck.ResearchInitialGround(false);
            if (calculateGroundPos)
            {
                OverrideContactPointOfClosestAttractor(groundCheck.ResearchInitialGround(false));
            }
        }
        else
        {
            Debug.Log("last normal jump!");
            CustomCalculationWithJumpIntoConsideration();
            wantedDirGravityOnGround = lastNormalJumpChoosen;
        }
    }

    private void CustomCalculationWithJumpIntoConsideration()
    {
        bool applyAllForce = WeCanApplyGravityForceButCanWeApplyAll();
        if (applyAllForce)
        {
            ////ici anuller les gravités avec un dot positif au jump            
            //for (int i = 0; i < allGravityAttractor.Count; i++)
            //{
            //    float dotGravity = Vector3.Dot(sphereDir[i], lastNormalJumpChoosen);
            //    //Debug.Log("dot: " + dotGravity);
            //    if (dotGravity > marginNegativeJumpHit)
            //    {
            //        sphereDir[i] = closestPost[i] = Vector3.zero;
            //    }
            //}
            ////here create a fake gravity close enought (from the hit point);
            //Vector3 pointHit = entityJumpCalculation.GetHitPoint();
            //sphereDir[allGravityAttractor.Count] = lastNormalJumpChoosen;
            //closestPost[allGravityAttractor.Count] = posEntity + lastNormalJumpChoosen.normalized * (posEntity - pointHit).magnitude;
            //Debug.DrawRay(posEntity, sphereDir[allGravityAttractor.Count], Color.black, 2f);
            //ExtDrawGuizmos.DebugWireSphere(closestPost[allGravityAttractor.Count], Color.black, 2f, 2f);
            Debug.Log("here ??");

            _graviton.CalculatePhysicNormalIgnoringADirection(-lastNormalJumpChoosen, marginNegativeJumpHit);
        }
        else
        {
            Debug.Log("or not!");
            _graviton.CalculatePhysicNormal();
        }
    }

    /*
    /// <summary>
    /// get median of all attraction (3 max ?)
    /// </summary>
    public override GravityAttractorLD.PointInfo GetAirSphereGravity(Vector3 posEntity)
    {
        bool applyAllForce = WeCanApplyGravityForceButCanWeApplyAll();

        //prepare array
        GravityAttractorLD.PointInfo[] allPointInfo = new GravityAttractorLD.PointInfo[allGravityAttractor.Count];
        Vector3[] closestPost = new Vector3[allGravityAttractor.Count + 1];
        Vector3[] sphereDir = new Vector3[allGravityAttractor.Count + 1];

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
                float dotGravity = Vector3.Dot(sphereDir[i], lastNormalJumpChoosen);
                //Debug.Log("dot: " + dotGravity);
                if (dotGravity > marginNegativeJumpHit)
                {
                    sphereDir[i] = closestPost[i] = Vector3.zero;
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
        Vector3 close = ExtVector3.GetClosestPoint(posEntity, closestPost, ref indexFound);

        if (close == Vector3.zero)
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

            if (sphereDir[i] == Vector3.zero || closestPost[i] == Vector3.zero)
                continue;


            Vector3 currentVectorDir = closestPost[i] - posEntity;
            float magnitudeCurrentForce = (currentVectorDir).sqrMagnitude;

            //
            if (magnitudeCurrentForce > defaultForce * maxDistBasedOnHowManyTimeDefault)
            {
                //Debug.DrawRay(posEntity, currentVectorDir.normalized, Color.black);
                sphereDir[i] = Vector3.zero;
                continue;
            }

            //tesst dot product



            float currentForce = defaultForce / (magnitudeCurrentForce * ratioOtherDistance);
            sphereDir[i] *= Mathf.Clamp(currentForce, 0f, 1f);

            //Debug.DrawRay(posEntity, currentVectorDir.normalized * currentForce, Color.magenta);
        }

        Vector3 middleOfAllVec = ExtVector3.GetMiddleOfXVector(sphereDir);

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
    */

    private void FixedUpdate()
    {
        if (!calculateEveryFixedFrame)
        {
            if (frequencyTimer.Ready())
            {
                CalculateGAGravity();
            }
        }
        else
        {
            CalculateGAGravity();
        }
    }
}
