using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastForwardOrientationLD : MonoBehaviour
{
    [Header("Chose orientation: object or direction")]
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("")]
    protected bool gravityTowardDirection = true;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("")]
    private bool automatic = true;
    public bool IsAutomatic() => automatic;

    

    [Space(10)]

    [FoldoutGroup("Debug"), SerializeField, Tooltip("")]
    public bool createMode = true;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("")]
    public Transform referenceObjecct;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("")]
    public Transform objectToTarget;

    //public Vector3 directionGravity;

    [ContextMenu("ResetDirection")]
    public void ResetDirection()
    {
        objectToTarget.position = referenceObjecct.transform.position + Vector3.up;
    }

    public Vector3 GetGravityDirection(Vector3 objPos)
    {
        if (gravityTowardDirection)
            return (referenceObjecct.transform.position - objectToTarget.position);

        return (objPos - objectToTarget.position);
    }

    public Vector3 GetPosition()
    {
        return (objectToTarget.position);
    }

    private void OnDrawGizmos()
    {
        if (!gravityTowardDirection)
        {
            Gizmos.color = Color.green;
            ExtDrawGuizmos.DrawCross(GetPosition());
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(GetPosition(), Vector3.one);
        }

        
        //Gizmos.DrawRay(new Ray(referenceObjecct.positionGetGravityDirection(referenceObjecct.position)));
        Gizmos.DrawLine(referenceObjecct.position, GetPosition());
    }
}
