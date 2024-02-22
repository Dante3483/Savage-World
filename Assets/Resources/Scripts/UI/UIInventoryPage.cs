using Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UIStorageItemCell _itemStoragePrefab;
    [SerializeField] private UIHotbarItemCell _itemHotbarPrefab;
    [SerializeField] private UIAccessoryItemCell _itemAccessoryPrefab;
    [SerializeField] private RectTransform _storageContent;
    [SerializeField] private RectTransform _hotbarContent;
    [SerializeField] private RectTransform _accessoriesContent;

    private List<UIStorageItemCell> _listOfStorageItems;
    private List<UIHotbarItemCell> _listOfHotbarItems;
    private List<UIAccessoryItemCell> _listOfAccessoryItems;
    private Dictionary<ItemLocations, List<UIItemCell>> _itemsByLocation;
    #endregion

    #region Public fields
    public event Action<int, ItemLocations> OnDraggingItem, OnStartTakeItem;
    public event Action OnMouseLeave, OnStopTakeItem;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void InitializePage(int storageSize, int hotbarSize, int accessoriesSize)
    {
        _itemsByLocation = new Dictionary<ItemLocations, List<UIItemCell>>();
        InitializeStorage(storageSize);
        InitializeHotbar(hotbarSize);
        InitializeAccessories(accessoriesSize);
    }

    private void InitializeAccessories(int accessoriesSize)
    {
        _listOfAccessoryItems = new List<UIAccessoryItemCell>();
        for (int i = 0; i < accessoriesSize; i++)
        {
            UIAccessoryItemCell uiItem = Instantiate(_itemAccessoryPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_accessoriesContent, false);
            uiItem.name = "AccessoryItemCell";
            SetHandlers(uiItem);
            _listOfAccessoryItems.Add(uiItem);
        }
        _itemsByLocation.Add(ItemLocations.Accessories, _listOfAccessoryItems.Cast<UIItemCell>().ToList());
    }

    private void InitializeHotbar(int hotbarSize)
    {
        _listOfHotbarItems = new List<UIHotbarItemCell>();
        for (int i = 0; i < hotbarSize; i++)
        {
            UIHotbarItemCell uiItem = Instantiate(_itemHotbarPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_hotbarContent, false);
            uiItem.name = "HotbarItemCell";
            uiItem.HotbarNumberTxt.text = $"{i + 1}";
            SetHandlers(uiItem);
            _listOfHotbarItems.Add(uiItem);
        }
        _itemsByLocation.Add(ItemLocations.Hotbar, _listOfHotbarItems.Cast<UIItemCell>().ToList());
    }

    private void InitializeStorage(int inventorySize)
    {
        _listOfStorageItems = new List<UIStorageItemCell>();
        for (int i = 0; i < inventorySize; i++)
        {
            UIStorageItemCell uiItem = Instantiate(_itemStoragePrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_storageContent, false);
            uiItem.name = "ItemCell";
            SetHandlers(uiItem);
            _listOfStorageItems.Add(uiItem);
        }
        _itemsByLocation.Add(ItemLocations.Storage, _listOfStorageItems.Cast<UIItemCell>().ToList());
    }

    private void SetHandlers(UIItemCell uiItem)
    {
        uiItem.OnLeftButtonClick += HandleDragItem;
        uiItem.OnRightMouseDown += HandleStartTakeItem;
        uiItem.OnRightMouseUp += HandleStopTakeItem;
    }

    public void UpdateStorageItemData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        _listOfStorageItems[itemIndex].SetData(itemImage, itemQuantity);
    }

    public void UpdateHotbarItemData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        _listOfHotbarItems[itemIndex].SetData(itemImage, itemQuantity);
    }

    public void UpdateAccessoriesItemData(int itemIndex, Sprite itemImage)
    {
        _listOfAccessoryItems[itemIndex].SetData(itemImage);
    }

    private void HandleDragItem(UIItemCell itemUI)
    {
        int index = _itemsByLocation[itemUI.ItemLocation].IndexOf(itemUI);
        
        if (index == -1)
        {
            return;
        }
        OnDraggingItem?.Invoke(index, itemUI.ItemLocation);
    }

    private void HandleStartTakeItem(UIItemCell itemUI)
    {
        int index = _itemsByLocation[itemUI.ItemLocation].IndexOf(itemUI);

        if (index == -1)
        {
            return;
        }
        OnStartTakeItem?.Invoke(index, itemUI.ItemLocation);
    }

    private void HandleStopTakeItem()
    {
        OnStopTakeItem?.Invoke();
    }
    #endregion
}
