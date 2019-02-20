using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kill any IKillable instance on contact
/// <summary>
public class KillTrigger : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("enum to interact")]
    private List<TagAccess.TagAccessEnum> tagList = new List<TagAccess.TagAccessEnum>() { TagAccess.TagAccessEnum.Player, TagAccess.TagAccessEnum.Enemy };


    [SerializeField]
	private bool killOnEnter = true;

	[SerializeField]
	private bool killOnExit = false;

    [SerializeField]
    private SphereCollider sphereCollider;


    private void OnTriggerEnter(Collider other)
    {
		if (killOnEnter && ExtEnum.ListContain(other.tag, tagList))
		{
			TryKill (other.gameObject);
		}
    }

	private void OnTriggerExit(Collider other)
	{
		if (killOnExit && ExtEnum.ListContain(other.tag, tagList))
		{
			TryKill (other.gameObject);
		}
	}

	private void TryKill(GameObject other)
	{
		IKillable killable = other.GetComponent<IKillable> ();
		if (killable != null)
		{
			killable.Kill ();
		}
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float radius = 1.5f;
        if (sphereCollider)
            radius = sphereCollider.radius;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
