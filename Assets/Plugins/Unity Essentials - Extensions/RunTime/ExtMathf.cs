using UnityEngine;

namespace UnityEssentials.Extensions
{
    /// <summary>
    /// Vector Extension for spline
    /// </summary>
    public static class ExtMathf
    {
        /// <summary>
        /// number convert range (55 from 0 to 100, to a base 0 - 1 for exemple)
        /// </summary>
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        /// <summary>
        /// number convert range (55 from 0 to 100, to a base 0 - 1 for exemple)
        /// </summary>
        public static double Remap(this double value, double from1, double to1, double from2, double to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        /// <summary>
        /// Get the center of a sphereCast calculation.
        /// </summary>
        /// <param name="origin">origin of the spherreCast</param>
        /// <param name="directionCast">direction of the sphere cast</param>
        /// <param name="hitInfoDistance">hitInfo.distance of the hitInfo</param>
        /// <returns>center position of the hit info</returns>
        public static Vector3 GetCollisionCenterSphereCast(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
        {
            return origin + (directionCast.normalized * hitInfoDistance);
        }

        /// <summary>
        /// From a given value (2), in an interval, from 0.5 to 3,
        /// give the mirror of this value in that interval, here: 1.5
        /// </summary>
        /// <param name="x">value to transpose</param>
        /// <param name="minInterval"></param>
        /// <param name="maxInterval"></param>
        /// <returns></returns>
        public static float MirrorFromInterval(float x, float minInterval, float maxInterval)
        {
            float middle = (minInterval + maxInterval) / 2f;
            return (SymetricToPivotPoint(x, middle));
        }
        public static int MirrorFromInterval(int x, int minInterval, int maxInterval)
        {
            int middle = (minInterval + maxInterval) / 2;
            return (SymetricToPivotPoint(x, middle));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">the value to transpose</param>
        /// <param name="a">pivot point</param>
        /// <returns></returns>
        public static float SymetricToPivotPoint(float x, float a)
        {
            return (-x + 2 * a);
        }
        public static int SymetricToPivotPoint(int x, int a)
        {
            return (-x + 2 * a);
        }

        public static bool IsBetween(this float currentValue, float value1, float value2)
        {
            return (value1 <= currentValue && currentValue <= value2);
        }
    }
}
