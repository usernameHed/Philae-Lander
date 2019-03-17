using AiUnity.MultipleTags.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGravityAttractorSwitch : BaseGravityAttractorSwitch
{
    public override void JustJumped()
    {

    }

    public override void OnGrounded()
    {

    }

    private void FixedUpdate()
    {
        base.CalculateGAGravity();
    }
}
