using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UIStorageItemCell _storageItemPrefab;
    [SerializeField] private UIStorageHotbarItemCell _hotbarItemPrefab;
    [SerializeField] private UIAccessoryItemCell _accessoryItemPrefab;
    [SerializeField] private RectTransform _storageContent;
    [SerializeField] private RectTransform _hotbarContent;
    [SerializeField] private RectTransform _accessoriesContent;
    [SerializeField] private RectTransform _armorContent;
    [SerializeField] private UITooltip _tooltipUI;
    [SerializeField] private UIStorageItemCell _itemInBuffer;

    private List<UIStorageItemCell> _listOfStorageItems;
    private List<UIStorageHotbarItemCell> _listOfHotbarItems;
    private List<UIAccessoryItemCell> _listOfAccessoryItems;
    private List<UIArmorItemCell> _listOfArmorItems;
    private Dictionary<ItemLocations, List<UIItemCell>> _itemsByLocation;
    #endregion

    #region Public fields
    public event Action<int, ItemLocations> OnDraggingItem, OnStartTakeItem, OnDescriptionRequested;
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
        InitializeArmor();
    }

    private void InitializeAccessories(int accessoriesSize)
    {
        _listOfAccessoryItems = new List<UIAccessoryItemCell>();
        for (int i = 0; i < accessoriesSize; i++)
        {
            UIAccessoryItemCell uiItem = Instantiate(_accessoryItemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_accessoriesContent, false);
            uiItem.name = "AccessoryItemCell";
            SetHandlers(uiItem);
            _listOfAccessoryItems.Add(uiItem);
        }
        _itemsByLocation.Add(ItemLocations.Accessories, _listOfAccessoryItems.Cast<UIItemCell>().ToList());
    }

    private void InitializeHotbar(int hotbarSize)
    {
        _listOfHotbarItems = new List<UIStorageHotbarItemCell>();
        for (int i = 0; i < hotbarSize; i++)
        {
            UIStorageHotbarItemCell uiItem = Instantiate(_hotbarItemPrefab, Vector3.zero, Quaternion.identity);
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
            UIStorageItemCell uiItem = Instantiate(_storageItemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_storageContent, false);
            uiItem.name = "ItemCell";
            SetHandlers(uiItem);
            _listOfStorageItems.Add(uiItem);
        }
        _itemsByLocation.Add(ItemLocations.Storage, _listOfStorageItems.Cast<UIItemCell>().ToList());
    }

    private void InitializeArmor()
    {
        _listOfArmorItems = GetComponentsInChildren<UIArmorItemCell>().OrderBy(a => a.ArmorType).ToList();
        foreach (UIArmorItemCell uiItem in _listOfArmorItems)
        {
            SetHandlers(uiItem);
        }
        _itemsByLocation.Add(ItemLocations.Armor, _listOfArmorItems.Cast<UIItemCell>().ToList());
    }

    private void SetHandlers(UIItemCell uiItem)
    {
        uiItem.OnLeftButtonClick += HandleDragItem;
        uiItem.OnLeftButtonClick += HandleUpdateTooltip;

        uiItem.OnRightMouseDown += HandleStartTakeItem;
        uiItem.OnRightMouseDown += HandleUpdateTooltip;

        uiItem.OnRightMouseUp += HandleStopTakeItem;
        uiItem.OnMouseEnter += HandleUpdateTooltip;
        uiItem.OnMouseLeave += HandleHideTooltip;
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

    private void HandleUpdateTooltip(UIItemCell itemUI)
    {
        int index = _itemsByLocation[itemUI.ItemLocation].IndexOf(itemUI);

        if (index == -1)
        {
            return;
        }
        OnDescriptionRequested?.Invoke(index, itemUI.ItemLocation);
    }

    private void HandleHideTooltip()
    {
        HideTooltip();
    }

    private void HideTooltip()
    {
        _tooltipUI.Hide();
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

    public void UpdateArmorItemData(int itemIndex, Sprite itemImage)
    {
        _listOfArmorItems[itemIndex].SetData(itemImage);
    }

    public void UpdateItemInBufferData(Sprite itemImage, int itemQuantity)
    {
        _itemInBuffer.SetData(itemImage, itemQuantity);
    }

    public void UpdateTooltip(StringBuilder text)
    {
        if (text == null)
        {
            HideTooltip();
        }
        else
        {
            _tooltipUI.Show(text);
        }
    }

    public void ResetPage()
    {
        HideTooltip();
    }
    #endregion
}
