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
        public float dist;
    }

    [FoldoutGroup("GamePlay"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private List<GravityPoint> gravityPoint;
    [FoldoutGroup("GamePlay"), Tooltip("when not grounded, check again if the distance is realy close to floor anyway"), SerializeField]
    private bool takeDistIntoAccount = true;

    [FoldoutGroup("Debug"), ReadOnly]
    public List<Transform> lastListFound = new List<Transform>();

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
    public List<Transform> GetPoint(Rigidbody entity)
    {
        lastListFound.Clear();
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

        return (lastListFound);
    }

    private void DisplayPoint()
    {
        for (int i = 0; i < gravityPoint.Count; i++)
        {
            if (gravityPoint[i].point != null)
            {
                Gizmos.color = Color.white;
                if (lastListFound.Contains(gravityPoint[i].point))
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
