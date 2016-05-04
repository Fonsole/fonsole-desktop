using UnityEngine;
using System.Collections;

public class ArrayHelper
{
    public static void Shuffle<T>(T[] array)
    {
        int len = array.Length;
        while (len > 1)
        {
            int next = Random.Range(0, len);
            --len;

            T temp = array[next];
            array[next] = array[len];
            array[len] = temp;
        }
    }
}
