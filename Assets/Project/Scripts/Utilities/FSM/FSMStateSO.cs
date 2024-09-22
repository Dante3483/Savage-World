using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Utilities.FSM.Actions;
using SavageWorld.Runtime.Utilities.FSM.Conditions;
using SavageWorld.Runtime.Utilities.SerializedDictionary;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FSM
{
    [CreateAssetMenu(fileName = "NewState", menuName = "FSM/State")]
    [FSMComponent("State", "")]
    public class FSMStateSO : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _guid;
        [SerializeField]
        private Vector2 _position;
        [SerializeField]
        private List<FSMStateSO> _listOfChildren = new();
        [SerializeField]
        private SerializedDictionary<FSMStateSO, FSMConditionBase> _conditionByChild = new();

        [SerializeField]
        [SerializeReference]
        private List<FSMActionBase> _listOfActionsOnEnter = new();
        [SerializeField]
        [SerializeReference]
        private List<FSMActionBase> _listOfActionsOnExit = new();
        [SerializeField]
        [SerializeReference]
        private List<FSMActionBase> _listOfActionsOnFixedUpdate = new();
        [SerializeField]
        [SerializeReference]
        private List<FSMActionBase> _listOfActionsOnUpdate = new();
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public List<FSMStateSO> ListOfChildren
        {
            get
            {
                return _listOfChildren;
            }
        }

        public SerializedDictionary<FSMStateSO, FSMConditionBase> ConditionByChild
        {
            get
            {
                return _conditionByChild;
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

        public List<FSMActionBase> ListOfActionsOnEnter
        {
            get
            {
                return _listOfActionsOnEnter;
            }
        }

        public List<FSMActionBase> ListOfActionsOnExit
        {
            get
            {
                return _listOfActionsOnExit;
            }
        }

        public List<FSMActionBase> ListOfActionsOnFixedUpdate
        {
            get
            {
                return _listOfActionsOnFixedUpdate;
            }
        }

        public List<FSMActionBase> ListOfActionsOnUpdate
        {
            get
            {
                return _listOfActionsOnUpdate;
            }
        }

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
        #endregion

        #region Events / Delegates
        public Action<Vector2> PositionChanged;
        public Action<FSMStateSO> Entered;
        public Action<FSMStateSO> Exited;
        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public void AddCondition(FSMStateSO child, FSMConditionBase condition)
        {
            Undo.RecordObject(this, "FSM (Add Condition)");
            _conditionByChild[child] = condition;
            EditorUtility.SetDirty(this);
        }

        public void Enter()
        {
            _listOfActionsOnEnter.ForEach(action => action.Invoke());
            Entered?.Invoke(this);
        }

        public void Exit()
        {
            _listOfActionsOnExit.ForEach(action => action.Invoke());
            foreach (FSMConditionBase condition in _conditionByChild.Values)
            {
                condition.Reset();
            }
            Exited?.Invoke(this);
        }

        public void FixedUpdate()
        {
            _listOfActionsOnFixedUpdate.ForEach(action => action.Invoke());
        }

        public void Update()
        {
            _listOfActionsOnUpdate.ForEach(action => action.Invoke());
        }

        public FSMStateSO Clone(GameObject gameObject)
        {
            FSMStateSO instance = Instantiate(this);
            instance.Initialize(gameObject);
            return instance;
        }
        #endregion

        #region Private Methods
        private void Initialize(GameObject gameObject)
        {
            _listOfActionsOnEnter.ForEach(action => action.Initialize(gameObject));
            _listOfActionsOnExit.ForEach(action => action.Initialize(gameObject));
            _listOfActionsOnFixedUpdate.ForEach(action => action.Initialize(gameObject));
            _listOfActionsOnUpdate.ForEach(action => action.Initialize(gameObject));
        }
        #endregion
    }
}