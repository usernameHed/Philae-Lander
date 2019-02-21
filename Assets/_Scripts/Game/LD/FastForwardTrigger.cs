using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kill any IKillable instance on contact
/// <summary>
public class FastForwardTrigger : FastForwardOrientationLD
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("enum to interact")]
    private List<TagAccess.TagAccessEnum> tagList = new List<TagAccess.TagAccessEnum>() { TagAccess.TagAccessEnum.Player, TagAccess.TagAccessEnum.Enemy};

    [SerializeField]
    private SphereCollider sphereCollider;

    private void OnTriggerEnter(Collider other)
    {
		if (ExtEnum.ListContain(other.tag, tagList))
		{
            TriggerController triggerController = other.gameObject.GetComponent<TriggerController>();
            if (triggerController)
            {
                triggerController.entityTriggerManager.fastForward.EnterInZone(this);
            }
        }
    }

	private void OnTriggerExit(Collider other)
	{
		if (ExtEnum.ListContain(other.tag, tagList))
		{
            TriggerController triggerController = other.gameObject.GetComponent<TriggerController>();
            if (triggerController)
            {
                triggerController.entityTriggerManager.fastForward.LeanInZone(this);
            }
        }
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float radius = 1.5f;
        if (sphereCollider)
            radius = sphereCollider.radius;
        Gizmos.DrawWireSphere(transform.position, radius);

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
