using UnityEngine;

/// <summary>
/// Kill any IKillable instance on contact
/// <summary>
public class KillTrigger : MonoBehaviour
{
	#region Attributes
	[SerializeField]
	private bool killOnEnter = true;

	[SerializeField]
	private bool killOnExit = false;
	#endregion

    #region Core

    private void OnTriggerEnter(Collider other)
    {
		if (killOnEnter)
		{
			TryKill (other.gameObject);
		}
    }

	private void OnTriggerExit(Collider other)
	{
		if (killOnExit)
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

    #endregion
}
