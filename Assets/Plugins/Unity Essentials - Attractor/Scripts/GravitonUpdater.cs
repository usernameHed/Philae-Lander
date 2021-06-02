using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.ActionTrigger.entity;
using UnityEssentials.ActionTrigger.Trigger;
using UnityEssentials.Extensions;
using UnityEssentials.PropertyAttribute.noNull;
using UnityEssentials.PropertyAttribute.readOnly;
using UnityEssentials.time;
using static UnityEssentials.Attractor.Attractor;

namespace UnityEssentials.Attractor
{
    public class GravitonUpdater : MonoBehaviour
    {
        [SerializeField, NoNull]        private Graviton _graviton = default;
        [SerializeField, NoNull]        private Rigidbody _rigidBody = default;

        protected virtual void FixedUpdate()
        {
            _graviton.CalculatePhysicNormal();
            _rigidBody.AddForce(_graviton.GravityDirection, ForceMode.Force);
        }
    }
}