using UnityEditor;
using UnityEngine;
using UnityEssentials.Geometry.Editor;

namespace UnityEssentials.Geometry.MovableShape.editor
{
    [CustomEditor(typeof(MovableDonut), true)]
    public class MovableDonutEditor : MovableShapeEditor
    {
        protected MovableDonut _attractorDonut;

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorDonut = (MovableDonut)GetTarget<MovableShape>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.UpdateEditor();
            base.ShowTinyEditorContent();

            SerializedProperty donut = this.GetPropertie(nameof(MovableDonut.Donut));

            float thickNess = ExtDonutProperty.GetThickNess(donut);
            float radius = donut.GetRadius();

            if (radius < 0)
            {
                ExtDonutProperty.SetRadius(donut, 0);
                this.ApplyModification();
            }
            if (thickNess < 0 || thickNess > radius)
            {
                ExtDonutProperty.SetThickNess(donut, Mathf.Clamp(thickNess, 0, radius));
                this.ApplyModification();
            }
        }
    }
}