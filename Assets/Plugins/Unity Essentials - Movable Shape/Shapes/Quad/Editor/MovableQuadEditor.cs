using UnityEditor;
using UnityEngine;
using UnityEssentials.Geometry.Editor;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.Geometry.MovableShape.editor
{
    [CustomEditor(typeof(MovableQuad), true)]
    public class MovableQuadEditor : MovableShapeEditor
    {
        protected MovableQuad _attractorQuad;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public MovableQuadEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {

        }
        public MovableQuadEditor()
            : base(true, "")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorQuad = (MovableQuad)GetTarget<MovableShape>();
        }

        protected override void OnCustomInspectorGUI()
        {
            this.UpdateEditor();
            using (HorizontalScope scope = new HorizontalScope())
            {
                SerializedProperty quad = this.GetPropertie("_quad");

                bool allowDown = ExtQuadProperty.GetAllowDown(quad);

                bool allowDownChange = GUILayout.Toggle(allowDown, "Allow Down", EditorStyles.miniButton);
                if (allowDownChange != allowDown)
                {
                    ExtQuadProperty.SetAllowDown(quad, allowDownChange);
                    this.ApplyModification();
                }
            }
        }
    }
}