using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Geometry.PropertyAttribute.ReadOnly;

namespace UnityEssentials.Geometry.shape2d
{
    /// <summary>
    /// //                row (horizontal)
    //               |  0   1   2 
    //            ---+------------
    //            0  | m00 m10 m20     //GetColumn(0)
    // column     1  | m01 m11 m21     //GetColumn(1)
    // (vertical) 2  | m02 m12 m22     //GetColumn(2)
    /// </summary>
    [Serializable]
    public struct Matrix3x3
    {
        [SerializeField, ReadOnly]
        private float[,] _values;

        public Matrix3x3(float [,] toFill)
        {
            _values = toFill;
        }

        public Matrix3x3(int colomn, int row)
        {
            _values = new float[colomn, row];
        }

        public float this[int colomn, int row]    // Indexer declaration  
        {
            get
            {
                return _values[colomn, row];
            }

            set
            {
                _values[colomn, row] = value;
            }
        }
    }
}