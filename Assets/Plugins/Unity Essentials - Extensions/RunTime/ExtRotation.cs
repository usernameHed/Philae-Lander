using UnityEngine;

namespace UnityEssentials.Extensions
{
    /// <summary>
    /// Rotation extensions
    /// </summary>
    public static class ExtRotation
    {
        #region Direction
        public enum TurnType
        {
            X,
            Y,
            Z,
            ALL,
        }

        public static Quaternion QuaternionLookRotation(Vector3 forward, Vector3 up)
        {
            Vector3 vector = Vector3.Normalize(forward);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(up, vector));
            Vector3 vector3 = Vector3.Cross(vector, vector2);
            var m00 = vector2.x;
            var m01 = vector2.y;
            var m02 = vector2.z;
            var m10 = vector3.x;
            var m11 = vector3.y;
            var m12 = vector3.z;
            var m20 = vector.x;
            var m21 = vector.y;
            var m22 = vector.z;


            float num8 = (m00 + m11) + m22;
            var quaternion = new Quaternion();
            if (num8 > 0f)
            {
                var num = (float)Mathf.Sqrt(num8 + 1f);
                quaternion.w = num * 0.5f;
                num = 0.5f / num;
                quaternion.x = (m12 - m21) * num;
                quaternion.y = (m20 - m02) * num;
                quaternion.z = (m01 - m10) * num;
                return quaternion;
            }
            if ((m00 >= m11) && (m00 >= m22))
            {
                var num7 = (float)Mathf.Sqrt(((1f + m00) - m11) - m22);
                var num4 = 0.5f / num7;
                quaternion.x = 0.5f * num7;
                quaternion.y = (m01 + m10) * num4;
                quaternion.z = (m02 + m20) * num4;
                quaternion.w = (m12 - m21) * num4;
                return quaternion;
            }
            if (m11 > m22)
            {
                var num6 = (float)Mathf.Sqrt(((1f + m11) - m00) - m22);
                var num3 = 0.5f / num6;
                quaternion.x = (m10 + m01) * num3;
                quaternion.y = 0.5f * num6;
                quaternion.z = (m21 + m12) * num3;
                quaternion.w = (m20 - m02) * num3;
                return quaternion;
            }
            var num5 = (float)Mathf.Sqrt(((1f + m22) - m00) - m11);
            var num2 = 0.5f / num5;
            quaternion.x = (m20 + m02) * num2;
            quaternion.y = (m21 + m12) * num2;
            quaternion.z = 0.5f * num5;
            quaternion.w = (m01 - m10) * num2;
            return quaternion;
        }


        /// <summary>
        /// Create a LookRotation for a non-standard 'forward' axis.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="forwardAxis"></param>
        /// <returns></returns>
        public static Quaternion AltForwardLookRotation(Vector3 dir, Vector3 forwardAxis, Vector3 upAxis)
        {
            //return Quaternion.LookRotation(dir, upAxis) * Quaternion.FromToRotation(forwardAxis, Vector3.forward);
            return Quaternion.LookRotation(dir) * Quaternion.Inverse(Quaternion.LookRotation(forwardAxis, upAxis));
        }

        /// <summary>
        /// Get the rotated forward axis based on some base forward.
        /// </summary>
        /// <param name="rot">The rotation</param>
        /// <param name="baseForward">Forward with no rotation</param>
        /// <returns></returns>
        public static Vector3 GetAltForward(Quaternion rot, Vector3 baseForward)
        {
            return rot * baseForward;
        }

        /// <summary>
        /// Returns a rotation of up attempting to face in the general direction of forward.
        /// </summary>
        /// <param name="up"></param>
        /// <param name="targForward"></param>
        /// <returns></returns>
        public static Quaternion FaceRotation(Vector3 forward, Vector3 up)
        {
            forward = ExtVector3.GetForwardTangent(forward, up);
            return Quaternion.LookRotation(forward, up);
        }

        //This is an alternative for Quaternion.LookRotation. Instead of aligning the forward and up vector of the game 
        //object with the input vectors, a custom direction can be used instead of the fixed forward and up vectors.
        //alignWithVector and alignWithNormal are in world space.
        //customForward and customUp are in object space.
        //Usage: use alignWithVector and alignWithNormal as if you are using the default LookRotation function.
        //Set customForward and customUp to the vectors you wish to use instead of the default forward and up vectors.
        public static void LookRotationExtended(ref GameObject gameObjectInOut, Vector3 alignWithVector, Vector3 alignWithNormal, Vector3 customForward, Vector3 customUp)
        {

            //Set the rotation of the destination
            Quaternion rotationA = Quaternion.LookRotation(alignWithVector, alignWithNormal);

            //Set the rotation of the custom normal and up vectors. 
            //When using the default LookRotation function, this would be hard coded to the forward and up vector.
            Quaternion rotationB = Quaternion.LookRotation(customForward, customUp);

            //Calculate the rotation
            gameObjectInOut.transform.rotation = rotationA * Quaternion.Inverse(rotationB);
        }

        //Returns the up vector of a quaternion
        public static Vector3 GetUpVector(Quaternion q)
        {
            return q * Vector3.up;
        }

        //Returns the right vector of a quaternion
        public static Vector3 GetRightVector(Quaternion q)
        {
            return q * Vector3.right;
        }

        public static void GetAngleAxis(this Quaternion q, out Vector3 axis, out float angle)
        {
            if (q.w > 1) q = ExtQuaternion.Normalize(q);

            //get as doubles for precision
            var qw = (double)q.w;
            var qx = (double)q.x;
            var qy = (double)q.y;
            var qz = (double)q.z;
            var ratio = System.Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * System.Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }

        public static void GetShortestAngleAxisBetween(Quaternion a, Quaternion b, out Vector3 axis, out float angle)
        {
            var dq = Quaternion.Inverse(a) * b;
            if (dq.w > 1) dq = ExtQuaternion.Normalize(dq);

            //get as doubles for precision
            var qw = (double)dq.w;
            var qx = (double)dq.x;
            var qy = (double)dq.y;
            var qz = (double)dq.z;
            var ratio = System.Math.Sqrt(1.0d - qw * qw);

            angle = (float)(2.0d * System.Math.Acos(qw)) * Mathf.Rad2Deg;
            if (ratio < 0.001d)
            {
                axis = new Vector3(1f, 0f, 0f);
            }
            else
            {
                axis = new Vector3(
                    (float)(qx / ratio),
                    (float)(qy / ratio),
                    (float)(qz / ratio));
                axis.Normalize();
            }
        }

        /// <summary>
        /// Two quaternions can represent different rotations that lead to the same final orientation (one rotating around Axis with Angle, the other around -Axis with 2Pi-Angle). In this case, the quaternion == operator will return false. This method will return true.
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static bool SameOrientation(this Quaternion q1, Quaternion q2)
        {
            return Mathf.Abs((float)Quaternion.Dot(q1, q2)) > 0.999998986721039;
        }

        /// <summary>
        /// Two quaternions can represent different rotations that lead to the same final orientation (one rotating around Axis with Angle, the other around -Axis with 2Pi-Angle). In this case, the quaternion != operator will return true. This method will return false.
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <returns></returns>
        public static bool DifferentOrientation(this Quaternion q1, Quaternion q2)
        {
            return Mathf.Abs((float)Quaternion.Dot(q1, q2)) <= 0.999998986721039;
        }

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

        /*
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
        */
        

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
        public static Vector3 TurretLookRotationVector(Vector3 approximateForward, Vector3 exactUp)
        {
            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -approximateForward);
            Quaternion rotateYToZ = Quaternion.Euler(90f, 0f, 0f);

            return (rotateZToUp * rotateYToZ) * Vector3.forward;
        }
        public static Quaternion TurretLookRotation(Quaternion approximateForward, Vector3 exactUp)
        {
            Vector3 forwardQuaternion = approximateForward.GetForwardVector();

            Quaternion rotateZToUp = Quaternion.LookRotation(exactUp, -forwardQuaternion);
            Quaternion rotateYToZ = Quaternion.Euler(90f, 0f, 0f);

            return rotateZToUp * rotateYToZ;
        }
        public static Quaternion SmoothTurretLookRotation(Vector3 approximateForward, Vector3 exactUp,
            Quaternion objCurrentRotation, float maxDegreesPerSecond)
        {
            Quaternion desiredOrientation = TurretLookRotation(approximateForward, exactUp);
            Quaternion smoothOrientation = Quaternion.RotateTowards(
                                        objCurrentRotation,
                                        desiredOrientation,
                                        maxDegreesPerSecond * Time.deltaTime
                                     );
            return (smoothOrientation);
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

        /// <summary>
        /// rotate an object in 2D coordonate
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="dir"></param>
        /// <param name="turnRate"></param>
        /// <param name="typeRotation"></param>
        /// <returns></returns>
        public static Quaternion DirObject2d(this Quaternion rotation, Vector2 dir, float turnRate, out Quaternion targetRotation, TurnType typeRotation = TurnType.Z)
        {
            float heading = Mathf.Atan2(-dir.x * turnRate * Time.deltaTime, dir.y * turnRate * Time.deltaTime);

            targetRotation = Quaternion.identity;

            float x = (typeRotation == TurnType.X) ? heading * 1 * Mathf.Rad2Deg : 0;
            float y = (typeRotation == TurnType.Y) ? heading * -1 * Mathf.Rad2Deg : 0;
            float z = (typeRotation == TurnType.Z) ? heading * -1 * Mathf.Rad2Deg : 0;

            targetRotation = Quaternion.Euler(x, y, z);
            rotation = Quaternion.RotateTowards(rotation, targetRotation, turnRate * Time.deltaTime);
            return (rotation);
        }

        /// Rotates a 2D object to face a target
        /// </summary>
        /// <param name="target">transform to look at</param>
        /// <param name="isXAxisForward">when true, the x axis of the transform is aligned to look at the target</param>
        public static void LookAt2D(this Transform transform, Vector2 target, bool isXAxisForward = true)
        {
            target = target - (Vector2)transform.position;
            float currentRotation = transform.eulerAngles.z;
            if (isXAxisForward)
            {
                if (target.x > 0)
                {
                    transform.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan(target.y / target.x) - currentRotation));
                }
                else if (target.x < 0)
                {
                    transform.Rotate(new Vector3(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan(target.y / target.x) - currentRotation));
                }
                else
                {
                    transform.Rotate(new Vector3(0, 0, (target.y > 0 ? 90 : -90) - currentRotation));
                }
            }
            else
            {
                if (target.y > 0)
                {
                    transform.Rotate(new Vector3(0, 0, -Mathf.Rad2Deg * Mathf.Atan(target.x / target.y) - currentRotation));
                }
                else if (target.y < 0)
                {
                    transform.Rotate(new Vector3(0, 0, 180 - Mathf.Rad2Deg * Mathf.Atan(target.x / target.y) - currentRotation));
                }
                else
                {
                    transform.Rotate(new Vector3(0, 0, (target.x > 0 ? 90 : -90) - currentRotation));
                }
            }
        }

        /// <summary>
        /// rotate smoothly selon 2 axe
        /// </summary>
        public static Quaternion DirObject(this Quaternion rotation, float horizMove, float vertiMove, float turnRate, out Quaternion _targetRotation, TurnType typeRotation = TurnType.Z)
        {
            float heading = Mathf.Atan2(horizMove * turnRate * Time.deltaTime, -vertiMove * turnRate * Time.deltaTime);

            //Quaternion _targetRotation = Quaternion.identity;

            float x = (typeRotation == TurnType.X) ? heading * 1 * Mathf.Rad2Deg : 0;
            float y = (typeRotation == TurnType.Y) ? heading * -1 * Mathf.Rad2Deg : 0;
            float z = (typeRotation == TurnType.Z) ? heading * -1 * Mathf.Rad2Deg : 0;

            _targetRotation = Quaternion.Euler(x, y, z);
            rotation = Quaternion.RotateTowards(rotation, _targetRotation, turnRate * Time.deltaTime);
            return (rotation);
        }

        public static Vector3 DirLocalObject(Vector3 rotation, Vector3 dirToGo, float turnRate, TurnType typeRotation = TurnType.Z)
        {
            Vector3 returnRotation = rotation;
            float x = returnRotation.x;
            float y = returnRotation.y;
            float z = returnRotation.z;

            //Debug.Log("Y current: " + y + ", y to go: " + dirToGo.y);



            x = (typeRotation == TurnType.X || typeRotation == TurnType.ALL) ? Mathf.LerpAngle(returnRotation.x, dirToGo.x, Time.deltaTime * turnRate) : x;
            y = (typeRotation == TurnType.Y || typeRotation == TurnType.ALL) ? Mathf.LerpAngle(returnRotation.y, dirToGo.y, Time.deltaTime * turnRate) : y;
            z = (typeRotation == TurnType.Z || typeRotation == TurnType.ALL) ? Mathf.LerpAngle(returnRotation.z, dirToGo.z, Time.deltaTime * turnRate) : z;

            //= Vector3.Lerp(rotation, dirToGo, Time.deltaTime * turnRate);
            return (new Vector3(x, y, z));
        }

        /// <summary>
        /// A cleaner version of FromToRotation, Quaternion.FromToRotation for some reason can only handle down to #.## precision.
        /// This will result in true 7 digits of precision down to depths of 0.00000# (depth tested so far).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Quaternion FromToRotation(Vector3 v1, Vector3 v2)
        {
            Vector3 a = Vector3.Cross(v1, v2);
            float w = Mathf.Sqrt(v1.sqrMagnitude * v2.sqrMagnitude) + Vector3.Dot(v1, v2);
            return new Quaternion(a.x, a.y, a.z, w);
        }

        /// <summary>
        /// Get the rotation that would be applied to 'start' to end up at 'end'.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Quaternion FromToRotation(Quaternion start, Quaternion end)
        {
            return Quaternion.Inverse(start) * end;
        }

        //Rotate a vector as if it is attached to an object with rotation "from", which is then rotated to rotation "to".
        //Similar to TransformWithParent(), but rotating a vector instead of a transform.
        public static Vector3 RotateVectorFromTo(Quaternion from, Quaternion to, Vector3 vector)
        {
            //Note: comments are in case all inputs are in World Space.
            Quaternion Q = ExtQuaternion.SubtractRotation(to, from);              //Output is in object space.
            Vector3 A = ExtQuaternion.InverseTransformDirectionMath(from, vector);//Output is in object space.
            Vector3 B = Q * A;                                      //Output is in local space.
            Vector3 C = ExtQuaternion.TransformDirectionMath(from, B);            //Output is in world space.
            return C;
        }
        #endregion

        #region Combine
        /// <summary>
        /// get an array of rotation from a list of transform
        /// </summary>
        public static Quaternion[] GetAllRotation(Transform[] rotations)
        {
            Quaternion[] array = new Quaternion[rotations.Length];
            for (int i = 0; i < rotations.Length; i++)
            {
                array[i] = rotations[i].rotation;
            }
            return (array);
        }

        // assuming qArray.Length > 1
        public static Quaternion AverageQuaternion(Quaternion[] qArray)
        {
            Quaternion qAvg = qArray[0];
            float weight;
            for (int i = 1; i < qArray.Length; i++)
            {
                weight = 1.0f / (float)(i + 1);
                qAvg = Quaternion.Slerp(qAvg, qArray[i], weight);
            }
            return qAvg;
        }

        //Get an average (mean) from more then two quaternions (with two, slerp would be used).
        //Note: this only works if all the quaternions are relatively close together.
        //Usage: 
        //-Cumulative is an external Vector4 which holds all the added x y z and w components.
        //-newRotation is the next rotation to be added to the average pool
        //-firstRotation is the first quaternion of the array to be averaged
        //-addAmount holds the total amount of quaternions which are currently added
        //This function returns the current average quaternion
        public static Quaternion AverageQuaternion(ref Vector4 cumulative, Quaternion newRotation, Quaternion firstRotation, int addAmount)
        {
            float w = 0.0f;
            float x = 0.0f;
            float y = 0.0f;
            float z = 0.0f;

            //Before we add the new rotation to the average (mean), we have to check whether the quaternion has to be inverted. Because
            //q and -q are the same rotation, but cannot be averaged, we have to make sure they are all the same.
            if (!ExtQuaternion.IsClose(newRotation, firstRotation))
            {
                newRotation = ExtQuaternion.InverseSignQuaternion(newRotation);
            }

            //Average the values
            float addDet = 1f / (float)addAmount;
            cumulative.w += newRotation.w;
            w = cumulative.w * addDet;
            cumulative.x += newRotation.x;
            x = cumulative.x * addDet;
            cumulative.y += newRotation.y;
            y = cumulative.y * addDet;
            cumulative.z += newRotation.z;
            z = cumulative.z * addDet;

            //note: if speed is an issue, you can skip the normalization step
            return ExtQuaternion.NormalizeQuaternion(x, y, z, w);
        }

        #endregion

        #region lerp
        public static Quaternion SpeedSlerp(Quaternion from, Quaternion to, float angularSpeed, float dt, bool bUseRadians = false)
        {
            if (bUseRadians) angularSpeed *= Mathf.Rad2Deg;
            var da = angularSpeed * dt;
            return Quaternion.RotateTowards(from, to, da);
        }
        /// <summary>
        /// Lerp a rotation
        /// </summary>
        /// <param name="currentRotation"></param>
        /// <param name="desiredRotation"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Quaternion LerpRotation(Quaternion currentRotation, Quaternion desiredRotation, float speed)
        {
            return (Quaternion.Lerp(currentRotation, desiredRotation, Time.time * speed));
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
        private static float SmoothDampAngle(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            target = current + DeltaAngle(current, target);
            return (ExtVector3.OwnSmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime));
        }

        // Calculates the shortest difference between two given angles.
        public static float DeltaAngle(float current, float target)
        {
            float delta = Mathf.Repeat((target - current), 360.0F);
            if (delta > 180.0F)
                delta -= 360.0F;
            return delta;
        }

        public static Quaternion OwnSmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time, float deltaTime)
        {
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
            // compute deriv
            var dtInv = 1f / deltaTime;
            deriv.x = (Result.x - rot.x) * dtInv;
            deriv.y = (Result.y - rot.y) * dtInv;
            deriv.z = (Result.z - rot.z) * dtInv;
            deriv.w = (Result.w - rot.w) * dtInv;
            return new Quaternion(Result.x, Result.y, Result.z, Result.w);
        }

        public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
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

        #endregion

        #region clamp
        /// <summary>
        /// smooth look rotation with clamped angle
        /// </summary>
        /// <param name="referenceRotation">reference rotation up, forward, right</param>
        /// <param name="vectorDirectorToTarget">vectorDirector where we want to target</param>
        /// <param name="left">angle in degree: clamp left from Up Axis</param>
        /// <param name="right">angle in degree: clamp right from Up axis</param>
        /// <param name="up">angle in degree: clamp Up axis</param>
        /// <param name="down">angle in degree: clamp Down axis</param>
        /// <param name="currentRotation">Current rotation of the object we want to smoothly rotate</param>
        /// <param name="maxDegreesPerSecond">speed of rotation in degree per seconds</param>
        /// <returns>Smooth clamped rotation</returns>
        public static Quaternion SmoothTurretLookRotationWithClampedAxis(
            Quaternion referenceRotation,
            Vector3 vectorDirectorToTarget,
            float left,
            float right,
            float up,
            float down,
            Quaternion currentRotation,
            float maxDegreesPerSecond)
        {
            Quaternion finalHeadRotation = ExtRotation.TurrentLookRotationWithClampedAxis(referenceRotation, vectorDirectorToTarget, left, right, up, down);
            Quaternion smoothOrientation = Quaternion.RotateTowards(currentRotation,
                finalHeadRotation,
                maxDegreesPerSecond * Time.fixedDeltaTime);

            return (smoothOrientation);
        }

        /// <summary>
        /// turret look rotation with clamped angles
        /// </summary>
        /// <param name="referenceRotation">reference rotation up, forward, right</param>
        /// <param name="vectorDirectorToTarget">vectorDirector where we want to target</param>
        /// <param name="left">angle in degree: clamp left from Up Axis</param>
        /// <param name="right">angle in degree: clamp right from Up axis</param>
        /// <param name="up">angle in degree: clamp Up axis</param>
        /// <param name="down">angle in degree: clamp Down axis</param>
        /// <returns>Clamped rotation</returns>
        public static Quaternion TurrentLookRotationWithClampedAxis(
            Quaternion referenceRotation,
            Vector3 vectorDirectorToTarget,
            float left,
            float right,
            float up,
            float down)
        {
            Vector3 originalForward = referenceRotation * Vector3.forward;

            Vector3 yAxis = Vector3.up; // world y axis
            Vector3 dirXZ = Vector3.ProjectOnPlane(vectorDirectorToTarget, yAxis);
            Vector3 forwardXZ = Vector3.ProjectOnPlane(originalForward, yAxis);
            float yAngle = Vector3.Angle(dirXZ, forwardXZ) * Mathf.Sign(Vector3.Dot(yAxis, Vector3.Cross(forwardXZ, dirXZ)));
            float yClamped = Mathf.Clamp(yAngle, left, right);
            Quaternion yRotation = Quaternion.AngleAxis(yClamped, Vector3.up);

            originalForward = yRotation * referenceRotation * Vector3.forward;
            Vector3 xAxis = yRotation * referenceRotation * Vector3.right; // our local x axis
            Vector3 dirYZ = Vector3.ProjectOnPlane(vectorDirectorToTarget, xAxis);
            Vector3 forwardYZ = Vector3.ProjectOnPlane(originalForward, xAxis);
            float xAngle = Vector3.Angle(dirYZ, forwardYZ) * Mathf.Sign(Vector3.Dot(xAxis, Vector3.Cross(forwardYZ, dirYZ)));
            float xClamped = Mathf.Clamp(xAngle, -up, -down);
            Quaternion xRotation = Quaternion.AngleAxis(xClamped, Vector3.right);


            Quaternion newRotation = yRotation * referenceRotation * xRotation;
            return (newRotation);
        }
        #endregion
    }
}
