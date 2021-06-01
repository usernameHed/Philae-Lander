using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.ScriptableObjectSingleton
{
    ///<summary>
    /// Default paths for icons
    ///</summary>
    //[CreateAssetMenu(fileName = "ExampleScriptable", menuName = "UnityEssentials/Scriptable Singleton")]
    public class ExampleScriptable : SingletonScriptableObject<ExampleScriptable>
    {
        public int Count = 2;
    }
}