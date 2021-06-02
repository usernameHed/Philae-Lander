using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

[System.Serializable]
public struct FrequencyEase
{
    [SerializeField, Tooltip("speed")]
    private float speedIn;

    [SerializeField, Tooltip("speed")]
    private bool doInverse;
    [SerializeField, Tooltip("timer back to 0 speed (default 1)")]
    private float ratioDecelerate;
    [SerializeField, Tooltip("curve, default: time: 0 to X; value: 0 to 1")]
    private AnimationCurve animationCurve;

    [SerializeField, Tooltip(""), ReadOnly]
    private bool timerStarted;
    [SerializeField, Tooltip(""), ReadOnly]
    private bool timerIsEnding;

    [SerializeField, Tooltip(""), ReadOnly]
    private float timeStart;
    [SerializeField, Tooltip(""), ReadOnly]
    private float timeEnd;
    [SerializeField, Tooltip(""), ReadOnly]
    private float currentTime;
    [SerializeField, Tooltip(""), ReadOnly]
    private float currentValue;

    private float timeWhenStart;
    private float previousTimeFrame;

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
        timerIsEnding = false;
        timeStart = 0f;
        timeEnd = 0f;
        currentTime = 0f;
    }

    public void Reset()
    {
        timerStarted = false;
        timerIsEnding = false;
        timeStart = 0f;
        timeEnd = 0f;
        currentTime = 0f;
    }

    public bool IsStarted()
    {
        return (timerStarted);
    }

    /// <summary>
    /// max: maxSecond
    /// </summary>
    public void StartCoolDown(float _maxSecond = 1)
    {
        timeStart = currentTime = 0;
        timeEnd = _maxSecond;

        timerStarted = true;
        timerIsEnding = false;
        timeWhenStart = Time.fixedTime;
        previousTimeFrame = Time.fixedTime;
    }

    public float EvaluateWithDeltaTime()
    {
        currentValue = animationCurve.Evaluate(currentTime);
        return (currentValue * Time.deltaTime * speedIn);
    }

    public float GetFinalSpeed()
    {
        currentValue = animationCurve.Evaluate(animationCurve.keys[animationCurve.length - 1].value);
        return (currentValue * Time.deltaTime * speedIn);
    }

    public float EvaluateWithoutDeltaTime()
    {
        currentValue = animationCurve.Evaluate(currentTime);
        return (currentValue * speedIn);
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
        if (!timerStarted && !timerIsEnding)
        {
            //Debug.Log("here go backward, but we did'nt go forward at first... do nothing");
            return;
        }

        if (timerStarted && !timerIsEnding)
        {
            //Debug.Log("here first time we go backward !");
            timerIsEnding = true;
        }
        timerStarted = false;

        RemoveOneFrame();

        if (currentTime == 0)
        {
            timerIsEnding = false;
        }        
    }

    private void AddOneFrame()
    {
        //float timePast = Time.fixedTime - timeWhenStart;
        float amountToAdd = Time.fixedTime - previousTimeFrame;
        currentTime += amountToAdd;

        currentTime = Mathf.Clamp(currentTime, 0, timeEnd);

        //Debug.Log("timePast: " + currentTime);

        previousTimeFrame = Time.fixedTime;
    }
    private void RemoveOneFrame()
    {
        float amountToDelete = Time.fixedTime - previousTimeFrame;

        currentTime -= amountToDelete;
        currentTime = Mathf.Clamp(currentTime, 0, timeEnd);

        previousTimeFrame = Time.fixedTime;
    }
    
}
