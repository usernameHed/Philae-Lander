using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class AimFollowerController : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("")]
    private float speedAim = 50f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("")]
    private EntityController entityController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("")]
    private Transform objToMove;
    [FoldoutGroup("Object"), SerializeField, Tooltip("")]
    private PlayerController _playerController;
    [FoldoutGroup("Object"), SerializeField, Tooltip("")]
    private EntityRaycastForward playerRaycastForward;

    [FoldoutGroup("SphereCast"), Tooltip(""), Range(0, 2), SerializeField]
    public float sizeRadiusSphereCast = 0.3f;

    private int layerMask = Physics.AllLayers;
    private InfoHit allInfoHit = new InfoHit();

    public struct InfoHit
    {
        public Vector3 toGo;
        public bool hasHit;
        public RaycastHit hitInfo;
    }

    public int GetLayerMaskRaycast()
    {
        layerMask = LayerMask.GetMask(entityController.allWalkablePlatform);
        return (layerMask);
    }

    private bool DoRayCastForward(Vector3 posA, Vector3 posB, Vector3 focusDir, float dist)
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(posA, sizeRadiusSphereCast, focusDir, out hitInfo,
                               dist, GetLayerMaskRaycast(), QueryTriggerInteraction.Ignore))
        {
            Vector3 centerOnCollision = ExtUtilityFunction.SphereOrCapsuleCastCenterOnCollision(posA, focusDir, hitInfo.distance);

            Debug.DrawLine(posA, centerOnCollision, Color.green);
            ExtDrawGuizmos.DebugWireSphere(centerOnCollision, Color.green, sizeRadiusSphereCast);
            Debug.DrawLine(centerOnCollision, hitInfo.point, Color.green);
            ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.green, 0.1f);

            allInfoHit.toGo = centerOnCollision;
            allInfoHit.hitInfo = hitInfo;
            allInfoHit.hasHit = true;

            return (true);
        }
        else
        {
            Debug.DrawLine(posA, posB, Color.red);
            allInfoHit.toGo = posB;
            allInfoHit.hasHit = false;
            return (false);
        }
    }


    private void Update()
    {
        


        Vector3 posA = objToMove.position;
        Vector3 posB = playerRaycastForward.GetLastPos();

        Vector3 dir = posB - posA;
        float dist = dir.magnitude;
        dir = dir.normalized;

        //DoRayCastForward(objToMove.position, playerRaycastForward.GetLastPos(), dir, dist);

        objToMove.position = Vector3.Lerp(objToMove.position, posB, speedAim * Time.deltaTime);
    }
}
