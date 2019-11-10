using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter  : INoiseFilter
{
    Noise noise = new Noise();
    NoiseSettings.SimpleNoiseSettings noiseSettings;

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings noiseSettings)
    {
        this.noiseSettings = noiseSettings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = noiseSettings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < noiseSettings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + noiseSettings.center);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= noiseSettings.roughness;
            amplitude *= noiseSettings.persistence;
        }

        noiseValue = noiseValue - noiseSettings.minValue;
        return noiseValue * noiseSettings.strength;
    }
}
