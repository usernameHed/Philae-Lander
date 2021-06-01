using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.SceneWorkflow.extension;

namespace UnityEssentials.SceneWorkflow
{
    public abstract class AbstractLinker : SingletonMono<AbstractLinker>
    {
        public abstract void InitFromEditor();
        public abstract void InitFromPlay();
    }
}