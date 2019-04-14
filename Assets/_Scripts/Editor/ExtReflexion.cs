using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.IMGUI.Controls;
using System.Reflection;
using Borodar.RainbowHierarchy;
using System;
using System.Collections.Generic;

/*
meth_PickObjectMeth = type_HandleUtility.GetMethod("myFunction",
                                                 BindingFlags.Static | BindingFlags.Public, //if static AND public
                                                 null,
                                                 new [] {typeof(Vector2), typeof(bool)},//specify arguments to tell reflection which variant to look for
                                                 null)
*/
public class ExtReflexion
{
    /// <summary>
    /// for adding, do a GetAllEditorWindowTypes(true);
    /// </summary>
    public enum AllNameAssemblyKnown
    {
        AnimationWindow,
        SearchWindow,
        SceneHierarchySortingWindow,
        SceneHierarchyWindow,
        AssetStoreWindow,
        GameView,
        InspectorWindow,
        SearchableEditorWindow,
        SceneView,
    }

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
    public static System.Type[] GetAllEditorWindowTypes(bool showInConsol = false)
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
                        //Debug.Log(T.FullName);
                        Debug.Log(T.Name);
                    }
                }
                    
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// repaint an Editor
    /// use: RepaintInspector(typeof(SomeTypeInspector));
    /// </summary>
    /// <param name="t"></param>
    public static void RepaintInspector(System.Type t)
    {
        Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
        for (int i = 0; i < ed.Length; i++)
        {
            if (ed[i].GetType() == t)
            {
                ed[i].Repaint();
                return;
            }
        }
    }

    /// <summary>
    /// System.Type animationWindowType = ExtReflexion.GetTypeFromAssembly("AnimationWindow", editorAssembly);
    /// </summary>
    public static MethodInfo[] GetAllMethodeOfType(System.Type type, System.Reflection.BindingFlags bindings, bool showInConsol = false)
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
    public static EditorWindow[] GetAllOpennedWindow(bool showInConsol = false)
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

    public static EditorWindow GetOpennedWindowByName(string editorToFind)
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

    public static System.Reflection.BindingFlags GetFullBinding()
    {
        return (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static);
    }

    /// <summary>
    /// System.Reflection.Assembly editorAssembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
    /// GetTypeFromAssembly("AnimationWindow", editorAssembly);
    /// </summary>
    /// <returns></returns>
    public static System.Type GetTypeFromAssembly(string typeName, System.Reflection.Assembly assembly, System.StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase, bool showNames = false)
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

    /// <summary>
    /// play button on animator
    /// </summary>
    public static void SetPlayButton()
    {
        //open Animation Editor Window
        System.Type animationWindowType = null;        
        EditorWindow animationWindowEditor = ExtReflexion.ShowAndReturnEditorWindow(ExtReflexion.AllNameAssemblyKnown.AnimationWindow, ref animationWindowType);

        //Get animationWindow Type
        animationWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationWindow");

        //Get field m_AnimEditor
        FieldInfo animEditorFI = animationWindowType.GetField("m_AnimEditor", ExtReflexion.GetFullBinding());

        /*
        object animEditorObject = animEditorFI.GetValue(animationWindowEditor);
        MethodInfo playMI = animEditorFI.FieldType.GetMethod("TogglePlayAnimation", ExtReflexion.GetFullBinding());
        Debug.Log(playMI.Name);


        Type[] types = new Type[1];
        object paramFunction = playMI.GetType().GetConstructor(GetFullBinding(), null, new type)
        ConstructorInfo constructorInfoObj = playMI.GetType().GetConstructor(GetFullBinding(), null,
                CallingConventions.HasThis, types, null);

        playMI.Invoke(animEditorObject, new object[0]);
        */
        //PlayButtonOnGUI

        

        //Get the propertue of animEditorFI
        PropertyInfo controlInterfacePI = animEditorFI.FieldType.GetProperty("controlInterface", ExtReflexion.GetFullBinding());

        //Get property i splaying or not
        PropertyInfo isPlaying = controlInterfacePI.PropertyType.GetProperty("playing", ExtReflexion.GetFullBinding());
        
        //get object controlInterface
        object controlInterface = controlInterfacePI.GetValue(animEditorFI.GetValue(animationWindowEditor));
        bool playing = (bool)isPlaying.GetValue(controlInterface);

        if (!playing)
        {
            MethodInfo playMI = controlInterfacePI.PropertyType.GetMethod("StartPlayback", ExtReflexion.GetFullBinding());
            playMI.Invoke(controlInterface, new object[0]);
        }
        else
        {
            MethodInfo playMI = controlInterfacePI.PropertyType.GetMethod("StopPlayback", ExtReflexion.GetFullBinding());
            playMI.Invoke(controlInterface, new object[0]);
        }
        
    }

    /// <summary>
    /// from a given name, return and open/show the editorWindow
    /// usage:
    /// System.Type animationWindowType = null;
    /// EditorWindow animationWindowEditor = ExtReflexion.ShowAndReturnEditorWindow(ExtReflexion.AllNameAssemblyKnown.AnimationWindow, ref animationWindowType);
    /// </summary>
    public static EditorWindow ShowAndReturnEditorWindow(AllNameAssemblyKnown editorWindow, ref System.Type animationWindowType)
    {
        System.Reflection.Assembly editorAssembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
        animationWindowType = ExtReflexion.GetTypeFromAssembly(editorWindow.ToString(), editorAssembly);
        EditorWindow animationWindowEditor = EditorWindow.GetWindow(animationWindowType);

        return (animationWindowEditor);
    }

    public static void SetSearch(string search)
    {
        System.Type type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        //MethodInfo[] allMethods = GetAllMethodeOfType(type, GetFullBinding(), true);
        MethodInfo methodInfo = type.GetMethod("SetSearchFilter", GetFullBinding());

        EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        if (methodInfo == null)
        {
            Debug.Log("null");
            return;
        }

        var window = EditorWindow.focusedWindow;
        methodInfo.Invoke(window, new object[] { search, SearchableEditorWindow.SearchMode.All, true, false });
    }

    public static void Collapse(GameObject go, bool collapse)
    {
        // bail out immediately if the go doesn't have children
        if (go.transform.childCount == 0)
            return;

        if (collapse)
        {
            EditorGUIUtility.PingObject(go.transform.GetChild(0).gameObject);
            Selection.activeObject = go;
        }
        else
        {
            SetExpandedRecursive(go, false);
        }
    }

    public static EditorWindow GetFocusedWindow(string window)
    {
        FocusOnWindow(window);
        return EditorWindow.focusedWindow;
    }

    public static void FocusOnWindow(string window)
    {
        EditorApplication.ExecuteMenuItem("Window/" + window);
    }

    /// <summary>
    /// expand recursivly a hierarchy foldout
    /// </summary>
    /// <param name="go"></param>
    /// <param name="expand"></param>
    public static void SetExpandedRecursive(GameObject go, bool expand)
    {
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        var methodInfo = type.GetMethod("SetExpandedRecursive");

        EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        var window = EditorWindow.focusedWindow;

        methodInfo.Invoke(window, new object[] { go.GetInstanceID(), expand });
    }

    public static void AssignLabel(GameObject g, int colorIconById)
    {
        Texture2D tex = EditorGUIUtility.IconContent("sv_label_" + colorIconById).image as Texture2D;
        Type editorGUIUtilityType = typeof(EditorGUIUtility);
        BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
        object[] args = new object[] { g, tex };
        editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
    }
}