using UnityEngine;

/*
public struct ExtTriangle
{
    public Vector3 A => EdgeAb.A;
    public Vector3 B => EdgeBc.A;
    public Vector3 C => EdgeCa.A;

    public readonly ExtLine EdgeAb;
    public readonly ExtLine EdgeBc;
    public readonly ExtLine EdgeCa;

    public bool unidirectionnal;
    public bool inverseDirection;
    public bool infinitePlane;
    public bool noGravityBorders;

    public ExtTriangle(Vector3 a, Vector3 b, Vector3 c,
        bool _unidirectionnal, bool _inverseDirection, bool _infinitePlane,
        bool _noGravityBorders)
    {
        EdgeAb = new ExtLine(a, b, false);
        EdgeBc = new ExtLine(b, c, false);
        EdgeCa = new ExtLine(c, a, false);

        unidirectionnal = _unidirectionnal;
        inverseDirection = _inverseDirection;
        infinitePlane = _infinitePlane;
        noGravityBorders = _noGravityBorders;

        TriNorm = Vector3.Cross(a - b, a - c);
        TriNormNormalize = TriNorm.normalized;
    }

    public Vector3[] Verticies => new[] { A, B, C };

    public readonly Vector3 TriNorm;
    public readonly Vector3 TriNormNormalize;

    //private static readonly RangeDouble ZeroToOne = new RangeDouble(0, 1);

    public ExtPlane TriPlane => new ExtPlane(A, TriNorm);

    // The below three could be pre-calculated to
    // trade off space vs time

    public ExtPlane PlaneAb => new ExtPlane(EdgeAb.A, Vector3.Cross(TriNorm, EdgeAb.Delta));
    public ExtPlane PlaneBc => new ExtPlane(EdgeBc.A, Vector3.Cross(TriNorm, EdgeBc.Delta));
    public ExtPlane PlaneCa => new ExtPlane(EdgeCa.A, Vector3.Cross(TriNorm, EdgeCa.Delta));

    //public static readonly RangeDouble Zero1 = new RangeDouble(0, 1);

    private Vector3 CalculateCornerAndLine(Vector3 p, double uab, double uca, ref bool isInPlane)
    {
        if (uca > 1 && uab < 0)
        {
            isInPlane = false;
            return (A);
        }
        var ubc = EdgeBc.Project(p);
        if (uab > 1 && ubc < 0)
        {
            isInPlane = false;
            return (B);
        }
        if (ubc > 1 && uca < 0)
        {
            isInPlane = false;
            return (C);
        }

        //line
        if (uab >= 0 && uab <= 1 && !PlaneAb.IsAbove(p))
        {
            isInPlane = false;
            return (EdgeAb.PointAt(uab));
        }
        if (ubc >= 0 && ubc <= 1 && !PlaneBc.IsAbove(p))
        {
            isInPlane = false;
            return (EdgeBc.PointAt(ubc));
        }
        if (uca >= 0 && uca <= 1 && !PlaneCa.IsAbove(p))
        {
            isInPlane = false;
            return (EdgeCa.PointAt(uca));
        }
        isInPlane = true;
        return (Vector3.zero);
    }

    public Vector3 ClosestPointTo(Vector3 p)
    {
        // Find the projection of the point onto the edge

        var uab = EdgeAb.Project(p);
        var uca = EdgeCa.Project(p);
        bool isInPlane = false;

        Vector3 rightPositionIfOutsidePlane = CalculateCornerAndLine(p, uab, uca, ref isInPlane);

        if (!infinitePlane)
        {
            //ici on est dans un plan fini, si on dis de ne pas prendre
            //en compte les borders, juste tomber !
            if (noGravityBorders && !isInPlane)
            {
                //Debug.Log("ici on est PAS dans le plane, juste tomber !");
                return (ExtUtilityFunction.GetNullVector());
            }
            else if (noGravityBorders && isInPlane)
            {
                //Debug.Log("ici on est DANS le plane, ET en noGravityBorder: tester la normal ensuite !");
                if (unidirectionnal)
                {
                    return (GetGoodPointUnidirectionnal(p, TriPlane.Project(EdgeAb.A, TriNormNormalize, p))); //get the good point (or null) in a plane unidirectionnal
                }
                else
                {
                    //ici en gravityBorder, et on est dans le plan, et c'esst multi directionnel, alors OK dac !
                    return (TriPlane.Project(EdgeAb.A, TriNormNormalize, p));
                }
            }
            else if (!noGravityBorders && unidirectionnal)
            {
                //here not infinite, and WITH borders AND unidirectionnal
                if (isInPlane)
                {
                    return (GetGoodPointUnidirectionnal(p, TriPlane.Project(EdgeAb.A, TriNormNormalize, p))); //get the good point (or null) in a plane unidirectionnal
                }
                else
                {
                    return (GetGoodPointUnidirectionnal(p, rightPositionIfOutsidePlane));
                }
            }
            else
            {
                //here Not infinite, WITH borders, NO unidirectionnal
                if (isInPlane)
                {
                    return (TriPlane.Project(EdgeAb.A, TriNormNormalize, p));
                }
                else
                {
                    return (rightPositionIfOutsidePlane);
                }
            }
        }
        else
        {
            if (unidirectionnal)
            {
                return (GetGoodPointUnidirectionnal(p, TriPlane.Project(EdgeAb.A, TriNormNormalize, p))); //get the good point (or null) in a plane unidirectionnal
            }
        }



        //ici le plan est infini, OU fini mais on est dedant


        // The closest point is in the triangle so 
        // project to the plane to find it
        //Vector3 projectedPoint = TriPlane.Project(EdgeAb.A, TriNorm.normalized, p);
        return (TriPlane.Project(EdgeAb.A, TriNormNormalize, p));
    }

    /// <summary>
    /// take into acount unidirectinnal option, return null if not found
    /// </summary>
    /// <returns></returns>
    private Vector3 GetGoodPointUnidirectionnal(Vector3 p, Vector3 foundPosition)
    {
        //Vector3 projectedOnPlane = TriPlane.Project(EdgeAb.A, TriNorm.normalized, p);
        Vector3 dirPlayer = p - foundPosition;

        float dotPlanePlayer = ExtQuaternion.DotProduct(dirPlayer.normalized, TriNormNormalize);
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
}
*/

public struct ExtTriangle
{
    public readonly Vector3 A;
    public readonly Vector3 B;
    public readonly Vector3 C;
    private Vector3 AB;
    private Vector3 AC;

    public bool unidirectionnal;
    public bool inverseDirection;
    public bool infinitePlane;
    public bool noGravityBorders;

    private readonly Vector3 TriNorm;
    public readonly Vector3 TriNormNormalize;

    public ExtTriangle(Vector3 a, Vector3 b, Vector3 c,
        bool _unidirectionnal, bool _inverseDirection, bool _infinitePlane,
        bool _noGravityBorders)
    {
        A = a;
        B = b;
        C = c;

        AB = B - C;
        AC = C - A;

        unidirectionnal = _unidirectionnal;
        inverseDirection = _inverseDirection;
        infinitePlane = _infinitePlane;
        noGravityBorders = _noGravityBorders;

        TriNorm = Vector3.Cross(a - b, a - c);
        TriNormNormalize = TriNorm.normalized;
    }

    public Vector3 ClosestPointTo(Vector3 p)
    {
        // Check if P in vertex region outside A
        Vector3 ap = p - A;
        float d1 = Vector3.Dot(AB, ap);
        float d2 = Vector3.Dot(AC, ap);
        if (d1 <= 0.0f && d2 <= 0.0f) return A; // barycentric coordinates (1,0,0)
                                                // Check if P in vertex region outside B
        Vector3 bp = p - B;
        float d3 = Vector3.Dot(AB, bp);
        float d4 = Vector3.Dot(AC, bp);
        if (d3 >= 0.0f && d4 <= d3) return B; // barycentric coordinates (0,1,0)
                                              // Check if P in edge region of AB, if so return projection of P onto AB
        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
        {
            float v1 = d1 / (d1 - d3);
            return A + v1 * AB; // barycentric coordinates (1-v,v,0)
        }
        // Check if P in vertex region outside C
        Vector3 cp = p - C;
        float d5 = Vector3.Dot(AB, cp);
        float d6 = Vector3.Dot(AC, cp);
        if (d6 >= 0.0f && d5 <= d6) return C; // barycentric coordinates (0,0,1)

        // Check if P in edge region of AC, if so return projection of P onto AC
        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
        {
            float w1 = d2 / (d2 - d6);
            return A + w1 * AC; // barycentric coordinates (1-w,0,w)
        }
        // Check if P in edge region of BC, if so return projection of P onto BC
        float va = d3 * d6 - d5 * d4;
        if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
        {
            float w2 = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            return B + w2 * (C - B); // barycentric coordinates (0,1-w,w)
        }
        // P inside face region. Compute Q through its barycentric coordinates (u,v,w)
        float denom = 1.0f / (va + vb + vc);
        float v = vb * denom;
        float w = vc * denom;
        return A + AB * v + AC * w; // = u*a + v*b + w*c, u = va * denom = 1.0f-v-w
    }
}

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
    public bool infinitePlane;
    public bool noGravityBorders;

    public ExtQuad(Vector3 a, Vector3 b, Vector3 c,
        bool _unidirectionnal, bool _inverseDirection, bool _infinitePlane,
        bool _noGravityBorders)
    {
        A = a;
        B = b;
        C = c;

        AB = B - C;
        AC = C - A;
        maxdistA = Vector3.Dot(AB, AB);
        maxdistC = Vector3.Dot(AC, AC);

        unidirectionnal = _unidirectionnal;
        inverseDirection = _inverseDirection;
        infinitePlane = _infinitePlane;
        noGravityBorders = _noGravityBorders;
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

        return (q);
    }
}

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
    public bool infinitePlane;
    public bool noGravityBorders;

    public ExtTetra(Vector3 a, Vector3 b, Vector3 c, Vector3 d,
        bool _unidirectionnal, bool _inverseDirection, bool _infinitePlane,
        bool _noGravityBorders)
    {
        A = a;
        B = b;
        C = c;
        D = d;

        BA = B - A;
        CA = C - A;
        DA = D - A;

        crossBACA = Vector3.Cross(B - A, C - A);

        triangleA = new ExtTriangle(a, b, c, _unidirectionnal, _inverseDirection, _infinitePlane, _noGravityBorders);
        triangleB = new ExtTriangle(a, c, d, _unidirectionnal, _inverseDirection, _infinitePlane, _noGravityBorders);
        triangleC = new ExtTriangle(a, d, b, _unidirectionnal, _inverseDirection, _infinitePlane, _noGravityBorders);
        triangleD = new ExtTriangle(b, d, c, _unidirectionnal, _inverseDirection, _infinitePlane, _noGravityBorders);

        unidirectionnal = _unidirectionnal;
        inverseDirection = _inverseDirection;
        infinitePlane = _infinitePlane;
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

    // Return point q on (or in) rect (specified by a, b, and c), closest to given point p
    public Vector3 ClosestPtPointRect(Vector3 p)
    {
        // Start out assuming point inside all halfspaces, so closest to itself
        Vector3 closestPt = p;
        float bestSqDist = float.MaxValue;
        // If point outside face abc then compute closest point on abc
        if (PointOutsideOfPlane(p, A, B, C))
        {
            Vector3 q = triangleA.ClosestPointTo(p);
            float sqDist = Vector3.Dot(q - p, q - p);
            // Update best closest point if (squared) distance is less than current best
            if (sqDist < bestSqDist)
            {
                bestSqDist = sqDist;
                closestPt = q;
            }
        }
        // Repeat test for face acd
        if (PointOutsideOfPlane(p, A, C, D))
        {
            Vector3 q = triangleB.ClosestPointTo(p);
            float sqDist = Vector3.Dot(q - p, q - p);
            if (sqDist < bestSqDist)
            {
                bestSqDist = sqDist;
                closestPt = q;
            }
        }

        // Repeat test for face adb
        if (PointOutsideOfPlane(p, A, D, B))
        {
            Vector3 q = triangleC.ClosestPointTo(p);
            float sqDist = Vector3.Dot(q - p, q - p);
            if (sqDist < bestSqDist)
            {
                bestSqDist = sqDist;
                closestPt = q;
            }
        }
        // Repeat test for face bdc
        if (PointOutsideOfPlane(p, B, D, C))
        {
            Vector3 q = triangleD.ClosestPointTo(p);
            float sqDist = Vector3.Dot(q - p, q - p);
            if (sqDist < bestSqDist)
            {
                bestSqDist = sqDist;
                closestPt = q;
            }
        }
        return closestPt;
    }
}

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

    public double Project(Vector3 p) => ExtQuaternion.DotProduct(Delta, p - A) / LengthSquared;
    */
}

public struct ExtPlane
{
    public Vector3 Point;
    public Vector3 Direction;

    public ExtPlane(Vector3 point, Vector3 direction)
    {
        Point = point;
        Direction = direction;
    }

    public bool IsAbove(Vector3 q) => ExtQuaternion.DotProduct(q - Point, Direction) > 0;

    
    public Vector3 Project(Vector3 randomPointInPlane, Vector3 normalPlane, Vector3 pointToProject)
    {
        //Vector3 v = pointToProject - randomPointInPlane;
        //Vector3 d = Vector3.Project(v, normalPlane.normalized);        
        //Vector3 projectedPoint = pointToProject - d;
        //return (projectedPoint);
        return (pointToProject - (Vector3.Project(pointToProject - randomPointInPlane, normalPlane.normalized)));
    }
}