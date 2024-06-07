using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class InventoryModel : ModelBase
{
    #region Fields
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
    private int _indexOfSelectedItem;
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
    private bool _isInitialized;
    #endregion

    #region Properties
    private int _hotbarStartIndex => _isFirstPartOfHotbar ? 0 : HotbarSize;

    private int _hotbarEndIndex => (_isFirstPartOfHotbar ? HotbarSize : _hotbarFullSize) - 1;

    public int HotbarSize => _hotbarFullSize / 2;

    public int StorageSize
    {
        get
        {
            return _storageSize;
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

    public bool IsItemInBuffer => !_bufferItem.IsEmpty;

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

    #region Events / Delegates
    public event Action<ItemSO, int, ItemLocations> ItemDataChanged;
    public event Action<int, int, ItemLocations> ItemQuantityChanged;
    public event Action<int> SelectedItemChanged;
    public event Action<ItemSO, int> InventoryOverflowed;
    #endregion

    #region Public Methods
    public override void Initialize()
    {
        if (!_isInitialized)
        {
            _itemsByLocation = new Dictionary<ItemLocations, InventoryItem[]>();
            _storageItems = new InventoryItem[_storageSize];
            _hotbarItems = new InventoryItem[_hotbarFullSize];
            _accessoriesItems = new InventoryItem[_accessoriesSize];
            _armorItems = new InventoryItem[_armorSize];
            InitializeArrayOfItems(_storageItems, ItemLocations.Storage);
            InitializeArrayOfItems(_hotbarItems, ItemLocations.Hotbar);
            InitializeArrayOfItems(_accessoriesItems, ItemLocations.Accessories);
            InitializeArrayOfItems(_armorItems, ItemLocations.Armor);

            _bufferItem = CreateItem(ItemLocations.Buffer);

            _isFirstPartOfHotbar = true;
            _indexOfSelectedItem = -1;
            _isInitialized = true;
        }
    }

    public int AddItem(ItemSO data, int quantity)
    {
        quantity = AddItem(data, quantity, ItemLocations.Hotbar);
        if (quantity > 0)
        {
            quantity = AddItem(data, quantity, ItemLocations.Storage);
        }
        return quantity;
    }

    public int AddItem(ItemSO data, int quantity, ItemLocations location)
    {
        if (data.IsStackable)
        {
            quantity = AddQuantityToSameItems(data, quantity, location);
            if (quantity > 0)
            {
                quantity = AddItemToFirstFreeCells(data, quantity, location);
            }
        }
        else
        {
            quantity = AddItemToFirstFreeCells(data, quantity, location);
        }
        return quantity;
    }

    public int AddItemToEmptyCellByIndex(ItemSO data, int quantity, int index, ItemLocations location)
    {
        InventoryItem item = _itemsByLocation[location][index];
        if (item.IsEmpty)
        {
            item.Update(data, quantity);
            return 0;
        }
        return quantity;
    }

    public void RemoveItem(ItemSO data, int quantity)
    {
        quantity = RemoveItem(data, quantity, ItemLocations.Hotbar);
        if (quantity != 0)
        {
            RemoveItem(data, quantity, ItemLocations.Storage);
        }
    }

    public void RemoveItemFromBuffer()
    {
        if (IsItemInBuffer)
        {
            ItemSO data = _bufferItem.Data;
            int quantity = _bufferItem.Quantity;
            quantity = AddItem(data, quantity, ItemLocations.Hotbar);
            if (quantity > 0)
            {
                quantity = AddItem(data, quantity, ItemLocations.Storage);
            }
            if (quantity > 0)
            {
                InventoryOverflowed?.Invoke(data, quantity);
            }
        }
    }

    public void RemoveQuantityFromSelectedItem(int quantity)
    {
        if (!_selectedItem.IsEmpty)
        {
            _selectedItem.UpdateQuantity(_selectedItem.Quantity - quantity);
        }
    }

    public void DragItem(int index, ItemLocations location)
    {
        InventoryItem item = GetItem(index, location);
        if (IsItemInBuffer)
        {
            AddItemFromBufferAt(index, location);
        }
        else
        {
            if (item.IsEmpty)
            {
                return;
            }
            AddItemToBuffer(item);
        }
    }

    public void DragItem(int index, ItemLocations location, int quantity)
    {
        InventoryItem item = GetItem(index, location);
        if (item.IsAccessory)
        {
            DragAccessory(item, location);
        }
        else if (item.IsArmor)
        {
            DragArmor(item, location);
        }
        else
        {
            AddItemToBuffer(item, quantity);
        }
    }

    public void SelectHotbarItem(int index)
    {
        if (_indexOfSelectedItem == index)
        {
            DeselectHotbarItem(index);
        }
        else
        {
            DeselectHotbarItem(_indexOfSelectedItem);
            _selectedItem = _hotbarItems[_hotbarStartIndex + index];
            _indexOfSelectedItem = index;
        }
        SelectedItemChanged?.Invoke(_indexOfSelectedItem);
    }

    public void SelectAdjacentItem(int side)
    {
        if (_indexOfSelectedItem == -1)
        {
            SelectHotbarItem(0);
        }
        else
        {
            int index = _indexOfSelectedItem + side;
            index = index < 0 ? _hotbarEndIndex : index;
            index = index > _hotbarEndIndex ? 0 : index;
            SelectHotbarItem(index);
        }
    }

    public bool CompareItemWithBuffer(int index, ItemLocations location)
    {
        return _bufferItem.Data == GetItem(index, location).Data;
    }

    public ItemSO GetItemData(int index, ItemLocations location)
    {
        return _itemsByLocation[location][index].Data;
    }

    public int GetItemQuantity(int index, ItemLocations location)
    {
        return _itemsByLocation[location][index].Quantity;
    }

    public ItemSO GetSelectedItemData()
    {
        return _selectedItem?.Data;
    }

    public int GetSelectedItemQuantity()
    {
        return _selectedItem.Quantity;
    }

    public StringBuilder GetItemDescription(int index, ItemLocations location)
    {
        if (IsItemInBuffer)
        {
            return null;
        }
        InventoryItem item = GetItem(index, location);
        return item.Data?.GetFullDescription(item.Quantity);
    }

    public int GetFullItemQuantity(ItemSO data)
    {
        return _itemsByLocation
            .SelectMany(array => array.Value)
            .Where(item => item.Data == data)
            .Sum(item => item.Quantity);
    }

    public bool IsEnoughSpaceForItem(ItemSO data, ItemLocations location)
    {
        if (location == ItemLocations.Hotbar)
        {
            return !_itemsByLocation[location]
                .Where((_, index) => index >= _hotbarStartIndex && index <= _hotbarEndIndex)
                .Where((item) => item.IsEmpty || (item.Data == data && item.Quantity != item.Data.MaxStackSize))
                .Any();
        }
        else
        {
            return !_itemsByLocation[location]
                .Where((item) => item.IsEmpty || (item.Data == data && item.Quantity != item.Data.MaxStackSize))
                .Any();
        }
    }
    #endregion

    #region Private Methods
    private void InitializeArrayOfItems(InventoryItem[] items, ItemLocations location)
    {
        for (int i = 0; i < items.Length; i++)
        {
            InventoryItem item = CreateItem(location);
            items[i] = item;
        }
        _itemsByLocation.Add(location, items);
    }

    private InventoryItem CreateItem(ItemLocations location)
    {
        InventoryItem item = new(location);
        item.DataChanged += OnItemDataChanged;
        item.QuantityChanged += OnItemQuantityChanged;
        return item;
    }

    private int AddQuantityToSameItems(ItemSO data, int quantity, ItemLocations location)
    {
        for (int i = GetStartIndex(location); i < GetEndIndex(location); i++)
        {
            InventoryItem item = _itemsByLocation[location][i];
            if (item.Data == data)
            {
                int remainingQuantity = item.GetRemainingQuantity();
                if (quantity > remainingQuantity)
                {
                    item.UpdateQuantity(item.StackSize);
                    quantity -= remainingQuantity;
                }
                else
                {
                    item.UpdateQuantity(item.Quantity + quantity);
                    return 0;
                }
            }
        }
        return quantity;
    }

    private int AddItemToFirstFreeCells(ItemSO data, int quantity, ItemLocations location)
    {
        if (IsStorageFull(location))
        {
            return quantity;
        }
        for (int i = GetStartIndex(location); i <= GetEndIndex(location); i++)
        {
            InventoryItem item = _itemsByLocation[location][i];
            if (item.IsEmpty)
            {
                int stackSize = data.MaxStackSize;
                if (quantity > stackSize)
                {
                    item.Update(data, stackSize);
                    quantity -= stackSize;
                }
                else
                {
                    item.Update(data, quantity);
                    return 0;
                }
                if (IsStorageFull(location))
                {
                    return quantity;
                }
            }
        }
        return quantity;
    }

    private void AddItemToBuffer(InventoryItem item)
    {
        _bufferItem.Update(item);
        item.Clear();
    }

    private void AddItemToBuffer(InventoryItem item, int quantity)
    {
        if (item.Quantity - quantity < 0)
        {
            quantity = item.Quantity;
        }
        if (IsItemInBuffer)
        {
            if (_bufferItem.Data != item.Data)
            {
                return;
            }
            if (_bufferItem.Quantity + quantity > _bufferItem.StackSize)
            {
                quantity = _bufferItem.GetRemainingQuantity();
            }
            _bufferItem.UpdateQuantity(_bufferItem.Quantity + quantity);
            item.UpdateQuantity(item.Quantity - quantity);
        }
        else
        {
            _bufferItem.Update(item.Data, quantity);
            item.UpdateQuantity(item.Quantity - quantity);
        }
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
            if (FindSameAccessory(_bufferItem.Data) != null)
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
            if (!IsCorrectArmorSlot(index, _bufferItem.Data))
            {
                return;
            }
        }
        if (item.IsEmpty)
        {
            item.Update(_bufferItem);
            _bufferItem.Clear();
        }
        else
        {
            if (item.Data == _bufferItem.Data)
            {
                if (item.Quantity == item.StackSize)
                {
                    item.SwapData(_bufferItem);
                }
                else
                {
                    int remainingQuantity = item.GetRemainingQuantity();
                    if (quantity > remainingQuantity)
                    {
                        item.UpdateQuantity(item.StackSize);
                        quantity -= remainingQuantity;
                        _bufferItem.UpdateQuantity(quantity);
                    }
                    else
                    {
                        item.UpdateQuantity(item.Quantity + quantity);
                        _bufferItem.Clear();
                    }
                }
            }
            else
            {
                item.SwapData(_bufferItem);
            }
        }
    }

    private int RemoveItem(ItemSO data, int quantity, ItemLocations location)
    {
        for (int i = 0; i < _itemsByLocation[location].Length; i++)
        {
            InventoryItem item = _itemsByLocation[location][i];
            if (item.Data == data)
            {
                int itemQuantity = _itemsByLocation[location][i].Quantity;
                if (quantity > itemQuantity)
                {
                    item.Clear();
                    quantity -= itemQuantity;
                }
                else
                {
                    item.UpdateQuantity(item.Quantity - quantity);
                    return 0;
                }
            }
        }
        return quantity;
    }

    private void DragAccessory(InventoryItem item, ItemLocations location)
    {
        if (location == ItemLocations.Accessories)
        {
            FastEquipAccessory(item);
        }
        else
        {
            int quantity = AddItemToFirstFreeCells(item.Data, item.Quantity, ItemLocations.Storage);
            if (quantity > 0)
            {
                AddItemToFirstFreeCells(item.Data, item.Quantity, ItemLocations.Hotbar);
            }
        }
    }

    private void DragArmor(InventoryItem item, ItemLocations location)
    {
        if (location != ItemLocations.Armor)
        {
            FastEquipArmor(item);
        }
        else
        {
            int quantity = AddItemToFirstFreeCells(item.Data, item.Quantity, ItemLocations.Storage);
            if (quantity > 0)
            {
                AddItemToFirstFreeCells(item.Data, item.Quantity, ItemLocations.Hotbar);
            }
        }
    }

    private void FastEquipAccessory(InventoryItem item)
    {
        InventoryItem sameAccessory = FindSameAccessory(item.Data);
        if (sameAccessory != null)
        {
            sameAccessory.SwapData(item);
            return;
        }
        AddItemToFirstFreeCells(item.Data, item.Quantity, ItemLocations.Accessories);
    }

    private void FastEquipArmor(InventoryItem item)
    {
        if (item.Data is ArmorItemSO armor)
        {
            _armorItems[(int)(armor.ArmorType)].SwapData(item);
        }
    }

    private void DeselectHotbarItem(int index)
    {
        if (index == -1)
        {
            return;
        }
        else
        {
            _selectedItem = null;
            _indexOfSelectedItem = -1;
        }
    }

    private InventoryItem GetItem(int index, ItemLocations location)
    {
        return _itemsByLocation[location][index];
    }

    private int GetStartIndex(ItemLocations location)
    {
        return location == ItemLocations.Hotbar ? _hotbarStartIndex : 0;
    }

    private int GetEndIndex(ItemLocations location)
    {
        return location == ItemLocations.Hotbar ? _hotbarEndIndex : _itemsByLocation[location].Length - 1;
    }

    private int GetIndexOfItem(InventoryItem item, ItemLocations location)
    {
        return location == ItemLocations.Buffer ? -1 : Array.IndexOf(_itemsByLocation[location], item);
    }

    private InventoryItem FindSameAccessory(ItemSO data)
    {
        return _accessoriesItems.FirstOrDefault(item => item.Data == data);
    }

    private bool IsStorageFull(ItemLocations location)
    {
        if (location == ItemLocations.Hotbar)
        {
            return !_itemsByLocation[location].Where((item, index) => item.IsEmpty && (index >= _hotbarStartIndex && index <= _hotbarEndIndex)).Any();
        }
        else
        {
            return !_itemsByLocation[location].Where(item => item.IsEmpty).Any();
        }
    }

    private bool IsCorrectArmorSlot(int index, ItemSO data)
    {
        return (int)(data as ArmorItemSO).ArmorType == index;
    }

    private void OnItemDataChanged(InventoryItem item, ItemLocations location)
    {
        int index = GetIndexOfItem(item, location);
        ItemDataChanged?.Invoke(item.Data, index, location);
    }

    private void OnItemQuantityChanged(InventoryItem item, ItemLocations location)
    {
        int index = GetIndexOfItem(item, location);
        ItemQuantityChanged?.Invoke(item.Quantity, index, location);
    }
    #endregion
}
