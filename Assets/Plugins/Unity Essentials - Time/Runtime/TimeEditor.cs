using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using UnityEssentials.time.extensions;

namespace UnityEssentials.time
{
    /// <summary>
    /// Use this class instead of the Time class if you want to use time in editor
    /// use TimeEditor.time instead of Time.time will work in editor & in play mode
    ///
    /// use TimeEditor.time instead of Time.time
    /// use TimeEditor.timeScale instead of Time.timeScale
    /// use TimeEditor.unscaledTime instead of Time.unscaledTime
    /// use TimeEditor.deltaTime instead of Time.deltaTime
    /// use TimeEditor.unscaledDeltaTime instead of Time.unscaledDeltaTime
    /// 
    /// In editor, you can set the timeScale to negative to backward time !
    /// but warning: doen't work in play mode
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class TimeEditor
    {
        //private float _editModeLastUpdate;   //the last time the controller was updated while in Edit Mode
        private static float _timeScale = 1f;       //current timeScale in both editor & play mode
        private static float _currentTime;          //current time timeScale-dependent (like Time.time but in editMode)
        private static float _currentTimeIndependentTimeScale;  //current time timeScale-independent (like Time.unscaledTime but in editMode)
        private static float _currentDeltaTime;     //current deltaTime timeScale-dependent (like Time.deltaTime but in editMode)
        private static float _currentUnscaledDeltaTime; //current deltaTime timeScale-independent (like Time.unscaledDeltaTime but in editMode)
        private static float _smoothDeltaTime;      //current deltatime, smoothed X time
        private static float _smoothUnscaledDeltaTime;      //current deltatime, smoothed X time

        private static Queue<float> _olfDeltaTimes = new Queue<float>();         //list of all previousDeltaTime;
        private static Queue<float> _olfUnscaledDeltaTimes = new Queue<float>();         //list of all previousDeltaTime;

        private static readonly int _maxAmountDeltaTimeSaved = 10; //amount of previous deltaTime to save for the smoothDeltaTime algorythm

        //don't let the deltaTime get higher than 1/30: if you have low fps (bellow 30),
        //the game start to go in slow motion.
        //if you don't want this behavior, put the value at 0.4 for exemple as precaution, we don't
        //want the player, after a huge spike of 5 seconds, to travel walls !
        private static readonly float _maxSizeDeltaTime = 0.033f;
        //private readonly float _maxSizeDeltaTime = 0.033f * 1000;

        private static bool _isInHyperSampling = false;
        public static bool IsInHyperSampling { get { return (_isInHyperSampling); } }

        #region public properties

        /// <summary>
        /// get the current timeScale
        /// </summary>
        public static float timeScale
        {
            get
            {
                if (Application.isPlaying)
                {
                    return (UnityEngine.Time.timeScale);
                }
                return (_timeScale);
            }
            set
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Time.timeScale = value;
                    return;
                }

                if (value != _timeScale)
                {
                    _timeScale = value;
                    UnityEngine.Time.timeScale = Mathf.Max(0, _timeScale);
                }
            }
        }
        /// <summary>
        /// the time (timeScale dependent) at the begening of this frame (Read only). This is the time in seconds since the start of the game / the editor compilation
        /// </summary>
        public static float time
        {
            get
            {
                if (Application.isPlaying)
                {
                    return (UnityEngine.Time.time);
                }
                return (_currentTime);
            }
        }

        public static float fixedTime
        {
            get
            {
                return (UnityEngine.Time.fixedTime);
            }
        }

        public static float fixedUnscaledTime
        {
            get
            {
                return (UnityEngine.Time.fixedUnscaledTime);
            }
        }

        /// <summary>
        /// The timeScale-independant time for this frame (Read Only). This is the time in seconds since the start of the game / the editor compilation
        /// </summary>
        public static float unscaledTime
        {
            get
            {
                if (Application.isPlaying)
                {
                    return (UnityEngine.Time.unscaledTime);
                }
                return (_currentTimeIndependentTimeScale);
            }
        }

        /// <summary>
        /// The completion time in seconds since the last from (Read Only)
        /// </summary>
        public static float deltaTime
        {
            get
            {
                if (Application.isPlaying)
                {
                    return (UnityEngine.Time.deltaTime);
                }
                return (_currentDeltaTime);
            }
        }

        /// <summary>
        /// The timeScale-independent interval in seconds from the last frames to the curren tone
        /// </summary>
        public static float unscaledDeltaTime
        {
            get
            {
                if (Application.isPlaying)
                {
                    return (UnityEngine.Time.unscaledDeltaTime);
                }
                return (_currentUnscaledDeltaTime);
            }
        }

        /// <summary>
        /// The timeScale-dependent smoothed DeltaTime, it's the average of the 'n = 10' previous frames
        /// </summary>
        public static float smoothDeltaTime
        {
            get
            {
                if (Application.isPlaying)
                {
                    return (UnityEngine.Time.smoothDeltaTime);
                }

                if (_timeScale == 0)
                {
                    return (0);
                }

                return (_smoothDeltaTime);
            }
        }

        /// <summary>
        /// get the fixedDeltaTime  in editor OR play mode, the method is different
        /// in play mode
        /// </summary>
        public static float fixedDeltaTime
        {
            get
            {
                if (!Application.isPlaying)
                {
                    return (TimeEditor.smoothDeltaTime);
                }
                else
                {
                    return (UnityEngine.Time.fixedDeltaTime);
                }
            }
        }

        public static float TimeEditorAndRunTime
        {
            get
            {
                if (!Application.isPlaying)
                {
                    return (TimeEditor.time);
                }
                else
                {
                    return (TimeEditor.fixedTime);
                }
            }
        }

        public static float TimeUnscaledEditorAndRunTime
        {
            get
            {
                if (!Application.isPlaying)
                {
                    return (TimeEditor.unscaledTime);
                }
                else
                {
                    return (TimeEditor.fixedUnscaledTime);
                }
            }
        }

        /// <summary>
        /// The timeScale-independent smoothed DeltaTime, it's the average of the 'n = 10' previous frames
        /// </summary>
        public static float smoothUnscaledDeltaTime
        {
            get
            {
                if (Application.isPlaying)
                {
                    return (UnityEngine.Time.smoothDeltaTime);
                }
                return (_smoothUnscaledDeltaTime);
            }
        }
        #endregion

        #region private functions

        /// <summary>
        /// subscribe to EditorApplication.update to be able to call EditorApplication.QueuePlayerLoopUpdate();
        /// We need to do it because unity doens't update when no even are triggered in the scene.
        /// 
        /// Then, Start the timer, this timer will increase every frame (in play or in editor mode), like the normal Time
        /// </summary>
        static TimeEditor()
        {
            _timeScale = 1f;
            UnityEngine.Time.timeScale = 1f;

#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
            StartCoolDown();
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// called every editorUpdate, tell unity to execute the Update() method
        /// even if no event are triggered in the scene
        /// in the scene.
        /// 
        /// called every frame in play and in editor, thanks to EditorApplication.QueuePlayerLoopUpdate();
        /// add to the current time, then save the current time for later.
        /// </summary>
        private static void EditorUpdate()
        {
            if (!Application.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }

            if (_isInHyperSampling)
            {
                return;
            }
            AdvanceFromOneFrame(UnityEngine.Time.unscaledDeltaTime);
        }
#endif

        /// <summary>
        /// at start, we initialize the current time
        /// </summary>
        public static void StartCoolDown(bool hyperSampling = false)
        {
            _isInHyperSampling = hyperSampling;
            _currentTime = 0;
            _currentTimeIndependentTimeScale = 0;
        }

        public static void StopHyperSampling()
        {
            _isInHyperSampling = false;
        }

        public static void AdvanceInHyperSampling(float deltaTime = 0.016f)
        {
            AdvanceFromOneFrame(deltaTime);
        }


        private static void AdvanceFromOneFrame(float deltaTime)
        {
            CalculateDeltaTimes(deltaTime);
            AdvanceTime();
        }

        private static void CalculateDeltaTimes(float deltaTime)
        {
            _currentUnscaledDeltaTime = deltaTime;
            _currentDeltaTime = _currentUnscaledDeltaTime * timeScale;

            ClampDeltaTime();
            
            SetSmoothDeltaTimes();
        }

        /// <summary>
        /// Don't allow fps to drop bellow _maxSizeDeltaTime (30FPS for 0.033f)
        /// if we are in hyperSampling, don't clamp.
        /// </summary>
        private static void ClampDeltaTime()
        {
            if (!_isInHyperSampling)
            {
                _currentDeltaTime = Mathf.Min(_currentDeltaTime, _maxSizeDeltaTime);
            }
        }

        /// <summary>
        /// Set and manage the smoothdeltaTime, by adding the average of the 'n' previous deltaTimes together
        /// </summary>
        private static void SetSmoothDeltaTimes()
        {
            float sumOfPreviousDeltaTimes = _olfDeltaTimes.GetSum();
            float sumOfPreviousUnslacedDeltaTime = _olfUnscaledDeltaTimes.GetSum();

            _smoothDeltaTime = (_currentDeltaTime + (sumOfPreviousDeltaTimes)) / (_olfDeltaTimes.Count + 1);
            _smoothUnscaledDeltaTime = (_currentUnscaledDeltaTime + (sumOfPreviousUnslacedDeltaTime)) / (_olfUnscaledDeltaTimes.Count + 1);
            //_smoothDeltaTime = (_currentDeltaTime + (sum of 'n' previous delta times)) / ('n' + 1)

            _olfDeltaTimes.Enqueue(_currentDeltaTime);
            _olfUnscaledDeltaTimes.Enqueue(_currentUnscaledDeltaTime);

            while (_olfDeltaTimes.Count > _maxAmountDeltaTimeSaved)
            {
                _olfDeltaTimes.Dequeue();
            }

            while (_olfUnscaledDeltaTimes.Count > _maxAmountDeltaTimeSaved)
            {
                _olfUnscaledDeltaTimes.Dequeue();
            }
        }

        /// <summary>
        /// called every frame, add delta time to the current timer, with or not timeScale
        /// </summary>
        private static void AdvanceTime()
        {
            _currentTime += _currentDeltaTime;
            _currentTimeIndependentTimeScale += _currentUnscaledDeltaTime;
        }

        #endregion
    }
}