using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.noNull;

namespace UnityEssentials.Attractor
{
    public class RotateToGround : MonoBehaviour
    {
        [SerializeField] protected float _rotationSpeed = 10f;

        [SerializeField, NoNull] protected Graviton _gravityEntity = default;
        [Tooltip("Transform to rotate toward gravity"), SerializeField, NoNull]
        private Transform _toRotate = null;

        public void Init()
        {
            InstantRotate(_gravityEntity.CurrentNormal);
        }

        protected void InstantRotate(Vector3 dirSmoothedNormal)
        {
            Vector3 dirOrientation = -dirSmoothedNormal;
            _toRotate.rotation = Quaternion.FromToRotation(_toRotate.up, dirOrientation) * _toRotate.rotation;
        }

        public static void InstantRotateObject(Vector3 dirOrientation, Transform objToRotate)
        {
            objToRotate.rotation = Quaternion.FromToRotation(objToRotate.up, dirOrientation) * objToRotate.rotation;
        }

        protected virtual void FixedUpdate()
        {
            RotateObject(_rotationSpeed, _gravityEntity.CurrentNormal);
        }

        protected void RotateObject(float speedRotate, Vector3 dirSmoothedNormal)
        {
            Vector3 dirOrientation = -dirSmoothedNormal;
            //Debug.DrawRay(rbObject.transform.position, dirOrientation * 5, Color.green, 0.3f);
            Quaternion targetRotation = Quaternion.FromToRotation(_toRotate.up, dirOrientation) * _toRotate.rotation;
            _toRotate.rotation = Quaternion.RotateTowards(_toRotate.rotation, targetRotation, speedRotate * Time.deltaTime);
        }
    }
}