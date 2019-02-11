using AiUnity.MultipleTags.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySphereAirMove : MonoBehaviour
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
    private List<Transform> gravityPoint = new List<Transform>();
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

    public bool IsNormalAcceptedIfWeAreInGA(Transform objHit, Vector3 normalHit)
    {
        int isForbidden = ExtList.ContainSubStringInArray(gravityAttractorLayer, LayerMask.LayerToName(objHit.gameObject.layer));
        //here it doen't concern us, say just yes
        if (isForbidden == -1)
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
            Debug.Log("ok normal correct for moving...");
            return (true);
        }
        Debug.Log("here we... have bad normal ! don't walk...");
        Debug.DrawRay(rbEntity.position, normalHit * 5, Color.red);
        Debug.DrawRay(rbEntity.position, sphereGravity * 5, Color.black);

        Debug.Break();
        return (false);
    }

    public void TryToSetNewGravityAttractor(Transform obj)
    {
        //bool hasTag = obj.gameObject.HasTag(tagWithAttractor);
        GravityAttractor tmpGravity = obj.gameObject.GetComponentInAllParentsWithTag<GravityAttractor>(tagWithAttractor, 3, true);

        if (!gravityAttractor && !tmpGravity)
        {
            Debug.Log("si on a pas d'ancien, et rien de nouveau, ne rien faire");
            return;
        }
        if (!gravityAttractor && tmpGravity)
        {
            Debug.Log("si on a pas d'ancien, mais un nouveau, alors banco");
            SelectNewGA(tmpGravity);
            gravityAttractorMode = true;
            return;
        }
        if (gravityAttractor && tmpGravity)
        {
            Debug.Log("on passe d'un ancien à un nouveau d'un coup !");
            if (tmpGravity.GetInstanceID() != gravityAttractor.GetInstanceID())
            {
                UnselectOldGA();
                SelectNewGA(tmpGravity);
            }
            return;
        }
        if (gravityAttractor && !tmpGravity)
        {
            Debug.Log("on passe d'un ancien à rien !");
            UnselectOldGA();
            gravityAttractorMode = false;
            return;
        }
    }

    private void SelectNewGA(GravityAttractor newGA)
    {
        gravityAttractor = newGA;
        gravityAttractor.SelectedGravityAttractor();
        gravityPoint = gravityAttractor.GetPoint(rbEntity);
    }

    private void UnselectOldGA()
    {
        gravityAttractor.UnselectGravityAttractor();
        gravityAttractor = null;
        gravityPoint = null;
    }

    private void CalculateSphereGravity()
    {
        gravityPoint = gravityAttractor.GetPoint(rbEntity);
        sphereGravity = (rbEntity.position - gravityPoint[0].position).normalized;
    }

    private void FixedUpdate()
    {
        if (gravityAttractor) //TODO: checker uniquement si on a bougé ??
            CalculateSphereGravity();
    }
}
