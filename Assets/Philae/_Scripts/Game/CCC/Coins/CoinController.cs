
using Philae.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.PropertyAttribute.onvalueChanged;

namespace Philae.CCC.Coins
{
    public class CoinController : MonoBehaviour, IPooledObject, IKillable
    {
        [OnValueChanged(nameof(SetKinematic)), SerializeField, Tooltip("ref script")]
        private readonly bool isKinematic = false;

        [OnValueChanged(nameof(SetKinematic)), SerializeField, Tooltip("ref script")]
        private readonly bool autoRotateAtStart = true;

        [OnValueChanged(nameof(SetKinematic)), SerializeField, Tooltip("ref script")]
        private Rigidbody rb = default;


        private void Awake()
        {
            SetKinematic();
        }

        private void SetKinematic()
        {
            rb.isKinematic = isKinematic;
            if (autoRotateAtStart)
            {
                //here rotate

            }
        }

        private void FixedUpdate()
        {

        }

        public void OnObjectSpawn()
        {
            rb.transform.position = transform.position;
            //throw new System.NotImplementedException();
        }

        public void OnDesactivePool()
        {
            //throw new System.NotImplementedException();
        }

        public void Kill()
        {
            Destroy(gameObject);
            //throw new System.NotImplementedException();
        }

        public void GetHit(int amount, Vector3 posAttacker)
        {
            //throw new System.NotImplementedException();
        }
    }
}