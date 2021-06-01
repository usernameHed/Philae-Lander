using UnityEditor;

namespace UnityEssentials.Geometry.Editor
{
    public static class ExtQuadProperty
    {
        public const string PROPERTY_MOVABLE_QUAD = "_movableQuad";

        public const string PROPERTY_MATRIX = "_quadMatrix";

        public const string PROPERTY_EXT_PLANE = "_plane";
        public const string PROPERTY_ALLOW_BOTTOM = "_allowBottom";

        public static bool GetAllowDown(SerializedProperty extQuad)
        {
            return (extQuad.GetPropertie(PROPERTY_EXT_PLANE).GetPropertie(PROPERTY_ALLOW_BOTTOM).boolValue);
        }

        public static void SetAllowDown(SerializedProperty extQuad, bool newAllowDown)
        {
            extQuad.GetPropertie(PROPERTY_EXT_PLANE).GetPropertie(PROPERTY_ALLOW_BOTTOM).boolValue = newAllowDown;
        }
    }
}