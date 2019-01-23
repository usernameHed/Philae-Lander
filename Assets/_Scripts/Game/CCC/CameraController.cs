﻿using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

/// <summary>
/// Manage camera
/// Smoothly move camera to m_DesiredPosition
/// m_DesiredPosition is the barycenter of target list
/// </summary>

public class CameraController : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip("marge de précision de la caméra sur sa cible"), SerializeField]
    private float closeMargin = 0.1f;
    [FoldoutGroup("GamePlay"), Tooltip("marge de précision de la caméra sur sa cible"), SerializeField]
    private float dampingMove = 1f;
    [FoldoutGroup("GamePlay"), Tooltip("marge de précision de la caméra sur sa cible"), SerializeField]
    private float dampingRotateY = 1f;

    [FoldoutGroup("Object"), Tooltip("ref de la camera"), SerializeField]
    private Camera cameraRef;
    [FoldoutGroup("Object"), Tooltip("ref de l'objet déplacable de la caméra"), SerializeField]
    private Transform movingCamera;
    [FoldoutGroup("Object"), Tooltip("ref de la rotation sur l'axe Y de la caméra"), SerializeField]
    private Transform rotateCameraY;

    //Target list
    [FoldoutGroup("Debug"), Tooltip("list de target"), SerializeField, ReadOnly]
    private List<CameraTarget> targetList = new List<CameraTarget>();
    [FoldoutGroup("Debug"), Tooltip("list de target"), SerializeField]
    private Transform targetPosition;
    [FoldoutGroup("Debug"), Tooltip("list de target"), SerializeField]
    private Transform targetToLook;

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
        InitializeCamera();
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

    /// <summary>
    /// Remove target from target list
    /// </summary>
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

    /// <summary>
    /// Clear targets
    /// </summary>
    public void ClearTarget()
    {
        targetList.Clear();
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
        movingCamera.position = Vector3.SmoothDamp(movingCamera.position, targetPosition.position, ref currentVelocity, dampingMove);
    }

    private void RotateCamera()
    {
        //Vector3 dirOrientation = camPointWhoRotate.localEulerAngles;// Quaternion.ToEulerAngles(camPointWhoRotate.rotation);
        Vector3 dirOrientation = targetToLook.position - movingCamera.position;
        Debug.DrawRay(rotateCameraY.position, dirOrientation, Color.red, 0.3f);

        // Preserve our current up direction
        Vector3 up = rotateCameraY.up;

        // Form a rotation facing the desired direction while keeping our
        // local up vector exactly matching the current up direction.
        Quaternion desiredOrientation = ExtQuaternion.TurretLookRotation(dirOrientation, up);

        // Move toward that rotation at a controlled, even speed regardless of framerate.
        rotateCameraY.rotation = Quaternion.RotateTowards(  
                                rotateCameraY.rotation,
                                desiredOrientation,
                                dampingRotateY * Time.deltaTime);


        //UnityRotateExtensions.Rotate_DegreesPerSecond(rbObject, dirOrientation, speedRotate);
        //Quaternion targetRotation = Quaternion.FromToRotation(rotateCameraY.forward, dirOrientation) * rotateCameraY.rotation;
        //Quaternion targetRotationY = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        //rotateCameraY.rotation = Quaternion.RotateTowards(rotateCameraY.rotation, targetRotation, dampingRotateY);
        //rotateCameraY.localRotation = Quaternion.Slerp(rotateCameraY.localRotation, camPointWhoRotate.localRotation, dampingRotateY);

    }

    private void FixedUpdate()
    {
        MoveCamera();
        RotateCamera();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, GameOver);
    }

}