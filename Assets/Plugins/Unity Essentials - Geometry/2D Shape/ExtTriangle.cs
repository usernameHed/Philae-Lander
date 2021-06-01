using UnityEngine;

namespace UnityEssentials.Geometry.shape2d
{
    /// <summary>
    /// a 3D triangle
    /// </summary>
    public struct ExtTriangle
    {
        public readonly Vector3 A;
        public readonly Vector3 B;
        public readonly Vector3 C;
        private Vector3 AB;
        private Vector3 AC;

        public bool unidirectionnal;
        public bool inverseDirection;
        public bool noGravityBorders;

        private readonly Vector3 TriNorm;
        public readonly Vector3 TriNormNormalize;

        private bool calculateAB;
        private bool calculateBC;
        private bool calculateCA;
        private bool calculateCorner;
        private LastType lastType;
        public LastType GetLastType() => lastType;

        public enum LastType
        {
            NONE,
            A,
            B,
            C,
            AB,
            BC,
            CA,
            ABC,
        }

        public ExtTriangle(Vector3 a, Vector3 b, Vector3 c,
            bool _unidirectionnal, bool _inverseDirection,
            bool _noGravityBorders, bool _calculateAB, bool _calculateBC, bool _calculateCA, bool _calculateCorner)
        {
            A = a;
            B = b;
            C = c;

            AB = B - A;
            AC = C - A;

            unidirectionnal = _unidirectionnal;
            inverseDirection = _inverseDirection;
            noGravityBorders = _noGravityBorders;

            TriNorm = Vector3.Cross(a - b, a - c);
            TriNormNormalize = TriNorm.normalized;
            calculateAB = _calculateAB;
            calculateBC = _calculateBC;
            calculateCA = _calculateCA;
            calculateCorner = _calculateCorner;

            lastType = LastType.NONE;
        }

        private Vector3 GetGoodPointUnidirectionnal(Vector3 p, Vector3 foundPosition)
        {
            Vector3 dirPlayer = p - foundPosition;

            float dotPlanePlayer = Vector3.Dot(dirPlayer.normalized, TriNormNormalize);
            if ((dotPlanePlayer < 0 && !inverseDirection) || dotPlanePlayer > 0 && inverseDirection)
            {
                return (foundPosition);
            }
            else
            {
                return (Vector3.zero);
            }
        }

        public Vector3 ClosestPointTo(Vector3 p)
        {
            // Check if P in vertex region outside A
            Vector3 ap = p - A;
            lastType = LastType.NONE;
            float d1 = Vector3.Dot(AB, ap);
            float d2 = Vector3.Dot(AC, ap);
            if (d1 <= 0.0f && d2 <= 0.0f)
            {
                lastType = LastType.A;

                if (noGravityBorders && (!calculateCorner || (calculateCorner && !calculateAB && !calculateCA)))
                    return (Vector3.zero);
                if (unidirectionnal)
                    return (GetGoodPointUnidirectionnal(p, A));


                return A; // barycentric coordinates (1,0,0)
            }

            // Check if P in vertex region outside B
            Vector3 bp = p - B;
            float d3 = Vector3.Dot(AB, bp);
            float d4 = Vector3.Dot(AC, bp);
            if (d3 >= 0.0f && d4 <= d3)
            {
                lastType = LastType.B;

                if (noGravityBorders && (!calculateCorner || (calculateCorner && !calculateAB && !calculateBC)))
                    return (Vector3.zero);

                if (unidirectionnal)
                    return (GetGoodPointUnidirectionnal(p, B));


                return B; // barycentric coordinates (0,1,0)
            }

            // Check if P in edge region of AB, if so return projection of P onto AB
            float vc = d1 * d4 - d3 * d2;
            if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
            {
                lastType = LastType.AB;

                if (noGravityBorders && !calculateAB)
                    return (Vector3.zero);

                float v1 = d1 / (d1 - d3);

                if (unidirectionnal)
                    return (GetGoodPointUnidirectionnal(p, A + v1 * AB));


                return A + v1 * AB; // barycentric coordinates (1-v,v,0)
            }

            // Check if P in vertex region outside C
            Vector3 cp = p - C;
            float d5 = Vector3.Dot(AB, cp);
            float d6 = Vector3.Dot(AC, cp);
            if (d6 >= 0.0f && d5 <= d6)
            {
                lastType = LastType.C;

                if (noGravityBorders && (!calculateCorner || (calculateCorner && !calculateBC && !calculateCA)))
                    return (Vector3.zero);


                if (unidirectionnal)
                    return (GetGoodPointUnidirectionnal(p, C));


                return C; // barycentric coordinates (0,0,1)
            }

            // Check if P in edge region of AC, if so return projection of P onto AC
            float vb = d5 * d2 - d1 * d6;
            if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
            {
                lastType = LastType.CA;

                if (noGravityBorders && !calculateCA)
                    return (Vector3.zero);

                float w1 = d2 / (d2 - d6);


                if (unidirectionnal)
                    return (GetGoodPointUnidirectionnal(p, A + w1 * AC));


                return A + w1 * AC; // barycentric coordinates (1-w,0,w)
            }
            // Check if P in edge region of BC, if so return projection of P onto BC
            float va = d3 * d6 - d5 * d4;
            if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
            {
                lastType = LastType.BC;

                if (noGravityBorders && !calculateBC)
                    return (Vector3.zero);

                float w2 = (d4 - d3) / ((d4 - d3) + (d5 - d6));


                if (unidirectionnal)
                    return (GetGoodPointUnidirectionnal(p, B + w2 * (C - B)));

                return B + w2 * (C - B); // barycentric coordinates (0,1-w,w)
            }
            // P inside face region. Compute Q through its barycentric coordinates (u,v,w)
            lastType = LastType.ABC;

            float denom = 1.0f / (va + vb + vc);
            float v = vb * denom;
            float w = vc * denom;



            if (unidirectionnal)
                return (GetGoodPointUnidirectionnal(p, A + AB * v + AC * w));

            return A + AB * v + AC * w; // = u*a + v*b + w*c, u = va * denom = 1.0f-v-w
        }
    }
}