using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Add target to camera
/// <summary>
public class CameraTarget : MonoBehaviour
{
	[FoldoutGroup("GamePlay"), SerializeField]
	private bool onEnableAdd = true;

	[FoldoutGroup("GamePlay"), SerializeField]
	private bool onDisableRemove = true;

    [FoldoutGroup("GamePlay"), SerializeField]
    private bool alwaysOnCamera = false;


    [FoldoutGroup("Debug"), SerializeField, ReadOnly]
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
