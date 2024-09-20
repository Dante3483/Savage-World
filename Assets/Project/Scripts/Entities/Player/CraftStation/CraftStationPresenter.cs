using SavageWorld.Runtime.Entities.Player.Book;
using SavageWorld.Runtime.Entities.Player.Inventory;
using SavageWorld.Runtime.Entities.Player.Inventory.Items;
using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.MVP;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.Player.CraftStation
{
    public class CraftStationPresenter : PresenterBaseGeneric<CraftStationModelSO, CraftStationView>
    {
        #region Fields
        private InventoryModel _inventory;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public CraftStationPresenter(CraftStationModelSO model, CraftStationView view, InventoryModel inventory) : base(model, view)
        {
            _inventory = inventory;
            _inventory.ItemQuantityChanged += ItemQuantityChangedEventHandler;
        }

        public override void ResetPresenter()
        {

        }

        public override void Enable()
        {
            base.Enable();
            _view.SelectBookmark(0);
        }

        public override void Disable()
        {
            base.Disable();
            _model.ResetSelection();
        }
        #endregion

        #region Private Methods
        protected override void InitializeModel()
        {
            CraftStationModelSO.CraftStationSelected += CraftStationSelectedEventHandler;
            CraftStationModelSO.SetSelected += SetSelectedEventHandler;
            CraftStationModelSO.RecipeSelected += RecipeSelectedEventHandler;
        }

        protected override void InitializeView()
        {
            base.InitializeView();
            _view.SelectSetRequested += SelectSetRequestedEventHandler;
            _view.SelectRecipeRequested += SelectRecipeRequestedEventHandler;
            _view.SearchingRecipes += SearchingRecipesEventHandler;
            _view.CreateItemRequested += CreateItemRequestedEventHandler;
        }

        private void UpdateView()
        {
            _view.UpdateName(_model.Name);
            UpdateBookmarks();
        }

        private void UpdateBookmarks()
        {
            _view.RemoveBookmarks();
            foreach (RecipesSet set in _model.ListOfSets)
            {
                _view.AddBookmark(set.Icon);
            }
            _view.SelectBookmark(0);
        }

        private void UpdateMaterials(RecipeSO recipe)
        {
            _view.ResetMaterials();
            for (int i = 0; i < recipe.Materials.Length; i++)
            {
                ItemQuantity material = recipe.Materials[i];
                _view.UpdateMaterial(i, material.Item.SmallItemImage, material.Item.Name, material.Quantity);
            }
        }

        private void UpdateMaxRecipeQuantity(RecipeSO recipe)
        {
            int itemQuantity;
            int currentRecipeQuantity;
            int minRecipeQuantity = int.MaxValue;
            foreach (ItemQuantity material in recipe.Materials)
            {
                itemQuantity = _inventory.GetFullItemQuantity(material.Item);
                currentRecipeQuantity = Mathf.FloorToInt(itemQuantity / material.Quantity);
                minRecipeQuantity = Mathf.Min(currentRecipeQuantity, minRecipeQuantity);
            }
            _view.UpdateQuantityInput(minRecipeQuantity);
        }

        private void ItemQuantityChangedEventHandler(int quantity, int index, ItemLocations location)
        {

        }

        private void CraftStationSelectedEventHandler(CraftStationModelSO model)
        {
            _model = model;
            UpdateView();
        }

        private void SetSelectedEventHandler(List<RecipeSO> recipes)
        {
            _view.ResetSearching();
            _view.RemoveRecipes();
            foreach (RecipeSO recipe in recipes)
            {
                if (recipe.IsUnlocked)
                {
                    _view.AddRecipe(recipe.Result.Item.SmallItemImage, recipe.Result.Item.Name);
                }
            }
            _view.SelectRecipe(0);
        }

        private void RecipeSelectedEventHandler(RecipeSO recipe)
        {
            ItemSO resultData = recipe.Result.Item;
            _view.UpdateBigImage(resultData.BigItemImage);
            _view.UpdateRarityText(resultData.Name, resultData.ItemRarity.RarityColor);
            _view.UpdateDescriptionText(resultData.Description);
            UpdateMaterials(recipe);
            UpdateMaxRecipeQuantity(recipe);
        }

        private void SelectSetRequestedEventHandler(int index)
        {
            _model.SelectSet(index);
        }

        private void SelectRecipeRequestedEventHandler(int index)
        {
            _model.SelectRecipe(index);
        }

        private void SearchingRecipesEventHandler(string filter)
        {
            _view.RemoveRecipes();
            foreach (RecipeSO recipe in _model.GetRecipesByFilter(filter))
            {
                if (recipe.IsUnlocked)
                {
                    _view.AddRecipe(recipe.Result.Item.SmallItemImage, recipe.Result.Item.Name);
                }
            }
            _view.SelectRecipe(0);
        }

        private void CreateItemRequestedEventHandler(int quantity)
        {
            RecipeSO recipe = _model.SelectedRecipe;
            if (recipe != null)
            {
                foreach (ItemQuantity material in recipe.Materials)
                {
                    _inventory.RemoveItem(material.Item, material.Quantity * quantity);
                }
                _inventory.AddItem(recipe.Result.Item, recipe.Result.Quantity * quantity);
                UpdateMaxRecipeQuantity(recipe);
            }
        }
        #endregion
    }
}