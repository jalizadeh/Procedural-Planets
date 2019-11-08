using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This cals create a rectangular face that it's resolution can vary.
 * By using 6 faces, a cube can be created
 */ 
public class TerrainFace
{
    Mesh mesh;
    ShapeGenerator shapeGenerator;
    int resolution; //number of vertecies on the mesh
    Vector3 localUp; //to which side it is looking at

    Vector3 axisA;
    Vector3 axisB;

    public Vector3[] vertices;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.shapeGenerator = shapeGenerator;
        this.resolution = resolution;
        this.localUp = localUp; //facing (out) axis

        //this axis is calculated based on vertex placment, in tutorial we place each vertex to the right,
        // then go down. We only have the facing axis (localUp), so based on it, we can calculate the rest 
        // axis to comply with our vertext placement rule.
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);

        // The third axis can be easily calculated by Cross Product technique
        //`axisB` is perpendicular to both `localUp` and `axisA`
        //https://en.wikipedia.org/wiki/Cross_product
        axisB = Vector3.Cross(localUp, axisA);

        /*
         * localUp                  axisA                   axisB
         * -------------------------------------------------------------------
         * ( 0.0,  1.0,  0.0)       ( 1.0,  0.0,  0.0)      ( 0.0,  0.0, -1.0)
         * ( 0.0, -1.0,  0.0)       (-1.0,  0.0,  0.0)      ( 0.0,  0.0, -1.0)
         * (-1.0,  0.0,  0.0)       ( 0.0,  0.0, -1.0)      ( 0.0, -1.0,  0.0)
         * ( 1.0,  0.0,  0.0)       ( 0.0,  0.0,  1.0)      ( 0.0, -1.0,  0.0)
         * ( 0.0,  0.0,  1.0)       ( 0.0,  1.0,  0.0)      (-1.0,  0.0,  0.0)
         * ( 0.0,  0.0, -1.0)       ( 0.0, -1.0,  0.0)      (-1.0,  0.0,  0.0)
         */ 
        //Debug.Log(localUp + " / " + axisA + " / " + axisB);
    }


    public void ConstructMesh() {
        //array of vertices = r^2
        vertices = new Vector3[resolution * resolution];

        //total number of meshes (3 connected vertecies) = (r-1)^2 * 2
        //total number of meshes * (vertecies each mesh uses) = ((r-1)^2 * 2) * 3
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 2 * 3];
        int triIndex = 0;
        Vector2[] uv = mesh.uv;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                //index counter
                int i = x + y * resolution;

                //it varies between 0-1
                //starts with 0, each time added by a fraction
                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                //the percent usage is very tricky and nice
                Vector3 pointOnUnitCube = 
                        localUp  //point position on the facing axis
                        + (percent.x - 0.5f) * 2 * axisA  //point position on the "relative" +x
                        + (percent.y - 0.5f) * 2 * axisB; //point position on the "relative" -y

                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                //vertices[i] = pointOnUnitCube;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                //I can not create any mesh on the edge
                if (x != resolution -1 && y!=resolution - 1)
                {
                    //first triangle
                    triangles[triIndex + 0] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    //second triangle
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    triIndex += 6;
                }
            }
        }

        mesh.Clear(); //clear previous data
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }



    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        Vector2[] uv = new Vector2[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                //index counter
                int i = x + y * resolution;

                //it varies between 0-1
                //starts with 0, each time added by a fraction
                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                //the percent usage is very tricky and nice
                Vector3 pointOnUnitCube =
                        localUp  //point position on the facing axis
                        + (percent.x - 0.5f) * 2 * axisA  //point position on the "relative" +x
                        + (percent.y - 0.5f) * 2 * axisB; //point position on the "relative" -y

                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                uv[i] = new Vector2(colorGenerator.BiomePercentFromPoint(pointOnUnitSphere),0);
                
            }
        }

        mesh.uv = uv;
    }
}
