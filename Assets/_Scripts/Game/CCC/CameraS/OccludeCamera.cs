using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OccludeCamera : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private FrequencyEase easeZoom = new FrequencyEase();
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private float backABit = 0.1f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public string[] allRaycastLayer = new string[] { "Walkable/Ground", "Walkable/Stick", "Walkable/Dont", "Walkable/FastForward" };
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    public TagAccess.TagAccessEnum[] allTagToZoom = new TagAccess.TagAccessEnum[] { TagAccess.TagAccessEnum.ZoomCamera };

    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform camPoint;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private Transform lookAt;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private DollyCamMove dollyCamMove;

    [FoldoutGroup("SphereCast"), Tooltip(""), Range(0, 2), SerializeField]
    public float sizeRadiusSphereCast = 0.3f;

    private int layerMask = Physics.AllLayers;

    public int GetLayerMaskRaycast()
    {
        layerMask = LayerMask.GetMask(allRaycastLayer);
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

            if (ExtLayer.ContainTag(hitInfo.collider.gameObject, allTagToZoom) != -1)
            {
                return (true);
            }
            return (false);
        }
        else
        {
            Debug.DrawLine(posA, posB, Color.red);
            return (false);
        }
    }

    private void Update()
    {
        Vector3 dir = lookAt.position - camPoint.position;
        float dist = dir.magnitude;
        dir = dir.normalized;

        if (DoRayCastForward(camPoint.position - (dir * backABit), lookAt.position, dir, dist))
        {
            easeZoom.StartOrContinue();
            Debug.Log("zoom or conturn ?");

            //dollyCamMove.GetCineCollider().OnTargetObjectWarped
            dollyCamMove.InputZoom(-1, easeZoom.Evaluate());
        }
        else
        {
            easeZoom.BackToTime();
        }
    }
}
