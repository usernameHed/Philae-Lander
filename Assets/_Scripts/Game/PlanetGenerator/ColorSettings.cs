using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public Color planetColor = Color.blue;
    public Material planetMaterial;
}
