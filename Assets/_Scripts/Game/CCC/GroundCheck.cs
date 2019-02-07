using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("check if player in the ground")]
public class GroundCheck : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Range(0f, 2f), Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
    private float groundCheckDistance = 0.8f;
    [FoldoutGroup("GamePlay"), Range(0f, 2f), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private float stickToFloorDist = 1.4f;
    [FoldoutGroup("GamePlay"), Range(1f, 1.5f), Tooltip("When almostGrounded, add more gravity to quicly fall down (if there is something close)"), SerializeField]
    public float stickGravityForce = 1.1f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public float sizeRadiusRayCast = 0.5f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] noPhysicsLayer = new string[] { "Walkable/Dont" };


    [FoldoutGroup("Object"), SerializeField]
    private SphereCollider sphereCollider;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private PlayerGravity playerGravity;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityJump entityJump;
    [FoldoutGroup("Object"), Tooltip("rigidbody"), SerializeField]
    private EntityController entityController;

    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isGrounded = false;
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isAlmostGrounded = false;
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private bool isFlying = true;
    
    [FoldoutGroup("Debug"), ReadOnly, SerializeField]
    private string currentFloorLayer;
    [FoldoutGroup("Debug"), Tooltip("reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)"), SerializeField]
    public float shellOffset = 0.1f;

    private float radius;
    private Vector3 dirNormal = Vector3.zero;
    private Transform lastPlatform = null;
    public Transform GetLastPlatform() { return (lastPlatform); }

    private void Awake()
    {
        isGrounded = isAlmostGrounded = false;
        isFlying = true;
        radius = sphereCollider.radius;
    }

    public bool IsSafeGrounded()
    {
        return (!isFlying);
    }
    public bool IsFlying()
    {
        return (isFlying);
    }
    public bool IsAlmostGrounded()
    {
        return (isAlmostGrounded && !isGrounded && !isFlying);
    }
    
    public Vector3 GetDirLastNormal()
    {
        return (dirNormal);
    }

    public void SetForwardWall(Vector3 hitNormal)
    {
        dirNormal = hitNormal;
        isGrounded = true;
        isFlying = false;
    }

    /// <summary>
    /// Set isGrounded
    /// sphere cast down just beyond the bottom of the capsule to see
    /// if the capsule is colliding round the bottom
    /// </summary>
    private void GroundChecking()
    {
        //isGrounded = false;
        //return;

        RaycastHit hitInfo;

        //Vector3 dirRaycast = playerGravity.GetMainAndOnlyGravity() * (radius + groundCheckDistance);
        //Debug.DrawRay(rb.transform.position, dirRaycast * -1, Color.blue, 0.1f);
        if (Physics.SphereCast(rb.transform.position, sizeRadiusRayCast, playerGravity.GetMainAndOnlyGravity() * -0.01f, out hitInfo,
                               groundCheckDistance, entityController.layerMask, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;

            int isForbidden = ExtList.ContainSubStringInArray(noPhysicsLayer, LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
            if (isForbidden != -1)
            {
                //here we are on a ground with no real gravity
            }
            else
            {
                dirNormal = hitInfo.normal;
            }

            //ExtDrawGuizmos.DebugWireSphere(rb.transform.position + (playerGravity.GetMainAndOnlyGravity() * -0.01f) * (stickToFloorDist), Color.red, sizeRadiusRayCast, 3f);
            //Debug.DrawRay(rb.transform.position, (playerGravity.GetMainAndOnlyGravity() * -0.01f) * (stickToFloorDist), Color.red, 5f);
            //ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.red, 0.1f, 3f);

            //m_GroundContactNormal = hitInfo.normal;
            lastPlatform = hitInfo.collider.transform;
            currentFloorLayer = LayerMask.LayerToName(hitInfo.collider.gameObject.layer);
            //Debug.Log("normal");
            //Debug.Break();
        }
        else
        {
            isGrounded = false;
            dirNormal = playerGravity.GetMainAndOnlyGravity() * 1;
            //m_GroundContactNormal = Vector3.up;
        }
        
    }
    

    /// <summary>
    /// try to stick to floor if the floor is flat, and we juste 
    /// </summary>
    private void StickToGroundHelper()
    {
        RaycastHit hitInfo;

        if (Physics.SphereCast(rb.transform.position, sizeRadiusRayCast, playerGravity.GetMainAndOnlyGravity() * -0.01f, out hitInfo,
                               stickToFloorDist, entityController.layerMask, QueryTriggerInteraction.Ignore))
        {
            isAlmostGrounded = true;
            currentFloorLayer = LayerMask.LayerToName(hitInfo.collider.gameObject.layer);

            int isForbidden = ExtList.ContainSubStringInArray(noPhysicsLayer, LayerMask.LayerToName(hitInfo.transform.gameObject.layer));
            if (isForbidden != -1)
            {
                //here we are almost grounded on ground with no real gravity, don't change gravity !
            }
            else
            {
                dirNormal = hitInfo.normal;
            }
            lastPlatform = hitInfo.collider.transform;
        }
        else
        {
            isAlmostGrounded = false;
            dirNormal = playerGravity.GetMainAndOnlyGravity() * 1;
        }
    }

    /// <summary>
    /// set the drag, and stick to ground if needed
    /// </summary>
    private void SetDragAndStick()
    {
        if (isGrounded)
        {
            isAlmostGrounded = false;
        }
        else
        {
            StickToGroundHelper();
        }
    }

    /// <summary>
    /// set if we are flying or not !
    /// </summary>
    private void SetFlying()
    {
        if (entityJump.IsJumpedAndNotReady())
        {
            isGrounded = false;
            isAlmostGrounded = false;
        }

        if (isGrounded || isAlmostGrounded)
        {
            isFlying = false;
            return;
        }
        isFlying = true;
    }

    private void FixedUpdate()
    {
        GroundChecking();           //set whenever or not we are grounded
        SetDragAndStick();          //set, depending on the grounded, the drag, and stick or not to the floor
        SetFlying();                //set if we fly or not !
    }
}
