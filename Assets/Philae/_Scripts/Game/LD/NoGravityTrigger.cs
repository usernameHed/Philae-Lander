
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kill any IKillable instance on contact
/// <summary>
public class NoGravityTrigger : MonoBehaviour
{
    [SerializeField, Tooltip("enum to interact")]
    public float ratioGravity = 0f;

    //[SerializeField, Tooltip("enum to interact")]
    //private List<TagAccess.TagAccessEnum> tagList = new List<TagAccess.TagAccessEnum>() { TagAccess.TagAccessEnum.Player, TagAccess.TagAccessEnum.Enemy};

    [SerializeField]
    private SphereCollider sphereCollider = null;
    /*
    private void OnTriggerEnter(Collider other)
    {
		if (ExtList.ListContain(other.tag, tagList))
		{
            TriggerController entityNoGravity = other.gameObject.GetComponent<TriggerController>();
            if (entityNoGravity)
            {
                entityNoGravity.entityTriggerManager.entityNoGravity.EnterInZone(this);
            }
		}
    }

	private void OnTriggerExit(Collider other)
	{
		if (ExtList.ListContain(other.tag, tagList))
		{
            TriggerController entityNoGravity = other.gameObject.GetComponent<TriggerController>();
            if (entityNoGravity)
            {
                entityNoGravity.entityTriggerManager.entityNoGravity.LeanInZone(this);
            }
        }
	}
    */

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        float radius = 1.5f;
        if (sphereCollider)
            radius = sphereCollider.radius;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    
}
