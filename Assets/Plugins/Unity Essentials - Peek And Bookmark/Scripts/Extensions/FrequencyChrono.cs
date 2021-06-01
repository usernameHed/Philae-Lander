using UnityEngine;

namespace UnityEssentials.Peek.Extensions
{
    /// <summary>
    /// Create a Chrono, who start at 0, and increment over time.
    /// 
    /// Use: create a new FrequencyChrono:
    /// private FrequencyChrono _chrono = new FrequencyChrono();
    /// 
    /// Start it when you want:
    /// _chrono.StartChono();
    /// 
    /// You can give it a max time (it will go from 0 to 5, and stay at 5)
    /// _chrono.StartChrono(5f);
    /// 
    /// You can even ask it to loop:
    /// _chrono.StartChrono(5f, true);
    /// 
    /// at any time you want, get the current value of the Chrono:
    /// _chrono.GetTimer();
    /// 
    /// if you want to test if the Chrono is currently running
    /// if (_chrono.IsRunning())
    /// 
    /// if you have set a maxTime, and you want to test if the _chrono started and finished:
    /// if (_chrono.IsFinished())
    /// 
    /// Sometime it nice to know if the timer is not initialize, or finished, to start it again, you can do it with this:
    /// if (_chrono.IsNotStartedOrFinished())
    /// </summary>
    [System.Serializable]
    public class FrequencyChrono
    {
        private float _timeStart = 0;
        private bool _loop = false;
        private float _maxTime = -1;
        public float MaxTime { get { return (_maxTime); } set { _maxTime = value; } }
        private float _saveTime = 0;
        private bool _isInPause = false;
        public bool IsInPause { get { return (_isInPause); } }
        private bool _isStarted = false;
        public bool IsStarted { get { return (_isStarted); } }
        private bool _useUnscaleTime = false;

        private float _currentTime;
        private bool _useFixedTime = false;
        private bool _useFixedTimeInPlayAndTimeInEditor = false;

        #region public functions

        /// <summary>
        /// Start the Chrono
        /// </summary>
        /// <param name="useUnscaleTime">define if we want to use a timeScale-independent timer or not</param>
        public void StartChrono(bool useUnscaleTime = false, bool useFixedTime = false, bool useFixedTimeInPlayAndTimeInEditor = false)
        {
            _useFixedTime = useFixedTime;
            _useUnscaleTime = useUnscaleTime;
            _useFixedTimeInPlayAndTimeInEditor = useFixedTimeInPlayAndTimeInEditor;
            _timeStart = GetTimeScaledOrNot();
            _loop = false;
            _isInPause = false;
            _maxTime = -1;
            _isStarted = true;
        }

        /// <summary>
        /// Start the Chrono, with a maxTime. The chrono will stop at the max time,
        /// or loop  if needed.
        /// </summary>
        /// <param name="maxTime">max time in seconds</param>
        /// <param name="loop">do the chrono loop modulo maxTime ?</param>
        /// <param name="useUnscaleTime">define if we want to use a timeScale-independent timer or not</param>
        public void StartChrono(float maxTime, bool loop, bool useUnscaleTime = false, bool useFixedTime = false, bool useFixedTimeInPlayAndTimeInEditor = false)
        {
            //if there is a negative maxTime, don't use it, and simply advance forward the timer without stopping
            if (maxTime < 0)
            {
                StartChrono(useUnscaleTime, useFixedTime, useFixedTimeInPlayAndTimeInEditor);
                return;
            }
            _useFixedTime = useFixedTime;
            _useUnscaleTime = useUnscaleTime;
            _useFixedTimeInPlayAndTimeInEditor = useFixedTimeInPlayAndTimeInEditor;
            _timeStart = GetTimeScaledOrNot();
            _loop = loop;
            _maxTime = maxTime;
            _isInPause = false;
            _isStarted = true;
        }

        /// <summary>
        /// return the current timer, in seconds
        /// eg: return 125.95 if the timer is at 2 minutes, 5 seconds and 95 miliseconds
        /// </summary>
        /// <param name="managePause">Should always be true, return the time when we were in pause</param>
        /// <returns>return the time in seconds eg: return 125.95 if the timer is at 2 minutes, 5 seconds and 95 miliseconds</returns>
        public float GetTimer(bool managePause = true)
        {
            if (!_isStarted)
            {
                return (0);
            }

            if (managePause && _isInPause)
            {
                return (_saveTime);
            }

            _currentTime = GetTimeScaledOrNot() - _timeStart;

            if (_loop)
            {
                if (_currentTime > _maxTime)
                {
                    _currentTime -= _maxTime;
                }
                _currentTime %= _maxTime;
            }
            else if (_maxTime != -1 && Mathf.Abs(_currentTime) > _maxTime)
            {
                _currentTime = _maxTime * Mathf.Sign(_currentTime);
            }

            return (_currentTime);
        }

        /// <summary>
        /// Is the Chrono started, but not finished yet ?
        /// if the Start options loop is set to false, and if the chrono is started,
        ///     it will return true as long as you don't reset it.
        /// </summary>
        public bool IsRunning()
        {
            if (IsStarted && _isInPause)
            {
                return (true);
            }

            if (IsStarted && (!IsTimeExceedMaxTime() || _loop))
            {
                return (true);
            }
            return (false);
        }

        /// <summary>
        /// Is the Chrono not started, or started and over ?
        /// </summary>
        /// <returns></returns>
        public bool IsNotRunning()
        {
            return (!IsRunning());
        }

        /// <summary>
        /// is the timer not started, or started and over, and not in pause
        /// </summary>
        /// <returns>return true if the chrono is not started, or started and over</returns>
        public bool IsNotStartedOrFinished()
        {
            if (!IsStarted)
            {
                return (true);
            }
            if (_isInPause)
            {
                return (false);
            }

            //if _maxTime is at 1, that mean we do'nt have any stop time, we will continue forever.
            if (_maxTime == -1)
            {
                return (false);
            }

            _currentTime = GetTimeScaledOrNot() - _timeStart;
            return (Mathf.Abs(_currentTime) >= _maxTime);
        }

        /// <summary>
        /// return true if the timer is finished (and reset the timer)
        /// 
        /// if the parametter resetIfFinished si set to true, the chrono will
        /// reset if the coolDown is finished. That's mean if we test IsFinished() again, it will return false
        /// 
        /// if the timer never started, return false;
        /// if the timer is in looping mode, return false;
        /// </summary>
        /// <param name="resetIfFinished">if true, the coolDown is reset. That's mean if we test IsFinished() again, it will return false</param>
        /// <returns></returns>
        public bool IsFinished(bool resetIfFinished = true)
        {
            if (_loop)
            {
                return (false);
            }

            if (!IsStarted)
            {
                return (false);
            }

            if (_maxTime == -1)
            {
                return (false);
            }

            if (IsTimeExceedMaxTime())
            {
                if (resetIfFinished)
                {
                    Reset();
                }
                return (true);
            }
            //Debug.Log("ic not finished...");
            return (false);
        }

        /// <summary>
        /// move in time forward manually by a certain amount
        /// </summary>
        /// <param name="jumpInTime">jump time in second</param>
        public void ManualForward(float jumpInTime = 0.016f)
        {
            _timeStart -= jumpInTime;
            _saveTime += jumpInTime;
        }

        /// <summary>
        /// move in time backward
        /// </summary>
        /// <param name="jumpInTime">jump time in second</param>
        public void ManualBackward(float jumpInTime = 0.016f)
        {
            _timeStart += jumpInTime;
            _saveTime -= jumpInTime;
        }

        public void ManualPercentForward(float percentForward)
        {
            float jump = Remap(percentForward, 0, 1, 0, _maxTime);
            ManualForward(jump);
        }
        public void ManualPercentBackward(float percentForward)
        {
            float jump = Remap(percentForward, 0, 1, 0, _maxTime);
            ManualBackward(jump);
        }

        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        /// <summary>
        /// set the Chrono to pause
        /// </summary>
        public void Pause()
        {
            if (_isInPause)
            {
                return;
            }

            _saveTime = GetTimer(false);
            _isInPause = true;
        }

        /// <summary>
        /// Resume the timer, assuming it was in pause previously.
        /// </summary>
        public void Resume()
        {
            if (!IsInPause)
            {
                return;
            }

            _isInPause = false;
            _timeStart = GetTimeScaledOrNot() - _saveTime;
        }

        /// <summary>
        /// from 0 to 1
        /// </summary>
        /// <returns></returns>
        public float GetCurrentPercentFromTheEnd()
        {
            if (IsFinished(false))
            {
                return (1f);
            }
            if (IsNotRunning())
            {
                return (0f);
            }
            float currentTimer = GetTimer();
            float currentPercent = currentTimer * 1f / _maxTime;
            return (currentPercent);
        }

        /// <summary>
        /// reset the chrono to 0 & all the options
        /// </summary>
        public void Reset()
        {
            _timeStart = GetTimeScaledOrNot();
            _loop = false;
            _isInPause = false;
            _isStarted = false;
        }

        #endregion

        #region private functions

        /// <summary>
        /// get the current time, scaled or unscaled depending on the Start options
        /// </summary>
        /// <returns>return the current timer of unity</returns>
        private float GetTimeScaledOrNot()
        {
#if UNITY_EDITOR
            if (_useFixedTimeInPlayAndTimeInEditor)
            {
                if (Application.isPlaying)
                {
                    return ((_useUnscaleTime) ? Time.fixedUnscaledTime : Time.fixedTime);
                }
                else
                {
                    return ((_useUnscaleTime) ? Time.unscaledTime : Time.time);
                }
            }
#endif

            if (!_useFixedTime)
            {
                return ((_useUnscaleTime) ? Time.unscaledTime : Time.time);
            }
            else
            {
                return ((_useUnscaleTime) ? Time.fixedUnscaledTime : Time.fixedTime);
            }
        }

        private bool IsTimeExceedMaxTime()
        {
            if (_maxTime < 0)
            {
                return (false);
            }
            return (Mathf.Abs(GetTimeScaledOrNot()) >= _timeStart + _maxTime);
        }


        #endregion


    }
}