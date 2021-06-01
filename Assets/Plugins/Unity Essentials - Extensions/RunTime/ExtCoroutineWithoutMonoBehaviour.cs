using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Extensions
{
    /// <summary>
    /// this script create an empty gameObject, add to it a monobehaviour,
    /// and from there you can use a coroutine, without having a monobehaviour reference.
    /// </summary>
    public static class ExtCoroutineWithoutMonoBehaviour
    {
        private class EmptyMonoBehaviour : MonoBehaviour { }
        private static EmptyMonoBehaviour _coroutineRunner;
        private static GameObject _referenceEmptyGameObject;

        private static EmptyMonoBehaviour InitCoroutineRunner()
        {
            // Gameobject is not instantiated so it's not in the scene. It's just an object in memory.
            _referenceEmptyGameObject = new GameObject();
            _referenceEmptyGameObject.isStatic = true;
            _referenceEmptyGameObject.hideFlags = HideFlags.HideAndDontSave;
            return (_referenceEmptyGameObject.AddComponent<EmptyMonoBehaviour>());
        }

        public static void StartUniqueCoroutine(IEnumerator routine)
        {
            CleanUp();
            _coroutineRunner = InitCoroutineRunner();
            _coroutineRunner.StartCoroutine(routine);
        }

        public static void StopUniqueCoroutine(IEnumerator routine)
        {
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StopCoroutine(routine);
            }
        }

        public static void CleanUp()
        {
            if (_referenceEmptyGameObject != null)
            {
                GameObject.Destroy(_referenceEmptyGameObject);
            }
            if (_coroutineRunner != null)
            {
                _coroutineRunner = null;
            }
        }
    }
}
