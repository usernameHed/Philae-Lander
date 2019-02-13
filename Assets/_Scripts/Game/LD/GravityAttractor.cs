using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    [Serializable]
    public struct GravityPoint
    {
        public Transform point;
        public float gravityRatio;
        public float dist;
    }

    [FoldoutGroup("GamePlay"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private List<GravityPoint> gravityPoint = new List<GravityPoint>();
    [FoldoutGroup("GamePlay"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private bool takeDistIntoAccount = true;

    [FoldoutGroup("Debug"), ReadOnly]
    public List<GravityPoint> lastListFound = new List<GravityPoint>();

    public void SelectedGravityAttractor()
    {
        Debug.Log(gameObject.name + " selected !" + gameObject);
        lastListFound.Clear();
    }

    public void UnselectGravityAttractor()
    {
        Debug.Log(gameObject.name + " un-selected !" + gameObject);
        lastListFound.Clear();
    }

    /// <summary>
    /// get the closest gravity point
    /// </summary>
    /// <param name="entity"></param>
    public List<GravityPoint> GetPoint(Vector3 entity)
    {
        lastListFound.Clear();

        float sqrDist = 0;
        int indexGravityPoint = -1;
        for (int i = 0; i < gravityPoint.Count; i++)
        {
            float dist = (entity - gravityPoint[i].point.position).sqrMagnitude;
            if (i == 0)
            {
                indexGravityPoint = 0;
                sqrDist = dist;
            }
            else if (dist < sqrDist)
            {
                sqrDist = dist;
                indexGravityPoint = i;
            }
        }
        if (indexGravityPoint == -1)
            Debug.LogError("nothing found");

        lastListFound.Add(gravityPoint[indexGravityPoint]);

         /*
        for (int i = 0; i < gravityPoint.Count; i++)
        {
            float dist = (entity.position - gravityPoint[i].point.position).sqrMagnitude;
            if (takeDistIntoAccount)
            {
                if (dist < gravityPoint[i].dist * gravityPoint[i].dist)
                    lastListFound.Add(gravityPoint[i].point);
            }
            else
            {

            }
        }
        if (lastListFound.Count == 0)
            Debug.LogError("nothing found");
            */
        return (lastListFound);
    }

    private void DisplayPoint()
    {
        for (int i = 0; i < gravityPoint.Count; i++)
        {
            if (gravityPoint[i].point != null)
            {
                Gizmos.color = Color.white;
                if (lastListFound.Contains(gravityPoint[i]))
                    Gizmos.color = Color.red;

                if (takeDistIntoAccount)
                    Gizmos.DrawWireSphere(gravityPoint[i].point.position, gravityPoint[i].dist * gravityPoint[i].dist);

                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(10, 10, 10)));
                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(-10, -10, -10)));

                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(-10, 10, 10)));
                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(10, -10, -10)));

                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(-10, 0, 10)));
                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(10, 0, -10)));

                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(10, 10, 0)));
                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(-10, -10, 0)));

                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(0, 10, 10)));
                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(0, -10, -10)));

                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(0, 0, 10)));
                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(0, 0, -10)));

                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(0, 10, 0)));
                Gizmos.DrawRay(new Ray(gravityPoint[i].point.position, new Vector3(0, -10, 0)));
            }
        }
    }

    private void OnDrawGizmos()
    {
        DisplayPoint();
    }
}
