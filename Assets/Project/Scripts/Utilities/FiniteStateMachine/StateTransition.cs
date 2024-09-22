using SavageWorld.Runtime.Utilities.FiniteStateMachine.Conditions;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine
{
    [Serializable]
    public class StateTransition
    {
        #region Fields
        [SerializeField]
        private StateBase _state;
        [SerializeField]
        [SerializeReference]
        private TransitionConditionBase _condition;
        #endregion

        #region Properties
        public StateBase State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
            }
        }

        public TransitionConditionBase Condition
        {
            get
            {
                return _condition;
            }

            set
            {
                _condition = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public bool CheckTransition()
        {
            return _condition.Check();
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
