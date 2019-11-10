using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back};
    public FaceRenderMask faceRenderMask;

    [Range(2, 256)] // 256 is the maximum number possible
    public int resolution = 10;

    public bool showVertecies = false;
    [Range(0.001f, 0.05f)]
    public float vertexGizmoSize = 0.01f;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters; // holds all 6 sides

    TerrainFace[] terrainFaces;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    /* used for closing/openning their editor in `PlanetEditor`
     * these can't be stored in `PlanetEditor`, so they are here
     * and accessed by `ref`, so the change can be stored here
     */
    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    //if disabled, you have to press "Update Planet" to manually update the settings
    public bool autoUpdate;


    //loaded on Game mode
    private void Start()
    {
        GeneratePlanet();
    }


    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

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

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<Renderer>().sharedMaterial = colorSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator,meshFilters[i].sharedMesh, resolution, directions[i]);

            //For optimizing rendering faces, So now I can choose to render all or one face at the time
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }


    public void GeneratePlanet() {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }


    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }


    void GenerateColors() {
        colorGenerator.UpdateColors();

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colorGenerator);
            }
        }
    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
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
                        Gizmos.DrawSphere(vs[i], vertexGizmoSize);
                    }
                }
            }
        }
        
    }
}
