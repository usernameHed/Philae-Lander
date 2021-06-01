using System;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.onvalueChanged
{
    public class OnValueChangedAttribute : UnityEngine.PropertyAttribute
    {
        public string MethodName;
        public OnValueChangedAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}