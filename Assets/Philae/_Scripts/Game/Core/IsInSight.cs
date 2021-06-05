
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

namespace Philae.Core
{
    public class IsInSight : MonoBehaviour
    {
        [Tooltip("head sight")]
        public float radius = 3f;
        [Tooltip("head sight")]
        public float offsetPlayerDist = 0.1f;
        [Tooltip("head sight")]
        public Transform headSight;
        [Tooltip("where to do  raycast")]
        public Transform target;



        public bool IsTargetInSight()
        {
            RaycastHit hitInfo;
            Vector3 dir = target.position - headSight.position;

            int layerMask = Physics.AllLayers;
            layerMask = ~LayerMask.GetMask(GameData.Tags.Enemy.ToString());

            if (Physics.SphereCast(headSight.position, radius, dir, out hitInfo,
                                   dir.magnitude + offsetPlayerDist, layerMask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log(hitInfo.collider.gameObject.name);

                ExtDrawGuizmos.DebugWireSphere(hitInfo.point, Color.blue, radius, 0.1f);
                Debug.DrawLine(headSight.position, hitInfo.point, Color.blue, 0.1f);
                return (true);
            }
            return (false);
        }
        /*
        private void Update()
        {
            IsTargetInSight();
        }
        */
    }
}