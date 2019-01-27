using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu()]
public class CameraTypes : SerializedScriptableObject
{
    public enum CameraType
    {
        BASE,
        PLANET_CHANGE,
        ATTRACTOR,
    }

    public interface ICAM_Type
    {

    }

    [Serializable]
    public struct CAM_BASE : ICAM_Type
    {
        public float DampingMove;
        public float dampingRotateY;
        public float rotateToGroundSpeed;
    }
    [Serializable]
    public struct CAM_PLANET_CHANGE : ICAM_Type
    {
        public float DampingMove;
        public float dampingRotateY;
        public float rotateToGroundSpeed;
    }
    [Serializable]
    public struct CAM_ATTRACTOR : ICAM_Type
    {
        public float DampingMove;
        public float dampingRotateY;
        public float rotateToGroundSpeed;
    }

    [Tooltip(""), SerializeField]
    public CameraType camType = CameraType.BASE;

    [Tooltip(""), SerializeField]
    private List<ICAM_Type> camTypes;

    public float GetDampingMove()
    {
        switch (camType)
        {  
            case CameraType.PLANET_CHANGE:
                CAM_PLANET_CHANGE cam2 = (CAM_PLANET_CHANGE)camTypes[1];
                return (cam2.DampingMove);

            case CameraType.ATTRACTOR:
                CAM_ATTRACTOR cam3 = (CAM_ATTRACTOR)camTypes[2];
                return (cam3.DampingMove);

            case CameraType.BASE:
            default:
                CAM_BASE cam = (CAM_BASE)camTypes[0];
                return (cam.DampingMove);
        }
    }


    public float GetDampingRotateY()
    {
        switch (camType)
        {
            case CameraType.PLANET_CHANGE:
                CAM_PLANET_CHANGE cam2 = (CAM_PLANET_CHANGE)camTypes[1];
                return (cam2.dampingRotateY);

            case CameraType.ATTRACTOR:
                CAM_ATTRACTOR cam3 = (CAM_ATTRACTOR)camTypes[2];
                return (cam3.dampingRotateY);

            case CameraType.BASE:
            default:
                CAM_BASE cam = (CAM_BASE)camTypes[0];
                return (cam.dampingRotateY);
        }
    }

    public float GetRotateSpeedRotate()
    {
        switch (camType)
        {
            case CameraType.PLANET_CHANGE:
                CAM_PLANET_CHANGE cam2 = (CAM_PLANET_CHANGE)camTypes[1];
                return (cam2.rotateToGroundSpeed);

            case CameraType.ATTRACTOR:
                CAM_ATTRACTOR cam3 = (CAM_ATTRACTOR)camTypes[2];
                return (cam3.rotateToGroundSpeed);

            case CameraType.BASE:
            default:
                CAM_BASE cam = (CAM_BASE)camTypes[0];
                return (cam.rotateToGroundSpeed);
        }
    }
}
