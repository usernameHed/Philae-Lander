
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

/// <summary>
/// Add target to camera
/// <summary>
public class CameraTarget : MonoBehaviour
{
	[SerializeField]
	private bool onEnableAdd = true;

	[SerializeField]
	private bool onDisableRemove = true;

    [SerializeField]
    private bool alwaysOnCamera = false;


    [SerializeField, ReadOnly]
    private CameraControllerOld cameraController;

    private void Awake()
    {
        cameraController = PhilaeManager.Instance.cameraController;
    }

    public void SetAlways(bool always)
    {
        //Debug.Log("ici active: " + always);
        if (always != alwaysOnCamera)
        {
            alwaysOnCamera = always;
            if (alwaysOnCamera)
            {
                AddTarget();
            }
            else
            {
                RemoveTarget();
            }
        }
    }

    public void AddTarget()
	{
        cameraController.AddTarget (this);
	}

	public void RemoveTarget()
	{
        if (cameraController)
		{
            cameraController.RemoveTarget (this);
		}
	}

	// Unity functions
	private void OnEnable()
	{
		if (onEnableAdd)
		{
			AddTarget ();
		}
	}

    private void OnDisable()
    {
		if (onDisableRemove)
		{
			RemoveTarget ();
		}
    }

	private void OnDestroy()
	{
		RemoveTarget ();
	}
}
