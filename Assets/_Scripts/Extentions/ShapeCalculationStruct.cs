using UnityEngine;

public struct ExtTriangle
{
    public Vector3 A => EdgeAb.A;
    public Vector3 B => EdgeBc.A;
    public Vector3 C => EdgeCa.A;

    public readonly ExtLine EdgeAb;
    public readonly ExtLine EdgeBc;
    public readonly ExtLine EdgeCa;

    public ExtTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        EdgeAb = new ExtLine(a, b);
        EdgeBc = new ExtLine(b, c);
        EdgeCa = new ExtLine(c, a);
        TriNorm = Vector3.Cross(a - b, a - c);
    }

    public Vector3[] Verticies => new[] { A, B, C };

    public readonly Vector3 TriNorm;

    //private static readonly RangeDouble ZeroToOne = new RangeDouble(0, 1);

    public ExtPlane TriPlane => new ExtPlane(A, TriNorm);

    // The below three could be pre-calculated to
    // trade off space vs time

    public ExtPlane PlaneAb => new ExtPlane(EdgeAb.A, Vector3.Cross(TriNorm, EdgeAb.Delta));
    public ExtPlane PlaneBc => new ExtPlane(EdgeBc.A, Vector3.Cross(TriNorm, EdgeBc.Delta));
    public ExtPlane PlaneCa => new ExtPlane(EdgeCa.A, Vector3.Cross(TriNorm, EdgeCa.Delta));

    //public static readonly RangeDouble Zero1 = new RangeDouble(0, 1);

    public Vector3 ClosestPointTo(Vector3 p)
    {
        // Find the projection of the point onto the edge

        var uab = EdgeAb.Project(p);
        var uca = EdgeCa.Project(p);

        //corner
        if (uca > 1 && uab < 0)
            return A;

        var ubc = EdgeBc.Project(p);

        if (uab > 1 && ubc < 0)
            return B;

        if (ubc > 1 && uca < 0)
            return C;

        //line
        if (uab >= 0 && uab <= 1 && !PlaneAb.IsAbove(p))
            return EdgeAb.PointAt(uab);

        if (ubc >= 0 && ubc <= 1 && !PlaneBc.IsAbove(p))
            return EdgeBc.PointAt(ubc);

        if (uca >= 0 && uca <= 1 && !PlaneCa.IsAbove(p))
            return EdgeCa.PointAt(uca);

        // The closest point is in the triangle so 
        // project to the plane to find it
        return (TriPlane.Project(EdgeAb.A, TriNorm.normalized, p));
    }
}

public struct ExtLine
{
    public readonly Vector3 A;
    public readonly Vector3 B;
    public readonly Vector3 Delta;

    public ExtLine(Vector3 a, Vector3 b)
    {
        A = a;
        B = b;
        Delta = b - a;
    }

    public Vector3 PointAt(double t) => A + (float)t * Delta;
    public double LengthSquared => Delta.sqrMagnitude;

    public Vector3 ClosestPointTo(Vector3 p)
    {
        //The normalized "distance" from a to your closest point 
        double distance = Project(p); 

        if (distance < 0)     //Check if P projection is over vectorAB     
        {
            return A;
        }
        else if (distance > 1)
        {
            return B;
        }
        else
        {
            return A + Delta * (float)distance;
        }
    }

    public double Project(Vector3 p) => ExtQuaternion.DotProduct(Delta, p - A) / LengthSquared;
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

    //public bool IsAbove(Vector3 q) => Direction.Dot(q - Point) > 0;
    public bool IsAbove(Vector3 q) => ExtQuaternion.DotProduct(q - Point, Direction) > 0;

    public Vector3 Project(Vector3 randomPointInPlane, Vector3 normalPlane, Vector3 pointToProject)
    {
        Vector3 v = pointToProject - randomPointInPlane;
        Vector3 d = Vector3.Project(v, normalPlane.normalized);
        Vector3 projectedPoint = pointToProject - d;
        return (projectedPoint);
    }
}