using SavageWorld.Runtime.Utilities.FiniteStateMachine.Actions;
using SavageWorld.Runtime.Utilities.FiniteStateMachine.Conditions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine
{
    public class StateBase : ScriptableObject
    {
        #region Fields
#if UNITY_EDITOR
        [SerializeField]
        private Vector2 _position;
#endif

        [SerializeField]
        private string _name;
        [SerializeField]
        private string _guid;
        [SerializeField]
        private List<StateTransition> _listOfTransitions = new();
        [SerializeField]
        [SerializeReference]
        private List<StateActionBase> _listOfEnterActions = new();
        [SerializeField]
        [SerializeReference]
        private List<StateActionBase> _listOfExitActions = new();
        [SerializeField]
        [SerializeReference]
        private List<StateActionBase> _listOfFixedUpdateActions = new();
        [SerializeField]
        [SerializeReference]
        private List<StateActionBase> _listOfUpdateActions = new();
        #endregion

        #region Properties
#if UNITY_EDITOR
        public Vector2 Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }
#endif

        public List<StateTransition> ListOfTransitions
        {
            get
            {
                return _listOfTransitions;
            }
        }

        public List<StateActionBase> ListOfEnterActions
        {
            get
            {
                return _listOfEnterActions;
            }
        }

        public List<StateActionBase> ListOfExitActions
        {
            get
            {
                return _listOfExitActions;
            }
        }

        public List<StateActionBase> ListOfFixedUpdateActions
        {
            get
            {
                return _listOfFixedUpdateActions;
            }
        }

        public List<StateActionBase> ListOfUpdateActions
        {
            get
            {
                return _listOfUpdateActions;
            }
        }
        #endregion

        #region Events / Delegates
        public event Action<StateBase> Entered;
        public event Action<StateBase> Exited;
        public event Action<StateBase> FixedUpdated;
        public event Action<StateBase> Updated;
        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
#if UNITY_EDITOR
        public void ChangeCondition(StateBase state, TransitionConditionBase condition)
        {
            Undo.RecordObject(this, "ChangeCondition");
            StateTransition transition = _listOfTransitions.Find(t => t.State == state);
            if (transition != null)
            {
                transition.Condition = condition;
            }
            EditorUtility.SetDirty(this);
        }
#endif

        public void Enter()
        {
            OnEntered();
        }

        public void Exit()
        {
            OnExited();
        }

        public void FixedUpdate()
        {
            OnFixedUpdated();
        }

        public void Update()
        {
            OnUpdated();
        }

        public StateBase GetNextTransitionState()
        {
            foreach (StateTransition transition in _listOfTransitions)
            {
                if (transition.CheckTransition())
                {
                    return transition.State;
                }
            }
            return this;
        }

        public StateBase Clone()
        {
            StateBase instance = Instantiate(this);
            instance._listOfTransitions.Clear();
            return instance;
        }
        #endregion

        #region Private Methods
        private void OnEntered()
        {
            Entered?.Invoke(this);
        }

        private void OnExited()
        {
            Entered?.Invoke(this);
        }

        private void OnFixedUpdated()
        {
            Entered?.Invoke(this);
        }

        private void OnUpdated()
        {
            Entered?.Invoke(this);
        }
        #endregion
    }
}