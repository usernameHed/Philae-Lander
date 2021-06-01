using UnityEditor;
using UnityEngine;
using UnityEssentials.ActionTrigger.entity;
using UnityEssentials.SceneWorkflow;

namespace UnityEssentials.ActionTrigger.editor
{
    [InitializeOnLoad]
    public class ExecutionOrderValidator
    {
        private const int _dependencyInjectorOrder = -2000;
        private const int _entityListerOrder = -1999;

        static ExecutionOrderValidator()
        {
            var temp = new GameObject();

            var reader = temp.AddComponent<DependencyInjectorSingleton>();
            MonoScript readerScript = MonoScript.FromMonoBehaviour(reader);
            if (MonoImporter.GetExecutionOrder(readerScript) != _dependencyInjectorOrder)
            {
                MonoImporter.SetExecutionOrder(readerScript, _dependencyInjectorOrder);
                Debug.Log("Fixing execution order for " + readerScript.name + " at "+_dependencyInjectorOrder);
            }

            var writer = temp.AddComponent<EntityListerInScenes>();
            MonoScript writerScript = MonoScript.FromMonoBehaviour(writer);
            if (MonoImporter.GetExecutionOrder(writerScript) != _entityListerOrder)
            {
                MonoImporter.SetExecutionOrder(writerScript, _entityListerOrder);
                Debug.Log("Fixing execution order for " + writerScript.name + " at " + _entityListerOrder);
            }

            Object.DestroyImmediate(temp);
        }

    }
}