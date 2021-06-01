using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEssentials.Extensions
{
    public static class ExtRect
    {
        public static Rect GetRectFromVector4(Vector4 vector)
        {
            return (new Rect(vector.x, vector.y, vector.z, vector.w));
        }

        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        public static Vector2 Middle(this Rect rect)
        {
            return new Vector2(rect.xMin - rect.xMax, rect.yMin - rect.yMax);
        }

        public static Vector3 GetMiddlePosition(this RectTransform trans)
        {
            Vector3 pos = new Vector3();
            Vector3[] corners = new Vector3[4];
            trans.GetWorldCorners(corners);
            // 0 - left bottom
            // 1 - left top
            // 2 - right top
            // 3 - right bottom
            pos.x = corners[1].x + ((corners[2].x - corners[1].x) / 2);
            pos.y = corners[1].y + ((corners[0].y - corners[1].y) / 2);
            return pos;
        }


        public static Rect ScaleSizeBy(this Rect rect, float scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }
        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
        {
            return rect.ScaleSizeBy(scale, rect.center);
        }
        public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale.x;
            result.xMax *= scale.x;
            result.yMin *= scale.y;
            result.yMax *= scale.y;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }



        public static Rect SetBetween(this Rect rect, Vector2 pos, Vector2 pos2)
        {
            rect.Set(pos.x, pos.y, pos2.x - pos.x, pos2.y - pos.y);
            return new Rect(rect);
        }

        /// <summary>
        /// Sets x/y
        /// </summary>
        public static Rect SetPosition(this Rect rect, Vector2 pos)
        {
            rect.x = pos.x;
            rect.y = pos.y;
            return new Rect(rect);
        }

        /// <summary>
        /// Sets x/y
        /// </summary>
        public static Rect SetPosition(this Rect rect, float x, float y)
        {
            rect.x = x;
            rect.y = y;
            return new Rect(rect);
        }

        /// <summary>
        /// gets width/height as Vector2
        /// </summary>
        public static Vector2 GetSize(this Rect rect)
        {
            return new Vector2(rect.width, rect.height);
        }

        public static Rect ShiftBy(this Rect rect, int x, int y)
        {
            rect.x += (float)x;
            rect.y += (float)y;
            return new Rect(rect);
        }

        public static Rect Include(this Rect rect, Rect other)
        {
            Rect r = new Rect();
            r.xMin = Mathf.Min(rect.xMin, other.xMin);
            r.xMax = Mathf.Max(rect.xMax, other.xMax);
            r.yMin = Mathf.Min(rect.yMin, other.yMin);
            r.yMax = Mathf.Max(rect.yMax, other.yMax);
            return r;
        }

        public static void SetRectX(this RectTransform rectTransform, float x)
        {
            rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
        }

        public static float GetRectX(this RectTransform rectTransform)
        {
            return (rectTransform.anchoredPosition.x);
        }

        public static Vector2 GetRectXY(this RectTransform rectTransform)
        {
            return (rectTransform.anchoredPosition);
        }

        public static void SetRectXY(this RectTransform rectTransform, float x, float y)
        {
            rectTransform.anchoredPosition = new Vector2(x, y);
        }

        public static void SetRectXY(this RectTransform rectTransform, Vector2 newPosition)
        {
            rectTransform.anchoredPosition = newPosition;
        }

        public static void SetRectY(this RectTransform rectTransform, float y)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
        }
        public static float GetRectY(this RectTransform rectTransform)
        {
            return (rectTransform.anchoredPosition.y);
        }
        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        }
        public static float GetWidth(this RectTransform rectTransform)
        {
            return (rectTransform.sizeDelta.x);
        }

        public static float GetRectWidth(this RectTransform rectTransform)
        {
            return (rectTransform.rect.width);
        }

        public static float GetRectHeight(this RectTransform rectTransform)
        {
            return (rectTransform.rect.height);
        }

        public static void SetOffsetMaxX(this RectTransform rectTransform, float maxX)
        {
            rectTransform.offsetMax = new Vector2(maxX, rectTransform.offsetMax.y);
        }

        public static void SetOffsetMaxY(this RectTransform rectTransform, float maxY)
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, maxY);
        }

        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }
        public static float GetHeight(this RectTransform rectTransform)
        {
            return (rectTransform.sizeDelta.y);
        }

        public static void SetSize(this RectTransform rectTransform, float width, float height)
        {
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        private static void SetToFillAmountPositonX(RectTransform parent, RectTransform cursor, Image fill)
        {
            float minX = parent.rect.x;
            float maxX = minX + parent.rect.width;
            float size = maxX - minX;
            float currentX = (fill.fillAmount * size / 1f) - (size / 2);
            cursor.SetX(currentX, false);
        }

        /// <summary>
        /// for a given rect, if width or height are negative, inverse x or y
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rect ReverseRectIfNeeded(Rect rect)
        {
            float width = rect.width;
            float height = rect.height;

            if (width < 0)
            {
                rect.x += width;
                rect.width = width * -1;
            }
            if (height < 0)
            {
                rect.y += height;
                rect.height = height * -1;
            }
            return rect;
        }

        public static bool Is3dPointInside2dRectInScreenSpace(Camera camera, Rect rect, Vector3 point)
        {
            Vector2 point2d = camera.WorldToScreenPoint(point);
            bool xOk = point2d.x >= rect.x && point2d.x <= rect.x + rect.width;

            float yInverse = ExtMathf.MirrorFromInterval(point2d.y, 0, camera.pixelHeight);
            bool yOk = yInverse >= rect.y && yInverse <= rect.y + rect.height;
            bool isInside = xOk && yOk;
            return (isInside);
        }

        /// <summary>
        /// Set a gameObject
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="cursor"></param>
        /// <param name="fill"></param>
        /// <param name="floatRange">Min: from 0 to 1, 0 is nother, 1 is Max</param>
        public static void SetToFillAmountPositonX(RectTransform parent, RectTransform cursor, float fillAmount, float minPercentOffset = 0, float maxPercentOffset = 0)
        {
            float minX = parent.rect.x;
            float maxX = (minX + parent.rect.width);
            float size = maxX - minX;

            float percentMinToRemove = minPercentOffset * size / 1f;
            float percentMaxToRemove = maxPercentOffset * size / 1f;

            float currentX = (fillAmount * size / 1f) - (size * parent.pivot.x);
            if (fillAmount < minPercentOffset)
            {
                currentX = minX + percentMinToRemove;
            }
            if (fillAmount > 1 - maxPercentOffset)
            {
                float pivot = parent.pivot.x == 0 ? 1 : parent.pivot.x;
                currentX = (size * pivot) - percentMaxToRemove;
            }

            cursor.SetX(currentX, false);
        }

        public static void CalculateHeightHomotetie(this RectTransform parent, float originalWidth, float originalHeight, float newWidth)
        {
            parent.SetHeight(originalHeight / originalWidth * newWidth);
        }
        public static void CalculateWidthHomotetie(this RectTransform parent, float originalWidth, float originalHeight, float newHeight)
        {
            parent.SetWidth(originalWidth / originalHeight * newHeight);
        }
        public static void CalculateHeightHomotetieWithClamp(this RectTransform parent, float originalWidth, float originalHeight, float newWidth, float maxHeight)
        {
            parent.SetHeight(Mathf.Min(originalHeight / originalWidth * newWidth, maxHeight));
        }
        public static void CalculateWidthHomotetieWithClamp(this RectTransform parent, float originalWidth, float originalHeight, float newHeight, float maxWidth)
        {
            parent.SetWidth(Mathf.Min(originalWidth / originalHeight * newHeight, maxWidth));
        }

        public static Rect Add(Rect one, Rect two)
        {
            return (new Rect(one.x + two.x, one.y + two.y, one.width + two.width, one.height + two.height));
        }

        public static Rect LerpUnclamped(Rect one, Rect two, float ease)
        {
            one.x = Mathf.LerpUnclamped(one.x, two.x, ease);
            one.y = Mathf.LerpUnclamped(one.y, two.y, ease);
            one.width = Mathf.LerpUnclamped(one.width, two.width, ease);
            one.height = Mathf.LerpUnclamped(one.height, two.height, ease);
            return (one);
        }

        public static Rect Lerp(Rect one, Rect two, float ease)
        {
            one.x = Mathf.Lerp(one.x, two.x, ease);
            one.y = Mathf.Lerp(one.y, two.y, ease);
            one.width = Mathf.Lerp(one.width, two.width, ease);
            one.height = Mathf.Lerp(one.height, two.height, ease);
            return (one);
        }

        public static Rect Set(this Rect rect, Vector2 pos, Vector2 size)
        {
            rect.Set(pos.x, pos.y, size.x, size.y);
            return new Rect(rect);
        }

        public static void Set(this RectTransform parent, float posX, float posY, float sizeX, float sizeY)
        {
            parent.SetRectXY(posX, posY);
            parent.sizeDelta = new Vector2(sizeX, sizeY);
        }

        public static void Set(this RectTransform parent, Rect rect)
        {
            parent.SetRectXY(rect.x, rect.y);
            parent.sizeDelta = new Vector2(rect.width, rect.height);
        }

        public static Rect GetWorldRect(this RectTransform rt, Vector2 scale)
        {
            // Convert the rectangle to world corners and grab the top left
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector3 topLeft = corners[0];

            // Rescale the size appropriately based on the current Canvas scale
            Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);

            return new Rect(topLeft, scaledSize);
        }

        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static Rect GetStretchRect(this RectTransform rt)
        {
            return (new Rect(rt.offsetMin.x, rt.offsetMax.y, rt.offsetMax.x, rt.offsetMin.y));
        }

        public static void SetStretchRect(this RectTransform rt, Rect stretchedRect)
        {
            rt.offsetMin = new Vector2(stretchedRect.x, stretchedRect.height);
            rt.offsetMax = new Vector2(stretchedRect.width, stretchedRect.y);
        }

        /*
        TODO: fin a way to automaticly calculate this
        public static Rect GetRelativeRect(this RectTransform rt)
        {
            Rect rect = new Rect();
            rt.

            bool isLeftStretch = (rt.anchorMin.x < 1 && rt.anchorMax.x > 0) || (rt.anchorMin.x < 1 && rt.anchorMax.x > 0);

            rect.x = (rt.anchorMin.x < 1 && rt.anchorMax.x > 0) ? rt.offsetMin.x : rt.GetRectX();
            rect.y = (rt.anchorMin.x < 1 && rt.anchorMax.x > 0) ? rt.offsetMax.y : rt.GetWidth();

            rect.width = (rt.anchorMax.x < 1 && rt.anchorMin.x > 0) ? rt.offsetMax.x : rt.GetRectX();
            rect.height = (rt.anchorMax.x < 1 && rt.anchorMin.x > 0) ? rt.offsetMin.y : rt.GetWidth();
            return (rect);
        }

        public static void SetRelativeRect(this RectTransform rt, Rect relativeRect)
        {
            
        }
        */

        public static void MoveToParentAndFullStretch(this Transform currentTransform, RectTransform parent)
        {
            RectTransform rt = currentTransform.GetComponent<RectTransform>();
            rt.MoveToParentAndFullStretch(parent);
        }

        public static void MoveToParentAndFullStretch(this Transform currentTransform, Transform parent)
        {
            RectTransform rt = currentTransform.GetComponent<RectTransform>();
            rt.MoveToParentAndFullStretch(parent);
        }

        public static void MoveToParentAndFullStretch(this RectTransform rt, RectTransform parent)
        {
            MoveToParentAndFullStretch(rt, parent.transform);
        }

        public static void MoveToParentAndFullStretch(this RectTransform rt, Transform parent)
        {
            if (rt == null)
            {
                Debug.LogWarning("null ? it shouldn't!");
                return;
            }
            rt.SetParent(parent);
            rt.SetStretchRect(new Rect(0, 0, 0, 0));
            rt.transform.localScale = Vector3.one;
        }
    }
}