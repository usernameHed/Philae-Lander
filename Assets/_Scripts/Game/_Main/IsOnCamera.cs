
using UnityEngine;
using UnityEngine.UI;
using UnityEssentials.PropertyAttribute.readOnly;
using UnityEssentials.time;

/// <summary>
/// Check object is on camera
/// <summary>
public class IsOnCamera : MonoBehaviour
{
    [SerializeField]
    private float xMargin = 0;
    [SerializeField]
    private float yMargin = 0;

    [SerializeField]
	private FrequencyTimer updateTimer = new FrequencyTimer(1.0f);

    [SerializeField]
    private Transform objetRef = null;
    [ReadOnly]
    public bool isOnScreen = false;
    [SerializeField, ReadOnly]
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
