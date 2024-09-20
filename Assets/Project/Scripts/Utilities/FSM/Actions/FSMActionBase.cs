using System;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FSM.Actions
{
    [Serializable]
    public abstract class FSMActionBase
    {
        #region Fields
        [SerializeField]
        [HideInInspector]
        protected string _name;
        #endregion

        #region Properties
        public abstract string Name { get; }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public FSMActionBase()
        {
            _name = Name;
        }

        public abstract void Initialize(GameObject gameObject);

        public abstract void Invoke();
        #endregion

        #region Private Methods

        #endregion
    }
}
