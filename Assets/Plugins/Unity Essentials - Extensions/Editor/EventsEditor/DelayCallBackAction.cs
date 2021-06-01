using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEssentials.Extensions.Editor.EventEditor
{
    [InitializeOnLoad]
    public static class DelayCallBackAction
    {
        private const int NUMBER_OF_FRAME_TO_WAIT_BEFORE_EXECUTE_FONCTIONS = 0;

        private class MethodsToCall
        {
            private UnityAction _method;
            public int CurrentFrameWaited { get; set; }

            public MethodsToCall(UnityAction method)
            {
                _method = method;
                CurrentFrameWaited = 0;
            }

            public void Invoke()
            {
                _method?.Invoke();
            }
        }
        private static List<MethodsToCall> _methodsToCalls = new List<MethodsToCall>(20);

        static DelayCallBackAction()
        {
            _methodsToCalls.Clear();
            EditorApplication.update -= CustomUpdate;
            EditorApplication.update += CustomUpdate;
        }

        public static void CallAfterDelay(UnityAction method)
        {
            if (method == null)
            {
                return;
            }
            _methodsToCalls.Add(new MethodsToCall(method));
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