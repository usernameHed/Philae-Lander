using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtList
{
    /// <summary>
    /// renvoi vrai si dans la liste, il y a un mot qui est substring du fileName
    /// </summary>
    /// <param name="toTransform"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static int ContainSubStringInList(List<string> toTransform, string fileName)
    {
        for (int i = 0; i < toTransform.Count; i++)
        {
            if (fileName.Contains(toTransform[i]))
                return (i);
        }
        return (-1);
    }

    public static int ContainSubStringInArray(string [] toTransform, string fileName)
    {
        for (int i = 0; i < toTransform.Length; i++)
        {
            if (fileName.Contains(toTransform[i]))
                return (i);
        }
        return (-1);
    }

    public static List<Transform> FillListFromChilds(Transform parent)
    {
        List<Transform> allChild = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            allChild.Add(parent.GetChild(0));
        }
        return (allChild);
    }
}
