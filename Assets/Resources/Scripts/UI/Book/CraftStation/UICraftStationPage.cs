using System;
using System.Collections.Generic;
using UnityEngine;

public class UICraftStationPage : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UIItemToCraftCell _itemToCraftPrefab;
    [SerializeField] private UIItemForCraftCell _itemForCraftPrefab;
    [SerializeField] private RectTransform _itemsToCraftContent;
    [SerializeField] private RectTransform _itemsForCraftContent;

    private List<UIItemToCraftCell> _listOfItemsToCraft;
    private List<UIItemForCraftCell> _listOfItemsForCraft;
    #endregion

    #region Public fields
    public Action<int> OnItemSelected;
    public Action OnItemCreate;
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _listOfItemsToCraft = new List<UIItemToCraftCell>();
    }

    public void InitializePage(int itemsForCraftCount)
    {
        InitializeItemsForCraft(itemsForCraftCount);
    }

    private void InitializeItemsForCraft(int itemsForCraftCount)
    {
        _listOfItemsForCraft = new List<UIItemForCraftCell>();
        for (int i = 0; i < itemsForCraftCount; i++)
        {
            UIItemForCraftCell uiItem = Instantiate(_itemForCraftPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_itemsForCraftContent, false);
            uiItem.name = "ItemForCraft";
            _listOfItemsForCraft.Add(uiItem);
        }
    }

    public void ResetPage()
    {
        foreach (UIItemToCraftCell itemToCraft in _listOfItemsToCraft)
        {
            Destroy(itemToCraft.gameObject);
        }
        _listOfItemsToCraft.Clear();
    }

    public void UpdateItemToCraft(Sprite sprite, string name)
    {
        UIItemToCraftCell uiItem = Instantiate(_itemToCraftPrefab, Vector3.zero, Quaternion.identity);
        uiItem.transform.SetParent(_itemsToCraftContent, false);
        uiItem.name = "ItemToCraft";
        uiItem.OnLeftButtonClick += HandleSelectItemToCraftCell;
        uiItem.SetData(sprite, name);
        _listOfItemsToCraft.Add(uiItem);
    }

    public void ResetItemsToCraft()
    {
        foreach (UIItemForCraftCell item in _listOfItemsForCraft)
        {
            item.ResetData();
        }
    }

    public void UpdateItemForCraft(int index, Sprite sprite, string name, int possibleQuantity, int requieredQuantity)
    {
        _listOfItemsForCraft[index].SetData(sprite, name, possibleQuantity, requieredQuantity);
    }

    public void SelectCell(int index)
    {
        DeselectAllCells();
        _listOfItemsToCraft[index].Select();
        OnItemSelected?.Invoke(index);
    }

    private void DeselectAllCells()
    {
        foreach (UIItemToCraftCell uiItem in _listOfItemsToCraft)
        {
            uiItem.Deselect();
        }
    }

    private void HandleSelectItemToCraftCell(UIItemToCraftCell cell)
    {
        int index = _listOfItemsToCraft.IndexOf(cell);
        SelectCell(index);
    }

    public void HandleCreateItem()
    {
        OnItemCreate?.Invoke();
    }
    #endregion
}
