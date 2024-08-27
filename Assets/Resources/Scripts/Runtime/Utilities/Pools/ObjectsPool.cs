using System;
using System.Collections.Generic;

namespace SavageWorld.Runtime.Utilities.Pools
{
    public class ObjectsPool<T>
    {
        #region Fields
        private readonly Stack<T> _pool;
        private readonly Func<T> _createInstance;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public ObjectsPool(Func<T> createInstance, int starterSize = 0)
        {
            _pool = new Stack<T>();
            _createInstance = createInstance;
            if (starterSize > 0)
            {
                for (int i = 0; i < starterSize; i++)
                {
                    _pool.Push(createInstance());
                }
            }
        }

        public T Get()
        {
            return _pool.Count == 0 ? _createInstance.Invoke() : _pool.Pop();
        }

        public void Release(T obj)
        {
            _pool.Push(obj);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
