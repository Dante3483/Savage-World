using SavageWorld.Runtime.Entities.NPC;
using SavageWorld.Runtime.Utilities.FiniteStateMachine.Actions;
using SavageWorld.Runtime.Utilities.FiniteStateMachine.Conditions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine
{
    public class State : ScriptableObject
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
        private NPCBase _entityData;
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

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string Guid
        {
            get
            {
                return _guid;
            }

            set
            {
                _guid = value;
            }
        }

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

        public NPCBase EntityData
        {
            get
            {
                return _entityData;
            }

            set
            {
                _entityData = value;
            }
        }
        #endregion

        #region Events / Delegates
        public event Action<State> Entered;
        public event Action<State> Exited;
        public event Action<State> FixedUpdated;
        public event Action<State> Updated;
        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
#if UNITY_EDITOR
        public void AddTransition(State state)
        {
            Undo.RecordObject(this, "AddTransition");
            _listOfTransitions.Add(new(state, new NoCondition()));
            EditorUtility.SetDirty(this);
        }

        public void RemoveTransition(State state)
        {
            Undo.RecordObject(this, "RemoveTransition");
            StateTransition transition = FindTransition(state);
            if (transition != null)
            {
                _listOfTransitions.Remove(transition);
            }
            EditorUtility.SetDirty(this);
        }

        public void ChangeCondition(State state, TransitionConditionBase condition)
        {
            Undo.RecordObject(this, "ChangeCondition");
            StateTransition transition = FindTransition(state);
            if (transition != null)
            {
                transition.Condition = condition;
            }
            EditorUtility.SetDirty(this);
        }

        public StateTransition FindTransition(State state)
        {
            return _listOfTransitions.Find(t => t.State == state);
        }
#endif

        public void Enter()
        {
            _listOfEnterActions.ForEach(action => action.Invoke(_entityData));
            OnEntered();
        }

        public void Exit()
        {
            _listOfExitActions.ForEach(action => action.Invoke(_entityData));
            OnExited();
        }

        public void FixedUpdate()
        {
            _listOfFixedUpdateActions.ForEach(action => action.Invoke(_entityData));
            OnFixedUpdated();
        }

        public void Update()
        {
            _listOfUpdateActions.ForEach(action => action.Invoke(_entityData));
            OnUpdated();
        }

        public State GetNextTransitionState()
        {
            foreach (StateTransition transition in _listOfTransitions)
            {
                if (transition.CheckTransition(_entityData))
                {
                    ResetAllTransitions();
                    return transition.State;
                }
            }
            return this;
        }

        public State Clone()
        {
            State instance = Instantiate(this);
            return instance;
        }
        #endregion

        #region Private Methods
        private void ResetAllTransitions()
        {
            _listOfTransitions.ForEach(t => t.ResetCondition());
        }

        private void OnEntered()
        {
            Entered?.Invoke(this);
        }

        private void OnExited()
        {
            Exited?.Invoke(this);
        }

        private void OnFixedUpdated()
        {
            FixedUpdated?.Invoke(this);
        }

        private void OnUpdated()
        {
            Updated?.Invoke(this);
        }
        #endregion
    }
}