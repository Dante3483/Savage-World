using Inventory;
using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Player/Inventory/Inventory")]
public class InventorySO : ScriptableObject
{
    #region Private fields
    [SerializeField] private InventoryItem[] _storageItems;
    [SerializeField] private InventoryItem[] _hotbarItems;
    [SerializeField] private InventoryItem[] _accessoriesItems;

    [SerializeField] private InventoryItem _helmetItem;
    [SerializeField] private InventoryItem _chestplateItem;
    [SerializeField] private InventoryItem _leggingsItem;

    [SerializeField] private InventoryItem _bufferItem;

    [SerializeField] private int _storageSize = 36;
    [SerializeField] private int _hotbarFullSize = 10;
    [SerializeField] private int _accessoriesSize = 6;

    private bool _isFirstPartOfHotbar;
    private bool _isItemInBuffer => !_bufferItem.IsEmpty;
    private int _hotbarSize => _hotbarFullSize / 2;
    private int _hotbarStartIndex => _isFirstPartOfHotbar ? 0 : _hotbarSize;
    private Dictionary<ItemLocations, InventoryItem[]> _itemsByLocation;
    #endregion

    #region Public fields
    public event Action<InventoryItem[]> OnStorageChanged;
    public event Action<InventoryItem[], int, int> OnHotbarChanged;
    public event Action<InventoryItem[]> OnAccessoriesChanged;
    #endregion

    #region Properties
    public int StorageSize
    {
        get
        {
            return _storageSize;
        }

        set
        {
            _storageSize = value;
        }
    }

    public int HotbarSize
    {
        get
        {
            return _hotbarFullSize;
        }

        set
        {
            _hotbarFullSize = value;
        }
    }

    public int AccessoriesSize
    {
        get
        {
            return _accessoriesSize;
        }

        set
        {
            _accessoriesSize = value;
        }
    }
    #endregion

    #region Methods
    public void Initialize()
    {
        _itemsByLocation = new Dictionary<ItemLocations, InventoryItem[]>();
        _storageItems = new InventoryItem[_storageSize];
        _hotbarItems = new InventoryItem[_hotbarFullSize];
        _accessoriesItems = new InventoryItem[_accessoriesSize];

        for (int i = 0; i < _storageSize; i++)
        {
            _storageItems[i] = InventoryItem.GetEmptyItem();
        }

        for (int i = 0; i < _hotbarFullSize; i++)
        {
            _hotbarItems[i] = InventoryItem.GetEmptyItem();
        }

        for (int i = 0; i < _accessoriesSize; i++)
        {
            _accessoriesItems[i] = InventoryItem.GetEmptyItem();
        }

        _itemsByLocation.Add(ItemLocations.Storage, _storageItems);
        _itemsByLocation.Add(ItemLocations.Hotbar, _hotbarItems);
        _itemsByLocation.Add(ItemLocations.Accessories, _accessoriesItems);

        _bufferItem = InventoryItem.GetEmptyItem();

        _isFirstPartOfHotbar = true;
    }

    public void InformAboutChange()
    {
        OnStorageChanged?.Invoke(_storageItems);
        OnHotbarChanged?.Invoke(_hotbarItems, _hotbarStartIndex, _hotbarSize);
        OnAccessoriesChanged?.Invoke(_accessoriesItems);
    }

    public int AddItem(ItemSO item, int quantity, ItemLocations location)
    {
        if (!item.IsStackable)
        {
            if (!IsStorageFull(location))
            {
                quantity -= AddItemToFirstFreeSlot(item, 1, location);
            }
        }
        else
        {
            quantity = AddStackableItem(item, quantity, location);
        }
        InformAboutChange();
        return quantity;
    }

    private int AddItemToFirstFreeSlot(ItemSO item, int quantity, ItemLocations location)
    {
        for (int i = 0; i < _itemsByLocation[location].Length; i++)
        {
            if (_itemsByLocation[location][i].IsEmpty)
            {
                _itemsByLocation[location][i].UpdateData(quantity, item);
                return quantity;
            }
        }
        return 0;
    }

    private int AddStackableItem(ItemSO item, int quantity, ItemLocations location)
    {
        for (int i = 0; i < _itemsByLocation[location].Length; i++)
        {
            if (_itemsByLocation[location][i].Item == item)
            {
                int amountPossibleToTake = _itemsByLocation[location][i].Item.MaxStackSize - _itemsByLocation[location][i].Quantity;
                if (quantity > amountPossibleToTake)
                {
                    _itemsByLocation[location][i].UpdateQuantity(_itemsByLocation[location][i].Item.MaxStackSize);
                    quantity -= amountPossibleToTake;
                }
                else
                {
                    _itemsByLocation[location][i].UpdateQuantity(_itemsByLocation[location][i].Quantity + quantity);
                    return 0;
                }
            }
        }

        while (quantity > 0 && !IsStorageFull(location))
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            quantity -= newQuantity;
            AddItemToFirstFreeSlot(item, newQuantity, location);
        }
        return quantity;
    }

    private void AddItemToBuffer(int index, ItemLocations location)
    {
        InventoryItem item = GetItemAt(index, location);
        if (item.IsEmpty)
        {
            return;
        }
        _bufferItem = item;
        RemoveItemAt(index, location);
    }

    private void AddItemToBuffer(int index, ItemLocations location, int quantity)
    {
        if (!_isItemInBuffer)
        {
            _bufferItem = _itemsByLocation[location][index].Clone();
            _itemsByLocation[location][index].UpdateQuantity(_itemsByLocation[location][index].Quantity - quantity);
            _bufferItem.UpdateQuantity(quantity);
        }
        else if (_bufferItem.Item == _itemsByLocation[location][index].Item)
        {
            _itemsByLocation[location][index].UpdateQuantity(_itemsByLocation[location][index].Quantity - quantity);
            _bufferItem.UpdateQuantity(_bufferItem.Quantity + quantity);
        }
        InformAboutChange();
    }

    private void AddItemFromBufferAt(int index, ItemLocations location)
    {
        InventoryItem itemAtIndex = GetItemAt(index, location);
        int quantity = _bufferItem.Quantity;

        if (itemAtIndex.IsEmpty)
        {
            _itemsByLocation[location][index] = GetItemFromBuffer();
        }
        else
        {
            if (itemAtIndex.Item == _bufferItem.Item)
            {
                if (itemAtIndex.Quantity == itemAtIndex.Item.MaxStackSize)
                {
                    _itemsByLocation[location][index] = GetItemFromBuffer();
                    _bufferItem = itemAtIndex;
                }
                else
                {
                    int amountPossibleToTake = itemAtIndex.Item.MaxStackSize - itemAtIndex.Quantity;
                    if (quantity > amountPossibleToTake)
                    {
                        _itemsByLocation[location][index].UpdateQuantity(itemAtIndex.Item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                        _bufferItem.UpdateQuantity(quantity);
                    }
                    else
                    {
                        _itemsByLocation[location][index].UpdateQuantity(itemAtIndex.Quantity + quantity);
                        _bufferItem = InventoryItem.GetEmptyItem();
                    }
                }
            }
            else
            {
                _itemsByLocation[location][index] = GetItemFromBuffer();
                _bufferItem = itemAtIndex;
            }
        }
        InformAboutChange();
    }

    public InventoryItem GetItemAt(int index, ItemLocations location)
    {
        return _itemsByLocation[location][index].Clone();
    }

    private InventoryItem GetItemFromBuffer()
    {
        InventoryItem item = _bufferItem.Clone();
        _bufferItem = InventoryItem.GetEmptyItem();
        return item;
    }

    public void RemoveItemAt(int index, ItemLocations location)
    {
        _itemsByLocation[location][index] = InventoryItem.GetEmptyItem();
        InformAboutChange();
    }

    public void RemoveItemAt(int index, ItemLocations location, int quantity)
    {
        int amountPossibliToRemove = _itemsByLocation[location][index].Quantity - quantity;
        if (amountPossibliToRemove > 0)
        {
            _itemsByLocation[location][index].UpdateQuantity(_itemsByLocation[location][index].Quantity - quantity);
            if (_itemsByLocation[location][index].Quantity == 0)
            {
                _itemsByLocation[location][index] = InventoryItem.GetEmptyItem();
            }
            InformAboutChange();
        }
        else
        {
            _itemsByLocation[location][index] = InventoryItem.GetEmptyItem();
            InformAboutChange();
        }
    }

    public void TakeItem(int index, ItemLocations location)
    {
        if (!_isItemInBuffer)
        {
            AddItemToBuffer(index, location);
        }
        else
        {
            AddItemFromBufferAt(index, location);
        }
    }

    public void TakeItem(int index, ItemLocations location, int quantity)
    {
        AddItemToBuffer(index, location, quantity);
    }

    public bool CompareItemWithBuffer(int index, ItemLocations location)
    {
        return _bufferItem.Item == _itemsByLocation[location][index].Item;
    }

    private bool IsStorageFull(ItemLocations location)
    {
        return !_itemsByLocation[location].Where(item => item.IsEmpty).Any();
    }
    #endregion
}
