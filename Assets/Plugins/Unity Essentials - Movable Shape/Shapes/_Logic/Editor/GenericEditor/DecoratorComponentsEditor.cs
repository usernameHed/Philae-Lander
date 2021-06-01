/// <summary>
/// MIT License - Copyright(c) 2019 Ugo Belfiore
/// </summary>

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Geometry.MovableShape.editor
{
    public abstract class DecoratorComponentsEditor : UnityEditor.Editor
    {
        /// <summary>
        /// all the virtual function at your disposal
        /// </summary>
        #region virtual functions

        /// <summary>
        /// called when the inspector is enabled
        /// </summary>
        public virtual void OnCustomEnable() { }

        /// <summary>
        /// called when the inspector is disabled
        /// </summary>
        public virtual void OnCustomDisable() { }


        /// <summary>
        /// called every OnInspectorGUI inside the herited class
        /// </summary>
        protected virtual void OnCustomInspectorGUI() { }

        /// <summary>
        /// called at the first OnInspectorGUI inside the herited class
        /// </summary>
        protected virtual void InitOnFirstInspectorGUI() { }

        /// <summary>
        /// called at the first OnSceneGUI inside the herited class
        /// </summary>
        /// <param name="sceneview">current drawing scene view</param>
        protected virtual void CustomOnSceneGUI(SceneView sceneview) { }

        /// <summary>
        /// called at the first OnSceneGUI inside the herited class
        /// </summary>
        /// <param name="sceneview">current drawing scene view</param>
        protected virtual void InitOnFirstOnSceneGUI(SceneView sceneview) { }

        /// <summary>
        /// called every editor Update inside the herited class
        /// </summary>
        protected virtual void OnEditorUpdate() { }

        /// <summary>
        /// called at the first Editor Update inside the herited class
        /// </summary>
        protected virtual void InitOnFirstEditorUpdate() { }

        public virtual void ShowTinyEditorContent() { }

        protected virtual void OnHierarchyChanged() { }


        #endregion

        /// <summary>
        /// some private variable
        /// </summary>
        #region private variables

        /// EditorPref key for save the open/close extension
        private const string KEY_EXPAND_EDITOR_EXTENSION = "KEY_EXPAND_EDITOR_EXTENSION";
        private bool _openExtensions = false;    //is this extension currently open or close ? save this for every gameObjects's components
        protected bool IsExtensionOpened() => _openExtensions;
        private readonly object[] EMPTY_ARRAY = new object[0];   //empty array for invoking methods using reflection
        private System.Type _decoratedEditorType = null;       //Type object for the internally used (decorated) editor.
        private System.Type _editedObjectType;          //Type object for the object that is edited by this editor.
        private UnityEditor.Editor _editorInstance;                 //urrent editorInstance
        private bool _isCustomEditor = false;
        private string _nameCustomEditor;
        private bool _hasInitSceneGUI = false;
        private bool _hasInitEditorUpdate = false;
        private bool _hasInitInspector = false;
        private bool _showExtension = true;
        private bool _showTinyEditor = false;
        public TinyEditorWindowSceneView TinyEditorWindow;
        private readonly string KEY_EDITOR_PREF_EDITOR_WINDOW = "KEY_EDITOR_PREF_EDITOR_WINDOW";
        private string _tinyEditorName = "tiny Editor";
        public void ChangeNameEditor(string newName)
        {
            _tinyEditorName = newName;
            if (TinyEditorWindow != null)
            { 
                TinyEditorWindow.NameEditorWindow = newName;
            }
        }

        protected Component ConcretTarget;
        protected Component[] ConcretTargets;

        /// reflexion cache
        private Dictionary<string, MethodInfo> _decoratedMethods = new Dictionary<string, MethodInfo>();
        private Assembly _editorAssembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));

        /// <summary>
        /// get editor instance
        /// </summary>
        /// <returns>editor instance</returns>
        protected UnityEditor.Editor GetEditorInstance()
        {
            return _editorInstance;
        }

        #endregion

        /// <summary>
        /// all the required setup and initialisation
        /// </summary>
        #region initialisation editor

        public DecoratorComponentsEditor(bool showExtension = false, string tinyEditorName = "")
        {
            SetupDecoratorSettings(showExtension, tinyEditorName, true, "");
        }

        /// <summary>
        /// constructor of the class: here we initialize reflexion variables
        /// </summary>
        /// <param name="editorTypeName">name of the component we inspect</param>
        public DecoratorComponentsEditor(BUILT_IN_EDITOR_COMPONENTS editorTypeName, bool showExtension = true, string tinyEditorName = "")
        {
            SetupDecoratorSettings(showExtension, tinyEditorName, isCustomEditor: false, editorTypeName.ToString());
            SetupCustomEditor(editorTypeName);
        }

        private void SetupDecoratorSettings(bool showExtension, string tinyEditorName, bool isCustomEditor, string nameEditor)
        {
            _showExtension = showExtension;
            _showTinyEditor = !string.IsNullOrEmpty(tinyEditorName);
            _isCustomEditor = isCustomEditor;
            _nameCustomEditor = nameEditor;
            _tinyEditorName = tinyEditorName;
        }

        private void InitTinyEditorWindow(string tinyEditorName)
        {
            TinyEditorWindow = new TinyEditorWindowSceneView();
            string key = KEY_EDITOR_PREF_EDITOR_WINDOW + this.GetType().ToString();
            TinyEditorWindow.TinyInit(key, tinyEditorName, TinyEditorWindowSceneView.DEFAULT_POSITION.UP_LEFT);
            TinyEditorWindow.IsClosable = false;
            TinyEditorWindow.IsMinimisable = true;
        }

        private void SetupCustomEditor(BUILT_IN_EDITOR_COMPONENTS editorTypeName)
        {
            _decoratedEditorType = _editorAssembly.GetTypes().Where(t => t.Name == editorTypeName.ToString()).FirstOrDefault();
            Init();

            // Check CustomEditor types.
            System.Type originalEditedType = GetCustomEditorType(_decoratedEditorType);

            if (originalEditedType != _editedObjectType)
            {
                throw new System.ArgumentException(
                    string.Format("Type {0} does not match the editor {1} type {2}",
                              _editedObjectType, editorTypeName, originalEditedType));
            }
        }

        /// <summary>
        /// need to be called here to initialize the default editor
        /// </summary>
        private void OnEnable()
        {
            SetupEditor();
            InitTargets();
            OnCustomEnable();
        }

        /// <summary>
        /// need to destroy the customEditor when quitting !
        /// </summary>
        private void OnDisable()
        {
            if (_editorInstance != null)
            {
                DestroyImmediate(_editorInstance);
            }
            _hasInitSceneGUI = false;
            _hasInitEditorUpdate = false;
            _hasInitInspector = false;
            SceneView.duringSceneGui -= OwnOnSceneGUI;
            EditorApplication.update -= CustomEditorApplicationUpdate;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            OnCustomDisable();
        }

        public T GetTarget<T>() where T : Component
        {
            if (ConcretTarget == null)
            {
                return (default(T));
            }
            return ((T)ConcretTarget);
        }

        public GameObject GetGameObject()
        {
            return (ConcretTarget.gameObject);
        }
        public GameObject[] GetGameObjects()
        {
            if (ConcretTargets == null)
            {
                return (null);
            }
            GameObject[] gameObjects = new GameObject[ConcretTargets.Length];
            for (int i = 0; i < ConcretTargets.Length; i++)
            {
                gameObjects[i] = ConcretTargets[i].gameObject;
            }
            return (gameObjects);
        }

        public T[] GetTargets<T>() where T : Component
        {
            if (ConcretTarget == null)
            {
                return (default(T[]));
            }
            T[] array = new T[ConcretTargets.Length];
            for (int i = 0; i < ConcretTargets.Length; i++)
            {
                array[i] = (T)ConcretTargets[i];
            }
            return (array);
        }

        public T GetTargetIndex<T>(int index) where T : Component
        {
            if (ConcretTargets == null
                || ConcretTargets.Length == 0
                || index < 0
                || index >= ConcretTargets.Length)
            {
                return (default(T));
            }
            return ((T)ConcretTargets[index]);
        }

        /// <summary>
        /// return true if we have still the reference of the editor, but no more target...
        /// </summary>
        /// <returns></returns>
        public bool HaveWeLostTargets()
        {
            bool targetLost = ConcretTarget != null && target == null;
            bool concretTargetLost = ConcretTarget == null;
            bool contextLost = ConcretTarget != null && !Selection.gameObjects.Contains(ConcretTarget.gameObject);

            return (/*_isCustomEditor && */(targetLost || concretTargetLost || contextLost));
        }

        /// <summary>
        /// Need to destroy the instance even on Destroy !
        /// I don't know why. Unity generate leaks after each compilation
        /// because of the CreateEditor
        /// </summary>
        private void OnDestroy()
        {
            if (_editorInstance != null)
            {
                DestroyImmediate(_editorInstance);
            }
            //SceneView.duringSceneGui -= OwnOnSceneGUI;
            //EditorApplication.update -= OnEditorApplicationUpdate;
        }

        /// <summary>
        /// here create the editor instance, and open / close the extension panel if needed
        /// </summary>
        private void SetupEditor()
        {
            if (_editorInstance != null)
            {
                return;
            }
            _hasInitSceneGUI = false;
            _hasInitEditorUpdate = false;
            _hasInitInspector = false;
            CreateDefaultInstance();
            OpenOrCloseExtension();
        }


        /// <summary>
        /// here create the editor instance
        /// </summary>
        private void CreateDefaultInstance()
        {
            if (_decoratedEditorType == null || targets == null)
            {
                return;
            }
            _editorInstance = UnityEditor.Editor.CreateEditor(targets, _decoratedEditorType);
        }

        /// <summary>
        /// manage if the extensions are opened or closed, and save this setting in EditorPrefs
        /// </summary>
        private void OpenOrCloseExtension()
        {
            if (EditorPrefs.HasKey(KEY_EXPAND_EDITOR_EXTENSION + _nameCustomEditor))
            {
                _openExtensions = EditorPrefs.GetBool(KEY_EXPAND_EDITOR_EXTENSION + _nameCustomEditor);
            }
            else
            {
                _openExtensions = false;
                EditorPrefs.SetBool(KEY_EXPAND_EDITOR_EXTENSION + _nameCustomEditor, _openExtensions);
            }
        }

       

        /// <summary>
        /// iniitalisation of reflexion variables
        /// </summary>
        private void Init()
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var attributes = this.GetType().GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
            var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();

            _editedObjectType = field.GetValue(attributes[0]) as System.Type;
        }

        /// <summary>
        /// from a given type of editor, get his CustomEditor as type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private System.Type GetCustomEditorType(System.Type type)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var attributes = type.GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
            var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();

            return field.GetValue(attributes[0]) as System.Type;
        }

        /// <summary>
        /// From a given name of class, invoke it using reflexion
        /// </summary>
        protected void CallMethodUsingReflexion(string methodName)
        {
            MethodInfo method = null;

            // Add MethodInfo to cache
            if (!_decoratedMethods.ContainsKey(methodName))
            {
                var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

                method = _decoratedEditorType.GetMethod(methodName, flags);

                if (method != null)
                {
                    _decoratedMethods[methodName] = method;
                }
                else
                {
                    //Debug.LogError(string.Format("Could not find method {0}", method));
                }
            }
            else
            {
                method = _decoratedMethods[methodName];
            }

            if (method != null && GetEditorInstance() != null)
            {
                method.Invoke(GetEditorInstance(), EMPTY_ARRAY);
            }
        }

        #endregion

        /// <summary>
        /// The core loop function for display
        /// everything
        /// </summary>
        #region Display Extension

        /// <summary>
        /// here the main InspectorGUI.
        /// It's here that we call the base OnInspecctorGUI.
        /// Then we show the GUI --------- Extension [+]
        /// then if it's open, we call the abstract class CustomOnInspectorGUI()
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (!_isCustomEditor)
            {
                ManageUnityInspector();
            }
            else
            {
                ManageCustomNewInspector();
            }
        }
        private void ManageUnityInspector()
        {
            if (GetEditorInstance() == null)
            {
                return;
            }
            if (HaveWeLostTargets())
            {
                DestroyImmediate(GetEditorInstance());
                return;
            }

            GetEditorInstance().OnInspectorGUI();
            CommonExtenstionInspectorGUI();
        }
        private void ManageCustomNewInspector()
        {
            base.OnInspectorGUI();
            CommonExtenstionInspectorGUI();
        }

        /// <summary>
        /// called whatever this editor is inherited from a custom unity component,
        /// or just a standar sutom editor
        /// </summary>
        private void CommonExtenstionInspectorGUI()
        {
            TryToInitOnFirstInspectorGUI();

            if (_showExtension
                && ConcretTarget != null
                && ExpendBehavior())
            {
                OnCustomInspectorGUI();
            }
        }

        private void InitTargets()
        {
            if (ConcretTargets == null)
            {
                ConcretTarget = (Component)target;
                ConcretTargets = new Component[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    ConcretTargets[i] = (Component)targets[i];
                }

                //_meshFilterHiddedTools = _concretTarget.GetOrAddComponent<MeshFilterHiddedTools>();

                SceneView.duringSceneGui -= OwnOnSceneGUI;
                SceneView.duringSceneGui += OwnOnSceneGUI;
                EditorApplication.update -= CustomEditorApplicationUpdate;
                EditorApplication.update += CustomEditorApplicationUpdate;

                EditorApplication.hierarchyChanged -= OnHierarchyChanged;
                EditorApplication.hierarchyChanged += OnHierarchyChanged;
            }
        }

        /// <summary>
        /// init the targets inspected and store them.
        /// This is called at the first OnInspectorGUI()
        /// Not in the constructor ! we don't have targets information in the constructor
        /// </summary>
        private void TryToInitOnFirstInspectorGUI()
        {
            if (!_hasInitInspector)
            {
                InitOnFirstInspectorGUI();
                _hasInitInspector = true;
            }
        }

        private void TryToInitOnFirstOnEditorUpdate()
        {
            if (!_hasInitEditorUpdate)
            {
                InitOnFirstEditorUpdate();
                _hasInitEditorUpdate = true;
            }
        }

        /// <summary>
        /// editor update loop
        /// </summary>
        private void CustomEditorApplicationUpdate()
        {
            if (!_isCustomEditor)
            {
                if (GetEditorInstance() == null)
                {
                    return;
                }
            }
            if (HaveWeLostTargets())
            {
                DestroyImmediate(GetEditorInstance());
                return;
            }

            if (!IsExtensionOpened())
            {
                return;
            }
            TryToInitOnFirstOnEditorUpdate();
            OnEditorUpdate();
        }

        /// <summary>
        /// This function is called at each OnSceneGUI to try to initialize it
        /// </summary>
        private void TryToInitOnFirstOnSceneGUI(SceneView sceneview)
        {
            if (!_hasInitSceneGUI)
            {
                if (_showTinyEditor)
                {
                    InitTinyEditorWindow(_tinyEditorName);
                }

                InitOnFirstOnSceneGUI(sceneview);
                _hasInitSceneGUI = true;
            }
        }

        public void OwnOnSceneGUI(SceneView sceneview)
        {
            if (!_isCustomEditor)
            {
                if (GetEditorInstance() == null)
                {
                    return;
                }
            }
            if (HaveWeLostTargets())
            {
                DestroyImmediate(GetEditorInstance());
                return;
            }

            TryToInitOnFirstOnSceneGUI(sceneview);

            if (_showTinyEditor)
            {
                TinyEditorWindow.ShowEditorWindow(ShowTinyEditorContent, SceneView.currentDrawingSceneView, Event.current);
            }

            
            if (!IsExtensionOpened())
            {
                return;
            }
            

            CustomOnSceneGUI(sceneview);
        }

        /// <summary>
        /// display the Expand behavior of the component editor
        /// </summary>
        private bool ExpendBehavior()
        {
            GUIStyle lineStyle = new GUIStyle();
            lineStyle.normal.background = EditorGUIUtility.whiteTexture;
            lineStyle.stretchWidth = true;
            lineStyle.margin = new RectOffset(0, 0, 12, 18);

            var c = GUI.color;
            var p = GUILayoutUtility.GetRect(GUIContent.none, lineStyle, GUILayout.Height(1));
            p.width -= 120;
            if (Event.current.type == EventType.Repaint)
            {
                GUI.color = EditorGUIUtility.isProSkin ?
                    new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);
                lineStyle.Draw(p, false, false, false, false);
            }

            EditorGUI.LabelField(new Rect(p.xMax + 30, p.y - 7, 70, 25), "Extensions");

            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            if (GUI.Button(new Rect(p.xMax + 100, p.y - 7, 18, 16), (_openExtensions ? " -" : " +")))
            {
                _openExtensions = !_openExtensions;
                EditorPrefs.SetBool(KEY_EXPAND_EDITOR_EXTENSION + _nameCustomEditor, _openExtensions);
            }
            GUI.backgroundColor = Color.white;


            GUI.color = c;

            return (_openExtensions);
        }

        
        #endregion

        /// <summary>
        /// other basic unity fonction we have to manage
        /// </summary>
        #region Unity Fonctions
        public void OnSceneGUI()
        {
            if (GetEditorInstance() == null)
            {
                return;
            }
            CallMethodUsingReflexion("OnSceneGUI");
        }

        protected override void OnHeaderGUI()
        {
            if (GetEditorInstance() == null)
            {
                return;
            }
            CallMethodUsingReflexion("OnHeaderGUI");
        }

        public override void DrawPreview(Rect previewArea)
        {
            if (GetEditorInstance() == null)
            {
                return;
            }
            GetEditorInstance().DrawPreview(previewArea);
        }

        public override string GetInfoString()
        {
            if (GetEditorInstance() == null)
            {
                return ("");
            }
            return GetEditorInstance().GetInfoString();
        }

        public override GUIContent GetPreviewTitle()
        {
            if (GetEditorInstance() == null)
            {
                return (null);
            }
            return GetEditorInstance().GetPreviewTitle();
        }

        public override bool HasPreviewGUI()
        {
            if (GetEditorInstance() == null)
            {
                return (false);
            }
            return GetEditorInstance().HasPreviewGUI();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (GetEditorInstance() == null)
            {
                return;
            }
            GetEditorInstance().OnInteractivePreviewGUI(r, background);
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (GetEditorInstance() == null)
            {
                return;
            }
            GetEditorInstance().OnPreviewGUI(r, background);
        }

        public override void OnPreviewSettings()
        {
            if (GetEditorInstance() == null)
            {
                return;
            }
            GetEditorInstance().OnPreviewSettings();
        }

        public override void ReloadPreviewInstances()
        {
            if (GetEditorInstance() == null)
            {
                return;
            }
            GetEditorInstance().ReloadPreviewInstances();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (GetEditorInstance() == null)
            {
                return (null);
            }
            return GetEditorInstance().RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override bool RequiresConstantRepaint()
        {
            if (GetEditorInstance() == null)
            {
                return (false);
            }
            return GetEditorInstance().RequiresConstantRepaint();
        }

        public override bool UseDefaultMargins()
        {
            if (GetEditorInstance() == null)
            {
                return (true);
            }
            return GetEditorInstance().UseDefaultMargins();
        }

        #endregion
    }
}

