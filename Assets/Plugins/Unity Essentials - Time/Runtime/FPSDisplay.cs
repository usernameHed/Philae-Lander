using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEssentials.time
{
    /// <summary>
    /// Display fps in game
    /// </summary>
    [ExecuteInEditMode]
    public class FPSDisplay : MonoBehaviour
    {
        FrequencyCoolDown FrequencyCoolDown = new FrequencyCoolDown();

        private const float TimeBetweenChange = 0.1f;

        private float _deltaTime = 0.0f;
        private float _fps;
        private float _mSec;
        private float _previousFps = float.MinValue;
        private GUIStyle guiStyle = new GUIStyle(); //create a new variable
        private string _fpsDisplay = "";

        private void Awake()
        {
            FrequencyCoolDown.StartCoolDown(TimeBetweenChange, true, false, false);
            guiStyle.fontSize = 50; //change the font size
            guiStyle.normal.textColor = Color.white;
        }

        private void Update()
        {
            _deltaTime += (UnityEngine.Time.unscaledDeltaTime - _deltaTime) * 0.1f;

            _mSec = _deltaTime * 1000.0f;
            _fps = 1.0f / _deltaTime;

            if (_fps != _previousFps)
            {
                _previousFps = _fps;
            }
        }

        private void OnGUI()
        {
            if (!FrequencyCoolDown.IsRunning())
            {
                string fps = _fps.ToString("0");
                string ms = _mSec.ToString("0");
                _fpsDisplay = fps + " fps (" + ms + ")";

                FrequencyCoolDown.StartCoolDown(TimeBetweenChange, true, false, false);
            }
            GUI.Label(new Rect(10, 10, 600, 100), _fpsDisplay, guiStyle);
        }
    }
}