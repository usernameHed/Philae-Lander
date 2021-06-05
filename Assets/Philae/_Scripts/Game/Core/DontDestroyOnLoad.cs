
using UnityEngine;

namespace Philae.Core
{
    /// <summary>
    /// DontDestroyOnLoad Description
    /// </summary>
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
}