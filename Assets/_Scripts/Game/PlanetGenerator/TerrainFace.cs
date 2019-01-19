using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// every planete has 6 or more terrain face, with it's own mesh
/// //tuto: https://www.youtube.com/watch?v=QN39W020LqU
/// </summary>
public class TerrainFace
{
    ShapeGenerator shapeGenerator;
    Mesh mesh;  //mesh of the terrain
    int resolution; //resolution of the face

    Vector3 localUp;    //in witch way the terrain is facing ?
    //we have the up of the square, (Z), we need axis A and B now
    Vector3 axisA;      //vector right
    Vector3 axisB;

    /// <summary>
    /// constructor
    /// </summary>
    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = ExtQuaternion.CrossProduct(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertice = new Vector3[resolution * resolution];
        int[] triangle = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution; //same as just adding ++i in each x loop;
                Vector2 percent = new Vector2(x, y) / (resolution - 1); //where the vertexe should be laong the face
                //5:28 tuto
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized; //every verticle is equaly dist from center of sphere
                

                vertice[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);



                //now create triangles (7:21)

                //if we are not on the right/bottom edge
                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangle[triIndex] = i;
                    triangle[triIndex + 1] = i + resolution + 1;
                    triangle[triIndex + 2] = i + resolution;

                    triangle[triIndex + 3] = i;
                    triangle[triIndex + 4] = i + 1;
                    triangle[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();   //remove previous cash data (when we chance resolution, less verticel and more triangle)
        mesh.vertices = vertice;
        mesh.triangles = triangle;
        mesh.RecalculateNormals();

    }
}
