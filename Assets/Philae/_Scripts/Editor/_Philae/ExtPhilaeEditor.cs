using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Reflection;
using System;
using System.Collections.Generic;
using TMPro;

public class ExtPhilaeEditor : ScriptableObject
{
    [MenuItem("Philae/SetGround Layer & Material Ground")]
    public static void SetGroundLayerAndMat()
    {
        SetLayerAndMat("Assets/Philae/Resources/Ground.mat", "Walkable/Ground");
    }
    [MenuItem("Philae/SetGround Layer & Material Stick")]
    public static void SetStickLayerAndMat()
    {
        SetLayerAndMat("Assets/Philae/Resources/Stick.mat", "Walkable/Stick");
    }
    [MenuItem("Philae/SetGround Layer & Material Dont")]
    public static void SetDontLayerAndMat()
    {
        SetLayerAndMat("Assets/Philae/Resources/Dont.mat", "Walkable/Dont");
    }
    [MenuItem("Philae/SetGround Layer & Material FastForward")]
    public static void SetFastForwardLayerAndMat()
    {
        SetLayerAndMat("Assets/Philae/Resources/FastForward.mat", "Walkable/FastForward");
    }
    
    public static void SetLayerAndMat(string materialName, string layer)
    {
        GameObject[] activeOne = Selection.gameObjects;//Selection.activeGameObject;
        if (activeOne.Length == 0)
            return;

        for (int k = 0; k < activeOne.Length; k++)
        {
            Transform[] allChild = activeOne[k].GetComponentsInChildren<Transform>();


            for (int i = 0; i < allChild.Length; i++)
            {
                MeshFilter mesh = allChild[i].GetComponent<MeshFilter>();
                MeshRenderer meshRenderer = allChild[i].GetComponent<MeshRenderer>();

                
                TextMeshPro text = allChild[i].GetComponent<TextMeshPro>();
                if (text)
                    continue;
                
                if (mesh && meshRenderer)
                {
                    Undo.RecordObject(allChild[i].gameObject, "layer change");
                    Undo.RecordObject(meshRenderer, "materials change");

                    allChild[i].gameObject.layer = LayerMask.NameToLayer(layer);
                    var mat = AssetDatabase.LoadAssetAtPath(materialName, typeof(Material));
                    Debug.Log("mat: " + mat);
                    meshRenderer.material = (Material)mat;
                }
            }
        }    
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