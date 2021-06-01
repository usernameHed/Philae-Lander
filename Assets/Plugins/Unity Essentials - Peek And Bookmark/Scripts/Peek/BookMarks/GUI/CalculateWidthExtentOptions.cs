using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Peek.gui
{
    public class CalculateWidthExtentOptions
    {
        public float DefaultPercentMin { get; private set; }
        public int MaxItems { get; private set; }
        public float WidthScope { get; private set; }
        public int Index { get; private set; }
        public int NumberItem { get; private set; }
        public AnimationCurve Ease { get; private set; }

        public CalculateWidthExtentOptions(float defaultPercentMin, int maxItems, AnimationCurve ease)
        {
            DefaultPercentMin = defaultPercentMin;
            MaxItems = maxItems;
            Ease = ease;
        }

        public float CalculateWidthExtent(float widthScope, int index, int numberItem)
        {
            WidthScope = widthScope;
            Index = index;
            NumberItem = numberItem;
            return (CalculateWidthExtent());
        }

        private float CalculateWidthExtent()
        {
            int index = Index;

            if (NumberItem - Index > MaxItems)
            {
                index -= (NumberItem - MaxItems);
                index = Mathf.Clamp(index, 0, MaxItems);
            }
            else
            {
                index -= NumberItem - MaxItems;
            }

            int currentMaxItem = Mathf.Min(MaxItems, NumberItem);
            float currentPercent = 1 - (index * 1f / currentMaxItem);


            float percentRemapped = Remap(currentPercent, 0f, 1, 0f, DefaultPercentMin);
            float percentEased = Ease.Evaluate(percentRemapped);

            float widthExtent = WidthScope / 1f * percentEased;
            float halfExtent = widthExtent / 2f;

            return (halfExtent);
        }

        public float CalculateButtonWidthWithoutExtent(float widthExtent, float heightLine, float margin)
        {
            float extentLeft = widthExtent;
            float bookMarkButton = GenericPeekListDrawer.WIDTH_BUTTON_HOVER;
            float iconButton = heightLine;
            float deleteButton = GenericPeekListDrawer.WIDTH_BUTTON_HOVER;
            float extentRight = widthExtent;
            float spacingExtra = margin * ((widthExtent > 0) ? 10 : 5);
            float dotted = 30f;

            float finalOffset = extentLeft + bookMarkButton + iconButton + deleteButton + extentRight + spacingExtra + dotted;
            return (finalOffset);
        }

        public float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}