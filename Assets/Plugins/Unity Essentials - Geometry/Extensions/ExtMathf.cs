using UnityEngine;

namespace UnityEssentials.Geometry.extensions
{
    /// <summary>
    /// Vector Extension for spline
    /// </summary>
    public static class ExtMathf
    {
        public static bool IsBetween(this float currentValue, float value1, float value2)
        {
            return (value1 <= currentValue && currentValue <= value2);
        }

        /// <summary>
        /// return the value clamped between the 2 value
        /// </summary>
        /// <param name="value1">must be less than value2</param>
        /// <param name="currentValue"></param>
        /// <param name="value2">must be more than value1</param>
        /// <returns></returns>
        public static float SetBetween(float currentValue, float value1, float value2)
        {
            if (value1 > value2)
            {
                Debug.LogError("value2 can be less than value1");
                return (0);
            }

            if (currentValue < value1)
            {
                currentValue = value1;
            }
            if (currentValue > value2)
            {
                currentValue = value2;
            }
            return (currentValue);
        }
    }
}
