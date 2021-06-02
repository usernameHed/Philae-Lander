using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.CameraMarioGalaxy
{
    /// <summary>
    /// 
    /// </summary>
    public class CameraUpdater : MonoBehaviour
    {
        [SerializeField] private DollyCamMove _dollyCamMove = default;
        [SerializeField] private CameraInputs _cameraInputs = default;

        private void Update()
        {
            _cameraInputs.CustomUpdate();
        }

        private void LateUpdate()
        {
            _dollyCamMove.CustomUpdate(_cameraInputs.CameraInput, _cameraInputs.TriggerZoom);
        }
    }
}