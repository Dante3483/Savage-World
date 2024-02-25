using Inventory;
using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

[CreateAssetMenu(fileName = "Inventory", menuName = "Player/Inventory/Inventory")]
public class InventorySO : ScriptableObject
{
    #region Private fields
    [SerializeField] private InventoryItem[] _storageItems;
    [SerializeField] private InventoryItem[] _hotbarItems;
    [SerializeField] private InventoryItem[] _accessoriesItems;
    [SerializeField] private InventoryItem[] _armorItems;

    [SerializeField] private InventoryItem _bufferItem;

    [SerializeField] private int _storageSize = 36;
    [SerializeField] private int _hotbarFullSize = 10;
    [SerializeField] private int _accessoriesSize = 6;
    [SerializeField] private int _armorSize = 3;

    private Dictionary<ItemLocations, InventoryItem[]> _itemsByLocation;
    private bool _isFirstPartOfHotbar;
    private bool _isItemInBuffer => !_bufferItem.IsEmpty;
    private int _hotbarSize => _hotbarFullSize / 2;
    private int _hotbarStartIndex => _isFirstPartOfHotbar ? 0 : _hotbarSize;
    #endregion

    #region Public fields
    public event Action<InventoryItem[]> OnStorageChanged;
    public event Action<InventoryItem[], int, int> OnHotbarChanged;
    public event Action<InventoryItem[]> OnAccessoriesChanged;
    public event Action<InventoryItem[]> OnArmorChanged;
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
        _armorItems = new InventoryItem[_armorSize];

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

        for (int i = 0; i < _armorSize; i++)
        {
            _armorItems[i] = InventoryItem.GetEmptyItem();
        }

        _itemsByLocation.Add(ItemLocations.Storage, _storageItems);
        _itemsByLocation.Add(ItemLocations.Hotbar, _hotbarItems);
        _itemsByLocation.Add(ItemLocations.Accessories, _accessoriesItems);
        _itemsByLocation.Add(ItemLocations.Armor, _armorItems);

        _bufferItem = InventoryItem.GetEmptyItem();

        _isFirstPartOfHotbar = true;
    }

    public void InformAboutChange()
    {
        OnStorageChanged?.Invoke(_storageItems);
        OnHotbarChanged?.Invoke(_hotbarItems, _hotbarStartIndex, _hotbarSize);
        OnAccessoriesChanged?.Invoke(_accessoriesItems);
        OnArmorChanged?.Invoke(_armorItems);
    }

    public int AddItem(ItemSO itemData, int quantity, ItemLocations location)
    {
        if (!itemData.IsStackable)
        {
            if (!IsStorageFull(location))
            {
                quantity -= AddItemToFirstFreeSlot(itemData, 1, location);
            }
        }
        else
        {
            quantity = AddStackableItem(itemData, quantity, location);
        }
        InformAboutChange();
        return quantity;
    }

    private int AddItemToFirstFreeSlot(ItemSO itemData, int quantity, ItemLocations location)
    {
        for (int i = 0; i < _itemsByLocation[location].Length; i++)
        {
            if (_itemsByLocation[location][i].IsEmpty)
            {
                _itemsByLocation[location][i].UpdateData(quantity, itemData);
                return quantity;
            }
        }
        return 0;
    }

    private int AddItemToFirstFreeSlot(InventoryItem item, ItemLocations location)
    {
        for (int i = 0; i < _itemsByLocation[location].Length; i++)
        {
            if (_itemsByLocation[location][i].IsEmpty)
            {
                _itemsByLocation[location][i].UpdateData(item);
                item.ClearData();
                return item.Quantity;
            }
        }
        return 0;
    }

    private void AddAccessory(InventoryItem item)
    {
        InventoryItem sameAccessory = FindSameAccessory(item);
        if (sameAccessory != null)
        {
            sameAccessory.SwapData(item);
            return;
        }
        for (int i = 0; i < _accessoriesSize; i++)
        {
            if (_accessoriesItems[i].IsEmpty)
            {
                _accessoriesItems[i].SwapData(item);
                return;
            }
        }
        _accessoriesItems[0].SwapData(item);
    }

    private void AddArmor(InventoryItem item)
    {
        if (item.ItemData is ArmorItemSO armor)
        {
            _armorItems[(int)(armor.ArmorType)].SwapData(item);
        }
    }

    private int AddStackableItem(ItemSO item, int quantity, ItemLocations location)
    {
        for (int i = 0; i < _itemsByLocation[location].Length; i++)
        {
            if (_itemsByLocation[location][i].ItemData == item)
            {
                int amountPossibleToTake = _itemsByLocation[location][i].ItemData.MaxStackSize - _itemsByLocation[location][i].Quantity;
                if (quantity > amountPossibleToTake)
                {
                    _itemsByLocation[location][i].UpdateQuantity(_itemsByLocation[location][i].ItemData.MaxStackSize);
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
        InventoryItem item = GetItem(index, location);

        if (item.IsEmpty)
        {
            return;
        }
        _bufferItem.UpdateData(item);
        RemoveItemAt(index, location);
    }

    private void AddItemToBuffer(int index, ItemLocations location, int quantity)
    {
        InventoryItem item = GetItem(index, location);

        if (item.Quantity - quantity < 0)
        {
            quantity = item.Quantity;
        }
        if (!_isItemInBuffer)
        {
            item.UpdateQuantity(item.Quantity - quantity);
            _bufferItem.UpdateItem(item.ItemData);
            _bufferItem.UpdateQuantity(quantity);
        }
        else if (_bufferItem.ItemData == item.ItemData)
        {
            if (_bufferItem.Quantity + quantity > _bufferItem.ItemData.MaxStackSize)
            {
                quantity = _bufferItem.ItemData.MaxStackSize - _bufferItem.Quantity;
            }
            item.UpdateQuantity(item.Quantity - quantity);
            _bufferItem.UpdateQuantity(_bufferItem.Quantity + quantity);
        }
        InformAboutChange();
    }

    private void AddItemFromBufferAt(int index, ItemLocations location)
    {
        InventoryItem item = GetItem(index, location);
        int quantity = _bufferItem.Quantity;

        if (location == ItemLocations.Accessories)
        {
            if (!_bufferItem.IsAccessory)
            {
                return;
            }
            if (FindSameAccessory(_bufferItem) != null)
            {
                return;
            }
        }
        if (location == ItemLocations.Armor)
        {
            if (!_bufferItem.IsArmor)
            {
                return;
            }
            if (!IsCorrectArmorSlot(index, _bufferItem))
            {
                return;
            }
        }
        if (item.IsEmpty)
        {
            item.UpdateData(_bufferItem);
            _bufferItem.ClearData();
        }
        else
        {
            if (item.ItemData == _bufferItem.ItemData)
            {
                if (item.Quantity == item.ItemData.MaxStackSize)
                {
                    item.SwapData(_bufferItem);
                }
                else
                {
                    int amountPossibleToTake = item.ItemData.MaxStackSize - item.Quantity;
                    if (quantity > amountPossibleToTake)
                    {
                        item.UpdateQuantity(item.ItemData.MaxStackSize);
                        quantity -= amountPossibleToTake;
                        _bufferItem.UpdateQuantity(quantity);
                    }
                    else
                    {
                        item.UpdateQuantity(item.Quantity + quantity);
                        _bufferItem.ClearData();
                    }
                }
            }
            else
            {
                item.SwapData(_bufferItem);
            }
        }
    }

    private InventoryItem GetItem(int index, ItemLocations location)
    {
        return _itemsByLocation[location][index];
    }

    public void RemoveItemAt(int index, ItemLocations location)
    {
        GetItem(index, location).ClearData();
        InformAboutChange();
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
        InformAboutChange();
    }

    public void TakeItem(int index, ItemLocations location, int quantity)
    {
        InventoryItem item = GetItem(index, location);
        if (item.IsAccessory)
        {
            FastEquipAccessory(location, item);
        }
        else if (item.IsArmor)
        {
            FastEquipArmor(location, item);
        }
        else
        {
            AddItemToBuffer(index, location, quantity);
        }
        InformAboutChange();
    }

    private void FastEquipAccessory(ItemLocations location, InventoryItem item)
    {
        if (location != ItemLocations.Accessories)
        {
            AddAccessory(item);
        }
        else
        {
            AddItemToFirstFreeSlot(item, ItemLocations.Storage);
        }
    }

    private void FastEquipArmor(ItemLocations location, InventoryItem item)
    {
        if (location != ItemLocations.Armor)
        {
            AddArmor(item);
        }
        else
        {
            AddItemToFirstFreeSlot(item, ItemLocations.Storage);
        }
    }

    public bool CompareItemWithBuffer(int index, ItemLocations location)
    {
        return _bufferItem.ItemData == GetItem(index, location).ItemData;
    }

    private bool IsStorageFull(ItemLocations location)
    {
        return !_itemsByLocation[location].Where(item => item.IsEmpty).Any();
    }

    private bool IsCorrectArmorSlot(int index, InventoryItem item)
    {
        return (int)(item.ItemData as ArmorItemSO).ArmorType == index;
    }

    private InventoryItem FindSameAccessory(InventoryItem item)
    {
        for (int i = 0; i < _accessoriesSize; i++)
        {
            if (_accessoriesItems[i].ItemData == item.ItemData)
            {
                return _accessoriesItems[i];
            }
        }
        return null;
    }
    #endregion
}
