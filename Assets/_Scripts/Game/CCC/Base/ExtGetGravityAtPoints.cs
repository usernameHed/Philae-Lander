using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtGetGravityAtPoints
{
    /// <summary>
    /// return the closest point of a given GravityAttractor
    /// </summary>
    /// <param name="posEntity"></param>
    /// <returns></returns>
    private static bool GetClosestPointOfGA(Vector3 posEntity, GravityAttractorLD gravityAttractorToTest, ref GravityAttractorLD.PointInfo pointInfoToFill)
    {
        //Debug.Log("ou la ?");
        pointInfoToFill = gravityAttractorToTest.FindNearestPoint(posEntity);
        if (ExtUtilityFunction.IsNullVector(pointInfoToFill.pos))
        {
            //Debug.LogWarning("ici on a pas trouvé de nouvelle gravité... garder comme maintenant ? mettre le compteur de mort ?");
            //Debug.DrawRay(posEntity, pointInfo.sphereGravity * -10, Color.red, 5f);
            return (false);
        }
        //pointInfo = tmpPointInfo;
        pointInfoToFill.sphereGravity = (posEntity - pointInfoToFill.pos).normalized;
        return (true);
    }

    public static GravityAttractorLD.PointInfo GetAirSphereGravityStatic(Vector3 posEntity, List<GravityAttractorLD> allGravityAttractor)
    {
        GravityAttractorLD.PointInfo pointInfo = new GravityAttractorLD.PointInfo();
        Vector3 lastNormalJumpChoosen = Vector3.down;

        //prepare array
        GravityAttractorLD.PointInfo[] allPointInfo = new GravityAttractorLD.PointInfo[allGravityAttractor.Count];
        Vector3[] closestPost = ExtUtilityFunction.CreateNullVectorArray(allGravityAttractor.Count + 1);
        Vector3[] sphereDir = ExtUtilityFunction.CreateNullVectorArray(allGravityAttractor.Count + 1);

        //fill array with data from 
        for (int i = 0; i < allGravityAttractor.Count; i++)
        {
            GetClosestPointOfGA(posEntity, allGravityAttractor[i], ref allPointInfo[i]);

            sphereDir[i] = allPointInfo[i].sphereGravity;

            //correct pos depending on ratio ?
            closestPost[i] = allPointInfo[i].posRange;
            //closestPost[i] = allPointInfo[i].pos;

            //ExtDrawGuizmos.DebugWireSphere(allPointInfo[i].posRange, Color.blue, 1f);
            //ExtDrawGuizmos.DebugWireSphere(allPointInfo[i].pos, Color.green, 1f);
        }

        //setup the closest point, and his vector director
        int indexFound = -1;
        Vector3 close = ExtUtilityFunction.GetClosestPoint(posEntity, closestPost, ref indexFound);

        if (ExtUtilityFunction.IsNullVector(close))
        {
            Debug.LogWarning("null gravity !!");
            pointInfo.sphereGravity = lastNormalJumpChoosen;
            pointInfo.pos = pointInfo.posRange = lastNormalJumpChoosen * 5;
            return (pointInfo);
        }

        GravityAttractorLD.PointInfo closestPoint = allPointInfo[indexFound];
        return (closestPoint);
    }
}
