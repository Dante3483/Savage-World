using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.Player.Book;
using SavageWorld.Runtime.Player.CraftStation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavageWorld.Runtime.Player.Research
{
    [CreateAssetMenu(fileName = "Research", menuName = "NodeSO")]
    public class ResearchSO : ScriptableObject
    {
        #region Private fields
        [SerializeField]
        private string _name;
        [SerializeField]
        private Sprite _iconImage;
        [SerializeField]
        private string _description;
        [SerializeField]
        private ResearchState _state;
        [SerializeField]
        private Vector3 _position;
        [SerializeField]
        private List<RecipeSO> _listOfRewards = new();
        [SerializeField]
        private List<ItemQuantity> _listOfCosts = new();
        [SerializeField]
        private List<ResearchSO> _listOfParents = new();
        [SerializeField]
        private List<ResearchSO> _listOfChildren = new();
        #endregion

        #region Public fields
        public event Action<ResearchSO> StateChanged;
        #endregion

        #region Properties
        public string Name { get => _name; set => _name = value; }
        public Sprite IconImage { get => _iconImage; set => _iconImage = value; }
        public List<RecipeSO> ListOfRewards { get => _listOfRewards; set => _listOfRewards = value; }
        public List<ItemQuantity> ListOfCosts { get => _listOfCosts; set => _listOfCosts = value; }
        public List<ResearchSO> ListOfParents { get => _listOfParents; set => _listOfParents = value; }
        public List<ResearchSO> ListOfChildren { get => _listOfChildren; set => _listOfChildren = value; }
        public string Description { get => _description; set => _description = value; }
        public Vector3 Position
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
        public ResearchState State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
            }
        }
        #endregion

        #region Methods
        public void ResetData()
        {
            _state = _listOfParents.Count == 0 ? ResearchState.Unlocked : ResearchState.Locked;
        }

        public void Complete()
        {
            foreach (RecipeSO reward in _listOfRewards)
            {
                reward.IsUnlocked = true;
            }
            ChangeState(ResearchState.Completed);
            _listOfChildren.ForEach(r => r.TryUnlock());
        }

        private void TryUnlock()
        {
            if (_listOfParents.Any(r => r._state != ResearchState.Completed))
            {
                return;
            }
            ChangeState(ResearchState.Unlocked);
        }

        public void ChangeState(ResearchState state)
        {
            _state = state;
            StateChanged?.Invoke(this);
        }

        [Button("Update references")]
        private void UpdateChildrens()
        {
            foreach (ResearchSO parent in _listOfParents)
            {
                parent.ListOfChildren.Add(this);
            }
        }
        #endregion
    }
}