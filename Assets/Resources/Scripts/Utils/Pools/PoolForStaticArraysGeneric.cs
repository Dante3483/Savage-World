using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolForStaticArraysGeneric<K, T>
{
    #region Private fields
    private Dictionary<K, T[]> _pool = new Dictionary<K, T[]>();
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public bool GetArray(K key, ref T[] buffer)
    {
        if (_pool.ContainsKey(key))
        {
            int i = 0;
            foreach(var item in _pool[key])
            {
                buffer[i] = item;
                i++;
            }
            return true;
        }
        return false;
    }

    public void SetArray(K key, T[] array)
    {
        _pool.Add(key, array);
        Debug.Log($"Write {key.ToString()} : {array.Length}");
    }
    #endregion
}
