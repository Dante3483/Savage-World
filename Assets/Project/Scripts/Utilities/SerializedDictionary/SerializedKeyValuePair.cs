using System;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.SerializedDictionary
{
    [Serializable]
    public struct SerializedKeyValuePair<TKey, TValue>
    {
        #region Fields
        [SerializeField]
        [SerializeReference]
        private TKey _key;
        [SerializeField]
        [SerializeReference]
        private TValue _value;
        #endregion

        #region Properties
        public TKey Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
            }
        }

        public TValue Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public SerializedKeyValuePair(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
