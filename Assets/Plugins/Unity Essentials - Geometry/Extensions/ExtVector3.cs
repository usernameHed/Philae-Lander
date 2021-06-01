using UnityEngine;

namespace UnityEssentials.Geometry.extensions
{
    /// <summary>
    /// Vector Extension for spline
    /// </summary>
    public static class ExtVector3
    {
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

        /// <summary>
        /// create a vector of direction "vector" with length "size"
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

        /// <summary>
        /// return the middle of X points (POINTS, NOT vector)
        /// </summary>
        public static Vector3 GetMeanOfXPoints(Vector3[] arrayVect, out Vector3 sizeBoundingBox, bool middleBoundingBox = true)
        {
            return (GetMeanOfXPoints(out sizeBoundingBox, middleBoundingBox, arrayVect));
        }
        public static Vector3 GetMeanOfXPoints(Vector3[] arrayVect, bool middleBoundingBox = true)
        {
            return (GetMeanOfXPoints(arrayVect, out Vector3 sizeBOundingBox, middleBoundingBox));
        }

        public static Vector3 GetMeanOfXPoints(params Vector3[] points)
        {
            return (GetMeanOfXPoints(out Vector3 sizeBoundingBox, false, points));
        }

        public static Vector3 GetMeanOfXPoints(out Vector3 sizeBoundingBox, bool middleBoundingBox = true, params Vector3[] points)
        {
            sizeBoundingBox = Vector2.zero;

            if (points.Length == 0)
            {
                return (Vector3.zero);
            }

            if (!middleBoundingBox)
            {
                Vector3 sum = Vector3.zero;
                for (int i = 0; i < points.Length; i++)
                {
                    sum += points[i];
                }
                return (sum / points.Length);
            }
            else
            {
                if (points.Length == 1)
                    return (points[0]);

                float xMin = points[0].x;
                float yMin = points[0].y;
                float zMin = points[0].z;
                float xMax = points[0].x;
                float yMax = points[0].y;
                float zMax = points[0].z;

                for (int i = 1; i < points.Length; i++)
                {
                    if (points[i].x < xMin)
                        xMin = points[i].x;
                    if (points[i].x > xMax)
                        xMax = points[i].x;

                    if (points[i].y < yMin)
                        yMin = points[i].y;
                    if (points[i].y > yMax)
                        yMax = points[i].y;

                    if (points[i].z < zMin)
                        zMin = points[i].z;
                    if (points[i].z > zMax)
                        zMax = points[i].z;
                }
                Vector3 lastMiddle = new Vector3((xMin + xMax) / 2, (yMin + yMax) / 2, (zMin + zMax) / 2);
                sizeBoundingBox.x = Mathf.Abs(xMin - xMax);
                sizeBoundingBox.y = Mathf.Abs(yMin - yMax);
                sizeBoundingBox.z = Mathf.Abs(zMin - zMax);

                return (lastMiddle);
            }
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

        /// <summary>
        /// get the max lenght of this vector
        /// </summary>
        /// <returns>min lenght of x, y or z</returns>
        public static float Maximum(this Vector3 vector)
        {
            return Max(vector.x, vector.y, vector.z);
        }

        /// <summary>
        /// return the max of 3 value
        /// </summary>
        public static float Max(float value1, float value2, float value3)
        {
            float max = (value1 > value2) ? value1 : value2;
            return (max > value3) ? max : value3;
        }

        public static float LengthSquared(this Vector3 a)
        {
            return (Vector3.Dot(a, a));
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
    }
}
