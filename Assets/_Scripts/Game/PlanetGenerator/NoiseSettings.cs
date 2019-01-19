using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType
    {
        SIMPLE,
        RIGID,
    }
    public FilterType filterType;
    [ConditionalHide("filterType", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)]
    public RigidNoiseSettings rigidNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings
    {
        public float strenght = 1;
        [Range(1, 8)]
        public int numLayer = 1;
        public float baseRoughness = 1;
        public float roughness = 2;
        public float persistence = 0.5f;

        public Vector3 center;
        public float minValue = 1f;
    }

    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {
        public float weightMultiplier = 0.8f;
    }

    
}
