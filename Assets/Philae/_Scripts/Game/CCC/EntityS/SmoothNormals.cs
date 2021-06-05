
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Philae.CCC
{
    public class SmoothNormals : UniqueSmoothNormals
    {
        [Tooltip("distance for checking if the controller is grounded (0.1f is good)"), SerializeField]
        private float smoothSpeedCamera = 2f;

        [SerializeField, Tooltip("ref script")]
        private EntityController entityController = null;
        [SerializeField, Tooltip("ref script")]
        private GroundCheck groundCheck = null;

        private void Start()
        {
            smoothedNormalPlayer = GetRotationOrientationDown();
        }

        public override Vector3 GetRotationOrientationDown()
        {
            if (!entityController && !groundCheck)
            {
                return (uniqueGravity.GetMainAndOnlyGravity());
            }

            if (entityController.GetMoveState() == EntityController.MoveState.InAir)
            {
                return (uniqueGravity.GetMainAndOnlyGravity());
            }
            return (groundCheck.GetDirLastNormal());
        }

        protected override void CalculateSmoothNormal()
        {
            Vector3 actualNormal = GetRotationOrientationDown();
            smoothedNormalPlayer = Vector3.Lerp(smoothedNormalPlayer, actualNormal, Time.deltaTime * smoothSpeedPlayer);
        }

        private void FixedUpdate()
        {
            CalculateSmoothNormal();
        }
    }
}