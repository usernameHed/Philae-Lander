using AiUnity.MultipleTags.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGravityAttractorSwitch : MonoBehaviour
{
    //[FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    //private TagAccess.TagAccessEnum tagWithAttractor = TagAccess.TagAccessEnum.GravityAttractor;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] gravityAttractorLayer = new string[] { "Walkable/GravityAttractor" };
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float marginDotGA = 0.86f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float marginNormalJumpInGA = 0.3f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float timeBeforeActiveAttractor = 0.4f;
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

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    private bool gravityAttractorMode = false;
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private GravityAttractorLD.PointInfo pointInfo = new GravityAttractorLD.PointInfo();
    
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private GravityAttractorLD.PointInfo tmpLastPointInfo = new GravityAttractorLD.PointInfo();

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private GravityAttractorLD groundAttractor = null;
    public GravityAttractorLD GetGroundAttractor() => groundAttractor;

    [FoldoutGroup("Debug"), SerializeField, Tooltip(""), ReadOnly]
    private List<GravityAttractorLD> allGravityAttractor = new List<GravityAttractorLD>();

    //[FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    //public Vector3 sphereGravity = Vector3.zero;

    private FrequencyCoolDown coolDownBeforeActiveAtractor = new FrequencyCoolDown();
    private Vector3 lastNormalJumpChoosen = Vector3.zero;

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
        if (!gravityAttractorMode)
            return (1);
        return (pointInfo.gravityDownRatio);
    }
    
    /// <summary>
    /// gravity base apply on this attractor
    /// </summary>
    /// <returns></returns>
    public float GetRatioGravity()
    {
        float normalRatio = (!gravityAttractorMode || coolDownBeforeActiveAtractor.IsRunning()) ? 1 : pointInfo.gravityBaseRatio;
        //Debug.Log("ratio: " + normalRatio);
        return (normalRatio);
    }

    public bool IsInGravityAttractorMode()
    {
        return (groundAttractor);
    }

    public bool CanApplyForceDown()
    {
        if (coolDownBeforeActiveAtractor.IsRunning())
            return (false);
        return (true);
    }

    /// <summary>
    /// do we do a jump based on gravity of the GravityAttractorSwitch or not ?
    /// realMagnitudeInput: input of player, but if we are against a wall, input = 0;
    /// </summary>
    /// <returns></returns>
    public bool CanDoGAJump(float realMagnitudeInput)
    {
        if (IsInGravityAttractorMode() && realMagnitudeInput < marginNormalJumpInGA)
            return (true);
        return (false);
    }

    public void JustJumped()
    {
        coolDownBeforeActiveAtractor.StartCoolDown(timeBeforeActiveAttractor);
    }

    public void SetLastDirJump(Vector3 dirNormalChoosen)
    {
        lastNormalJumpChoosen = dirNormalChoosen;
    }

    public void OnGrounded()
    {
        coolDownBeforeActiveAtractor.Reset();
    }

    public Vector3 GetDirGAGravity()
    {
        return (pointInfo.sphereGravity);
    }

    public bool IsAirAttractorLayer(int layer)
    {
        int isGravityAttractor = ExtList.ContainSubStringInArray(gravityAttractorLayer, LayerMask.LayerToName(layer));
        if (isGravityAttractor == -1)
            return (false);
        return (true);
    }
    
    /// <summary>
    /// calculate the gravity based on a point
    /// chose the jump normal at first, and then calculate
    /// if fromScript is true, always calculate
    /// </summary>
    public void CalculateSphereGravity(Vector3 posEntity, bool calculateNow = false)
    {
        if (coolDownBeforeActiveAtractor.IsRunning() && !calculateNow)
        {
            //sphereGravity = groundCheck.GetDirLastNormal();
            pointInfo.sphereGravity = lastNormalJumpChoosen;
        }
        else
        {
            //Debug.Log("ou la ?");
            GravityAttractorLD.PointInfo tmpPointInfo = groundAttractor.FindNearestPoint(posEntity);
            if (ExtUtilityFunction.IsNullVector(tmpPointInfo.pos))
            {
                Debug.LogWarning("ici on a pas trouvé de nouvelle gravité... garder comme maintenant ? mettre le compteur de mort ?");
                Debug.DrawRay(posEntity, pointInfo.sphereGravity * -10, Color.red, 5f);
                return;
            }
            pointInfo = tmpPointInfo;
            pointInfo.sphereGravity = (posEntity - pointInfo.pos).normalized;
        }
    }

    /// <summary>
    /// get median of all attraction (3 max ?)
    /// </summary>
    private GravityAttractorLD.PointInfo GetAirSphereGravity(Vector3 posEntity)
    {
        //prepare array
        GravityAttractorLD.PointInfo[] allPointInfo = new GravityAttractorLD.PointInfo[allGravityAttractor.Count];
        Vector3[] closestPost = new Vector3[allGravityAttractor.Count];
        Vector3[] sphereDir = new Vector3[allGravityAttractor.Count];

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

        //setup the closest point, and his vector director
        int indexFound = -1;
        Vector3 close = ExtUtilityFunction.GetClosestPoint(posEntity, closestPost, ref indexFound);

        if (ExtUtilityFunction.IsNullVector(close))
        {
            Debug.LogError("null gravity !!");
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
            Vector3 currentVectorDir = closestPost[i] - posEntity;
            float magnitudeCurrentForce = (currentVectorDir).sqrMagnitude;

            if (magnitudeCurrentForce > defaultForce * maxDistBasedOnHowManyTimeDefault)
            {
                Debug.DrawRay(posEntity, currentVectorDir.normalized, Color.black);
                sphereDir[i] = ExtUtilityFunction.GetNullVector();
                continue;
            }

            float currentForce = defaultForce / (magnitudeCurrentForce * ratioOtherDistance);
            sphereDir[i] *= Mathf.Clamp(currentForce, 0f, 1f);

            Debug.DrawRay(posEntity, currentVectorDir.normalized * currentForce, Color.magenta);
        }

        Vector3 middleOfAllVec = ExtQuaternion.GetMiddleOfXVector(sphereDir);

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

    
    private void CalculateGAGravity()
    {
        if (IsInGravityAttractorMode())
        {
            if (coolDownBeforeActiveAtractor.IsRunning())
            {
                //sphereGravity = groundCheck.GetDirLastNormal();
                pointInfo.sphereGravity = lastNormalJumpChoosen;
            }
            else
            {
                pointInfo = GetAirSphereGravity(rbEntity.position);
            }
        }
        else
        {
            /*
            //ici on est pas en attractor mode, on prend les plus proche trouvé
            //MAIS il faut qu'il soit à une distance mminimum !
            //si on est assez proche, alors isAttractorMode = true !
            //sphereGravity = ;
            Vector3 tmpSphereGravity = GetSphereGravity(rbEntity.position);
            if ((rbEntity.position - pointInfo.pos).sqrMagnitude < 10)
            {
                //active gravityAttractor
                SelectNewGA(pointInfo.gravityAttractor, true);
            }
            //sphereGravity = GetSphereGravity(rbEntity.position);
            */
        }
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

    private void SelectNewGA(GravityAttractorLD newGA)
    {
        groundAttractor = newGA;
        groundAttractor.SelectedGravityAttractor();
        gravityAttractorMode = true;
    }

    public void UnselectOldGA()
    {
        if (groundAttractor)
            groundAttractor.UnselectGravityAttractor();
        groundAttractor = null;
        gravityAttractorMode = false;
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

    /// <summary>
    /// called by entityContact switch to know if this object normal is ok
    /// </summary>
    /// <returns></returns>
    public bool IsThisHitImpactCouldBeOk(RaycastHit htiInfoToCheck,
        ref GravityAttractorLD.PointInfo pointInfoToCatch, ref bool normalOk)
    {
        //here this object is not in the good layer
        if (!IsAirAttractorLayer(htiInfoToCheck.transform.gameObject.layer))
        {
            Debug.Log("on est ici normalement non ??????????");
            return (false);
        }
        Debug.Log("ou pas...............");
            

        GravityAttractorLD tmpGA = htiInfoToCheck.transform.gameObject.GetComponentInParent<GravityAttractorLD>();
        //here we don't even find a gravityAttractorLD associeted !
        if (!tmpGA)
            return (false);

        bool closestPooint = GetClosestPointOfGA(rbEntity.position, tmpGA, ref pointInfoToCatch);
        //here we do not find any point !
        if (!closestPooint)
            return (false);

        Debug.Log("on arrive jusque l'a ?");
        Debug.Log(pointInfoToCatch.refGA);
        Debug.Log(pointInfoToCatch.refGA.gameObject.name);
        //Debug.DrawRay(rbEntity.position, htiInfoToCheck.normal, Color.green, 3f);
        //Debug.DrawRay(rbEntity.position, pointInfoToCatch.sphereGravity, Color.red, 3f);

        //here we have a valide point in this gravitySphere !
        //return true if the normal is ok
        normalOk = IsNormalIsOkWithCurrentGravity(htiInfoToCheck.normal, pointInfoToCatch.sphereGravity);

        Debug.Log("normal: " + normalOk);

        //Debug.Break();
        return (true);
    }

    /*
    /// <summary>
    /// called by GroundCheck every time we are on a new ground object
    /// </summary>
    /// <param name="hitInfo"></param>
    public void UpdateGroundObject(RaycastHit hitInfo)
    {
        if (!IsAirAttractorLayer(hitInfo.transform.gameObject.layer))
        {
            //Debug.Log("unselect");
            UnselectOldGA();
        }
        else
        {
            //Debug.Log("select one ????");
            groundAttractor = hitInfo.transform.gameObject.GetComponentInParent<GravityAttractorLD>();
            if (groundAttractor == null)
                groundAttractor = FindClosestGravityAttractor(rbEntity.position);
            if (groundAttractor == null)
            {
                UnselectOldGA();
                return;
            }
            
            bool findGround = GetClosestPointOfGA(rbEntity.position, groundAttractor, ref tmpLastPointInfo);

            if (findGround)
            {
                SelectNewGA(groundAttractor);
                pointInfo = tmpLastPointInfo;
            }
            else
            {
                UnselectOldGA();
                return;
            }
        }
    }
    */

    private void FixedUpdate()
    {
        //if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        //{
            CalculateGAGravity();
        //}
    }
}
