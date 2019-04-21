using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtNumbers
{
    /// <summary>
    /// return the average number between an array of points
    /// </summary>
    public static float GetAverageOfNumbers(float [] arrayNumber)
    {
        if (arrayNumber.Length == 0)
            return (0);

        float sum = 0;
        for (int i = 0; i < arrayNumber.Length; i++)
        {
            sum += arrayNumber[i];
        }
        return (sum / arrayNumber.Length);
    }
    /*
    /// <summary>
    /// copy the bytes of the specified int into the buffer
    /// </summary>
    /// <param name="value"></param>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public static unsafe void CopyBytes(int value, byte[] buffer, int offset)
    {
        // Here should be a range check. For example:
        if (offset + sizeof(int) > buffer.Length) throw new IndexOutOfRangeException();

        fixed (byte* numPtr = &buffer[offset])
            *(int*)numPtr = value;
    }
    /// <summary>
    /// copy the bytes of the specified float into the buffer
    /// </summary>
    /// <param name="value"></param>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    public static unsafe void CopyBytes(float value, byte[] buffer, int offset)
    {
        // Here should be a range check. For example:
        if (offset + sizeof(float) > buffer.Length) throw new IndexOutOfRangeException();

        fixed (byte* numPtr = &buffer[offset])
            *(float*)numPtr = value;
    }
    */
}
