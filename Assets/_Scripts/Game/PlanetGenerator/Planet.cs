using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;
    public enum FaceRenderMask
    {
        ALL,
        TOP,
        BOTTOM,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    }
    public FaceRenderMask faceRenderMaks;

    //[InlineEditor, OnValueChanged("OnShapeSettingsUpdate")]
    public ShapeSettings shapeSettings;
    //[InlineEditor, OnValueChanged("OnColorSettingsUpdate")]
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;


    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;

    private TerrainFace[] terrainFace;
    private ShapeGenerator shapeGenerator;
    private static int sizeMesh = 6;

    private void OnValidate()
    {
        GeneratePlanet();
    }

    private void Init()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[sizeMesh];
        }
        terrainFace = new TerrainFace[sizeMesh];
        Vector3[] direction = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        transform.ClearChild();

        for (int i = 0; i < sizeMesh; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.SetParent(gameObject.transform);

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            terrainFace[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, direction[i]);

            bool renderFace = faceRenderMaks == FaceRenderMask.ALL || (int)faceRenderMaks - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    [Button]
    public void GeneratePlanet()
    {
        Init();
        GenerateMesh();
        GenerateColor();
    }

    /// <summary>
    /// called when changing shape in scriptableObject
    /// </summary>
    public void OnShapeSettingsUpdate()
    {
        Init();
        GenerateMesh();
    }

    /// <summary>
    /// called when changing Color in scriptableObject
    /// </summary>
    public void OnColorSettingsUpdate()
    {
        Init();
        GenerateColor();
    }
    
    private void GenerateMesh()
    {
        for (int i = 0; i < sizeMesh; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFace[i].ConstructMesh();
            }
        }
    }

    private void GenerateColor()
    {
        foreach(MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColor;
        }
    }
}
