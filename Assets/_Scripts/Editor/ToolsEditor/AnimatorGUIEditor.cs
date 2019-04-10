﻿using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(AnimatorGUI))]
public class AnimatorGUIEditor : OdinEditor
{
    AnimatorGUI animatorGUI;

    private void OnSceneGUI()
    {
        animatorGUI = (AnimatorGUI)target;

        Handles.color = Color.green;
        if (Handles.Button(animatorGUI.transform.position + Vector3.up * 1 + Vector3.right * 0.3f, Quaternion.identity, 0.2f, 0.2f, Handles.SphereHandleCap))
        {
            System.Reflection.Assembly editorAssembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
            System.Type animationWindowType = ExtReflexion.GetTypeFromAssembly(ExtReflexion.AllNameAssembly.AnimationWindow.ToString(), editorAssembly);
            System.Object animationWindowObject = EditorWindow.GetWindow(animationWindowType);

            //ethodInfo[] allMathod = animationWindowType.GetMethods();

            if (animationWindowObject != null)
            {
                System.Reflection.MethodInfo previewMethod = animationWindowType.GetMethod("PreviewFrame", ExtReflexion.GetFullBinding());
                
                ExtReflexion.GetAllMethodeOfType(animationWindowType, true);
                Debug.Log(previewMethod);
                //previewMethod.Invoke(animationWindowObject, new System.Object[]);
            }

            //ExtReflexion.GetAllOpennedWindow(true);
            //ExtReflexion.GetAllEditorWindowTypes(true);

            //SEARCH WINDOW WHEN OPENING GRAVITY ATTRACTOR !!!

            /*
            EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            bool isOpen = false;
            for (int i = 0; i < allWindows.Length; i++)
            {
                //Debug.Log(allWindows[i].titleContent.text);
                if (string.Equals(allWindows[i].titleContent.text, "Animation"))
                {
                    Debug.Log("focus");
                    allWindows[i].Focus();
                    IEnumerable<MethodInfo> method = allWindows[i].GetType().GetRuntimeMethods();
                    UtilityEditor.DisplayAllMethodes(method);
                    //MethodInfo methode = allWindows[i].GetType().GetMethod("AnimEditor");
                    Debug.Log("...");
                    Debug.Log(allWindows[i].GetType().GetMethod("UnityEditor.AnimEditor"));
                    Debug.Log(allWindows[i].GetType().GetMethod("AnimEditor"));
                    Debug.Log(allWindows[i].GetType().GetMethod("get_animEditor"));
                    Debug.Log(allWindows[i].GetType().GetMethod("animEditor"));
                    Debug.Log(allWindows[i].GetType().GetMethod("animEditor()"));
                    Debug.Log(allWindows[i].GetType().GetMethod("get_animEditor()"));
                    isOpen = true;
                }
            }

            if (!isOpen)
            {
                //Debug.Log("create window !");
                System.Type[] allUnityWindow = UtilityEditor.GetAllEditorWindowTypes();
                for (int i = 0; i < allUnityWindow.Length; i++)
                {
                    Debug.Log(allUnityWindow[i].Name);

                    //EditorWindow editorWindow = EditorWindow.GetWindow(allUnityWindow[i]);

                    if (string.Equals(allUnityWindow[i].Name, "AnimationWindow"))
                    {
                        EditorWindow editorWindow = EditorWindow.GetWindow(allUnityWindow[i]);
                        editorWindow.Focus();
                        break;
                    }
                    
                    //EditorWindow editorWindow = EditorWindow.GetWindow(allUnityWindow[i]);
                    //Debug.Log(editorWindow);
                    //if (string.Equals(editorWindow.titleContent.text, "Animation"))
                    //{
                        //allWindows[i].Show();
                        //break;
                    //}
                    
                    //Debug.Log();
                }

                //EditorWindow animationWindow = EditorWindow.GetWindow(Animation);
                //EditorWindow animationWindow = ScriptableObject.CreateInstance<EditorWindow>();
            }
            */
            //animatorGUI.PlayAnimation();
        }
    }
}
