using UnityEngine;

namespace SavageWorld.Runtime.Utilities.StateMachine
{
    public class StateMachine<T> where T : StateBase
    {
        #region Private fields
        private string _name;
        private T _currentState;
        private T _prevState;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public T CurrentState
        {
            get
            {
                return _currentState;
            }
        }

        public T PrevState
        {
            get
            {
                return _prevState;
            }
        }
        #endregion

        #region Methods
        public StateMachine(string name)
        {
            _name = name;
        }

        public void ChangeState(T nextState)
        {
            PrintMessage(nextState);
            if (_currentState != null)
            {
                _currentState.Exit();
                _prevState = _currentState;
            }
            _currentState = nextState;
            _currentState?.Enter();
        }

        private void PrintMessage(T nextState)
        {
            if (_currentState == null && nextState != null)
            {
                Debug.Log($"{_name}: Changed state to {nextState.GetType().Name}.");
            }
            else if (nextState != null)
            {
                Debug.Log($"{_name}: Changed state from {_currentState.GetType().Name} to {nextState.GetType().Name}.");
            }
            else
            {
                Debug.Log($"{_name}: Changed state from {_currentState.GetType().Name} to NULL.");
            }
        }
        #endregion
    }
}