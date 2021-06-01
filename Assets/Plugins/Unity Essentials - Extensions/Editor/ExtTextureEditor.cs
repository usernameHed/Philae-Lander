using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.Extensions.Editor
{
    public static class ExtTextureEditor
    {
        public static Texture GetTexture(string imageName)
        {
            Texture texture = (Texture)EditorGUIUtility.Load("SceneView/" + imageName + ".png");
            return (texture);
        }

        public static Texture2D GetTexture2D(string imageName)
        {
            Texture2D texture = (Texture2D)EditorGUIUtility.Load("SceneView/" + imageName + ".png");
            return (texture);
        }
    }
}