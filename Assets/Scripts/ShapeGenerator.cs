using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;
    INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < settings.noiseLayers.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }

        elevationMinMax = new MinMax();
    }

    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float elevation = 0;
        float firstNoiseElevation = 0;

        if(noiseFilters.Length > 0)
        {
            if (settings.noiseLayers[0].enable)
            {
                firstNoiseElevation = noiseFilters[0].Evaluate(pointOnUnitSphere);
                elevation = firstNoiseElevation;
            }
        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enable)
            {
                float mask = (settings.noiseLayers[i].useFisrtLayerAsMask) ? firstNoiseElevation : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
                
            }
        }

        //find the min & max values
        elevationMinMax.AddValue(elevation);

        return elevation; 
    }

    public float GetScaledElevation(float unscaledElevation)
    {
        //always [0, unscaledElevation]
        float elevation = Mathf.Max(0, unscaledElevation);
        elevation = settings.planetRadius * (1 + elevation);
        return elevation;
    }
}
