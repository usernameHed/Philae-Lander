using UnityEngine;

namespace UnityEssentials.Geometry.extensions
{
    /// <summary>
    /// Rotation extensions
    /// </summary>
    public static class ExtRotation
    {
        /// <summary>
        /// rotate a given quaternion in x, y and z
        /// </summary>
        /// <param name="currentQuaternion">current quaternion to rotate</param>
        /// <param name="axis">axis of rotation in degree</param>
        /// <returns>new rotated quaternion</returns>
        public static Quaternion RotateQuaternion(Quaternion currentQuaternion, Vector3 axis)
        {
            return (currentQuaternion * Quaternion.Euler(axis));
        }

        /// <summary>
        /// From a Vector director, Get the quaternion representing this vector
        /// the up vector can be Vector3.up if you have no reference.
        /// </summary>
        /// <param name="vectorDirector"></param>
        /// <param name="upNormalized">default is Vector3.up</param>
        /// <returns>Quaternion representing the rotation of p2 - p1</returns>
        public static Quaternion QuaternionFromVectorDirector(Vector3 vectorDirector, Vector3 upNormalized)
        {
            Matrix4x4 rotationMatrix = ExtMatrix.LookAt(vectorDirector, vectorDirector * 2, upNormalized);
            Quaternion rotation = rotationMatrix.ExtractRotation();
            return (rotation);
        }

        public static Quaternion TurretLookRotation(Quaternion approximateForward, Vector3 exactUp)
        {
            Vector3 forwardQuaternion = approximateForward.GetForwardVector();

            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -forwardQuaternion);
            Quaternion rotateYToZ = Quaternion.Euler(90f, 0f, 0f);

            return rotateZToUp * rotateYToZ;
        }

        /// <summary>
        /// Turret lock rotation
        /// https://gamedev.stackexchange.com/questions/167389/unity-smooth-local-rotation-around-one-axis-oriented-toward-a-target/167395#167395
        /// 
        /// Vector3 relativeDirection = mainReferenceObjectDirection.right * dirInput.x + mainReferenceObjectDirection.forward * dirInput.y;
        /// Vector3 up = objectToRotate.up;
        /// Quaternion desiredOrientation = TurretLookRotation(relativeDirection, up);
        ///objectToRotate.rotation = Quaternion.RotateTowards(
        ///                         objectToRotate.rotation,
        ///                         desiredOrientation,
        ///                         turnRate* Time.deltaTime
        ///                        );
        /// </summary>
        public static Quaternion TurretLookRotation(Vector3 approximateForward, Vector3 exactUp)
        {
            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -approximateForward);
            Quaternion rotateYToZ = Quaternion.Euler(90f, 0f, 0f);

            return rotateZToUp * rotateYToZ;
        }

        /// <summary>
        /// Returns the forward vector of a quaternion
        /// </summary>
        /// <param name="q">quaternion</param>
        /// <returns>forward vector</returns>
        public static Vector3 GetForwardVector(this Quaternion q)
        {
            return q * Vector3.forward;
        }
    }
}
