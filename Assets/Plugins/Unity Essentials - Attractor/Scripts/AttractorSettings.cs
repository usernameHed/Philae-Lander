using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.ScriptableObjectSingleton;

namespace UnityEssentials.Attractor
{
    ///<summary>
    /// 
    ///</summary>
    [CreateAssetMenu(fileName = "Attractor Settings", menuName = "UnityEssentials/Attractor/Attractor Settings", order = 0)]
    public class AttractorSettings : SingletonScriptableObject<AttractorSettings>
    {
        [Tooltip("Simulate physic movement. If Not, still calculate force")]                                            public bool SimulatePhysic = true;
        [Range(0, 20), Tooltip("Define how fast the distance of the others gravity is impacted")]                       public float RatioEaseOutAttractors = 0f;
        [Tooltip("Default gravity, as soon as an object attract a graviton")]                                           public float Gravity = 9.81f;
        [Tooltip("Show arrow of gravity. White: force, Green: Closest, Blue: final force, Cyan: Final Clamped force")]  public bool ShowArrow = true;
        [Tooltip("If Gravity Arrow are too high, resize them (warning, it resize the arrow, not the force!")]           public float RatioSizeArrow = 0.1f;
    }
}