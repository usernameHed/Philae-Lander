using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEssentials.Geometry.MovableShape;

namespace UnityEssentials.Geometry.MovableShape.editor
{
    [CustomEditor(typeof(MovableShape), true)]
    public class MovableShapeEditor : DecoratorComponentsEditor
    {
        protected MovableShape _attractor;

        private Transform _parent;

        /// <summary>
        /// here call the constructor of the CustomWrapperEditor class,
        /// by telling it who we are (a Transform Inspector)
        /// NOTE: if you want to decorate your own inspector, or decorate an inspector
        ///   witch doesn't have a Unity Editor, you can call base() without parametter:
        ///   : base()
        /// </summary>
        public MovableShapeEditor(bool showExtension, string tinyEditorName)
            : base(showExtension, tinyEditorName)
        {


        }

        public MovableShapeEditor()
            : base(false, "")
        {

        }

        /// <summary>
        /// you should use that function instead of OnEnable()
        /// </summary>
        public override void OnCustomEnable()
        {
            _attractor = GetTarget<MovableShape>();
            _parent = _attractor.transform.parent;
        }
    }
}