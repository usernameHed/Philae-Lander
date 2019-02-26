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

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    private bool gravityAttractorMode = false;
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private GravityAttractor.PointInfo pointInfo = new GravityAttractor.PointInfo();
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private GravityAttractor gravityAttractor = null;
    

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    public Vector3 sphereGravity = Vector3.zero;

    private FrequencyCoolDown coolDownBeforeActiveAtractor = new FrequencyCoolDown();
    private Vector3 lastNormalJumpChoosen = Vector3.zero;

    
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
        return (gravityAttractor);
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
        return (sphereGravity);
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
        //bool canDo = !((gravityAttractorMode && IsAirAttractorLayer(objHit.gameObject.layer)));

        if (/*!gravityAttractorMode && */IsAirAttractorLayer(objHit.gameObject.layer)/* && IsNormalOk(objHit, normalHit)*/)
        {
            Debug.LogWarning("ici on peut pas faire un side jump, ");
            return (false);
        }

        Debug.Log("can do side jump switchesss");
        return (true);
    }

    /// <summary>
    /// Do a pre-calcul of a new gravity, but get back to old one after (like in a jumpCalculation)
    /// for forwardCheck, we can keep our new one found
    /// </summary>
    private Vector3 GetTmpSphereGravityForAPoint(Transform objHit, Vector3 posToTest, bool canChangeAttractor)
    {
        GravityAttractor tmpOld = gravityAttractor;
        TryToSetNewGravityAttractor(objHit);
        CalculateSphereGravity(rbEntity.position, true);
        Vector3 tmpSphereGravity = sphereGravity;
        
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
        return (tmpSphereGravity);
    }

    /// <summary>
    /// Check if the normal collision is ok with this gravity
    /// canChange: change the attractor at the end if we find another one
    /// </summary>
    public bool IsNormalOk(Transform objHit, Vector3 normalHit, bool canChange)
    {
        Vector3 tmpSphereGravity = sphereGravity;

        //if (!gravityAttractor)
        //{
            //TODO: here calculate the gravity...
            tmpSphereGravity = GetTmpSphereGravityForAPoint(objHit, rbEntity.position, canChange);
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

        CalculateSphereGravity(rbEntity.position, true);

        //if angle hitInfo.normal eet notre gravity est pas bonne,
        //dire de ne pas ground ! return false !
        //else, angle ok, return true !
        float dotDiff = ExtQuaternion.DotProduct(normalHit, sphereGravity);
        if (dotDiff > marginDotGA)
        {
            //Debug.Log("ok normal correct for moving...");
            return (true);
        }
        //Debug.Log("here we... have bad normal ! don't walk...");
        Debug.DrawRay(rbEntity.position, normalHit * 5, Color.red);
        Debug.DrawRay(rbEntity.position, sphereGravity * 5, Color.black);

        //Debug.Break();
        return (false);
    }

    public void TryToSetNewGravityAttractor(Transform obj)
    {
        //bool hasTag = obj.gameObject.HasTag(tagWithAttractor);
        //GravityAttractor tmpGravity = obj.gameObject.GetComponentInAllParentsWithTag<GravityAttractor>(tagWithAttractor, 3, true);
        Debug.Log("obj hit: " + obj);
        GravityAttractor tmpGravity = obj.GetComponentInParent<GravityAttractor>();

        //if there is a gravityAttactor, but wrong layer, do nothing
        if (!IsAirAttractorLayer(obj.gameObject.layer))
            tmpGravity = null;

        if (!gravityAttractor && !tmpGravity)
        {
            Debug.Log("si on a pas d'ancien, et rien de nouveau, ne rien faire");
            return;
        }
        if (!gravityAttractor && tmpGravity)
        {
            Debug.Log("si on a pas d'ancien, mais un nouveau, alors banco");
            SelectNewGA(tmpGravity, false);
            return;
        }
        if (gravityAttractor && tmpGravity)
        {
            Debug.Log("tmpGravity: " + tmpGravity);
            //Debug.Log("obj.gameObject " + obj.gameObject);

            Debug.Log("on passe d'un ancien à un nouveau d'un coup !");
            //Debug.Break();
            if (tmpGravity.GetInstanceID() != gravityAttractor.GetInstanceID())
            {
                UnselectOldGA();
                SelectNewGA(tmpGravity, true);
            }
            //Debug.Break();
            return;
        }
        if (gravityAttractor && !tmpGravity)
        {
            //Debug.Log("on passe d'un ancien à rien !");
            UnselectOldGA();
            return;
        }
        Debug.Log("not found ?");
    }

    private void SelectNewGA(GravityAttractor newGA, bool calculateNow)
    {
        gravityAttractor = newGA;
        gravityAttractor.SelectedGravityAttractor();

        gravityAttractorMode = true;

        CalculateSphereGravity(rbEntity.position, calculateNow);
    }

    /// <summary>
    /// leave GA
    /// </summary>
    public void UnselectOldGA()
    {
        gravityAttractor.UnselectGravityAttractor();
        gravityAttractor = null;
        //gravityPoint.Clear();
        gravityAttractorMode = false;
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
            //Debug.Log("ici ?");
            //sphereGravity = groundCheck.GetDirLastNormal();
            sphereGravity = lastNormalJumpChoosen;
        }
        else
        {
            //Debug.Log("ou la ?");
            GravityAttractor.PointInfo tmpPointInfo = gravityAttractor.FindNearestPoint(posEntity);
            if (ExtUtilityFunction.IsNullVector(tmpPointInfo.pos))
            {
                Debug.LogWarning("ici on a pas trouvé de nouvelle gravité... garder comme maintenant ? mettre le compteur de mort ?");
                Debug.DrawRay(posEntity, sphereGravity * -10, Color.red, 5f);
                return;
            }
            pointInfo = tmpPointInfo;
            sphereGravity = (posEntity - pointInfo.pos).normalized;
        }
    }
}
