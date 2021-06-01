using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Geometry.Editor;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.Geometry.MovableShape.editor
{
    [CustomEditor(typeof(MovableDisc), true)]
    public class MovableDiscEditor : MovableShapeEditor
    {
        protected MovableDisc _attractorDisc;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public MovableDiscEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {

        }
        public MovableDiscEditor()
            : base(true, "")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorDisc = (MovableDisc)GetTarget<MovableShape>();
        }

        protected override void OnCustomInspectorGUI()
        {
            this.UpdateEditor();
            using (HorizontalScope scope = new HorizontalScope())
            {
                SerializedProperty disc = this.GetPropertie(nameof(MovableDisc.Disc));

                bool allowDown = ExtDiscProperty.GetAllowDown(disc);

                bool allowDownChange = GUILayout.Toggle(allowDown, "Allow Down", EditorStyles.miniButton);
                if (allowDownChange != allowDown)
                {
                    ExtDiscProperty.SetAllowDown(disc, allowDownChange);
                    this.ApplyModification();
                }
            }
        }
    }
}