using Items;
using System;
using UnityEngine;

namespace Inventory
{
    [Serializable]
    public class InventoryItem
    {
        #region Private fields
        [SerializeField] private int _quantity;
        [SerializeField] private ItemSO _item;
        #endregion

        #region Public fields
        public bool IsEmpty => _item == null;
        #endregion

        #region Properties
        public int Quantity
        {
            get
            {
                return _quantity;
            }
        }

        public ItemSO Item
        {
            get
            {
                return _item;
            }
        }
        #endregion

        #region Methods
        public void UpdateQuantity(int newQuantity)
        {
            _quantity = newQuantity;
        }

        public void UpdateItem(ItemSO item)
        {
            _item = item;
        }

        public void UpdateData(int quantity, ItemSO item)
        {
            _quantity = quantity;
            _item = item;
        }

        public void ClearData()
        {
            _quantity = 0;
            _item = null;
        }

        public static InventoryItem GetEmptyItem()
        {
            return new InventoryItem()
            {
                _quantity = 0,
                _item = null,
            };
        }

        public InventoryItem Clone()
        {
            return new InventoryItem()
            {
                _quantity = this._quantity,
                _item = this._item
            };
        }
        #endregion
    }
}
