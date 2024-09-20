using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.SerializedDictionary
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        #region Fields
        [SerializeField]
        private List<SerializedKeyValuePair<TKey, TValue>> _listOfKeyValuePair = new();
        #endregion

        #region Properties
        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }

            set
            {
                base[key] = value;
                bool isAnyEntryWasFound = false;
                for (int i = 0; i < _listOfKeyValuePair.Count; i++)
                {
                    var kvp = _listOfKeyValuePair[i];
                    if (!KeysAreEqual(key, kvp.Key))
                    {
                        continue;
                    }
                    isAnyEntryWasFound = true;
                    kvp.Value = value;
                    _listOfKeyValuePair[i] = kvp;
                }
                if (!isAnyEntryWasFound)
                {
                    _listOfKeyValuePair.Add(new(key, value));
                }
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public SerializedDictionary() : base()
        {

        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            _listOfKeyValuePair.Add(new(key, value));
        }

        public new void Clear()
        {
            base.Clear();
            _listOfKeyValuePair.Clear();
        }

        public new bool Remove(TKey key)
        {
            if (TryGetValue(key, out TValue value))
            {
                base.Remove(key);
                _listOfKeyValuePair.Remove(new(key, value));
                return true;
            }
            return false;
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            if (base.TryAdd(key, value))
            {
                _listOfKeyValuePair.Add(new(key, value));
                return true;
            }
            return false;
        }

        public void OnAfterDeserialize()
        {
            base.Clear();
            foreach (var kvp in _listOfKeyValuePair)
            {
#if UNITY_EDITOR
                if (IsValidKey(kvp.Key) && !ContainsKey(kvp.Key))
                {
                    base.Add(kvp.Key, kvp.Value);
                }
#else
                Add(kvp.Key, kvp.Value);
#endif

#if UNITY_EDITOR

#else
            _listOfKeyValuePair.Clear();
#endif
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR

#else
            _listOfKeyValuePair.Clear();
            foreach (var kvp in this)
            {
                _listOfKeyValuePair.Add(new(kvp.Key, kvp.Value));
            }
#endif
        }
        #endregion

        #region Private Methods
        private bool IsValidKey(object obj)
        {
            try
            {
                return obj != null || obj is UnityEngine.Object unityObj && unityObj != null;
            }
            catch
            {
                return false;
            }
        }

        private bool KeysAreEqual<T>(T key, object otherKey)
        {
            return (object)key == otherKey || key.Equals(otherKey);
        }
        #endregion
    }
}
