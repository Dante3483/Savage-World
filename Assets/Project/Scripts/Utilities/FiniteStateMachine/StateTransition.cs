using SavageWorld.Runtime.Entities.NPC;
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
        private State _state;
        [SerializeField]
        [SerializeReference]
        private TransitionConditionBase _condition;
        #endregion

        #region Properties
        public State State
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
        public StateTransition(State state, TransitionConditionBase condition)
        {
            _state = state;
            _condition = condition;
        }

        public bool CheckTransition(NPCBase entityData)
        {
            return _condition.Check(entityData);
        }

        public void ResetCondition()
        {
            _condition.Reset();
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
