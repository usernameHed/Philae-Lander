using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.IMGUI.Controls;
using System.Reflection;
using Borodar.RainbowHierarchy;
using System;
using System.Collections.Generic;

public class UtilityEditor : ScriptableObject
{
    public const int FILTERMODE_ALL = 0;
    public const int FILTERMODE_NAME = 1;
    public const int FILTERMODE_TYPE = 2;

    private static string SaveAsset(string nameMesh, string extention = "asset")
    {
        return string.Format("{0}_{1}.{2}",
                            nameMesh,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                             extention);
    }

    [MenuItem("PERSO/Procedural/Save Selected Mesh")]
    public static void SaveSelectedMesh()
	{
        GameObject activeOne = Selection.activeGameObject;
        if (!activeOne)
            return;

        MeshFilter meshRoad = activeOne.GetComponent<MeshFilter>();

        Mesh tempMesh = (Mesh)UnityEngine.Object.Instantiate(meshRoad.sharedMesh);

        string path = "Assets/Resources/Procedural/" + SaveAsset("savedMesh");
        Debug.Log(path);
        AssetDatabase.CreateAsset(tempMesh, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static T GetScript<T>()
    {
        object obj = UnityEngine.Object.FindObjectOfType(typeof(T));

        if (obj != null)
        {
            return ((T)obj);
            //gameManager = (GameManager)obj;
            //gameManager.indexSaveEditorTmp = gameManager.saveManager.GetMainData().GetLastMapSelectedIndex();
        }

        return (default(T));
    }

    public static void FocusOnSelection(GameObject objToFocus, float zoom = 5f)
    {
        SceneView.lastActiveSceneView.LookAt(objToFocus.transform.position);
        if (zoom != -1)
            SceneViewCameraFunction.ViewportPanZoomIn(zoom);
    }

    

    [MenuItem("PERSO/Philae/Select Gravity Attractor _g")]
    public static bool SelectParentOfAttractor()
    {
        GravityAttractorLD gravityAttractorLD = Selection.activeGameObject.GetComponentInAllParents<GravityAttractorLD>(99, true);
        if (gravityAttractorLD)
        {
            Selection.activeGameObject = gravityAttractorLD.transform.gameObject;
            Debug.Log("we found a gravityAttractor here !!");
            //FocusOnSelection(Selection.activeGameObject, -1);
            //SetSearchFilter(Selection.activeGameObject.name, FILTERMODE_ALL);

            return (true);
            
        }
        return (false);
    }

    [MenuItem("PERSO/Philae/CreateEmptyParent #e")]
    public static void CreateEmptyParent()
    {
        if (!Selection.activeGameObject)
            return;
        GameObject newParent = new GameObject("Parent of " + Selection.activeGameObject.name);
        int indexFocused = Selection.activeGameObject.transform.GetSiblingIndex();
        newParent.transform.SetParent(Selection.activeGameObject.transform.parent);
        newParent.transform.position = Selection.activeGameObject.transform.position;

        Selection.activeGameObject.transform.SetParent(newParent.transform);
        newParent.transform.SetSiblingIndex(indexFocused);


        //EditorGUIUtility.PingObject(Selection.activeGameObject);
        Selection.activeGameObject = newParent;

        ExtReflexion.SetExpandedRecursive(newParent, true);

        //EditorGUILayout.Foldout(true, newParent);
    }

    



    [MenuItem("PERSO/Philae/DeleteEmptyParent %&e")]
    public static void DeleteEmptyParent()
    {
        if (!Selection.activeGameObject)
            return;

        int sibling = Selection.activeGameObject.transform.GetSiblingIndex();
        Transform parentOfParent = Selection.activeGameObject.transform.parent;
        Transform firstChild = Selection.activeGameObject.transform.GetChild(0);
        while (Selection.activeGameObject.transform.childCount > 0)
        {
            Transform child = Selection.activeGameObject.transform.GetChild(0);
            child.SetParent(parentOfParent);
            child.SetSiblingIndex(sibling);
            sibling++;
        }
        DestroyImmediate(Selection.activeGameObject);

        Selection.activeGameObject = firstChild.gameObject;
    }
    

    [MenuItem("PERSO/Philae/Create Gravity Attractor %g")]
    public static void CreateGravityAttractor()
    {
        if (SelectParentOfAttractor())
            return;

        if (Selection.activeGameObject.HasComponent<MeshFilter>() || Selection.activeGameObject.HasComponent<MeshRenderer>()
            || Selection.activeGameObject.HasComponent<MeshCollider>())
        {
            Selection.activeGameObject = Selection.activeGameObject.transform.parent.gameObject;
        }

        Debug.Log("try to create new gravity attractor ??");
        GravityAttractorLD gravityAttractorLD = Selection.activeGameObject.AddComponent<GravityAttractorLD>();
        gravityAttractorLD.CreateEditor();

        GravityAttractorEditor gravityAttractorEditor = Selection.activeGameObject.GetComponent<GravityAttractorEditor>();
        gravityAttractorEditor.GenerateParenting();

        gravityAttractorEditor.createMode = 1;
        gravityAttractorEditor.CreateTrigger();

        //AddCustomEditorToObject(Selection.activeGameObject, true, HierarchyIcon.None, false, Borodar.RainbowCore.CoreBackground.ClrIndigo, false);

        SceneView.FocusWindowIfItsOpen(typeof(SceneView));

        Selection.activeGameObject = gravityAttractorEditor.triggerRef;

        if (gravityAttractorEditor.GetGravityAttractor())
        {
            gravityAttractorEditor.GetGravityAttractor().philaeManager = UtilityEditor.GetScript<PhilaeManager>();
            gravityAttractorEditor.GetGravityAttractor().philaeManager.ldManager.FillList(true);
        }
    }

    [MenuItem("PERSO/Philae/Delete Gravity Attractor %&g")]
    public static void DeleteGravityAttractor()
    {
        if (!SelectParentOfAttractor())
            return;
        
        GravityAttractorLD gravityAttractorLD = Selection.activeGameObject.GetComponent<GravityAttractorLD>();
        GravityAttractorEditor gravityAttractorEditor = Selection.activeGameObject.GetComponent<GravityAttractorEditor>();
        

        gravityAttractorEditor.RemoveTrigger();
        gravityAttractorEditor.RemoveParenting();
        gravityAttractorLD.RemoveEditor();
        DestroyImmediate(gravityAttractorLD);
    }

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
}