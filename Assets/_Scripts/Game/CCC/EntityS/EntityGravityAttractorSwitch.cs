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
    public float timeBeforeActiveAttractor = 0.5f;

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
    /// can do sideJump ONLY if we were NOT
    /// previously in airMove, AND the layer is still an yellow GA
    /// </summary>
    /// <returns></returns>
    public bool CanDoSideJump(Transform objHit, Vector3 normalHit)
    {
        if (IsAirAttractorLayer(objHit.gameObject.layer))
        {
            Debug.LogWarning("ici on peut pas faire un side jump, ");
            return (false);
        }
        return (true);
    }

    /*
    /// <summary>
    /// Do a pre-calcul of a new gravity, but get back to old one after (like in a jumpCalculation)
    /// for forwardCheck, we can keep our new one found
    /// </summary>
    private Vector3 GetPrecalculPointForJump(Transform objHit, Vector3 posToTest, bool canChangeAttractor)
    {
        GravityAttractorLD tmpOld = lastClosestGravityAttractor;
        TryToSetNewGravityAttractor(objHit);
        tmpLastPointInfo = GetGroundSphereGravity(rbEntity.position);
        CalculateSphereGravity(rbEntity.position, true);
        //Vector3 tmpSphereGravity = sphereGravity;
        
        if (tmpOld != null && !canChangeAttractor)
        {
            Debug.Log("ok, choose to take the old one here");
            SelectNewGA(tmpOld, true);
        }
        else
        {
            Debug.Log("choose to unselect here !");
            //fuck;
            if (!canChangeAttractor)
                UnselectOldGA();
        }
        return (tmpLastPointInfo.sphereGravity);
    }
    */
    /*
    /// <summary>
    /// Check if the normal collision is ok with this gravity
    /// canChange: change the attractor at the end if we find another one
    /// </summary>
    public bool IsNormalOk(Transform objHit, Vector3 normalHit, bool canChange)
    {
        Vector3 tmpSphereGravity = pointInfo.sphereGravity;

        //if (!gravityAttractor)
        //{
            //TODO: here calculate the gravity...
            tmpSphereGravity = GetPrecalculPointForJump(objHit, rbEntity.position, canChange);
        //}

        Debug.DrawRay(rbEntity.position, normalHit, Color.blue, 5f);
        Debug.DrawRay(rbEntity.position, tmpSphereGravity, Color.black, 5f);

        float dotDiff = ExtQuaternion.DotProduct(normalHit, tmpSphereGravity);
        //Debug.Log("dot diff: " + dotDiff);
        if (dotDiff > marginDotGA)
        {
            //Debug.Log("ok normal correct for moving...");
            return (true);
        }
        return (false);
    }
    */

        /*
    public bool IsNormalAcceptedIfWeAreInGA(Transform objHit, Vector3 normalHit)
    {
        //here it doen't concern us, say just yes
        if (!IsAirAttractorLayer(objHit.gameObject.layer))
            return (true);

        //here we are in this layer, test if we are in a gravityAttractor mode, if not
        if (!gravityAttractorMode)
        {
            //here we are not... mmm what to do ?
            Debug.LogWarning("we are on a gravityAttractor tag, but we are not" +
                "may be we are in jump calculation, it's okk then");
            TryToSetNewGravityAttractor(objHit);

            return (true);
        }

        //tmpLastPointInfo = GetGroundSphereGravity(rbEntity.position);
        //CalculateSphereGravity(rbEntity.position, true);

        //if angle hitInfo.normal eet notre gravity est pas bonne,
        //dire de ne pas ground ! return false !
        //else, angle ok, return true !
        float dotDiff = ExtQuaternion.DotProduct(normalHit, tmpLastPointInfo.sphereGravity);
        if (dotDiff > marginDotGA)
        {
            //Debug.Log("ok normal correct for moving...");
            pointInfo = tmpLastPointInfo;
            //sphereGravity = tmpSphereGravity;
            return (true);
        }
        //Debug.Log("here we... have bad normal ! don't walk...");
        Debug.DrawRay(rbEntity.position, normalHit * 5, Color.red);
        Debug.DrawRay(rbEntity.position, pointInfo.sphereGravity * 5, Color.black);

        //Debug.Break();
        return (false);
    }
    */

    /*
    public void TryToSetNewGravityAttractor(Transform obj)
    {
        if (!IsAirAttractorLayer(obj.gameObject.layer))
        {
            UnselectOldGA();
        }
        else
        {
            lastClosestGravityAttractor = obj.GetComponentInParent<GravityAttractorLD>();
            if (lastClosestGravityAttractor == null)
                lastClosestGravityAttractor = FindClosestGravityAttractor();
            if (lastClosestGravityAttractor == null)
            {
                UnselectOldGA();
                return;
            }
        }

        
        if (!lastClosestGravityAttractor && !tmpGravity)
        {
            Debug.Log("si on a pas d'ancien, et rien de nouveau, ne rien faire");
            return;
        }
        if (!lastClosestGravityAttractor && tmpGravity)
        {
            Debug.Log("si on a pas d'ancien, mais un nouveau, alors banco");
            SelectNewGA(tmpGravity, false);
            return;
        }
        if (lastClosestGravityAttractor && tmpGravity)
        {
            Debug.Log("tmpGravity: " + tmpGravity);
            //Debug.Log("obj.gameObject " + obj.gameObject);

            Debug.Log("on passe d'un ancien à un nouveau d'un coup !");
            //Debug.Break();
            if (tmpGravity.GetInstanceID() != lastClosestGravityAttractor.GetInstanceID())
            {
                UnselectOldGA();
                SelectNewGA(tmpGravity, true);
            }
            //Debug.Break();
            return;
        }
        if (lastClosestGravityAttractor && !tmpGravity)
        {
            //Debug.Log("on passe d'un ancien à rien !");
            UnselectOldGA();
            return;
        }
        Debug.Log("not found ?");
        
    }
    */

        /*
    private void SelectNewGA(GravityAttractorLD newGA, bool calculateNow)
    {
        lastClosestGravityAttractor = newGA;
        lastClosestGravityAttractor.SelectedGravityAttractor();

        gravityAttractorMode = true;
        //EnterInZone(newGA);

        CalculateGAGravity();
    }
    */

        /*
    /// <summary>
    /// leave GA
    /// </summary>
    public void UnselectOldGA()
    {
        lastClosestGravityAttractor.UnselectGravityAttractor();
        lastClosestGravityAttractor = null;
        //gravityPoint.Clear();
        gravityAttractorMode = false;
    }
    */
    
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

    
    private GravityAttractorLD.PointInfo GetAirSphereGravity(Vector3 posEntity)
    {
        /*
        //Debug.Log("ou la ?");
        for (int i = 0; i < allGravityAttractor.Count; i++)
        {
            tmpLastPointInfo = allGravityAttractor[i].FindNearestPoint(posEntity);
        }
        lastClosestGravityAttractor = //le plus proche
        */
        tmpLastPointInfo = groundAttractor.FindNearestPoint(posEntity);
        if (ExtUtilityFunction.IsNullVector(tmpLastPointInfo.pos))
        {
            Debug.LogWarning("ici on a pas trouvé de nouvelle gravité... garder comme maintenant ? mettre le compteur de mort ?");
            Debug.DrawRay(posEntity, pointInfo.sphereGravity * -10, Color.red, 5f);
            return (pointInfo);
        }
        //pointInfo = tmpPointInfo;
        tmpLastPointInfo.sphereGravity = (posEntity - pointInfo.pos).normalized;
        return (tmpLastPointInfo);
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
            Debug.LogWarning("ici on a pas trouvé de nouvelle gravité... garder comme maintenant ? mettre le compteur de mort ?");
            Debug.DrawRay(posEntity, pointInfo.sphereGravity * -10, Color.red, 5f);
            return (false);
        }
        //pointInfo = tmpPointInfo;
        pointInfoToFill.sphereGravity = (posEntity - pointInfo.pos).normalized;
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
        Debug.DrawRay(rbEntity.position, htiInfoToCheck.normal, Color.green, 3f);
        Debug.DrawRay(rbEntity.position, pointInfoToCatch.sphereGravity, Color.red, 3f);

        //here we have a valide point in this gravitySphere !
        //return true if the normal is ok
        normalOk = IsNormalIsOkWithCurrentGravity(htiInfoToCheck.normal, pointInfoToCatch.sphereGravity);

        Debug.Log("normal: " + normalOk);

        //Debug.Break();
        return (true);
    }

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

    private void FixedUpdate()
    {
        if (entityController.GetMoveState() == EntityController.MoveState.InAir)
        {
            CalculateGAGravity();
        }
    }
}
