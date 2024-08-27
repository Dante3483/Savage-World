using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.Enums.Types;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Player.Inventory.Items
{
    [Serializable]
    public class InventoryItem
    {
        #region Fields
        [SerializeField]
        private ItemSO _data;
        [SerializeField]
        private int _quantity;
        [SerializeField]
        private readonly ItemLocations _location;
        #endregion

        #region Properties
        public ItemSO Data
        {
            get
            {
                return _data;
            }

            private set
            {
                if (_data != value)
                {
                    _data = value;
                    DataChanged?.Invoke(this, _location);
                }
            }
        }

        public int Quantity
        {
            get
            {
                return _quantity;
            }

            private set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    QuantityChanged?.Invoke(this, _location);
                }
            }
        }

        public int StackSize => _data.MaxStackSize;

        public bool IsEmpty => _data == null;

        public bool IsAccessory => _data?.ItemType == ItemTypes.Accessory;

        public bool IsArmor => _data?.ItemType == ItemTypes.Armor;
        #endregion

        #region Events / Delegates
        public event Action<InventoryItem, ItemLocations> DataChanged;
        public event Action<InventoryItem, ItemLocations> QuantityChanged;
        #endregion

        #region Public Methods
        public InventoryItem(ItemLocations location)
        {
            _quantity = 0;
            _data = null;
            _location = location;
        }

        public void UpdateData(ItemSO data)
        {
            if (data == null)
            {
                Clear();
            }
            else
            {
                Data = data;
            }
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                Clear();
            }
            else
            {
                Quantity = quantity;
            }
        }

        public void Update(ItemSO data, int quantity)
        {
            UpdateData(data);
            UpdateQuantity(quantity);
        }

        public void Update(InventoryItem item)
        {
            UpdateData(item.Data);
            UpdateQuantity(item.Quantity);
        }

        public void Clear()
        {
            Data = null;
            Quantity = 0;
        }

        public void SwapData(InventoryItem item)
        {
            (Data, item.Data) = (item.Data, Data);
            (Quantity, item.Quantity) = (item.Quantity, Quantity);
        }

        public int GetRemainingQuantity()
        {
            return StackSize - _quantity;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}