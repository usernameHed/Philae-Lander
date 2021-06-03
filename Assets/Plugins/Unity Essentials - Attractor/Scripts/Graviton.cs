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
        public float Mass { get { return (_mass); } }
        [SerializeField] private float _frequencySearchClosestPoint = 0.1f;
        

        private Vector3 _defaultNormal = Vector3.down;
        private Vector3 _gravityDirection = Vector3.down;
        public Vector3 GravityDirection { get { return (_gravityDirection); } set { _gravityDirection = value; } }

        
        //refs
        [SerializeField, NoNull] protected Rigidbody _rigidBody = default;
        [SerializeField, NoNull] protected Transform _relativeForward = default;
        public Vector3 Up { get { return (_relativeForward.up); } }
        public Vector3 Forward { get { return (_relativeForward.forward); } }
        public Vector3 Right { get { return (_relativeForward.right); } }

        [Header("Debug")]
        [SerializeField]
        private List<Attractor> _attractorApplyingForce = new List<Attractor>(Attractor.DEFAULT_MAX_ATTRACTOR);
        public List<Attractor> AttractorApplyingForce { get { return (_attractorApplyingForce); } }

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
            if (_attractorApplyingForce.Count == 0)
            {
                Debug.Log("no attraction...");
                _gravityDirection = Vector3.down;
                return;
            }

            if (_frequencySearchClosestPoint == 0)
            {
                _extGravitonCalculation.SetupGravityFields(_attractorApplyingForce, _rigidBody.position);
                _extGravitonCalculation.CalculateGravityFields();
            }
            else if (_coolDownCalculation.IsNotRunning())
            {
                _extGravitonCalculation.SetupGravityFields(_attractorApplyingForce, _rigidBody.position);
                _extGravitonCalculation.CalculateGravityFields();
                _coolDownCalculation.StartCoolDown(_frequencySearchClosestPoint);
            }
            _gravityDirection = _extGravitonCalculation.CalculateForces(_mass);
        }

        public void CalculatePhysicNormalIgnoringADirection(Vector3 directionToIgnore, float dotMargin)
        {
            if (_frequencySearchClosestPoint == 0)
            {
                _extGravitonCalculation.SetupGravityFields(_attractorApplyingForce, _rigidBody.position);
                _extGravitonCalculation.RemoveAttractorFromOneDirection(directionToIgnore, dotMargin);
                _extGravitonCalculation.CalculateGravityFields();
            }
            else if (_coolDownCalculation.IsNotRunning())
            {
                _extGravitonCalculation.SetupGravityFields(_attractorApplyingForce, _rigidBody.position);
                _extGravitonCalculation.RemoveAttractorFromOneDirection(directionToIgnore, dotMargin);
                _extGravitonCalculation.CalculateGravityFields();
                _coolDownCalculation.StartCoolDown(_frequencySearchClosestPoint);
            }
            _gravityDirection = _extGravitonCalculation.CalculateForces(_mass);
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