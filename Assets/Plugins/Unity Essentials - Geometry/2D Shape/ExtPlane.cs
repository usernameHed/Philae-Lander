using System;
using UnityEngine;
using UnityEssentials.Geometry.extensions;
using UnityEssentials.Geometry.PropertyAttribute.ReadOnly;

namespace UnityEssentials.Geometry.shape2d
{
    /// <summary>
    /// a 3D plane
    /// </summary>
    [Serializable]
    public struct ExtPlane
    {
        [ReadOnly]        public Vector3 Point;
        [ReadOnly]        public Vector3 Normal;

        [SerializeField, Tooltip("if true, allow calculation of closestPoints / IsInsideShape of point that are below plane")]
        private bool _allowBottom;
        public bool AllowBottom { get { return (_allowBottom); } }

        public ExtPlane(Vector3 point, Vector3 normal, bool allowBottom)
        {
            Point = point;
            Normal = normal;
            _allowBottom = allowBottom;
        }

        /// <summary>
        ///  a plane defined by 3 points
        /// 
        ///   B
        ///   |\ 
        ///   | \
        ///   |  \
        ///   |   \
        ///   |    \
        ///   |     \
        ///   A------C
        /// </summary>
        /// <param name="allowBottom"></param>
        public ExtPlane(Vector3 pointA, Vector3 pointB, Vector3 pointC, bool allowBottom)
        {
            Point = pointA;
            Normal = Vector3.Cross(pointB - pointA, pointC - pointA);
            _allowBottom = allowBottom;
        }

        public void Draw(Color color)
        {
            Debug.DrawRay(Point, Normal, color);
        }

        public void MoveShape(Vector3 point, Vector3 normal)
        {
            Point = point;
            Normal = normal;
        }

        public void MoveShape(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            Point = pointA;
            Normal = Vector3.Cross(pointB - pointA, pointC - pointA);
        }

        public bool IsAbove(Vector3 K)
        {
            bool isAbove = Vector3.Dot(K - Point, Normal) > 0;
            return (isAbove);
        }

        public static bool IsAbove(Vector3 K, Vector3 PointPlane, Vector3 NormalPlane)
        {
            bool isAbove = Vector3.Dot(K - PointPlane, NormalPlane) > 0;
            return (isAbove);
        }

        public static Vector3 ProjectPointInPlane(ExtPlane plane, Vector3 pointToProject)
        {
            return (pointToProject - (Vector3.Project(pointToProject - plane.Point, plane.Normal.normalized)));
        }

        //This function returns a point which is a projection from a point to a plane.
        public static Vector3 ProjectPointInPlaneMethod2(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {

            float distance;
            Vector3 translationVector;

            //First calculate the distance from the point to the plane:
            distance = SignedDistancePlanePoint(planeNormal, planePoint, point);

            //Reverse the sign of the distance
            distance *= -1;

            //Get a translation vector
            translationVector = ExtVector3.SetVectorLength(planeNormal, distance);

            //Translate the point to form a projection
            return point + translationVector;
        }

        //Get the shortest distance between a point and a plane. The output is signed so it holds information
        //as to which side of the plane normal the point is.
        public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            return Vector3.Dot(planeNormal, (point - planePoint));
        }

        //Projects a vector onto a plane. The output is not normalized.
        public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
        {
            return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
        }

        public static bool PlanePlaneIntersection(ExtPlane plane1, ExtPlane plane2, out Vector3 linePoint, out Vector3 lineVec)
        {
            return (PlanePlaneIntersection(plane1.Point, plane1.Normal, plane2.Point, plane2.Normal, out linePoint, out lineVec));
        }

        //Find the line of intersection between two planes.
        //The inputs are two game objects which represent the planes.
        //The outputs are a point on the line and a vector which indicates it's direction.
        private static bool PlanePlaneIntersection(Vector3 plane1Point, Vector3 plane1Normal, Vector3 planePoint2, Vector3 plane2Normal, out Vector3 linePoint, out Vector3 lineVec)
        {
            linePoint = Vector3.zero;
            lineVec = Vector3.zero;

            //We can get the direction of the line of intersection of the two planes by calculating the
            //cross product of the normals of the two planes. Note that this is just a direction and the line
            //is not fixed in space yet.
            lineVec = Vector3.Cross(plane1Normal, plane2Normal);

            //Next is to calculate a point on the line to fix it's position. This is done by finding a vector from
            //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
            //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
            //the cross product of the normal of plane2 and the lineDirection.      
            Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

            float numerator = Vector3.Dot(plane1Normal, ldir);

            //Prevent divide by zero.
            if (Mathf.Abs(numerator) > 0.000001f)
            {
                Vector3 plane1ToPlane2 = plane1Point - planePoint2;
                float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / numerator;
                linePoint = planePoint2 + t * ldir;
                return (true);
            }
            else
            {
                return (false);
            }
        }

        /// <summary>
        /// get intersection between line and plane
        /// </summary>
        /// <param name="rayVector"></param>
        /// <param name="rayPoint"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Vector3 PlaneLineIntersection(Ray ray, ExtPlane plane)
        {
            var diff = ray.origin - plane.Point;
            var prod1 = Vector3.Dot(diff, plane.Normal);
            var prod2 = Vector3.Dot(ray.direction, plane.Normal);
            var prod3 = prod1 / prod2;
            return ray.origin - ray.direction * prod3;
        }

        /// <summary>
        /// Get the intersection between a line and a plane. 
        //  If the line and plane are not parallel, the function outputs true, otherwise false.
        /// </summary>
        /// <param name="intersection"></param>
        /// <param name="ray"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static bool LinePlaneIntersection(out Vector3 intersection, Ray ray, ExtPlane plane)
        {

            float length;
            float dotNumerator;
            float dotDenominator;
            Vector3 vector;
            intersection = Vector3.zero;

            //calculate the distance between the linePoint and the line-plane intersection point
            dotNumerator = Vector3.Dot((plane.Point - ray.origin), plane.Normal);
            dotDenominator = Vector3.Dot(ray.direction, plane.Normal);

            //line and plane are not parallel
            if (dotDenominator != 0.0f)
            {
                length = dotNumerator / dotDenominator;

                //create a vector from the linePoint to the intersection point
                vector = ExtVector3.SetVectorLength(ray.direction, length);

                //get the coordinates of the line-plane intersection point
                intersection = ray.origin + vector;

                return true;
            }

            //output not valid
            else
            {
                return false;
            }
        }

        //Calculate the angle between a vector and a plane. The plane is made by a normal vector.
        //Output is in radians.
        public static float AngleVectorPlane(Vector3 vector, Vector3 normal)
        {

            float dot;
            float angle;

            //calculate the the dot product between the two input vectors. This gives the cosine between the two vectors
            dot = Vector3.Dot(vector, normal);

            //this is in radians
            angle = (float)Mathf.Acos(dot);

            return 1.570796326794897f - angle; //90 degrees - angle
        }

        //Convert a plane defined by 3 points to a plane defined by a vector and a point. 
        //The plane point is the middle of the triangle defined by the 3 points.
        public static ExtPlane PlaneFrom3Points(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            Vector3 planeNormal = Vector3.zero;
            Vector3 planePoint = Vector3.zero;

            //Make two vectors from the 3 input points, originating from point A
            Vector3 AB = pointB - pointA;
            Vector3 AC = pointC - pointA;

            //Calculate the normal
            planeNormal = Vector3.Normalize(Vector3.Cross(AB, AC));

            //Get the points in the middle AB and AC
            Vector3 middleAB = pointA + (AB / 2.0f);
            Vector3 middleAC = pointA + (AC / 2.0f);

            //Get vectors from the middle of AB and AC to the point which is not on that line.
            Vector3 middleABtoC = pointC - middleAB;
            Vector3 middleACtoB = pointB - middleAC;

            //Calculate the intersection between the two lines. This will be the center 
            //of the triangle defined by the 3 points.
            //We could use LineLineIntersection instead of ClosestPointsOnTwoLines but due to rounding errors 
            //this sometimes doesn't work.
            Vector3 temp;
            ExtLine.ClosestPointsOnTwoLines(out planePoint, out temp, middleAB, middleABtoC, middleAC, middleACtoB);

            return (new ExtPlane(planePoint, planeNormal, true));
        }
        //end class
    }
    //end nameSpace
}