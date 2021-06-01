using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.CameraMarioGalaxy
{
    public class CameraInputs : MonoBehaviour
    {
        public Vector2 CameraInput { get; private set; }
        public float TriggerZoom { get; private set; }

        public void CustomUpdate()
        {
            CameraInput = new Vector2(Input.GetAxis("Horizontal 2"), Input.GetAxis("Vertical 2"));
            TriggerZoom = Input.GetAxis("Trigger Zoom");
        }
    }
}