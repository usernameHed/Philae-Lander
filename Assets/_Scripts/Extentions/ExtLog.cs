using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ExtLog
{
    public enum Log
    {
        BASE,
        IA
    }

    /// <summary>
    /// read every line in the file
    /// </summary>
    public static void DebugLogIa(object toDosiplay, Log logtype = Log.BASE)
    {
        if (GameManager.Instance.logType == logtype)
        {
            Debug.Log(toDosiplay);
        }
    }
}
