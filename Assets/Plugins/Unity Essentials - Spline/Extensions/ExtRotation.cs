using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Spline.Extensions
{
    /// <summary>
    /// Rotation extensions
    /// </summary>
    public static class ExtRotation
    {
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
        public static Quaternion TurretLookRotation(Quaternion approximateForward, Vector3 exactUp)
        {
            Vector3 forwardQuaternion = approximateForward.GetForwardVector();

            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -forwardQuaternion);
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

        public static Quaternion OwnSmoothDamp3Axes(Quaternion rot, Quaternion target, ref Vector3 deriv, float time, float maxTime, float deltaTime)
        {
            Vector3 lookAtCurrentVector = rot.eulerAngles;// mainCamera.forward;
            Vector3 lookAtWantedVector = target.eulerAngles;

            Vector3 currentDamp = lookAtWantedVector;

            float z = deriv.z;
            float y = deriv.y;
            float x = deriv.x;

            currentDamp.z = SmoothDampAngle(lookAtCurrentVector.z, lookAtWantedVector.z, ref z, time, maxTime, deltaTime);
            currentDamp.y = SmoothDampAngle(lookAtCurrentVector.y, lookAtWantedVector.y, ref y, time, maxTime, deltaTime);
            currentDamp.x = SmoothDampAngle(lookAtCurrentVector.x, lookAtWantedVector.x, ref x, time, maxTime, deltaTime);

            deriv = new Vector3(x, y, z);

            Quaternion dampedRotation = Quaternion.Euler(currentDamp);

            return (dampedRotation);
        }
        public static Quaternion OwnSmoothDamp3Axes(Quaternion rot, Quaternion target, ref Vector3 deriv, Vector3 time, Vector3 maxTime, float deltaTime)
        {
            Vector3 lookAtCurrentVector = rot.eulerAngles;// mainCamera.forward;
            Vector3 lookAtWantedVector = target.eulerAngles;

            Vector3 currentDamp = lookAtWantedVector;

            float z = deriv.z;
            float y = deriv.y;
            float x = deriv.x;

            currentDamp.z = SmoothDampAngle(lookAtCurrentVector.z, lookAtWantedVector.z, ref z, time.z, maxTime.z, deltaTime);
            currentDamp.y = SmoothDampAngle(lookAtCurrentVector.y, lookAtWantedVector.y, ref y, time.y, maxTime.y, deltaTime);
            currentDamp.x = SmoothDampAngle(lookAtCurrentVector.x, lookAtWantedVector.x, ref x, time.x, maxTime.x, deltaTime);

            deriv = new Vector3(x, y, z);

            Quaternion dampedRotation = Quaternion.Euler(currentDamp);

            return (dampedRotation);
        }
        public static Quaternion OwnSmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
        {
            if (Time.deltaTime < Mathf.Epsilon) return rot;
            // account for double-cover
            var Dot = Quaternion.Dot(rot, target);
            var Multi = Dot > 0f ? 1f : -1f;
            target.x *= Multi;
            target.y *= Multi;
            target.z *= Multi;
            target.w *= Multi;
            // smooth damp (nlerp approx)
            var Result = new Vector4(
                Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
                Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
                Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
                Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
            ).normalized;

            // ensure deriv is tangent
            var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
            deriv.x -= derivError.x;
            deriv.y -= derivError.y;
            deriv.z -= derivError.z;
            deriv.w -= derivError.w;

            return new Quaternion(Result.x, Result.y, Result.z, Result.w);
        }

        private static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            target = current + DeltaAngle(current, target);
            return (ExtVector3.OwnSmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime));
        }

        public static float DeltaAngle(float current, float target)
        {
            float delta = Mathf.Repeat((target - current), 360.0F);
            if (delta > 180.0F)
                delta -= 360.0F;
            return delta;
        }
    }
}