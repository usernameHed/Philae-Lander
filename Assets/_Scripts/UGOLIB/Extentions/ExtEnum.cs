using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtEnum
{
    public static int GetIndexOfEnum<T>(T enumValue)
    {
        int index = Array.IndexOf(Enum.GetValues(enumValue.GetType()), enumValue);
        return (index);
    }
    public static T GetEnumValueFromString<T>(string value, T enumType)
    {
        T parsed_enum = (T)System.Enum.Parse(typeof(T), value);
        return (parsed_enum);
    }

    public static string GetNameOfIndexEnumValue<T>(T enumType, string enumItemName)
    {
        T parsed_enum = (T)System.Enum.Parse(typeof(T), enumItemName);
        return (parsed_enum.ToString());
    }

    public static bool ListContain<T>(string tag, List<T> listEnum)
    {
        for (int i = 0; i < listEnum.Count; i++)
        {
            if (String.Equals(listEnum[i].ToString(), tag))
                return (true);
        }
        return (false);
    }
}
