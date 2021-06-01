using UnityEngine;

namespace UnityEssentials.Spline
{
    /// <summary>
    /// Vector Extension for spline
    /// </summary>
    public static class ExtVector3
    {
        /// <summary>
        /// Get the closest point on a line segment.
        /// </summary>
        /// <param name="p">A point in space</param>
        /// <param name="s0">Start of line segment</param>
        /// <param name="s1">End of line segment</param>
        /// <returns>The interpolation parameter representing the point on the segment, with 0==s0, and 1==s1</returns>
        public static float ClosestPointOnSegment(this Vector3 p, Vector3 s0, Vector3 s1)
        {
            Vector3 s = s1 - s0;
            float len2 = Vector3.SqrMagnitude(s);
            if (len2 < Mathf.Epsilon)
                return 0; // degenrate segment
            return Mathf.Clamp01(Vector3.Dot(p - s0, s) / len2);
        }

        /// <summary>Is the vector within Epsilon of zero length?</summary>
        /// <param name="v"></param>
        /// <returns>True if the square magnitude of the vector is within Epsilon of zero</returns>
        public static bool AlmostZero(this Vector3 v, float diff = 0.001f)
        {
            return v.sqrMagnitude < diff;
        }
        public static bool AlmostZero(this float v, float diff = 0.001f)
        {
            return (Mathf.Abs(v) < diff);
        }

        public static Vector2 OwnSmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, Vector2 smoothTime, Vector2 maxSpeed, float deltaTime)
        {
            if (smoothTime == Vector2Int.zero)
            {
                return (target);
            }

            float y = currentVelocity.y;
            float x = currentVelocity.x;
            Vector2 smoothedVector = new Vector2(
                OwnSmoothDamp(current.x, target.x, ref x, smoothTime.x, maxSpeed.x, deltaTime),
                OwnSmoothDamp(current.y, target.y, ref y, smoothTime.y, maxSpeed.y, deltaTime));

            currentVelocity = new Vector2(x, y);
            return (smoothedVector);
        }
        public static Vector3 OwnSmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            if (smoothTime == 0)
            {
                return (target);
            }

            float z = currentVelocity.z;
            float y = currentVelocity.y;
            float x = currentVelocity.x;
            Vector3 smoothedVector = new Vector3(
                OwnSmoothDamp(current.x, target.x, ref x, smoothTime, maxSpeed, deltaTime),
                OwnSmoothDamp(current.y, target.y, ref y, smoothTime, maxSpeed, deltaTime),
                OwnSmoothDamp(current.z, target.z, ref z, smoothTime, maxSpeed, deltaTime));

            currentVelocity = new Vector3(x, y, z);
            return (smoothedVector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="currentVelocity"></param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
        /// <param name="maxSpeed"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static float OwnSmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = Mathf.Max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
            float change = current - target;
            float originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            change = Mathf.Clamp(change, -maxChange, maxChange);
            target = current - change;

            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            float output = target + (change + temp) * exp;

            // Prevent overshooting
            if (originalTo - current > 0.0F == output > originalTo)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }
    }
}
