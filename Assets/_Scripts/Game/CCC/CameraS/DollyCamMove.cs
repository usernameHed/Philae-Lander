using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[TypeInfoBox("Rotate Cam point"), ExecuteInEditMode]
public class DollyCamMove : MonoBehaviour
{
    [FoldoutGroup("Right-Left"), Tooltip(""), SerializeField]
    private FrequencyEase easeRotate = new FrequencyEase();
    [FoldoutGroup("Right-Left"), Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving X")]
    private float deadZoneHoriz = 0.25f;
    [FoldoutGroup("Right-Left"), Tooltip("dobject to rotate"), SerializeField]
    private Transform toRotate = null;

    [FoldoutGroup("Up-Down"), Tooltip(""), SerializeField]
    private float speedDolly = 1f;
    [FoldoutGroup("Up-Down"), Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float deadZoneVerti = 0.3f;
    [FoldoutGroup("Up-Down"), Tooltip("dobject to rotate"), SerializeField]
    private CinemachineVirtualCamera cineWithDolly = null;
    [FoldoutGroup("Up-Down"), Tooltip("dobject to rotate"), SerializeField]
    private CinemachinePath cinemachinePath = null;

    [FoldoutGroup("Zoom-Dezoom"), Tooltip(""), SerializeField]
    private float speedZoom = 1f;
    [FoldoutGroup("Zoom-Dezoom"), MinMaxSlider(0.1f, 5f), Tooltip(""), SerializeField]
    private Vector2 minMaxZoom = new Vector2(0.1f, 5f);
    [FoldoutGroup("Zoom-Dezoom"), Tooltip(""), SerializeField]
    private Transform toScale = null;
    [FoldoutGroup("Zoom-Dezoom"), Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float deadZoneZoom = 0.1f;
    [FoldoutGroup("Zoom-Dezoom"), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private SplineController splineController;



    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerInput playerInput = null;


    [FoldoutGroup("Debug"), Range(0, 1f), Tooltip(""), OnValueChanged("InputDolly"), SerializeField]
    private float currentIndexDolly;
    [FoldoutGroup("Debug"), Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float maxIndexDolly = 1;


    private CinemachineTrackedDolly cineDolly;

    private CinemachineCollider cineCollider;
    public CinemachineCollider GetCineCollider() => cineCollider;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        cineDolly = cineWithDolly.GetCinemachineComponent<CinemachineTrackedDolly>();
        SetAimPosition();
    }

    /// <summary>
    /// rotate left-right
    /// </summary>
    private void InputRotate()
    {
        //start or continue ease rotate
        Vector2 dirInput = playerInput.GetCameraInput();

        //if margin turn is ok for HORIZ move
        if (Mathf.Abs(dirInput.x) >= deadZoneHoriz)
        {
            easeRotate.StartOrContinue();

            float remapedInput = ExtUtilityFunction.Remap(Mathf.Abs(dirInput.x), deadZoneHoriz, 1f, 0f, 1f) * Mathf.Sign(dirInput.x);

            toRotate.Rotate(0, easeRotate.Evaluate() * remapedInput, 0);
        }
        else
        {
            easeRotate.BackToTime();
        }
    }

    /// <summary>
    /// move up-down in the dolly
    /// </summary>
    private void InputDolly()
    {
        if (cineDolly == null)
        {
            Init();
        }

        Vector2 dirInput = playerInput.GetCameraInput();

        if (Mathf.Abs(dirInput.y) >= deadZoneVerti)
        {
            float remapedInput = ExtUtilityFunction.Remap(Mathf.Abs(dirInput.y), deadZoneVerti, 1f, 0f, 1f);
            currentIndexDolly -= speedDolly * remapedInput * Mathf.Sign(dirInput.y) * Time.deltaTime;
            currentIndexDolly = Mathf.Clamp(currentIndexDolly, 0, maxIndexDolly);
        }
        cineDolly.m_PathPosition = currentIndexDolly;
    }

    /// <summary>
    /// tel to zoom
    /// </summary>
    /// <param name="zoomRatio">between -1 and 1</param>
    public void InputZoom(float zoomRatio, float speedRatio = 1f)
    {
        if (Mathf.Abs(zoomRatio) >= deadZoneZoom)
        {
            float remapedInput = ExtUtilityFunction.Remap(Mathf.Abs(zoomRatio), deadZoneVerti, 1f, 0f, 1f);
            float localScaleAllAxis = toScale.localScale.x;
            localScaleAllAxis += speedZoom * remapedInput * speedRatio * Mathf.Sign(zoomRatio) * Time.deltaTime;
            localScaleAllAxis = Mathf.Clamp(localScaleAllAxis, minMaxZoom.x, minMaxZoom.y);

            toScale.localScale = new Vector3(localScaleAllAxis, localScaleAllAxis, localScaleAllAxis);

            SetAimPosition();
        }
    }

    private void SetAimPosition()
    {
        float localScaleAllAxis = toScale.localScale.x;
        float max = minMaxZoom.y - minMaxZoom.x;
        float percent = ((localScaleAllAxis - minMaxZoom.y) * 100 / max) + 100;

        percent = Mathf.Clamp(percent / 100f, 0f, 1f);
        splineController.CurvePercent = percent;
        splineController.CustomUpdate();
    }

    private void Update()
    {
        InputZoom(playerInput.trigger);

        InputRotate();
        InputDolly();
        
    }
}
