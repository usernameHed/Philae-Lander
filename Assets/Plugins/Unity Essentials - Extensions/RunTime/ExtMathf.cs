using UnityEngine;

namespace UnityEssentials.Extensions
{
    /// <summary>
    /// Vector Extension for spline
    /// </summary>
    public static class ExtMathf
    {
        private static Vector3[] positionRegister;
        private static float[] posTimeRegister;
        private static int positionSamplesTaken = 0;

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

        //This function calculates the acceleration vector in meter/second^2.
        //Input: position. If the output is used for motion simulation, the input transform
        //has to be located at the seat base, not at the vehicle CG. Attach an empty GameObject
        //at the correct location and use that as the input for this function.
        //Gravity is not taken into account but this can be added to the output if needed.
        //A low number of samples can give a jittery result due to rounding errors.
        //If more samples are used, the output is more smooth but has a higher latency.
        public static bool LinearAcceleration(out Vector3 vector, Vector3 position, int samples)
        {

            Vector3 averageSpeedChange = Vector3.zero;
            vector = Vector3.zero;
            Vector3 deltaDistance;
            float deltaTime;
            Vector3 speedA;
            Vector3 speedB;

            //Clamp sample amount. In order to calculate acceleration we need at least 2 changes
            //in speed, so we need at least 3 position samples.
            if (samples < 3)
            {

                samples = 3;
            }

            //Initialize
            if (positionRegister == null)
            {

                positionRegister = new Vector3[samples];
                posTimeRegister = new float[samples];
            }

            //Fill the position and time sample array and shift the location in the array to the left
            //each time a new sample is taken. This way index 0 will always hold the oldest sample and the
            //highest index will always hold the newest sample. 
            for (int i = 0; i < positionRegister.Length - 1; i++)
            {

                positionRegister[i] = positionRegister[i + 1];
                posTimeRegister[i] = posTimeRegister[i + 1];
            }
            positionRegister[positionRegister.Length - 1] = position;
            posTimeRegister[posTimeRegister.Length - 1] = Time.time;

            positionSamplesTaken++;

            //The output acceleration can only be calculated if enough samples are taken.
            if (positionSamplesTaken >= samples)
            {

                //Calculate average speed change.
                for (int i = 0; i < positionRegister.Length - 2; i++)
                {

                    deltaDistance = positionRegister[i + 1] - positionRegister[i];
                    deltaTime = posTimeRegister[i + 1] - posTimeRegister[i];

                    //If deltaTime is 0, the output is invalid.
                    if (deltaTime == 0)
                    {

                        return false;
                    }

                    speedA = deltaDistance / deltaTime;
                    deltaDistance = positionRegister[i + 2] - positionRegister[i + 1];
                    deltaTime = posTimeRegister[i + 2] - posTimeRegister[i + 1];

                    if (deltaTime == 0)
                    {

                        return false;
                    }

                    speedB = deltaDistance / deltaTime;

                    //This is the accumulated speed change at this stage, not the average yet.
                    averageSpeedChange += speedB - speedA;
                }

                //Now this is the average speed change.
                averageSpeedChange /= positionRegister.Length - 2;

                //Get the total time difference.
                float deltaTimeTotal = posTimeRegister[posTimeRegister.Length - 1] - posTimeRegister[0];

                //Now calculate the acceleration, which is an average over the amount of samples taken.
                vector = averageSpeedChange / deltaTimeTotal;

                return true;
            }

            else
            {

                return false;
            }
        }

    }
}
