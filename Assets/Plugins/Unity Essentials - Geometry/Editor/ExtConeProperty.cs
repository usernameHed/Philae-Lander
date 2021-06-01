using UnityEditor;

namespace UnityEssentials.Geometry.Editor
{
    public static class ExtConeProperty
    {
        public const string PROPERTY_MOVABLE_CONE = "_movableCone";
        public const string PROPERTY_EXT_CONE = "_cone";
        public const string PROPERTY_MATRIX = "_coneMatrix";

        public const string PROPERTY_RADIUS = "_radius";
        public const string PROPERTY_RADIUS_SQUARED = "_radiusSquared";
        public const string PROPERTY_REAL_RADIUS = "_realRadius";
        public const string PROPERTY_REAL_RADIUS_SQUARED = "_realSquaredRadius";
        public const string PROPERTY_LENGHT = "_lenght";
        public const string PROPERTY_LENGHT_SQUARED = "_lenghtSquared";

        public const string PROPERTY_CIRCLE_BASE = "_circleBase";

        public static float GetRadius(SerializedProperty ExtCylinderProperty)
        {
            return (ExtCylinderProperty.GetPropertie(PROPERTY_RADIUS).floatValue);
        }
        
    }
}