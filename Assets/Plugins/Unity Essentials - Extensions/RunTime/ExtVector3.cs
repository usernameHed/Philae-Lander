using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Extensions
{
    /// <summary>
    /// Rotation extensions
    /// </summary>
    public static class ExtVector3
    {
        public static readonly Vector3[] GeneralDirections = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward, Vector3.left, Vector3.down, Vector3.back };

        #region Vector (direction)
        /// <summary>
        /// return if we are right or left from a vector. 1: right, -1: left, 0: forward
        /// </summary>
        /// <param name="forwardDir"></param>
        /// <param name="upDir">up reference of the forward dir</param>
        /// <param name="toGoDir">target direction to test</param>
        public static int IsRightOrLeft(Vector3 forwardDir, Vector3 upDir, Vector3 toGoDir, ref float dotLeft, ref float dotRight)
        {
            Vector3 left = Vector3.Cross(forwardDir, upDir);
            Vector3 right = -left;
            dotRight = Vector3.Dot(right, toGoDir);
            dotLeft = Vector3.Dot(left, toGoDir);
            if (dotRight > 0)
            {
                return (1);
            }
            else if (dotLeft > 0)
            {
                return (-1);
            }
            return (0);
        }

        /// <summary>
        /// return if we are right or left from a vector. 1: right, -1: left, 0: forward
        /// </summary>
        /// <param name="forwardDir"></param>
        /// <param name="upDir">up reference of the forward dir</param>
        /// <param name="toGoDir">target direction to test</param>
        public static int IsRightOrLeft(Vector3 forwardDir, Vector3 upDir, Vector3 toGoDir)
        {
            Vector3 left = Vector3.Cross(forwardDir, upDir);
            Vector3 right = -left;
            float dotRight = Vector3.Dot(right, toGoDir);
            float dotLeft = Vector3.Dot(left, toGoDir);
            if (dotRight > 0)
            {
                return (1);
            }
            else if (dotLeft > 0)
            {
                return (-1);
            }
            return (0);
        }

        /// <summary>
        /// Resize a vector director by a given size
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Vector3 SetVectorLength(Vector3 vector, float size)
        {
            //normalize the vector
            Vector3 vectorNormalized = Vector3.Normalize(vector);

            //scale the vector
            return vectorNormalized *= size;
        }

        //increase or decrease the length of vector by size
        public static Vector3 AddVectorLength(Vector3 vector, float size)
        {

            //get the vector length
            float magnitude = Vector3.Magnitude(vector);

            //calculate new vector length
            float newMagnitude = magnitude + size;

            //calculate the ratio of the new length to the old length
            float scale = newMagnitude / magnitude;

            //scale the vector
            return vector * scale;
        }

        /// <summary>
        /// get an angle in degree using 2 vector
        /// (must be normalized)
        /// </summary>
        /// <param name="from">vector 1</param>
        /// <param name="to">vector 2</param>
        /// <returns></returns>
        public static float Angle(Vector3 from, Vector3 to)
        {
            return Mathf.Acos(Mathf.Clamp(Vector3.Dot(from, to), -1f, 1f)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// get X amount of angle randomly, and evenly shared in the sphere
        /// </summary>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        public static Vector3[] EvenlyRepartitionOfPointsOnSphere(int numberOfPoints)
        {
            Vector3[] pointsPosition = new Vector3[numberOfPoints];
            float pointsAsFloat = numberOfPoints;

            float goldenAngle = Mathf.PI * (3 - Mathf.Sqrt(5));
            float step = 2f / pointsAsFloat;
            for (int i = 0; i < numberOfPoints; i++)
            {
                float y = i * step - 1 + (step / 2);
                float r = Mathf.Sqrt(1 - y * y);
                float phi = i * goldenAngle;

                pointsPosition[i] = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);
            }

            return pointsPosition;
        }

        /// <summary>
        /// prend un vecteur2 et retourne l'angle x, y en degré
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static float GetAngleFromVector2(Vector2 dir)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));       //Cross for testing -1, 0, 1
            //float signed_angle = angle * sign;                                  // angle in [-179,180]
            float angle360 = (angle + 360) % 360;                       // angle in [0,360]
            return (angle360);

            //return (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }

        /// <summary>
        /// The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
        //. If you imagine the from and to vectors as lines on a piece of paper, both originating from the same point, then the /axis/ vector would point up out of the paper.
        /// The measured angle between the two vectors would be positive in a clockwise direction and negative in an anti-clockwise direction.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="normal"></param>
        /// <returns>Signed Angle form -180 to 180</returns>
        public static float SignedAngle180(Vector3 from, Vector3 to, Vector3 axis)
        {
            float unsignedAngle = Angle(from, to);

            float cross_x = from.y * to.z - from.z * to.y;
            float cross_y = from.z * to.x - from.x * to.z;
            float cross_z = from.x * to.y - from.y * to.x;
            float sign = Mathf.Sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
            return unsignedAngle * sign;
        }


        /// <summary>
        /// renvoi l'angle entre deux vecteur, avec le 3eme vecteur de référence
        /// </summary>
        /// <param name="from">vecteur A</param>
        /// <param name="to">vecteur B</param>
        /// <param name="axis">reference</param>
        /// <returns>Retourne un angle en degré</returns>
        public static float SignedAngle360(Vector3 from, Vector3 to, Vector3 axis)
        {
            float signed_angle = SignedAngle180(from, to, axis);                                  // angle in [-179,180]
            float angle360 = (signed_angle + 360) % 360;                       // angle in [0,360]
            return (angle360);
        }

        /// <summary>
        /// Get Vector2 from angle
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2 GetVectorDirector2DFromAngle(float a, bool useRadians = false, bool yDominant = false)
        {
            if (!useRadians) a *= Mathf.Rad2Deg;
            if (yDominant)
            {
                return new Vector2(Mathf.Sin(a), Mathf.Cos(a));
            }
            else
            {
                return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            }
        }

        /*
        /// <summary>
        /// check la différence d'angle entre les 2 vecteurs
        /// </summary>
        public static float GetDiffAngleBetween2Vectors(Vector2 dir1, Vector2 dir2)
        {
            float angle1 = GetAngleFromVector2(dir1);
            float angle2 = GetAngleFromVector2(dir2);

            float diffAngle;
            IsAngleCloseToOtherByAmount(angle1, angle2, 10f, out diffAngle);
            return (diffAngle);
        }

        /// <summary>
        /// prend un angle A, B, en 360 format, et test si les 2 angles sont inférieur à différence (180, 190, 20 -> true, 180, 210, 20 -> false)
        /// </summary>
        /// <param name="angleReference">angle A</param>
        /// <param name="angleToTest">angle B</param>
        /// <param name="differenceAngle">différence d'angle accepté</param>
        /// <returns></returns>
        public static bool IsAngleCloseToOtherByAmount(float angleReference, float angleToTest, float differenceAngle, out float diff)
        {
            if (angleReference < 0 || angleReference > 360 ||
                angleToTest < 0 || angleToTest > 360)
            {
                Debug.LogError("angle non valide: " + angleReference + ", " + angleToTest);
            }

            diff = 180 - Mathf.Abs(Mathf.Abs(angleReference - angleToTest) - 180);

            //diff = Mathf.Abs(angleReference - angleToTest);        

            if (diff <= differenceAngle)
                return (true);
            return (false);
        }
        public static bool IsAngleCloseToOtherByAmount(float angleReference, float angleToTest, float differenceAngle)
        {
            if (angleReference < 0 || angleReference > 360 ||
                angleToTest < 0 || angleToTest > 360)
            {
                Debug.LogError("angle non valide: " + angleReference + ", " + angleToTest);
            }

            float diff = 180 - Mathf.Abs(Mathf.Abs(angleReference - angleToTest) - 180);

            //diff = Mathf.Abs(angleReference - angleToTest);        

            if (diff <= differenceAngle)
                return (true);
            return (false);
        }
        */

        /// <summary>
        /// from a vector in 3d space, and a reference, clamp that 3d vector to that reference
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="up"></param>
        /// <param name="maxMagnitudeInThatAxis"></param>
        /// <param name="tmpPosition"></param>
        /// <returns></returns>
        public static Vector3 ClampVectorAlongUpAxis(this Vector3 vector,
            Vector3 up, float maxMagnitudeUp)
        {
            Vector3 right = Vector3.Cross(vector, up).FastNormalized();
            Vector3 forward = -Vector3.Cross(right, up).FastNormalized();
            return (vector.ClampVectorAlongUpAxis(up, forward, maxMagnitudeUp));
            /*
            Debug.DrawRay(tmpPosition, vector, Color.white);
            Vector3 projectUp = Vector3.Project(vector, up);
            Vector3 projectUpClamped = Vector3.ClampMagnitude(projectUp, maxMagnitudeUp);
            ExtDrawGuizmos.DebugArrow(tmpPosition, projectUpClamped, Color.magenta);
            Debug.DrawRay(tmpPosition, up, Color.green);

            Vector3 right = Vector3.Cross(vector, up).FastNormalized();
            ExtDrawGuizmos.DebugArrow(tmpPosition, right, Color.red);

            Vector3 forward = -Vector3.Cross(right, up).FastNormalized();
            ExtDrawGuizmos.DebugArrow(tmpPosition, forward, Color.blue);

            Vector3 projectedForward = Vector3.Project(vector, forward);
            ExtDrawGuizmos.DebugArrow(tmpPosition, projectedForward, Color.blue);

            Vector3 final = projectedForward + projectUpClamped;
            ExtDrawGuizmos.DebugArrow(tmpPosition, final, Color.yellow);

            return (vector.ClampVectorAlongUpAxis(up, forward, maxMagnitudeUp));

            //return (vector);
            */
        }

        public static Vector3 ClampVectorAlongUpAxis(this Vector3 vector, Vector3 up, Vector3 forward, float maxMagnitudeUp)
        {
            Vector3 projectUpClamped = Vector3.ClampMagnitude(up, maxMagnitudeUp);
            Vector3 projectedForward = Vector3.Project(vector, forward);
            return (projectUpClamped + projectedForward);
        }

        #endregion

        #region calculation related to vectors
        /// <summary>
        /// is a vector considered by unity as NaN
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static bool IsNaN(this Vector3 vec)
        {
            return float.IsNaN(vec.x * vec.y * vec.z);
        }

        /// <summary>
        /// get the min lenght axis of this vector
        /// </summary>
        /// <returns>min lenght of x, y or z</returns>
        public static float Minimum(this Vector3 vector)
        {
            return Min(vector.x, vector.y, vector.z);
        }

        /// <summary>
        /// get the max lenght axis of this vector
        /// </summary>
        /// <returns>max lenght of x, y or z</returns>
        public static float Maximum(this Vector3 vector)
        {
            return Max(vector.x, vector.y, vector.z);
        }

        /// <summary>
        /// return the min of 3 value
        /// </summary>
        public static float Min(float value1, float value2, float value3)
        {
            float min = (value1 < value2) ? value1 : value2;
            return (min < value3) ? min : value3;
        }

        /// <summary>
        /// return the max of 3 value
        /// </summary>
        public static float Max(float value1, float value2, float value3)
        {
            float max = (value1 > value2) ? value1 : value2;
            return (max > value3) ? max : value3;
        }

        public unsafe static float FastInvSqrt(float x)
        {
            float xhalf = 0.5f * x;
            int i = *(int*)&x;
            i = 0x5f375a86 - (i >> 1); //this constant is slightly more accurate than the common one
            x = *(float*)&i;
            x = x * (1.5f - xhalf * x * x);
            return x;
        }

        /// <summary>
        /// Using the magic of 0x5f3759df
        /// </summary>
        /// <param name="vec1"></param>
        /// <returns></returns>
        public static Vector3 FastNormalized(this Vector3 vec1)
        {
            var componentMult = FastInvSqrt(vec1.sqrMagnitude);
            return new Vector3(vec1.x * componentMult, vec1.y * componentMult, vec1.z * componentMult);
        }

        
        public static float Distance(Vector3 a, Vector3 b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            float diff_z = a.z - b.z;
            return Mathf.Sqrt(diff_x * diff_x + diff_y * diff_y + diff_z * diff_z);
        }

        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;
            float diff_z = a.z - b.z;
            return (diff_x * diff_x + diff_y * diff_y + diff_z * diff_z);
        }

        public static float LengthSquared(this Vector3 a)
        {
            return (Vector3.Dot(a, a));
        }

        

        /// <summary>
        /// Get the closest point on a line segment.
        /// </summary>
        /// <param name="p">A point in space</param>
        /// <param name="s0">Start of line segment</param>
        /// <param name="s1">End of line segment</param>
        /// <returns>The interpolation parameter representing the point on the segment, with 0==s0, and 1==s1</returns>
        public static float ClosestPointOnSegment(this Vector3 p, Vector3 s0, Vector3 s1)
        {
            Vector3 s = s1 - s0;
            float len2 = Vector3.SqrMagnitude(s);
            if (len2 < Mathf.Epsilon)
                return 0; // degenrate segment
            return Mathf.Clamp01(Vector3.Dot(p - s0, s) / len2);
        }

        /// <summary>Is the vector within Epsilon of zero length?</summary>
        /// <param name="v"></param>
        /// <returns>True if the square magnitude of the vector is within Epsilon of zero</returns>
        public static bool AlmostZero(this Vector3 v)
        {
            return v.sqrMagnitude < (Mathf.Epsilon * Mathf.Epsilon);
        }

        /// <summary>
        /// has a target reach a position in space ?
        /// </summary>
        /// <param name="objectMoving"></param>
        /// <param name="target"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        public static bool HasReachedTargetPosition(Vector3 objectMoving, Vector3 target, float margin = 0)
        {
            float x = objectMoving.x;
            float y = objectMoving.y;
            float z = objectMoving.z;

            return (x > target.x - margin
                && x < target.x + margin
                && y > target.y - margin
                && y < target.y + margin
                && z > target.z - margin
                && z < target.z + margin);
        }


        public static Vector3 ClosestDirectionTo(Vector3 direction1, Vector3 direction2, Vector3 targetDirection)
        {
            return (Vector3.Dot(direction1, targetDirection) > Vector3.Dot(direction2, targetDirection)) ? direction1 : direction2;
        }

        /// <summary>
        /// test if a Vector3 is close to another Vector3 (due to floating point inprecision)
        /// compares the square of the distance to the square of the range as this
        /// avoids calculating a square root which is much slower than squaring the range
        /// </summary>
        /// <param name="val"></param>
        /// <param name="about"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsClose(Vector3 val, Vector3 about, float range)
        {
            return ((val - about).sqrMagnitude < range * range);
        }

        public static Vector3 Sum(params Vector3[] positions)
        {
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < positions.Length; i++)
            {
                sum += positions[i];
            }
            return (sum);
        }

        /// <summary>
        /// divide 2 vector together, the first one is the numerator, the second the denominator
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static Vector3 DivideVectors(Vector3 numerator, Vector3 denominator)
        {
            return (new Vector3(numerator.x / denominator.x, numerator.y / denominator.y, numerator.z / denominator.z));
        }

        public static int Sign(this Vector3 vector)
        {
            float sign = vector.x * vector.y * vector.z;
            if (sign < 0)
            {
                return (-1);
            }
            else if (sign > 0)
            {
                return (1);
            }
            else
            {
                return (0);
            }
        }

        public static Vector3 AddDeltaToPositionFromLocalSpace(Vector3 globalPosition, Vector3 localDelta, Vector3 up, Vector3 forward, Vector3 right)
        {
            Vector3 forwardLocal = forward * localDelta.x;
            Vector3 upLocal = up * localDelta.y;
            Vector3 rightLocal = right * localDelta.z;

            //Debug.DrawLine(globalPosition, globalPosition + forwardLocal, Color.blue);
            //Debug.DrawLine(globalPosition, globalPosition + upLocal, Color.green);
            //Debug.DrawLine(globalPosition, globalPosition + rightLocal, Color.red);



            globalPosition = globalPosition + forwardLocal + upLocal + rightLocal;

            //Vector3 orientedDelta = localDelta.z * right + localDelta.y * up + localDelta.x * forward;
            return (globalPosition);
        }

        /// <summary>
        /// is 2 vectors parallel
        /// </summary>
        /// <param name="direction">vector 1</param>
        /// <param name="otherDirection">vector 2</param>
        /// <param name="precision">test precision</param>
        /// <returns>is parallel</returns>
        public static bool IsParallel(Vector3 direction, Vector3 otherDirection, float precision = .000001f)
        {
            return Vector3.Cross(direction, otherDirection).sqrMagnitude < precision;
        }

        public static Vector3 MultiplyVector(Vector3 one, Vector3 two)
        {
            return new Vector3(one.x * two.x, one.y * two.y, one.z * two.z);
        }

        public static float MagnitudeInDirection(Vector3 vector, Vector3 direction, bool normalizeParameters = true)
        {
            if (normalizeParameters) direction.Normalize();
            return ExtVector3.DotProduct(vector, direction);
        }

        /// <summary>
        /// Absolute value of vector
        /// </summary>
        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }


        /// <summary>
        /// from a given plane (define by a normal), return the projection of a vector
        /// </summary>
        /// <param name="relativeDirection"></param>
        /// <param name="normalPlane"></param>
        /// <returns></returns>
        public static Vector3 ProjectVectorIntoPlane(Vector3 relativeDirection, Vector3 normalPlane)
        {
            //Projection of a vector on a plane and matrix of the projection.
            //http://users.telenet.be/jci/math/rmk.htm

            Vector3 Pprime = Vector3.Project(relativeDirection, normalPlane);
            Vector3 relativeProjeted = relativeDirection - Pprime;
            return (relativeProjeted);
        }


        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/Vector3.Reflect.html
        /// VectorA: input
        /// VectorB: normal
        /// Vector3: result
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetReflectAngle(Vector3 inputVector, Vector3 normalVector)
        {
            return (Vector3.Reflect(inputVector.normalized, normalVector.normalized));
        }


        public static void Reflect(ref Vector2 v, Vector2 normal)
        {
            var dp = 2f * Vector2.Dot(v, normal);
            var ix = v.x - normal.x * dp;
            var iy = v.y - normal.y * dp;
            v.x = ix;
            v.y = iy;
        }

        public static Vector2 Reflect(Vector2 v, Vector2 normal)
        {
            var dp = 2 * Vector2.Dot(v, normal);
            return new Vector2(v.x - normal.x * dp, v.y - normal.y * dp);
        }

        public static void Mirror(ref Vector2 v, Vector2 axis)
        {
            v = (2 * (Vector2.Dot(v, axis) / Vector2.Dot(axis, axis)) * axis) - v;
        }

        public static Vector2 Mirror(Vector2 v, Vector2 axis)
        {
            return (2 * (Vector2.Dot(v, axis) / Vector2.Dot(axis, axis)) * axis) - v;
        }

        /// <summary>
        /// Returns a vector orthogonal to up in the general direction of forward.
        /// </summary>
        /// <param name="up"></param>
        /// <param name="targForward"></param>
        /// <returns></returns>
        public static Vector3 GetForwardTangent(Vector3 forward, Vector3 up)
        {
            return Vector3.Cross(Vector3.Cross(up, forward), up);
        }

        /// <summary>
        /// Dot product de 2 vecteur, retourne négatif si l'angle > 90°, 0 si angle = 90, positif si moin de 90
        /// </summary>
        /// <param name="a">vecteur A</param>
        /// <param name="b">vecteur B</param>
        /// <returns>retourne négatif si l'angle > 90°</returns>
        public static float DotProduct(Vector3 a, Vector3 b)
        {
            return (a.x * b.x + a.y * b.y + a.z * b.z);
        }

        //This function calculates a signed (+ or - sign instead of being ambiguous) dot product. It is basically used
        //to figure out whether a vector is positioned to the left or right of another vector. The way this is done is
        //by calculating a vector perpendicular to one of the vectors and using that as a reference. This is because
        //the result of a dot product only has signed information when an angle is transitioning between more or less
        //than 90 degrees.
        public static float SignedDotProduct(Vector3 vectorA, Vector3 vectorB, Vector3 normal)
        {

            Vector3 perpVector;
            float dot;

            //Use the geometry object normal and one of the input vectors to calculate the perpendicular vector
            perpVector = CrossProduct(normal, vectorA);

            //Now calculate the dot product between the perpendicular vector (perpVector) and the other input vector
            dot = DotProduct(perpVector, vectorB);

            return dot;
        }

        //Calculate the dot product as an angle
        public static float DotProductAngle(Vector3 vec1, Vector3 vec2)
        {

            double dot;
            double angle;

            //get the dot product
            dot = Vector3.Dot(vec1, vec2);

            //Clamp to prevent NaN error. Shouldn't need this in the first place, but there could be a rounding error issue.
            if (dot < -1.0f)
            {
                dot = -1.0f;
            }
            if (dot > 1.0f)
            {
                dot = 1.0f;
            }

            //Calculate the angle. The output is in radians
            //This step can be skipped for optimization...
            angle = Math.Acos(dot);

            return (float)angle;
        }

        /// <summary>
        /// retourne le vecteur de droite au vecteur A, selon l'axe Z
        /// </summary>
        /// <param name="a"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 CrossProduct(Vector3 a, Vector3 z)
        {
            return new Vector3(
                a.y * z.z - a.z * z.y,
                a.z * z.x - a.x * z.z,
                a.x * z.y - a.y * z.x);
        }

        /// <summary>
        /// get mirror of a vector, according to a normal
        /// </summary>
        /// <param name="point">Vector 1</param>
        /// <param name="normal">normal</param>
        /// <returns>vector mirror to 1 (reference= normal)</returns>
        public static Vector3 ReflectionOverPlane(Vector3 point, Vector3 normal)
        {
            return point - 2 * normal * Vector3.Dot(point, normal) / Vector3.Dot(normal, normal);
        }

        /// <summary>
        /// Return the projection of A on B (with the good magnitude), based on ref (ex: Vector3.up)
        /// </summary>
        public static Vector3 GetProjectionOfAOnB(Vector3 A, Vector3 B, Vector3 refVector)
        {
            float angleDegre = SignedAngle360(A, B, refVector); //get angle A-B
            angleDegre *= Mathf.Deg2Rad;                            //convert to rad
            float magnitudeX = Mathf.Cos(angleDegre) * A.magnitude; //get magnitude
            Vector3 realDir = B.normalized * magnitudeX;            //set magnitude of new Vector
            return (realDir);   //vector A with magnitude based on B
        }

        /// <summary>
        /// Return the projection of A on B (with the good magnitude), based on ref (ex: Vector3.up)
        /// </summary>
        public static Vector3 GetProjectionOfAOnB(Vector3 A, Vector3 B, Vector3 refVector, float minMagnitude, float maxMagnitude)
        {
            float angleDegre = SignedAngle360(A, B, refVector); //get angle A-B
            angleDegre *= Mathf.Deg2Rad;                            //convert to rad
            float magnitudeX = Mathf.Cos(angleDegre) * A.magnitude; //get magnitude
                                                                    //set magnitude of new Vector
            Vector3 realDir = B.normalized * Mathf.Clamp(Mathf.Abs(magnitudeX), minMagnitude, maxMagnitude) * Mathf.Sign(magnitudeX);
            return (realDir);   //vector A with magnitude based on B
        }

        /// <summary>
        /// Gets the normal of the triangle formed by the 3 vectors
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <param name="vec3"></param>
        /// <returns></returns>
        public static Vector3 GetNormalFromTriangle(Vector3 vec1, Vector3 vec2, Vector3 vec3)
        {
            return Vector3.Cross((vec3 - vec1), (vec2 - vec1));
        }

        #endregion

        #region damping
        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="currentVelocity"></param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster., in 3 separate axis</param>
        /// <param name="maxSpeed"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static Vector3 OwnSmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            if (smoothTime == 0)
            {
                return (target);
            }

            float z = currentVelocity.z;
            float y = currentVelocity.y;
            float x = currentVelocity.x;
            Vector3 smoothedVector = new Vector3(
                OwnSmoothDamp(current.x, target.x, ref x, smoothTime, maxSpeed, deltaTime),
                OwnSmoothDamp(current.y, target.y, ref y, smoothTime, maxSpeed, deltaTime),
                OwnSmoothDamp(current.z, target.z, ref z, smoothTime, maxSpeed, deltaTime));

            currentVelocity = new Vector3(x, y, z);
            return (smoothedVector);
        }

        public static Vector2 OwnSmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, Vector2 smoothTime, Vector2 maxSpeed, float deltaTime)
        {
            float y = currentVelocity.y;
            float x = currentVelocity.x;
            Vector2 smoothedVector = new Vector2(
                OwnSmoothDamp(current.x, target.x, ref x, smoothTime.x, maxSpeed.x, deltaTime),
                OwnSmoothDamp(current.y, target.y, ref y, smoothTime.y, maxSpeed.y, deltaTime));

            currentVelocity = new Vector2(x, y);
            return (smoothedVector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="currentVelocity"></param>
        /// <param name="smoothTime">Approximately the time it will take to reach the target. A smaller value will reach the target faster.</param>
        /// <param name="maxSpeed"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public static float OwnSmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            // Based on Game Programming Gems 4 Chapter 1.10
            smoothTime = Mathf.Max(0.0001F, smoothTime);
            float omega = 2F / smoothTime;

            float x = omega * deltaTime;
            float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);
            float change = current - target;
            float originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            change = Mathf.Clamp(change, -maxChange, maxChange);
            target = current - change;

            float temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;
            float output = target + (change + temp) * exp;

            // Prevent overshooting
            if (originalTo - current > 0.0F == output > originalTo)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        /// <summary>
        /// Direct speedup of <seealso cref="Vector3.Lerp"/>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 v1, Vector3 v2, float value)
        {
            if (value > 1.0f)
                return v2;
            if (value < 0.0f)
                return v1;
            return new Vector3(v1.x + (v2.x - v1.x) * value,
                               v1.y + (v2.y - v1.y) * value,
                               v1.z + (v2.z - v1.z) * value);
        }
        public static Vector3 Sinerp(Vector3 from, Vector3 to, float value)
        {
            value = Mathf.Sin(value * Mathf.PI * 0.5f);
            return Vector3.Lerp(from, to, value);
        }
        #endregion

        #region Get middle
        /// <summary>
        /// from a given point, get the closest point found on the list
        /// </summary>
        /// <param name="current"></param>
        /// <param name="listOfPoints"></param>
        /// <param name="indexFound"></param>
        /// <returns></returns>
        public static Vector3 GetClosestPoint(Vector3 current, Vector3[] listOfPoints, ref int indexFound)
        {
            indexFound = -1;
            if (listOfPoints == null || listOfPoints.Length == 0)
            {
                return (current);
            }

            float closestDist = Vector3.SqrMagnitude(current - listOfPoints[0]);
            indexFound = 0;

            for (int i = 1; i < listOfPoints.Length; i++)
            {
                Vector3 point = listOfPoints[i];
                float dist = Vector3.SqrMagnitude(current - point);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    indexFound = i;
                }
            }
            return (listOfPoints[indexFound]);
        }

        public static Vector3 GetMiddleOf2Vector(Vector3 a, Vector3 b)
        {
            return ((a + b).normalized);
        }
        public static Vector3 GetMiddleOfXVector(ContactPoint[] arrayVect)
        {
            Vector3[] arrayTmp = new Vector3[arrayVect.Length];

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < arrayVect.Length; i++)
            {
                arrayTmp[i] = arrayVect[i].normal;
            }
            return (GetMiddleOfXVector(arrayTmp));
        }

        public static Vector3 GetMiddleOfXVector(Vector3[] arrayVect)
        {
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < arrayVect.Length; i++)
            {
                sum += arrayVect[i];
            }
            return ((sum).normalized);
        }

        public static Vector3 GetMeanOfXPoints(Transform[] arrayVect, out Vector3 sizeBoundingBox, bool middleBoundingBox = true)
        {
            return GetMeanOfXPoints(ToGameObjectsArray(arrayVect), out sizeBoundingBox, middleBoundingBox);
        }

        public static Vector3 GetMeanOfXPoints(GameObject[] arrayVect, out Vector3 sizeBoundingBox, bool middleBoundingBox = true)
        {
            Vector3[] arrayPoint = new Vector3[arrayVect.Length];
            for (int i = 0; i < arrayPoint.Length; i++)
            {
                arrayPoint[i] = arrayVect[i].transform.position;
            }
            return (GetMeanOfXPoints(arrayPoint, out sizeBoundingBox, middleBoundingBox));
        }

        /// <summary>
        /// return the middle of X points (POINTS, NOT vector)
        /// </summary>
        public static Vector3 GetMeanOfXPoints(Vector3[] arrayVect, out Vector3 sizeBoundingBox, bool middleBoundingBox = true)
        {
            sizeBoundingBox = Vector2.zero;

            if (arrayVect.Length == 0)
                return (Vector3.zero);

            if (!middleBoundingBox)
            {
                Vector3 sum = Vector3.zero;
                for (int i = 0; i < arrayVect.Length; i++)
                {
                    sum += arrayVect[i];
                }
                return (sum / arrayVect.Length);
            }
            else
            {
                if (arrayVect.Length == 1)
                    return (arrayVect[0]);

                float xMin = arrayVect[0].x;
                float yMin = arrayVect[0].y;
                float zMin = arrayVect[0].z;
                float xMax = arrayVect[0].x;
                float yMax = arrayVect[0].y;
                float zMax = arrayVect[0].z;

                for (int i = 1; i < arrayVect.Length; i++)
                {
                    if (arrayVect[i].x < xMin)
                        xMin = arrayVect[i].x;
                    if (arrayVect[i].x > xMax)
                        xMax = arrayVect[i].x;

                    if (arrayVect[i].y < yMin)
                        yMin = arrayVect[i].y;
                    if (arrayVect[i].y > yMax)
                        yMax = arrayVect[i].y;

                    if (arrayVect[i].z < zMin)
                        zMin = arrayVect[i].z;
                    if (arrayVect[i].z > zMax)
                        zMax = arrayVect[i].z;
                }
                Vector3 lastMiddle = new Vector3((xMin + xMax) / 2, (yMin + yMax) / 2, (zMin + zMax) / 2);
                sizeBoundingBox.x = Mathf.Abs(xMin - xMax);
                sizeBoundingBox.y = Mathf.Abs(yMin - yMax);
                sizeBoundingBox.z = Mathf.Abs(zMin - zMax);

                return (lastMiddle);
            }
        }

        /// <summary>
        /// get la bisection de 2 vecteur
        /// </summary>
        public static Vector3 GetbisectionOf2Vector(Vector3 a, Vector3 b)
        {
            return ((a + b) * 0.5f);
        }

        /// <summary>
        /// is a vector is in the same direction of another, with a given precision
        /// </summary>
        /// <param name="direction">vector 1</param>
        /// <param name="otherDirection">vector 2</param>
        /// <param name="precision">precision</param>
        /// <param name="normalizeParameters">Should normalise the vector first</param>
        /// <returns>is in the same direction</returns>
        public static bool IsInDirection(Vector3 direction, Vector3 otherDirection, float precision, bool normalizeParameters = true)
        {
            if (normalizeParameters)
            {
                direction.Normalize();
                otherDirection.Normalize();
            }
            return Vector3.Dot(direction, otherDirection) > 0f + precision;
        }
        public static bool IsInDirection(Vector3 direction, Vector3 otherDirection)
        {
            return Vector3.Dot(direction, otherDirection) > 0f;
        }


        public static Vector3 ClosestGeneralDirection(Vector3 vector) { return ClosestGeneralDirection(vector, GeneralDirections); }
        public static Vector3 ClosestGeneralDirection(Vector3 vector, IList<Vector3> directions)
        {
            float maxDot = float.MinValue;
            int closestDirectionIndex = 0;

            for (int i = 0; i < directions.Count; i++)
            {
                float dot = Vector3.Dot(vector, directions[i]);
                if (dot > maxDot)
                {
                    closestDirectionIndex = i;
                    maxDot = dot;
                }
            }

            return directions[closestDirectionIndex];
        }

        /// <summary>
        /// return an angle in degree between 2 vector, based on an axis
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float AngleAroundAxis(Vector3 dir1, Vector3 dir2, Vector3 axis)
        {
            dir1 = dir1 - Vector3.Project(dir1, axis);
            dir2 = dir2 - Vector3.Project(dir2, axis);

            float angle = Vector3.Angle(dir1, dir2);
            return angle * (Vector3.Dot(axis, Vector3.Cross(dir1, dir2)) < 0 ? -1 : 1);
        }

        #endregion

        #region GameObject
        /// <summary>
        /// transform a transform[] array to a gameobejct[] array
        /// </summary>
        /// <param name="arrayTransform"></param>
        /// <returns></returns>
        public static GameObject[] ToGameObjectsArray(this Transform[] arrayTransform)
        {
            if (arrayTransform == null)
            {
                return (null);
            }

            GameObject[] gameObjectArray = new GameObject[arrayTransform.Length];
            for (int i = 0; i < gameObjectArray.Length; i++)
            {
                gameObjectArray[i] = arrayTransform[i].gameObject;
            }
            return (gameObjectArray);
        }
        #endregion

        #region misc
        /// <summary>
        /// from a given sphereCastHit, if we want to have the normal of the mesh hit, we have to do another raycast
        /// </summary>
        /// <param name="castOrigin"></param>
        /// <param name="direction"></param>
        /// <param name="magnitude"></param>
        /// <param name="radius"></param>
        /// <param name="hitPoint"></param>
        /// <param name="rayCastMargin"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static Vector3 GetSurfaceNormal(Vector3 castOrigin, Vector3 direction,
            float magnitude, float radius, Vector3 hitPoint,
            float rayCastMargin, int layerMask)
        {
            Vector3 centerCollision = GetCollisionCenterSphereCast(castOrigin, direction, magnitude);
            Vector3 dirCenterToHit = hitPoint - castOrigin;
            float sizeRay = dirCenterToHit.magnitude;
            Vector3 surfaceNormal = CalculateRealNormal(centerCollision, dirCenterToHit, sizeRay, rayCastMargin, layerMask);
            return (surfaceNormal);
        }
        /// <summary>
        /// Get the center of a sphereCast calculation.
        /// </summary>
        /// <param name="origin">origin of the spherreCast</param>
        /// <param name="directionCast">direction of the sphere cast</param>
        /// <param name="hitInfoDistance">hitInfo.distance of the hitInfo</param>
        /// <returns>center position of the hit info</returns>
        public static Vector3 GetCollisionCenterSphereCast(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
        {
            return origin + (directionCast.normalized * hitInfoDistance);
        }
        private static Vector3 CalculateRealNormal(Vector3 origin, Vector3 direction, float magnitude, float rayCastMargin, int layermask)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, magnitude + rayCastMargin, layermask))
            {
                return (hit.normal);
            }
            return (Vector3.zero);
        }
        #endregion
    }
}
