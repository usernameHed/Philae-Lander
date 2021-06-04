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
    public class GravitonUpdaterIgnoreDirection : MonoBehaviour
    {
        [SerializeField, NoNull]        private Graviton _graviton = default;
        [SerializeField, NoNull]        private Rigidbody _rigidBody = default;
        [SerializeField, NoNull]        private Transform _directionToIgnore = default;
        [SerializeField]                private float _dotMargin = 0.4f;

        protected virtual void FixedUpdate()
        {
            _graviton.CalculatePhysicNormalIgnoringADirection(_directionToIgnore.forward, _dotMargin);
            _rigidBody.AddForce(_graviton.GravityDirection, ForceMode.Force);
        }
    }
}