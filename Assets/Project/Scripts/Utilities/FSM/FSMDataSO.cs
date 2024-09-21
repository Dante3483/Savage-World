using SavageWorld.Runtime.Entities.NPC;
using SavageWorld.Runtime.Utilities.FSM.Conditions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FSM
{
    [CreateAssetMenu(fileName = "NewFiniteStateMachine", menuName = "FSM/FiniteStateMachineSO")]
    public class FSMDataSO : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private FSMStateSO _rootState;
        [SerializeField]
        private FSMStateSO _currentState;
        [SerializeField]
        private List<FSMStateSO> _listOfStates = new();
        private NPCBase _entity;
        #endregion

        #region Properties
        public List<FSMStateSO> ListOfStates
        {
            get
            {
                return _listOfStates;
            }
        }

        public FSMStateSO RootState
        {
            get
            {
                return _rootState;
            }
        }

        public FSMStateSO CurrentState
        {
            get
            {
                return _currentState;
            }

            set
            {
                if (_currentState != null)
                {
                    _currentState.Exit();
                }
                _currentState = value;
                _currentState.Enter();
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public void MoveToNextState()
        {
            foreach (FSMStateSO state in _currentState.ConditionByChild.Keys)
            {
                FSMConditionBase condition = _currentState.ConditionByChild[state];
                if (condition != null)
                {
                    if (condition.Check(_entity))
                    {

                        CurrentState = state;
                        return;
                    }
                }
                else
                {
                    CurrentState = state;
                }
            }
        }

        public FSMStateSO CreateState()
        {
            FSMStateSO state = CreateInstance<FSMStateSO>();
            state.name = typeof(FSMStateSO).Name;
            state.Guid = GUID.Generate().ToString();
            if (_rootState == null)
            {
                _rootState = state;
            }

            Undo.RecordObject(this, "FSM (Add State)");
            _listOfStates.Add(state);
            AssetDatabase.AddObjectToAsset(state, this);
            Undo.RegisterCreatedObjectUndo(state, "FSM (Add State)");
            AssetDatabase.SaveAssets();
            return state;
        }

        public void DeleteState(FSMStateSO state)
        {
            Undo.RecordObject(this, "FSM (Remove State)");
            _listOfStates.Remove(state);
            //AssetDatabase.RemoveObjectFromAsset(state);
            Undo.DestroyObjectImmediate(state);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(FSMStateSO parent, FSMStateSO child)
        {
            Undo.RecordObject(parent, "FSM (Add Child)");
            parent.ListOfChildren.Add(child);
            parent.ConditionByChild.Add(child, new NoCondition());
            EditorUtility.SetDirty(parent);
        }

        public void RemoveChild(FSMStateSO parent, FSMStateSO child)
        {
            Undo.RecordObject(parent, "FSM (Remove Child)");
            parent.ListOfChildren.Remove(child);
            parent.ConditionByChild.Remove(child);
            EditorUtility.SetDirty(parent);
        }

        public FSMDataSO Clone(NPCBase entity, GameObject gameObject)
        {
            Dictionary<FSMStateSO, FSMStateSO> cloneByOriginal = new();
            FSMDataSO instance = Instantiate(this);
            CreateStates(gameObject, cloneByOriginal, instance);
            SetupStates(cloneByOriginal);
            instance._entity = entity;
            instance.CurrentState = instance._rootState;
            return instance;
        }
        #endregion

        #region Private Methods
        private void CreateStates(GameObject gameObject, Dictionary<FSMStateSO, FSMStateSO> cloneByOriginal, FSMDataSO instance)
        {
            instance._listOfStates.Clear();
            _listOfStates.ForEach(state =>
            {
                FSMStateSO clone = state.Clone(gameObject);
                clone.ConditionByChild.Clear();
                clone.ListOfChildren.Clear();
                if (state == _rootState)
                {
                    instance._rootState = clone;
                }
                instance._listOfStates.Add(clone);
                cloneByOriginal.Add(state, clone);
            });
        }

        private void SetupStates(Dictionary<FSMStateSO, FSMStateSO> _cloneByOriginal)
        {
            _listOfStates.ForEach(state =>
            {
                FSMStateSO clone = _cloneByOriginal[state];
                state.ListOfChildren.ForEach(child =>
                {
                    FSMStateSO childClone = _cloneByOriginal[child];
                    clone.ListOfChildren.Add(childClone);
                    if (state.ConditionByChild.TryGetValue(child, out FSMConditionBase condition))
                    {
                        clone.ConditionByChild.Add(childClone, (FSMConditionBase)condition.Clone());
                    }
                });
            });
        }
        #endregion
    }
}