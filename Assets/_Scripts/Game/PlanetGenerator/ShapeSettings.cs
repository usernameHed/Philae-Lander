using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1f;
    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enable = true;
        public bool useFirstLayerAsMask;
        public NoiseSettings noiseSettings;
    }

}
