using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueGravity : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    protected bool isUniqueGravity = true;

    [FoldoutGroup("GamePlay"), Tooltip("gravité du saut"), SerializeField]
    protected float gravity = 9.81f;
    public float Gravity { get { return (gravity); } }
    
    [FoldoutGroup("GamePlay"), Tooltip("default air gravity"), SerializeField]
    protected float defaultGravityInAir = 2f;

    [FoldoutGroup("GamePlay"), Tooltip("default air gravity"), SerializeField]
    protected UniqueGravityAttractorSwitch uniqueGravityAttractorSwitch;

    private Vector3 mainAndOnlyGravity = Vector3.zero;

    public Vector3 GetMainAndOnlyGravity()
    {
        return (mainAndOnlyGravity);
    }
    
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected Rigidbody rb = null;

    public Vector3 CalculateGravity(Vector3 positionEntity)
    {
        mainAndOnlyGravity = uniqueGravityAttractorSwitch.GetDirGAGravity();

        return (mainAndOnlyGravity);
    }

    /// <summary>
    /// apply base air gravity
    /// </summary>
    protected Vector3 AirBaseGravity(Vector3 gravityOrientation, Vector3 positionEntity, float boost = 1)
    {
        Vector3 forceBaseGravityInAir = -gravityOrientation * gravity * (defaultGravityInAir - 1) * boost * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, forceBaseGravityInAir, Color.green, 5f);
        return (forceBaseGravityInAir);
    }

    private void FixedUpdate()
    {
        if (isUniqueGravity)
        {
            rb.velocity += AirBaseGravity(CalculateGravity(rb.position), rb.position);
        }
    }
}
