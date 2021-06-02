using UnityEngine;

/// <summary>
/// a perfect quad OR triangle (3 points)
/// </summary>
public struct ExtTriangleOrQuad
{
    public bool unidirectionnal;
    public bool inverseDirection;
    public bool noGravityBorders;
    public bool isQuad;

    public ExtQuad quad;
    public ExtTriangle triangle;

    public ExtTriangleOrQuad(Vector3 a, Vector3 b, Vector3 c,
        bool _unidirectionnal, bool _inverseDirection,
        bool _noGravityBorders, bool _calculateAB, bool _calculateBC, bool _calculateCA, bool _calculateCorner, bool _isQuad)
    {
        unidirectionnal = _unidirectionnal;
        inverseDirection = _inverseDirection;
        noGravityBorders = _noGravityBorders;
        isQuad = _isQuad;

        quad = new ExtQuad(a, b, c, _unidirectionnal, _inverseDirection, _noGravityBorders);
        triangle = new ExtTriangle(a, b, c, _unidirectionnal, _inverseDirection, _noGravityBorders, _calculateAB, _calculateBC, _calculateCA, _calculateCorner);
    }

    public Vector3 GetNormal()
    {
        if (isQuad)
            return (quad.TriNormNormalize);
        return (triangle.TriNormNormalize);
    }

    public Vector3 ClosestPointTo(Vector3 p)
    {
        if (isQuad)
            return (quad.ClosestPtPointRect(p));
        return (triangle.ClosestPointTo(p));
    }
}

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
        //Vector3 projectedOnPlane = TriPlane.Project(EdgeAb.A, TriNorm.normalized, p);
        Vector3 dirPlayer = p - foundPosition;

        float dotPlanePlayer = Vector3.Dot(dirPlayer.normalized, TriNormNormalize);
        if ((dotPlanePlayer < 0 && !inverseDirection) || dotPlanePlayer > 0 && inverseDirection)
        {
            return (foundPosition);
        }
        else
        {
            //Debug.DrawRay(p, dirPlayer, Color.yellow, 5f);
            //Debug.DrawRay(p, TriNorm.normalized, Color.black, 5f);
            return (ExtUtilityFunction.GetNullVector());
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

            if (noGravityBorders && (!calculateCorner || (calculateCorner && !calculateAB && !calculateCA) ))
                return (ExtUtilityFunction.GetNullVector());
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
                return (ExtUtilityFunction.GetNullVector());

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
                return (ExtUtilityFunction.GetNullVector());

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
                return (ExtUtilityFunction.GetNullVector());

            
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
                return (ExtUtilityFunction.GetNullVector());

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
                return (ExtUtilityFunction.GetNullVector());

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

/// <summary>
/// a perfect 3D Quad, with 3 points
/// </summary>
public struct ExtQuad
{
    public readonly Vector3 A;
    public readonly Vector3 B;
    public readonly Vector3 C;
    private Vector3 AB;
    private Vector3 AC;
    private float maxdistA;
    private float maxdistC;

    public bool unidirectionnal;
    public bool inverseDirection;
    public bool noGravityBorders;
    
    private readonly Vector3 TriNorm;
    public readonly Vector3 TriNormNormalize;

    public ExtQuad(Vector3 a, Vector3 b, Vector3 c,
        bool _unidirectionnal, bool _inverseDirection,
        bool _noGravityBorders)
    {
        A = a;
        B = b;
        C = c;

        AB = B - A;
        AC = C - A;
        maxdistA = Vector3.Dot(AB, AB);
        maxdistC = Vector3.Dot(AC, AC);

        unidirectionnal = _unidirectionnal;
        inverseDirection = _inverseDirection;
        noGravityBorders = _noGravityBorders;
        
        TriNorm = Vector3.Cross(a - b, a - c);
        TriNormNormalize = TriNorm.normalized;
    }

    private Vector3 GetGoodPointUnidirectionnal(Vector3 p, Vector3 foundPosition)
    {
        //Vector3 projectedOnPlane = TriPlane.Project(EdgeAb.A, TriNorm.normalized, p);
        Vector3 dirPlayer = p - foundPosition;

        float dotPlanePlayer = Vector3.Dot(dirPlayer.normalized, TriNormNormalize);
        if ((dotPlanePlayer < 0 && !inverseDirection) || dotPlanePlayer > 0 && inverseDirection)
        {
            return (foundPosition);
        }
        else
        {
            //Debug.DrawRay(p, dirPlayer, Color.yellow, 5f);
            //Debug.DrawRay(p, TriNorm.normalized, Color.black, 5f);
            return (ExtUtilityFunction.GetNullVector());
        }
    }

    // Return point q on (or in) rect (specified by a, b, and c), closest to given point p
    public Vector3 ClosestPtPointRect(Vector3 p)
    {
        Vector3 d = p - A;
        // Start result at top-left corner of rect; make steps from there
        Vector3 q = A;
        // Clamp p’ (projection of p to plane of r) to rectangle in the across direction
        float dist = Vector3.Dot(d, AB);
        //float maxdist = Vector3.Dot(ab, ab);
        if (dist >= maxdistA)
            q += AB;
        else if (dist > 0.0f)
            q += (dist / maxdistA) * AB;
        // Clamp p' (projection of p to plane of r) to rectangle in the down direction
        dist = Vector3.Dot(d, AC);

        if (dist >= maxdistC)
            q += AC;
        else if (dist > 0.0f)
            q += (dist / maxdistC) * AC;

        //if (unidirectionnal)
        //    return (GetGoodPointUnidirectionnal(p, q));

        return (q);
    }
}

/// <summary>
/// a 3D Tetra: 4 points linked together
/// </summary>
public struct ExtTetra
{
    public readonly Vector3 A;
    public readonly Vector3 B;
    public readonly Vector3 C;
    public readonly Vector3 D;

    private Vector3 BA;
    private Vector3 CA;
    private Vector3 DA;

    private Vector3 crossBACA;

    private ExtTriangle triangleA;
    private ExtTriangle triangleB;
    private ExtTriangle triangleC;
    private ExtTriangle triangleD;

    public bool unidirectionnal;
    public bool inverseDirection;
    public bool noGravityBorders;
    public bool precise;

    private readonly Vector3 TriNorm;
    public readonly Vector3 TriNormNormalize;

    public ExtTetra(Vector3 a, Vector3 b, Vector3 c, Vector3 d,
        bool _unidirectionnal, bool _inverseDirection,
        bool _noGravityBorders, bool _precise)
    {
        A = a;
        B = b;
        C = c;
        D = d;

        BA = B - A;
        CA = C - A;
        DA = D - A;

        crossBACA = Vector3.Cross(B - A, C - A);

        TriNorm = ExtQuaternion.GetMiddleOf2Vector(Vector3.Cross(a - b, a - c), Vector3.Cross(a - c, a - d));
        TriNormNormalize = TriNorm.normalized;

        precise = _precise;

        if (precise)
        {
            triangleA = new ExtTriangle(a, b, c, false, false, _noGravityBorders, false, false, true, true);
            triangleB = new ExtTriangle(a, c, d, false, false, _noGravityBorders, true, false, false, true);
            triangleC = new ExtTriangle(a, d, b, false, false, _noGravityBorders, false, true, false, true);
            triangleD = new ExtTriangle(b, d, c, false, false, _noGravityBorders, true, false, false, true);
        }
        else
        {
            triangleA = new ExtTriangle(a, b, c, _unidirectionnal, _inverseDirection, _noGravityBorders, false, false, true, false);
            triangleB = new ExtTriangle(c, d, a, _unidirectionnal, _inverseDirection, _noGravityBorders, false, false, true, false);
            triangleC = new ExtTriangle();
            triangleD = new ExtTriangle();
        }


        unidirectionnal = _unidirectionnal;
        inverseDirection = _inverseDirection;
        noGravityBorders = _noGravityBorders;
    }

    // Test if point p lies outside plane through abc
    bool PointOutsideOfPlane(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        return Vector3.Dot(p - a, Vector3.Cross(b - a, c - a)) >= 0.0f; // [AP AB AC] >= 0
    }

    // Test if point p and d lie on opposite sides of plane through abc
    bool PointOutsideOfPlane(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        float signp = Vector3.Dot(p - a, Vector3.Cross(b - a, c - a)); // [AP AB AC]
        float signd = Vector3.Dot(d - a, Vector3.Cross(b - a, c - a)); // [AD AB AC]
                                                       // Points on opposite sides if expression signs are opposite
        return signp * signd < 0.0f;
    }

    public Vector3 CalculateSmothlyFourPlane(Vector3 p)
    {
        // Start out assuming point inside all halfspaces, so closest to itself
        Vector3 closestPt = p;
        float bestSqDist = float.MaxValue;
        bool insideAll = true;
        // If point outside face abc then compute closest point on abc
        if (PointOutsideOfPlane(p, A, B, C))
        {
            insideAll = false;
            Vector3 q = triangleA.ClosestPointTo(p);
            if (!ExtUtilityFunction.IsNullVector(q))
            {
                float sqDist = Vector3.Dot(q - p, q - p);
                // Update best closest point if (squared) distance is less than current best
                if (sqDist < bestSqDist)
                {
                    bestSqDist = sqDist;
                    closestPt = q;
                }
            }
        }
        // Repeat test for face acd
        if (PointOutsideOfPlane(p, A, C, D))
        {
            insideAll = false;
            Vector3 q = triangleB.ClosestPointTo(p);
            if (!ExtUtilityFunction.IsNullVector(q))
            {
                float sqDist = Vector3.Dot(q - p, q - p);
                if (sqDist < bestSqDist)
                {
                    bestSqDist = sqDist;
                    closestPt = q;
                }
            }
        }

        // Repeat test for face adb
        if (PointOutsideOfPlane(p, A, D, B))
        {
            insideAll = false;
            Vector3 q = triangleC.ClosestPointTo(p);
            if (!ExtUtilityFunction.IsNullVector(q))
            {
                float sqDist = Vector3.Dot(q - p, q - p);
                if (sqDist < bestSqDist)
                {
                    bestSqDist = sqDist;
                    closestPt = q;
                }
            }
        }
        // Repeat test for face bdc
        if (PointOutsideOfPlane(p, B, D, C))
        {
            insideAll = false;
            Vector3 q = triangleD.ClosestPointTo(p);
            if (!ExtUtilityFunction.IsNullVector(q))
            {
                float sqDist = Vector3.Dot(q - p, q - p);
                if (sqDist < bestSqDist)
                {
                    bestSqDist = sqDist;
                    closestPt = q;
                }
            }
        }

        if (unidirectionnal)
            return (GetGoodPointUnidirectionnal(p, closestPt));

        if (noGravityBorders && !insideAll && closestPt == p)
            return (ExtUtilityFunction.GetNullVector());

        return closestPt;
    }

    private Vector3 GetGoodPointUnidirectionnal(Vector3 p, Vector3 foundPosition)
    {
        //Vector3 projectedOnPlane = TriPlane.Project(EdgeAb.A, TriNorm.normalized, p);
        Vector3 dirPlayer = p - foundPosition;

        float dotPlanePlayer = Vector3.Dot(dirPlayer.normalized, TriNormNormalize);
        if ((dotPlanePlayer < 0 && !inverseDirection) || dotPlanePlayer > 0 && inverseDirection)
        {
            return (foundPosition);
        }
        else
        {
            //Debug.DrawRay(p, dirPlayer, Color.yellow, 5f);
            //Debug.DrawRay(p, TriNorm.normalized, Color.black, 5f);
            return (ExtUtilityFunction.GetNullVector());
        }
    }

    public Vector3 CalculateWithTwoTriangle(Vector3 p)
    {
        Vector3 closestToA = triangleA.ClosestPointTo(p);
        Vector3 closestToB = triangleB.ClosestPointTo(p);

        //aditional test when no Gravity Border
        if (noGravityBorders)
        {
            //if A is nul and not B. return null IF B is from the middle !
            if (ExtUtilityFunction.IsNullVector(closestToA) && !ExtUtilityFunction.IsNullVector(closestToB))
            {
                if (triangleB.GetLastType() == ExtTriangle.LastType.CA)
                    return (ExtUtilityFunction.GetNullVector());
            }
            //if B is nul and not A. return null IF A is from the middle !
            if (ExtUtilityFunction.IsNullVector(closestToB) && !ExtUtilityFunction.IsNullVector(closestToA))
            {
                if (triangleA.GetLastType() == ExtTriangle.LastType.CA)
                    return (ExtUtilityFunction.GetNullVector());
            }
        }
        if (unidirectionnal)
        {
            //if A is nul and not B. return null IF B is from the middle !
            if (ExtUtilityFunction.IsNullVector(closestToA) && !ExtUtilityFunction.IsNullVector(closestToB))
            {
                if (triangleB.GetLastType() == ExtTriangle.LastType.CA || triangleB.GetLastType() == ExtTriangle.LastType.C || triangleB.GetLastType() == ExtTriangle.LastType.A)
                    return (ExtUtilityFunction.GetNullVector());
            }
            //if B is nul and not A. return null IF A is from the middle !
            if (ExtUtilityFunction.IsNullVector(closestToB) && !ExtUtilityFunction.IsNullVector(closestToA))
            {
                if (triangleA.GetLastType() == ExtTriangle.LastType.CA || triangleA.GetLastType() == ExtTriangle.LastType.C || triangleA.GetLastType() == ExtTriangle.LastType.A)
                    return (ExtUtilityFunction.GetNullVector());
            }
        }

        int indexFound = -1;
        return (ExtUtilityFunction.GetClosestPoint(p, new Vector3[] { closestToA, closestToB }, ref indexFound));
    }

    // Return point q on (or in) rect (specified by a, b, and c), closest to given point p
    public Vector3 ClosestPtPointRect(Vector3 p)
    {
        if (precise)
            return (CalculateSmothlyFourPlane(p));
        return (CalculateWithTwoTriangle(p));
    }
}

/// <summary>
/// a 3D line, with 2 points
/// </summary>
public struct ExtLine
{
    public readonly Vector3 A;
    public readonly Vector3 B;
    public readonly Vector3 Delta;
    public bool noGravityBorders;
    private float dist;
    private float denom;

    public ExtLine(Vector3 a, Vector3 b, bool _noGravityBorders)
    {
        A = a;
        B = b;
        Delta = b - a;
        noGravityBorders = _noGravityBorders;

        dist = 0;
        denom = Vector3.Dot(Delta, Delta);
    }

    //public Vector3 PointAt(double t) => A + (float)t * Delta;
    //public double LengthSquared => Delta.sqrMagnitude;
    //public double LengthLine => Delta.magnitude;

    /*
    public Vector3 ClosestPointTo(Vector3 p)
    {
        //The normalized "distance" from a to your closest point 
        double distance = Project(p); 

        if (distance < 0)     //Check if P projection is over vectorAB     
        {
            if (noGravityBorders)
                return (ExtUtilityFunction.GetNullVector());
            return A;
        }
        else if (distance > 1)
        {
            if (noGravityBorders)
                return (ExtUtilityFunction.GetNullVector());
            return B;
        }
        else
        {
            return A + Delta * (float)distance;
        }
    }
    */
    public Vector3 ClosestPointTo(Vector3 c)
    {
        // Project c onto ab, but deferring divide by Dot(ab, ab)
        dist = Vector3.Dot(c - A, Delta);
        if (dist <= 0.0f)
        {
            // c projects outside the [a,b] interval, on the a side; clamp to a
            dist = 0.0f;
            if (noGravityBorders)
                return (ExtUtilityFunction.GetNullVector());
            return (A);
        }
        else
        {
            //denom = Vector3.Dot(Delta, Delta); // Always nonnegative since denom = ||ab||∧2
            if (dist >= denom)
            {
                // c projects outside the [a,b] interval, on the b side; clamp to b
                dist = 1.0f;
                if (noGravityBorders)
                    return (ExtUtilityFunction.GetNullVector());
                return (B);
            }
            else
            {
                // c projects inside the [a,b] interval; must do deferred divide now
                dist = dist / denom;
                return (A + dist * Delta);
            }
        }
    }

    // Returns the squared distance between point c and segment ab
    float SqDistPointSegment(Vector3 c)
    {
        //Vector3 ab = b – a;
        Vector3 ac = c - A;
        Vector3 bc = c - B;
        float e = Vector3.Dot(ac, Delta);
        // Handle cases where c projects outside ab
        if (e <= 0.0f) return Vector3.Dot(ac, ac);
        if (e >= denom) return Vector3.Dot(bc, bc);
        // Handle cases where c projects onto ab
        return Vector3.Dot(ac, ac) - e* e / denom;
    }

    /*
    public double GetLenght()
    {
        return (LengthLine);
    }

    public double Project(Vector3 p) => Vector3.Dot(Delta, p - A) / LengthSquared;
    */
}

/// <summary>
/// a 3D plane
/// </summary>
public struct ExtPlane
{
    public Vector3 Point;
    public Vector3 Direction;

    public ExtPlane(Vector3 point, Vector3 direction)
    {
        Point = point;
        Direction = direction;
    }

    public bool IsAbove(Vector3 q) => Vector3.Dot(q - Point, Direction) > 0;

    
    public Vector3 Project(Vector3 randomPointInPlane, Vector3 normalPlane, Vector3 pointToProject)
    {
        //Vector3 v = pointToProject - randomPointInPlane;
        //Vector3 d = Vector3.Project(v, normalPlane.normalized);        
        //Vector3 projectedPoint = pointToProject - d;
        //return (projectedPoint);
        return (pointToProject - (Vector3.Project(pointToProject - randomPointInPlane, normalPlane.normalized)));
    }
}