using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Reflection;
using System;
using System.Collections.Generic;
using TMPro;

public class ExtPhilaeEditor : ScriptableObject
{
    [MenuItem("PERSO/Philae/SetGround Layer & Material Ground")]
    public static void SetGroundLayerAndMat()
    {
        SetLayerAndMat("Assets/Resources/Ground.mat", "Walkable/Ground");
    }
    [MenuItem("PERSO/Philae/SetGround Layer & Material Stick")]
    public static void SetStickLayerAndMat()
    {
        SetLayerAndMat("Assets/Resources/Stick.mat", "Walkable/Stick");
    }
    [MenuItem("PERSO/Philae/SetGround Layer & Material Dont")]
    public static void SetDontLayerAndMat()
    {
        SetLayerAndMat("Assets/Resources/Dont.mat", "Walkable/Dont");
    }
    [MenuItem("PERSO/Philae/SetGround Layer & Material FastForward")]
    public static void SetFastForwardLayerAndMat()
    {
        SetLayerAndMat("Assets/Resources/FastForward.mat", "Walkable/FastForward");
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

    /*
    /// <summary>
    /// change the visual of the editor config for this object
    /// </summary>
    /// <param name="selectedObject"></param>
    /// <param name="iconType"></param>
    /// <param name="_IsIconRecursive"></param>
    /// <param name="coreBackground"></param>
    /// <param name="_IsBackgroundRecursive"></param>
    public static void AddCustomEditorToObject(GameObject selectedObject, bool create = true,
        HierarchyIcon iconType = HierarchyIcon.None,
        bool _IsIconRecursive = false,
        Borodar.RainbowCore.CoreBackground coreBackground = Borodar.RainbowCore.CoreBackground.None,
        bool _IsBackgroundRecursive = false)
    {
        GameObject hierarchy = GameObject.Find("RainbowHierarchyConf");
        HierarchySceneConfig hierarchySceneConfig = hierarchy.GetComponent<HierarchySceneConfig>();
        if (hierarchySceneConfig)
        {
            HierarchyItem newItem = hierarchySceneConfig.GetItem(selectedObject);
            if (newItem == null)
            {
                if (!create)
                    return;

                newItem = new HierarchyItem(HierarchyItem.KeyType.Object, selectedObject, selectedObject.name)
                {
                    IconType = HierarchyIcon.None,
                    IsIconRecursive = false,
                    BackgroundType = Borodar.RainbowCore.CoreBackground.ClrIndigo,
                    IsBackgroundRecursive = false,
                };
                hierarchySceneConfig.AddItem(newItem);
            }
            else
            {
                if (!create)
                {
                    hierarchySceneConfig.RemoveAll(selectedObject, HierarchyItem.KeyType.Object);
                }
                else
                {
                    hierarchySceneConfig.RemoveAll(selectedObject, HierarchyItem.KeyType.Object);
                    newItem = new HierarchyItem(HierarchyItem.KeyType.Object, selectedObject, selectedObject.name)
                    {
                        IconType = HierarchyIcon.None,
                        IsIconRecursive = false,
                        BackgroundType = Borodar.RainbowCore.CoreBackground.ClrIndigo,
                        IsBackgroundRecursive = false,
                    };
                    hierarchySceneConfig.AddItem(newItem);
                }
            }
        }
    }
    */
}