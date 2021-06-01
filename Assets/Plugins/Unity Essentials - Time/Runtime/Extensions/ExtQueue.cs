using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.time.extensions
{
    public static class ExtQueue
    {
        public static float GetSum(this Queue<float> queue)
        {
            float sum = 0;
            foreach (float value in queue)
            {
                sum += value;
            }
            return (sum);
        }

        public static float GetSum(this Queue<float> queue, int max)
        {
            float sum = 0;
            int index = 0;
            foreach (float value in queue)
            {
                sum += value;
                index++;
                if (index >= max)
                {
                    break;
                }
            }
            return (sum);
        }
    }
}