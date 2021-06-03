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
        /// Get a random from chance / max: 2 / 3
        /// use: ExtRandom.GetRandomNumberProbability(2, 3);
        /// </summary>
        /// <param name="chance"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool GetRandomNumberProbability(int chance, int max)
        {
            float number = GetRandomNumber(0f, 1f);
            return (number > chance / max);
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

        /// <summary>
        /// do a coin flip, return true or false
        /// </summary>
        public static bool GetRandomBool()
        {
            float number = UnityEngine.Random.Range(0f, 1f);
            return (number > 0.5f);
        }

        /// <summary>
        /// get a random color
        /// </summary>
        /// <returns></returns>
        public static Color GetRandomColor()
        {
            Color randomColor = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), 1);
            return (randomColor);
        }

        /// <summary>
        /// get a normal random
        /// use: GenerateNormalRandom(0, 0.1);
        /// </summary>
        /// <param name="mu">centre of the distribution</param>
        /// <param name="sigma">Standard deviation (spread or "width") of the distribution</param>
        /// <returns></returns>
        public static float GenerateNormalRandom(float mu, float sigma)
        {
            float rand1 = UnityEngine.Random.Range(0.0f, 1.0f);
            float rand2 = UnityEngine.Random.Range(0.0f, 1.0f);

            float n = Mathf.Sqrt(-2.0f * Mathf.Log(rand1)) * Mathf.Cos((2.0f * Mathf.PI) * rand2);

            return (mu + sigma * n);
        }

        #region seed
        /// <summary>
        /// generate a random based on static hash
        /// </summary>
        /// <param name="seed">string based</param>
        /// <returns>rendom generated from the seed</returns>
        public static System.Random Seedrandom(string seed)
        {
            System.Random random = new System.Random(seed.GetHashCode());
            return (random);
        }

        /// <summary>
        /// here we have a min & max, we remap the random 0,1 value finded to this min&max
        /// warning: we cast it into an int at the end
        /// use: 
        /// System.Random seed = ExtRandom.Seedrandom("hash");
        /// int randomInt = ExtRandom.RemapFromSeed(0, 50, seed);
        /// </summary>
        public static int RemapFromSeed(double min, double max, System.Random randomSeed)
        {
            double zeroToOneValue = randomSeed.NextDouble();
            int minToMaxValue = (int)ExtMathf.Remap(zeroToOneValue, 0, 1, min, max);
            return (minToMaxValue);
        }
        public static double RemapFromSeedDecimal(double min, double max, System.Random randomSeed)
        {
            double zeroToOneValue = randomSeed.NextDouble();
            double minToMaxValue = ExtMathf.Remap(zeroToOneValue, 0, 1, min, max);
            return (minToMaxValue);
        }
        public static float RemapFromSeedDecimal(float min, float max, System.Random randomSeed)
        {
            float zeroToOneValue = (float)randomSeed.NextDouble();
            float minToMaxValue = ExtMathf.Remap(zeroToOneValue, 0, 1, min, max);
            return (minToMaxValue);
        }

        /// <summary>
        /// get a random color, with alpha 1
        /// </summary>
        /// <returns></returns>
        public static Color GetRandomColorSeed(System.Random randomSeed)
        {
            Color randomColor = new Color((float)RemapFromSeedDecimal(0.0f, 1.0f, randomSeed), (float)RemapFromSeedDecimal(0.0f, 1.0f, randomSeed), (float)RemapFromSeedDecimal(0.0f, 1.0f, randomSeed), 1);
            return (randomColor);
        }
        #endregion
    }
}