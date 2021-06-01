using UnityEssentials.Geometry.shape3d;
using UnityEditor;

namespace UnityEssentials.Geometry.Editor
{
    public static class ExtDonutProperty
    {
        public const string PROPERTY_MOVABLE_DONUT = "_movableDonut";

        public const string PROPERTY_THICKNESS = "_thickNess";
        public const string PROPERTY_REAL_THICKNESS = "_realThickNess";

        public const string PROPERTY_RADIUS = "_radius";
        public const string PROPERTY_RADIUS_SQUARED = "_radiusSquared";
        public const string PROPERTY_REAL_RADIUS = "_realRadius";
        public const string PROPERTY_REAL_RADIUS_SQUARED = "_realSquaredRadius";

        public const string PROPERTY_MATRIX = "_donutMatrix";

        public static float GetRadius(this SerializedProperty extDonut)
        {
            return (extDonut.GetPropertie(PROPERTY_RADIUS).floatValue);
        }

        public static float GetThickNess(SerializedProperty extDonut)
        {
            return (extDonut.GetPropertie(PROPERTY_THICKNESS).floatValue);
        }

        public static void SetRadius(SerializedProperty extDonut, float newRadius)
        {
            extDonut.GetPropertie(PROPERTY_RADIUS).floatValue = newRadius;
        }

        public static void SetThickNess(SerializedProperty extDonut, float newThickNess)
        {
            ExtDonut donut = extDonut.GetValue<ExtDonut>();

            extDonut.GetPropertie(PROPERTY_THICKNESS).floatValue = newThickNess;
            extDonut.GetPropertie(PROPERTY_REAL_THICKNESS).floatValue = newThickNess * donut.MaxXY(donut.LocalScale);
        }
    }
}