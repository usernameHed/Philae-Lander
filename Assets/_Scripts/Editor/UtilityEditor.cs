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

    [MenuItem("PERSO/Philae/SetGround Layer & Material")]
    public static void SetGroundLayerAndMat()
    {
        GameObject activeOne = Selection.activeGameObject;
        if (!activeOne)
            return;

        Transform[] allChild = activeOne.GetComponentsInChildren<Transform>();
        

        for (int i = 0; i < allChild.Length; i++)
        {
            MeshFilter mesh = allChild[i].GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = allChild[i].GetComponent<MeshRenderer>();

            TextMesh text = allChild[i].GetComponent<TextMesh>();
            if (text)
                continue;

            if (mesh && meshRenderer)
            {
                Undo.RecordObject(allChild[i].gameObject, "layer change");
                Undo.RecordObject(meshRenderer, "materials change");

                allChild[i].gameObject.layer = LayerMask.NameToLayer("Walkable/Ground");
                var mat = AssetDatabase.LoadAssetAtPath("Assets/Resources/Ground.mat", typeof(Material));
                Debug.Log("mat: " + mat);
                meshRenderer.material = (Material)mat;
            }
        }        
    }
}