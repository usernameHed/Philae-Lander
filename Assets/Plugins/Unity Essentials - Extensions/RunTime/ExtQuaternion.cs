using UnityEngine;

namespace UnityEssentials.Extensions
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

        /// <summary>
        /// is a quaternion NaN
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static bool IsNaN(Quaternion q)
        {
            return float.IsNaN(q.x * q.y * q.z * q.w);
        }

        public static string Stringify(Quaternion q)
        {
            return q.x.ToString() + "," + q.y.ToString() + "," + q.z.ToString() + "," + q.w.ToString();
        }

        public static string ToDetailedString(this Quaternion v)
        {
            return System.String.Format("<{0}, {1}, {2}, {3}>", v.x, v.y, v.z, v.w);
        }

        /// <summary>
        /// normalise a quaternion
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Quaternion Normalize(Quaternion q)
        {
            var mag = System.Math.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
            q.w = (float)((double)q.w / mag);
            q.x = (float)((double)q.x / mag);
            q.y = (float)((double)q.y / mag);
            q.z = (float)((double)q.z / mag);
            return q;
        }

        ///returns quaternion raised to the power pow. This is useful for smoothly multiplying a Quaternion by a given floating-point value.
        ///transform.rotation = rotateOffset.localRotation.Pow(Time.time);
        public static Quaternion Pow(this Quaternion input, float power)
        {
            float inputMagnitude = input.Magnitude();
            Vector3 nHat = new Vector3(input.x, input.y, input.z).normalized;
            Quaternion vectorBit = new Quaternion(nHat.x, nHat.y, nHat.z, 0)
                .ScalarMultiply(power * Mathf.Acos(input.w / inputMagnitude))
                    .Exp();
            return vectorBit.ScalarMultiply(Mathf.Pow(inputMagnitude, power));
        }

        ///returns euler's number raised to quaternion
        public static Quaternion Exp(this Quaternion input)
        {
            float inputA = input.w;
            Vector3 inputV = new Vector3(input.x, input.y, input.z);
            float outputA = Mathf.Exp(inputA) * Mathf.Cos(inputV.magnitude);
            Vector3 outputV = Mathf.Exp(inputA) * (inputV.normalized * Mathf.Sin(inputV.magnitude));
            return new Quaternion(outputV.x, outputV.y, outputV.z, outputA);
        }

        ///returns the float magnitude of quaternion
        public static float Magnitude(this Quaternion input)
        {
            return Mathf.Sqrt(input.x * input.x + input.y * input.y + input.z * input.z + input.w * input.w);
        }

        ///returns quaternion multiplied by scalar
        public static Quaternion ScalarMultiply(this Quaternion input, float scalar)
        {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }

        //Returns true if the two input quaternions are close to each other. This can
        //be used to check whether or not one of two quaternions which are supposed to
        //be very similar but has its component signs reversed (q has the same rotation as
        //-q)
        public static bool IsClose(Quaternion q1, Quaternion q2)
        {
            float dot = Quaternion.Dot(q1, q2);

            if (dot < 0.0f)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Changes the sign of the quaternion components. This is not the same as the inverse.
        public static Quaternion InverseSignQuaternion(Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, -q.w);
        }

        public static Quaternion NormalizeQuaternion(float x, float y, float z, float w)
        {

            float lengthD = 1.0f / (w * w + x * x + y * y + z * z);
            w *= lengthD;
            x *= lengthD;
            y *= lengthD;
            z *= lengthD;

            return new Quaternion(x, y, z, w);
        }

        //caclulate the rotational difference from A to B
        public static Quaternion SubtractRotation(Quaternion B, Quaternion A)
        {
            Quaternion C = Quaternion.Inverse(A) * B;
            return C;
        }

        //Add rotation B to rotation A.
        public static Quaternion AddRotation(Quaternion A, Quaternion B)
        {
            Quaternion C = A * B;
            return C;
        }

        //Same as the build in TransformDirection(), but using a rotation instead of a transform.
        public static Vector3 TransformDirectionMath(Quaternion rotation, Vector3 vector)
        {

            Vector3 output = rotation * vector;
            return output;
        }

        //Same as the build in InverseTransformDirection(), but using a rotation instead of a transform.
        public static Vector3 InverseTransformDirectionMath(Quaternion rotation, Vector3 vector)
        {
            Vector3 output = Quaternion.Inverse(rotation) * vector;
            return output;
        }
    }
}
