using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.IMGUI.Controls;
using System.Reflection;
using Borodar.RainbowHierarchy;
using System;
using System.Collections.Generic;

public class ExtReflexion
{
    public enum AllNameAssembly
    {
        AnimationWindow,
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
    /// System.Type animationWindowType = ExtReflexion.GetTypeFromAssembly("AnimationWindow", editorAssembly);
    /// </summary>
    public static MethodInfo[] GetAllMethodeOfType(System.Type type, bool showInConsol = false)
    {
        MethodInfo[] allMathod = type.GetMethods();
        if (showInConsol)
        {
            for (int i = 0; i < allMathod.Length; i++)
            {
                Debug.Log(allMathod[i].Name);
            }
        }
        return (allMathod);
    }

    public static void DisplayAllMethodes(IEnumerable<MethodInfo> method)
    {
        foreach (MethodInfo e in method)
        {
            Debug.Log(e);
        }
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

    public static System.Reflection.BindingFlags GetFullBinding()
    {
        return (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
    }

    /// <summary>
    /// System.Reflection.Assembly editorAssembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
    /// GetTypeFromAssembly("AnimationWindow", editorAssembly);
    /// </summary>
    /// <returns></returns>
    public static System.Type GetTypeFromAssembly(string typeName, System.Reflection.Assembly assembly, System.StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase)
    {
        if (assembly == null)
            return (null);

        System.Type[] types = assembly.GetTypes();
        foreach (System.Type type in types)
        {
            if (type.Name.Equals(typeName, ignoreCase) || type.Name.Contains('+' + typeName))
                return (type);
        }
        return (null);
    }

    public static void SetSearchFilter(string filter, int filterMode)
    {

        SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll(typeof(SearchableEditorWindow));
        SearchableEditorWindow hierarchy = windows[0];
        foreach (SearchableEditorWindow window in windows)
        {

            if (window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow")
            {
                hierarchy = window;
                break;
            }
        }

        if (hierarchy == null)
            return;

        MethodInfo setSearchType = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
        object[] parameters = new object[] { filter, filterMode, false };

        setSearchType.Invoke(hierarchy, parameters);
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