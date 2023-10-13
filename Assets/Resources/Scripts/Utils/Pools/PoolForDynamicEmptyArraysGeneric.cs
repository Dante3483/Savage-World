using System.Collections.Generic;

public class PoolForDynamicEmptyArraysGeneric<T>
{
    #region Private fields
    private Dictionary<int, T[]> _pool = new Dictionary<int, T[]>();
    private T[] _array;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public T[] GetArray(int size)
    {
        if (_pool.ContainsKey(size))
        {
            _array = _pool[size];
        }
        else
        {
            _array = new T[size];
            _pool.Add(size, _array);
        }
        return _array;
    }
    #endregion
}
