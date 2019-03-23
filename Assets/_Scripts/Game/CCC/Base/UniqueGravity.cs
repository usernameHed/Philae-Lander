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
    [FoldoutGroup("Air Gravity"), Tooltip("Down gravity when we are falling into the planet"), SerializeField]
    protected float rbDownAddGravity = 3.5f;

    [FoldoutGroup("Switch"), SerializeField, Tooltip("down a partir du moment ou on est donw la premiere fois")]
    protected bool doWeSwitchBetweenBoth = true;
    [FoldoutGroup("Switch"), SerializeField, Tooltip("up or down selon la normal dot"), ReadOnly]
    protected bool isGoingDown = false;
    public bool IsGoingDown() => isGoingDown;
    [FoldoutGroup("Switch"), SerializeField, Tooltip("down a partir du moment ou on est donw la premiere fois"), ReadOnly]
    protected bool isGoingDownToGround = false;

    public bool IsGoingDownToGround()
    {
        return (isGoingDownToGround);
    }

    public virtual void OnGrounded()
    {
        isGoingDown = isGoingDownToGround = false;
    }

    public virtual void JustJumped()
    {
        isGoingDown = isGoingDownToGround = false;
    }

    [FoldoutGroup("GamePlay"), Tooltip("default air gravity"), SerializeField]
    protected UniqueGravityAttractorSwitch uniqueGravityAttractorSwitch;

    protected Vector3 mainAndOnlyGravity = Vector3.zero;

    public Vector3 GetMainAndOnlyGravity()
    {
        return (mainAndOnlyGravity);
    }
    
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    protected Rigidbody rb = null;

    public virtual Vector3 CalculateGravity(Vector3 positionEntity)
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

    /// <summary>
    /// here we fall down toward a planet, apply gravity down
    /// </summary>
    protected Vector3 AirAddGoingDown(Vector3 gravityOrientation, Vector3 positionEntity)
    {
        //Debug.Log("ici down ?");
        Vector3 orientationDown = -gravityOrientation * gravity * (rbDownAddGravity - 1) * Time.fixedDeltaTime;
        Debug.DrawRay(positionEntity, orientationDown, Color.blue, 5f);
        return (orientationDown);
    }

    protected void SetGoingDown()
    {
        float dotGravityRigidbody = ExtQuaternion.DotProduct(GetMainAndOnlyGravity(), rb.velocity);
        if (dotGravityRigidbody < 0 || (isGoingDown && !doWeSwitchBetweenBoth))
        {
            //first time falling
            if (!isGoingDown)
            {
                isGoingDown = true;
            }
            if (!isGoingDownToGround)
            {
                isGoingDownToGround = true;
            }
        }
        else
        {
            isGoingDown = false;
        }
    }

    /// <summary>
    /// return the right gravity
    /// </summary>
    /// <returns></returns>
    protected virtual Vector3 FindAirGravity(Vector3 positionObject, Vector3 rbVelocity, Vector3 gravityOrientation)
    {
        Vector3 finalGravity = rbVelocity;

        finalGravity += AirBaseGravity(gravityOrientation, positionObject, uniqueGravityAttractorSwitch.GetAirRatioGravity());

        if (isGoingDown)
        {
            finalGravity += AirAddGoingDown(gravityOrientation, positionObject) * uniqueGravityAttractorSwitch.GetRatioGravityDown();
        }
        
        return (finalGravity);
    }

    private void FixedUpdate()
    {
        if (isUniqueGravity)
        {
            CalculateGravity(rb.position);
            SetGoingDown();
            rb.velocity = FindAirGravity(rb.position, rb.velocity, GetMainAndOnlyGravity());
        }
    }
}
