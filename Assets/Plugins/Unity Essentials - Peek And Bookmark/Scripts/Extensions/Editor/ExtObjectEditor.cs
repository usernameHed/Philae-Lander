﻿
using UnityEditor;

namespace UnityEssentials.Peek.Extensions.Editor
{
    public static class ExtObjectEditor
    {
        public static string GetPath(this UnityEngine.Object asset)
        {
            return (AssetDatabase.GetAssetPath(asset));
        }

        public static bool IsTruelyNull(this object aRef)
        {
            return aRef != null && aRef.Equals(null);
        }
    }
}