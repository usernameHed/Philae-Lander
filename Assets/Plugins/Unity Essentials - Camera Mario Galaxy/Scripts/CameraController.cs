using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

namespace UnityEssentials.CameraMarioGalaxy
{
    [ExecuteInEditMode]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _upReference;
        [SerializeField] private Transform _follow;
        [SerializeField] private Transform _lookAt;
        [SerializeField] private float _dampingPosition = 0.1f;
        [SerializeField] private float _dampingLookAt = 0.1f;

        private Vector3 _refPositionVelocity;
        private Quaternion _refRotationVelocity;

        private void FixedUpdate()
        {
            if (_follow == null || _upReference == null)
            {
                return;
            }
            ApplyChange();
        }

        private void ApplyChange()
        {
            //_camera.transform.position = _follow.position;
            //_camera.transform.LookAt(_lookAt, _upReference.up);



            _camera.transform.position = ExtVector3.OwnSmoothDamp(_camera.transform.position, _follow.position, ref _refPositionVelocity, _dampingPosition, Mathf.Infinity, Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(_lookAt.position - _camera.transform.position, _upReference.up);
            //target = target * Quaternion.Euler(_offsetRotation);
            _camera.transform.rotation = ExtRotation.OwnSmoothDamp(_camera.transform.rotation, targetRotation, ref _refRotationVelocity, _dampingLookAt, Time.deltaTime);

        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                ApplyChange();
            }
        }
#endif
    }
}