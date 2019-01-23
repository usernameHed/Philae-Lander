using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalForces : MonoBehaviour
{
    /// <summary>
    /// don't apply down force when we juste jump
    /// </summary>
    public void JustJumped()
    {
        Debug.Log("just jumped ?");
    }

    private void ApplyGravity()
    {
        /*
        if (gravityApply[0])
        {
            //Debug.Log("ici ceilling");
            ExtRigidbody.ApplyConstForce(rb, -Vector3.up, forceUpWhenCeilling);
        }
        else if (gravityApply[1])
        {
            //Debug.Log("ici gravity");
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (gravityApply[2])
        {
            //Debug.Log("ici low gravity");
            rb.velocity += Vector3.up * Physics.gravity.y * (lowMultiplier - 1) * Time.fixedDeltaTime;
        }
        */
    }

    private void FixedUpdate()
    {
        
    }
}
