
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.onvalueChanged;

namespace Philae.CCC
{
    public class EntityRotateToGround : RotateToGround
    {
        [Tooltip("speed of rotation to ground"), SerializeField, OnValueChanged(nameof(UpdateSpeed))]
        public float speedRotate = 5f;
        [Tooltip("speed of rotation to ground"), SerializeField, OnValueChanged(nameof(UpdateSpeed))]
        public float speedLerpRaulBack = 5f;
        [Tooltip("ref script"), SerializeField]
        private UniqueSmoothNormals uniqueSmoothNormals = null;

        private float tmpSpeed;

        private void Start()
        {
            UpdateSpeed();
            InstantRotate(uniqueSmoothNormals.GetSmoothedNormalPlayer());
        }
        private void UpdateSpeed()
        {
            tmpSpeed = speedRotate;
        }

        public void SetNewTempSpeed(float newSpeed)
        {
            speedRotate = newSpeed;
        }

        private void FixedUpdate()
        {
            if (instantRotation)
            {
                InstantRotate(uniqueSmoothNormals.GetRotationOrientationDown());
                return;
            }

            RotateObject(speedRotate, uniqueSmoothNormals.GetSmoothedNormalPlayer());
            speedRotate = Mathf.Lerp(speedRotate, tmpSpeed, Time.deltaTime * speedLerpRaulBack);
        }
    }
}