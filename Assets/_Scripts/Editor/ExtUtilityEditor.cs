using UnityEngine;
using UnityEditor;

public class ExtUtilityEditor : ScriptableObject
{
    /// <summary>
    /// save the MeshFilter of the selected object in Assets/Resources/Procedural/
    /// </summary>
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
    private static string SaveAsset(string nameMesh, string extention = "asset")
    {
        return string.Format("{0}_{1}.{2}",
                            nameMesh,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"),
                             extention);
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
    
    [MenuItem("PERSO/Ext/CreateEmptyParent #e")]
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

        Selection.activeGameObject = newParent;
        ExtReflexion.SetExpandedRecursive(newParent, true);
    }

    [MenuItem("PERSO/Ext/DeleteEmptyParent %&e")]
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
}