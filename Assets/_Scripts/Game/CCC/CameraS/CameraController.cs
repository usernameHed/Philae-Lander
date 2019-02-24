using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

/// <summary>
/// Manage camera
/// Smoothly move camera to m_DesiredPosition
/// m_DesiredPosition is the barycenter of target list
/// </summary>

public class CameraController : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("marge de précision de la caméra sur sa cible"), InlineEditor]
    public CameraTypes cameraTypes;// = new CameraTypes();
    public CameraTypes GetCameraType() { return (cameraTypes); }
    [FoldoutGroup("GamePlay"), Tooltip("Base cam value"), SerializeField]
    private CameraTypes.CAM_CURRENT camCurrent;// = new CameraTypes.CAM_CURRENT();

    [FoldoutGroup("GamePlay"), Tooltip("marge de précision de la caméra sur sa cible"), SerializeField]
    private float closeMargin = 0.1f;
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private float autoZoomRatio = 1.05f;
    
    [FoldoutGroup("Object"), Tooltip("ref de l'objet déplacable de la caméra"), SerializeField]
    private Transform movingCamera = null;
    [FoldoutGroup("Object"), Tooltip("ref de la rotation sur l'axe Y de la caméra"), SerializeField]
    private Transform rotateCameraY = null;

    //Target list
    [FoldoutGroup("Debug"), Tooltip("list de target"), SerializeField, ReadOnly]
    private List<CameraTarget> targetList = new List<CameraTarget>();
    [FoldoutGroup("Debug"), Tooltip("list de target"), SerializeField]
    private Transform targetPosition = null;
    [FoldoutGroup("Debug"), Tooltip("list de target"), SerializeField]
    private Transform targetToLook = null;

    [FoldoutGroup("Debug"), Tooltip("list de target"), SerializeField]
    private PlayerRotateCamPoint playerRotateCamPoint = null;
    

    private bool freez = false;

    private Vector3 currentVelocity;


    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        cameraTypes.Init();
        freez = false;
        InitializeCamera();
        SetBaseCamera();
        InitBaseCameraValue();
    }

    public float GetRotateSpeedRotate()
    {
        float value = cameraTypes.GetRotateSpeedRotate(ref camCurrent);
        return (value);
    }

    private void InitBaseCameraValue()
    {
        //cameraRotateToGround.speedRotate = cameraTypes.GetRotateSpeedRotate();
        //currentDampingMove = cameraTypes.GetDampingMove();
    }

    public void SetBaseCamera()
    {
        cameraTypes.camType = CameraTypes.CameraType.BASE;
        Debug.Log("set base camera !");
    }

    public bool IsOnAttractorMode()
    {
        return (cameraTypes.camType == CameraTypes.CameraType.ATTRACTOR);
    }

    public void SetAttractorCamera()
    {
        cameraTypes.camType = CameraTypes.CameraType.ATTRACTOR;
        Debug.Log("set base camera !");
    }

    public void SetChangePlanetCam()
    {
        Debug.Log("set change palnet camera");
        cameraTypes.camType = CameraTypes.CameraType.PLANET_CHANGE;
    }

    /// <summary>
    /// Initialize camera
    /// </summary>
    public void InitializeCamera()
    {
        movingCamera.position = targetPosition.position;
    }

    /// <summary>
    /// fonction appelé lorsque la partie est fini
    /// </summary>
    private void GameOver()
    {
        freez = true;
    }

    /// <summary>
    /// Add target to camera
    /// </summary>
    public void AddTarget(CameraTarget other)
    {
		// Check object is not already a target
        if (targetList.IndexOf(other) < 0)
        {
            targetList.Add(other);
        }
    }

	public void RemoveTarget(CameraTarget other)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i].GetInstanceID() == other.GetInstanceID())
            {
                targetList.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    /// clean la list des targets
    /// </summary>
    private void CleanListTarget()
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (!targetList[i])
                targetList.RemoveAt(i);
        }
    }

    public void ClearTarget()
    {
        targetList.Clear();
    }

    /// <summary>
    /// move faster when zclip
    /// </summary>
    /// <returns></returns>
    public float GetAutoZoomRatio()
    {
        if (playerRotateCamPoint.IsInsideSomething())
            return (autoZoomRatio);
        return (1);
    }

    /// <summary>
    /// Smoothly move camera toward targetPosition
    /// </summary>
    private void MoveCamera()
    {
        if (ExtUtilityFunction.HasReachedTargetPosition(movingCamera.position, targetPosition.position, closeMargin))
        {
            return;
        }
        movingCamera.position = Vector3.SmoothDamp(movingCamera.position, targetPosition.position, ref currentVelocity, cameraTypes.GetDampingMove(ref camCurrent) * GetAutoZoomRatio());
    }

    private void RotateCamera()
    {
        Vector3 dirOrientation = targetToLook.position - movingCamera.position;
        //Debug.DrawRay(rotateCameraY.position, dirOrientation, Color.red, 0.3f);

        // Preserve our current up direction
        Vector3 up = rotateCameraY.up;

        // Form a rotation facing the desired direction while keeping our
        // local up vector exactly matching the current up direction.
        Quaternion desiredOrientation = ExtQuaternion.TurretLookRotation(dirOrientation, up);

        // Move toward that rotation at a controlled, even speed regardless of framerate.
        rotateCameraY.rotation = Quaternion.RotateTowards(  
                                rotateCameraY.rotation,
                                desiredOrientation,
                                cameraTypes.GetDampingRotateY(ref camCurrent) * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (freez)
            return;

        MoveCamera();
        RotateCamera();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, GameOver);
    }

}