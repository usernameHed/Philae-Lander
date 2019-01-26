using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[TypeInfoBox("Main player controller")]
public class IAController : EntityController
{
    [FoldoutGroup("Object"), Tooltip("ref script")]
    public IAInput iaInput;

    private void Awake()
    {
        base.Init();
    }

    private void Update()
    {
        if (!enabledScript)
            return;
    }

    private void FixedUpdate()
    {
        base.ChangeState(iaInput);
    }
}
