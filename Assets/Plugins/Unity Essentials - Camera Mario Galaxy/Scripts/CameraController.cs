using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.CameraMarioGalaxy
{
    [ExecuteInEditMode]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _upReference;
        [SerializeField] private Transform _follow;
        [SerializeField] private Transform _lookAt;

        private void LateUpdate()
        {
            if (_follow == null)
            {
                return;
            }

            ApplyChange();
        }

        private void ApplyChange()
        {
            _camera.transform.position = _follow.position;
            _camera.transform.LookAt(_lookAt, _upReference.up);
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