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
    [FoldoutGroup("Object"), SerializeField, Tooltip("ref script")]
    private Transform rotateObject;


    private Vector3 lastVectorRelativeDirection;  //last desired rotation

    private Quaternion lastQuaternionRelativeDirection;  //last desired rotation
    public Vector3 GetLastDesiredDirection() => rotateObject.forward;
    private Vector3 lastPosDir = Vector3.zero;

    private bool isFullSpeedBefore = false;

    private void Start()
    {
        Vector3 normalizedNormalGravity = baseGravity.GetMainAndOnlyGravity();
        lastQuaternionRelativeDirection = GetLastDesiredRotation(entityController.GetFocusedForwardDirPlayer(normalizedNormalGravity), objectToRotate.up);
        lastPosDir = objectToRotate.position;
    }

    private Quaternion GetLastDesiredRotation(Vector3 dirRelativeInput, Vector3 up)
    {
        //Vector3 relativeDirection = entityAction.GetRelativeDirection().normalized;
        // Preserve our current up direction
        // (or you could calculate this as the direction away from the planet's center)
        //Vector3 up = objectToRotate.up;
        //Vector3 up = baseGravity.GetMainAndOnlyGravity();

        // Form a rotation facing the desired direction while keeping our
        // local up vector exactly matching the current up direction.
        return (ExtQuaternion.TurretLookRotation(dirRelativeInput, up));
    }

    private void DoRotate(Quaternion calculatedDir)
    {
        
        //Debug.DrawRay(objectToRotate.position, entityAction.GetMainReferenceForwardDirection(), Color.red);
        //Debug.DrawRay(objectToRotate.position, objectToRotate.forward, Color.red, 2f);

        // Move toward that rotation at a controlled, even speed regardless of framerate.
        objectToRotate.rotation = Quaternion.RotateTowards(
                                objectToRotate.rotation,
                                calculatedDir,
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
            isFullSpeedBefore = true;
            Debug.Log("iiciii ???");
            return;
        }

        if (!entityAction.NotMoving())
        {
            lastPosDir = objectToRotate.position;
            lastVectorRelativeDirection = entityAction.GetRelativeDirection();

            DoRotate(GetLastDesiredRotation(lastVectorRelativeDirection, objectToRotate.up));
            isFullSpeedBefore = false;
        }
        else
        {
            //if we are considered at full speed before, don't turn
            if (entityMove.IsInputRelativeFullSpeed(ratioConsideredFullSpeed))
            {
                //lastVectorRelativeDirection = entityAction.GetRelativeDirection();
                isFullSpeedBefore = true;
            }

            //rotate only if we have moved a very little bit
            if (!isFullSpeedBefore)
            {
                DoRotate(GetLastDesiredRotation(lastVectorRelativeDirection, objectToRotate.up));
            }
        }

        lastQuaternionRelativeDirection = GetLastDesiredRotation(lastVectorRelativeDirection, baseGravity.GetMainAndOnlyGravity());

        rotateObject.rotation = lastQuaternionRelativeDirection;

        Debug.DrawRay(lastPosDir, lastVectorRelativeDirection * 5, Color.blue, 5f);
        Debug.DrawRay(lastPosDir, rotateObject.forward * 5, Color.red, 5f);
    }
}
