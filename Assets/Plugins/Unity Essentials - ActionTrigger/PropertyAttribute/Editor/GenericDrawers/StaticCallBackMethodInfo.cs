using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.ActionTrigger.PropertyAttribute.Generic
{
    [InitializeOnLoad]
    public static class StaticCallBackMethodInfo
    {
        private const int NUMBER_OF_FRAME_TO_WAIT_BEFORE_EXECUTE_FONCTIONS = 0;

        private class MethodsToCall
        {
            private MethodInfo _method;
            private object _targetObject;
            private object[] _parameters;
            public int CurrentFrameWaited { get; set; }

            public MethodsToCall(MethodInfo method, object targetObject, object[] parameters)
            {
                _method = method;
                _targetObject = targetObject;
                _parameters = parameters;
                CurrentFrameWaited = 0;
            }

            public void Invoke()
            {
                _method?.Invoke(_targetObject, _parameters);
            }
        }
        private static List<MethodsToCall> _methodsToCalls = new List<MethodsToCall>(20);

        static StaticCallBackMethodInfo()
        {
            _methodsToCalls.Clear();
            EditorApplication.update -= CustomUpdate;
            EditorApplication.update += CustomUpdate;
        }

        public static void CallAfterDelay(MethodInfo method, object targetObject, object[] parameters)
        {
            if (method == null)
            {
                return;
            }
            _methodsToCalls.Add(new MethodsToCall(method, targetObject, parameters));
        }

        private static void CustomUpdate()
        {
            for (int i = 0; i < _methodsToCalls.Count; i++)
            {
                _methodsToCalls[i].CurrentFrameWaited++;
                if (_methodsToCalls[i].CurrentFrameWaited > NUMBER_OF_FRAME_TO_WAIT_BEFORE_EXECUTE_FONCTIONS)
                {
                    _methodsToCalls[i].Invoke();
                    _methodsToCalls.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}