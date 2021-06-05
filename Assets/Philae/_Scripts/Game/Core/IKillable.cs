using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Philae.Core
{
    public interface IKillable
    {
        void Kill();
        void GetHit(int amount, Vector3 posAttacker);
    }
}