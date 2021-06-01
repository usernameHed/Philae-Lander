using UnityEngine;

namespace UnityEssentials.Extensions
{
    public static class ExtAnimationCurve
    {
        public enum EaseType
        {
            LINEAR,
            EASE_IN_OUT,
        }

        public static AnimationCurve GetCopyOf(AnimationCurve curve)
        {
            AnimationCurve newCurve = new AnimationCurve();
            for (int i = 0; i < curve.length; i++)
            {
                newCurve.AddKey(curve[i]);
            }
            return newCurve;
        }

        /// <summary>
        /// return a color from a string
        /// </summary>
        public static float GetMaxValue(this AnimationCurve animationCurve, ref int index)
        {
            if (animationCurve.length == 0)
            {
                Debug.LogWarning("no keys");
                index = -1;
                return 0;
            }

            index = 0;
            float maxValue = animationCurve[0].value;
            for (int i = 1; i < animationCurve.length; i++)
            {
                if (animationCurve[i].value > maxValue)
                {
                    maxValue = animationCurve[i].value;
                    index = i;
                }
            }
            return maxValue;
        }

        public static float GetMaxTime(this AnimationCurve animationCurve)
        {
            return (animationCurve[animationCurve.length - 1].time);
        }

        /// <summary>
        /// return a color from a string
        /// </summary>
        public static float GetMinValue(this AnimationCurve animationCurve, ref int index)
        {
            if (animationCurve.length == 0)
            {
                Debug.LogWarning("no keys");
                index = -1;
                return 0;
            }

            index = 0;
            float minValue = animationCurve[0].value;
            for (int i = 1; i < animationCurve.length; i++)
            {
                if (animationCurve[i].value < minValue)
                {
                    minValue = animationCurve[i].value;
                    index = i;
                }
            }
            return minValue;
        }

        public static AnimationCurve GetDefaultCurve(EaseType easeType = EaseType.LINEAR)
        {
            switch (easeType)
            {
                case EaseType.LINEAR:
                    AnimationCurve curveLinear = AnimationCurve.Linear(0, 0, 1, 1);// new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
                                                                                   //curveLinear.postWrapMode = WrapMode.Clamp;
                    return curveLinear;
                case EaseType.EASE_IN_OUT:
                    AnimationCurve curveEased = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    return curveEased;
            }
            return new AnimationCurve();
        }


        /// <summary>
        /// from a set of points corresponding to values, create and return an animationCuve
        /// </summary>
        /// <param name="time"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static AnimationCurve GetCurveFromDatas(float[] time, float[] datas)
        {
            if (time.Length == 0 || datas.Length == 0 || time.Length != datas.Length)
            {
                Debug.LogError("array not suitable");
                return null;
            }
            //AnimationCurve curve = ExtAnimationCurve.GetDefaultCurve(ExtAnimationCurve.EaseType.EASE_IN_OUT);
            AnimationCurve curve = new AnimationCurve();

            for (int i = 0; i < time.Length; i++)
            {

                curve.AddKey(new Keyframe(time[i], datas[i], 0, 0));
            }
            return curve;
        }

        public static AnimationCurve Scale(this AnimationCurve animationCurve, float newScale)
        {
            float maxLenght = animationCurve.GetMaxTime();
            float scaleFactor = newScale / maxLenght;
            Keyframe[] keys = animationCurve.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = new Keyframe(keys[i].time * scaleFactor, keys[i].value);
            }
            animationCurve.keys = keys;

            return (animationCurve);
        }

        /// <summary>
        /// inverse an Animation curve
        /// curve need to be strictly monotonic to be able to do that
        /// (NO 2 times the same value)
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float EvaluateInverseMonotonic(this AnimationCurve curve, float time)
        {
            AnimationCurve inverseCurve = ExtAnimationCurve.ReverseCurve(curve);
            return (inverseCurve.Evaluate(time));
        }

        public static AnimationCurve ReverseCurve(AnimationCurve c)
        {
            // TODO: check c is strictly monotonic and Piecewise linear, log error otherwise
            var rev = new AnimationCurve();
            for (int i = 0; i < c.keys.Length; i++)
            {
                var kf = c.keys[i];
                var rkf = new Keyframe(kf.value, kf.time);
                if (kf.inTangent < 0)
                {
                    rkf.inTangent = 1 / kf.outTangent;
                    rkf.outTangent = 1 / kf.inTangent;
                }
                else
                {
                    rkf.inTangent = 1 / kf.inTangent;
                    rkf.outTangent = 1 / kf.outTangent;
                }
                rev.AddKey(rkf);
            }
            return rev;
        }

        /// <summary>
        /// get the time, from a value
        /// </summary>
        /// <param name="c"></param>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static float EvaluateInverse(this AnimationCurve c, float value, float precision = 1e-6f)
        {
            float minTime = c.keys[0].time;
            float maxTime = c.keys[c.keys.Length - 1].time;
            float best = (maxTime + minTime) / 2;
            float bestVal = c.Evaluate(best);
            int it = 0;
            const int maxIt = 1000;
            float sign = Mathf.Sign(c.keys[c.keys.Length - 1].value - c.keys[0].value);
            while (it < maxIt && Mathf.Abs(minTime - maxTime) > precision)
            {
                if ((bestVal - value) * sign > 0)
                {
                    maxTime = best;
                }
                else
                {
                    minTime = best;
                }
                best = (maxTime + minTime) / 2;
                bestVal = c.Evaluate(best);
                it++;
            }
            return best;
        }
    }
}