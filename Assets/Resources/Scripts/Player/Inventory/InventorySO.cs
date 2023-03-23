using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newInventory", menuName = "UI/Inventory/Inventory")]
public class InventorySO : ScriptableObject
{
    [SerializeField] private List<InventoryItem> _inventoryItems;
    [SerializeField] private HotbarSO _hotbar;
    [SerializeField] private InventoryItem _itemInChangeState;
    [SerializeField] private int _size = 10;

    public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;

    public int Size
    {
        get
        {
            return _size;
        }

        set
        {
            _size = value;
        }
    }

    public InventoryItem ItemInChangeState
    {
        get
        {
            return _itemInChangeState;
        }

        set
        {
            _itemInChangeState = value;
        }
    }

    public void Initialize()
    {
        _inventoryItems = new List<InventoryItem>();
        for (int i = 0; i < Size; i++)
        {
            _inventoryItems.Add(InventoryItem.GetEmptyItem());
        }
        ItemInChangeState = InventoryItem.GetEmptyItem();
    }

    #region Add item
    public int AddItem(ItemSO item, int quantity)
    {
        if (!item.IsStackable)
        {
            for (int i = 0; i < _inventoryItems.Count; i++)
            {
                while (quantity > 0 && !IsInventoryFull())
                {
                    quantity -= AddItemToFirstFreeSlot(item, 1);
                }
                InformAboutChange();
                return quantity;
            }
        }
        quantity = AddStackableItem(item, quantity);
        InformAboutChange();
        return quantity;
    }

    private int AddItemToFirstFreeSlot(ItemSO item, int quantity)
    {
        InventoryItem newItem = new InventoryItem()
        {
            Item = item,
            Quantity = quantity,
        };

        for (int i = 0; i < _inventoryItems.Count; i++)
        {
            if (_inventoryItems[i].IsEmpty)
            {
                _inventoryItems[i] = newItem;
                return quantity;
            }
        }
        return 0;
    }

    private int AddStackableItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < _inventoryItems.Count; i++)
        {
            if (_inventoryItems[i].IsEmpty)
            {
                continue;
            }
            if (_inventoryItems[i].Item == item)
            {
                int amountPossibleToTake = _inventoryItems[i].Item.MaxStackSize - _inventoryItems[i].Quantity;
                if (quantity > amountPossibleToTake)
                {
                    _inventoryItems[i] = _inventoryItems[i].ChangeQuantity(_inventoryItems[i].Item.MaxStackSize);
                    quantity -= amountPossibleToTake;
                }
                else
                {
                    _inventoryItems[i] = _inventoryItems[i].ChangeQuantity(_inventoryItems[i].Quantity + quantity);
                    InformAboutChange();
                    return 0;
                }
            }
        }
        while (quantity > 0 && !IsInventoryFull())
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            quantity -= newQuantity;
            AddItemToFirstFreeSlot(item, newQuantity);
        }
        return quantity;
    }

    public int AddItem(InventoryItem item)
    {
        return AddItem(item.Item, item.Quantity);
    }

    public void AddItemInBuffer(InventoryItem item)
    {
        InventoryItem newItem = new InventoryItem()
        {
            Item = item.Item,
            Quantity = item.Quantity,
        };
        _itemInChangeState = newItem;
    }

    public void AddItemInBuffer(InventoryItem item, int quantity)
    {
        if (ItemInChangeState.IsEmpty)
        {
            InventoryItem newItem = new InventoryItem()
            {
                Item = item.Item,
                Quantity = 1,
            };
            ItemInChangeState = newItem;
        }
        else
        {
            ItemInChangeState = ItemInChangeState.ChangeQuantity(ItemInChangeState.Quantity + quantity);
        }
    }

    public void AddItemAt(InventoryItem item, int index)
    {
        //Item in changing cell
        InventoryItem newItem = new InventoryItem()
        {
            Item = item.Item,
            Quantity = item.Quantity,
        };

        //Item in cell to change
        InventoryItem itemAtIndex = GetItemAt(index);

        //Current quantity
        int quantity = item.Quantity;

        //If item in cell to change is empty cell
        if (itemAtIndex.IsEmpty)
        {
            _inventoryItems[index] = newItem;
            ItemInChangeState = InventoryItem.GetEmptyItem();
        }
        //Otherwise
        else
        {
            //If item same as changing item
            if (itemAtIndex.Item == item.Item)
            {
                if (itemAtIndex.Quantity == itemAtIndex.Item.MaxStackSize)
                {
                    _inventoryItems[index] = newItem;
                    AddItemInBuffer(itemAtIndex);
                }
                else
                {
                    int amountPossibleToTake = itemAtIndex.Item.MaxStackSize - itemAtIndex.Quantity;
                    //If quantity greater than maximus stack size
                    if (quantity > amountPossibleToTake)
                    {
                        _inventoryItems[index] = itemAtIndex.ChangeQuantity(itemAtIndex.Item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                        InventoryItem remainderItem = new InventoryItem()
                        {
                            Item = item.Item,
                            Quantity = quantity,
                        };
                        AddItemInBuffer(remainderItem);
                    }
                    //Otherwise
                    else
                    {
                        _inventoryItems[index] = itemAtIndex.ChangeQuantity(itemAtIndex.Quantity + quantity);
                        ItemInChangeState = InventoryItem.GetEmptyItem();
                    }
                }
            }
            //Otherwise
            else
            {
                AddItemInBuffer(itemAtIndex);
                _inventoryItems[index] = newItem;
            }
        }
        InformAboutChange();
    }
    #endregion

    private bool IsInventoryFull()
    {
        return !_inventoryItems.Where(item => item.IsEmpty).Any();
    }

    public bool IsCanAddItemToInventory(ItemSO item)
    {
        bool isSameItemInInventory = _inventoryItems.Where(x => x.Item == item && x.Quantity != item.MaxStackSize).Any();
        if (isSameItemInInventory)
        {
            return true;
        }
        if (!IsInventoryFull())
        {
            return true;
        }
        return false;
    }

    public Dictionary<int, InventoryItem> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
        for (int i = 0; i < _inventoryItems.Count; i++)
        {
            returnValue[i] = _inventoryItems[i];
        }
        return returnValue;
    }

    public Dictionary<int, InventoryItem> GetHotbarInventoryState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
        for (int i = 0; i < 10; i++)
        {
            returnValue[i] = _inventoryItems[i];
        }
        return returnValue;
    }

    public ItemSO GetSelectedItem()
    {
        return _inventoryItems[_hotbar.SelectedIndex].Item;
    }

    public InventoryItem GetItemAt(int itemIndex)
    {
        return _inventoryItems[itemIndex];
    }

    public int RemoveItemAt(int index, int quantity = 0)
    {
        if (quantity == 0)
        {
            _inventoryItems[index] = InventoryItem.GetEmptyItem();
            InformAboutChange();
            return 0;
        }
        else
        {
            int amountPossibliToRemove = _inventoryItems[index].Quantity - quantity;
            if (amountPossibliToRemove > 0)
            {
                _inventoryItems[index] = _inventoryItems[index].ChangeQuantity(_inventoryItems[index].Quantity - quantity);
                if (_inventoryItems[index].Quantity == 0)
                {
                    _inventoryItems[index] = InventoryItem.GetEmptyItem();
                }
                InformAboutChange();
                return quantity;
            }
            else
            {
                _inventoryItems[index] = InventoryItem.GetEmptyItem();
                InformAboutChange();
                return amountPossibliToRemove + quantity;
            }
        }
    }

    public void RemoveSelectedItem(int quantity)
    {
        int index = _hotbar.SelectedIndex;
        _inventoryItems[index] = _inventoryItems[index].ChangeQuantity(_inventoryItems[index].Quantity - quantity);
        if (_inventoryItems[index].Quantity == 0)
        {
            _inventoryItems[index] = InventoryItem.GetEmptyItem();
        }
        InformAboutChange();
    }

    public InventoryItem CloseInventory(InventoryItem item)
    {
        ItemInChangeState = InventoryItem.GetEmptyItem();
        int quantity = AddItem(item);
        if (quantity != 0)
        {
            InventoryItem remainderItem = new InventoryItem()
            {
                Item = item.Item,
                Quantity = quantity,
            };
            return remainderItem;
        }
        else
        {
            return InventoryItem.GetEmptyItem();
        }
    }

    private void InformAboutChange()
    {
        OnInventoryChanged?.Invoke(GetCurrentInventoryState());
        _hotbar.InformAboutChange(GetHotbarInventoryState());
    }
}

[Serializable]
public struct InventoryItem
{
    public int Quantity;
    public ItemSO Item;
    public bool IsEmpty => Item == null;

    public InventoryItem ChangeQuantity(int newQuantity)
    {
        return new InventoryItem()
        {
            Item = this.Item,
            Quantity = newQuantity,
        };
    }

    public static InventoryItem GetEmptyItem()
    {
        return new InventoryItem()
        {
            Item = null,
            Quantity = 0,
        };
    }
}