﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Spline.PropertyAttribute.NoNull
{
    /// <summary>
    /// Cannot Be Null will red-flood the field if the reference is null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NoNull : UnityEngine.PropertyAttribute
    {

    }
}