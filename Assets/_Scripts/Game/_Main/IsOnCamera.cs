using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Check object is on camera
/// <summary>
public class IsOnCamera : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField]
    private float xMargin = 0;
    [FoldoutGroup("GamePlay"), SerializeField]
    private float yMargin = 0;

    [FoldoutGroup("GamePlay"), SerializeField]
	private FrequencyTimer updateTimer = new FrequencyTimer(1.0f);

    [FoldoutGroup("Object"), SerializeField]
    private Transform objetRef = null;
    [FoldoutGroup("Debug"), ReadOnly]
    public bool isOnScreen = false;
    [FoldoutGroup("Debug"), SerializeField, ReadOnly]
    private Camera cam;

    private Vector3 bounds = Vector3.zero;

    #region Initialization
    private void Start()
	{
        TryToGetCam();
    }
	#endregion

    #region Core

    private bool TryToGetCam()
    {
        cam = GameManager.Instance.cameraMain;
        return (cam != null);
    }

	/// <summary>
	/// Check object is on screen
	/// <summary>
	private void CheckOnCamera()
	{
		if (!cam)
		{
            if (!TryToGetCam())
                return;
		}

		Vector3 bottomCorner = cam.WorldToViewportPoint(objetRef.position - bounds);
		Vector3 topCorner = cam.WorldToViewportPoint(objetRef.position + bounds);

        isOnScreen = (topCorner.x >= -xMargin && bottomCorner.x <= 1 + xMargin && topCorner.y >= -yMargin && bottomCorner.y <= 1 + yMargin);
    }

    // Unity functions
    private void Update()
    {
		if (updateTimer.Ready())
        {
			CheckOnCamera();
        }
    }
	#endregion
}
