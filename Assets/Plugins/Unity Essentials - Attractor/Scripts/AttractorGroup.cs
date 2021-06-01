using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

namespace UnityEssentials.Attractor
{
    /// <summary>
    /// 
    /// </summary>
    public class AttractorGroup : MonoBehaviour
    {
        [SerializeField, ReadOnly] private float _nearestSqrDistanceFound;
        [SerializeField, ReadOnly] private Attractor _closest;

        public void ResetGroup()
        {
            _closest = null;
        }

        public void AttemptToSetNewClosestGravityField(Attractor potentialClosest, float distance)
        {
            if (_closest == null)
            {
                _closest = potentialClosest;
                _nearestSqrDistanceFound = distance;
                return;
            }
            if (distance < _nearestSqrDistanceFound)
            {
                _closest = potentialClosest;
                _nearestSqrDistanceFound = distance;
            }
        }

        /// <summary>
        /// return true if this GravityFieldAction is actually the closest one to the player
        /// </summary>
        public bool IsTheValidOneInTheGroup(Attractor toTest)
        {
            return (toTest == _closest);
        }
    }
}