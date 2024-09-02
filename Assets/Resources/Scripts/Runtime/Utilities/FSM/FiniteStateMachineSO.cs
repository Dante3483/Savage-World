using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FSM
{
    [CreateAssetMenu(fileName = "NewFiniteStateMachine", menuName = "FSM/FiniteStateMachineSO")]
    public class FiniteStateMachineSO : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private StateSO _rootState;
        [SerializeField]
        private List<StateSO> _listOfStates = new();
        #endregion

        #region Properties
        public List<StateSO> ListOfStates
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
        public StateSO CreateState()
        {
            StateSO state = CreateInstance<StateSO>();
            state.name = typeof(StateSO).Name;
            state.Guid = GUID.Generate().ToString();
            _listOfStates.Add(state);
            AssetDatabase.AddObjectToAsset(state, this);
            AssetDatabase.SaveAssets();
            return state;
        }

        public void DeleteState(StateSO state)
        {
            _listOfStates.Remove(state);
            AssetDatabase.RemoveObjectFromAsset(state);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(StateSO parent, StateSO child)
        {
            parent.ListOfChildren.Add(child);
        }

        public void RemoveChild(StateSO parent, StateSO child)
        {
            parent.ListOfChildren.Remove(child);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}