using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityEssentials.Extensions
{
    public static class ExtHash
    {
        public static int CombineHashCodes(int h1, int h2)
        {
            return ((h1 << 5) + h1) ^ h2;
        }
    }
}