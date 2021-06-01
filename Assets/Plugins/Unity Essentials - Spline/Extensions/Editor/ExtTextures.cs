using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEssentials.Spline.Extensions.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtTexture
    {
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
