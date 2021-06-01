using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityEssentials.CrossSceneReference
{
    public class TestCrossScene : MonoBehaviour
    {
        [Space(10)]
        [SerializeField]
        private GuidReference _referenceLight1 = new GuidReference();

        private void Awake()
        {
            // set up a callback when the target is destroyed so we can remove references to the destroyed object
            _referenceLight1.OnGuidRemoved += ClearCache;
        }

        private void ClearCache()
        {
            Debug.Log("action when removed");
        }

        private void Update()
        {
            // simple example looking for our reference and spinning both if we get one.
            // due to caching, this only causes a dictionary lookup the first time we call it, so you can comfortably poll. 
            if (_referenceLight1.gameObject != null)
            {

            }
        }
    }
}