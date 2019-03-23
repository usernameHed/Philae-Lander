using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Rotate localy the player")]
public class EntityRotate : MonoBehaviour
{
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    private float turnRate = 700f;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    public ExtQuaternion.OrientationRotation CameraFromPlayerOrientation = ExtQuaternion.OrientationRotation.NONE;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    public ExtQuaternion.OrientationRotation InputFromPlayerOrientation = ExtQuaternion.OrientationRotation.NONE;
    [FoldoutGroup("GamePlay"), SerializeField, Tooltip("ref rigidbody")]
    public ExtQuaternion.OrientationRotation InputFromCameraOrientation = ExtQuaternion.OrientationRotation.NONE;

    [FoldoutGroup("GamePlay"), Range(0, 1), SerializeField, Tooltip("ref rigidbody")]
    private float forwardDot = 0.3f;
    [FoldoutGroup("GamePlay"), Range(0, 1), SerializeField, Tooltip("ref rigidbody")]
    private float behindDot = 0.3f;
    [FoldoutGroup("GamePlay"), Range(0, 1), Tooltip("base speed"), SerializeField]
    private float ratioConsideredFullSpeed = 0.5f;


    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityAction entityAction = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private BaseGravity baseGravity = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityMove entityMove;
    [FoldoutGroup("Object"), Tooltip("dobject to rotate"), SerializeField]
    private Transform objectToRotate = null;
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private EntityController entityController = null;

    private Vector3 lastRelativeDirection;  //last desired rotation
    private Vector3 lastPosDir = Vector3.zero;

    private bool isFullSpeedBefore = false;

    private void Start()
    {
        Vector3 normalizedNormalGravity = baseGravity.GetMainAndOnlyGravity();
        lastRelativeDirection = entityController.GetFocusedForwardDirPlayer(normalizedNormalGravity);
        lastPosDir = objectToRotate.position;
    }

    private void RotatePlayer(Vector3 relativeDirection)
    {
        //Debug.DrawRay(objectToRotate.position, relativeDirection, Color.white, 3f);

        // Preserve our current up direction
        // (or you could calculate this as the direction away from the planet's center)
        Vector3 up = objectToRotate.up;
        //Vector3 up = baseGravity.GetMainAndOnlyGravity();

        // Form a rotation facing the desired direction while keeping our
        // local up vector exactly matching the current up direction.
        Quaternion desiredOrientation = ExtQuaternion.TurretLookRotation(relativeDirection, up);

        Debug.DrawRay(objectToRotate.position, relativeDirection, Color.blue);  //normalize ?

        //Debug.DrawRay(objectToRotate.position, entityAction.GetMainReferenceForwardDirection(), Color.red);
        //Debug.DrawRay(objectToRotate.position, objectToRotate.forward, Color.red, 2f);

        // Move toward that rotation at a controlled, even speed regardless of framerate.
        objectToRotate.rotation = Quaternion.RotateTowards(
                                objectToRotate.rotation,
                                desiredOrientation,
                                turnRate * Time.deltaTime);
    }

    private void SetOrientationRotation()
    {
        Vector3 relativeDirectionPlayer = objectToRotate.forward;
        Vector3 relativeInputPlayer = entityAction.GetRelativeDirection();
        Vector3 relativeDirectionCamera = entityAction.GetMainReferenceForwardDirection();
        Vector3 relativeUp = entityAction.GetMainReferenceUpDirection();


        CameraFromPlayerOrientation = ExtQuaternion.IsForwardBackWardRightLeft(objectToRotate.forward, entityAction.GetMainReferenceForwardDirection(), entityAction.GetMainReferenceUpDirection(), objectToRotate.position);
        InputFromPlayerOrientation = ExtQuaternion.IsForwardBackWardRightLeft(objectToRotate.forward, relativeInputPlayer, entityAction.GetMainReferenceUpDirection(), objectToRotate.position);
        InputFromCameraOrientation = ExtQuaternion.IsForwardBackWardRightLeft(relativeDirectionCamera, relativeInputPlayer, entityAction.GetMainReferenceUpDirection(), objectToRotate.position);
    }

    private void SaveLastForward()
    {

    }

    private void FixedUpdate()
    {
        SetOrientationRotation();

        if (entityController.GetMoveState() == PlayerController.MoveState.InAir)
        {
            return;
        }

        if (!entityAction.NotMoving())
        {
            lastPosDir = objectToRotate.position;
            lastRelativeDirection = entityAction.GetRelativeDirection().normalized;


            RotatePlayer(entityAction.GetRelativeDirection());

            Debug.DrawRay(objectToRotate.position, lastRelativeDirection, Color.green, 2f);
            isFullSpeedBefore = false;
        }
        else
        {
            //if we are considered at full speed before, don't turn
            if (entityMove.IsInputRelativeFullSpeed(ratioConsideredFullSpeed))
            {
                lastRelativeDirection = entityAction.GetRelativeDirection().normalized;
                isFullSpeedBefore = true;
            }

            //rotate only if we have moved a very little bit
            if (!isFullSpeedBefore)
            {
                Debug.DrawRay(lastPosDir, lastRelativeDirection, Color.magenta, 2f);
                //Vector3 last = entityAction.GetRelativeDirectionFromManualInput(new Vector2(lastRelativeDirection.x, lastRelativeDirection.z));
                //Vector3 last = entityAction.GetMainReferenceUpDirection() + lastRelativeDirection;
                Vector3 last = lastRelativeDirection;
                //Debug.DrawRay(lastPosDir, last, Color.red, 2f);
                RotatePlayer(last);
            }
        }
        

    }
}
