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
        [SerializeField] private float _maxSpeed = 70f;
        [SerializeField, NoNull]        private Graviton _graviton = default;
        [SerializeField, NoNull] private Rigidbody _rigidBody = default;

        protected virtual void FixedUpdate()
        {
            _graviton.CalculatePhysicNormal();
            MoveAction(_graviton.CurrentNormal);
        }

        public void MoveAction(Vector3 direction)
        {
            if (!AttractorSettings.Instance.SimulatePhysic)
            {
                return;
            }

            direction = ExtRigidBody.ClampVelocity(direction, _maxSpeed);
            _rigidBody.AddForce(direction, ForceMode.Force);
        }
    }
}