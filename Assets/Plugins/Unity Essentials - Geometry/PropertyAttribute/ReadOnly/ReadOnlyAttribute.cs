using System;
using UnityEngine;

namespace UnityEssentials.Geometry.PropertyAttribute.ReadOnly
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : UnityEngine.PropertyAttribute
    {

    }
}