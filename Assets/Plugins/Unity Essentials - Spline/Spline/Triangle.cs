using UnityEngine;
using System;

namespace UnityEssentials.Spline
{
    /// <summary>Defines a world-space path, consisting of an array of waypoints,
    /// each of which has position and roll settings.  Bezier interpolation
    /// is performed between the waypoints, to get a smooth and continuous path.
    /// The path will pass through all waypoints, and (unlike Spline) first 
    /// and second order continuity is guaranteed</summary>
    [AddComponentMenu("Unity Essentials/Spline/Spline Type/Triangle")]
    public class Triangle : Spline
    {
        public bool IsInsideShape(Vector3 position)
        {
            Vector3 A = Waypoints[0].Position;
            Vector3 B = Waypoints[1].Position;
            Vector3 C = Waypoints[2].Position;

            return (SameSide(position, A, B, C)
                && SameSide(position, B, A, C)
                && SameSide(position, C, A, B));
        }

        private bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
        {
            Vector3 cp1 = Vector3.Cross(b - a, p1 - a);
            Vector3 cp2 = Vector3.Cross(b - a, p2 - a);

            return (Vector3.Dot(cp1, cp2) >= 0);
        }

        /// <summary>
        /// return the closest point to the triangle
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public Vector3 GetClosestPoint(Vector3 sourcePosition)
        {
            Vector3 A = Waypoints[0].Position;
            Vector3 B = Waypoints[1].Position;
            Vector3 C = Waypoints[2].Position;

            Vector3 edge0 = B - A;
            Vector3 edge1 = C - A;
            Vector3 v0 = A - sourcePosition;

            float a = Vector3.Dot(edge0, edge0);
            float b = Vector3.Dot(edge0, edge1);
            float c = Vector3.Dot(edge1, edge1);
            float d = Vector3.Dot(edge0, v0);
            float e = Vector3.Dot(edge1, v0);

            float det = a * c - b * b;
            float s = b * e - c * d;
            float t = b * d - a * e;

            if (s + t < det)
            {
                if (s < 0f)
                {
                    if (t < 0f)
                    {
                        if (d < 0f)
                        {
                            s = Mathf.Clamp(-d / a, 0f, 1f);
                            t = 0f;
                        }
                        else
                        {
                            s = 0f;
                            t = Mathf.Clamp(-e / c, 0f, 1f);
                        }
                    }
                    else
                    {
                        s = 0f;
                        t = Mathf.Clamp(-e / c, 0f, 1f);
                    }
                }
                else if (t < 0f)
                {
                    s = Mathf.Clamp(-d / a, 0f, 1f);
                    t = 0f;
                }
                else
                {
                    float invDet = 1f / det;
                    s *= invDet;
                    t *= invDet;
                }
            }
            else
            {
                if (s < 0f)
                {
                    float tmp0 = b + d;
                    float tmp1 = c + e;
                    if (tmp1 > tmp0)
                    {
                        float numer = tmp1 - tmp0;
                        float denom = a - 2 * b + c;
                        s = Mathf.Clamp(numer / denom, 0f, 1f);
                        t = 1 - s;
                    }
                    else
                    {
                        t = Mathf.Clamp(-e / c, 0f, 1f);
                        s = 0f;
                    }
                }
                else if (t < 0f)
                {
                    if (a + d > b + e)
                    {
                        float numer = c + e - b - d;
                        float denom = a - 2 * b + c;
                        s = Mathf.Clamp(numer / denom, 0f, 1f);
                        t = 1 - s;
                    }
                    else
                    {
                        s = Mathf.Clamp(-e / c, 0f, 1f);
                        t = 0f;
                    }
                }
                else
                {
                    float numer = c + e - b - d;
                    float denom = a - 2 * b + c;
                    s = Mathf.Clamp(numer / denom, 0f, 1f);
                    t = 1f - s;
                }
            }

            return A + s * edge0 + t * edge1;
        }
    }
}
