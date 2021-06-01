using UnityEditor;

namespace UnityEssentials.Geometry.Editor
{
    public static class ExtDiscProperty
    {
        public const string PROPERTY_MOVABLE_DISC = "_movableDisc";

        public const string PROPERTY_MATRIX = "_discMatrix";

        public const string PROPERTY_EXT_CIRCLE = "_circle";
        public const string PROPERTY_EXT_PLANE = "_plane";
        public const string PROPERTY_ALLOW_BOTTOM = "_allowBottom";

        public const string PROPERTY_RADIUS = "_radius";
        public const string PROPERTY_RADIUS_SQUARED = "_radiusSquared";
        public const string PROPERTY_REAL_RADIUS = "_realRadius";
        public const string PROPERTY_REAL_RADIUS_SQUARED = "_realSquaredRadius";

        
        public static bool GetAllowDown(SerializedProperty extDisc)
        {
            return (extDisc.GetPropertie(PROPERTY_EXT_CIRCLE).GetPropertie(PROPERTY_EXT_PLANE).GetPropertie(PROPERTY_ALLOW_BOTTOM).boolValue);
        }
        
        public static void SetAllowDown(SerializedProperty extDisc, bool newAllowDown)
        {
            extDisc.GetPropertie(PROPERTY_EXT_CIRCLE).GetPropertie(PROPERTY_EXT_PLANE).GetPropertie(PROPERTY_ALLOW_BOTTOM).boolValue = newAllowDown;
        }
    }
}