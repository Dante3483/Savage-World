using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class InventoryModelOld
{
    #region Private fields
    [SerializeField]
    private InventoryItem[] _storageItems;
    [SerializeField]
    private InventoryItem[] _hotbarItems;
    [SerializeField]
    private InventoryItem[] _accessoriesItems;
    [SerializeField]
    private InventoryItem[] _armorItems;

    [SerializeField]
    private InventoryItem _bufferItem;
    [SerializeField]
    private InventoryItem _selectedItem;

    [SerializeField]
    private int _storageSize = 36;
    [SerializeField]
    private int _hotbarFullSize = 10;
    [SerializeField]
    private int _accessoriesSize = 6;
    [SerializeField]
    private int _armorSize = 3;

    private Dictionary<ItemLocations, InventoryItem[]> _itemsByLocation;
    private bool _isFirstPartOfHotbar;
    private bool _isItemInBuffer => !_bufferItem.IsEmpty;
    private int _hotbarSize => _hotbarFullSize / 2;
    private int _hotbarStartIndex => _isFirstPartOfHotbar ? 0 : _hotbarSize;
    private int _hotbarEndIndex => (_isFirstPartOfHotbar ? _hotbarSize : _hotbarFullSize) - 1;
    private int _indexOfSelectedItem;
    private bool _isInitialized;
    #endregion

    #region Public fields
    public event Action<InventoryItem[]> StorageChanged;
    public event Action<InventoryItem[], int, int> HotbarChanged;
    public event Action<InventoryItem[]> AccessoriesChanged;
    public event Action<InventoryItem[]> ArmorChanged;
    public event Action<InventoryItem> BufferItemChanged;
    public event Action<ItemSO, int> InventoryOverflowed;
    public event Action ItemsUpdated;
    #endregion

    #region Properties
    public int StorageSize
    {
        get
        {
            return _storageSize;
        }
    }

    public int HotbarSize
    {
        get
        {
            return _hotbarSize;
        }
    }

    public int AccessoriesSize
    {
        get
        {
            return _accessoriesSize;
        }
    }

    public int HotbarSelectedIndex
    {
        get
        {
            return _indexOfSelectedItem;
        }
    }

    public int ArmorSize
    {
        get
        {
            return _armorSize;
        }
    }

    public bool IsInitialized
    {
        get
        {
            return _isInitialized;
        }

        set
        {
            _isInitialized = value;
        }
    }
    #endregion

    #region Methods
    public void AddItemAtWithoutNotification(ItemSO itemData, int quantity, int index, ItemLocations location)
    {
        if (_itemsByLocation[location][index].IsEmpty)
        {
            InventoryItem inventoryItem = new(location);
            inventoryItem.Update(itemData, quantity);
            _itemsByLocation[location][index] = inventoryItem;
        }
    }

    public InventoryItem GetSelectedItem()
    {
        return _selectedItem;
    }

    public void RemoveSelectedItem()
    {
        _selectedItem.Clear();
        //UpdateUI();
    }

    public void DecreaseSelectedItemQuantity(int value)
    {
        _selectedItem.UpdateQuantity(_selectedItem.Quantity - value);
        //UpdateUI();
    }

    public bool IsEnoughSpaceForItem(ItemSO itemData, ItemLocations location)
    {
        if (location == ItemLocations.Hotbar)
        {
            return !_itemsByLocation[location]
                .Where((_, index) => index >= _hotbarStartIndex && index <= _hotbarEndIndex)
                .Where((item) => item.IsEmpty || (item.Data == itemData && item.Quantity != item.Data.MaxStackSize))
                .Any();
        }
        else
        {
            return !_itemsByLocation[location]
                .Where((item) => item.IsEmpty || (item.Data == itemData && item.Quantity != item.Data.MaxStackSize))
                .Any();
        }
    }
    #endregion
}
