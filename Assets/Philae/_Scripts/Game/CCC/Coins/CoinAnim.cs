
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Philae.CCC.Coins
{
    public class CoinAnim : MonoBehaviour
    {
        [SerializeField, Tooltip("ref rigidbody")]
        private float turnRate = 200f;

        [SerializeField, Tooltip("ref rigidbody")]
        private Transform objToRotate = null;

        private void RotateCoin()
        {
            objToRotate.Rotate(objToRotate.up, turnRate * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            RotateCoin();
        }
    }
}