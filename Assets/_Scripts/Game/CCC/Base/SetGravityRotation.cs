
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

[ExecuteInEditMode]
public class SetGravityRotation : MonoBehaviour
{
    [Tooltip("gravité du saut"), SerializeField]
    private bool autoRotate = true;

    [Tooltip("gravité du saut"), SerializeField]
    private Transform objToRotate = default;
    [Tooltip("gravité du saut"), SerializeField, ReadOnly]
    public PhilaeManager philaeManager;

    
    public void SetGravityRotate()
    {
        if (!autoRotate)
            return;

        Debug.Log("rotate to gravity with static findGravity function !");
        GravityAttractorLD.PointInfo point = ExtGetGravityAtPoints.GetAirSphereGravityStatic(objToRotate.position, philaeManager.ldManager.allGravityAttractorLd);

        //Debug.DrawLine(objToRotate.position, point.posRange, Color.red);
        RotateToGround.InstantRotateObject(objToRotate.position - point.posRange, objToRotate);
    }
}
