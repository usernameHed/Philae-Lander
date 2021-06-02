using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

[CreateAssetMenu()]
public class CameraTypes : ScriptableObject
{
    public enum CameraType
    {
        BASE = 0,
        PLANET_CHANGE = 1,
        ATTRACTOR = 2,
    }

    #region CAM Type
    public interface ICAM_Type
    {

    }

    [Serializable]
    public struct CAM_CURRENT : ICAM_Type
    {
        //[ReadOnly]
        //public float speedLerpToUs;
        [ReadOnly]
        public float DampingMove;
        [ReadOnly]
        public float dampingRotateY;
        [ReadOnly]
        public float rotateToGroundSpeed;
    }

    [Serializable]
    public struct CAM_BASE : ICAM_Type
    {
        public float speedLerpToUs;
        public float DampingMove;
        public float dampingRotateY;
        public float rotateToGroundSpeed;
    }
    [Serializable]
    public struct CAM_PLANET_CHANGE : ICAM_Type
    {
        public float speedLerpToUs;
        public float DampingMove;
        public float dampingRotateY;
        public float rotateToGroundSpeed;
    }
    [Serializable]
    public struct CAM_ATTRACTOR : ICAM_Type
    {
        public float speedLerpToUs;
        public float DampingMove;
        public float dampingRotateY;
        public float rotateToGroundSpeed;
    }

    private CAM_BASE camBase;
    private CAM_PLANET_CHANGE camPlanetChange;
    private CAM_ATTRACTOR camAttractor;
    #endregion

    public void Init()
    {
        camBase = (CAM_BASE)camTypes[0];
        camPlanetChange = (CAM_PLANET_CHANGE)camTypes[1];
        camAttractor = (CAM_ATTRACTOR)camTypes[2];
    }

    [Tooltip(""), SerializeField]
    public CameraControllerOld cameraController;

    [Tooltip(""), SerializeField]
    public CameraType camType;// = CameraType.BASE;

    [Tooltip(""), SerializeField]
    public List<ICAM_Type> camTypes;

    public float GetDampingMove(ref CAM_CURRENT camCurrent)
    {
        switch (camType)
        {  
            case CameraType.PLANET_CHANGE:
                camCurrent.DampingMove = Mathf.Lerp(camCurrent.DampingMove, camPlanetChange.DampingMove, camPlanetChange.speedLerpToUs * Time.deltaTime);
                return (camCurrent.DampingMove);

            case CameraType.ATTRACTOR:
                camCurrent.DampingMove = Mathf.Lerp(camCurrent.DampingMove, camAttractor.DampingMove, camAttractor.speedLerpToUs * Time.deltaTime);
                return (camCurrent.DampingMove);

            case CameraType.BASE:
            default:
                camCurrent.DampingMove = Mathf.Lerp(camCurrent.DampingMove, camBase.DampingMove, camBase.speedLerpToUs * Time.deltaTime);
                return (camCurrent.DampingMove);
        }
    }

    public float GetDampingRotateY(ref CAM_CURRENT camCurrent)
    {
        switch (camType)
        {
            case CameraType.PLANET_CHANGE:
                camCurrent.dampingRotateY = Mathf.Lerp(camCurrent.dampingRotateY, camPlanetChange.dampingRotateY, camPlanetChange.speedLerpToUs * Time.deltaTime);
                return (camCurrent.dampingRotateY);

            case CameraType.ATTRACTOR:
                camCurrent.dampingRotateY = Mathf.Lerp(camCurrent.dampingRotateY, camAttractor.dampingRotateY, camAttractor.speedLerpToUs * Time.deltaTime);
                return (camCurrent.dampingRotateY);

            case CameraType.BASE:
            default:
                camCurrent.dampingRotateY = Mathf.Lerp(camCurrent.dampingRotateY, camBase.dampingRotateY, camBase.speedLerpToUs * Time.deltaTime);
                return (camCurrent.dampingRotateY);
        }
    }

    public float GetRotateSpeedRotate(ref CAM_CURRENT camCurrent)
    {
        switch (camType)
        {
            case CameraType.PLANET_CHANGE:
                camCurrent.rotateToGroundSpeed = Mathf.Lerp(camCurrent.rotateToGroundSpeed, camPlanetChange.rotateToGroundSpeed, camPlanetChange.speedLerpToUs * Time.deltaTime);
                return (camCurrent.rotateToGroundSpeed);

            case CameraType.ATTRACTOR:
                camCurrent.rotateToGroundSpeed = Mathf.Lerp(camCurrent.rotateToGroundSpeed, camAttractor.rotateToGroundSpeed, camAttractor.speedLerpToUs * Time.deltaTime);
                return (camCurrent.rotateToGroundSpeed);

            case CameraType.BASE:
            default:
                camCurrent.rotateToGroundSpeed = Mathf.Lerp(camCurrent.rotateToGroundSpeed, camBase.rotateToGroundSpeed, camBase.speedLerpToUs * Time.deltaTime);
                return (camCurrent.rotateToGroundSpeed);
        }
    }
}
