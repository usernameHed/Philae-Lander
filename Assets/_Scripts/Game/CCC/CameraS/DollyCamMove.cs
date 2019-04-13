using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[TypeInfoBox("Rotate Cam point")]
public class DollyCamMove : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private FrequencyEase easeRotate = new FrequencyEase();
    [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
    private float speedDolly = 1f;
    [FoldoutGroup("GamePlay"), MinMaxSlider(0.1f, 5f), Tooltip(""), SerializeField]
    private Vector2 minMaxZoom = new Vector2(0.1f, 5f);

    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving X")]
    private float marginTurnHoriz = 0.25f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float marginTurnVerti = 0.25f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerInput playerInput = null;
    [FoldoutGroup("Object"), Tooltip("dobject to rotate"), SerializeField]
    private CinemachineVirtualCamera cineWithDolly = null;
    [FoldoutGroup("Object"), Tooltip("dobject to rotate"), SerializeField]
    private Transform objectToRotate = null;
    [FoldoutGroup("Object"), Tooltip("dobject to rotate"), SerializeField]
    private CinemachinePath cinemachinePath = null;



    [FoldoutGroup("Debug"), Range(0, 1f), Tooltip(""), OnValueChanged("InputDolly"), SerializeField]
    private float currentIndexDolly;
    [FoldoutGroup("Debug"), Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float maxIndexDolly = 1;


    private CinemachineTrackedDolly cineDolly;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        cineDolly = cineWithDolly.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    /// <summary>
    /// rotate left-right
    /// </summary>
    private void InputRotate()
    {
        //start or continue ease rotate
        Vector2 dirInput = playerInput.GetCameraInput();

        //if margin turn is ok for HORIZ move
        if (Mathf.Abs(dirInput.x) >= marginTurnHoriz)
        {
            easeRotate.StartOrContinue();

            float remapedInput = ExtUtilityFunction.Remap(Mathf.Abs(dirInput.x), marginTurnHoriz, 1f, 0f, 1f) * Mathf.Sign(dirInput.x);

            objectToRotate.Rotate(0, easeRotate.Evaluate() * remapedInput, 0);
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

        if (Mathf.Abs(dirInput.y) >= marginTurnVerti)
        {
            float remapedInput = ExtUtilityFunction.Remap(Mathf.Abs(dirInput.y), marginTurnVerti, 1f, 0f, 1f);
            currentIndexDolly -= speedDolly * remapedInput * Mathf.Sign(dirInput.y);
            currentIndexDolly = Mathf.Clamp(currentIndexDolly, 0, maxIndexDolly);
        }
        cineDolly.m_PathPosition = currentIndexDolly;
    }

    private void InputZoom()
    {
        bool dashUp = playerInput.dashUp;
        bool focusUp = playerInput.focus;

        /*
        if (Mathf.Abs(dirInput.y) >= marginTurnVerti)
        {
            float remapedInput = ExtUtilityFunction.Remap(Mathf.Abs(dirInput.y), marginTurnVerti, 1f, 0f, 1f);
            
        }
        */
    }

    private void Update()
    {
        InputRotate();
        InputDolly();
    }
}
