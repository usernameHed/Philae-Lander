using System;
using UnityEngine;

namespace UnityEssentials.Geometry.PropertyAttribute.OnvalueChanged
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