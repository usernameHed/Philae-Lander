using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Geometry.Editor
{
    public static class ExtLineProperty
    {
        public const string PROPEPRTY_MOVABLE_POLY_LINE = "_movablePolyLines";

        public const string PROPEPRTY_POLY_EXT_LINE_3D = "_polyLines";
        public const string PROPERTY_POLY_LINE_MATRIX = "_polyLinesMatrix";
        public const string PROPERTY_LIST_LINES_LOCAL = "_listLinesLocal";
        public const string PROPERTY_LIST_LINES_GLOBAL = "_listLines";
        public const string PROPERTY_P1 = "_p1";
        public const string PROPERTY_P2 = "_p2";
        private const string PROPERTY_DELTA = "_delta";
        private const string PROPERTY_DELTA_SQUARED = "_deltaSquared";

        /// <summary>
        /// See MoveShape of ExtLine to know what's going on
        /// </summary>
        /// <param name="extLine">line to change</param>
        /// <param name="p1">new p1</param>
        /// <param name="p2">new p2</param>
        public static void MoveLineFromSerializeProperties(SerializedProperty extLine, Vector3 p1, Vector3 p2)
        {
            SerializedProperty p1Propertie = extLine.GetPropertie(PROPERTY_P1);
            SerializedProperty p2Propertie = extLine.GetPropertie(PROPERTY_P2);
            p1Propertie.vector3Value = p1;
            p2Propertie.vector3Value = p2;

            UpdateLineFromSerializeProperties(extLine);
        }

        /// <summary>
        /// See MoveShape of ExtLine to know what's going on
        /// </summary>
        /// <param name="extLine">line to change</param>
        /// <param name="p1">new p1</param>
        /// <param name="p2">new p2</param>
        public static void UpdateLineFromSerializeProperties(SerializedProperty extLine)
        {
            SerializedProperty p1Propertie = extLine.GetPropertie(PROPERTY_P1);
            SerializedProperty p2Propertie = extLine.GetPropertie(PROPERTY_P2);
            SerializedProperty delta = extLine.GetPropertie(PROPERTY_DELTA);
            SerializedProperty deltaSquared = extLine.GetPropertie(PROPERTY_DELTA_SQUARED);
            delta.vector3Value = p2Propertie.vector3Value - p1Propertie.vector3Value;
            deltaSquared.floatValue = Vector3.Dot(delta.vector3Value, delta.vector3Value);
        }
    }
}