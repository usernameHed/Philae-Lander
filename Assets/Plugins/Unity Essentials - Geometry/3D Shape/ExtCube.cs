using UnityEssentials.Geometry.shape2d;
using System;
using UnityEngine;
using UnityEssentials.Geometry.extensions;
using UnityEssentials.Geometry.PropertyAttribute.ReadOnly;
using UnityEssentials.Geometry.GravityOverride;

namespace UnityEssentials.Geometry.shape3d
{
    [Serializable]
    public struct ExtCube
    {
        [SerializeField, ReadOnly] private Vector3 _position;       public Vector3 Position { get { return (_position); } }
        [SerializeField, ReadOnly] private Quaternion _rotation;    public Quaternion Rotation { get { return (_rotation); } }
        [SerializeField, ReadOnly] private Vector3 _localScale;     public Vector3 LocalScale { get { return (_localScale); } }
        [SerializeField, ReadOnly] private Matrix4x4 _cubeMatrix;

        [SerializeField, ReadOnly] private Vector3 _p1;   public Vector3 P1 { get { return (_p1); } }
        [SerializeField, ReadOnly] private Vector3 _p2;   public Vector3 P2 { get { return (_p2); } }
        [SerializeField, ReadOnly] private Vector3 _p3;   public Vector3 P3 { get { return (_p3); } }
        [SerializeField, ReadOnly] private Vector3 _p4;   public Vector3 P4 { get { return (_p4); } }
        [SerializeField, ReadOnly] private Vector3 _p5;   public Vector3 P5 { get { return (_p5); } }
        [SerializeField, ReadOnly] private Vector3 _p6;   public Vector3 P6 { get { return (_p6); } }
        [SerializeField, ReadOnly] private Vector3 _p7;   public Vector3 P7 { get { return (_p7); } }
        [SerializeField, ReadOnly] private Vector3 _p8;   public Vector3 P8 { get { return (_p8); } }

        [SerializeField, ReadOnly] private Vector3 _v41;
        [SerializeField, ReadOnly] private Vector3 _v51;
        [SerializeField, ReadOnly] private Vector3 _v21;

        [SerializeField, ReadOnly] private float _v41Squared;
        [SerializeField, ReadOnly] private float _v51Squared;
        [SerializeField, ReadOnly] private float _v21Squared;

        [SerializeField, ReadOnly] private float _uP1;
        [SerializeField, ReadOnly] private float _uP2;
        [SerializeField, ReadOnly] private float _vP1;
        [SerializeField, ReadOnly] private float _vP4;
        [SerializeField, ReadOnly] private float _wP1;
        [SerializeField, ReadOnly] private float _wP5;

        [SerializeField, ReadOnly] private ExtPlane _face1;
        [SerializeField, ReadOnly] private ExtPlane _face2;
        [SerializeField, ReadOnly] private ExtPlane _face3;
        [SerializeField, ReadOnly] private ExtPlane _face4;
        [SerializeField, ReadOnly] private ExtPlane _face5;
        [SerializeField, ReadOnly] private ExtPlane _face6;


        public ExtCube(Vector3 position, Quaternion rotation, Vector3 localScale) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;

            UpdateMatrix();
        }

        ///
        ///      6 ------------ 7
        ///    / |            / |
        ///  5 ------------ 8   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |  2 ----------|-- 3
        ///  |/             | /
        ///  1 ------------ 4
        private void UpdateMatrix()
        {
            _cubeMatrix = ExtMatrix.GetMatrixTRS(_position, _rotation, _localScale);

            Vector3 size = Vector3.one;

            _p1 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((-size) * 0.5f));
            _p2 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, -size.y, size.z) * 0.5f));
            _p3 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            _p4 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            _p5 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            _p6 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, size.z) * 0.5f));
            _p7 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((size) * 0.5f));
            _p8 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, size.y, -size.z) * 0.5f));

            _v41 = (_p4 - _p1);
            _v21 = (_p2 - _p1);
            _v51 = (_p5 - _p1);

            _v41Squared = _v41.LengthSquared();
            _v21Squared = _v21.LengthSquared();
            _v51Squared = _v51.LengthSquared();

            _uP1 = Vector3.Dot(-_v21, _p1);
            _uP2 = Vector3.Dot(-_v21, _p2);
            _vP1 = Vector3.Dot(-_v41, _p1);
            _vP4 = Vector3.Dot(-_v41, _p4);
            _wP1 = Vector3.Dot(-_v51, _p1);
            _wP5 = Vector3.Dot(-_v51, _p5);

            _face1.MoveShape(_p1, _p5, _p4);
            _face2.MoveShape(_p4, _p8, _p3);
            _face3.MoveShape(_p5, _p6, _p8);
            _face4.MoveShape(_p2, _p1, _p3);
            _face5.MoveShape(_p2, _p6, _p1);
            _face6.MoveShape(_p3, _p7, _p2);
        }

        public void Draw(Color color, bool drawFace, bool drawPoints)
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DrawLocalCube(_p1, _p2, _p3, _p4, _p5, _p6, _p7, _p8, color, drawFace, drawPoints);
#endif
        }

        public void DrawWithExtraSize(Color color, Vector3 extraSize)
        {
#if UNITY_EDITOR
            Matrix4x4 cubeMatrix = ExtMatrix.GetMatrixTRS(_position, _rotation, _localScale + extraSize);

            Vector3 size = Vector3.one;

            Vector3 p1 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((-size) * 0.5f));
            Vector3 p2 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, -size.y, size.z) * 0.5f));
            Vector3 p3 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 p4 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            Vector3 p5 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 p6 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(-size.x, size.y, size.z) * 0.5f));
            Vector3 p7 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((size) * 0.5f));
            Vector3 p8 = cubeMatrix.MultiplyPoint3x4(Vector3.zero + (new Vector3(size.x, size.y, -size.z) * 0.5f));
            ExtDrawGuizmos.DrawLocalCube(p1, p2, p3, p4, p5, p6, p7, p8, color);
#endif
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            UpdateMatrix();
        }


        /// <summary>
        /// return true if the position is inside the sphape
        /// 
        ///      6 ------------ 7
        ///    / |            / |
        ///  5 ------------ 8   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |  2 ----------|-- 3
        ///  |/             | /
        ///  1 ------------ 4
        ///  
        /// perpendiculare:         not perpendiculare:
        /// u = 1 - 2               u = (1 - 4) × (1 - 5)
        /// v = 1 - 4               v = (1 - 2) × (1 - 5)
        /// w = 1 - 5               w = (1 - 2) × (1 - 4)
        /// 
        /// </summary>
        public bool IsInsideShape(Vector3 k)
        {
#if UNITY_EDITOR
            if (_p1 == _p2 && _p1 == _p4 && _p1 == _p5)
            {
                return (false);
            }
#endif

            float ux = Vector3.Dot(-_v21, k);
            float vx = Vector3.Dot(-_v41, k);
            float wx = Vector3.Dot(-_v51, k);

            bool isUBetween = ux.IsBetween(_uP2, _uP1);
            bool isVBetween = vx.IsBetween(_vP4, _vP1);
            bool isWBetween = wx.IsBetween(_wP5, _wP1);

            bool isInside = isUBetween && isVBetween && isWBetween;
            return (isInside);
        }

        /// <summary>
        /// Return the closest point on the surface of the cube, from a given point x
        ///
        ///      6 ------------ 7
        ///    / |            / |
        ///  5 ------------ 8   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |   |          |   |
        ///  |  2 ----------|-- 3
        ///  |/             | /
        ///  1 ------------ 4
        ///  
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            Vector3 vK1 = k - _p1;
            float tx = Vector3.Dot(vK1, _v41) / _v41Squared;
            float ty = Vector3.Dot(vK1, _v51) / _v51Squared;
            float tz = Vector3.Dot(vK1, _v21) / _v21Squared;

            tx = ExtMathf.SetBetween(tx, 0, 1);
            ty = ExtMathf.SetBetween(ty, 0, 1);
            tz = ExtMathf.SetBetween(tz, 0, 1);

            Vector3 closestPoint = tx * _v41
                                    + ty * _v51
                                    + tz * _v21
                                    + _p1;

            return (closestPoint);
        }

        ///
        ///      6 ------------ 7
        ///    / |    3       / |
        ///  5 ------------ 8   |       
        ///  |   |          |   |      
        ///  | 5 |     6    | 2 |     ------8-----  
        ///  |   |   1      |   |                   
        ///  |  2 ----------|-- 3                   
        ///  |/       4     | /     |       3      | 
        ///  1 ------------ 4                       
        ///                                         
        ///          6 ------6----- 5 ------2----- 8 -----10----- 7       -       
        ///          |              |              |              |               
        ///          |              |              |              |               
        ///          5      5       1       1      3       2      11       6       |
        ///          |              |              |              |               
        ///          |              |              |              |               
        ///          2 ------7----- 1 ------4----- 4 ------12---- 3       -
        ///                                         
        ///                                         
        ///                         |       4      |  
        ///                                         
        ///                                         
        ///                           ------9-----       
        public bool GetClosestPointIfWeCan(Vector3 K, GravityOverrideCube cube, out Vector3 closestPoint)
        {
            closestPoint = Vector3.zero;

            bool canApply = CanApplyFaces(K, cube);
            if (!canApply)
            {
                return (false);
            }

            closestPoint = GetClosestPoint(K);
            return (true);
        }

        private bool CanApplyFaces(Vector3 K, GravityOverrideCube cube)
        {
            bool isAboveFace1 = _face1.IsAbove(K);
            bool isAboveFace2 = _face2.IsAbove(K);
            bool isAboveFace3 = _face3.IsAbove(K);
            bool isAboveFace4 = _face4.IsAbove(K);
            bool isAboveFace5 = _face5.IsAbove(K);
            bool isAboveFace6 = _face6.IsAbove(K);
            if (!cube.Face1 && isAboveFace1 && !isAboveFace5 && !isAboveFace3 && !isAboveFace4 && !isAboveFace2) { return (false); }
            if (!cube.Face2 && isAboveFace2 && !isAboveFace1 && !isAboveFace3 && !isAboveFace6 && !isAboveFace4) { return (false); }
            if (!cube.Face3 && isAboveFace3 && !isAboveFace5 && !isAboveFace6 && !isAboveFace2 && !isAboveFace1) { return (false); }
            if (!cube.Face4 && isAboveFace4 && !isAboveFace1 && !isAboveFace5 && !isAboveFace6 && !isAboveFace2) { return (false); }
            if (!cube.Face5 && isAboveFace5 && !isAboveFace1 && !isAboveFace3 && !isAboveFace6 && !isAboveFace4) { return (false); }
            if (!cube.Face6 && isAboveFace6 && !isAboveFace5 && !isAboveFace3 && !isAboveFace2 && !isAboveFace4) { return (false); }
            if (!cube.Line1 && isAboveFace1 && isAboveFace5 && !isAboveFace3 && !isAboveFace4) { return (false); }
            if (!cube.Line2 && isAboveFace1 && isAboveFace3 && !isAboveFace5 && !isAboveFace2) { return (false); }
            if (!cube.Line3 && isAboveFace1 && isAboveFace2 && !isAboveFace3 && !isAboveFace4) { return (false); }
            if (!cube.Line4 && isAboveFace1 && isAboveFace4 && !isAboveFace5 && !isAboveFace2) { return (false); }
            if (!cube.Line5 && isAboveFace5 && isAboveFace6 && !isAboveFace3 && !isAboveFace4) { return (false); }
            if (!cube.Line6 && isAboveFace5 && isAboveFace3 && !isAboveFace6 && !isAboveFace1) { return (false); }
            if (!cube.Line7 && isAboveFace5 && isAboveFace4 && !isAboveFace6 && !isAboveFace1) { return (false); }
            if (!cube.Line8 && isAboveFace6 && isAboveFace3 && !isAboveFace2 && !isAboveFace5) { return (false); }
            if (!cube.Line9 && isAboveFace6 && isAboveFace4 && !isAboveFace2 && !isAboveFace5) { return (false); }
            if (!cube.Line10 && isAboveFace3 && isAboveFace2 && !isAboveFace1 && !isAboveFace6) { return (false); }
            if (!cube.Line11 && isAboveFace2 && isAboveFace6 && !isAboveFace3 && !isAboveFace4) { return (false); }
            if (!cube.Line12 && isAboveFace2 && isAboveFace4 && !isAboveFace1 && !isAboveFace6) { return (false); }
            if (!cube.Point1 && isAboveFace1 && isAboveFace5 && isAboveFace4) { return (false); }
            if (!cube.Point2 && isAboveFace5 && isAboveFace6 && isAboveFace4) { return (false); }
            if (!cube.Point3 && isAboveFace6 && isAboveFace2 && isAboveFace4) { return (false); }
            if (!cube.Point4 && isAboveFace1 && isAboveFace2 && isAboveFace4) { return (false); }
            if (!cube.Point5 && isAboveFace1 && isAboveFace3 && isAboveFace5) { return (false); }
            if (!cube.Point6 && isAboveFace3 && isAboveFace5 && isAboveFace6) { return (false); }
            if (!cube.Point7 && isAboveFace3 && isAboveFace6 && isAboveFace2) { return (false); }
            if (!cube.Point8 && isAboveFace1 && isAboveFace2 && isAboveFace3) { return (false); }
            return (true);
        }

        //Returns true if a line segment (made up of linePoint1 and linePoint2) is fully or partially in a rectangle
        //made up of RectA to RectD. The line segment is assumed to be on the same plane as the rectangle. If the line is 
        //not on the plane, use ProjectPointOnPlane() on linePoint1 and linePoint2 first.
        public static bool IsLineInRectangle(Vector3 linePoint1, Vector3 linePoint2, Vector3 rectA, Vector3 rectB, Vector3 rectC, Vector3 rectD)
        {
            bool pointAInside = false;
            bool pointBInside = false;

            pointAInside = IsPointIn2DRectangle(linePoint1, rectA, rectC, rectB, rectD);

            if (!pointAInside)
            {
                pointBInside = IsPointIn2DRectangle(linePoint2, rectA, rectC, rectB, rectD);
            }

            //none of the points are inside, so check if a line is crossing
            if (!pointAInside && !pointBInside)
            {
                bool lineACrossing = ExtLine.AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);
                bool lineBCrossing = ExtLine.AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);
                bool lineCCrossing = ExtLine.AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);
                bool lineDCrossing = ExtLine.AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);

                if (lineACrossing || lineBCrossing || lineCCrossing || lineDCrossing)
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            else
            {
                return (true);
            }
        }

        //Returns true if "point" is in a rectangle mad up of RectA to RectD. The line point is assumed to be on the same 
        //plane as the rectangle. If the point is not on the plane, use ProjectPointOnPlane() first.
        public static bool IsPointIn2DRectangle(Vector3 point, Vector3 rectA, Vector3 rectC, Vector3 rectB, Vector3 rectD)
        {
            Vector3 vector;
            Vector3 linePoint;

            //get the center of the rectangle
            vector = rectC - rectA;
            float size = -(vector.magnitude / 2f);
            vector = ExtVector3.AddVectorLength(vector, size);
            Vector3 middle = rectA + vector;

            Vector3 xVector = rectB - rectA;
            float width = xVector.magnitude / 2f;

            Vector3 yVector = rectD - rectA;
            float height = yVector.magnitude / 2f;

            linePoint = ExtLine.ProjectPointOnLine(middle, xVector.normalized, point);
            vector = linePoint - point;
            float yDistance = vector.magnitude;

            linePoint = ExtLine.ProjectPointOnLine(middle, yVector.normalized, point);
            vector = linePoint - point;
            float xDistance = vector.magnitude;

            if ((xDistance <= width) && (yDistance <= height))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        /// <summary>
        /// from a given point in space, order the face, from the closest to the farrest from the point
        /// 
        ///         cube
        ///      6 ------------ 7
        ///    / |    3       / |
        ///  5 ------------ 8   | 
        ///  |   |          |   |   
        ///  | 5 |     6    | 2-|------------  face
        ///  |   |   1      |   | 
        ///  |  2 ----------|-- 3 
        ///  |/       4     | /   
        ///  1 ------------ 4     
        /// </summary>
        /// <returns></returns>
        public static FloatRange[] GetOrdersOfFaceFromPoint(ExtCube cube, Vector3 point)
        {
            FloatRange[] faceDistance = new FloatRange[6];

            faceDistance[0] = new FloatRange(1, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P1, cube.P5, cube.P8, cube.P4), point));
            faceDistance[1] = new FloatRange(2, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P4, cube.P8, cube.P7, cube.P3), point));
            faceDistance[2] = new FloatRange(3, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P5, cube.P6, cube.P7, cube.P8), point));
            faceDistance[3] = new FloatRange(4, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P1, cube.P2, cube.P3, cube.P4), point));
            faceDistance[4] = new FloatRange(5, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P2, cube.P6, cube.P5, cube.P1), point));
            faceDistance[5] = new FloatRange(6, ExtVector3.DistanceSquared(ExtVector3.GetMeanOfXPoints(cube.P3, cube.P7, cube.P6, cube.P2), point));

            faceDistance = FloatRange.Sort(faceDistance);
            return (faceDistance);
        }
        //end class
    }
    //end nameSpace
}