﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)] // 256 is the maximum number possible
    public int resolution = 10;
    public bool showVertecies = false;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters; // holds all 6 sides

    TerrainFace[] terrainFaces;


    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        //create it once, and later update it ONLY
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6]; //a cube has 6 sided
        }

        terrainFaces = new TerrainFace[6];

        //used as localUp or side, that the `terrainFaces[i]` use it as the base direction
        Vector3[] directions = {
                    Vector3.up, Vector3.down,
                    Vector3.left, Vector3.right,
                    Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh" + i);
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }


    //show vertecies by dots
    private void OnDrawGizmos()
    {
        Color[] gizmoColors = { Color.red, Color.blue, Color.green, Color.cyan, Color.gray, Color.magenta };

        if (showVertecies)
        {
            for (int f = 0; f < 6; f++)
            {
                if (terrainFaces[f].vertices != null)
                {
                    Vector3[] vs = terrainFaces[f].vertices;
                    Gizmos.color = gizmoColors[f];

                    for (int i = 0; i < vs.Length; i++)
                    {
                        Gizmos.DrawSphere(vs[i], 0.05f);
                    }
                }
            }
        }
        
    }
}