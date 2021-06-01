using UnityEngine;

namespace UnityEssentials.Geometry.extensions
{
    /// <summary>
    /// Vector Extension for spline
    /// </summary>
    public static class ExtQuaternion
    {
        /// <summary>
        /// From a Line, Get the quaternion representing the Vector p2 - p1
        /// the up vector can be Vector3.up if you have no reference.
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">point 2</param>
        /// <param name="upNormalized">default is Vector3.up</param>
        /// <returns>Quaternion representing the rotation of p2 - p1</returns>
        public static Quaternion QuaternionFromLine(Vector3 p1, Vector3 p2, Vector3 upNormalized)
        {
            Matrix4x4 rotationMatrix = ExtMatrix.LookAt(p1, p2, upNormalized);
            Quaternion rotation = rotationMatrix.ExtractRotation();
            return (rotation);
        }
    }
}
