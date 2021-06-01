using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnityEssentials.Spline.editor
{
    public class InspectorUtility
    {
        public static void RepaintGameView(UnityEngine.Object dirtyObject = null)
        {
            /*
#if UNITY_2019_1_OR_NEWER
            if (dirtyObject != null)
                EditorUtility.SetDirty(dirtyObject);

            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            Type type = assembly.GetType("UnityEditor.GameView");
            EditorWindow gameview = EditorWindow.GetWindow(type);
            if (gameview != null)
                gameview.Repaint();
            else
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#else
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
            */
        }
    }
}
