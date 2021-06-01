using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor.EventEditor
{
    [InitializeOnLoad]
    public static class CallBackToolRotationEditor
    {
        public delegate void EventToolRotationChange(PivotRotation current);
        public static event EventToolRotationChange OnToolRotationChange;
        
        private static PivotRotation _pivotRotation;

        static CallBackToolRotationEditor()
        {
            _pivotRotation = Tools.pivotRotation;
            EditorApplication.update += CatchToolTipChange;
        }

        private static void CatchToolTipChange()
        {
            if (Tools.pivotRotation != _pivotRotation)
            {
                OnToolRotationChange?.Invoke(Tools.pivotRotation);
            }
            _pivotRotation = Tools.pivotRotation;
        }
    }
}