using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEssentials.Geometry.shape3d;
using UnityEssentials.Geometry.shape2d;
using System.Collections.Generic;

namespace UnityEssentials.Geometry.extensions
{
    /// <summary>
    /// Debug Extension
    /// 	- Static class that extends Unity's debugging functionallity.
    /// 	- Attempts to mimic Unity's existing debugging behaviour for ease-of-use.
    /// 	- Includes gizmo drawing methods for less memory-intensive debug visualization.
    /// </summary>

    public static class ExtDrawGuizmos
    {
        #region DebugDrawFunctions

#if UNITY_EDITOR
        public static void DrawLabel(Vector3 position, string label, Color color)
        {
            Handles.color = color;
            Handles.Label(position + Vector3.down * 0.03f + Vector3.right * 0.03f, label);
        }
#endif

        public static void DrawTransform(Vector3 position, Vector3 forward, Vector3 up, Vector3 right, float size = 1f, float duration = 0)
        {
            Debug.DrawLine(position, position + (forward.normalized * size), Color.blue, duration);
            Debug.DrawLine(position, position + (up.normalized * size), Color.green, duration);
            Debug.DrawLine(position, position + (right.normalized * size), Color.red, duration);
        }
        public static void DrawTransform(Transform point, float size = 1f, float duration = 0)
        {
            DrawTransform(point.position, point.forward, point.up, point.right, size, duration);
        }

        public static void DrawCross(Vector3 position)
        {
            Gizmos.DrawRay(new Ray(position, new Vector3(10, 10, 10)));
            Gizmos.DrawRay(new Ray(position, new Vector3(-10, -10, -10)));

            Gizmos.DrawRay(new Ray(position, new Vector3(-10, 10, 10)));
            Gizmos.DrawRay(new Ray(position, new Vector3(10, -10, -10)));

            Gizmos.DrawRay(new Ray(position, new Vector3(-10, 0, 10)));
            Gizmos.DrawRay(new Ray(position, new Vector3(10, 0, -10)));

            Gizmos.DrawRay(new Ray(position, new Vector3(10, 10, 0)));
            Gizmos.DrawRay(new Ray(position, new Vector3(-10, -10, 0)));

            Gizmos.DrawRay(new Ray(position, new Vector3(0, 10, 10)));
            Gizmos.DrawRay(new Ray(position, new Vector3(0, -10, -10)));

            Gizmos.DrawRay(new Ray(position, new Vector3(0, 0, 10)));
            Gizmos.DrawRay(new Ray(position, new Vector3(0, 0, -10)));

            Gizmos.DrawRay(new Ray(position, new Vector3(0, 10, 0)));
            Gizmos.DrawRay(new Ray(position, new Vector3(0, -10, 0)));
        }

        /*public static void DrawArrow(Vector3 start, Vector3 end)
        {

        }*/

        /// <summary>
        /// 	- Debugs a point.
        /// </summary>
        /// <param name='position'>
        /// 	- The point to debug.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the point.
        /// </param>
        /// <param name='scale'>
        /// 	- The size of the point.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the point.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not this point should be faded when behind other objects.
        /// </param>
        public static void DebugPoint(Vector3 position, Color color, float scale = 1.0f, float duration = 0, bool depthTest = true)
        {
            color = (color == default(Color)) ? Color.white : color;

            Debug.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale, color, duration, depthTest);
            Debug.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale, color, duration, depthTest);
            Debug.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a point.
        /// </summary>
        /// <param name='position'>
        /// 	- The point to debug.
        /// </param>
        /// <param name='scale'>
        /// 	- The size of the point.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the point.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not this point should be faded when behind other objects.
        /// </param>
        public static void DebugPoint(Vector3 position, float scale = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugPoint(position, Color.white, scale, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs an axis-aligned bounding box.
        /// </summary>
        /// <param name='bounds'>
        /// 	- The bounds to debug.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the bounds.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the bounds.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the bounds should be faded when behind other objects.
        /// </param>
        public static void DebugBounds(Bounds bounds, Color color, float duration = 0, bool depthTest = true)
        {
            Vector3 center = bounds.center;

            float x = bounds.extents.x;
            float y = bounds.extents.y;
            float z = bounds.extents.z;

            Vector3 ruf = center + new Vector3(x, y, z);
            Vector3 rub = center + new Vector3(x, y, -z);
            Vector3 luf = center + new Vector3(-x, y, z);
            Vector3 lub = center + new Vector3(-x, y, -z);

            Vector3 rdf = center + new Vector3(x, -y, z);
            Vector3 rdb = center + new Vector3(x, -y, -z);
            Vector3 lfd = center + new Vector3(-x, -y, z);
            Vector3 lbd = center + new Vector3(-x, -y, -z);

            Debug.DrawLine(ruf, luf, color, duration, depthTest);
            Debug.DrawLine(ruf, rub, color, duration, depthTest);
            Debug.DrawLine(luf, lub, color, duration, depthTest);
            Debug.DrawLine(rub, lub, color, duration, depthTest);

            Debug.DrawLine(ruf, rdf, color, duration, depthTest);
            Debug.DrawLine(rub, rdb, color, duration, depthTest);
            Debug.DrawLine(luf, lfd, color, duration, depthTest);
            Debug.DrawLine(lub, lbd, color, duration, depthTest);

            Debug.DrawLine(rdf, lfd, color, duration, depthTest);
            Debug.DrawLine(rdf, rdb, color, duration, depthTest);
            Debug.DrawLine(lfd, lbd, color, duration, depthTest);
            Debug.DrawLine(lbd, rdb, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs an axis-aligned bounding box.
        /// </summary>
        /// <param name='bounds'>
        /// 	- The bounds to debug.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the bounds.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the bounds should be faded when behind other objects.
        /// </param>
        public static void DebugBounds(Bounds bounds, float duration = 0, bool depthTest = true)
        {
            DebugBounds(bounds, Color.white, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a local cube.
        /// </summary>
        /// <param name='transform'>
        /// 	- The transform that the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The size of the cube.
        /// </param>
        /// <param name='color'>
        /// 	- Color of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The position (relative to transform) where the cube will be debugged.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cube.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cube should be faded when behind other objects.
        /// </param>
        public static void DebugLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
        {
            Vector3 lbb = transform.TransformPoint(center + ((-size) * 0.5f));
            Vector3 rbb = transform.TransformPoint(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            Vector3 lbf = transform.TransformPoint(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 rbf = transform.TransformPoint(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

            Vector3 lub = transform.TransformPoint(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 rub = transform.TransformPoint(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

            Vector3 luf = transform.TransformPoint(center + ((size) * 0.5f));
            Vector3 ruf = transform.TransformPoint(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

            Debug.DrawLine(lbb, rbb, color, duration, depthTest);
            Debug.DrawLine(rbb, lbf, color, duration, depthTest);
            Debug.DrawLine(lbf, rbf, color, duration, depthTest);
            Debug.DrawLine(rbf, lbb, color, duration, depthTest);

            Debug.DrawLine(lub, rub, color, duration, depthTest);
            Debug.DrawLine(rub, luf, color, duration, depthTest);
            Debug.DrawLine(luf, ruf, color, duration, depthTest);
            Debug.DrawLine(ruf, lub, color, duration, depthTest);

            Debug.DrawLine(lbb, lub, color, duration, depthTest);
            Debug.DrawLine(rbb, rub, color, duration, depthTest);
            Debug.DrawLine(lbf, luf, color, duration, depthTest);
            Debug.DrawLine(rbf, ruf, color, duration, depthTest);
        }

        public static void DebugCube(Vector3 position, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
        {
            Vector3 lbb = position + center + ((-size) * 0.5f);
            Vector3 rbb = position + center + (new Vector3(size.x, -size.y, -size.z) * 0.5f);

            Vector3 lbf = position + center + (new Vector3(size.x, -size.y, size.z) * 0.5f);
            Vector3 rbf = position + center + (new Vector3(-size.x, -size.y, size.z) * 0.5f);

            Vector3 lub = position + center + (new Vector3(-size.x, size.y, -size.z) * 0.5f);
            Vector3 rub = position + center + (new Vector3(size.x, size.y, -size.z) * 0.5f);

            Vector3 luf = position + center + ((size) * 0.5f);
            Vector3 ruf = position + center + (new Vector3(-size.x, size.y, size.z) * 0.5f);

            Debug.DrawLine(lbb, rbb, color, duration, depthTest);
            Debug.DrawLine(rbb, lbf, color, duration, depthTest);
            Debug.DrawLine(lbf, rbf, color, duration, depthTest);
            Debug.DrawLine(rbf, lbb, color, duration, depthTest);

            Debug.DrawLine(lub, rub, color, duration, depthTest);
            Debug.DrawLine(rub, luf, color, duration, depthTest);
            Debug.DrawLine(luf, ruf, color, duration, depthTest);
            Debug.DrawLine(ruf, lub, color, duration, depthTest);

            Debug.DrawLine(lbb, lub, color, duration, depthTest);
            Debug.DrawLine(rbb, rub, color, duration, depthTest);
            Debug.DrawLine(lbf, luf, color, duration, depthTest);
            Debug.DrawLine(rbf, ruf, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a local cube.
        /// </summary>
        /// <param name='transform'>
        /// 	- The transform that the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The size of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The position (relative to transform) where the cube will be debugged.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cube.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cube should be faded when behind other objects.
        /// </param>
        public static void DebugLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
        {
            DebugLocalCube(transform, size, Color.white, center, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a local cube.
        /// </summary>
        /// <param name='space'>
        /// 	- The space the cube will be local to.
        /// </param>
        /// <param name='size'>
        ///		- The size of the cube.
        /// </param>
        /// <param name='color'>
        /// 	- Color of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The position (relative to transform) where the cube will be debugged.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cube.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cube should be faded when behind other objects.
        /// </param>
        public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
        {
            color = (color == default(Color)) ? Color.white : color;

            Vector3 lbb = space.MultiplyPoint3x4(center + ((-size) * 0.5f));
            Vector3 rbb = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            Vector3 lbf = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 rbf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

            Vector3 lub = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 rub = space.MultiplyPoint3x4(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

            Vector3 luf = space.MultiplyPoint3x4(center + ((size) * 0.5f));
            Vector3 ruf = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

            Debug.DrawLine(lbb, rbb, color, duration, depthTest);
            Debug.DrawLine(rbb, lbf, color, duration, depthTest);
            Debug.DrawLine(lbf, rbf, color, duration, depthTest);
            Debug.DrawLine(rbf, lbb, color, duration, depthTest);

            Debug.DrawLine(lub, rub, color, duration, depthTest);
            Debug.DrawLine(rub, luf, color, duration, depthTest);
            Debug.DrawLine(luf, ruf, color, duration, depthTest);
            Debug.DrawLine(ruf, lub, color, duration, depthTest);

            Debug.DrawLine(lbb, lub, color, duration, depthTest);
            Debug.DrawLine(rbb, rub, color, duration, depthTest);
            Debug.DrawLine(lbf, luf, color, duration, depthTest);
            Debug.DrawLine(rbf, ruf, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a local cube.
        /// </summary>
        /// <param name='space'>
        /// 	- The space the cube will be local to.
        /// </param>
        /// <param name='size'>
        ///		- The size of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The position (relative to transform) where the cube will be debugged.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cube.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cube should be faded when behind other objects.
        /// </param>
        public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3), float duration = 0, bool depthTest = true)
        {
            DebugLocalCube(space, size, Color.white, center, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='up'>
        /// 	- The direction perpendicular to the surface of the circle.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the circle.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            Vector3 _up = up.normalized * radius;
            Vector3 _forward = Vector3.Slerp(_up, -_up, 0.5f);
            Vector3 _right = Vector3.Cross(_up, _forward).normalized * radius;

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = _right.x;
            matrix[1] = _right.y;
            matrix[2] = _right.z;

            matrix[4] = _up.x;
            matrix[5] = _up.y;
            matrix[6] = _up.z;

            matrix[8] = _forward.x;
            matrix[9] = _forward.y;
            matrix[10] = _forward.z;

            Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 _nextPoint = Vector3.zero;

            color = (color == default(Color)) ? Color.white : color;

            for (var i = 0; i < 91; i++)
            {
                _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
                _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
                _nextPoint.y = 0;

                _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

                Debug.DrawLine(_lastPoint, _nextPoint, color, duration, depthTest);
                _lastPoint = _nextPoint;
            }
        }

        /// <summary>
        /// 	- Debugs a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the circle.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugCircle(position, Vector3.up, color, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='up'>
        /// 	- The direction perpendicular to the surface of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the circle.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector3 position, Vector3 up, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugCircle(position, up, Color.white, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the circle.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the circle should be faded when behind other objects.
        /// </param>
        public static void DebugCircle(Vector3 position, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugCircle(position, Vector3.up, Color.white, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a wire sphere.
        /// </summary>
        /// <param name='position'>
        /// 	- The position of the center of the sphere.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the sphere.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the sphere.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the sphere.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the sphere should be faded when behind other objects.
        /// </param>
        public static void DebugWireSphere(Vector3 position, Color color, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            float angle = 10.0f;

            Vector3 x = new Vector3(position.x, position.y + radius * Mathf.Sin(0), position.z + radius * Mathf.Cos(0));
            Vector3 y = new Vector3(position.x + radius * Mathf.Cos(0), position.y, position.z + radius * Mathf.Sin(0));
            Vector3 z = new Vector3(position.x + radius * Mathf.Cos(0), position.y + radius * Mathf.Sin(0), position.z);

            Vector3 new_x;
            Vector3 new_y;
            Vector3 new_z;

            for (int i = 1; i < 37; i++)
            {

                new_x = new Vector3(position.x, position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad));
                new_y = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y, position.z + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad));
                new_z = new Vector3(position.x + radius * Mathf.Cos(angle * i * Mathf.Deg2Rad), position.y + radius * Mathf.Sin(angle * i * Mathf.Deg2Rad), position.z);

                Debug.DrawLine(x, new_x, color, duration, depthTest);
                Debug.DrawLine(y, new_y, color, duration, depthTest);
                Debug.DrawLine(z, new_z, color, duration, depthTest);

                x = new_x;
                y = new_y;
                z = new_z;
            }
        }

#if UNITY_EDITOR
        public static void DebugSphere(Vector3 position, Color color, float radius)
        {
            Handles.color = color;
            Handles.SphereHandleCap(0, position, Quaternion.identity, radius, EventType.Repaint);
        }
#endif

        public static void DrawHalfWireSphere(Vector3 position, Vector3 normal, Color color, float radius)
        {
            Vector3 up = normal * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            //Radial circles
            ExtDrawGuizmos.DebugCircle(position, up, color, radius);

            for (int i = 1; i < 26; i++)
            {
                Debug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + position, Vector3.Slerp(right, up, (i - 1) / 25.0f) + position, color);
                Debug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + position, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + position, color);
                Debug.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + position, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + position, color);
                Debug.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + position, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + position, color);
            }
        }

        public static void DebugCross(Transform transform, float lenght = 1, float duration = 1)
        {
            Debug.DrawRay(transform.position, transform.up * lenght, Color.green);
            Debug.DrawRay(transform.position, transform.forward * lenght, Color.blue);
            Debug.DrawRay(transform.transform.position, transform.right * lenght, Color.red);
        }

        public static void DebugCross(Vector3 position, Quaternion rotation, float lenght = 1, float duration = 1)
        {
            Debug.DrawRay(position, rotation * Vector3.up * lenght, Color.green);
            Debug.DrawRay(position, rotation * Vector3.forward * lenght, Color.blue);
            Debug.DrawRay(position, rotation * Vector3.right * lenght, Color.red);
        }

        /// <summary>
        /// 	- Debugs a wire sphere.
        /// </summary>
        /// <param name='position'>
        /// 	- The position of the center of the sphere.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the sphere.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the sphere.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the sphere should be faded when behind other objects.
        /// </param>
        public static void DebugWireSphere(Vector3 position, float radius = 1.0f, float duration = 0, bool depthTest = true)
        {
            DebugWireSphere(position, Color.white, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cylinder.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the cylinder.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the cylinder.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cylinder.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the cylinder.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cylinder.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cylinder should be faded when behind other objects.
        /// </param>
        public static void DebugCylinder(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true)
        {
            Vector3 up = (end - start).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            //Radial circles
            ExtDrawGuizmos.DebugCircle(start, up, color, radius, duration, depthTest);
            ExtDrawGuizmos.DebugCircle(end, -up, color, radius, duration, depthTest);
            ExtDrawGuizmos.DebugCircle((start + end) * 0.5f, up, color, radius, duration, depthTest);

            //Side lines
            Debug.DrawLine(start + right, end + right, color, duration, depthTest);
            Debug.DrawLine(start - right, end - right, color, duration, depthTest);

            Debug.DrawLine(start + forward, end + forward, color, duration, depthTest);
            Debug.DrawLine(start - forward, end - forward, color, duration, depthTest);

            //Start endcap
            Debug.DrawLine(start - right, start + right, color, duration, depthTest);
            Debug.DrawLine(start - forward, start + forward, color, duration, depthTest);

            //End endcap
            Debug.DrawLine(end - right, end + right, color, duration, depthTest);
            Debug.DrawLine(end - forward, end + forward, color, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cylinder.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the cylinder.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the cylinder.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the cylinder.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cylinder.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cylinder should be faded when behind other objects.
        /// </param>
        public static void DebugCylinder(Vector3 start, Vector3 end, float radius = 1, float duration = 0, bool depthTest = true)
        {
            DebugCylinder(start, end, Color.white, radius, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction for the cone gets wider in.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cone.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cone.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cone should be faded when behind other objects.
        /// </param>
        public static void DebugCone(Vector3 position, Vector3 direction, Color color, float angle = 45, float duration = 0, bool depthTest = true)
        {
            float length = direction.magnitude;

            Vector3 _forward = direction;
            Vector3 _up = Vector3.Slerp(_forward, -_forward, 0.5f);
            Vector3 _right = Vector3.Cross(_forward, _up).normalized * length;

            direction = direction.normalized;

            Vector3 slerpedVector = Vector3.Slerp(_forward, _up, angle / 90.0f);

            float dist;
            var farPlane = new Plane(-direction, position + _forward);
            var distRay = new Ray(position, slerpedVector);

            farPlane.Raycast(distRay, out dist);

            Debug.DrawRay(position, slerpedVector.normalized * dist, color);
            Debug.DrawRay(position, Vector3.Slerp(_forward, -_up, angle / 90.0f).normalized * dist, color, duration, depthTest);
            Debug.DrawRay(position, Vector3.Slerp(_forward, _right, angle / 90.0f).normalized * dist, color, duration, depthTest);
            Debug.DrawRay(position, Vector3.Slerp(_forward, -_right, angle / 90.0f).normalized * dist, color, duration, depthTest);

            ExtDrawGuizmos.DebugCircle(position + _forward, direction, color, (_forward - (slerpedVector.normalized * dist)).magnitude, duration, depthTest);
            ExtDrawGuizmos.DebugCircle(position + (_forward * 0.5f), direction, color, ((_forward * 0.5f) - (slerpedVector.normalized * (dist * 0.5f))).magnitude, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction for the cone gets wider in.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cone.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cone should be faded when behind other objects.
        /// </param>
        public static void DebugCone(Vector3 position, Vector3 direction, float angle = 45, float duration = 0, bool depthTest = true)
        {
            DebugCone(position, direction, Color.white, angle, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cone.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cone.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cone should be faded when behind other objects.
        /// </param>
        public static void DebugCone(Vector3 position, Color color, float angle = 45, float duration = 0, bool depthTest = true)
        {
            DebugCone(position, Vector3.up, color, angle, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the cone.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the cone should be faded when behind other objects.
        /// </param>
        public static void DebugCone(Vector3 position, float angle = 45, float duration = 0, bool depthTest = true)
        {
            DebugCone(position, Vector3.up, Color.white, angle, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs an arrow.
        /// </summary>
        /// <param name='position'>
        /// 	- The start position of the arrow.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction the arrow will point in.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the arrow.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the arrow.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the arrow should be faded when behind other objects. 
        /// </param>
        public static void DebugArrow(Vector3 position, Vector3 direction, Color color, float duration = 0, bool depthTest = true)
        {
            Debug.DrawRay(position, direction, color, duration, depthTest);
            ExtDrawGuizmos.DebugCone(position + direction, -direction * 0.333f, color, 15, duration, depthTest);
        }

        public static void DebugArrowConstant(Vector3 position, Vector3 direction, Color color, float size = 0.2f, float duration = 0, bool depthTest = true)
        {
            Debug.DrawRay(position, direction, color, duration, depthTest);
            ExtDrawGuizmos.DebugCone(position + direction, -direction.normalized * size, color, 15, duration, depthTest);
        }

        /// <summary>
        /// 	- Debugs an arrow.
        /// </summary>
        /// <param name='position'>
        /// 	- The start position of the arrow.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction the arrow will point in.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the arrow.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the arrow should be faded when behind other objects. 
        /// </param>
        public static void DebugArrow(Vector3 position, Vector3 direction, float duration = 0, bool depthTest = true)
        {
            DebugArrow(position, direction, Color.white, duration, depthTest);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 	- Debugs a capsule.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the capsule.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the capsule.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the capsule.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the capsule.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the capsule.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the capsule should be faded when behind other objects.
        /// </param>
        public static void DebugCapsule(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true, bool drawPoint = false)
        {
            Vector3 up = (end - start).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            float height = (start - end).magnitude;
            float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
            Vector3 middle = (end + start) * 0.5f;

            start = middle + ((start - middle).normalized * sideLength);
            end = middle + ((end - middle).normalized * sideLength);

            //Radial circles
            ExtDrawGuizmos.DebugCircle(start, up, color, radius, duration, depthTest);
            ExtDrawGuizmos.DebugCircle(end, -up, color, radius, duration, depthTest);

            //Side lines
            Debug.DrawLine(start + right, end + right, color, duration, depthTest);
            Debug.DrawLine(start - right, end - right, color, duration, depthTest);

            Debug.DrawLine(start + forward, end + forward, color, duration, depthTest);
            Debug.DrawLine(start - forward, end - forward, color, duration, depthTest);

            if (drawPoint)
            {
                Handles.Label(start + Vector3.down * 0.03f + Vector3.right * 0.03f, "1");
                Handles.Label(end + Vector3.down * 0.03f + Vector3.right * 0.03f, "2");
            }

            for (int i = 1; i < 26; i++)
            {
                Debug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                //End endcap
                Debug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end, color, duration, depthTest);
            }
        }



        public static void DebugCapsuleFromInsidePoint(Vector3 p1, Vector3 p2, Color color, float radius = 1, float duration = 0, bool depthTest = true, bool drawPoint = false)
        {
            Vector3 extremityP1 = p1 + (p1 - p2).normalized * radius;
            Vector3 extremityP2 = p2 - (p1 - p2).normalized * radius;
            DebugCapsule(extremityP1, extremityP2, color, radius, duration, depthTest, drawPoint);
        }

        /// <summary>
        /// 	- Debugs a capsule.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the capsule.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the capsule.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the capsule.
        /// </param>
        /// <param name='duration'>
        /// 	- How long to draw the capsule.
        /// </param>
        /// <param name='depthTest'>
        /// 	- Whether or not the capsule should be faded when behind other objects.
        /// </param>
        public static void DebugCapsule(Vector3 start, Vector3 end, float radius = 1, float duration = 0, bool depthTest = true)
        {
            DebugCapsule(start, end, Color.white, radius, duration, depthTest);
        }


        public static void DebugHalfCapsule(Vector3 start, Vector3 end, Color color, float radius = 1, float duration = 0, bool depthTest = true, bool drawPoint = true)
        {
            Vector3 up = (end - start).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            float height = (start - end).magnitude;
            float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
            Vector3 middle = (end + start) * 0.5f;

            start = middle + ((start - middle).normalized * sideLength);
            end = middle + ((end - middle).normalized * sideLength);

            //Radial circles
            ExtDrawGuizmos.DebugCircle(start, up, color, radius, duration, depthTest);
            ExtDrawGuizmos.DebugCircle(end, -up, color, radius, duration, depthTest);

            //Side lines
            Debug.DrawLine(start + right, end + right, color, duration, depthTest);
            Debug.DrawLine(start - right, end - right, color, duration, depthTest);

            Debug.DrawLine(start + forward, end + forward, color, duration, depthTest);
            Debug.DrawLine(start - forward, end - forward, color, duration, depthTest);

            if (drawPoint)
            {
                Handles.Label(start + Vector3.down * 0.03f + Vector3.right * 0.03f, "1");
                Handles.Label(end + Vector3.down * 0.03f + Vector3.right * 0.03f, "2");
            }

            for (int i = 1; i < 26; i++)
            {
                Debug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
                Debug.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start, color, duration, depthTest);
            }
        }

        public static void DebugHalfCapsuleFromInsidePoint(Vector3 p1, Vector3 p2, Color color, float radius = 1, float duration = 0, bool depthTest = true, bool drawPoint = false)
        {
            Vector3 extremityP1 = p1 + (p1 - p2).normalized * radius;
            Vector3 extremityP2 = p2 - (p1 - p2).normalized * radius;
            DebugHalfCapsule(extremityP1, extremityP2, color, radius, duration, depthTest, drawPoint);
        }
#endif


        #endregion

        #region GizmoDrawFunctions

        /// <summary>
        /// 	- Draws a point.
        /// </summary>
        /// <param name='position'>
        /// 	- The point to draw.
        /// </param>
        ///  <param name='color'>
        /// 	- The color of the drawn point.
        /// </param>
        /// <param name='scale'>
        /// 	- The size of the drawn point.
        /// </param>
        public static void DrawPoint(Vector3 position, Color color, float scale = 1.0f)
        {
            Color oldColor = Gizmos.color;

            Gizmos.color = color;
            Gizmos.DrawRay(position + (Vector3.up * (scale * 0.5f)), -Vector3.up * scale);
            Gizmos.DrawRay(position + (Vector3.right * (scale * 0.5f)), -Vector3.right * scale);
            Gizmos.DrawRay(position + (Vector3.forward * (scale * 0.5f)), -Vector3.forward * scale);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a point.
        /// </summary>
        /// <param name='position'>
        /// 	- The point to draw.
        /// </param>
        /// <param name='scale'>
        /// 	- The size of the drawn point.
        /// </param>
        public static void DrawPoint(Vector3 position, float scale = 1.0f)
        {
            DrawPoint(position, Color.white, scale);
        }

        /// <summary>
        /// 	- Draws an axis-aligned bounding box.
        /// </summary>
        /// <param name='bounds'>
        /// 	- The bounds to draw.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the bounds.
        /// </param>
        public static void DrawBounds(Bounds bounds, Color color)
        {
            Vector3 center = bounds.center;

            float x = bounds.extents.x;
            float y = bounds.extents.y;
            float z = bounds.extents.z;

            Vector3 ruf = center + new Vector3(x, y, z);
            Vector3 rub = center + new Vector3(x, y, -z);
            Vector3 luf = center + new Vector3(-x, y, z);
            Vector3 lub = center + new Vector3(-x, y, -z);

            Vector3 rdf = center + new Vector3(x, -y, z);
            Vector3 rdb = center + new Vector3(x, -y, -z);
            Vector3 lfd = center + new Vector3(-x, -y, z);
            Vector3 lbd = center + new Vector3(-x, -y, -z);

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawLine(ruf, luf);
            Gizmos.DrawLine(ruf, rub);
            Gizmos.DrawLine(luf, lub);
            Gizmos.DrawLine(rub, lub);

            Gizmos.DrawLine(ruf, rdf);
            Gizmos.DrawLine(rub, rdb);
            Gizmos.DrawLine(luf, lfd);
            Gizmos.DrawLine(lub, lbd);

            Gizmos.DrawLine(rdf, lfd);
            Gizmos.DrawLine(rdf, rdb);
            Gizmos.DrawLine(lfd, lbd);
            Gizmos.DrawLine(lbd, rdb);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws an axis-aligned bounding box.
        /// </summary>
        /// <param name='bounds'>
        /// 	- The bounds to draw.
        /// </param>
        public static void DrawBounds(Bounds bounds)
        {
            DrawBounds(bounds, Color.white);
        }

        /// <summary>
        /// 	- Draws a local cube.
        /// </summary>
        /// <param name='transform'>
        /// 	- The transform the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The local size of the cube.
        /// </param>
        /// <param name='center'>
        ///		- The local position of the cube.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cube.
        /// </param>
        public static void DrawLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3))
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Vector3 lbb = transform.TransformPoint(center + ((-size) * 0.5f));
            Vector3 rbb = transform.TransformPoint(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));

            Vector3 lbf = transform.TransformPoint(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 rbf = transform.TransformPoint(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));

            Vector3 lub = transform.TransformPoint(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 rub = transform.TransformPoint(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));

            Vector3 luf = transform.TransformPoint(center + ((size) * 0.5f));
            Vector3 ruf = transform.TransformPoint(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));

            Gizmos.DrawLine(lbb, rbb);
            Gizmos.DrawLine(rbb, lbf);
            Gizmos.DrawLine(lbf, rbf);
            Gizmos.DrawLine(rbf, lbb);

            Gizmos.DrawLine(lub, rub);
            Gizmos.DrawLine(rub, luf);
            Gizmos.DrawLine(luf, ruf);
            Gizmos.DrawLine(ruf, lub);

            Gizmos.DrawLine(lbb, lub);
            Gizmos.DrawLine(rbb, rub);
            Gizmos.DrawLine(lbf, luf);
            Gizmos.DrawLine(rbf, ruf);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a local cube.
        /// </summary>
        /// <param name='transform'>
        /// 	- The transform the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The local size of the cube.
        /// </param>
        /// <param name='center'>
        ///		- The local position of the cube.
        /// </param>	
        public static void DrawLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3))
        {
            DrawLocalCube(transform, size, Color.white, center);
        }

        /// <summary>
        /// https://math.stackexchange.com/questions/1472049/check-if-a-point-is-inside-a-rectangular-shaped-area-3d
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
        /// 
        /// </summary>
        public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3))
        {
#if UNITY_EDITOR
            Vector3 p1 = space.MultiplyPoint3x4(center + ((-size) * 0.5f));
            Vector3 p2 = space.MultiplyPoint3x4(center + (new Vector3(-size.x, -size.y, size.z) * 0.5f));
            Vector3 p3 = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, size.z) * 0.5f));
            Vector3 p4 = space.MultiplyPoint3x4(center + (new Vector3(size.x, -size.y, -size.z) * 0.5f));


            Vector3 p5 = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, -size.z) * 0.5f));
            Vector3 p6 = space.MultiplyPoint3x4(center + (new Vector3(-size.x, size.y, size.z) * 0.5f));
            Vector3 p7 = space.MultiplyPoint3x4(center + ((size) * 0.5f));
            Vector3 p8 = space.MultiplyPoint3x4(center + (new Vector3(size.x, size.y, -size.z) * 0.5f));


            DrawLocalCube(p1, p2, p3, p4, p5, p6, p7, p8, color);
#endif
        }

#if UNITY_EDITOR

        ///      6 ------------ 7            . ------------ .
        ///    / |            / |          / |    5       / |
        ///  5 ------------ 8   |        . ------------ .   |
        ///  |   |          |   |        |   |          |   |
        ///  |   |          |   |        | 4 |     3    | 2 |
        ///  |   |          |   |        |   |   1      |   |
        ///  |  2 ----------|-- 3        |  . ----------|-- .
        ///  |/             | /          |/       6     | /
        ///  1 ------------ 4            . ------------ .
        ///  
        public static void DrawLocalCube(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5, Vector3 p6, Vector3 p7, Vector3 p8, Color color, bool drawFaces = false, bool drawPoints = false)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;


            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);

            Gizmos.DrawLine(p5, p8);
            Gizmos.DrawLine(p8, p7);
            Gizmos.DrawLine(p7, p6);
            Gizmos.DrawLine(p6, p5);

            Gizmos.DrawLine(p1, p5);
            Gizmos.DrawLine(p4, p8);
            Gizmos.DrawLine(p3, p7);
            Gizmos.DrawLine(p2, p6);

            if (drawFaces)
            {
                Handles.Label(GetMeanOfXPoints(p1, p5, p8, p4), "1");
                Handles.Label(GetMeanOfXPoints(p4, p8, p7, p3), "2");
                Handles.Label(GetMeanOfXPoints(p5, p6, p7, p8), "3");
                Handles.Label(GetMeanOfXPoints(p2, p1, p4, p3), "4");
                Handles.Label(GetMeanOfXPoints(p2, p6, p5, p1), "5");
                Handles.Label(GetMeanOfXPoints(p3, p7, p6, p2), "6");
            }

            if (drawPoints)
            {
                Handles.Label(p1 + Vector3.down * 0.03f + Vector3.right * 0.03f, "1");
                Handles.Label(p2 + Vector3.down * 0.03f + Vector3.right * 0.03f, "2");
                Handles.Label(p3 + Vector3.down * 0.03f + Vector3.right * 0.03f, "3");
                Handles.Label(p4 + Vector3.down * 0.03f + Vector3.right * 0.03f, "4");

                Handles.Label(p5 + Vector3.down * 0.03f + Vector3.right * 0.03f, "5");
                Handles.Label(p6 + Vector3.down * 0.03f + Vector3.right * 0.03f, "6");
                Handles.Label(p7 + Vector3.down * 0.03f + Vector3.right * 0.03f, "7");
                Handles.Label(p8 + Vector3.down * 0.03f + Vector3.right * 0.03f, "8");
            }

            Gizmos.color = oldColor;
        }


        ///
        ///     2 ------------- 3 
        ///   /       1       /   
        ///  1 ------------ 4     

        public static void DrawLocalPlane3d(ExtPlane3d plane3d, Color color, bool drawFaces = false, bool drawPoints = false, bool drawNormal = false)
        {
            //DrawLocalQuad(quad.P1, quad.P2, quad.P3, quad.P4, color, drawFaces, drawPoints);
            Debug.DrawRay(plane3d.Position, plane3d.Normal, color);
            if (drawNormal && plane3d.AllowBottom)
            {
                Debug.DrawRay(plane3d.Position, -plane3d.Normal, color);
            }
        }

        ///
        ///     2 ------------- 3 
        ///   /       1       /   
        ///  1 ------------ 4     

        public static void DrawLocalQuad(ExtQuad quad, Color color, bool drawFaces = false, bool drawPoints = false, bool drawNormal = false)
        {
            DrawLocalQuad(quad.P1, quad.P2, quad.P3, quad.P4, color, drawFaces, drawPoints);
            Debug.DrawRay(quad.Position, quad.Normal, color);
            if (drawNormal && quad.AllowBottom)
            {
                Debug.DrawRay(quad.Position, -quad.Normal, color);
            }
        }

        ///
        ///     2 ------------- 3 
        ///   /       1       /   
        ///  1 ------------ 4     
        public static void DrawLocalQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color, bool drawFaces = false, bool drawPoints = false)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;



            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);

            if (drawFaces)
            {
                Handles.Label(GetMeanOfXPoints(p1, p2, p3, p4), "1");
            }

            if (drawPoints)
            {
                Handles.Label(p1 + Vector3.down * 0.03f + Vector3.right * 0.03f, "1");
                Handles.Label(p2 + Vector3.down * 0.03f + Vector3.right * 0.03f, "2");
                Handles.Label(p3 + Vector3.down * 0.03f + Vector3.right * 0.03f, "3");
                Handles.Label(p4 + Vector3.down * 0.03f + Vector3.right * 0.03f, "4");
            }

            Gizmos.color = oldColor;
        }
#endif


        /// <summary>
        /// 	- Draws a local cube.
        /// </summary>
        /// <param name='space'>
        /// 	- The space the cube will be local to.
        /// </param>
        /// <param name='size'>
        /// 	- The local size of the cube.
        /// </param>
        /// <param name='center'>
        /// 	- The local position of the cube.
        /// </param>
        public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3))
        {
            DrawLocalCube(space, size, Color.white, center);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 	- Draws a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='up'>
        /// 	- The direction perpendicular to the surface of the circle.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1.0f, bool displayNormal = false, string index = "1")
        {
            up = ((up == Vector3.zero) ? Vector3.up : up).normalized * radius;
            Vector3 _forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 _right = Vector3.Cross(up, _forward).normalized * radius;

            Handles.Label(position + Vector3.down * 0.03f + Vector3.right * 0.03f, index);
            if (displayNormal)
            {
                Debug.DrawRay(position, up, color);
            }

            Matrix4x4 matrix = new Matrix4x4();

            matrix[0] = _right.x;
            matrix[1] = _right.y;
            matrix[2] = _right.z;

            matrix[4] = up.x;
            matrix[5] = up.y;
            matrix[6] = up.z;

            matrix[8] = _forward.x;
            matrix[9] = _forward.y;
            matrix[10] = _forward.z;

            Vector3 _lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            Vector3 _nextPoint = Vector3.zero;

            for (var i = 0; i < 91; i++)
            {
                _nextPoint.x = Mathf.Cos((i * 4) * Mathf.Deg2Rad);
                _nextPoint.z = Mathf.Sin((i * 4) * Mathf.Deg2Rad);
                _nextPoint.y = 0;

                _nextPoint = position + matrix.MultiplyPoint3x4(_nextPoint);

                Debug.DrawLine(_lastPoint, _nextPoint, (color == default(Color)) ? Color.white : color);
                _lastPoint = _nextPoint;
            }

        }

        public static void DrawCircle(ExtCircle circle, Color color, float radius = 1.0f, bool displayNormal = false, string index = "1")
        {
            DrawCircle(circle.Point, circle.Normal, color, radius, displayNormal, index);
            if (displayNormal && circle.AllowBottom)
            {
                Debug.DrawRay(circle.Point, -circle.Normal, color);
            }
        }


        /// <summary>
        /// 	- Draws a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        public static void DrawCircle(Vector3 position, Color color, float radius = 1.0f)
        {
            DrawCircle(position, Vector3.up, color, radius);
        }

        /// <summary>
        /// 	- Draws a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='up'>
        /// 	- The direction perpendicular to the surface of the circle.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        public static void DrawCircle(Vector3 position, Vector3 up, float radius = 1.0f)
        {
            DrawCircle(position, position, Color.white, radius);
        }

        /// <summary>
        /// 	- Draws a circle.
        /// </summary>
        /// <param name='position'>
        /// 	- Where the center of the circle will be positioned.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the circle.
        /// </param>
        public static void DrawCircle(Vector3 position, float radius = 1.0f)
        {
            DrawCircle(position, Vector3.up, Color.white, radius);
        }


        //Wiresphere already exists

        /// <summary>
        /// 	- Draws a cylinder.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the cylinder.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the cylinder.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cylinder.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the cylinder.
        /// </param>
        public static void DrawCylinder(Vector3 start, Vector3 end, Color color, float radius = 1.0f)
        {
            Vector3 up = (end - start).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            Handles.Label(start + Vector3.down * 0.03f + Vector3.right * 0.03f, "1");
            Handles.Label(end + Vector3.down * 0.03f + Vector3.right * 0.03f, "2");

            //Radial circles
            ExtDrawGuizmos.DrawCircle(start, up, color, radius);
            ExtDrawGuizmos.DrawCircle(end, -up, color, radius);
            ExtDrawGuizmos.DrawCircle((start + end) * 0.5f, up, color, radius);

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            //Side lines
            Gizmos.DrawLine(start + right, end + right);
            Gizmos.DrawLine(start - right, end - right);

            Gizmos.DrawLine(start + forward, end + forward);
            Gizmos.DrawLine(start - forward, end - forward);

            //Start endcap
            Gizmos.DrawLine(start - right, start + right);
            Gizmos.DrawLine(start - forward, start + forward);

            //End endcap
            Gizmos.DrawLine(end - right, end + right);
            Gizmos.DrawLine(end - forward, end + forward);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a cylinder.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the cylinder.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the cylinder.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the cylinder.
        /// </param>
        public static void DrawCylinder(Vector3 start, Vector3 end, float radius = 1.0f)
        {
            DrawCylinder(start, end, Color.white, radius);
        }

        /// <summary>
        /// 	- Draws a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction for the cone to get wider in.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        public static void DrawCone(Vector3 position, Vector3 direction, Color color, float angle = 45)
        {
            float length = direction.magnitude;

            Vector3 _forward = direction;
            Vector3 _up = Vector3.Slerp(_forward, -_forward, 0.5f);
            Vector3 _right = Vector3.Cross(_forward, _up).normalized * length;

            direction = direction.normalized;

            Vector3 slerpedVector = Vector3.Slerp(_forward, _up, angle / 90.0f);

            float dist;
            var farPlane = new Plane(-direction, position + _forward);
            var distRay = new Ray(position, slerpedVector);

            farPlane.Raycast(distRay, out dist);

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawRay(position, slerpedVector.normalized * dist);
            Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_up, angle / 90.0f).normalized * dist);
            Gizmos.DrawRay(position, Vector3.Slerp(_forward, _right, angle / 90.0f).normalized * dist);
            Gizmos.DrawRay(position, Vector3.Slerp(_forward, -_right, angle / 90.0f).normalized * dist);

            ExtDrawGuizmos.DrawCircle(position + _forward, direction, color, (_forward - (slerpedVector.normalized * dist)).magnitude);
            ExtDrawGuizmos.DrawCircle(position + (_forward * 0.5f), direction, color, ((_forward * 0.5f) - (slerpedVector.normalized * (dist * 0.5f))).magnitude);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction for the cone to get wider in.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        public static void DrawCone(Vector3 position, Vector3 direction, float angle = 45)
        {
            DrawCone(position, direction, Color.white, angle);
        }

        /// <summary>
        /// 	- Draws a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        public static void DrawCone(Vector3 position, Color color, float angle = 45)
        {
            DrawCone(position, Vector3.up, color, angle);
        }

        /// <summary>
        /// 	- Draws a cone.
        /// </summary>
        /// <param name='position'>
        /// 	- The position for the tip of the cone.
        /// </param>
        /// <param name='angle'>
        /// 	- The angle of the cone.
        /// </param>
        public static void DrawCone(Vector3 position, float angle = 45)
        {
            DrawCone(position, Vector3.up, Color.white, angle);
        }

        /// <summary>
        /// 	- Draws an arrow.
        /// </summary>
        /// <param name='position'>
        /// 	- The start position of the arrow.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction the arrow will point in.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the arrow.
        /// </param>
        public static void DrawArrow(Vector3 position, Vector3 direction, Color color)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            Gizmos.DrawRay(position, direction);
            ExtDrawGuizmos.DrawCone(position + direction, -direction * 0.333f, color, 15);

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws an arrow.
        /// </summary>
        /// <param name='position'>
        /// 	- The start position of the arrow.
        /// </param>
        /// <param name='direction'>
        /// 	- The direction the arrow will point in.
        /// </param>
        public static void DrawArrow(Vector3 position, Vector3 direction)
        {
            DrawArrow(position, direction, Color.white);
        }

        /// <summary>
        /// 	- Draws a capsule.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the capsule.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the capsule.
        /// </param>
        /// <param name='color'>
        /// 	- The color of the capsule.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the capsule.
        /// </param>
        public static void DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1)
        {
            Vector3 up = (end - start).normalized * radius;
            Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
            Vector3 right = Vector3.Cross(up, forward).normalized * radius;

            Color oldColor = Gizmos.color;
            Gizmos.color = color;

            float height = (start - end).magnitude;
            float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
            Vector3 middle = (end + start) * 0.5f;

            start = middle + ((start - middle).normalized * sideLength);
            end = middle + ((end - middle).normalized * sideLength);

            //Radial circles
            ExtDrawGuizmos.DrawCircle(start, up, color, radius);
            ExtDrawGuizmos.DrawCircle(end, -up, color, radius);

            //Side lines
            Gizmos.DrawLine(start + right, end + right);
            Gizmos.DrawLine(start - right, end - right);

            Gizmos.DrawLine(start + forward, end + forward);
            Gizmos.DrawLine(start - forward, end - forward);

            for (int i = 1; i < 26; i++)
            {

                //Start endcap
                Gizmos.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start);
                Gizmos.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start);
                Gizmos.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start);
                Gizmos.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start);

                //End endcap
                Gizmos.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end);
                Gizmos.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end);
                Gizmos.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end);
                Gizmos.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end);
            }

            Gizmos.color = oldColor;
        }

        /// <summary>
        /// 	- Draws a capsule.
        /// </summary>
        /// <param name='start'>
        /// 	- The position of one end of the capsule.
        /// </param>
        /// <param name='end'>
        /// 	- The position of the other end of the capsule.
        /// </param>
        /// <param name='radius'>
        /// 	- The radius of the capsule.
        /// </param>
        public static void DrawCapsule(Vector3 start, Vector3 end, float radius = 1)
        {
            DrawCapsule(start, end, Color.white, radius);
        }

        public static void DrawCapsule(Vector3 position, Quaternion rotation, float radius, float height, int direction)
        {
            Debug.LogWarning("no timplemented");
        }
#endif
        #endregion

        #region DebugFunctions

        /// <summary>
        /// 	- Gets the methods of an object.
        /// </summary>
        /// <returns>
        /// 	- A list of methods accessible from this object.
        /// </returns>
        /// <param name='obj'>
        /// 	- The object to get the methods of.
        /// </param>
        /// <param name='includeInfo'>
        /// 	- Whether or not to include each method's method info in the list.
        /// </param>
        public static string MethodsOfObject(System.Object obj, bool includeInfo = false)
        {
            string methods = "";
            MethodInfo[] methodInfos = obj.GetType().GetMethods();
            for (int i = 0; i < methodInfos.Length; i++)
            {
                if (includeInfo)
                {
                    methods += methodInfos[i] + "\n";
                }

                else
                {
                    methods += methodInfos[i].Name + "\n";
                }
            }

            return (methods);
        }

        /// <summary>
        /// 	- Gets the methods of a type.
        /// </summary>
        /// <returns>
        /// 	- A list of methods accessible from this type.
        /// </returns>
        /// <param name='type'>
        /// 	- The type to get the methods of.
        /// </param>
        /// <param name='includeInfo'>
        /// 	- Whether or not to include each method's method info in the list.
        /// </param>
        public static string MethodsOfType(System.Type type, bool includeInfo = false)
        {
            string methods = "";
            MethodInfo[] methodInfos = type.GetMethods();
            for (var i = 0; i < methodInfos.Length; i++)
            {
                if (includeInfo)
                {
                    methods += methodInfos[i] + "\n";
                }

                else
                {
                    methods += methodInfos[i].Name + "\n";
                }
            }

            return (methods);
        }

        #endregion

        #region vector extension
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
        #endregion

        #region GL draw
#if UNITY_EDITOR
        public static void DrawHandleCaps(Matrix4x4 matrix, IList<Vector3> positions, float size)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            Vector3 sideways = (Camera.current == null ? Vector3.right : Camera.current.transform.right) * size;
            Vector3 up = (Camera.current == null ? Vector3.up : Camera.current.transform.up) * size;
            Color col = Handles.color * new Color(1, 1, 1, 0.99f);

            // After drawing the first dot cap, the handle material and matrix are set up, so there's no need to keep
            // resetting the state.
            Handles.DotHandleCap(0, matrix.MultiplyPoint(positions[0]), Quaternion.identity,
                        HandleUtility.GetHandleSize(matrix.MultiplyPoint(positions[0])) * .05f, EventType.Repaint);

            GL.Begin(GL.QUADS);
            for (int i = 1, c = positions.Count; i < c; i++)
            {
                var position = matrix.MultiplyPoint(positions[i]);

                GL.Color(col);
                GL.Vertex(position + sideways + up);
                GL.Vertex(position + sideways - up);
                GL.Vertex(position - sideways - up);
                GL.Vertex(position - sideways + up);
            }
            GL.End();
        }

        public static void DrawArrowCap(Vector3 position, Quaternion rotation, float size)
        {
            Handles.ArrowHandleCap(
                0,
                position,
                rotation,
                size,
                EventType.Repaint
            );
        }
#endif
        #endregion
    }
}