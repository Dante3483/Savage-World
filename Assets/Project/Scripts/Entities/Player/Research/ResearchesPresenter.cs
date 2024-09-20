using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.MVP;
using SavageWorld.Runtime.Entities.Player.Book;
using SavageWorld.Runtime.Entities.Player.CraftStation;
using SavageWorld.Runtime.Entities.Player.Inventory;
using SavageWorld.Runtime.Entities.Player.Inventory.Items;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.Player.Research
{
    public class ResearchesPresenter : PresenterBaseGeneric<ResearchesModelSO, ResearchesView>
    {
        #region Fields
        private InventoryModel _inventory;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public ResearchesPresenter(ResearchesModelSO model, ResearchesView view, InventoryModel inventory) : base(model, view)
        {
            _inventory = inventory;
        }

        public override void ResetPresenter()
        {

        }
        #endregion

        #region Private Methods
        protected override void InitializeModel()
        {
            base.InitializeModel();
            _model.ResearchChangedState += ResearchChangedStateEventHandler;
        }

        protected override void InitializeView()
        {
            int researchCount = _model.GetResearchesCount();
            _view.Configure(researchCount);
            _view.Initialize();
            for (int i = 0; i < researchCount; i++)
            {
                string name = _model.GetName(i);
                Sprite icon = _model.GetIcon(i);
                Vector3 position = _model.GetPosition(i);
                ResearchState state = _model.GetState(i);
                _view.UpdateResearch(i, name, icon, position, state);
                foreach (ResearchSO parent in _model.GetListOfPerents(i))
                {
                    Vector3 parentPosition = _model.GetPosition(parent);
                    _view.CreateResearchLine(i, position, parentPosition);
                }
            }
            _view.CompleteResearchRequested += CompleteResearchRequestedEventHandler;
            _view.ResearchDescriptionRequested += ResearchDescriptionRequestedEventHandler;
            _view.RewardDescriptionRequested += RewardDescriptionRequestedEventHandler;
            _view.CostDescriptionRequested += CostDescriptionRequestedEventHandler;
        }

        private void ResearchChangedStateEventHandler(int index, ResearchState state)
        {
            _view.UpdateResearchState(index, state);
        }

        private void CompleteResearchRequestedEventHandler(int index)
        {
            _model.ExamineResearch(index, _inventory);
        }

        private void ResearchDescriptionRequestedEventHandler(int index)
        {
            string name = _model.GetName(index);
            string description = _model.GetDescription(index);
            _view.UpdateResearchDescription(name, description);
            foreach (RecipeSO reward in _model.GetListOfRewards(index))
            {
                _view.AddRewardToResearchDescription(reward.Result.Item.SmallItemImage);
            }
            foreach (ItemQuantity cost in _model.GetListOfCosts(index))
            {
                _view.AddCostToResearchDescription(cost.Item.SmallItemImage, cost.Quantity);
            }
        }

        private void RewardDescriptionRequestedEventHandler(int researchIndex, int index)
        {
            ItemSO reward = _model.GetListOfRewards(researchIndex)[index].Result.Item;
            _view.UpdateItemDescription(reward.SmallItemImage, reward.ColoredName, reward.Description);
        }

        private void CostDescriptionRequestedEventHandler(int researchIndex, int index)
        {
            ItemSO cost = _model.GetListOfCosts(researchIndex)[index].Item;
            _view.UpdateItemDescription(cost.SmallItemImage, cost.ColoredName, cost.Description);
        }
        #endregion
    }
}