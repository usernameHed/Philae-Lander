using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// DontDestroyOnLoad Description
/// </summary>
[TypeInfoBox("Special type of Singleton who delete other DontDestroyOnLoad")]
public class DontDestroyOnLoad : MonoBehaviour
{
    #region Attributes
    private static DontDestroyOnLoad instance;
    public static DontDestroyOnLoad GetSingleton
    {
        get { return instance; }
    }
    #endregion

    #region Initialization
    public void SetSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }            
        else if (instance != this)
            DestroyImmediate(gameObject);
    }

    private void Awake()
    {
        SetSingleton();
    }
    #endregion
}
