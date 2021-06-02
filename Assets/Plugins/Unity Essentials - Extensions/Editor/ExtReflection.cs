using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace UnityEssentials.Extensions.Editor
{
    /// <summary>
    /// useful reflection methods
    /// </summary>
    public static class ExtReflection
    {
        /// <summary>
        /// Get full type name with full namespace names
        /// </summary>
        /// <param name="baseType">The type to get the C# name for (may be a generic type or a nullable type).</param>
        /// <param name="fullName">if set to <c>true</c> [full name].</param>
        /// <returns>Full type name, fully qualified namespaces</returns>
        // http://stackoverflow.com/questions/2579734/get-the-type-name
        // http://stackoverflow.com/questions/6402864/c-pretty-type-name-function
        public static string GetCSharpName(this Type type, bool fullName = false)
        {
            if (type == null || type.Equals(typeof(void)))
            {
                return "void";
            }

            Type nullableType = Nullable.GetUnderlyingType(type);
            Type baseType = nullableType != null ? nullableType : type;
            string nullableSuffix = nullableType != null ? "?" : string.Empty;

            if (baseType.IsGenericType)
            {
                return string.Format("{0}<{1}>{2}",
                    baseType.Name.Substring(0, baseType.Name.IndexOf('`')),
                    string.Join(", ", baseType.GetGenericArguments().Select(ga => ga.GetCSharpName()).ToArray()),
                    nullableSuffix);
            }

            switch (Type.GetTypeCode(baseType))
            {
                case TypeCode.Boolean:
                    return "bool";
                case TypeCode.Byte:
                    return "byte";
                case TypeCode.SByte:
                    return "sbyte";
                case TypeCode.Char:
                    return "char";
                case TypeCode.Decimal:
                    return "decimal" + nullableSuffix;
                case TypeCode.Double:
                    return "double" + nullableSuffix;
                case TypeCode.Single:
                    return "float" + nullableSuffix;
                case TypeCode.Int32:
                    return "int" + nullableSuffix;
                case TypeCode.UInt32:
                    return "uint" + nullableSuffix;
                case TypeCode.Int64:
                    return "long" + nullableSuffix;
                case TypeCode.UInt64:
                    return "ulong" + nullableSuffix;
                case TypeCode.Int16:
                    return "short" + nullableSuffix;
                case TypeCode.UInt16:
                    return "ushort" + nullableSuffix;
                case TypeCode.String:
                    return "string";
                case TypeCode.Object:
                    return (fullName ? baseType.FullName : baseType.Name) + nullableSuffix;
                default:
                    return null;
            }

        }

        /// <summary>
        /// Gets the inheritance depth of a type.
        /// </summary>
        /// <param name="type">The type to analyze</param>
        /// <returns>Inheritance depth</returns>
        public static int GetInheritanceDepth(this Type type)
        {
            int count = 0;

            for (var current = type; current != null; current = current.BaseType)
                count++;
            return count;
        }

        public static void ClearConsole()
        {
            // This simply does "LogEntries.Clear()" the long way:
            var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        public static void Place(GameObject go, GameObject parent)
        {
            if (parent != null)
            {
                var transform = go.transform;
                Undo.SetTransformParent(transform, parent.transform, "Reparenting");
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                go.layer = parent.layer;

                if (parent.GetComponent<RectTransform>())
                    ObjectFactory.AddComponent<RectTransform>(go);
            }
            else
            {
                SceneView.lastActiveSceneView.MoveToView(go.transform);
                StageUtility.PlaceGameObjectInCurrentStage(go); // may change parent
            }

            // Only at this point do we know the actual parent of the object and can mopdify its name accordingly.
            GameObjectUtility.EnsureUniqueNameForSibling(go);
            Undo.SetCurrentGroupName("Create " + go.name);

            //EditorWindow.FocusWindowIfItsOpen<SceneHierarchyWindow>();
            Selection.activeGameObject = go;
        }

        /// <summary>
        /// search for an object in all editro search bar
        /// use: ExtReflexion.SetSearch("to find", ExtReflexion.AllNameAssemblyKnown.SceneHierarchyWindow);
        /// use: ExtReflexion.SetSearch("to find", ExtReflexion.AllNameAssemblyKnown.SceneView);
        /// </summary>
        /// <param name="search"></param>
        public static void SetSearch(string search, ExtEditorWindow.AllNameAssemblyKnown nameEditorWindow = ExtEditorWindow.AllNameAssemblyKnown.SceneView)
        {
            //open animation window
            EditorWindow animationWindowEditor = ExtEditorWindow.OpenEditorWindow(nameEditorWindow, out System.Type animationWindowType);

            //System.Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            //EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");

            MethodInfo methodInfo = animationWindowType.GetMethod("SetSearchFilter", GetFullBinding());

            if (methodInfo == null)
            {
                Debug.Log("null");
                return;
            }

            var window = EditorWindow.focusedWindow;
            methodInfo.Invoke(window, new object[] { search, SearchableEditorWindow.SearchMode.All, true, false });
        }

        /// <summary>
        /// play button on animator
        /// </summary>
        public static void SetPlayButton(out bool isPlayingAnimation)
        {
            //open animator
            EditorWindow animatorWindow = ExtEditorWindow.OpenEditorWindow(ExtEditorWindow.AllNameAssemblyKnown.AnimatorControllerTool, out System.Type animatorWindowType);

            //open animation
            EditorWindow animationWindowEditor = ExtEditorWindow.OpenEditorWindow(ExtEditorWindow.AllNameAssemblyKnown.AnimationWindow, out System.Type animationWindowType);

            //Get field m_AnimEditor
            FieldInfo animEditorFI = animationWindowType.GetField("m_AnimEditor", ExtReflection.GetFullBinding());

            //Get the propertue of animEditorFI
            PropertyInfo controlInterfacePI = animEditorFI.FieldType.GetProperty("controlInterface", ExtReflection.GetFullBinding());

            //Get property i splaying or not
            PropertyInfo isPlaying = controlInterfacePI.PropertyType.GetProperty("playing", ExtReflection.GetFullBinding());

            //get object controlInterface
            object controlInterface = controlInterfacePI.GetValue(animEditorFI.GetValue(animationWindowEditor));
            //get the state of the "play" button
            isPlayingAnimation = (bool)isPlaying.GetValue(controlInterface);

            if (!isPlayingAnimation)
            {
                MethodInfo playMI = controlInterfacePI.PropertyType.GetMethod("StartPlayback", ExtReflection.GetFullBinding());
                playMI.Invoke(controlInterface, new object[0]);
            }
            else
            {
                MethodInfo playMI = controlInterfacePI.PropertyType.GetMethod("StopPlayback", ExtReflection.GetFullBinding());
                playMI.Invoke(controlInterface, new object[0]);
            }
        }

        /// <summary>
        /// hide/show the cursor
        /// </summary>
        public static bool Hidden
        {
            get
            {
                Type type = typeof(Tools);
                FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
                return ((bool)field.GetValue(null));
            }
            set
            {
                Type type = typeof(Tools);
                FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
                field.SetValue(null, value);
            }
        }



        public static void PreventUnselect(GameObject currentTarget)
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                Selection.activeGameObject = null;
            }
            else
            {
                if (EditorWindow.focusedWindow == SceneView.currentDrawingSceneView)
                {
                    Selection.activeGameObject = currentTarget;
                }
            }
        }

        /// <summary>
        /// Check if the target GameObject is expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static bool IsExpanded(GameObject go)
        {
            return GetExpandedGameObjects().Contains(go);
        }

        /// <summary>
        /// Get a list of all GameObjects which are expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static List<GameObject> GetExpandedGameObjects()
        {
            object sceneHierarchy = GetSceneHierarchy();

            MethodInfo methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("GetExpandedGameObjects");

            object result = methodInfo.Invoke(sceneHierarchy, new object[0]);

            return (List<GameObject>)result;
        }

        public static void Rename(GameObject gameObject)
        {
            try
            {
                Selection.activeObject = gameObject;
                var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
                var hierarchyWindow = EditorWindow.GetWindow(type);
                var rename = type.GetMethod("RenameGO", BindingFlags.Instance | BindingFlags.NonPublic);
                rename.Invoke(hierarchyWindow, null);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Set the target GameObject as expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static void SetExpanded(GameObject go, bool expand)
        {
            object sceneHierarchy = GetSceneHierarchy();

            MethodInfo methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
        }

        /// <summary>
        /// Set the target GameObject and all children as expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static void SetExpandedRecursive(GameObject go, bool expand)
        {
            object sceneHierarchy = GetSceneHierarchy();

            MethodInfo methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);

            methodInfo.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
        }

        private static object GetSceneHierarchy()
        {
            EditorWindow window = GetHierarchyWindow();

            object sceneHierarchy = typeof(EditorWindow).Assembly
                .GetType("UnityEditor.SceneHierarchyWindow")
                .GetProperty("sceneHierarchy")
                .GetValue(window);

            return sceneHierarchy;
        }

        private static EditorWindow GetHierarchyWindow()
        {
            // For it to open, so that it the current focused window.
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            return EditorWindow.focusedWindow;
        }


        public static void SetAllInspectorsExpanded(GameObject go, bool expanded)
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component is Renderer)
                {
                    var mats = ((Renderer)component).sharedMaterials;
                    for (int i = 0; i < mats.Length; ++i)
                    {
                        InternalEditorUtility.SetIsInspectorExpanded(mats[i], expanded);
                    }
                }
                InternalEditorUtility.SetIsInspectorExpanded(component, expanded);
            }
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        /// <summary>
        /// repaint an Editor
        /// use: RepaintInspector(typeof(SomeTypeInspector));
        /// </summary>
        /// <param name="t"></param>
        public static void RepaintInspector(System.Type t)
        {
            UnityEditor.Editor[] ed = (UnityEditor.Editor[])Resources.FindObjectsOfTypeAll<UnityEditor.Editor>();
            for (int i = 0; i < ed.Length; i++)
            {
                if (ed[i].GetType() == t)
                {
                    ed[i].Repaint();
                    return;
                }
            }
        }


        /////////////////////////utility reflexion


        public enum AllNameEditorWindowKnown
        {
            Lighting,
            Game,
            Scene,
            Hierarchy,
            Project,
            Inspector
        }

        /// <summary>
        /// Get all editor window type.
        /// If we want just the one open, we can do just:
        /// EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
        /// System.Type[] allUnityWindow = UtilityEditor.GetAllEditorWindowTypes(true);
        /// </summary>
        /// <returns></returns>
        private static System.Type[] GetAllEditorWindowTypes(bool showInConsol = false)
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
                        result.Add(T);
                        if (showInConsol)
                        {
                            Debug.Log(T.Name);
                        }
                    }

                }
            }
            return result.ToArray();
        }

        

        /// <summary>
        /// System.Type animationWindowType = ExtReflexion.GetTypeFromAssembly("AnimationWindow", editorAssembly);
        /// </summary>
        private static MethodInfo[] GetAllMethodeOfType(System.Type type, System.Reflection.BindingFlags bindings, bool showInConsol = false)
        {
            MethodInfo[] allMathod = type.GetMethods(bindings);
            if (showInConsol)
            {
                for (int i = 0; i < allMathod.Length; i++)
                {
                    Debug.Log(allMathod[i].Name);
                }
            }
            return (allMathod);
        }

        public static FieldInfo[] GetAllFieldOfType(System.Type type, System.Reflection.BindingFlags bindings, bool showInConsol = false)
        {
            FieldInfo[] allField = type.GetFields(bindings);
            if (showInConsol)
            {
                for (int i = 0; i < allField.Length; i++)
                {
                    Debug.Log(allField[i].Name);
                }
            }
            return (allField);
        }

        public static PropertyInfo[] GetAllpropertiesOfType(System.Type type, System.Reflection.BindingFlags bindings, bool showInConsol = false)
        {
            PropertyInfo[] allProperties = type.GetProperties(bindings);
            if (showInConsol)
            {
                for (int i = 0; i < allProperties.Length; i++)
                {
                    Debug.Log(allProperties[i].Name);
                }
            }
            return (allProperties);
        }

        /// <summary>
        /// show all opened window
        /// </summary>
        private static EditorWindow[] GetAllOpennedWindow(bool showInConsol = false)
        {
            EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

            if (showInConsol)
            {
                for (int i = 0; i < allWindows.Length; i++)
                {
                    Debug.Log(allWindows[i].titleContent.text);
                }
            }
            return (allWindows);
        }

        private static EditorWindow GetOpennedWindowByName(string editorToFind)
        {
            EditorWindow[] allWIndow = GetAllOpennedWindow();
            for (int i = 0; i < allWIndow.Length; i++)
            {
                if (allWIndow[i].titleContent.text.Equals(editorToFind))
                {
                    return (allWIndow[i]);
                }
            }
            return (null);
        }

        /// <summary>
        /// Get all the most common Binding type of elements
        /// </summary>
        /// <returns></returns>
        public static System.Reflection.BindingFlags GetFullBinding()
        {
            return (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// System.Reflection.Assembly editorAssembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
        /// GetTypeFromAssembly("AnimationWindow", editorAssembly);
        /// </summary>
        /// <returns></returns>
        private static System.Type GetTypeFromAssembly(string typeName, System.Reflection.Assembly assembly, System.StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase, bool showNames = false)
        {
            if (assembly == null)
                return (null);

            System.Type[] types = assembly.GetTypes();
            foreach (System.Type type in types)
            {
                if (showNames)
                {
                    Debug.Log(type.Name);
                }
                if (type.Name.Equals(typeName, ignoreCase) || type.Name.Contains('+' + typeName))
                    return (type);
            }
            return (null);
        }

        /// <summary>Get the object owner of a field.  This method processes
        /// the '.' separator to get from the object that owns the compound field
        /// to the object that owns the leaf field</summary>
        /// <param name="path">The name of the field, which may contain '.' separators</param>
        /// <param name="obj">the owner of the compound field</param>
        public static object GetParentObject(string path, object obj)
        {
            var fields = path.Split('.');
            if (fields.Length == 1)
                return obj;

            var info = obj.GetType().GetField(
                    fields[0], System.Reflection.BindingFlags.Public
                        | System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Instance);
            obj = info.GetValue(obj);

            return GetParentObject(string.Join(".", fields, 1, fields.Length - 1), obj);
        }

        /// <summary>Returns a string path from an expression - mostly used to retrieve serialized properties
        /// without hardcoding the field path. Safer, and allows for proper refactoring.</summary>
        public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    me = expr.Body as MemberExpression;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var members = new List<string>();
            while (me != null)
            {
                members.Add(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            var sb = new StringBuilder();
            for (int i = members.Count - 1; i >= 0; i--)
            {
                sb.Append(members[i]);
                if (i > 0) sb.Append('.');
            }
            return sb.ToString();
        }
    }
}