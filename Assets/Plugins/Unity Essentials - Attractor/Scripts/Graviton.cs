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

        
        //refs
        [SerializeField, NoNull] protected Rigidbody _rigidBody = default;
        [SerializeField, NoNull] protected Transform _relativeForward = default;
        public Vector3 Up { get { return (_relativeForward.up); } }
        public Vector3 Forward { get { return (_relativeForward.forward); } }
        public Vector3 Right { get { return (_relativeForward.right); } }

        [Header("Debug")]
        [SerializeField]
        private List<Attractor> _attractorApplyingForce = new List<Attractor>(Attractor.DEFAULT_MAX_ATTRACTOR);

        [SerializeField]
        private ExtGravitonCalculation _extGravitonCalculation = new ExtGravitonCalculation();
        public AttractorInfo ClosestAttractor { get { return (_extGravitonCalculation.ClosestAttractor); } }

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
            _extGravitonCalculation.OverrideContactPointOfClosestAttractor(newContactPoint);
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
                _extGravitonCalculation.CalculateGravityFields(_attractorApplyingForce, transform.position);
            }
            else if (_coolDownCalculation.IsNotRunning())
            {
                _extGravitonCalculation.CalculateGravityFields(_attractorApplyingForce, transform.position);
                _coolDownCalculation.StartCoolDown(_frequencySearchClosestPoint);
            }
            Vector3 sumForce = _extGravitonCalculation.CalculateForces(_mass);
            return (sumForce);
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

            for (int i = 0; i < _extGravitonCalculation.AttractorCounts; i++)
            {
                ExtDrawGuizmos.DebugWireSphere(_extGravitonCalculation.AttractorIndex(i).ContactPoint, Color.green, 0.1f);
                ExtDrawGuizmos.DrawArrow(_rigidBody.position, _extGravitonCalculation.AttractorIndex(i).NormalizedDirection * _extGravitonCalculation.ForceAmountIndex(i) * AttractorSettings.Instance.RatioSizeArrow, Color.white);
            }
            ExtDrawGuizmos.DrawArrow(_rigidBody.position, _gravityDirection * AttractorSettings.Instance.RatioSizeArrow, Color.cyan);
        }
#endif
    }
}