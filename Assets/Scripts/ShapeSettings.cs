using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1f;
    public NoiseLayer[] noiseLayers;


    [System.Serializable]
    public class NoiseLayer
    {
        public bool enable;
        //if it is checked, this layer's noise is overlayed on the first layer, otherwise
        // it is has separate noise settings
        public bool useFisrtLayerAsMask;
        public NoiseSettings noiseSettings;
    }
}
