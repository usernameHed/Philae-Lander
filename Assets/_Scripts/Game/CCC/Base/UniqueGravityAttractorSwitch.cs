using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Attractor;
using UnityEssentials.PropertyAttribute.noNull;
using UnityEssentials.PropertyAttribute.readOnly;
using UnityEssentials.time;

public class UniqueGravityAttractorSwitch : MonoBehaviour
{
    [SerializeField, NoNull] protected Graviton _graviton = default;
    [SerializeField, NoNull] protected Rigidbody _rigidBody = default;

    [Tooltip("gravité du saut"), SerializeField]
    protected bool calculateEveryFixedFrame = true;


    [Tooltip(""), SerializeField]
    public float marginDotGA = 0.71f;
    
    [Tooltip("More you have, less they attract !"), SerializeField]
    public float ratioOtherDistance = 1.3f;
    [Tooltip(""), SerializeField]
    public float maxDistBasedOnHowManyTimeDefault = 3f;

    [Tooltip(""), SerializeField]
    protected Rigidbody rbEntity = null;

    public float gravityBaseRatio = 0.3f;
    public float gravityDownRatio = 0.5f;
    public Vector3 GravityDirection { get { return (_graviton.GravityDirection); } set { _graviton.GravityDirection = value; } }

    public Vector3 GetPosRange() => _graviton.ClosestAttractor.ContactPoint;// pointInfo.posRange;
    //protected Vector3 pointGroundHit;
    public void OverrideContactPointOfClosestAttractor(Vector3 newContactPoint)
    {
        _graviton.OverrideContactPointOfClosestAttractor(newContactPoint);
    }

    [Tooltip("time between 2 update"), SerializeField]
    protected FrequencyTimer frequencyTimer;

    protected Vector3 lastNormalJumpChoosen = Vector3.up;

    public virtual void SetLastDirJump(Vector3 dirNormalChoosen)
    {
        lastNormalJumpChoosen = dirNormalChoosen;
    }

    /// <summary>
    /// gravity base apply on this attractor
    /// </summary>
    /// <returns></returns>
    public virtual float GetAirRatioGravity()
    {
        float normalRatio = gravityBaseRatio;
        return (normalRatio);
    }

    /// <summary>
    /// ratio only for gravityDown
    /// </summary>
    /// <returns></returns>
    public float GetRatioGravityDown()
    {
        return (gravityDownRatio);
    }

    
    public virtual void SetDefaultGAgravity(Vector3 posHit, Vector3 gravity)
    {
        gravityBaseRatio = 0.3f;
        gravityDownRatio = 0.5f;

        OverrideContactPointOfClosestAttractor(posHit);
        GravityDirection = -gravity;
        lastNormalJumpChoosen = gravity;
    }
    

    //public Vector3 GetDirGAGravity()
    //{
    //    return (pointInfo.sphereGravity);
    //}


    //public virtual Vector3 GetGAGravityAtThisPoint()
    //{
    //    return (_graviton.GravityDirection);
    //}

    /////// <summary>
    /////// calculate and set the gravity
    /////// if justCalculate = true, do NOT set the gravity, but return it
    /////// </summary>
    /////// <param name="entityPosition"></param>
    /////// <param name="justCalculate"></param>
    /////// <returns></returns>
    protected virtual void CalculateGAGravity()
    {
        _graviton.CalculatePhysicNormal();
    }

    /// <summary>
    /// get median of all attraction (3 max ?)
    /// </summary>
    //public virtual GravityAttractorLD.PointInfo GetAirSphereGravity(Vector3 posEntity)
    //{
    //    //prepare array
    //    GravityAttractorLD.PointInfo[] allPointInfo = new GravityAttractorLD.PointInfo[allGravityAttractor.Count];
    //    Vector3[] closestPost = ExtUtilityFunction.CreateNullVectorArray(allGravityAttractor.Count + 1);
    //    Vector3[] sphereDir = ExtUtilityFunction.CreateNullVectorArray(allGravityAttractor.Count + 1);
    //
    //    //fill array with data from 
    //    for (int i = 0; i < allGravityAttractor.Count; i++)
    //    {
    //        GetClosestPointOfGA(posEntity, allGravityAttractor[i], ref allPointInfo[i]);
    //
    //        sphereDir[i] = allPointInfo[i].sphereGravity;
    //
    //        //correct pos depending on ratio ?
    //        closestPost[i] = allPointInfo[i].posRange;
    //        //closestPost[i] = allPointInfo[i].pos;
    //
    //        //ExtDrawGuizmos.DebugWireSphere(allPointInfo[i].posRange, Color.blue, 1f);
    //        //ExtDrawGuizmos.DebugWireSphere(allPointInfo[i].pos, Color.green, 1f);
    //    }
    //
    //    //setup the closest point, and his vector director
    //    int indexFound = -1;
    //    Vector3 close = ExtUtilityFunction.GetClosestPoint(posEntity, closestPost, ref indexFound);
    //
    //    if (ExtUtilityFunction.IsNullVector(close))
    //    {
    //        Debug.LogError("null gravity !!");
    //        pointInfo.sphereGravity = lastNormalJumpChoosen;
    //        pointInfo.pos = pointInfo.posRange = lastNormalJumpChoosen * 999;
    //        return (pointInfo);
    //    }
    //
////////////////////////////////////////////////////// TMP
//Gr//avityAttractorLD.PointInfo closestPointTmp = allPointInfo[indexFound];
//re//turn (closestPointTmp);
////////////////////////////////////////////////////// TMP
    //
    //
    //    Vector3 closestVectorDir = close - posEntity;
    //
    //    //the default force is this point
    //    float defaultForce = (closestVectorDir).sqrMagnitude;
    //    Debug.DrawRay(posEntity, closestVectorDir.normalized * defaultForce, Color.cyan);
    //
    //    for (int i = 0; i < sphereDir.Length; i++)
    //    {
    //        if (i == indexFound)
    //            continue;
    //
    //        if (ExtUtilityFunction.IsNullVector(sphereDir[i]) || ExtUtilityFunction.IsNullVector(closestPost[i]))
    //            continue;
    //
    //
    //        Vector3 currentVectorDir = closestPost[i] - posEntity;
    //        float magnitudeCurrentForce = (currentVectorDir).sqrMagnitude;
    //
    //        //
    //        if (magnitudeCurrentForce > defaultForce * maxDistBasedOnHowManyTimeDefault)
    //        {
    //            //Debug.DrawRay(posEntity, currentVectorDir.normalized, Color.black);
    //            sphereDir[i] = ExtUtilityFunction.GetNullVector();
    //            continue;
    //        }
    //
    //        //tesst dot product
    //
    //
    //
    //        float currentForce = defaultForce / (magnitudeCurrentForce * ratioOtherDistance);
    //        sphereDir[i] *= Mathf.Clamp(currentForce, 0f, 1f);
    //
    //        //Debug.DrawRay(posEntity, currentVectorDir.normalized * currentForce, Color.magenta);
    //    }
    //
    //    Vector3 middleOfAllVec = ExtQuaternion.GetMiddleOfXVector(sphereDir);
    //
    //    //here we found nothing exept the last jump !
    //    if (indexFound >= allPointInfo.Length)
    //    {
    //        GravityAttractorLD.PointInfo closestPointJump = pointInfo;
    //        closestPointJump.sphereGravity = middleOfAllVec;
    //        closestPointJump.pos = closestPointJump.posRange = -lastNormalJumpChoosen * 999;
    //        return (closestPointJump);
    //    }
    //
    //    GravityAttractorLD.PointInfo closestPoint = allPointInfo[indexFound];
    //    closestPoint.sphereGravity = middleOfAllVec;
    //    //pointGroundHit = closestPoint.posRange;
    //    //Debug.Break();
    //
    //    return (closestPoint);
    //}

    ///// <summary>
    ///// return the closest point of a given GravityAttractor
    ///// </summary>
    ///// <param name="posEntity"></param>
    ///// <returns></returns>
    //protected bool GetClosestPointOfGA(Vector3 posEntity, GravityAttractorLD gravityAttractorToTest, ref GravityAttractorLD.PointInfo pointInfoToFill)
    //{
    //    //Debug.Log("ou la ?");
    //    pointInfoToFill = gravityAttractorToTest.FindNearestPoint(posEntity);
    //    if (ExtUtilityFunction.IsNullVector(pointInfoToFill.pos))
    //    {
    //        //Debug.LogWarning("ici on a pas trouvé de nouvelle gravité... garder comme maintenant ? mettre le compteur de mort ?");
    //        //Debug.DrawRay(posEntity, pointInfo.sphereGravity * -10, Color.red, 5f);
    //        return (false);
    //    }
    //    //pointInfo = tmpPointInfo;
    //    pointInfoToFill.sphereGravity = (posEntity - pointInfoToFill.pos).normalized;
    //    return (true);
    //}

    
    ///// <summary>
    ///// get the closest grabityLdAttractor !
    ///// </summary>
    ///// <returns></returns>
    //protected GravityAttractorLD FindClosestGravityAttractor(Vector3 posEntity)
    //{
    //    if (allGravityAttractor.Count == 0)
    //        return (null);
    //
    //    //GravityAttractorLD tmpGravityAttractor = allGravityAttractor[0];
    //    Vector3[] allPosAttractro = new Vector3[allGravityAttractor.Count];
    //
    //    for (int i = 0; i < allGravityAttractor.Count; i++)
    //    {
    //        allPosAttractro[i] = allGravityAttractor[i].transform.position;
    //    }
    //    int indexFound = -1;
    //    Vector3 closestGravityLd = ExtUtilityFunction.GetClosestPoint(posEntity, allPosAttractro, ref indexFound);
    //    if (ExtUtilityFunction.IsNullVector(closestGravityLd))
    //        return (null);
    //    return (allGravityAttractor[indexFound]);
    //}

    public virtual bool CanApplyGravityForce()
    {
        return (true);
    }

    public bool IsNormalIsOkWithCurrentGravity(Vector3 normalHit, Vector3 currentGravity)
    {
        //if angle hitInfo.normal eet notre gravity est pas bonne,
        //dire de ne pas ground ! return false !
        //else, angle ok, return true !
        float dotDiff = Vector3.Dot(normalHit.normalized, currentGravity.normalized);
        if (dotDiff > marginDotGA)
        {
            //Debug.Log("ok normal correct for moving...");
            //pointInfo = tmpLastPointInfo;
            //sphereGravity = tmpSphereGravity;
            return (true);
        }
        //Debug.Log("here we... have bad normal ! don't walk...");
        //Debug.DrawRay(rbEntity.position, normalHit * 5, Color.red);
        //Debug.DrawRay(rbEntity.position, currentGravity * 5, Color.black);
        return (false);
    }

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
