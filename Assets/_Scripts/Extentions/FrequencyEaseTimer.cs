using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public struct FrequencyEase
{
    [SerializeField, Tooltip("speed")]
    private float speedIn;
    [SerializeField, Tooltip("Ratio Settings (default 1)")]
    public float ratioSettings;
    [SerializeField, Tooltip("timer back to 0 speed (default 1)")]
    private float ratioDecelerate;
    [SerializeField, Tooltip("curve, default: time: 0 to X; value: 0 to 1")]
    private AnimationCurve animationCurve;

    [SerializeField, ReadOnly, Tooltip("")]
    private bool timerStarted;
    [SerializeField, ReadOnly, Tooltip("")]
    private float timeStart;
    [SerializeField, ReadOnly, Tooltip("")]
    private float timeEnd;
    [SerializeField, ReadOnly, Tooltip("")]
    private float currentTime;    
    
    public void Init()
    {
        speedIn = 5f;
        ratioDecelerate = 1f;
        ratioDecelerate = 2f;
        animationCurve = new AnimationCurve();
        animationCurve.AddKey(new Keyframe(0, 0));
        animationCurve.AddKey(new Keyframe(1, 1));
        animationCurve.postWrapMode = WrapMode.Clamp;
        animationCurve.preWrapMode = WrapMode.Clamp;

        timerStarted = false;
        timeStart = 0f;
        timeEnd = 0f;
        currentTime = 0f;
    }

    /// <summary>
    /// max: maxSecond
    /// </summary>
    public void StartCoolDown(float _maxSecond = 1)
    {
        timeStart = currentTime = 0;
        timeEnd = _maxSecond;

        timerStarted = true;
    }

    public float Evaluate()
    {
        return (animationCurve.Evaluate(currentTime) * speedIn * ratioSettings);
    }

    /// <summary>
    /// start a cooldown, or continue it (clamp to the end !)
    /// </summary>
    /// <param name="_maxSecond"></param>
    public void StartOrContinue()
    {
        if (!timerStarted)
        {
            StartCoolDown(animationCurve.keys[animationCurve.length - 1].time);
            return;
        }
        timeEnd = animationCurve.keys[animationCurve.length - 1].time;
        AddOneFrame();
    }

    public void BackToTime()
    {
        if (!timerStarted)
        {
            currentTime = 0;
            return;
        }
        RemoveOneFrame();
    }

    private void AddOneFrame()
    {
        currentTime += Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0, timeEnd);
    }
    private void RemoveOneFrame()
    {
        currentTime -= Time.deltaTime * ratioDecelerate;
        currentTime = Mathf.Clamp(currentTime, 0, timeEnd);
    }
    
}
