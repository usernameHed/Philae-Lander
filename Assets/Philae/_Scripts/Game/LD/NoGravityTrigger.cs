
using Philae.CCC;
using Philae.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Extensions;

namespace Philae.LD
{
    /// <summary>
    /// Kill any IKillable instance on contact
    /// <summary>
    public class NoGravityTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("enum to interact")]
        public float ratioGravity = 0f;

        [SerializeField, Tooltip("enum to interact")]
        private List<GameData.Tags> tagList = new List<GameData.Tags>() { GameData.Tags.Player, GameData.Tags.Enemy };

        [SerializeField]
        private SphereCollider sphereCollider = null;

        private void OnTriggerEnter(Collider other)
        {
            if (ExtList.ListContain<GameData.Tags>(tagList, other.tag))
            {
                TriggerController entityNoGravity = other.gameObject.GetComponent<TriggerController>();
                if (entityNoGravity)
                {
                    entityNoGravity.entityTriggerManager.entityNoGravity.EnterInZone(this);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (ExtList.ListContain<GameData.Tags>(tagList, other.tag))
            {
                TriggerController entityNoGravity = other.gameObject.GetComponent<TriggerController>();
                if (entityNoGravity)
                {
                    entityNoGravity.entityTriggerManager.entityNoGravity.LeanInZone(this);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            float radius = 1.5f;
            if (sphereCollider)
                radius = sphereCollider.radius;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

    }
}