// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
    // Representation of four-dimensional vectors.
    public struct Vector7
    {
        // *undocumented*
        public const float kEpsilon = 0.00001F;

        public float a;
        public float b;
        public float c;
        public float d;
        public float e;
        public float f;
        public float g;

        // Access the a, b, c, e, f, g, h components using [0], [1], [2], [3]... respectively.
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return a;
                    case 1: return b;
                    case 2: return c;
                    case 3: return d;
                    case 4: return e;
                    case 5: return f;
                    case 6: return g;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4 index!");
                }
            }

            set
            {
                switch (index)
                {
                    case 0: a = value; break;
                    case 1: b = value; break;
                    case 2: c = value; break;
                    case 3: d = value; break;
                    case 4: e = value; break;
                    case 5: f = value; break;
                    case 6: g = value; break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4 index!");
                }
            }
        }

        // Creates a new vector with given x, y, z, w components.
        public Vector7(float a, float b, float c, float d, float e, float f, float g) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = g; }
        public Vector7(float a, float b, float c, float d, float e, float f) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = 0f; }
        public Vector7(float a, float b, float c, float d, float e) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = 0f; this.g = 0f; }
        public Vector7(float a, float b, float c, float d) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = 0f; this.f = 0f; this.g = 0f; }
        public Vector7(float a, float b, float c) { this.a = a; this.b = b; this.c = c; this.d = 0f; this.e = 0f; this.f = 0f; this.g = 0f; }
        public Vector7(float a, float b) { this.a = a; this.b = b; this.c = 0f; this.d = 0f; this.e = 0f; this.f = 0f; this.g = 0f; }
        public Vector7(Vector3 a, Vector3 b, float g) { this.a = a.x; this.b = a.y; this.c = a.z; this.d = b.x; this.e = b.y; this.f = b.z; this.g = g; }
        public Vector7(Vector3 a, Vector3 b) { this.a = a.x; this.b = a.y; this.c = a.z; this.d = b.x; this.e = b.y; this.f = b.z; this.g = 0f; }

        // Set x, y, z and w components of an existing Vector4.
        public void Set(float newA, float newB, float newC, float newD, float newE, float newF, float newG) { a = newA; b = newB; c = newC; d = newD; e = newE; f = newF; g = newG; }

        // Linearly interpolates between two vectors.
        public static Vector7 Lerp(Vector7 a, Vector7 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector7(
                a.a + (b.a - a.a) * t,
                a.b + (b.b - a.b) * t,
                a.c + (b.c - a.c) * t,
                a.d + (b.d - a.d) * t,
                a.e + (b.e - a.e) * t,
                a.f + (b.f - a.f) * t,
                a.g + (b.g - a.g) * t
            );
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Vector7 LerpUnclamped(Vector7 a, Vector7 b, float t)
        {
            return new Vector7(
                a.a + (b.a - a.a) * t,
                a.b + (b.b - a.b) * t,
                a.c + (b.c - a.c) * t,
                a.d + (b.d - a.d) * t,
                a.e + (b.e - a.e) * t,
                a.f + (b.f - a.f) * t,
                a.g + (b.g - a.g) * t
            );
        }

        // Multiplies two vectors component-wise.
        public static Vector7 Scale(Vector7 a, Vector7 b)
        {
            return new Vector7(a.a * b.a, a.b * b.b, a.c * b.c, a.d * b.d, a.e * b.e, a.f * b.f, a.g * b.g);
        }

        // Multiplies every component of this vector by the same component of /scale/.
        public void Scale(Vector7 scale)
        {
            a *= scale.a;
            b *= scale.b;
            c *= scale.c;
            d *= scale.d;
            e *= scale.e;
            f *= scale.f;
            g *= scale.g;
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() ^ (b.GetHashCode() << 2) ^ (c.GetHashCode() >> 2) ^ (d.GetHashCode() >> 1);
        }

        // also required for being able to use Vector4s as keys in hash tables
        public override bool Equals(object other)
        {
            if (!(other is Vector7)) return false;

            return Equals((Vector7)other);
        }

        public bool Equals(Vector7 other)
        {
            return a == other.a && b == other.b && c == other.c && d == other.d && e == other.e && f == other.f && g == other.g;
        }

        public static Vector7 operator +(Vector7 a, Vector7 b) { return new Vector7(a.a + b.a, a.b + b.b, a.c + b.c, a.d + b.d, a.e + b.e, a.f + b.f, a.g + b.g); }

        public static Vector7 operator -(Vector7 a, Vector7 b) { return new Vector7(a.a - b.a, a.b - b.b, a.c - b.c, a.d - b.d, a.e - b.e, a.f - b.f, a.g - b.g); }
        // Negates a vector.

        public static Vector7 operator -(Vector7 a) { return new Vector7(-a.a, -a.b, -a.c, -a.d, -a.e, -a.f, -a.g); }
        // Multiplies a vector by a number.

        public static Vector7 operator *(Vector7 a, float d) { return new Vector7(a.a * d, a.b * d, a.c * d, a.d * d, a.e * d, a.f * d, a.g * d); }
        // Multiplies a vector by a number.

        public static Vector7 operator *(float d, Vector7 a) { return new Vector7(a.a * d, a.b * d, a.c * d, a.d * d, a.e * d, a.f * d, a.g * d); }
        // Divides a vector by a number.

        public static Vector7 operator /(Vector7 a, float d) { return new Vector7(a.a / d, a.b / d, a.c / d, a.d / d, a.e / d, a.f / d, a.g / d); }

        // Returns true if the vectors are equal.

        public static bool operator ==(Vector7 lhs, Vector7 rhs)
        {
            // Returns false in the presence of NaN values.
            float diffa = lhs.a - rhs.a;
            float diffb = lhs.b - rhs.b;
            float diffc = lhs.c - rhs.c;
            float diffd = lhs.d - rhs.d;
            float diffe = lhs.e - rhs.e;
            float difff = lhs.f - rhs.f;
            float diffg = lhs.g - rhs.g;
            float sqrmag = diffa * diffa + diffb * diffb + diffc * diffc + diffd * diffd + diffe * diffe + difff * difff + diffg * diffg;
            return sqrmag < kEpsilon * kEpsilon;
        }

        // Returns true if vectors are different.
        public static bool operator !=(Vector7 lhs, Vector7 rhs)
        {
            // Returns true in the presence of NaN values.
            return !(lhs == rhs);
        }

        // Converts a [[Vector3]] to a Vector4.
        public static implicit operator Vector7(Vector3 v)
        {
            return new Vector7(v.x, v.y, v.z, 0.0F);
        }

        // Converts a Vector4 to a [[Vector3]].
        public static implicit operator Vector3(Vector7 v)
        {
            return new Vector3(v.a, v.b, v.c);
        }

        // Converts a [[Vector2]] to a Vector4.
        public static implicit operator Vector7(Vector2 v)
        {
            return new Vector7(v.x, v.y, 0.0F, 0.0F);
        }

        // Converts a Vector4 to a [[Vector2]].
        public static implicit operator Vector2(Vector7 v)
        {
            return new Vector2(v.a, v.b);
        }
    }
} // namespace
