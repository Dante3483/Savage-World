using SavageWorld.Runtime.Utilities.FSM.Actions;
using SavageWorld.Runtime.Utilities.FSM.Conditions;
using SavageWorld.Runtime.Utilities.SerializedDictionary;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FSM
{
    [CreateAssetMenu(fileName = "NewState", menuName = "FSM/State")]
    public class FSMStateSO : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private string _stateName;
        [SerializeField]
        private List<FSMStateSO> _listOfChildren = new();
        [SerializeField]
        private SerializedDictionary<FSMStateSO, FSMConditionBase> _conditionByChild = new();
        //private Dictionary<FSMStateSO, FSMConditionBase> _conditionByChild = new();

        [SerializeField]
        [SerializeReference]
        private List<FSMActionBase> _listOfActionOnEnter = new();
        [SerializeField]
        [SerializeReference]
        private List<FSMActionBase> _listOfActionOnExit = new();
        [SerializeField]
        [SerializeReference]
        private List<FSMActionBase> _listOfActionOnFixedUpdate = new();
        [SerializeField]
        [SerializeReference]
        private List<FSMActionBase> _listOfActionOnUpdate = new();
        [SerializeField]
        private string _guid;
        [SerializeField]
        public Vector2 Position;
        private Vector2 _prevPosition;
        #endregion

        #region Properties
        public string StateName
        {
            get
            {
                return _stateName;
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

        public List<FSMActionBase> ListOfActionOnEnter
        {
            get
            {
                return _listOfActionOnEnter;
            }
        }

        public List<FSMActionBase> ListOfActionOnExit
        {
            get
            {
                return _listOfActionOnExit;
            }
        }

        public List<FSMActionBase> ListOfActionOnFixedUpdate
        {
            get
            {
                return _listOfActionOnFixedUpdate;
            }
        }

        public List<FSMActionBase> ListOfActionOnUpdate
        {
            get
            {
                return _listOfActionOnUpdate;
            }
        }
        #endregion

        #region Events / Delegates
        public Action<Vector2> PositionChanged;
        #endregion

        #region Monobehaviour Methods
        private void OnValidate()
        {
            if (_prevPosition != Position)
            {
                PositionChanged?.Invoke(Position);
                _prevPosition = Position;
            }
        }
        #endregion

        #region Public Methods
        public void Enter()
        {
            _listOfActionOnEnter.ForEach(action => action.Invoke());
        }

        public void Exit()
        {
            _listOfActionOnExit.ForEach(action => action.Invoke());
        }

        public void FixedUpdate()
        {
            _listOfActionOnFixedUpdate.ForEach(action => action.Invoke());
        }

        public void Update()
        {
            _listOfActionOnUpdate.ForEach(action => action.Invoke());
        }

        public FSMStateSO Clone(GameObject gameObject)
        {
            FSMStateSO instance = Instantiate(this);
            //_listOfChildren.ForEach(child =>
            //{
            //    FSMStateSO clone = child.Clone(gameObject);
            //    if (_conditionByChild.TryGetValue(child, out FSMConditionBase condition))
            //    {
            //        instance._conditionByChild.Add(clone, (FSMConditionBase)condition.Clone());
            //    }
            //    instance._listOfChildren.Add(clone);
            //});
            //instance._listOfChildren = _listOfChildren.ConvertAll(child => child.Clone(gameObject));
            instance.Initialize(gameObject);
            return instance;
        }
        #endregion

        #region Private Methods
        private void Initialize(GameObject gameObject)
        {
            _listOfActionOnEnter.ForEach(action => action.Initialize(gameObject));
            _listOfActionOnExit.ForEach(action => action.Initialize(gameObject));
            _listOfActionOnFixedUpdate.ForEach(action => action.Initialize(gameObject));
            _listOfActionOnUpdate.ForEach(action => action.Initialize(gameObject));
        }
        #endregion
    }
}