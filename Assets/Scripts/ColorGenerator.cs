using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings colorSettings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;


    public void UpdateSettings(ColorSettings colorSettings)
    {
        this.colorSettings = colorSettings;

        if (texture == null || texture.height != colorSettings.biomeColorSettings.biomes.Length) {
            texture = new Texture2D(textureResolution, colorSettings.biomeColorSettings.biomes.Length);
        }

        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(colorSettings.biomeColorSettings.noise);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        colorSettings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }


    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - colorSettings.biomeColorSettings.noiseOffset) * colorSettings.biomeColorSettings.noiseStrength;
        float biomeIndex = 0;
        int numBiomes = colorSettings.biomeColorSettings.biomes.Length;
        float blendRange = colorSettings.biomeColorSettings.blendAmount / 2f + 0.001f;

        for (int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - colorSettings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }

                            //in case it gets 0
        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];
        int colorIndex = 0;

        foreach (var biome in colorSettings.biomeColorSettings.biomes) 
        {
            for (int i = 0; i < textureResolution; i++)
            {
                Color gradientCol = biome.gradient.Evaluate(i / (textureResolution - 1f));
                Color tintCol = biome.tint;
                colors[colorIndex] = gradientCol * (1 - biome.tintPercent) + tintCol * biome.tintPercent;
                colorIndex++;
            }
        }
        texture.SetPixels(colors);
        texture.Apply();
        colorSettings.planetMaterial.SetTexture("_texture", texture);
    }
}
