using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class FrequencyChrono
{
    float timeStart = 0;

    /// <summary>
    /// Initialise l'optimisation
    /// </summary>
    public void StartCoolDown()
    {
        timeStart = Time.fixedTime;
    }
    
    /// <summary>
    /// return actual time
    /// </summary>
    public float GetTimer()
    {
        float time = Time.time - timeStart;
        return (time);
    }

    public float GetMinutes()
    {
        float time = Time.time - timeStart;
        int minutes = (int)(time / 60);
        return (minutes);
    }
    public float GetSecondes()
    {
        float time = Time.time - timeStart;
        //int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        return (seconds);
    }
}
