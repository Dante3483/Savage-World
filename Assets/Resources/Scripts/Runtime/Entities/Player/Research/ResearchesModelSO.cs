using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.MVP;
using SavageWorld.Runtime.Entities.Player.Book;
using SavageWorld.Runtime.Entities.Player.CraftStation;
using SavageWorld.Runtime.Entities.Player.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.Player.Research
{
    [CreateAssetMenu(fileName = "Researches", menuName = "Player/Researches/Researches")]
    public class ResearchesModelSO : ModelBaseSO
    {
        #region Fields
        [SerializeField]
        private List<ResearchSO> _listOfReserches;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates
        public event Action<int, ResearchState> ResearchChangedState;
        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public override void Initialize()
        {
            foreach (ResearchSO research in _listOfReserches)
            {
                research.ResetData();
                research.StateChanged += StateChangedEventHandler;
            }
        }

        public bool ExamineResearch(int index, InventoryModel inventory)
        {
            ResearchSO research = _listOfReserches[index];
            foreach (ItemQuantity item in research.ListOfCosts)
            {
                if (inventory.GetFullItemQuantity(item.Item) < item.Quantity)
                {
                    return false;
                }
            }
            foreach (ItemQuantity item in research.ListOfCosts)
            {
                inventory.RemoveItem(item.Item, item.Quantity);
            }
            research.Complete();
            return true;
        }

        public void SetResearchState(int index, ResearchState state)
        {
            ResearchSO research = _listOfReserches[index];
            if (state == ResearchState.Completed)
            {
                research.Complete();
            }
            else
            {
                research.ChangeState(state);
            }
        }

        public string GetName(int index)
        {
            return _listOfReserches[index].Name;
        }

        public Sprite GetIcon(int index)
        {
            return _listOfReserches[index].IconImage;
        }

        public string GetDescription(int index)
        {
            return _listOfReserches[index].Description;
        }

        public Vector3 GetPosition(int index)
        {
            return _listOfReserches[index].Position;
        }

        public Vector3 GetPosition(ResearchSO research)
        {
            return _listOfReserches.Find(r => r == research).Position;
        }

        public ResearchState GetState(int index)
        {
            return _listOfReserches[index].State;
        }

        public int GetResearchesCount()
        {
            return _listOfReserches.Count;
        }

        public List<RecipeSO> GetListOfRewards(int index)
        {
            return _listOfReserches[index].ListOfRewards;
        }

        public List<ItemQuantity> GetListOfCosts(int index)
        {
            return _listOfReserches[index].ListOfCosts;
        }

        public List<ResearchSO> GetListOfPerents(int index)
        {
            return _listOfReserches[index].ListOfParents;
        }
        #endregion

        #region Private Methods
        private bool CheckIndex(int index)
        {
            return index < 0 || index >= _listOfReserches.Count;
        }

        private void StateChangedEventHandler(ResearchSO research)
        {
            int index = _listOfReserches.IndexOf(research);
            ResearchChangedState?.Invoke(index, research.State);
        }
        #endregion
    }
}