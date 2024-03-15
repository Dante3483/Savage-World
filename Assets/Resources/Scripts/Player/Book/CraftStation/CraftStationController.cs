using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
        _craftStationUI.ResetItemsToCraft();
        int i = 0;
        foreach (RecipeItem material in recipe.Meterials)
        {
            _craftStationUI.UpdateItemForCraft(i++, material.Item.ItemImage, material.Item.Name, "");
        }
    }

    private void HandleCreateItem()
    {

    }
    #endregion
}
