using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor
{
    /// <summary>
    /// exemple usage:
    /// 
    /// Open scene view:
    /// ExtEditorWindow.OpenEditorWindow(ExtEditorWindow.AllNameAssemblyKnown.SceneView, out Type animationWindowType);
    /// 
    /// Open animationWindow:
    /// EditorWindow animationWindowEditor = ExtEditorWindow.OpenEditorWindow(nameEditorWindow, out System.Type animationWindowType);
    /// 
    /// </summary>
    public static class ExtEditorWindow
    {
        /// <summary>
        /// Open of focus an editor Window
        /// if canDuplicate = true, duplicate the window if it existe
        /// </summary>
        public static T OpenEditorWindow<T>(bool canDuplicate = false) where T : EditorWindow
        {
            T editorWindow;

            if (canDuplicate)
            {
                editorWindow = ScriptableObject.CreateInstance<T>();
                editorWindow.Show();
                return (editorWindow);
            }

            // create a new instance
            editorWindow = EditorWindow.GetWindow<T>();

            // show the window
            editorWindow.Show();
            return (editorWindow);
        }

        public static EditorWindow FocusedWindow()
        {
            return (EditorWindow.focusedWindow);
        }

        public static void CloseEditorWindow<T>() where T : EditorWindow
        {
            T raceTrackNavigator = EditorWindow.GetWindow<T>();
            raceTrackNavigator.Close();
        }

        /// <summary>
        /// from a given name, Open an EditorWindow.
        /// you can also do:
        /// EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        /// to open an known EditorWindow
        /// 
        /// To get the type of a known script, like UnityEditor.SceneHierarchyWindow, you can do also:
        /// System.Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        /// </summary>
        /// <param name="editorWindowName">name of the editorWindow to open</param>
        /// <param name="animationWindowType">type of the editorWindow (useful for others functions)</param>
        /// <returns></returns>
        public static EditorWindow OpenEditorWindow(AllNameAssemblyKnown editorWindowName, out System.Type animationWindowType)
        {
            animationWindowType = GetEditorWindowTypeByName(editorWindowName.ToString());
            EditorWindow animatorWindow = EditorWindow.GetWindow(animationWindowType);
            return (animatorWindow);
        }

        #region reflection
        /// <summary>
        /// for adding, do a GetAllEditorWindowTypes(true);
        /// </summary>
        public enum AllNameAssemblyKnown
        {
            BuildPlayerWindow,
            ConsoleWindow,
            IconSelector,
            ObjectSelector,
            ProjectBrowser,
            ProjectTemplateWindow,
            RagdollBuilder,
            SceneHierarchySortingWindow,
            SceneHierarchyWindow,
            ScriptableWizard,
            AddCurvesPopup,
            AnimationWindow,
            CurveEditorWindow,
            MinMaxCurveEditorWindow,
            AnnotationWindow,
            LayerVisibilityWindow,
            AssetStoreInstaBuyWindow,
            AssetStoreLoginWindow,
            AssetStoreWindow,
            AudioMixerWindow,
            CollabPublishDialog,
            CollabCannotPublishDialog,
            GameView,
            AboutWindow,
            AssetSaveDialog,
            BumpMapSettingsFixingWindow,
            ColorPicker,
            EditorUpdateWindow,
            FallbackEditorWindow,
            GradientPicker,
            PackageExport,
            PackageImport,
            PopupWindow,
            PopupWindowWithoutFocus,
            PragmaFixingWindow,
            SaveWindowLayout,
            DeleteWindowLayout,
            EditorToolWindow,
            SnapSettings,
            TreeViewTestWindow,
            GUIViewDebuggerWindow,
            InspectorWindow,
            PreviewWindow,
            AddShaderVariantWindow,
            AddComponentWindow,
            AdvancedDropdownWindow,
            LookDevView,
            AttachToPlayerPlayerIPWindow,
            HolographicEmulationWindow,
            FrameDebuggerWindow,
            SearchableEditorWindow,
            LightingExplorerWindow,
            LightingWindow,
            LightmapPreviewWindow,
            NavMeshEditorWindow,
            OcclusionCullingWindow,
            PhysicsDebugWindow,
            TierSettingsWindow,
            SceneView,
            SettingsWindow,
            ProjectSettingsWindow,
            PreferenceSettingsWindow,
            PackerWindow,
            SpriteUtilityWindow,
            TroubleshooterWindow,
            UIElementsEditorWindowCreator,
            UndoWindow,
            UnityConnectConsentView,
            UnityConnectEditorWindow,
            MetroCertificatePasswordWindow,
            MetroCreateTestCertificateWindow,
            WindowChange,
            WindowCheckoutFailure,
            WindowPending,
            WindowResolve,
            WindowRevert,
            WebViewEditorStaticWindow,
            WebViewEditorWindow,
            WebViewEditorWindowTabs,
            SearchWindow,
            LicenseManagementWindow,
            PackageManagerWindow,
            ParticleSystemWindow,
            PresetSelector,
            ProfilerWindow,
            UISystemPreviewWindow,
            ConflictResolverWindow,
            DeleteShortcutProfileWindow,
            PromptWindow,
            ShortcutManagerWindow,
            SketchUpImportDlg,
            TerrainWizard,
            ImportRawHeightmap,
            ExportRawHeightmap,
            TreeWizard,
            DetailMeshWizard,
            DetailTextureWizard,
            PlaceTreeWizard,
            FlattenHeightmap,
            TestEditorWindow,
            PanelDebugger,
            UIElementsDebugger,
            PainterSwitcherWindow,
            AllocatorDebugger,
            UIRDebugger,
            UIElementsSamples,
            AnimatorControllerTool,
            LayerSettingsWindow,
            ParameterControllerEditor,
            AddStateMachineBehaviourComponentWindow,
            AndroidKeystoreWindow,
            TimelineWindow,
            TMP_ProjectConversionUtility,
            TMP_SpriteAssetImporter,
            TMPro_FontAssetCreatorWindow,
            CollabHistoryWindow,
            CollabToolbarWindow,
            TestRunnerWindow,
            TMP_PackageResourceImporterWindow,

            ConsoleE_Window,
        }

        public static System.Type GetEditorWindowTypeByName(string editorToFind, bool showDebug = false)
        {
            var result = new System.Collections.Generic.List<System.Type>();
            System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
            System.Type editorWindow = typeof(EditorWindow);
            foreach (var A in AS)
            {
                System.Type[] types = A.GetTypes();
                foreach (var T in types)
                {
                    if (T.IsSubclassOf(editorWindow))
                    {
                        if (showDebug)
                        {
                            Debug.Log(T.Name);
                        }
                        if (T.Name.Equals(editorToFind))
                        {
                            return (T);
                        }
                    }

                }
            }
            return (null);
        }
        #endregion
    }
}