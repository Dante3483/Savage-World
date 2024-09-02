using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime
{
    [CreateAssetMenu(fileName = "NewState", menuName = "FSM/State")]
    public class StateSO : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private List<StateSO> _listOfChildren = new();
        [SerializeField]
        private string _guid;
        [SerializeField]
        public Vector2 Position;
        private Vector2 _prevPosition;
        #endregion

        #region Properties
        public List<StateSO> ListOfChildren
        {
            get
            {
                return _listOfChildren;
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

        #endregion

        #region Private Methods

        #endregion
    }
}