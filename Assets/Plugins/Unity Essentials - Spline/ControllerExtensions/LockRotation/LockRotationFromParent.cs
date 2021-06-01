using System.Collections;
using UnityEngine;
using UnityEssentials.Spline.Extensions;

namespace UnityEssentials.Spline.ControllerExtensions
{
    [ExecuteInEditMode]
    public class LockRotationFromParent : MonoBehaviour
    {
        [Tooltip("")]
        public Transform ToLock;
        [Tooltip("")]
        public bool RotateWithTheParent = true;
        [Tooltip("")]
        public bool OverrideRotationUpGlobal = false;
        [SerializeField] private bool _onlyInEditor = false;

        [SerializeField, HideInInspector] private Quaternion _saveRotation;

        private void OnEnable()
        {
            if (_onlyInEditor && Application.isPlaying)
            {
                this.enabled = false;
                return;
            }

            if (ToLock == null && transform.childCount > 0)
            {
                ToLock = transform.GetChild(0);
            }
            _saveRotation = ToLock.rotation;
        }

        private void Update()
        {
            if (OverrideRotationUpGlobal)
            {
                if (!RotateWithTheParent)
                {
                    ToLock.rotation = ExtRotation.TurretLookRotation(_saveRotation * Vector3.forward, Vector3.up);
                }
                else
                {
                    ToLock.rotation = ExtRotation.TurretLookRotation(ToLock.forward, Vector3.up);
                }
                _saveRotation = ToLock.rotation;
            }

            if (RotateWithTheParent)
            {
                _saveRotation = ToLock.rotation;
            }
            else
            {
                ToLock.rotation = _saveRotation;
            }
        }
    }
}