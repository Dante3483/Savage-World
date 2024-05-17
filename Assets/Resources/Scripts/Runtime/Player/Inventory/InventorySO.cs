using Inventory;
using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Player/Inventory/Inventory")]
public class InventorySO : ScriptableObject
{
    #region Private fields
    [SerializeField] private InventoryItem[] _storageItems;
    [SerializeField] private InventoryItem[] _hotbarItems;
    [SerializeField] private InventoryItem[] _accessoriesItems;
    [SerializeField] private InventoryItem[] _armorItems;

    [SerializeField] private InventoryItem _bufferItem;
    [SerializeField] private InventoryItem _selectedItem;

    [SerializeField] private int _storageSize = 36;
    [SerializeField] private int _hotbarFullSize = 10;
    [SerializeField] private int _accessoriesSize = 6;
    [SerializeField] private int _armorSize = 3;

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
    public event Action<InventoryItem[]> OnStorageChanged;
    public event Action<InventoryItem[], int, int> OnHotbarChanged;
    public event Action<InventoryItem[]> OnAccessoriesChanged;
    public event Action<InventoryItem[]> OnArmorChanged;
    public event Action<InventoryItem> OnBufferItemChanged;
    public event Action<ItemSO, int> OnInventoryFull;
    public event Action OnItemsUpdate;
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
    public void Initialize()
    {
        if (!_isInitialized)
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
            _isInitialized = true;
        }
    }

    public void UpdateUI()
    {
        OnStorageChanged?.Invoke(_storageItems);
        OnHotbarChanged?.Invoke(_hotbarItems, _hotbarStartIndex, _hotbarSize);
        OnAccessoriesChanged?.Invoke(_accessoriesItems);
        OnArmorChanged?.Invoke(_armorItems);
        OnBufferItemChanged?.Invoke(_bufferItem);
        OnItemsUpdate?.Invoke();
    }

    public void AddItem(ItemSO itemData, int quantity)
    {
        if (!itemData.IsStackable)
        {
            quantity -= AddItemToFirstFreeSlot(itemData, 1, ItemLocations.Hotbar);
            if (quantity != 0)
            {
                quantity -= AddItemToFirstFreeSlot(itemData, 1, ItemLocations.Storage);
            }
        }
        else
        {
            quantity = AddStackableItem(itemData, quantity, ItemLocations.Hotbar);
            if (quantity != 0)
            {
                quantity = AddStackableItem(itemData, quantity, ItemLocations.Storage);
            }
        }
        if (quantity != 0)
        {
            OnInventoryFull?.Invoke(itemData, quantity);
        }
        UpdateUI();
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
        UpdateUI();
        return quantity;
    }

    public int AddItem(InventoryItem item, ItemLocations location)
    {
        ItemSO itemData = item.ItemData;
        int quantity = item.Quantity;
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
        item.UpdateQuantity(quantity);
        UpdateUI();
        return quantity;
    }

    public void AddItemAtWithoutNotification(ItemSO itemData, int quantity, int index, ItemLocations location)
    {
        if (_itemsByLocation[location][index].IsEmpty)
        {
            InventoryItem inventoryItem = new();
            inventoryItem.UpdateData(quantity, itemData);
            _itemsByLocation[location][index] = inventoryItem;
        }
    }

    private int AddItemToFirstFreeSlot(ItemSO itemData, int quantity, ItemLocations location)
    {
        GetIndexesByLocation(location, out int startIndex, out int endIndex);

        for (int i = startIndex; i <= endIndex; i++)
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
        GetIndexesByLocation(location, out int startIndex, out int endIndex);

        for (int i = startIndex; i <= endIndex; i++)
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
        GetIndexesByLocation(location, out int startIndex, out int endIndex);

        for (int i = startIndex; i <= endIndex; i++)
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
        UpdateUI();
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

    public InventoryItem GetItem(int index, ItemLocations location)
    {
        return _itemsByLocation[location][index];
    }

    public StringBuilder GetItemDescription(int index, ItemLocations location)
    {
        if (_isItemInBuffer)
        {
            return null;
        }
        InventoryItem item = _itemsByLocation[location][index];
        return item.ItemData?.GetFullDescription(item.Quantity);
    }

    public int GetItemQuantity(ItemSO itemData)
    {
        return _hotbarItems.Union(_storageItems).Where(i => i.ItemData == itemData).Sum(i => i.Quantity);
    }

    public void RemoveItemAt(int index, ItemLocations location)
    {
        GetItem(index, location).ClearData();
        UpdateUI();
    }

    public void RemoveItemFromFirstSlot(ItemSO itemData, int quantity)
    {
        quantity = RemoveItemFromFirstSlot(itemData, quantity, ItemLocations.Hotbar);
        if (quantity != 0)
        {
            RemoveItemFromFirstSlot(itemData, quantity, ItemLocations.Storage);
        }
        UpdateUI();
    }

    private int RemoveItemFromFirstSlot(ItemSO itemData, int quantity, ItemLocations location)
    {
        for (int i = 0; i < _itemsByLocation[location].Length; i++)
        {
            if (quantity == 0)
            {
                return 0;
            }
            if (_itemsByLocation[location][i].ItemData == itemData)
            {
                int amountPossibleToRemove = _itemsByLocation[location][i].Quantity;
                if (quantity > amountPossibleToRemove)
                {
                    _itemsByLocation[location][i].ClearData();
                    quantity -= amountPossibleToRemove;
                }
                else
                {
                    _itemsByLocation[location][i].UpdateQuantity(_itemsByLocation[location][i].Quantity - quantity);
                    return 0;
                }
            }
        }
        return quantity;
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
        UpdateUI();
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
        UpdateUI();
    }

    public void ClearBuffer()
    {
        if (_isItemInBuffer)
        {
            AddItem(_bufferItem, ItemLocations.Storage);
        }
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

    public void SelectItem(int index)
    {
        _selectedItem = _hotbarItems[_hotbarStartIndex + index];
        _indexOfSelectedItem = index;
    }

    public InventoryItem GetSelectedItem()
    {
        return _selectedItem;
    }

    public void RemoveSelectedItem()
    {
        _selectedItem.ClearData();
        UpdateUI();
    }

    public void DecreaseSelectedItemQuantity(int value)
    {
        _selectedItem.UpdateQuantity(_selectedItem.Quantity - value);
        UpdateUI();
    }

    public bool CompareItemWithBuffer(int index, ItemLocations location)
    {
        return _bufferItem.ItemData == GetItem(index, location).ItemData;
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

    public bool IsEnoughSpaceForItem(ItemSO itemData, ItemLocations location)
    {
        if (location == ItemLocations.Hotbar)
        {
            return !_itemsByLocation[location]
                .Where((_, index) => index >= _hotbarStartIndex && index <= _hotbarEndIndex)
                .Where((item) => item.IsEmpty || (item.ItemData == itemData && item.Quantity != item.ItemData.MaxStackSize))
                .Any();
        }
        else
        {
            return !_itemsByLocation[location]
                .Where((item) => item.IsEmpty || (item.ItemData == itemData && item.Quantity != item.ItemData.MaxStackSize))
                .Any();
        }
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

    private void GetIndexesByLocation(ItemLocations location, out int startIndex, out int endIndex)
    {
        if (location == ItemLocations.Hotbar)
        {
            startIndex = _hotbarStartIndex;
            endIndex = _hotbarEndIndex;
        }
        else
        {
            startIndex = 0;
            endIndex = _itemsByLocation[location].Length - 1;
        }
    }
    #endregion
}
