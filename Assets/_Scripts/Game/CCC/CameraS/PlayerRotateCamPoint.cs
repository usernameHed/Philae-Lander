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
    private Vector2 minMaxZoom = new Vector2(2.5f, 15f);
    [FoldoutGroup("Zoom"), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float speedZoom = 5f;
    [FoldoutGroup("Zoom"), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
    private float stepZoom = 1f;
    [FoldoutGroup("Sight"), Tooltip(""), SerializeField]
    private string[] objOnSight = new string[] { "Walkable/Floor", "Walkable/NoSide", "Walkable/Up" };
    [FoldoutGroup("Sight"), Tooltip(""), SerializeField]
    private float radiusRaycast = 0.7f;
    [FoldoutGroup("Sight"), Tooltip("dist min when we need to zoom the camera"), SerializeField]
    private float minDistToZoom = 0.1f;
    [FoldoutGroup("Sight"), Tooltip("dist min when we need to zoom the camera"), SerializeField]
    private float speedBoostZoom = 1f;

    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private Transform mainReferenceObjectDirection = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private PlayerInput playerInput = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref rigidbody")]
    private Rigidbody rb = null;
    [FoldoutGroup("Object"), Tooltip("dobject to rotate"), SerializeField]
    private Transform objectToRotate = null;
    [FoldoutGroup("Object"), Tooltip("point child who rotate"), SerializeField]
    private Transform tpsSpacePoint = null;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref rigidbody")]
    private bool isInsideSomzthing = false;
    [FoldoutGroup("Debug"), SerializeField, Tooltip("ref rigidbody")]
    private bool isFocusing = false;

    [FoldoutGroup("Debug"), SerializeField, Tooltip("default Length CamPoint"), ReadOnly]
    private float defaultLenghtCamPointDist;

    private float oldDefaultLenghtCamPointDist;
    private bool hasZoomed = false;
    private bool isRaulbacking = false;

    private void Start()
    {
        defaultLenghtCamPointDist = (objectToRotate.position - tpsSpacePoint.position).magnitude;
    }

    private void InputRotate()
    {
        Vector2 dirInput = playerInput.GetCameraInput();

        //if margin turn is ok for HORIZ move
        if (Mathf.Abs(dirInput.x) >= marginTurnHoriz)
        {
            objectToRotate.Rotate(0, Time.deltaTime * turnRate * dirInput.x, 0);
        }
    }
    private void InputZoom()
    {
        Vector2 dirInput = playerInput.GetCameraInput();

        if (Mathf.Abs(dirInput.y) >= marginTurnVerti)
        {
            float remapedInput = ExtUtilityFunction.Remap(Mathf.Abs(dirInput.y), marginTurnVerti, 1f, 0f, 1f);
            hasZoomed = isRaulbacking = false;  //cancel raulback

            if (dirInput.y < 0)
            {
                float lerpValue = Mathf.Lerp(defaultLenghtCamPointDist, defaultLenghtCamPointDist + stepZoom, Time.deltaTime * speedZoom * remapedInput);
                float clampValue = Mathf.Clamp(lerpValue, minMaxZoom.x, minMaxZoom.y);

                //float realStickValue = (Mathf.Abs(dirInput.y - marginTurnVerti) * 1 / (1 - marginTurnVerti)) * Mathf.Sign(dirInput.y);

                defaultLenghtCamPointDist = clampValue;
            }
            else if (dirInput.y > 0)
            {
                Zoom(remapedInput);
            }
            ChangePositionPoint();
        }
    }

    private void ChangePositionPoint()
    {
        Vector3 newDistPoint = ((tpsSpacePoint.position - objectToRotate.position).normalized) * defaultLenghtCamPointDist;
        //Debug.DrawRay(objectToRotate.position, newDistPoint, Color.red, 3f);
        tpsSpacePoint.position = objectToRotate.position + newDistPoint;
    }

    private void Zoom(float remapedInput)
    {
        float lerpValue = Mathf.Lerp(defaultLenghtCamPointDist, defaultLenghtCamPointDist - stepZoom, Time.deltaTime * speedZoom * remapedInput);
        float clampValue = Mathf.Clamp(lerpValue, minMaxZoom.x, minMaxZoom.y);
        defaultLenghtCamPointDist = clampValue;
    }

    /// <summary>
    /// tel if we are inside something
    /// </summary>
    /// <returns></returns>
    public bool IsInsideSomething()
    {
        return (isInsideSomzthing);
    }

    private void ZoomIfSometingOnSight()
    {
        Vector2 dirInput = playerInput.GetCameraInput();

        if (Mathf.Abs(dirInput.y) >= marginTurnVerti)
            return;

        RaycastHit hitInfo;

        int layerMask = LayerMask.GetMask(objOnSight);


        Vector3 dirPoint = (tpsSpacePoint.position - objectToRotate.position).normalized;
        float dist = Vector3.Distance(objectToRotate.position, mainReferenceObjectDirection.position);

        if (Physics.SphereCast(rb.transform.position, radiusRaycast, dirPoint, out hitInfo,
                               dist, layerMask, QueryTriggerInteraction.Ignore))
        {
            ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.red, 0.1f, 3f);
            Debug.DrawRay(rb.transform.position, dirPoint.normalized * dist);

            float distPlayerToPoint = Vector3.Distance(hitInfo.point, objectToRotate.position);
            float diffDist = dist - distPlayerToPoint;
            
            if (diffDist > minDistToZoom)
            {
                /*if (!hasZoomed)
                {
                    oldDefaultLenghtCamPointDist = defaultLenghtCamPointDist;
                    hasZoomed = true;
                }*/



                diffDist = Mathf.Clamp(diffDist, 0, 10) / 10;
                isInsideSomzthing = true;
                Zoom(diffDist * speedBoostZoom);
                ChangePositionPoint();
            }
            /*else
            {
                if (hasZoomed && !isRaulbacking)
                {
                    defaultLenghtCamPointDist = oldDefaultLenghtCamPointDist;
                    isRaulbacking = true;
                }
            }*/
        }
    }

    private void Raulbacking()
    {
        if (isRaulbacking)
        {
            Zoom(0.2f);
            ChangePositionPoint();
        }
    }

    private void InputFocus()
    {
        if (playerInput.focus && !isFocusing)
        {
            isFocusing = true;

        }
    }

    private void Update()
    {
        isInsideSomzthing = false;
        

        if (!playerInput.NotMovingCamera())
        {
            InputRotate();
            InputZoom();
        }
        else
        {
            InputFocus();
        }
        ZoomIfSometingOnSight();
        //Raulbacking();
    }
}
