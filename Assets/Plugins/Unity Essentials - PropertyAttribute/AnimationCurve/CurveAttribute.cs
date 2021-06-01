using UnityEngine;

namespace UnityEssentials.PropertyAttribute.animationCurve
{
    /// <summary>
    /// usage:
    /// [SerializeField, Curve(0, 1, 0, 100)]
    /// private AnimationCurve _curvePercentage = new AnimationCurve();
    /// </summary>
    public class CurveAttribute : UnityEngine.PropertyAttribute
    {
        public float XMaxValue = 0f;
        public float XMinValue = 0f;
        public float YMaxValue = 0f;
        public float YMinValue = 0f;

        public string XMinVariable = "";
        public string XMaxVariable = "";
        public string YMinVariable = "";
        public string YMaxVariable = "";

        public CurveAttribute(float xMin, float xMax, float yMin, float yMax)
        {
            this.XMinValue = xMin;
            this.XMaxValue = xMax;
            this.YMinValue = yMin;
            this.YMaxValue = yMax;
        }
        
        public CurveAttribute(string xMinVariable, string xMaxVariable, string yMinVariable, string yMaxVariable)
        {
            this.XMinVariable = xMinVariable;
            this.XMaxVariable = xMaxVariable;
            this.YMinVariable = yMinVariable;
            this.YMaxVariable = yMaxVariable;
        }

        public CurveAttribute(float xMin, string xMaxVariable, float yMin, float yMax)
        {
            this.XMinValue = xMin;
            this.XMaxVariable = xMaxVariable;
            this.YMinValue = yMin;
            this.YMaxValue = yMax;
        }
        public CurveAttribute(float xMin, float xMax, float yMin, string yMaxVariable)
        {
            this.XMinValue = xMin;
            this.XMaxValue = xMax;
            this.YMinValue = yMin;
            this.YMaxVariable = yMaxVariable;
        }
        public CurveAttribute(float xMin, string xMaxVariable, float yMin, string yMaxVariable)
        {
            this.XMinValue = xMin;
            this.XMaxVariable = xMaxVariable;
            this.YMinValue = yMin;
            this.YMaxVariable = yMaxVariable;
        }
        
    }
}