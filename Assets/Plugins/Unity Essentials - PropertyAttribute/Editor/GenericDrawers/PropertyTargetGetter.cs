using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials.PropertyAttribute.generic
{
    public static class PropertyTargetGetter
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public static T GetTargetObject<T>(SerializedProperty property)
        {
            object targetObject = property.serializedObject.targetObject;
            int pathPosition = 0;
            string[] allPaths = property.propertyPath.Split('.');
            while (pathPosition < allPaths.Length)
            {
                string path = allPaths[pathPosition];
                Type type = targetObject.GetType();                
                FieldInfo field = type.GetField(path, BINDING_FLAGS);
                if (type.IsArray)
                {
                    pathPosition = FindObjectInArray(allPaths, pathPosition, ref targetObject);
                }
                else
                {
                    targetObject = field.GetValue(targetObject);
                }
                pathPosition++;
            }
            return (T)targetObject;
        }

        private static int FindObjectInArray(string[] allPaths, int pathPosition, ref object targetObject)
        {
            string path;
            Array array = (Array)targetObject;
            pathPosition++;
            path = allPaths[pathPosition];
            int firstPosition = path.IndexOf('[');
            int secondPosition = path.IndexOf(']');
            string positionInArrayAsString = path.Substring(firstPosition + 1, secondPosition - firstPosition - 1);
            int positionInArray = int.Parse(positionInArrayAsString);
            targetObject = array.GetValue(positionInArray);
            return pathPosition;
        }
    }
}
