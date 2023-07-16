using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayObjectPool<T>
{
    private List<T[]> pool = new List<T[]>();

    public T[] GetArray(int size)
    {
        T[] array;
        if (pool.Count > 0)
        {
            array = pool.Find(a => a.Length == size);
            if (array == null)
            {
                array = new T[size];
                pool.Add(array);
            }
            return array;
        }
        else
        {
            array = new T[size];
            pool.Add(array);
            return array;
        }
    }
}
