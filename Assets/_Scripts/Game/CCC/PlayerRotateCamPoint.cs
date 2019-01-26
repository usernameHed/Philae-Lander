using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Rotate Cam point")]
public class PlayerRotateCamPoint : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("turn rate")]
    private float turnRate = 5f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving X")]
    private float marginTurnHoriz = 0.25f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float marginTurnVerti = 0.25f;

    [FoldoutGroup("Zoom"), MinMaxSlider(2.5f, 15f), SerializeField]
    private Vector2 minMaxZoom;
    [FoldoutGroup("Zoom"), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float speedZoom = 5f;
    [FoldoutGroup("Zoom"), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float stepZoom = 1f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private Transform mainReferenceObjectDirection;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerInput playerInput;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Rigidbody rb;
    [FoldoutGroup("Object"), Tooltip("dobject to rotate"), SerializeField]
    private Transform objectToRotate;
    [FoldoutGroup("Object"), Tooltip("point child who rotate"), SerializeField]
    private Transform tpsSpacePoint;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private GroundCheck groundCheck;

    public float valueEase = 0;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("default Length CamPoint"), ReadOnly]
    private float defaultLenghtCamPointDist;

    private void Start()
    {
        defaultLenghtCamPointDist = (objectToRotate.position - tpsSpacePoint.position).magnitude;
    }

    private void RotateCamPoint()
    {
        Vector2 dirInput = playerInput.GetCameraInput();

        //if margin turn is ok for HORIZ move
        if (Mathf.Abs(dirInput.x) >= marginTurnHoriz)
        {
            objectToRotate.Rotate(0, Time.deltaTime * turnRate * dirInput.x, 0);
        }
        if (Mathf.Abs(dirInput.y) >= marginTurnVerti)
        {
            float remapedInput = ExtUtilityFunction.Remap(Mathf.Abs(dirInput.y), marginTurnVerti, 1f, 0f, 1f);

            if (dirInput.y < 0)
            {
                float lerpValue = Mathf.Lerp(defaultLenghtCamPointDist, defaultLenghtCamPointDist + stepZoom, Time.deltaTime * speedZoom * remapedInput);
                float clampValue = Mathf.Clamp(lerpValue, minMaxZoom.x, minMaxZoom.y);

                //float realStickValue = (Mathf.Abs(dirInput.y - marginTurnVerti) * 1 / (1 - marginTurnVerti)) * Mathf.Sign(dirInput.y);
                
                defaultLenghtCamPointDist = clampValue;
            }
            else if (dirInput.y > 0)
            {
                float lerpValue = Mathf.Lerp(defaultLenghtCamPointDist, defaultLenghtCamPointDist - stepZoom, Time.deltaTime * speedZoom * remapedInput);
                float clampValue = Mathf.Clamp(lerpValue, minMaxZoom.x, minMaxZoom.y);
                defaultLenghtCamPointDist = clampValue;
            }

            Vector3 newDistPoint = ((tpsSpacePoint.position - objectToRotate.position).normalized) * defaultLenghtCamPointDist;
            //Debug.DrawRay(objectToRotate.position, newDistPoint, Color.red, 3f);
            tpsSpacePoint.position = objectToRotate.position + newDistPoint;
        }
    }

    private void Update()
    {
        if (!playerInput.NotMovingCamera())
        {
            RotateCamPoint();
        }
    }
}
