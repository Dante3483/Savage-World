using UnityEngine;

public class CraftStationController : MonoBehaviour, IBookPageController
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UICraftStationPage _craftStationUI;
    [SerializeField] private CraftStationSO _craftStationData;
    [SerializeField] private InventorySO _inventoryData;
    [SerializeField] private int _itemsForCraftCount;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        PrepareUI();
        PrepareData();
    }

    public void PrepareUI()
    {
        _craftStationUI.InitializePage(_itemsForCraftCount);
        _craftStationUI.OnItemSelected += HandleUpdateItemsForCraft;
        _craftStationUI.OnItemCreate += HandleCreateItem;
    }

    public void PrepareData()
    {
        _inventoryData.OnItemsUpdate += HandleUpdateItemsForCraft;
    }

    public void ResetData()
    {
        if (UIManager.Instance.CraftStationUI.IsActive && _craftStationData != CraftStationSO.CurrentCraftStation)
        {
            UpdateCraftStation();
            return;
        }

        UIManager.Instance.CraftStationUI.ReverseActivity();
        if (UIManager.Instance.CraftStationUI.IsActive)
        {
            UpdateCraftStation();
        }
    }

    private void UpdateCraftStation()
    {
        _craftStationData = CraftStationSO.CurrentCraftStation;
        _craftStationUI.ResetPage();
        foreach (RecipeSO recipe in _craftStationData.Recipes)
        {
            _craftStationUI.UpdateItemToCraft(recipe.Result.Item.ItemImage, recipe.Result.Item.Name);
        }
        _craftStationUI.SelectCell(0);
    }

    private void HandleUpdateItemsForCraft(int index)
    {
        RecipeSO recipe = _craftStationData.Recipes[index];
        recipe.SelectRecipe();
        _craftStationUI.ResetItemsToCraft();
        UpdateItemsForCraft(recipe);
    }

    private void HandleUpdateItemsForCraft()
    {
        if (UIManager.Instance.CraftStationUI.IsActive)
        {
            RecipeSO recipe = RecipeSO.SelectedRecipe;
            UpdateItemsForCraft(recipe);
        }
    }

    private void UpdateItemsForCraft(RecipeSO recipe)
    {
        int i = 0;
        bool isEnoughMaterials = true;
        foreach (RecipeItem material in recipe.Materials)
        {
            int possibleQuantity = Mathf.Min(_inventoryData.GetItemQuantity(material.Item), material.Item.MaxStackSize);
            isEnoughMaterials &= possibleQuantity >= material.Quantity;
            _craftStationUI.UpdateItemForCraft(i++, material.Item.ItemImage, material.Item.Name, possibleQuantity, material.Quantity);
        }
        recipe.IsEnoughMaterials = isEnoughMaterials;
    }

    private void HandleCreateItem()
    {
        RecipeSO recipe = RecipeSO.SelectedRecipe;
        if (recipe.IsEnoughMaterials)
        {
            foreach (RecipeItem material in recipe.Materials)
            {
                _inventoryData.RemoveItemFromFirstSlot(material.Item, material.Quantity);
            }
            _inventoryData.AddItem(recipe.Result.Item, recipe.Result.Quantity);
        }
    }
    #endregion
}
