using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;
using UnityEssentials.PropertyAttribute.noNull;
using UnityEssentials.PropertyAttribute.onvalueChanged;
using UnityEssentials.Spline.Controller;
using UnityEssentials.time;

namespace UnityEssentials.CameraMarioGalaxy
{
    [ExecuteInEditMode]
    public class DollyCamMove : MonoBehaviour
    {
        [Tooltip(""), SerializeField]
        private FrequencyEase _easeRotate = new FrequencyEase();
        [Header("Right - Left")]
        [Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving X")]
        private float _deadZoneHoriz = 0.25f;
        [Tooltip("dobject to rotate"), SerializeField]
        private Transform _toRotate = null;

        [Header("Up-Down")]
        [Tooltip(""), SerializeField]
        private float _speedDolly = 1f;
        [Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
        private float _deadZoneVerti = 0.3f;
        [SerializeField, NoNull]
        private ControllerStick _follow = default;
        [SerializeField, NoNull]
        private ControllerStick _lookAt = default;

        [Header("Zoom-Dezoom")]
        [Tooltip(""), SerializeField]
        private float _speedZoom = 1f;
        [Tooltip(""), SerializeField]
        private Vector2 _minMaxZoom = new Vector2(0.1f, 5f);
        [Tooltip(""), SerializeField]
        private Transform _toScale = null;
        [Range(0f, 1f), SerializeField, Tooltip("deadzone gamepad stick when we consiere moving Y")]
        private float _deadZoneZoom = 0.1f;

        [Range(0, 1f), Tooltip(""), OnValueChanged("InputDolly"), SerializeField]
        private float _followPercent;
        [Range(0, 1f), Tooltip(""), OnValueChanged("InputDolly"), SerializeField]
        private float _currentZoomRatio = 0;

        private Vector2 _cameraInput;
        private float _trigerZoom;

        private void Start()
        {
            if (_toRotate == null)
            {
                this.enabled = false;
                return;
            }

            Init();
        }

        public void Init()
        {
            SetAimPosition();
        }

        /// <summary>
        /// rotate left-right
        /// </summary>
        private void InputRotate()
        {
            //start or continue ease rotate
            Vector2 dirInput = _cameraInput;

            //if margin turn is ok for HORIZ move
            if (Mathf.Abs(dirInput.x) >= _deadZoneHoriz)
            {
                _easeRotate.StartOrContinue();

                float remapedInput = ExtMathf.Remap(Mathf.Abs(dirInput.x), _deadZoneHoriz, 1f, 0f, 1f) * Mathf.Sign(dirInput.x);

                _toRotate.Rotate(0, _easeRotate.EvaluateWithDeltaTime() * remapedInput, 0);
            }
            else
            {
                _easeRotate.BackToTime();
            }
        }

        /// <summary>
        /// move up-down in the dolly
        /// </summary>
        private void InputDolly()
        {
            Vector2 dirInput = _cameraInput;

            if (Mathf.Abs(dirInput.y) >= _deadZoneVerti)
            {
                float remapedInput = ExtMathf.Remap(Mathf.Abs(dirInput.y), _deadZoneVerti, 1f, 0f, 1f);
                _followPercent -= _speedDolly * remapedInput * Mathf.Sign(dirInput.y) * Time.deltaTime;
                _followPercent = Mathf.Clamp(_followPercent, 0, 1f);
            }
            _follow.SetPercent(_followPercent);
        }

        /// <summary>
        /// tel to zoom
        /// </summary>
        /// <param name="zoomRatio">between -1 and 1</param>
        public void InputZoom(float speedRatio = 1f)
        {
            if (Mathf.Abs(_trigerZoom) >= _deadZoneZoom)
            {
                float remapedInput = ExtMathf.Remap(Mathf.Abs(_trigerZoom), _deadZoneVerti, 1f, 0f, 1f);
                float localScaleAllAxis = _toScale.localScale.x;
                localScaleAllAxis += _speedZoom * remapedInput * speedRatio * Mathf.Sign(_trigerZoom) * Time.deltaTime;
                localScaleAllAxis = Mathf.Clamp(localScaleAllAxis, _minMaxZoom.x, _minMaxZoom.y);

                _toScale.localScale = new Vector3(localScaleAllAxis, localScaleAllAxis, localScaleAllAxis);

                SetAimPosition();
            }
        }

        [ContextMenu("Zoom")]
        public void ForceInputZoom(float zoomRatioScale)
        {
            zoomRatioScale = Mathf.Clamp(zoomRatioScale, _minMaxZoom.x, _minMaxZoom.y);
            _toScale.localScale = new Vector3(zoomRatioScale, zoomRatioScale, zoomRatioScale);
            SetAimPosition();
        }

        private void SetAimPosition()
        {
            float localScaleAllAxis = _toScale.localScale.x;
            float max = _minMaxZoom.y - _minMaxZoom.x;
            float percent = ((localScaleAllAxis - _minMaxZoom.y) * 100 / max) + 100;

            percent = Mathf.Clamp(percent / 100f, 0f, 1f);
            _lookAt.SetPercent(percent);
            _lookAt.Stick();
        }

        public void CustomUpdate(Vector2 cameraInput, float trigerZoom)
        {
            _cameraInput = cameraInput;
            _trigerZoom = trigerZoom;

            InputZoom();
            InputRotate();
            InputDolly();
        }
    }
}