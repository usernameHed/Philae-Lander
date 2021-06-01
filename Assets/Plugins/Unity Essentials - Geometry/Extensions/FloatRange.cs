using System;
using System.Collections.Generic;

namespace UnityEssentials.Geometry.extensions
{
    [Serializable]
    public struct FloatRange
    {
        public float Min;
        public float Max;

        public FloatRange(float min, float max) : this()
        {
            Min = min;
            Max = max;
        }

        public void Set(float min, float max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// sort FloatRange, according to there Max value
        /// </summary>
        /// <param name="floatRanges"></param>
        /// <returns></returns>
        public static FloatRange[] Sort(FloatRange[] floatRanges)
        {
            List<FloatRange> list = floatRanges.ToList();
            for (int i = list.Count - 1; i >= 1; i--)
            {
                bool sorted = true;
                for (int j = 0; j <= i - 1; j++)
                {
                    if (list[j + 1].Max < list[j].Max)
                    {
                        list.Move(j + 1, j);
                        sorted = false;
                    }
                }
                if (sorted)
                    break;
            }
            floatRanges = list.ToArray();
            return (floatRanges);
        }
    }
}