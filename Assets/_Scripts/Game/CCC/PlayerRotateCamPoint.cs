using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotateCamPoint : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float turnRate = 5f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("ref rigidbody")]
    private float marginTurnHoriz = 0.25f;
    [FoldoutGroup("GamePlay"), Range(0f, 1f), SerializeField, Tooltip("ref rigidbody")]
    private float marginTurnVerti = 0.25f;

    [FoldoutGroup("GamePlay"), MinMaxSlider(0.2f, 15f), SerializeField]
    private Vector2 minMaxLength;
    [FoldoutGroup("GamePlay"), Tooltip("ease dezoom"), SerializeField]
    private EasingFunction.Ease easeDezoom;
    [FoldoutGroup("GamePlay"), Tooltip("ease dezoom speed"), SerializeField]
    private float speedEase = 5f;

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
            EasingFunction.Function funcEasing = EasingFunction.GetEasingFunction(easeDezoom);

            if (dirInput.y < 0)
            {
                //float realStickValue = (Mathf.Abs(dirInput.y - marginTurnVerti) * 1 / (1 - marginTurnVerti)) * Mathf.Sign(dirInput.y);
                defaultLenghtCamPointDist = funcEasing(defaultLenghtCamPointDist, minMaxLength.y, dirInput.y);
            }
            else if (dirInput.y > 0)
            {
                
                
                defaultLenghtCamPointDist = funcEasing(defaultLenghtCamPointDist, minMaxLength.x, dirInput.y);
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
