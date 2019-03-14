using AiUnity.MultipleTags.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGravityAttractorSwitch : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float marginDotGA = 0.86f;
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

    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private Rigidbody rbEntity = null;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityController entityController;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityJumpCalculation entityJumpCalculation;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityGravity entityGravity;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private GroundCheck groundCheck;
    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private EntityJump entityJump;

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private GravityAttractorLD.PointInfo pointInfo = new GravityAttractorLD.PointInfo();
    
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private GravityAttractorLD.PointInfo tmpLastPointInfo = new GravityAttractorLD.PointInfo();

    [FoldoutGroup("Debug"), SerializeField, Tooltip(""), ReadOnly]
    private List<GravityAttractorLD> allGravityAttractor = new List<GravityAttractorLD>();

    //[FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    //public Vector3 sphereGravity = Vector3.zero;

    private FrequencyCoolDown coolDownBeforeApplyForceDown = new FrequencyCoolDown();
    private FrequencyCoolDown coolDownBeforeAttract = new FrequencyCoolDown();
    private Vector3 lastNormalJumpChoosen = Vector3.up;
    private bool applyGalaxyForce = false;
    private Vector3 wantedDirGravityOnGround = Vector3.zero;

    public Vector3 GetWantedGravityOnGround() => wantedDirGravityOnGround;
    //private bool isGoingDown = false;

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


    /// <summary>
    /// ratio only for gravityDown
    /// </summary>
    /// <returns></returns>
    public float GetRatioGravityDown()
    {
        return (pointInfo.gravityDownRatio);
    }
    
    /// <summary>
    /// can we apply gravity force ?
    /// if we are going up (from jump), we can't
    /// if the timer is still running, we can't neiter
    /// </summary>
    /// <returns></returns>
    private bool CanApplyGravityForce()
    {
        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
            return (true);
        if (entityController.GetMoveState() == EntityController.MoveState.InAir && !entityJump.HasJumped())
            return (true);

        if (entityGravity.IsGoingDownToGround())
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
    public float GetAirRatioGravity()
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

    public void JustJumped()
    {
        coolDownBeforeAttract.Reset();
        coolDownBeforeApplyForceDown.StartCoolDown(timeBeforeApplyForceDown);
        applyGalaxyForce = false;
    }

    public void SetLastDirJump(Vector3 dirNormalChoosen)
    {
        lastNormalJumpChoosen = wantedDirGravityOnGround = dirNormalChoosen;
    }

    public void OnGrounded()
    {
        coolDownBeforeAttract.Reset();
        coolDownBeforeApplyForceDown.Reset();
        applyGalaxyForce = false;
    }

    public Vector3 GetDirGAGravity()
    {
        return (pointInfo.sphereGravity);
    }

    private void CalculateGAGravity()
    {
        //Setup jump calculation when going down
        if (!applyGalaxyForce && entityGravity.IsGoingDownToGround())
        {
            //here do a jumpCalculation
            applyGalaxyForce = true;

            if (entityJumpCalculation.UltimeTestBeforeAttractor())
            {
                Debug.Log("here we have hit, fall down with normal gravity jump");
                coolDownBeforeAttract.StartCoolDown(timeBeforeActiveAllAttractorAfterJumpCalculation);
            }
        }

        if (entityController.GetMoveState() != EntityController.MoveState.InAir)
        {
            pointInfo.sphereGravity = groundCheck.GetDirLastNormal();
            wantedDirGravityOnGround = GetAirSphereGravity(rbEntity.position).sphereGravity;
        }
        //here we can't apply, because we just jump (OR because we are falling and in the timer
        else if (!CanApplyGravityForce())
        {
            pointInfo.sphereGravity = wantedDirGravityOnGround = lastNormalJumpChoosen;
        }
        else
        {
            pointInfo = GetAirSphereGravity(rbEntity.position);
            wantedDirGravityOnGround = lastNormalJumpChoosen;
        }
    }

    /// <summary>
    /// get median of all attraction (3 max ?)
    /// </summary>
    private GravityAttractorLD.PointInfo GetAirSphereGravity(Vector3 posEntity)
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

            ExtDrawGuizmos.DebugWireSphere(allPointInfo[i].posRange, Color.blue, 1f);
            ExtDrawGuizmos.DebugWireSphere(allPointInfo[i].pos, Color.green, 1f);
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
            Debug.LogError("null gravity !!");
            pointInfo.sphereGravity = lastNormalJumpChoosen;
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
                Debug.DrawRay(posEntity, currentVectorDir.normalized, Color.black);
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
            return (closestPointJump);
        }

        GravityAttractorLD.PointInfo closestPoint = allPointInfo[indexFound];
        closestPoint.sphereGravity = middleOfAllVec;
        //Debug.Break();
        
        return (closestPoint);
    }

    /// <summary>
    /// return the closest point of a given GravityAttractor
    /// </summary>
    /// <param name="posEntity"></param>
    /// <returns></returns>
    private bool GetClosestPointOfGA(Vector3 posEntity, GravityAttractorLD gravityAttractorToTest, ref GravityAttractorLD.PointInfo pointInfoToFill)
    {
        //Debug.Log("ou la ?");
        pointInfoToFill = gravityAttractorToTest.FindNearestPoint(posEntity);
        if (ExtUtilityFunction.IsNullVector(pointInfoToFill.pos))
        {
            //Debug.LogWarning("ici on a pas trouvé de nouvelle gravité... garder comme maintenant ? mettre le compteur de mort ?");
            //Debug.DrawRay(posEntity, pointInfo.sphereGravity * -10, Color.red, 5f);
            return (false);
        }
        //pointInfo = tmpPointInfo;
        pointInfoToFill.sphereGravity = (posEntity - pointInfoToFill.pos).normalized;
        return (true);
    }

    
    /// <summary>
    /// get the closest grabityLdAttractor !
    /// </summary>
    /// <returns></returns>
    private GravityAttractorLD FindClosestGravityAttractor(Vector3 posEntity)
    {
        if (allGravityAttractor.Count == 0)
            return (null);

        //GravityAttractorLD tmpGravityAttractor = allGravityAttractor[0];
        Vector3[] allPosAttractro = new Vector3[allGravityAttractor.Count];

        for (int i = 0; i < allGravityAttractor.Count; i++)
        {
            allPosAttractro[i] = allGravityAttractor[i].transform.position;
        }
        int indexFound = -1;
        Vector3 closestGravityLd = ExtUtilityFunction.GetClosestPoint(posEntity, allPosAttractro, ref indexFound);
        if (ExtUtilityFunction.IsNullVector(closestGravityLd))
            return (null);
        return (allGravityAttractor[indexFound]);
    }

    public bool IsNormalIsOkWithCurrentGravity(Vector3 normalHit, Vector3 currentGravity)
    {
        //if angle hitInfo.normal eet notre gravity est pas bonne,
        //dire de ne pas ground ! return false !
        //else, angle ok, return true !
        float dotDiff = ExtQuaternion.DotProduct(normalHit.normalized, currentGravity.normalized);
        if (dotDiff > marginDotGA)
        {
            //Debug.Log("ok normal correct for moving...");
            //pointInfo = tmpLastPointInfo;
            //sphereGravity = tmpSphereGravity;
            return (true);
        }
        //Debug.Log("here we... have bad normal ! don't walk...");
        Debug.DrawRay(rbEntity.position, normalHit * 5, Color.red);
        Debug.DrawRay(rbEntity.position, currentGravity * 5, Color.black);
        return (false);
    }

    private void FixedUpdate()
    {
        CalculateGAGravity();
    }
}
