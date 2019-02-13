using AiUnity.MultipleTags.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityGravityAttractorSwitch : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private string tagWithAttractor = "GravityAttractor";
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] gravityAttractorLayer = new string[] { "Walkable/GravityAttractor" };
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float marginDotGA = 0.86f;

    [FoldoutGroup("Object"), Tooltip(""), SerializeField]
    private Rigidbody rbEntity;

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField]
    private bool gravityAttractorMode = false;
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private List<GravityAttractor.GravityPoint> gravityPoint = new List<GravityAttractor.GravityPoint>();
    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    private GravityAttractor gravityAttractor = null;

    [FoldoutGroup("Debug"), Tooltip(""), SerializeField, ReadOnly]
    public Vector3 sphereGravity = Vector3.zero;

    public bool IsInGravityAttractorMode()
    {
        return (gravityAttractor);
    }

    public Vector3 GetDirGAGravity()
    {
        return (sphereGravity);
    }

    public float GetRatioGravity()
    {
        if (!gravityAttractorMode)
            return (1);
        return (gravityPoint[0].gravityRatio);
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
    public bool CanDoSideJump(Transform objHit)
    {
        bool canDo = !((gravityAttractorMode && IsAirAttractorLayer(objHit.gameObject.layer)));
        Debug.Log("can du: " + canDo);
        return (canDo);
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


            return (true);
        }

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
        GravityAttractor tmpGravity = obj.GetComponentInParent<GravityAttractor>();



        if (!gravityAttractor && !tmpGravity)
        {
            //Debug.Log("si on a pas d'ancien, et rien de nouveau, ne rien faire");
            return;
        }
        if (!gravityAttractor && tmpGravity)
        {
            //Debug.Log("si on a pas d'ancien, mais un nouveau, alors banco");
            SelectNewGA(tmpGravity);
            return;
        }
        if (gravityAttractor && tmpGravity)
        {
            //Debug.Log("tmpGravity: " + tmpGravity);
            //Debug.Log("obj.gameObject " + obj.gameObject);

            //Debug.Log("on passe d'un ancien à un nouveau d'un coup !");
            //Debug.Break();
            if (tmpGravity.GetInstanceID() != gravityAttractor.GetInstanceID())
            {
                UnselectOldGA();
                SelectNewGA(tmpGravity);
            }
            return;
        }
        if (gravityAttractor && !tmpGravity)
        {
            //Debug.Log("on passe d'un ancien à rien !");
            UnselectOldGA();
            return;
        }
        //Debug.Log("not found ?");
    }

    private void SelectNewGA(GravityAttractor newGA)
    {
        gravityAttractor = newGA;
        gravityAttractor.SelectedGravityAttractor();
        gravityPoint = gravityAttractor.GetPoint(rbEntity.position);
        gravityAttractorMode = true;
    }

    /// <summary>
    /// leave GA
    /// </summary>
    public void UnselectOldGA()
    {
        gravityAttractor.UnselectGravityAttractor();
        gravityAttractor = null;
        gravityPoint = null;
        gravityAttractorMode = false;
    }

    public void CalculateSphereGravity(Vector3 posEntity)
    {
        gravityPoint = gravityAttractor.GetPoint(posEntity);
        sphereGravity = (posEntity - gravityPoint[0].point.position).normalized;
    }

    private void FixedUpdate()
    {
        if (gravityAttractor) //TODO: checker uniquement si on a bougé ??
            CalculateSphereGravity(rbEntity.position);
    }
}
