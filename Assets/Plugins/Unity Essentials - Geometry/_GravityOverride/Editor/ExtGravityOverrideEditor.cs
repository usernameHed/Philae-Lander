using UnityEssentials.Geometry.shape2d;
using UnityEssentials.Geometry.shape3d;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Geometry.extensions;
using UnityEssentials.Geometry.Editor;

namespace UnityEssentials.Geometry.GravityOverride.Editor
{
    public static class ExtGravityOverrideEditor
    {

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
        public static GravityOverrideCube DrawCube(ExtCube cube, GravityOverrideCube cubeGravity, Color color, out bool hasChanged)
        {
            hasChanged = false;
            bool changed = hasChanged;

            float sizeLine = cube.LocalScale.magnitude / 35;
            float sizePoint = cube.LocalScale.magnitude / 30;

            Color colorQuad = new Color(color.r, color.g, color.b, color.a * 0.5f);
            Color colorLine = new Color(color.r, color.g, color.b, color.a * 0.8f);

            //we have to order the button from the point of view of the camera
            FloatRange[] facesArranged = ExtCube.GetOrdersOfFaceFromPoint(cube, ExtSceneView.GetSceneViewCameraTransform().position);
            for (int i = facesArranged.Length - 1; i >= 0; i--)
            {
                switch (facesArranged[i].Min)
                {
                    case 1:
                        cubeGravity.Face1 = DrawQuadFace(new ExtQuad(cube.P1, cube.P5, cube.P8, cube.P4), cubeGravity.Face1, false, colorQuad, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line1 = DrawLineTrunk(cubeGravity.Line1, cube.P1, cube.P5, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line2 = DrawLineTrunk(cubeGravity.Line2, cube.P5, cube.P8, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line3 = DrawLineTrunk(cubeGravity.Line3, cube.P8, cube.P4, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line4 = DrawLineTrunk(cubeGravity.Line4, cube.P4, cube.P1, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point1 = DrawPoint(cubeGravity.Point1, cube.P1, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point5 = DrawPoint(cubeGravity.Point5, cube.P5, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point8 = DrawPoint(cubeGravity.Point8, cube.P8, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point4 = DrawPoint(cubeGravity.Point4, cube.P4, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        break;
                    case 2:
                        cubeGravity.Face2 = DrawQuadFace(new ExtQuad(cube.P4, cube.P8, cube.P7, cube.P3), cubeGravity.Face2, false, colorQuad, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line3 = DrawLineTrunk(cubeGravity.Line3, cube.P4, cube.P8,   colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line10 = DrawLineTrunk(cubeGravity.Line10, cube.P8, cube.P7, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line11 = DrawLineTrunk(cubeGravity.Line11, cube.P7, cube.P3, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line12 = DrawLineTrunk(cubeGravity.Line12, cube.P3, cube.P4, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point4 = DrawPoint(cubeGravity.Point4, cube.P4, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point8 = DrawPoint(cubeGravity.Point8, cube.P8, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point7 = DrawPoint(cubeGravity.Point7, cube.P7, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point3 = DrawPoint(cubeGravity.Point3, cube.P3, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        break;
                    case 3:
                        cubeGravity.Face3 = DrawQuadFace(new ExtQuad(cube.P5, cube.P6, cube.P7, cube.P8), cubeGravity.Face3, false, colorQuad, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line6 = DrawLineTrunk(cubeGravity.Line6, cube.P5, cube.P6, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line8 = DrawLineTrunk(cubeGravity.Line8, cube.P6, cube.P7,  colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line10 = DrawLineTrunk(cubeGravity.Line10, cube.P7, cube.P8, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line2 = DrawLineTrunk(cubeGravity.Line2, cube.P5, cube.P8,  colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point5 = DrawPoint(cubeGravity.Point5, cube.P5, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point6 = DrawPoint(cubeGravity.Point6, cube.P6, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point7 = DrawPoint(cubeGravity.Point7, cube.P7, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point8 = DrawPoint(cubeGravity.Point8, cube.P8, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        break;
                    case 4:
                        cubeGravity.Face4 = DrawQuadFace(new ExtQuad(cube.P2, cube.P1, cube.P4, cube.P3), cubeGravity.Face4, false, colorQuad, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line7 = DrawLineTrunk(cubeGravity.Line7, cube.P2, cube.P1,   colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line4 = DrawLineTrunk(cubeGravity.Line4, cube.P1, cube.P4,   colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line12 = DrawLineTrunk(cubeGravity.Line12, cube.P4, cube.P3, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line9 = DrawLineTrunk(cubeGravity.Line9, cube.P2, cube.P3,   colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point2 = DrawPoint(cubeGravity.Point2, cube.P2, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point1 = DrawPoint(cubeGravity.Point1, cube.P1, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point4 = DrawPoint(cubeGravity.Point4, cube.P4, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point3 = DrawPoint(cubeGravity.Point3, cube.P3, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        break;
                    case 5:
                        cubeGravity.Face5 = DrawQuadFace(new ExtQuad(cube.P2, cube.P6, cube.P5, cube.P1), cubeGravity.Face5, false, colorQuad, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line5 = DrawLineTrunk(cubeGravity.Line5, cube.P2, cube.P6, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line6 = DrawLineTrunk(cubeGravity.Line6, cube.P6, cube.P5, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line1 = DrawLineTrunk(cubeGravity.Line1, cube.P5, cube.P1, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line7 = DrawLineTrunk(cubeGravity.Line7, cube.P1, cube.P2, colorLine,  sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point2 = DrawPoint(cubeGravity.Point2, cube.P2, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point6 = DrawPoint(cubeGravity.Point6, cube.P6, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point5 = DrawPoint(cubeGravity.Point5, cube.P5, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point1 = DrawPoint(cubeGravity.Point1, cube.P1, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        break;
                    case 6:
                        cubeGravity.Face6 = DrawQuadFace(new ExtQuad(cube.P3, cube.P7, cube.P6, cube.P2), cubeGravity.Face6, false, colorQuad, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line11 = DrawLineTrunk(cubeGravity.Line11, cube.P3, cube.P7, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line8 = DrawLineTrunk(cubeGravity.Line8, cube.P7, cube.P6,   colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line5 = DrawLineTrunk(cubeGravity.Line5, cube.P6, cube.P2,   colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Line9 = DrawLineTrunk(cubeGravity.Line9, cube.P3, cube.P2, colorLine, sizeLine, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point3 = DrawPoint(cubeGravity.Point3, cube.P3, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point7 = DrawPoint(cubeGravity.Point7, cube.P7, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point6 = DrawPoint(cubeGravity.Point6, cube.P6, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        cubeGravity.Point2 = DrawPoint(cubeGravity.Point2, cube.P2, color, sizePoint, out changed); hasChanged = (changed) ? true : hasChanged;
                        break;
                }
            }
            return (cubeGravity);
        }

        ///     2 ------------- 3 
        ///   /               /   
        ///  1 ------------ 4  
        public static GravityOverrideQuad DrawQuadWithBorders(ExtQuad quad, GravityOverrideQuad quadGravity, Color color, out bool hasChanged)
        {
            hasChanged = false;
            bool changed = hasChanged;

            float sizeLine = quad.LocalScale.magnitude / 35;
            float sizePoint = quad.LocalScale.magnitude / 30;

            Color colorFace = new Color(color.r, color.g, color.b, color.a * 0.5f);
            Color colorLine = new Color(color.r, color.g, color.b, color.a * 0.8f);

            quadGravity.Face1 = DrawQuadFace(quad, quadGravity.Face1, quad.AllowBottom, colorFace, out changed);     hasChanged = (changed) ? true : hasChanged;

            quadGravity.Line1 = ExtGravityOverrideEditor.DrawLineTrunk(quadGravity.Line1, quad.P1, quad.P2, colorLine, sizeLine, out changed);    hasChanged = (changed) ? true : hasChanged;
            quadGravity.Line2 = ExtGravityOverrideEditor.DrawLineTrunk(quadGravity.Line2, quad.P2, quad.P3, colorLine, sizeLine, out changed);    hasChanged = (changed) ? true : hasChanged;
            quadGravity.Line3 = ExtGravityOverrideEditor.DrawLineTrunk(quadGravity.Line3, quad.P3, quad.P4, colorLine, sizeLine, out changed);    hasChanged = (changed) ? true : hasChanged;
            quadGravity.Line4 = ExtGravityOverrideEditor.DrawLineTrunk(quadGravity.Line4, quad.P4, quad.P1, colorLine, sizeLine, out changed);    hasChanged = (changed) ? true : hasChanged;

            quadGravity.Point1 = ExtGravityOverrideEditor.DrawPoint(quadGravity.Point1, quad.P1, color, sizePoint, out changed);  hasChanged = (changed) ? true : hasChanged;
            quadGravity.Point2 = ExtGravityOverrideEditor.DrawPoint(quadGravity.Point2, quad.P2, color, sizePoint, out changed);  hasChanged = (changed) ? true : hasChanged;
            quadGravity.Point3 = ExtGravityOverrideEditor.DrawPoint(quadGravity.Point3, quad.P3, color, sizePoint, out changed);  hasChanged = (changed) ? true : hasChanged;
            quadGravity.Point4 = ExtGravityOverrideEditor.DrawPoint(quadGravity.Point4, quad.P4, color, sizePoint, out changed);  hasChanged = (changed) ? true : hasChanged;

            return (quadGravity);
        }

        public static bool DrawQuadFace(ExtQuad quad, bool face, bool allowBottom, Color color, out bool hasChanged)
        {
            hasChanged = false;

            float scaleRect = quad.LocalScale.magnitude;
            float xScale = quad.LenghtX / 2;
            float yScale = quad.LenghtY / 2;


            Matrix4x4 scaleMatrix = Matrix4x4.TRS(quad.Position, quad.Rotation, new Vector3(xScale, 1, yScale));
            using (new Handles.DrawingScope(scaleMatrix))
            {
                Vector3 up = scaleMatrix.ExtractRotation() * Vector3.up;
                bool isCameraViewBehindFace = Vector3.Dot(ExtSceneView.GetSceneViewCameraTransform().forward, up) > 0 && !allowBottom;

                Handles.color = (isCameraViewBehindFace) ? new Color(color.r, color.g, color.b, color.a / 2f) : color;

                float scale = 1f;
                Vector3[] verts = new Vector3[]
                {
                    new Vector3(- scale, 0,  - scale),
                    new Vector3(- scale, 0,  + scale),
                    new Vector3(+ scale, 0,  + scale),
                    new Vector3(+ scale, 0,  - scale)
                };

                if (!face)
                {
                    Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Color.clear);
                }

                Quaternion rotation = Quaternion.identity * Quaternion.LookRotation(Vector3.up);
                

                if (!Event.current.alt && Event.current.button != 2 && Handles.Button(
                    Vector3.zero,
                    Quaternion.identity * Quaternion.LookRotation(Vector3.up),
                    1,
                    1,
                    Handles.RectangleHandleCap))
                {
                    if (isCameraViewBehindFace)
                    {
                        Debug.Log("not behind face");
                        return (false);
                    }
                    Debug.Log("face pressed");
                    face = !face;
                    hasChanged = true;
                    Use();
                }
            }
            return (face);
        }

        public static void Use()
        {
            if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
            {
                Event.current.Use();
            }
        }

        public static GravityOverrideCylinder DrawCylinder(ExtCylinder cylinder, ExtCircle circle1, ExtCircle circle2, GravityOverrideCylinder cylinderGravity, Color color, out bool hasChanged)
        {
            float sizeLine = cylinder.LocalScale.magnitude / 20;
            
            hasChanged = false;
            bool changed = hasChanged;

            cylinderGravity.Disc1 = ExtGravityOverrideEditor.DrawDisc(circle1, cylinderGravity.Disc1, color, false, out changed);
            hasChanged = (changed) ? true : hasChanged;
            cylinderGravity.Disc2 = ExtGravityOverrideEditor.DrawDisc(circle2, cylinderGravity.Disc2, color, false, out changed);
            hasChanged = (changed) ? true : hasChanged;
            cylinderGravity.Trunk = ExtGravityOverrideEditor.DrawLineTrunk(cylinderGravity.Trunk, cylinder.P1, cylinder.P2, new Color(color.r, color.g, color.b, color.a * 0.8f), sizeLine, out changed);
            hasChanged = (changed) ? true : hasChanged;
            return (cylinderGravity);
        }

        /// <summary>
        /// draw a disc
        /// </summary>
        public static GravityOverrideDisc DrawDisc(ExtCircle circle, GravityOverrideDisc discGravity, Color color, bool allowBottom, out bool hasChanged)
        {
            hasChanged = false;
            Quaternion rotation = ExtRotation.QuaternionFromVectorDirector(circle.Normal, Vector3.up);
            bool topFace = discGravity.Face;
            bool topExtremity = discGravity.Borders;

            bool isCameraViewBehindFace = Vector3.Dot(ExtSceneView.GetSceneViewCameraTransform().forward, circle.Normal) > 0 && !allowBottom;

            Handles.color = color;
            if (!Event.current.alt && Event.current.button != 2 && Handles.Button(circle.Point,
                rotation,
                circle.Radius,
                circle.Radius, Handles.CircleHandleCap))
            {
                Debug.Log("extremity pressed");
                discGravity.Borders = !discGravity.Borders;
                hasChanged = true;
                Use();
            }

            if (!topFace)
            {
                Handles.color = (isCameraViewBehindFace) ? new Color(color.r, color.g, color.b, color.a / 2) : color;
                Handles.DrawSolidDisc(circle.Point, circle.Normal, circle.Radius / 10 * 8);
            }
            if (!topExtremity)
            {
                Handles.color = (isCameraViewBehindFace) ? new Color(color.r, color.g, color.b, color.a / 2) : color;
                ExtHandle.DrawCircleThickness(circle, 50, ExtHandle.DrawOutlineType.INSIDE);
            }
            Handles.color = Color.red;

            if (!Event.current.alt && Event.current.button != 2 && Handles.Button(circle.Point,
                rotation,
                circle.Radius / 10 * 7,
                circle.Radius / 10 * 7, Handles.CircleHandleCap))
            {
                if (isCameraViewBehindFace)
                {
                    Debug.Log("not behind face");
                }
                else
                {
                    Debug.Log("Face pressed !");
                    discGravity.Face = !discGravity.Face;
                    hasChanged = true;
                    Use();
                }
            }

            return (discGravity);
        }

        /// <summary>
        /// draw a line
        /// </summary>
        public static bool DrawLineTrunk(bool trunk, Vector3 p1, Vector3 p2, Color color, float size, out bool hasChanged)
        {
            hasChanged = false;

            Vector3 direction = (p1 - p2);
            Vector3 middle = ExtVector3.GetMeanOfXPoints(p1, p2);
            Quaternion rotation = ExtQuaternion.QuaternionFromLine(p1, p2, Vector3.up);

            Handles.color = color;
            float scaleCylinder = direction.magnitude;
            if (!trunk)
            {
                Matrix4x4 scaleMatrix = Matrix4x4.TRS(middle, rotation, new Vector3(size, size, scaleCylinder));
                using (new Handles.DrawingScope(scaleMatrix))
                {
                    Handles.CylinderHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
                }
            }
            
            if (!Event.current.alt && Event.current.button != 2 && Handles.Button(
                middle,
                rotation,
                scaleCylinder,
                scaleCylinder,
                ExtGravityOverrideEditor.LineHandleCap))
            {
                trunk = !trunk;
                hasChanged = true;
                Use();
            }
            return (trunk);
        }


        /// <summary>
        /// do a custom Line Handle Cap
        /// </summary>
        public static void LineHandleCap(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
            Vector3 sideways = rotation * new Vector3(0, 0, size / 2);
            Vector3 p1 = position - sideways;
            Vector3 p2 = position + sideways;

            switch (eventType)
            {
                case (EventType.Layout):
                    float distance = ExtGravityOverrideEditor.DistanceToCylinder(p1, p2, size);
                    HandleUtility.AddControl(controlId, distance);
                    break;
                case (EventType.Repaint):

                    Handles.DrawPolyLine(p1, p2);
                    break;
            }
        }


        public static float DistanceToCylinder(Vector3 p1, Vector3 p2, float size)
        {
            p1 = HandleUtility.WorldToGUIPoint(p1);
            p2 = HandleUtility.WorldToGUIPoint(p2);

            Vector2 point = Event.current.mousePosition;

            float retval = HandleUtility.DistancePointLine(point, p1, p2);
            retval -= size;
            if (retval < 0)
                retval = 0.0f;
            return retval;
        }

        public static bool DrawPoint(bool point, Vector3 position, Color color, float size, out bool hasChanged)
        {
            hasChanged = false;

            Handles.color = color;
            if (!point)
            {
                Handles.SphereHandleCap(0, position, Quaternion.identity, size, EventType.Repaint);
            }

            Handles.color = Color.clear;
            if (!Event.current.alt && Event.current.button != 2 && Handles.Button(
                position,
                Quaternion.identity,
                size / 2,
                size,
                Handles.SphereHandleCap))
            {
                point = !point;
                hasChanged = true;
                Use();
            }
            return (point);
        }

        public static void DrawPoint(Vector3 position, float size, Color color)
        {
            Handles.color = color;
            Handles.SphereHandleCap(0, position, Quaternion.identity, size, EventType.Repaint);
        }


        public static GravityOverrideLineTopDown DrawCapsule(ExtCapsule capsule, GravityOverrideLineTopDown capsuleGravity, Color color, out bool hasChanged)
        {
            float sizeLine = capsule.LocalScale.magnitude / 25;
            float sizePoint = capsule.LocalScale.magnitude / 20;

            hasChanged = false;
            bool changed = hasChanged;

            capsuleGravity.Trunk = ExtGravityOverrideEditor.DrawLineTrunk(capsuleGravity.Trunk, capsule.P1, capsule.P2, color, sizeLine, out changed);
            hasChanged = (changed) ? true : hasChanged;
            capsuleGravity.Top = ExtGravityOverrideEditor.DrawPoint(capsuleGravity.Top, capsule.P1, color, sizePoint, out changed);
            hasChanged = (changed) ? true : hasChanged;
            capsuleGravity.Bottom = ExtGravityOverrideEditor.DrawPoint(capsuleGravity.Bottom, capsule.P2, color, sizePoint, out changed);
            hasChanged = (changed) ? true : hasChanged;
            return (capsuleGravity);
        }

        public static GravityOverrideLineTopDown DrawLine3d(ExtLine3d line, GravityOverrideLineTopDown lineGravity, Color color, out bool hasChanged)
        {
            float sizeLine = line.LocalScale.magnitude / 25;
            float sizePoint = line.LocalScale.magnitude / 20;

            hasChanged = false;
            bool changed = hasChanged;

            lineGravity.Trunk = ExtGravityOverrideEditor.DrawLineTrunk(lineGravity.Trunk, line.P1, line.P2, color, sizeLine, out changed);
            hasChanged = (changed) ? true : hasChanged;
            lineGravity.Top = ExtGravityOverrideEditor.DrawPoint(lineGravity.Top, line.P1, color, sizePoint, out changed);
            hasChanged = (changed) ? true : hasChanged;
            lineGravity.Bottom = ExtGravityOverrideEditor.DrawPoint(lineGravity.Bottom, line.P2, color, sizePoint, out changed);
            hasChanged = (changed) ? true : hasChanged;
            return (lineGravity);
        }

        public static GravityOverrideLineTopDown[] DrawPolyLines(ExtPolyLines polyLine, GravityOverrideLineTopDown[] polyLineGravity, Color color, out bool hasChanged)
        {
            float sizeLine = polyLine.LocalScale.magnitude / 25;
            float sizePoint = polyLine.LocalScale.magnitude / 20;

            hasChanged = false;
            bool changed = hasChanged;

            for (int i = 0; i < polyLineGravity.Length; i++)
            {
                polyLineGravity[i].Trunk = ExtGravityOverrideEditor.DrawLineTrunk(polyLineGravity[i].Trunk, polyLine.LineAt(i).P1, polyLine.LineAt(i).P2, color, sizeLine, out changed);
                hasChanged = (changed) ? true : hasChanged;
                polyLineGravity[i].Top = ExtGravityOverrideEditor.DrawPoint(polyLineGravity[i].Top, polyLine.LineAt(i).P1, color, sizePoint, out changed);
                hasChanged = (changed) ? true : hasChanged;
                polyLineGravity[i].Bottom = ExtGravityOverrideEditor.DrawPoint(polyLineGravity[i].Bottom, polyLine.LineAt(i).P2, color, sizePoint, out changed);
                hasChanged = (changed) ? true : hasChanged;
            }
            return (polyLineGravity);
        }

        public static GravityOverrideConeSphereBase DrawConeSphereBase(ExtConeSphereBase cone, GravityOverrideConeSphereBase coneGravity, Color color, out bool hasChanged)
        {
            float sizeLine = cone.LocalScale.magnitude / 20;
            float sizePoint = cone.LocalScale.magnitude / 15;

            hasChanged = false;
            bool changed = hasChanged;

            coneGravity.Base = ExtGravityOverrideEditor.DrawDisc(cone.Base, coneGravity.Base, color, false, out changed);
            hasChanged = (changed) ? true : hasChanged;

            Color colorLine = new Color(color.r, color.g, color.b, color.a * 0.8f);

            coneGravity.Trunk = ExtGravityOverrideEditor.DrawLineTrunk(coneGravity.Trunk, cone.P1, cone.P2, colorLine, sizeLine, out changed);
            hasChanged = (changed) ? true : hasChanged;
            coneGravity.Top = ExtGravityOverrideEditor.DrawPoint(coneGravity.Top, cone.P1, color, sizePoint, out changed);
            hasChanged = (changed) ? true : hasChanged;


            return (coneGravity);
        }


        public static GravityOverrideLineTopDown DrawCapsuleHalf(ExtHalfCapsule capsuleHalf, GravityOverrideLineTopDown capsuleGravity, Color color, out bool hasChanged)
        {
            float sizeLine = capsuleHalf.LocalScale.magnitude / 25;
            float sizePoint = capsuleHalf.LocalScale.magnitude / 20;

            hasChanged = false;
            bool changed = hasChanged;

            Color colorLine = new Color(color.r, color.g, color.b, color.a * 0.8f);
            capsuleGravity.Trunk = ExtGravityOverrideEditor.DrawLineTrunk(capsuleGravity.Trunk, capsuleHalf.P1, capsuleHalf.P2, colorLine, sizeLine, out changed);
            hasChanged = (changed) ? true : hasChanged;
            capsuleGravity.Top = ExtGravityOverrideEditor.DrawPoint(capsuleGravity.Top, capsuleHalf.P1, color, sizePoint, out changed);
            hasChanged = (changed) ? true : hasChanged;
            capsuleGravity.Bottom = ExtGravityOverrideEditor.DrawPoint(capsuleGravity.Bottom, capsuleHalf.P2, color, sizePoint, out changed);
            hasChanged = (changed) ? true : hasChanged;
            return (capsuleGravity);
        }

        public static void ApplyModificationToDisc(SerializedProperty disc1, GravityOverrideDisc datas)
        {
            disc1.GetPropertie(nameof(datas.Face)).boolValue = datas.Face;
            disc1.GetPropertie(nameof(datas.Borders)).boolValue = datas.Borders;
            disc1.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
        }

        public static void ApplyModificationToCylinder(SerializedProperty cylinder, GravityOverrideCylinder datas)
        {
            cylinder.GetPropertie(nameof(datas.Trunk)).boolValue = datas.Trunk;
            cylinder.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
            ApplyModificationToDisc(cylinder.GetPropertie(nameof(datas.Disc1)), datas.Disc1);
            ApplyModificationToDisc(cylinder.GetPropertie(nameof(datas.Disc2)), datas.Disc2);
        }

        public static void ApplyModificationToCapsuleOrLine(SerializedProperty capsule, GravityOverrideLineTopDown datas)
        {
            capsule.GetPropertie(nameof(datas.Trunk)).boolValue = datas.Trunk;
            capsule.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
            capsule.GetPropertie(nameof(datas.Top)).boolValue = datas.Top;
            capsule.GetPropertie(nameof(datas.Bottom)).boolValue = datas.Bottom;
        }

        public static void ApplyModificationOfExtPolyLine(SerializedProperty polyLineGravityArray, GravityOverrideLineTopDown[] datas)
        {
            for (int i = 0; i < polyLineGravityArray.arraySize; i++)
            {
                SerializedProperty polyLineGravity = polyLineGravityArray.GetArrayElementAtIndex(i);
                GravityOverrideLineTopDown gravityData = datas[i];

                polyLineGravity.GetPropertie(nameof(gravityData.Trunk)).boolValue = gravityData.Trunk;
                polyLineGravity.GetPropertie("_canApplyGravity").boolValue = gravityData.CanApplyGravity;
                polyLineGravity.GetPropertie(nameof(gravityData.Top)).boolValue = gravityData.Top;
                polyLineGravity.GetPropertie(nameof(gravityData.Bottom)).boolValue = gravityData.Bottom;
            }
        }

        public static void ApplyModificationToConeSphereBase(SerializedProperty cone, GravityOverrideConeSphereBase datas)
        {
            cone.GetPropertie(nameof(datas.Trunk)).boolValue = datas.Trunk;
            cone.GetPropertie(nameof(datas.Top)).boolValue = datas.Top;
            cone.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
            ApplyModificationToDisc(cone.GetPropertie(nameof(datas.Base)), datas.Base);
        }

        public static void ApplyModificationToQuad(SerializedProperty quad, GravityOverrideQuad datas)
        {
            quad.GetPropertie(nameof(datas.Face1)).boolValue = datas.Face1;
            quad.GetPropertie(nameof(datas.Line1)).boolValue = datas.Line1;
            quad.GetPropertie(nameof(datas.Line2)).boolValue = datas.Line2;
            quad.GetPropertie(nameof(datas.Line3)).boolValue = datas.Line3;
            quad.GetPropertie(nameof(datas.Line4)).boolValue = datas.Line4;
            quad.GetPropertie(nameof(datas.Point1)).boolValue = datas.Point1;
            quad.GetPropertie(nameof(datas.Point2)).boolValue = datas.Point2;
            quad.GetPropertie(nameof(datas.Point3)).boolValue = datas.Point3;
            quad.GetPropertie(nameof(datas.Point4)).boolValue = datas.Point4;
            quad.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
        }

        public static void ApplyModificationToCube(SerializedProperty cube, GravityOverrideCube datas)
        {
            cube.GetPropertie(nameof(datas.Face1)).boolValue = datas.Face1;
            cube.GetPropertie(nameof(datas.Face2)).boolValue = datas.Face2;
            cube.GetPropertie(nameof(datas.Face3)).boolValue = datas.Face3;
            cube.GetPropertie(nameof(datas.Face4)).boolValue = datas.Face4;
            cube.GetPropertie(nameof(datas.Face5)).boolValue = datas.Face5;
            cube.GetPropertie(nameof(datas.Face6)).boolValue = datas.Face6;
            
            cube.GetPropertie(nameof(datas.Line1)).boolValue = datas.Line1;
            cube.GetPropertie(nameof(datas.Line2)).boolValue = datas.Line2;
            cube.GetPropertie(nameof(datas.Line3)).boolValue = datas.Line3;
            cube.GetPropertie(nameof(datas.Line4)).boolValue = datas.Line4;
            cube.GetPropertie(nameof(datas.Line5)).boolValue = datas.Line5;
            cube.GetPropertie(nameof(datas.Line6)).boolValue = datas.Line6;
            cube.GetPropertie(nameof(datas.Line7)).boolValue = datas.Line7;
            cube.GetPropertie(nameof(datas.Line8)).boolValue = datas.Line8;
            cube.GetPropertie(nameof(datas.Line9)).boolValue = datas.Line9;
            cube.GetPropertie(nameof(datas.Line10)).boolValue = datas.Line10;
            cube.GetPropertie(nameof(datas.Line11)).boolValue = datas.Line11;
            cube.GetPropertie(nameof(datas.Line12)).boolValue = datas.Line12;


            cube.GetPropertie(nameof(datas.Point1)).boolValue = datas.Point1;
            cube.GetPropertie(nameof(datas.Point2)).boolValue = datas.Point2;
            cube.GetPropertie(nameof(datas.Point3)).boolValue = datas.Point3;
            cube.GetPropertie(nameof(datas.Point4)).boolValue = datas.Point4;
            cube.GetPropertie(nameof(datas.Point5)).boolValue = datas.Point5;
            cube.GetPropertie(nameof(datas.Point6)).boolValue = datas.Point6;
            cube.GetPropertie(nameof(datas.Point7)).boolValue = datas.Point7;
            cube.GetPropertie(nameof(datas.Point8)).boolValue = datas.Point8;

            cube.GetPropertie("_canApplyGravityBordersAndFace1").boolValue = datas.CanApplyGravityBordersAndFace1;
            cube.GetPropertie("_canApplyGravityBordersAndFace2").boolValue = datas.CanApplyGravityBordersAndFace2;
            cube.GetPropertie("_canApplyGravityBordersAndFace3").boolValue = datas.CanApplyGravityBordersAndFace3;
            cube.GetPropertie("_canApplyGravityBordersAndFace4").boolValue = datas.CanApplyGravityBordersAndFace4;
            cube.GetPropertie("_canApplyGravityBordersAndFace5").boolValue = datas.CanApplyGravityBordersAndFace5;
            cube.GetPropertie("_canApplyGravityBordersAndFace6").boolValue = datas.CanApplyGravityBordersAndFace6;

            cube.GetPropertie("_canApplyGravity").boolValue = datas.CanApplyGravity;
        }

        //end of class
    }
    //end of nameSpace
}