using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Geometry.extensions
{
    public static class ExtMatrix
    {
        /// Creates a translation, rotation and scaling matrix.
        /// The returned matrix is such that places things at position pos, oriented in rotation q and scaled by s.
        ///                  row (horizontal)
        ///               |  0   1   2   3
        ///            ---+----------------
        ///            0  | m00 m10 m20 m30      //GetColumn(0)
        /// column     1  | m01 m11 m21 m31      //GetColumn(1)
        /// (vertical) 2  | m02 m12 m22 m32      //GetColumn(2)
        ///            3  | m03 m13 m23 m33      //GetColumn(3)
        /// </remarks>
        public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
        {
            Matrix4x4 result = new Matrix4x4();
            // Rotation and Scale
            // Quaternion multiplication can be used to represent rotation. 
            // If a quaternion is represented by qw + i qx + j qy + k qz , then the equivalent matrix for rotation is (including scale):
            // Remarks: https://forums.inovaestudios.com/t/math-combining-a-translation-rotation-and-scale-matrix-question-to-you-math-magicians/5194/2
            double sqw = q.w * q.w;
            double sqx = q.x * q.x;
            double sqy = q.y * q.y;
            double sqz = q.z * q.z;
            result.m00 = (float)(1 - 2 * sqy - 2 * sqz) * (float)s.x;
            result.m01 = (float)(2 * q.x * q.y - 2 * q.z * q.w);
            result.m02 = (float)(2 * q.x * q.z + 2 * q.y * q.w);
            result.m10 = (float)(2 * q.x * q.y + 2 * q.z * q.w);
            result.m11 = (float)(1 - 2 * sqx - 2 * sqz) * (float)s.y;
            result.m12 = (float)(2 * q.y * q.z - 2 * q.x * q.w);
            result.m20 = (float)(2 * q.x * q.z - 2 * q.y * q.w);
            result.m21 = (float)(2 * q.y * q.z + 2 * q.x * q.w);
            result.m22 = (float)(1 - 2 * sqx - 2 * sqy) * (float)s.z;
            // Translation
            result.m03 = (float)pos.x;
            result.m13 = (float)pos.y;
            result.m23 = (float)pos.z;
            result.m33 = 1.0f;
            return result;
        }

        /// <summary>
        /// apply a lerp between the 2 matrix by time
        /// // memory layout:
        //
        //                  row (horizontal)
        //               |  0   1   2   3
        //            ---+----------------
        //            0  | m00 m10 m20 m30      //GetColumn(0)
        // column     1  | m01 m11 m21 m31      //GetColumn(1)
        // (vertical) 2  | m02 m12 m22 m32      //GetColumn(2)
        //            3  | m03 m13 m23 m33      //GetColumn(3)
        /// </summary>
        /// <returns>Lerped matrix</returns>
        public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
        {
            Matrix4x4 ret = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                ret[i] = Mathf.Lerp(from[i], to[i], time);
            return ret;
        }

        public static Matrix4x4 GetMatrixOrthographic(float aspect, float orthographicSize, float nearClippingPlane, float farClippingPlane)
        {
            return (Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, nearClippingPlane, farClippingPlane));
        }
        public static Matrix4x4 GetMatrixPerspective(float aspect, float fieldOfView, float nearClippingPlane, float farClippingPlane)
        {
            return (Matrix4x4.Perspective(fieldOfView, aspect, nearClippingPlane, farClippingPlane));
        }

        /// <summary>
        /// from a Perspective projection matrix, get the fov
        /// </summary>
        /// <param name="projectionMatrix"></param>
        /// <returns></returns>
        public static float GetFOVFromPerspectiveProjectionMatrix(Matrix4x4 projectionMatrix)
        {
            float b = projectionMatrix[5];

            float RAD2DEG = 180.0f / 3.14159265358979323846f;
            float fov = RAD2DEG * (2.0f * (float)Mathf.Atan(1.0f / b));
            return (fov);
        }

        /// <summary>
        /// from a Perspective projection matrix, get the aspect ratio
        /// </summary>
        /// <param name="projectionMatrix"></param>
        /// <returns></returns>
        public static float GetAspectRatioFromPerspectiveProjectionMatrix(Matrix4x4 projectionMatrix)
        {
            float a = projectionMatrix[0];
            float b = projectionMatrix[5];
            float aspect_ratio = b / a;
            return (aspect_ratio);
        }

        /// <summary>
        /// from a Perspective projection matrix, get the clip min/max
        /// </summary>
        /// <param name="projectionMatrix"></param>
        /// <returns></returns>
        public static Vector2 GetClipMinMaxPerspectiveProjectionMatrix(Matrix4x4 projectionMatrix)
        {
            float c = projectionMatrix[10];
            float d = projectionMatrix[14];

            float k = (c - 1.0f) / (c + 1.0f);
            float clip_min = (d * (1.0f - k)) / (2.0f * k);
            float clip_max = k * clip_min;
            return (new Vector2(clip_min, clip_max));
        }

        public static void SetTRSFromMatrix(this Transform transform, Matrix4x4 matrix)
        {
            transform.localScale = matrix.ExtractScale();
            transform.rotation = matrix.ExtractRotation();
            transform.position = matrix.ExtractPosition();
        }

        public static Matrix4x4 GetMatrixTRS(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return (Matrix4x4.TRS(position, rotation, scale));
        }

        public static Matrix4x4 GetMatrixTRS(Transform transform)
        {
            return (GetMatrixTRS(transform.position, transform.rotation, transform.localScale));
        }

        public static Matrix4x4 SetPosition(this Matrix4x4 matrix, Vector3 position)
        {
            matrix.m03 = position.x;
            matrix.m13 = position.y;
            matrix.m23 = position.z;
            return (matrix);
        }

        public static Vector3 ExtractPosition(this Matrix4x4 matrix)
        {
            Vector3 position;// = m.GetColumn(3)
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }

        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            Vector3 forward = matrix.ForwardFast();
            Vector3 upwards = matrix.UpFast();

            return Quaternion.LookRotation(forward, upwards);
        }

        public static Matrix4x4 SetRositionOfTRS(this Matrix4x4 matrix, Quaternion rotation)
        {
            matrix = Matrix4x4.TRS(matrix.ExtractPosition(), rotation, matrix.ExtractScale());
            return (matrix);
        }

        public static Matrix4x4 SetScaleOfTRS(this Matrix4x4 matrix, Vector3 scale)
        {
            matrix = Matrix4x4.TRS(matrix.ExtractPosition(), matrix.ExtractRotation(), scale);
            return (matrix);
        }

        #region forward/Up/Right fast (non normalized)
        public static Vector3 ForwardFast(this Matrix4x4 matrix)
        {
            return (matrix.GetColumn(2));
        }
        public static Vector3 BackwardFast(this Matrix4x4 matrix)
        {
            return (-matrix.GetColumn(2));
        }

        public static Vector3 UpFast(this Matrix4x4 matrix)
        {
            return (matrix.GetColumn(1));
        }

        public static Vector3 DownFast(this Matrix4x4 matrix)
        {
            return (-matrix.GetColumn(1));
        }

        public static Vector3 RightFast(this Matrix4x4 matrix)
        {
            return (matrix.GetColumn(0));
        }

        public static Vector3 LeftFast(this Matrix4x4 matrix)
        {
            return (-matrix.GetColumn(0));
        }
        #endregion

        #region forward/Up/Right normalized
        public static Vector3 ForwardNormalized(this Matrix4x4 matrix)
        {
            return (matrix.ForwardFast().FastNormalized());
        }
        public static Vector3 BackwardNormalized(this Matrix4x4 matrix)
        {
            return (matrix.BackwardFast().FastNormalized());
        }

        public static Vector3 UpNormalized(this Matrix4x4 matrix)
        {
            return (matrix.UpFast().FastNormalized());
        }

        public static Vector3 DownNormalized(this Matrix4x4 matrix)
        {
            return (matrix.DownFast().FastNormalized());
        }

        public static Vector3 RightNormalized(this Matrix4x4 matrix)
        {
            return (matrix.RightFast().FastNormalized());
        }

        public static Vector3 LeftNormalized(this Matrix4x4 matrix)
        {
            return (matrix.LeftFast().FastNormalized());
        }
        #endregion

        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }

        /// <summary>
        /// usage to rotate around a transform using this:
        /// Matrix4x4 currentTRSTransform = ExtMatrix.GetMatrixTRS(_currentOrbitter);
        /// Matrix4x4 newMatrixTrs = ExtMatrix.RotateAround(currentTRSTransform, _pivotTransform.position, _offsetRotate, Time.deltaTime * _speed);
        /// _currentOrbitter.FromMatrix(newMatrixTrs);
        /// </summary>
        /// <param name="customTransform"></param>
        /// <param name="center"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix4x4 RotateAround(Matrix4x4 customTransform, Vector3 center, Vector3 axis, float angle)
        {
            Vector3 pos = customTransform.ExtractPosition();
            Quaternion rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
            Vector3 dir = pos - center; // find current direction relative to center
            dir = rot * dir; // rotate the direction

            Vector3 finalPosition = center + dir;

            // rotate object to keep looking at the center:
            Quaternion myRot = customTransform.ExtractRotation();
            Quaternion finalRotation = myRot * Quaternion.Inverse(myRot) * rot * myRot;

            Vector3 localScale = customTransform.ExtractScale();

            Matrix4x4 newTrsMatrix = ExtMatrix.GetMatrixTRS(finalPosition, finalRotation, localScale);
            return (newTrsMatrix);
        }

        public static void RotateAround(Transform transform, Vector3 center, Vector3 axis, float angle)
        {
            Vector3 pos = transform.position;
            Quaternion rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
            Vector3 dir = pos - center; // find current direction relative to center
            dir = rot * dir; // rotate the direction
            transform.position = center + dir; // define new position
                                               // rotate object to keep looking at the center:
            Quaternion myRot = transform.rotation;
            transform.rotation *= Quaternion.Inverse(myRot) * rot * myRot;
        }

        /// <summary>
        /// doesn't return a good TRS matrix, but a good rotation matrix !
        /// usage:
        /// Matrix4x4 rotationMatrix = ExtMatrix.LookAt(p1, p2, Vector3.up);
        /// Quaternion rotation = rotationMatrix.ExtractRotation();
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="upNormalized"></param>
        /// <returns></returns>
        public static Matrix4x4 LookAt(Vector3 from, Vector3 to, Vector3 upNormalized)
        {
            Vector3 forward = (from - to).FastNormalized();
            Vector3 right = Vector3.Cross(upNormalized, forward);
            upNormalized = Vector3.Cross(forward, right);

            Matrix4x4 rotation = new Matrix4x4();
            rotation.SetColumn(0, new Vector4(right.x, right.y, right.z, 1));
            rotation.SetColumn(1, new Vector4(upNormalized.x, upNormalized.y, upNormalized.z, 1));
            rotation.SetColumn(2, new Vector4(forward.x, forward.y, forward.z, 1));
            rotation.SetColumn(3, new Vector4(from.x, from.y, from.z, 1));
            return (rotation);
        }

        //end of class
    }
    //end of nameSpace
}