using UnityEngine;
using UnityEditor;

public class UtilityEditor : ScriptableObject
{
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

}