using System;
using UnityEngine;


namespace UnityEssentials.time
{
    /// <summary>
    /// Create a cooldown, who go from [5] seconds to 0.
    /// 
    /// Use: create a new FrequancyCoolDown:
    /// private FrequencyCoolDown _coolDown = new FrequencyCoolDown();
    /// 
    /// Start it when you want, with a given time in second:
    /// _coolDown.StartCoolDown(5f);
    /// 
    /// at any time you want, get the current value of the coolDown:
    /// _coolDown.GetTimer();
    /// 
    /// if you want to test if the _coolDown is currently running, and not finished:
    /// if (_coolDown.IsRunning())
    /// 
    /// if you want to test if the _coolDown started and finished
    /// if (_coolDown.IsFinished())
    /// 
    /// Sometime it nice to know if the timer is not initialize, or finished, to start it again, you can do it with this:
    /// if (_coolDown.IsNotStartedOrFinished())
    /// </summary>
    [System.Serializable]
    public class FrequencyCoolDown
    {
        private float _departureTime = 0; //save the time departure of the coolDown
        private float DepartureTime { get { return (_departureTime); } }
        private bool _isStarted = false;
        public bool IsStarted { get { return (_isStarted); } }
        private bool _useUnscaleTime = false;
        private float _currentTime;
        private bool _isInPause = false;
        public bool IsInPause() { return (_isInPause); }
        private float _savePause;
        private bool _useFixedTime = false;
        private bool _useFixedTimeInPlayAndTimeInEditor = false;

        #region public functions
        /// <summary>
        /// Start the CoolDown with a given time. It will go from time to 0
        /// </summary>
        public void StartCoolDown(float time, bool useUnscaleTime = false, bool useFixedTime = false, bool useFixedTimeInPlayAndTimeInEditor = false)
        {
            _useFixedTime = useFixedTime;
            _useFixedTimeInPlayAndTimeInEditor = useFixedTimeInPlayAndTimeInEditor;
            _departureTime = time;
            _departureTime = (_departureTime < 0) ? 0 : _departureTime;

            _useUnscaleTime = useUnscaleTime;
            _currentTime = GetTimeScaledOrNot() + _departureTime * Mathf.Sign(TimeEditor.timeScale);

            _isStarted = true;
            _isInPause = false;
        }

        /// <summary>
        /// return the current timer, in seconds
        /// eg: return 125.95 if the timer is at 2 minutes, 5 seconds and 95 miliseconds
        /// </summary>
        /// <returns>return the time in seconds eg: return 125.95 if the timer is at 2 minutes, 5 seconds and 95 miliseconds</returns>
        public float GetTimer()
        {
            if (!IsRunning())
                return (0);
            if (IsInPause())
            {
                return (_savePause);
            }
            return (_currentTime - GetTimeScaledOrNot());
        }

        /// <summary>
        /// Is the CoolDown started, but not finished yet ?
        /// </summary>
        public bool IsRunning()
        {
            if (IsStarted && !IsReady(false))
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
        /// return true if the timer is finished
        /// if the parametter resetIfFinished si set to true, the coolDown will
        /// reset if the coolDown is finished. That's mean if we test IsFinished() again, it will return false
        /// 
        /// if the timer never started, return false;
        /// </summary>
        /// <param name="resetIfFinished">if true, the coolDown is reset. That's mean if we test IsFinished() again, it will return false</param>
        /// <returns>true if the coolDown is finished</returns>
        public bool IsFinished(bool resetIfFinished = true)
        {
            if (IsStarted && IsReady(resetIfFinished))
            {
                return (true);
            }
            return (false);
        }

        /// <summary>
        /// set the Chrono to pause
        /// </summary>
        public void Pause()
        {
            if (_isInPause || !_isStarted)
            {
                return;
            }
            _savePause = GetTimer();
            _isInPause = true;
        }

        /// <summary>
        /// Resume the timer, assuming it was in pause previously.
        /// </summary>
        public void Resume()
        {
            if (!_isInPause || !_isStarted)
            {
                return;
            }
            _currentTime = GetTimeScaledOrNot() + _savePause;
            _isInPause = false;
        }

        /// <summary>
        /// reset the coolDown & all the options
        /// </summary>
        public void Reset()
        {
            _isStarted = false;    //le cooldown est fini, on reset
            _currentTime = GetTimeScaledOrNot();
            _isInPause = false;
        }
        #endregion

        #region private function
        /// <summary>
        /// get the current time, scaled or unscaled depending on the Start options
        /// Note if you don't have TimeEditor, you can use the normal Time of Unity
        /// </summary>
        /// <returns>return the current timer of unity</returns>
        private float GetTimeScaledOrNot()
        {
#if UNITY_EDITOR
            if (_useFixedTimeInPlayAndTimeInEditor)
            {
                if (Application.isPlaying)
                {
                    return ((_useUnscaleTime) ? TimeEditor.fixedUnscaledTime : TimeEditor.fixedTime);
                }
                else
                {
                    return ((_useUnscaleTime) ? TimeEditor.unscaledTime : TimeEditor.time);
                }
            }
#endif
            if (!_useFixedTime)
            {
                return ((_useUnscaleTime) ? TimeEditor.unscaledTime : TimeEditor.time);
            }
            else
            {
                return ((_useUnscaleTime) ? TimeEditor.fixedUnscaledTime : TimeEditor.fixedTime);
            }
        }

        private bool IsReady(bool canDoReset)
        {
            if (!IsStarted)   //cooldown never started, do'nt do anything
                return (false);

            if (_isInPause)
            {
                return (false);
            }


            if (GetTimeScaledOrNot() >= _currentTime && Mathf.Sign(TimeEditor.timeScale) > 0) //le coolDown a commencé, et est terminé !
            {
                if (canDoReset)
                    Reset();
                return (true);
            }
            if (_currentTime - GetTimeScaledOrNot() > 0 && Mathf.Sign(TimeEditor.timeScale) < 0) //le coolDown a commencé, et est terminé !
            {
                if (canDoReset)
                    Reset();
                return (true);
            }
            return (false); //cooldown a commencé, et est en cours
        }

        #endregion



    }
}