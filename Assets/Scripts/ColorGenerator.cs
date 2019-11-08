using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings colorSettings;
    Texture2D texture;
    const int textureResolution = 50;

    public void UpdateSettings(ColorSettings colorSettings)
    {
        this.colorSettings = colorSettings;

        if (texture == null) {
            texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        colorSettings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[textureResolution];
        for (int i = 0; i < textureResolution; i++)
        {
            colors[i] = colorSettings.gradient.Evaluate(i / (textureResolution - 1f));
        }

        texture.SetPixels(colors);
        texture.Apply();
        colorSettings.planetMaterial.SetTexture("_texture", texture);
    }
}
