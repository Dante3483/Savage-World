using Items;
using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    #region Private fields
    [SerializeField] private int _quantity;
    [SerializeField] private ItemSO _itemData;
    #endregion

    #region Public fields
    public bool IsEmpty => _itemData == null;
    public bool IsAccessory => _itemData?.ItemType == ItemTypes.Accessory;
    public bool IsArmor => _itemData?.ItemType == ItemTypes.Armor;
    #endregion

    #region Properties
    public int Quantity
    {
        get
        {
            return _quantity;
        }
        private set
        {
            _quantity = value;
        }
    }

    public ItemSO ItemData
    {
        get
        {
            return _itemData;
        }
        private set
        {
            _itemData = value;
        }
    }
    #endregion

    #region Methods
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
        {
            ClearData();
        }
        _quantity = newQuantity;
    }

    public void UpdateItem(ItemSO item)
    {
        if (item == null)
        {
            ClearData();
        }
        _itemData = item;
    }

    public void UpdateData(int quantity, ItemSO item)
    {
        _quantity = quantity;
        _itemData = item;
    }

    public void UpdateData(InventoryItem inventoryItem)
    {
        _quantity = inventoryItem.Quantity;
        _itemData = inventoryItem.ItemData;
    }

    public void SwapData(InventoryItem inventoryItem)
    {
        (_itemData, inventoryItem.ItemData) = (inventoryItem.ItemData, _itemData);
        (_quantity, inventoryItem.Quantity) = (inventoryItem.Quantity, _quantity);
    }

    public void ClearData()
    {
        _quantity = 0;
        _itemData = null;
    }

    public InventoryItem Clone()
    {
        return new InventoryItem()
        {
            _quantity = this._quantity,
            _itemData = this._itemData
        };
    }

    public static InventoryItem GetEmptyItem()
    {
        return new InventoryItem()
        {
            _quantity = 0,
            _itemData = null,
        };
    }
    #endregion
}
