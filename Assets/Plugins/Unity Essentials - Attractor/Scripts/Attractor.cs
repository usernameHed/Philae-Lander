using System;
using UnityEngine;
using UnityEssentials.ActionTrigger.Actions;
using UnityEssentials.ActionTrigger.entity;
using UnityEssentials.Extensions;
using UnityEssentials.Geometry.MovableShape;
using UnityEssentials.PropertyAttribute.noNull;
using UnityEssentials.PropertyAttribute.readOnly;

namespace UnityEssentials.Attractor
{
    public class Attractor : ActionTrigger.Actions.Action
    {
        public const int DEFAULT_MAX_ATTRACTOR = 100;

        [Serializable]//to Remove
        public struct AttractorInfo
        {
            public bool CanApplyGravity;
            public Vector3 ContactPoint;
            public Vector3 NormalizedDirection;
            public float SqrDistance;
            public float Gravity;
            public Attractor AttractorRef;
        }

        [SerializeField, NoNull] private MovableShape _attractorShape = default;
        [SerializeField] private float _gravityFactor = 1f;
        [SerializeField, Tooltip("If group can be set & unset inplay mode, tick that to true")] private bool _dynamiclyTestGroup = false;
        [SerializeField, ReadOnly] private AttractorGroup _currentGroup = default;
        public AttractorGroup CurrentGroup { get { return (_currentGroup); } }

        private AttractorInfo _attractorInfo = new AttractorInfo();

        private void OnEnable()
        {
            _attractorInfo.AttractorRef = this;
            _attractorInfo.Gravity = _gravityFactor;
            _currentGroup = transform.GetComponentInParent<AttractorGroup>();
        }

        public AttractorInfo GetGravityDirectionFromPointInSpace(Vector3 position, ref Vector3 closestPoint)
        {
            _attractorInfo.CanApplyGravity = _attractorShape.GetClosestPoint(position, ref closestPoint);
            _attractorInfo.ContactPoint = closestPoint;
            _attractorInfo.NormalizedDirection = (closestPoint - position).FastNormalized();
            _attractorInfo.Gravity = _gravityFactor;
            _attractorInfo.SqrDistance = Vector3.SqrMagnitude(closestPoint - position);
            return (_attractorInfo);
        }

        public override void OnEnter(Entity entity)
        {
            if (entity.HasIActionComponent<Graviton>(out Graviton gravityEntity))
            {
                gravityEntity.AddGravityAction(this);
            }
        }

        public override void OnExit(Entity entity)
        {
            if (entity.HasIActionComponent<Graviton>(out Graviton gravityEntity))
            {
                gravityEntity.RemoveGravityAction(this);
            }
        }

        public override void OnStay(Entity entity)
        {
            if (_dynamiclyTestGroup)
            {
                _currentGroup = transform.GetComponentInParent<AttractorGroup>();
            }
        }
    }
}