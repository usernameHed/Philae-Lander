using System;
using UnityEngine;

namespace UnityEssentials.Extensions
{
    /// <summary>
    /// usage:
    /// [SerializeField] private EvaluateLerp _transitionLerp = new EvaluateLerp(1);
    /// _transitionLerp.StartLerp();
    ///  _transitionLerp.Lerp(X, Y);
    /// </summary>
    [Serializable]
    public class EvaluateLerp
    {
        [SerializeField] private float _transitionTime = 1f;
        [SerializeField] private AnimationCurve _curveTransition = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private FrequencyChrono _lerpChrono = new FrequencyChrono();
        private bool _isLerping = false;

        /// <summary>
        /// time in second, curve of animation
        /// </summary>
        /// <param name="time">in seconds</param>
        /// <param name="curve">curve clamped from 0 to 1</param>
        public EvaluateLerp(float time, AnimationCurve curve)
        {
            _transitionTime = time;
            _curveTransition = curve;
        }
        /// <summary>
        /// time in second
        /// </summary>
        /// <param name="time">in seconds</param>
        public EvaluateLerp(float time)
        {
            _transitionTime = time;
        }

        /// <summary>
        /// call it to start or override lerp.
        /// You can call Lerp() without it,
        /// but it you call Lerp multiple times,
        /// it will first start,
        /// then lerp forever until you choose to start again
        /// </summary>
        public void StartLerp()
        {
            _lerpChrono.StartChrono(_transitionTime, false);
            _isLerping = true;
        }

        public float Lerp(float a, float b)
        {
            if (!_isLerping)
            {
                _lerpChrono.StartChrono(_transitionTime, false);
                _isLerping = true;
            }
            return (Mathf.Lerp(a, b, _curveTransition.Evaluate(_lerpChrono.GetCurrentPercentFromTheEnd())));
        }

        public Vector3 Lerp(Vector3 a, Vector3 b)
        {
            if (!_isLerping)
            {
                _lerpChrono.StartChrono(_transitionTime, false);
                _isLerping = true;
            }
            return (Vector3.Lerp(a, b, _curveTransition.Evaluate(_lerpChrono.GetCurrentPercentFromTheEnd())));
        }

        public Quaternion Slerp(Quaternion a, Quaternion b)
        {
            if (!_isLerping)
            {
                _lerpChrono.StartChrono(_transitionTime, false);
                _isLerping = true;
            }
            return (Quaternion.Slerp(a, b, _curveTransition.Evaluate(_lerpChrono.GetCurrentPercentFromTheEnd())));
        }

        public Quaternion Lerp(Quaternion a, Quaternion b)
        {
            if (!_isLerping)
            {
                _lerpChrono.StartChrono(_transitionTime, false);
                _isLerping = true;
            }
            return (Quaternion.Lerp(a, b, _curveTransition.Evaluate(_lerpChrono.GetCurrentPercentFromTheEnd())));
        }

        /// <summary>
        /// return true if the lerping started, and the chrono is not over !
        /// </summary>
        /// <returns></returns>
        public bool IsLerping()
        {
            return (_lerpChrono.IsRunning() && !_lerpChrono.IsFinished(false));
        }

        /// <summary>
        /// return true if the lerping started, EVEN IF the chrono is at the end!
        /// </summary>
        /// <returns></returns>
        public bool IsLerpingStarted()
        {
            return (_isLerping);
        }

        public void Reset()
        {
            _isLerping = false;
        }
    }
}