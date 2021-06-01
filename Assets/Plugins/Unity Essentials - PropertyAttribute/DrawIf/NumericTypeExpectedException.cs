﻿using System;
using System.Runtime.Serialization;

namespace UnityEssentials.PropertyAttribute.drawIf
{
    /// <summary>
    /// An exception that is thrown whenever a numeric type is expected as an input somewhere but the input wasn't numeric.
    /// </summary>
    [Serializable]
    public class NumericTypeExpectedException : Exception
    {
        public NumericTypeExpectedException()
        {

        }

        public NumericTypeExpectedException(string message) :
            base(message)
        {

        }

        public NumericTypeExpectedException(string message, Exception inner) :
            base(message, inner)
        {

        }

        protected NumericTypeExpectedException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {

        }
    }
}