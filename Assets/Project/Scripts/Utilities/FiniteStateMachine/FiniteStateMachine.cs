using SavageWorld.Runtime.Entities.NPC;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine
{
    [CreateAssetMenu(fileName = "NewAI", menuName = "NPC/AI")]
    public class FiniteStateMachine : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private State _starterState;
        [SerializeField]
        private State _currentState;
        [SerializeField]
        private List<State> _listOfStates = new();
        #endregion

        #region Properties
        public State StarterState
        {
            get
            {
                return _starterState;
            }

            set
            {
                _starterState = value;
            }
        }

        public List<State> ListOfStates
        {
            get
            {
                return _listOfStates;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
#if UNITY_EDITOR
        public State AddState(string name = "")
        {
            Undo.RecordObject(this, "AddState");
            State state = CreateInstance<State>();
            state.name = "State";
            state.Name = name;
            state.Guid = GUID.Generate().ToString();
            _listOfStates.Add(state);
            AssetDatabase.AddObjectToAsset(state, this);
            AssetDatabase.SaveAssets();
            Undo.RegisterCreatedObjectUndo(state, "AddState");
            return state;
        }

        public void RemoveState(State state)
        {
            Undo.RecordObject(this, "RemoveState");
            _listOfStates.Remove(state);
            Undo.DestroyObjectImmediate(state);
            AssetDatabase.SaveAssets();
        }
#endif

        public void Initialize(NPCBase entityData)
        {
            _listOfStates.ForEach(state => state.EntityData = entityData);
        }

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
                State nextState = _currentState.GetNextTransitionState();
                ChangeCurrentState(nextState);
            }
        }

        public FiniteStateMachine Clone()
        {
            FiniteStateMachine instance = Instantiate(this);
            CloneStates(instance);
            return instance;
        }
        #endregion

        #region Private Methods
        private void ChangeCurrentState(State nextState)
        {
            if (nextState == _currentState)
            {
                return;
            }
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

        private void CloneStates(FiniteStateMachine instance)
        {
            Dictionary<State, State> cloneStateByOriginal = new();
            instance._listOfStates.Clear();
            foreach (State state in _listOfStates)
            {
                State stateClone = state.Clone();
                if (state == _starterState)
                {
                    instance._starterState = stateClone;
                }
                instance._listOfStates.Add(stateClone);
                cloneStateByOriginal.Add(state, stateClone);
            }
            CloneTransitions(cloneStateByOriginal);
        }

        private void CloneTransitions(Dictionary<State, State> cloneStateByOriginal)
        {
            foreach (State state in _listOfStates)
            {
                State stateClone = cloneStateByOriginal[state];
                foreach (StateTransition transitionClone in stateClone.ListOfTransitions)
                {
                    transitionClone.State = cloneStateByOriginal[transitionClone.State];
                }
            }
        }
        #endregion
    }
}