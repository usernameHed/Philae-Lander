using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Geometry.Editor
{
    public static class ExtShapeSerializeProperty
    {
        public static void MoveSplinePointFromSerializePropertie(SerializedProperty extSpline, Vector3 p1)
        {
            SerializedProperty listPointsOnSpline = extSpline.GetPropertie("_listPoints");
            listPointsOnSpline.arraySize = listPointsOnSpline.arraySize + 1;

            SerializedProperty point = listPointsOnSpline.GetArrayElementAtIndex(listPointsOnSpline.arraySize - 1);
            SerializedProperty localPoint = point.GetPropertie("PointLocal");
            SerializedProperty globalPoint = point.GetPropertie("PointGlobal");

            localPoint.vector3Value = p1;
            Matrix4x4 matrix = extSpline.GetPropertie("_splinesMatrix").GetValue<Matrix4x4>();
            globalPoint.vector3Value = matrix.MultiplyPoint3x4(p1);
        }
    }
}