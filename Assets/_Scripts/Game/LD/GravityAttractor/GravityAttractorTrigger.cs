using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kill any IKillable instance on contact
/// <summary>
public class GravityAttractorTrigger : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("enum to interact")]
    private List<TagAccess.TagAccessEnum> tagList = new List<TagAccess.TagAccessEnum>() { TagAccess.TagAccessEnum.Player, TagAccess.TagAccessEnum.Enemy};

    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref of the main GravityAttractor")]
    public GravityAttractorLD refGravityAttractor = null;

    private void OnTriggerStay(Collider other)
    {
		if (ExtEnum.ListContain(other.tag, tagList))
		{
            TriggerController entityNoGravity = other.gameObject.GetComponent<TriggerController>();
            if (entityNoGravity)
            {
                entityNoGravity.entityTriggerManager.entityGravityAttractorSwitch.EnterInZone(refGravityAttractor);
            }
		}
    }

	private void OnTriggerExit(Collider other)
	{
		if (ExtEnum.ListContain(other.tag, tagList))
		{
            TriggerController entityNoGravity = other.gameObject.GetComponent<TriggerController>();
            if (entityNoGravity)
            {
                entityNoGravity.entityTriggerManager.entityGravityAttractorSwitch.LeanInZone(refGravityAttractor);
            }
        }
	}

    private void OnDrawGizmos()
    {
        
    }
    
}
