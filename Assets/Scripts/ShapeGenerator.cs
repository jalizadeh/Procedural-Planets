using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;
    NoiseFilter[] noiseFilters;

    public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new NoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < settings.noiseLayers.Length; i++)
        {
            noiseFilters[i] = new NoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
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
        return pointOnUnitSphere * settings.planetRadius * (1+  elevation); 
    }
}
