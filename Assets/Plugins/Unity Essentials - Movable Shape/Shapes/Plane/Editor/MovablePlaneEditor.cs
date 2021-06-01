using UnityEditor;
using UnityEngine;
using UnityEssentials.Geometry.Editor;
using static UnityEditor.EditorGUILayout;

namespace UnityEssentials.Geometry.MovableShape.editor
{
    [CustomEditor(typeof(MovablePlane), true)]
    public class MovablePlaneEditor : MovableShapeEditor
    {
        protected MovablePlane _attractorPlane;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public MovablePlaneEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {

        }
        public MovablePlaneEditor()
            : base(true, "")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            base.OnCustomEnable();
            _attractorPlane = (MovablePlane)GetTarget<MovableShape>();
        }

        protected override void OnCustomInspectorGUI()
        {
            this.UpdateEditor();
            using (HorizontalScope scope = new HorizontalScope())
            {
                SerializedProperty plane3d = this.GetPropertie(ExtPlaneProperty.PROPERTY_PLANE_3D);

                bool allowDown = ExtPlaneProperty.GetAllowDown(plane3d);

                bool allowDownChange = GUILayout.Toggle(allowDown, "Allow Down", EditorStyles.miniButton);
                if (allowDownChange != allowDown)
                {
                    ExtPlaneProperty.SetAllowDown(plane3d, allowDownChange);
                    this.ApplyModification();
                }
            }
        }
    }
}