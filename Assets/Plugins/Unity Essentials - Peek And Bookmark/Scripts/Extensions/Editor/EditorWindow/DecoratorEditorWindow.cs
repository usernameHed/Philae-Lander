using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Peek.Extensions.Editor.editorWindow
{
    public abstract class DecoratorEditorWindow : EditorWindow
    {
        protected bool _allowScrolling = true;
        protected GUIStyle _textStyle = new GUIStyle();

        private bool _isFirstGUI = false;
        protected Vector2 _scrollPosition;
        private EditorChrono _refreshRateInspector = new EditorChrono();
        protected float _timeRefreshInspector = 1f;

        #region virtual function
        protected virtual void OnCustomEnable()
        {
            EditorApplication.update += UpdateEditor;
            //EditorPrefs.SetBool(EditorContants.EditorOpenPreference.KEY_EDITOR_PREF_SCRUB_IS_OPEN, true);
        }
        protected virtual void OnCustomDestroy()
        {
            EditorApplication.update -= UpdateEditor;
            //EditorPrefs.SetBool(EditorContants.EditorOpenPreference.KEY_EDITOR_PREF_SCRUB_IS_OPEN, false);
        }
        protected virtual void OnCustomDisable()
        {
            EditorApplication.update -= UpdateEditor;
        }

        /// <summary>
        /// override it with "new" keyword
        /// </summary>
        public static void ShowEditorWindow()
        {
            DecoratorEditorWindow window = EditorWindow.GetWindow<DecoratorEditorWindow>("DecoratorEditorWindow");
            window.InitConstructor();
        }

        protected virtual void InitOnCustomGUI() { }
        protected virtual void OnCustomGUI() { }

        protected virtual void UpdateEditor() { }

        #endregion


        protected void OnEnable()
        {
            OnCustomEnable();
        }

        private void OnDestroy()
        {
            OnCustomDestroy();
        }

        protected void OnDisable()
        {
            OnCustomDisable();
        }

        /// <summary>
        /// cosntructor
        /// </summary>
        public DecoratorEditorWindow()
        {

        }

        /// <summary>
        /// can't be a constructor
        /// </summary>
        public virtual void InitConstructor()
        {
            ActualizeOnCreationOrAfterCompiling();
            this.wantsMouseMove = true;
        }

        protected virtual void SetMinSize(Vector2 minSize)
        {
            this.minSize = minSize;
        }
        protected virtual void SetMinSize(float width, float height)
        {
            this.minSize = new Vector2(width, height);
        }

        protected virtual void SetMaxSize(Vector2 maxSize)
        {
            this.maxSize = maxSize;
        }

        protected virtual void SetMaxSize(float width, float height)
        {
            this.maxSize = new Vector2(width, height);
        }
        protected virtual void SetMaxWidth(float width)
        {
            this.maxSize = new Vector2(width, this.maxSize.y);
        }
        protected virtual void SetMaxHeight(float height)
        {
            this.maxSize = new Vector2(this.maxSize.x, height);
        }



        /// <summary>
        /// called every time we need to actualize the editor (at start, and after compilation)
        /// </summary>
        protected virtual void ActualizeOnCreationOrAfterCompiling()
        {
            _isFirstGUI = false;
        }

        private /*sealed */ void OnGUI()
        {
            if (!_isFirstGUI)
            {
                InitOnCustomGUI();
                _isFirstGUI = true;
            }
            bool allosScroll = _allowScrolling;

            if (allosScroll)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            }

            OnCustomGUI();

            if (allosScroll)
            {
                EditorGUILayout.EndScrollView();
            }
        }

        public void OnInspectorUpdate()
        {
            if (_timeRefreshInspector == 0)
            {
                Repaint();
            }
            else
            {
                // This will only get called 2 times per second.
                if (!_refreshRateInspector.IsRunning())
                {
                    Repaint();
                    _refreshRateInspector.StartChrono(_timeRefreshInspector);
                }
            }
        }
    }
}