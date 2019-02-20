using UnityEngine;
using UnityEditor;

public class AssetGenerator : ScriptableObject
{
    [MenuItem("PERSO/Meshs Modifier/Separate Faces")]
    public static void SeparateFaces()
    {
        Transform[] obj = Selection.transforms;
        for (int i = 0; i < obj.Length; i++)
        {
            MeshFilter mesh = obj[i].GetComponent<MeshFilter>();
            SepatateFace(mesh);
        }
    }

    private static void SepatateFace(MeshFilter mesh)
    {
        Debug.Log(mesh.gameObject.name);
    }
}