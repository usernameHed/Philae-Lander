using UnityEngine;

namespace UnityEssentials.ActionTrigger.Extensions
{
    /// <summary>
    /// Vector Extension for spline
    /// </summary>
    public static class ExtVector3
    {
        /// <summary>
        /// get the max lenght of this vector
        /// </summary>
        /// <returns>min lenght of x, y or z</returns>
        public static float Minimum(this Vector3 vector)
        {
            return Min(vector.x, vector.y, vector.z);
        }

        /// <summary>
        /// get the max lenght of this vector
        /// </summary>
        /// <returns>min lenght of x, y or z</returns>
        public static float Maximum(this Vector3 vector)
        {
            return Max(vector.x, vector.y, vector.z);
        }

        /// <summary>
        /// return the min of 3 value
        /// </summary>
        public static float Min(float value1, float value2, float value3)
        {
            float min = (value1 < value2) ? value1 : value2;
            return (min < value3) ? min : value3;
        }

        /// <summary>
        /// return the max of 3 value
        /// </summary>
        public static float Max(float value1, float value2, float value3)
        {
            float max = (value1 > value2) ? value1 : value2;
            return (max > value3) ? max : value3;
        }
    }
}
