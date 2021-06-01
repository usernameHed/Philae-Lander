using UnityEditor;

namespace UnityEssentials.Geometry.Editor
{
    public static class ExtCylinderProperty
    {
        public const string PROPERTY_MOVABLE_CYLINDER = "_movableCylinder";

        public const string PROPERTY_EXT_CYLINDER = "_cylinder";
        public const string PROPERTY_MATRIX = "_cylinderMatrix";

        public const string PROPERTY_RADIUS = "_radius";
        public const string PROPERTY_RADIUS_SQUARED = "_radiusSquared";
        public const string PROPERTY_REAL_RADIUS = "_realRadius";
        public const string PROPERTY_REAL_RADIUS_SQUARED = "_realSquaredRadius";
        public const string PROPERTY_LENGHT = "_lenght";
        public const string PROPERTY_LENGHT_SQUARED = "_lenghtSquared";

        public const string PROPERTY_CIRCLE_1 = "_circle1";
        public const string PROPERTY_CIRCLE_2 = "_circle2";

        
        public static float GetRadius(SerializedProperty ExtCylinderProperty)
        {
            return (ExtCylinderProperty.GetPropertie(PROPERTY_RADIUS).floatValue);
        }
        
    }
}