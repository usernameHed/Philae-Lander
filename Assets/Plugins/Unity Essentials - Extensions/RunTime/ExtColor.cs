using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEssentials.Hypersampling.Extensions
{
    public static class ExtColor
    {
        //all colors
        public static Color orangeBright { get { return (Color255(255, 82, 3, 1)); } }
        public static Color transparent { get { return (Color255(0, 0, 0, 0)); } }
        public static Color grayLight { get { return (Color255(200, 200, 200, 1)); } }
        public static Color grayLight2 { get { return (Color255(220, 220, 220, 1)); } }
        public static Color grayHalfTransparent { get { return (Color255(200, 200, 200, 0.8f)); } }
        public static Color whiteHalfTransparent { get { return (Color255(255, 255, 255, 0.7f)); } }
        public static Color grayInspectorDark { get { return (Color255(108, 108, 108, 1)); } }
        public static Color grayInspector { get { return (Color255(194, 194, 194, 1)); } }
        public static Color greenBlack { get { return (Color255(8, 140, 0, 1)); } }
        public static Color purple { get { return (Color255(156, 141, 221, 1)); } }
        public static Color cyanBlue { get { return (Color255(66, 133, 244, 0.5f)); } }

        public static Color greenLight { get { return (Color255(0, 255, 0, 0.5f)); } }
        public static Color redLight { get { return (Color255(255, 0, 0, 0.5f)); } }
        public static Color redSoft { get { return (Color255(255, 0, 0, 0.1f)); } }

        public static Color none { get { return (Color255(0, 0, 0, 0)); } }


        /// <summary>
        /// return a color from a string
        /// </summary>
        public static Color ColorHTML(string color)
        {
            Color newCol;
            ColorUtility.TryParseHtmlString(color, out newCol);
            return (newCol);
        }

        public static string ToHtml(this Color c)
        {
            Color32 col = c;
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { col.r, col.g, col.b, col.a });
        }

        /// <summary>
        /// return a color, based on r, g, b, a variable from 0 to 255
        /// use: Color newColor = ExtColor.Color255(25, 255, 0, 1);
        /// </summary>
        /// <param name="r">from 0 to 255</param>
        /// <param name="g">from 0 to 255</param>
        /// <param name="b">from 0 to 255</param>
        /// <param name="a">from 0 to 1</param>
        /// <returns></returns>
        public static Color Color255(float r, float g, float b, float a)
        {
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a);
        }

        /// <summary>
        /// only change the alpha of a color
        /// GUI.color = desiredColor.WithAlpha(0.5f);
        /// </summary>
        /// <param name="a">from 0 to 1</param>
        public static Color ChangeAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// Direct speedup of <seealso cref="Color.Lerp"/>
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color Lerp(Color c1, Color c2, float value)
        {
            if (value > 1.0f)
                return c2;
            if (value < 0.0f)
                return c1;
            return new Color(c1.r + (c2.r - c1.r) * value,
                             c1.g + (c2.g - c1.g) * value,
                             c1.b + (c2.b - c1.b) * value,
                             c1.a + (c2.a - c1.a) * value);
        }

        public static Color Color(this int n)
        {
            return n.Color32();
        }

        /// <summary>
        /// return a color from hex
        /// </summary>
        /// <param name="color">#FF21D0 for exemple</param>
        /// <param name="defaultColor"></param>
        /// <returns></returns>
        public static Color ColorHex(string color, Color defaultColor)
        {
            Color newColor;
            if (ColorUtility.TryParseHtmlString(color, out newColor))
            {
                return (newColor);
            }
            return (defaultColor);
        }

        public static Color32 Color32(this int n)
        {
            return new Color32((byte)((n >> 16) & 0xff), (byte)((n >> 8) & 0xff), (byte)((n >> 0) & 0xff), 0xff);
        }



        public static Color R(this Color c, float r)
        {
            c.r = r;
            return c;
        }

        public static Color G(this Color c, float g)
        {
            c.g = g;
            return c;
        }

        public static Color B(this Color c, float b)
        {
            c.b = b;
            return c;
        }

        public static Color A(this Color c, float a)
        {
            c.a = a;
            return c;
        }

        public static Color R(this Color c, Func<float, float> f)
        {
            c.r = f(c.r);
            return c;
        }

        public static Color G(this Color c, Func<float, float> f)
        {
            c.g = f(c.g);
            return c;
        }

        public static Color B(this Color c, Func<float, float> f)
        {
            c.b = f(c.b);
            return c;
        }

        public static Color A(this Color c, Func<float, float> f)
        {
            c.a = f(c.a);
            return c;
        }
    }
}