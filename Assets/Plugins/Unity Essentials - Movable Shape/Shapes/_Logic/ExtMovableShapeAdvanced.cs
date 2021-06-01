using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.extensions;

namespace UnityEssentials.Geometry.MovableShape
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtMovableShapeAdvanced
    {
        public static Vector3 GetRightPosWithRange(Vector3 posEntity, Vector3 posCenter, float range, float maxRange, out bool outOfRange)
        {
            outOfRange = false;

            if (range == 0 && maxRange == 0)
            {
                //Debug.Log("always 'in zone', no range defined");
                return (posCenter);
            }

            Vector3 posFound = posCenter;
            float lenghtCenterToPlayer = 0;

            if (range > 0)
            {
                Vector3 realPos = posCenter + (posEntity - posCenter).FastNormalized() * range;
                lenghtCenterToPlayer = (posEntity - posCenter).sqrMagnitude;
                float lenghtCenterToRangeMax = (realPos - posCenter).sqrMagnitude;
                if (lenghtCenterToRangeMax > lenghtCenterToPlayer)
                {
                    realPos = posEntity;
                    //Debug.Log("is inside !");
                    //outOfRange = true;
                }
                else
                {
                    //Debug.Log("is in zone !");
                }

                posFound = realPos;
            }

            //if player is out of range, return null
            if (maxRange > range && maxRange != 0)
            {
                Vector3 realPos = posCenter + (posEntity - posCenter).normalized * maxRange;
                //calculate only if we havn't already calculate
                if (range == 0)
                    lenghtCenterToPlayer = (posEntity - posCenter).sqrMagnitude;
                float lenghtCenterToRangeMax = (realPos - posCenter).sqrMagnitude;
                if (lenghtCenterToPlayer > lenghtCenterToRangeMax)
                {
                    posFound = Vector3.zero;
                    outOfRange = true;
                    //Debug.Log("is out of zone !");
                }
            }

            return (posFound);
        }
    }
}