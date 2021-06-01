using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Extensions
{
    public static class ExtRandom
    {
        /// <summary>
        /// Get random float between min and max included
        /// </summary>
        public static float GetRandomNumber(float minimum, float maximum)
        {
            float number = UnityEngine.Random.Range(minimum, maximum);
            return (number);
        }

        /// <summary>
        /// Get random float between min and maximum INCLUDED
        /// </summary>
        public static int GetRandomNumber(int minimum, int maximum)
        {
            int number = UnityEngine.Random.Range(minimum, maximum + 1);
            return (number);
        }

        /// <summary>
        /// Get a random unitsphere (or donut)
        /// </summary>
        /// <param name="radius">radius of circle</param>
        /// <param name="toreCenter">Excluse center from random</param>
        /// <returns></returns>
        public static Vector3 GetRandomInsideUnitSphere(float radius)
        {
            return (UnityEngine.Random.insideUnitSphere * radius);
        }
        public static Vector3 GetRandomInsideUnitSphere(Vector3 radius)
        {
            Vector3 unitSphere = UnityEngine.Random.insideUnitSphere;
            return (new Vector3(unitSphere.x * radius.x, unitSphere.y * radius.y, unitSphere.z * radius.z));
        }
    }
}