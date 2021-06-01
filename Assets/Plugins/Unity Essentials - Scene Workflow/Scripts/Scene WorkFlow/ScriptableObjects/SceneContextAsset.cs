using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.SceneWorkflow.PropertyAttribute.SceneReference;

namespace UnityEssentials.SceneWorkflow
{
    [CreateAssetMenu(fileName = "Context", menuName = "UnityEssentials/Scene Workflow/Context")]
    public class SceneContextAsset : ScriptableObject
    {
        public string NameContext = "Scene List";
        public List<SceneReference> SceneToLoad = new List<SceneReference>();
    }
}