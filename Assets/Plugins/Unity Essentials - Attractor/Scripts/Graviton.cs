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
    public class Graviton : MonoBehaviour, IActionComponents
    {
        [SerializeField] private float _mass = 1f;
        [SerializeField] private float _frequencySearchClosestPoint = 0.1f;
        

        private Vector3 _defaultNormal = Vector3.down;
        private Vector3 _gravityDirection = Vector3.down;
        public Vector3 GravityDirection { get { return (_gravityDirection); } }

        private AttractorInfo _closestAttractor = default;
        public AttractorInfo ClosestAttractor { get { return (_closestAttractor); } }

        //refs
        [SerializeField, NoNull] protected Rigidbody _rigidBody = default;
        [SerializeField, NoNull] protected Transform _relativeForward = default;
        public Vector3 Up { get { return (_relativeForward.up); } }
        public Vector3 Forward { get { return (_relativeForward.forward); } }
        public Vector3 Right { get { return (_relativeForward.right); } }

        [SerializeField, ReadOnly]
        private List<Attractor> _attractorApplyingForce = new List<Attractor>(Attractor.DEFAULT_MAX_ATTRACTOR);


        [SerializeField, ReadOnly]
        private List<AttractorInfo> _attractorInfo = new List<AttractorInfo>(Attractor.DEFAULT_MAX_ATTRACTOR);
        [SerializeField, ReadOnly]
        private List<float> _forceAmount = new List<float>(Attractor.DEFAULT_MAX_ATTRACTOR);

        private AttractorInfo _tmpAttractorInfo = new AttractorInfo();
        private int _closestIndex;
        private FrequencyCoolDown _coolDownCalculation = new FrequencyCoolDown();

        private void Awake()
        {
            _gravityDirection = _defaultNormal;
        }

        public void AddGravityAction(Attractor gravityField)
        {
            _attractorApplyingForce.AddIfNotContain(gravityField);
        }

        public void RemoveGravityAction(Attractor gravityField)
        {
            _attractorApplyingForce.Remove(gravityField);
        }

        public void OverrideContactPointOfClosestAttractor(Vector3 newContactPoint)
        {
            _closestAttractor.ContactPoint = newContactPoint;
        }
        //protected virtual void FixedUpdate()
        //{
        //    CalculatePhysicNormal();
        //    MoveAction(_currentCalculatedNormal);
        //}

        public void CalculatePhysicNormal()
        {
            //if there is no gravity fields, keep old gravity
            if (_attractorApplyingForce.Count == 0)
            {
                //_currentCalculatedNormal = _defaultNormal;
            }
            else
            {
                //apply sum of the normals
                _gravityDirection = CalculatePhysicBasedOnGravityFields();
            }
        }

        private Vector3 CalculatePhysicBasedOnGravityFields()
        {
            if (_frequencySearchClosestPoint == 0)
            {
                CalculateGravityFields();
            }
            else if (_coolDownCalculation.IsNotRunning())
            {
                CalculateGravityFields();
                _coolDownCalculation.StartCoolDown(_frequencySearchClosestPoint);
            }
            Vector3 sumForce = CalculateForces();
            return (sumForce);
        }

        

        private void CalculateGravityFields()
        {
            //calculate all force from all shape (even the ones in the same groups)
            _attractorInfo.Clear();
            _forceAmount.Clear();
            for (int i = 0; i < _attractorApplyingForce.Count; i++)
            {
                Vector3 closestPoint = Vector3.zero;
                _tmpAttractorInfo = _attractorApplyingForce[i].GetGravityDirectionFromPointInSpace(transform.position, ref closestPoint);
                if (_tmpAttractorInfo.AttractorRef.CurrentGroup != null)
                {
                    _tmpAttractorInfo.AttractorRef.CurrentGroup.ResetGroup();
                }
                _attractorInfo.Add(_tmpAttractorInfo);
                _forceAmount.Add(0);
            }

            //settup closest valid attractor element on each group
            for (int i = 0; i < _attractorInfo.Count; i++)
            {
                if (_attractorInfo[i].CanApplyGravity && _attractorInfo[i].AttractorRef.CurrentGroup != null)
                {
                    _attractorInfo[i].AttractorRef.CurrentGroup.AttemptToSetNewClosestGravityField(_attractorInfo[i].AttractorRef, _attractorInfo[i].SqrDistance);
                }
            }

            //invalidate all attractor that are not the closest in each group
            for (int i = 0; i < _attractorInfo.Count; i++)
            {
                if (_attractorInfo[i].CanApplyGravity && _attractorInfo[i].AttractorRef.CurrentGroup != null)
                {
                    bool canReallyApplyGravity = _attractorInfo[i].AttractorRef.CurrentGroup.IsTheValidOneInTheGroup(_attractorInfo[i].AttractorRef);

                    if (_attractorInfo[i].CanApplyGravity != canReallyApplyGravity)
                    {
                        _tmpAttractorInfo = _attractorInfo[i];
                        _tmpAttractorInfo.CanApplyGravity = canReallyApplyGravity;
                        _attractorInfo[i] = _tmpAttractorInfo;
                    }
                }
            }

            //find closest index
            _closestIndex = -1;
            float shortestDistance = 9999999;
            for (int i = 0; i < _attractorInfo.Count; i++)
            {
                if (!_attractorInfo[i].CanApplyGravity)
                {
                    continue;
                }
                if (_closestIndex == -1 || _attractorInfo[i].SqrDistance < shortestDistance)
                {
                    _closestIndex = i;
                    shortestDistance = _attractorInfo[i].SqrDistance;
                }
            }
            _closestAttractor = _attractorInfo[_closestIndex];
        }

        private Vector3 CalculateForces()
        {
            //apply default force setting for the closest gravityFields
            float referenceSqrDistance = _attractorInfo[_closestIndex].SqrDistance;
            float force = _mass * _attractorInfo[_closestIndex].Gravity * AttractorSettings.Instance.Gravity;
            Vector3 sumForce = _attractorInfo[_closestIndex].NormalizedDirection * force;
            _forceAmount[_closestIndex] = force;


            //finally, loop though all valid forces, and apply them
            for (int i = 0; i < _attractorInfo.Count; i++)
            {
                if (i == _closestIndex || !_attractorInfo[i].CanApplyGravity)
                {
                    continue;
                }

                float gravityOfGravityField = _attractorInfo[i].Gravity * AttractorSettings.Instance.Gravity;
                float sqrDistanceGravitonToGravityField = _attractorInfo[i].SqrDistance;
                float K = AttractorSettings.Instance.RatioEaseOutAttractors;

                // F = (mass_GRAVITON * gravity_ATTRACTOR) / ((distance_GRAVITON_ATTRACTOR / distance_reference) * K)
                float forceMagnitude = (_mass * gravityOfGravityField) / ((sqrDistanceGravitonToGravityField / (referenceSqrDistance - K)));
                _forceAmount[i] = forceMagnitude;

                Vector3 forceAttraction = _attractorInfo[i].NormalizedDirection * _forceAmount[i];
                sumForce += forceAttraction;
            }

            return sumForce;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (!AttractorSettings.Instance.ShowArrow)
            {
                return;
            }

            for (int i = 0; i < _attractorInfo.Count; i++)
            {
                ExtDrawGuizmos.DebugWireSphere(_attractorInfo[i].ContactPoint, Color.green, 0.1f);
                ExtDrawGuizmos.DrawArrow(_rigidBody.position, _attractorInfo[i].NormalizedDirection * _forceAmount[i] * AttractorSettings.Instance.RatioSizeArrow, Color.white);
            }
            ExtDrawGuizmos.DrawArrow(_rigidBody.position, _gravityDirection * AttractorSettings.Instance.RatioSizeArrow, Color.cyan);
        }
#endif
    }
}