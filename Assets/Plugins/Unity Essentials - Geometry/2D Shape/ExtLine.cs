using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.GravityOverride;
using UnityEssentials.Geometry.PropertyAttribute.OnvalueChanged;

namespace UnityEssentials.Geometry.shape2d
{
    /// <summary>
    /// 
    ///     1
    ///     |
    ///     |
    ///     |
    ///     |
    ///     2
    /// 
    /// a 3D line, with 2 points
    /// </summary>
    [Serializable]
    public struct ExtLine
    {
        [SerializeField, OnValueChanged("ShapeMoved")]
        private Vector3 _p1;
        public Vector3 P1 { get { return (_p1); } }
        [SerializeField, OnValueChanged("ShapeMoved")]
        private Vector3 _p2;
        public Vector3 P2 { get { return (_p2); } }

        [SerializeField]
        private Vector3 _delta;
        [SerializeField]
        private float _deltaSquared;

        public ExtLine(Vector3 p1, Vector3 p2) : this()
        {
            MoveShape(p1, p2);
        }

        public void MoveShape(Vector3 p1, Vector3 p2)
        {
            _p1 = p1;
            _p2 = p2;
            ShapeMoved();
        }

        public void ShapeMoved()
        {
            _delta = _p2 - _p1;
            _deltaSquared = Vector3.Dot(_delta, _delta);
        }

        public Vector3 PointAt(float t)
        {
            return (_p1 + t * _delta);
        }

        public double GetLenght()
        {
            return (_delta.magnitude);
        }

        public float Project(Vector3 p)
        {
            return (Vector3.Dot(_delta, p - _p1) / _deltaSquared);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="nullIfOutside"></param>
        /// <returns></returns>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            float dist = Vector3.Dot(k - _p1, _delta);

            //k projection is outside the [_p1, _p2] interval, closest to _p1
            if (dist <= 0.0f)
            {
                return (_p1);
            }
            //k projection is outside the [_p1, p2] interval, closest to _p2
            else if (dist >= _deltaSquared)
            {
                return (_p2);
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                dist = dist / _deltaSquared;
                Vector3 pointOnLine = _p1 + dist * _delta;
                return (pointOnLine);
            }
        }

        public bool GetClosestPointIfWeCan(Vector3 k, out Vector3 closestPoint, GravityOverrideLineTopDown gravityOverride)
        {
            closestPoint = Vector3.zero;
            if (!gravityOverride.CanApplyGravity)
            {
                return (false);
            }
            float dist = Vector3.Dot(k - _p1, _delta);

            //k projection is outside the [_p1, _p2] interval, closest to _p1
            if (dist <= 0.0f)
            {
                closestPoint = _p1;
                return (gravityOverride.Top);
            }
            //k projection is outside the [_p1, p2] interval, closest to _p2
            else if (dist >= _deltaSquared)
            {
                closestPoint = _p2;
                return (gravityOverride.Bottom);
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                if (!gravityOverride.Trunk)
                {
                    return (false);
                }
                dist = dist / _deltaSquared;
                closestPoint = _p1 + dist * _delta;
                return (true);
            }
        }

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            Debug.DrawLine(_p1, _p2, color);
        }

        public void DrawWithOffset(Color color, Vector3 offset)
        {
            Debug.DrawLine(_p1 + offset, _p2 + offset, color);
        }
#endif

        public static Vector3 GetClosestPointFromLines(Vector3 k, ExtLine[] lines, out int indexLine)
        {
            Vector3 closestFound = lines[0].GetClosestPoint(k);
            float sqrDist = (k - closestFound).sqrMagnitude;
            indexLine = 0;

            for (int i = 1; i < lines.Length; i++)
            {
                Vector3 closest = lines[i].GetClosestPoint(k);
                float dist = (k - closest).sqrMagnitude;
                if (dist < sqrDist)
                {
                    sqrDist = dist;
                    closestFound = closest;
                    indexLine = i;
                }
            }
            return (closestFound);
        }



        /// <summary>
        /// return the point at a given percentage of a line
        /// </summary>
        /// <param name="percentage">0 to 1</param>
        /// <returns></returns>
        public static Vector3 GetPositionInLineFromPercentage(ExtLine line, float percentage)
        {
            return (GetPositionInLineFromPercentage(line._p1, line._p2, percentage));
        }

        private static Vector3 GetPositionInLineFromPercentage(Vector3 a, Vector3 b, float percentage)
        {
            return (Vector3.Lerp(a, b, percentage));
        }

        /// <summary>
        /// return percentage (from 0 to 1) of the position of the C vector
        /// </summary>
        public static float GetPercentageInLineFromPosition(ExtLine line, Vector3 c)
        {
            return (GetPercentageInLineFromPosition(line._p1, line._p2, c));
        }

        private static float GetPercentageInLineFromPosition(Vector3 a, Vector3 b, Vector3 c)
        {
            var ab = b - a;
            var ac = c - a;
            return Vector3.Dot(ac, ab) / ab.sqrMagnitude;
        }

        public static Vector2 CalculateLineLineIntersection2d(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
        {
            float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

            if (tmp == 0)
            {
                // No solution!
                found = false;
                return Vector2.zero;
            }

            float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

            found = true;

            return new Vector2(
                B1.x + (B2.x - B1.x) * mu,
                B1.y + (B2.y - B1.y) * mu
            );
        }

        /// <summary>
        /// Calculates the intersection line segment between 2 lines (not segments).
        /// Returns false if no solution can be found.
        /// </summary>
        /// <returns></returns>
        public static bool CalculateLineLineIntersection3d(ExtLine line1, ExtLine line2, out Vector3 resulmtSegmentPoint1, out Vector3 resultSegmentPoint2)
        {
            return (CalculateLineLineIntersection3d(line1._p1, line1._p2, line2._p1, line2._p2, out resulmtSegmentPoint1, out resultSegmentPoint2));
        }

        private static bool CalculateLineLineIntersection3d(Vector3 line1Point1, Vector3 line1Point2,
            Vector3 line2Point1, Vector3 line2Point2, out Vector3 resultSegmentPoint1, out Vector3 resultSegmentPoint2)
        {
            // Algorithm is ported from the C algorithm of 
            // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
            resultSegmentPoint1 = new Vector3(0, 0, 0);
            resultSegmentPoint2 = new Vector3(0, 0, 0);

            var p1 = line1Point1;
            var p2 = line1Point2;
            var p3 = line2Point1;
            var p4 = line2Point2;
            var p13 = p1 - p3;
            var p43 = p4 - p3;

            if (p4.sqrMagnitude < float.Epsilon)
            {
                return false;
            }
            var p21 = p2 - p1;
            if (p21.sqrMagnitude < float.Epsilon)
            {
                return false;
            }

            var d1343 = p13.x * p43.x + p13.y * p43.y + p13.z * p43.z;
            var d4321 = p43.x * p21.x + p43.y * p21.y + p43.z * p21.z;
            var d1321 = p13.x * p21.x + p13.y * p21.y + p13.z * p21.z;
            var d4343 = p43.x * p43.x + p43.y * p43.y + p43.z * p43.z;
            var d2121 = p21.x * p21.x + p21.y * p21.y + p21.z * p21.z;

            var denom = d2121 * d4343 - d4321 * d4321;
            if (Mathf.Abs(denom) < float.Epsilon)
            {
                return false;
            }
            var numer = d1343 * d4321 - d1321 * d4343;

            var mua = numer / denom;
            var mub = (d1343 + d4321 * (mua)) / d4343;

            resultSegmentPoint1.x = p1.x + mua * p21.x;
            resultSegmentPoint1.y = p1.y + mua * p21.y;
            resultSegmentPoint1.z = p1.z + mua * p21.z;
            resultSegmentPoint2.x = p3.x + mub * p43.x;
            resultSegmentPoint2.y = p3.y + mub * p43.y;
            resultSegmentPoint2.z = p3.z + mub * p43.z;

            return true;
        }

        //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
        //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
        //same plane, use ClosestPointsOnTwoLines() instead.
        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parrallel
            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        //to each other. This function finds those two points. If the lines are not parallel, the function 
        //outputs true, otherwise false.
        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            //lines are not parallel
            if (d != 0.0f)
            {

                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;

                return true;
            }

            else
            {
                return false;
            }
        }

        //This function returns a point which is a projection from a point to a line.
        //The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
        public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
        {
            //get vector from point on line to point in space
            Vector3 linePointToPoint = point - linePoint;

            float t = Vector3.Dot(linePointToPoint, lineVec);

            return linePoint + lineVec * t;
        }

        //This function returns a point which is a projection from a point to a line segment.
        //If the projected point lies outside of the line segment, the projected point will 
        //be clamped to the appropriate line edge.
        //If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
        public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
        {

            Vector3 vector = linePoint2 - linePoint1;

            Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);

            int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

            //The projected point is on the line segment
            if (side == 0)
            {

                return projectedPoint;
            }

            if (side == 1)
            {

                return linePoint1;
            }

            if (side == 2)
            {

                return linePoint2;
            }

            //output is invalid
            return Vector3.zero;
        }

        //This function finds out on which side of a line segment the point is located.
        //The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
        //the line segment, project it on the line using ProjectPointOnLine() first.
        //Returns 0 if point is on the line segment.
        //Returns 1 if point is outside of the line segment and located on the side of linePoint1.
        //Returns 2 if point is outside of the line segment and located on the side of linePoint2.
        public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
        {

            Vector3 lineVec = linePoint2 - linePoint1;
            Vector3 pointVec = point - linePoint1;

            float dot = Vector3.Dot(pointVec, lineVec);

            //point is on side of linePoint2, compared to linePoint1
            if (dot > 0)
            {

                //point is on the line segment
                if (pointVec.magnitude <= lineVec.magnitude)
                {

                    return 0;
                }

                //point is not on the line segment and it is on the side of linePoint2
                else
                {

                    return 2;
                }
            }

            //Point is not on side of linePoint2, compared to linePoint1.
            //Point is not on the line segment and it is on the side of linePoint1.
            else
            {

                return 1;
            }
        }

        //Returns true if line segment made up of pointA1 and pointA2 is crossing line segment made up of
        //pointB1 and pointB2. The two lines are assumed to be in the same plane.
        public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
        {

            Vector3 closestPointA;
            Vector3 closestPointB;
            int sideA;
            int sideB;

            Vector3 lineVecA = pointA2 - pointA1;
            Vector3 lineVecB = pointB2 - pointB1;

            bool valid = ClosestPointsOnTwoLines(out closestPointA, out closestPointB, pointA1, lineVecA.normalized, pointB1, lineVecB.normalized);

            //lines are not parallel
            if (valid)
            {

                sideA = PointOnWhichSideOfLineSegment(pointA1, pointA2, closestPointA);
                sideB = PointOnWhichSideOfLineSegment(pointB1, pointB2, closestPointB);

                if ((sideA == 0) && (sideB == 0))
                {

                    return true;
                }

                else
                {

                    return false;
                }
            }

            //lines are parallel
            else
            {

                return false;
            }
        }

        /// <summary>
        /// from a set of points, return the closest one to the ray
        /// </summary>
        /// <param name="index"></param>
        /// <param name="ray"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static int GetClosestPointToRay(Ray ray, out float minDist, params Vector3[] points)
        {
            minDist = 0;
            if (points.Length == 0)
            {
                return (-1);
            }
            Vector3 p1 = ray.origin;
            Vector3 p2 = ray.GetPoint(1);
            minDist = DistancePointToLine3D(points[0], p1, p2);
            int index = 0;
            for (int i = 1; i < points.Length; i++)
            {
                float dist = DistancePointToLine3D(points[i], p1, p2);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = i;
                }
            }
            return (index);
        }

        private static float DistancePointToLine3D(Vector3 point, Vector3 lineP1, Vector3 lineP2)
        {
            return (Vector3.Cross(lineP1 - lineP2, point - lineP1).magnitude);
        }
    }

}
