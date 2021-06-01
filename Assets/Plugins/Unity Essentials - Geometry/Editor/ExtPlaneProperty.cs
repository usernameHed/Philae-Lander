using UnityEditor;

namespace UnityEssentials.Geometry.Editor
{
    public static class ExtPlaneProperty
    {
        public const string PROPERTY_PLANE_3D = "_plane3d";
        public const string PROPERTY_MATRIX = "_planeMatrix";

        public const string PROPERTY_EXT_PLANE = "_plane";
        public const string PROPERTY_ALLOW_BOTTOM = "_allowBottom";

        public static bool GetAllowDown(SerializedProperty extPlane3d)
        {
            return (extPlane3d.GetPropertie(PROPERTY_EXT_PLANE).GetPropertie(PROPERTY_ALLOW_BOTTOM).boolValue);
        }

        public static void SetAllowDown(SerializedProperty extQuad, bool newAllowDown)
        {
            extQuad.GetPropertie(PROPERTY_EXT_PLANE).GetPropertie(PROPERTY_ALLOW_BOTTOM).boolValue = newAllowDown;
        }
    }
}