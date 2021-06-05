using System;
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
    [Serializable]//to Remove
    public class ExtGravitonCalculation
    {
        [SerializeField]//to Remove ?
        private List<AttractorInfo> _attractorInfo = new List<AttractorInfo>(Attractor.DEFAULT_MAX_ATTRACTOR);
        public AttractorInfo AttractorIndex(int index) { return (_attractorInfo[index]); }
        public int AttractorCounts { get { return (_attractorInfo.Count); } }

        [SerializeField]//to Remove ?
        private List<float> _forceAmount = new List<float>(Attractor.DEFAULT_MAX_ATTRACTOR);
        public float ForceAmountIndex(int index) { return (_forceAmount[index]); }

        private AttractorInfo _closestAttractor = default;
        public AttractorInfo ClosestAttractor { get { return (_closestAttractor); } }

        private AttractorInfo _tmpAttractorInfo = new AttractorInfo();
        private int _closestIndex;

        public void OverrideContactPointOfClosestAttractor(Vector3 newContactPoint)
        {
            _closestAttractor.ContactPoint = newContactPoint;
        }

        public void SetupGravityFields(List<Attractor> attractorApplyingForce, Vector3 position)
        {
            //calculate all force from all shape (even the ones in the same groups)
            _attractorInfo.Clear();
            _forceAmount.Clear();
            for (int i = 0; i < attractorApplyingForce.Count; i++)
            {
                Vector3 closestPoint = Vector3.zero;
                _tmpAttractorInfo = attractorApplyingForce[i].GetGravityDirectionFromPointInSpace(position, ref closestPoint);
                if (_tmpAttractorInfo.AttractorRef.CurrentGroup != null)
                {
                    _tmpAttractorInfo.AttractorRef.CurrentGroup.ResetGroup();
                }
                _attractorInfo.Add(_tmpAttractorInfo);
                _forceAmount.Add(0);
            }
        }

        public void RemoveAttractorFromOneDirection(Vector3 directionToIgnore, float dotMargin)
        {
            //settup closest valid attractor element on each group
            for (int i = _attractorInfo.Count - 1; i >= 0; i--)
            {
                if (_attractorInfo[i].CanApplyGravity && i != _closestIndex)
                {
                    float dotGravity = Vector3.Dot(_attractorInfo[i].NormalizedDirection, directionToIgnore);
                    //Debug.Log("dot: " + dotGravity);
                    if (dotGravity > dotMargin)
                    {
                        //Debug.Log("remove " + _attractorInfo[i].AttractorRef, _attractorInfo[i].AttractorRef.gameObject);
                        //_attractorInfo.RemoveAt(i);
                        _tmpAttractorInfo = _attractorInfo[i];
                        _tmpAttractorInfo.CanApplyGravity = false;
                        _attractorInfo[i] = _tmpAttractorInfo;
                        //_forceAmount.RemoveAt(i);
                    }
                }
            }
        }

        public void CalculateGravityFields()
        {
            if (_attractorInfo.Count == 0)
            {
                return;
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
            if (_closestIndex == -1)
            {
                return;
            }

            _closestAttractor = _attractorInfo[_closestIndex];
        }


        public Vector3 CalculateForces(float mass)
        {
            if (_closestIndex == -1 || _attractorInfo.Count == 0)
            {
                Debug.Log("No Force :)");
                return (Vector3.zero);
            }

            //apply default force setting for the closest gravityFields
            float referenceSqrDistance = _attractorInfo[_closestIndex].SqrDistance;
            float force = mass * _attractorInfo[_closestIndex].Gravity * AttractorSettings.Instance.Gravity;
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
                float forceMagnitude = (mass * gravityOfGravityField) / ((sqrDistanceGravitonToGravityField / (referenceSqrDistance - K)));
                _forceAmount[i] = forceMagnitude;

                Vector3 forceAttraction = _attractorInfo[i].NormalizedDirection * _forceAmount[i];
                sumForce += forceAttraction;
            }

            return sumForce;
        }
    }
}