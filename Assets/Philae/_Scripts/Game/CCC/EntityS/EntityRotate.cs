
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

namespace Philae.CCC
{
    public class EntityRotate : MonoBehaviour
    {

        [SerializeField, Tooltip("ref rigidbody")]
        private float turnRate = 700f;
        [SerializeField, Tooltip("ref rigidbody")]
        private float turnRateInAir = 300f;
        [SerializeField, Tooltip("ref rigidbody")]
        private bool instantRotate = false;
        [SerializeField, Tooltip("ref rigidbody")]
        private bool doSimpleAirRotate = false;
        [SerializeField, Tooltip("ref rigidbody")]
        public ExtRotation.OrientationRotation CameraFromPlayerOrientation = ExtRotation.OrientationRotation.NONE;
        [SerializeField, Tooltip("ref rigidbody")]
        public ExtRotation.OrientationRotation InputFromPlayerOrientation = ExtRotation.OrientationRotation.NONE;
        [SerializeField, Tooltip("ref rigidbody")]
        public ExtRotation.OrientationRotation InputFromCameraOrientation = ExtRotation.OrientationRotation.NONE;
        [SerializeField, Tooltip("ref rigidbody")]
        private bool calculateOrientation = true;

        [Range(0, 1), Tooltip("base speed"), SerializeField]
        private float ratioConsideredFullSpeed = 0.5f;

        [SerializeField, Tooltip("ref script")]
        private EntityAction entityAction = null;
        [SerializeField, Tooltip("ref script")]
        private UniqueGravity baseGravity = null;
        [SerializeField, Tooltip("ref script")]
        private EntityMove entityMove = default;
        [Tooltip("dobject to rotate"), SerializeField]
        private Transform objectToRotate = null;
        [SerializeField, Tooltip("ref script")]
        private EntityController entityController = null;

        [SerializeField, Tooltip("ref script")]
        private Vector3 lastDesiredDirection;

        private Vector3 lastVectorRelativeDirection;  //last desired rotation
        private Quaternion lastQuaternionRelativeDirection;  //last desired rotation
        public Vector3 GetLastDesiredDirection() => lastDesiredDirection;
        private Vector3 lastPosDir = Vector3.zero;

        private bool isFullSpeedBefore = false;

        private void Start()
        {
            Vector3 normalizedNormalGravity = baseGravity.GetMainAndOnlyGravity();
            if (entityController)
            {
                lastQuaternionRelativeDirection = GetLastDesiredRotation(entityController.GetFocusedForwardDirPlayer(normalizedNormalGravity), objectToRotate.up);
            }
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
            return (ExtRotation.TurretLookRotation(dirRelativeInput, up));
        }

        private void DoRotate(Quaternion calculatedDir, float speed)
        {

            //Debug.DrawRay(objectToRotate.position, entityAction.GetMainReferenceForwardDirection(), Color.red);
            //Debug.DrawRay(objectToRotate.position, objectToRotate.forward, Color.red, 2f);

            // Move toward that rotation at a controlled, even speed regardless of framerate.
            objectToRotate.rotation = Quaternion.RotateTowards(
                                    objectToRotate.rotation,
                                    calculatedDir,
                                    speed * Time.deltaTime);
        }

        private void DoInstantRotate(Quaternion calculatedDir)
        {
            objectToRotate.rotation = calculatedDir;
        }

        private void SetOrientationRotation()
        {
            Vector3 relativeDirectionPlayer = objectToRotate.forward;
            Vector3 relativeInputPlayer = entityAction.GetRelativeDirection();
            Vector3 relativeDirectionCamera = entityAction.GetMainReferenceForwardDirection();
            Vector3 relativeUp = entityAction.GetMainReferenceUpDirection();


            CameraFromPlayerOrientation = ExtRotation.IsForwardBackWardRightLeft(objectToRotate.forward, entityAction.GetMainReferenceForwardDirection(), entityAction.GetMainReferenceUpDirection());
            InputFromPlayerOrientation = ExtRotation.IsForwardBackWardRightLeft(objectToRotate.forward, relativeInputPlayer, entityAction.GetMainReferenceUpDirection());
            InputFromCameraOrientation = ExtRotation.IsForwardBackWardRightLeft(relativeDirectionCamera, relativeInputPlayer, entityAction.GetMainReferenceUpDirection());
        }

        /// <summary>
        /// called by entityAirMove
        /// </summary>
        public void DoAirRotate(float speedRatio = 1)
        {
            if (!entityAction.IsMoving(0.05f))
                return;

            //Debug.Log("ratio speedRotate air: " + speedRatio);

            Vector3 dirInput = entityAction.GetRelativeDirection();
            DoRotate(GetLastDesiredRotation(dirInput, objectToRotate.up), turnRateInAir * speedRatio);
        }

        private void FixedUpdate()
        {
            if (calculateOrientation)
                SetOrientationRotation();

            if (instantRotate)
            {
                DoRotate(GetLastDesiredRotation(entityAction.GetRelativeDirection(), objectToRotate.up), turnRate);
                return;
            }


            if (entityController.GetMoveState() == PlayerController.MoveState.InAir)
            {
                isFullSpeedBefore = true;
                if (doSimpleAirRotate)
                    DoAirRotate();
                return;
            }

            if (entityAction.IsMoving())
            {
                lastPosDir = objectToRotate.position;
                lastVectorRelativeDirection = entityAction.GetRelativeDirection();

                DoRotate(GetLastDesiredRotation(lastVectorRelativeDirection, objectToRotate.up), turnRate);
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
                    DoRotate(GetLastDesiredRotation(lastVectorRelativeDirection, objectToRotate.up), turnRate);
                }
            }

            lastQuaternionRelativeDirection = GetLastDesiredRotation(lastVectorRelativeDirection, baseGravity.GetMainAndOnlyGravity());
            lastDesiredDirection = lastQuaternionRelativeDirection * Vector3.forward;
            //rotateObject.rotation = lastQuaternionRelativeDirection;

            //Debug.DrawRay(lastPosDir, lastDesiredDirection, Color.blue, 5f);
            //Debug.DrawRay(lastPosDir, rotateObject.forward * 5, Color.red, 5f);
        }
    }
}