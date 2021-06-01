using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityEssentials.Extensions
{
    public static class ExtRigidBody
    {
        public static Vector3 ClampVelocity(Vector3 currentVelocity, float maxSpeed)
        {
            if (currentVelocity.magnitude > maxSpeed)
            {
                currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
            }
            return (currentVelocity);
        }
    }
}