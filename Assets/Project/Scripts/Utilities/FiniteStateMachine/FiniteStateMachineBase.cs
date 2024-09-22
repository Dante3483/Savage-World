using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine
{
    [CreateAssetMenu(fileName = "NewFiniteStateMachine", menuName = "FSM/FiniteStateMachine2s")]
    public class FiniteStateMachineBase : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private StateBase _starterState;
        [SerializeField]
        private StateBase _currentState;
        [SerializeField]
        private List<StateBase> _listOfStates;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public void FixedUpdate()
        {
            if (_currentState != null)
            {
                _currentState.FixedUpdate();
            }
        }

        public void Update()
        {
            if (_currentState != null)
            {
                _currentState.Update();
            }
        }

        public void MakeTranstition()
        {
            if (_currentState == null)
            {
                ChangeCurrentState(_starterState);
            }
            else
            {
                StateBase nextState = _currentState.GetNextTransitionState();
                ChangeCurrentState(nextState);
            }
        }

        public FiniteStateMachineBase Clone()
        {
            FiniteStateMachineBase instance = Instantiate(this);
            CloneStates(instance);
            return instance;
        }
        #endregion

        #region Private Methods
        private void ChangeCurrentState(StateBase nextState)
        {
            if (_currentState != null)
            {
                _currentState.Exit();
            }
            _currentState = nextState;
            if (_currentState != null)
            {
                _currentState.Enter();
            }
        }

        private void CloneStates(FiniteStateMachineBase instance)
        {
            Dictionary<StateBase, StateBase> cloneStateByOriginal = new();
            instance._listOfStates.Clear();
            foreach (StateBase state in _listOfStates)
            {
                StateBase stateClone = state.Clone();
                if (state == _starterState)
                {
                    instance._starterState = stateClone;
                }
                cloneStateByOriginal.Add(state, stateClone);
            }
            CloneTransitions(cloneStateByOriginal);
        }

        private void CloneTransitions(Dictionary<StateBase, StateBase> cloneStateByOriginal)
        {
            foreach (StateBase state in _listOfStates)
            {
                StateBase stateClone = cloneStateByOriginal[state];
                foreach (StateTransition transitionClone in stateClone.ListOfTransitions)
                {
                    transitionClone.State = cloneStateByOriginal[transitionClone.State];
                }
            }
        }
        #endregion
    }
}