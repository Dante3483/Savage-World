//using Items;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using UnityEngine;

//namespace Inventory
//{
//    [CreateAssetMenu(fileName = "newInventory", menuName = "UI/Inventory/Inventory")]
//    public class InventorySO : ScriptableObject
//    {
//        #region Private fields
//        [SerializeField] private List<InventoryItem> _inventoryItems;
//        [SerializeField] private HotbarSO _hotbar;
//        [SerializeField] private InventoryItem _itemInChangeState;
//        [SerializeField] private int _size = 10;
//        [Header("Armor")]
//        [SerializeField] private InventoryItem _helmet;
//        [SerializeField] private InventoryItem _chestplate;
//        [SerializeField] private InventoryItem _leggings;
//        [Space]
//        [SerializeField] private InventoryItem _helmetClothes;
//        [SerializeField] private InventoryItem _chestplateClothes;
//        [SerializeField] private InventoryItem _leggingsClothes;
//        #endregion

//        #region Public fields
//        public event Action<Dictionary<int, InventoryItem>, List<InventoryItem>> OnInventoryChanged;
//        #endregion

//        #region Properties
//        public int Size
//        {
//            get
//            {
//                return _size;
//            }

//            set
//            {
//                _size = value;
//            }
//        }

//        public InventoryItem ItemInChangeState
//        {
//            get
//            {
//                return _itemInChangeState;
//            }

//            set
//            {
//                _itemInChangeState = value;
//            }
//        }
//        #endregion

//        #region Methods

//        #region General
//        public void Initialize()
//        {
//            _inventoryItems = new List<InventoryItem>();
//            for (int i = 0; i < Size; i++)
//            {
//                _inventoryItems.Add(InventoryItem.GetEmptyItem());
//            }
//            ItemInChangeState = InventoryItem.GetEmptyItem();
//            _helmet = InventoryItem.GetEmptyItem();
//            _helmetClothes = InventoryItem.GetEmptyItem();
//            _chestplate = InventoryItem.GetEmptyItem();
//            _chestplateClothes = InventoryItem.GetEmptyItem();
//            _leggings = InventoryItem.GetEmptyItem();
//            _leggingsClothes = InventoryItem.GetEmptyItem();
//        }

//        public void InformAboutChange()
//        {
//            OnInventoryChanged?.Invoke(GetCurrentInventoryState(), GetArmorInventoryState());
//            _hotbar.InformAboutChange(GetHotbarInventoryState());
//        }
//        #endregion

//        #region Get inventory state
//        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
//        {
//            Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
//            for (int i = 0; i < _inventoryItems.Count; i++)
//            {
//                returnValue[i] = _inventoryItems[i];
//            }
//            return returnValue;
//        }

//        public List<InventoryItem> GetArmorInventoryState()
//        {
//            return new List<InventoryItem>()
//            {
//                _helmet,
//                _chestplate,
//                _leggings,
//                _helmetClothes,
//                _chestplateClothes,
//                _leggingsClothes
//            };
//        }

//        public Dictionary<int, InventoryItem> GetHotbarInventoryState()
//        {
//            Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
//            for (int i = 0; i < 10; i++)
//            {
//                returnValue[i] = _inventoryItems[i];
//            }
//            return returnValue;
//        }
//        #endregion

//        #region Add item
//        public int AddItem(ItemSO item, int quantity)
//        {
//            if (!item.IsStackable)
//            {
//                for (int i = 0; i < _inventoryItems.Count; i++)
//                {
//                    while (quantity > 0 && !IsInventoryFull())
//                    {
//                        quantity -= AddItemToFirstFreeSlot(item, 1);
//                    }
//                    InformAboutChange();
//                    return quantity;
//                }
//            }
//            quantity = AddStackableItem(item, quantity);
//            InformAboutChange();
//            return quantity;
//        }

//        public int AddItem(InventoryItem item)
//        {
//            return AddItem(item.Item, item.Quantity);
//        }

//        public void AddItemAt(InventoryItem item, int index)
//        {
//            //Item in changing cell
//            InventoryItem newItem = new InventoryItem()
//            {
//                Item = item.Item,
//                Quantity = item.Quantity,
//            };

//            //Item in cell to change
//            InventoryItem itemAtIndex = GetItemAt(index);

//            //Current quantity
//            int quantity = item.Quantity;

//            //If item in cell to change is empty cell
//            if (itemAtIndex.IsEmpty)
//            {
//                _inventoryItems[index] = newItem;
//                ItemInChangeState = InventoryItem.GetEmptyItem();
//            }
//            //Otherwise
//            else
//            {
//                //If item same as changing item
//                if (itemAtIndex.Item == item.Item)
//                {
//                    if (itemAtIndex.Quantity == itemAtIndex.Item.MaxStackSize)
//                    {
//                        _inventoryItems[index] = newItem;
//                        AddItemInBuffer(itemAtIndex);
//                    }
//                    else
//                    {
//                        int amountPossibleToTake = itemAtIndex.Item.MaxStackSize - itemAtIndex.Quantity;
//                        //If quantity greater than maximus stack size
//                        if (quantity > amountPossibleToTake)
//                        {
//                            _inventoryItems[index] = itemAtIndex.SetQuantity(itemAtIndex.Item.MaxStackSize);
//                            quantity -= amountPossibleToTake;
//                            InventoryItem remainderItem = new InventoryItem()
//                            {
//                                Item = item.Item,
//                                Quantity = quantity,
//                            };
//                            AddItemInBuffer(remainderItem);
//                        }
//                        //Otherwise
//                        else
//                        {
//                            _inventoryItems[index] = itemAtIndex.SetQuantity(itemAtIndex.Quantity + quantity);
//                            ItemInChangeState = InventoryItem.GetEmptyItem();
//                        }
//                    }
//                }
//                //Otherwise
//                else
//                {
//                    AddItemInBuffer(itemAtIndex);
//                    _inventoryItems[index] = newItem;
//                }
//            }
//            InformAboutChange();
//        }

//        private int AddItemToFirstFreeSlot(ItemSO item, int quantity)
//        {
//            InventoryItem newItem = new InventoryItem()
//            {
//                Item = item,
//                Quantity = quantity,
//            };

//            for (int i = 0; i < _inventoryItems.Count; i++)
//            {
//                if (_inventoryItems[i].IsEmpty)
//                {
//                    _inventoryItems[i] = newItem;
//                    return quantity;
//                }
//            }
//            return 0;
//        }

//        private int AddStackableItem(ItemSO item, int quantity)
//        {
//            for (int i = 0; i < _inventoryItems.Count; i++)
//            {
//                if (_inventoryItems[i].IsEmpty)
//                {
//                    continue;
//                }
//                if (_inventoryItems[i].Item == item)
//                {
//                    int amountPossibleToTake = _inventoryItems[i].Item.MaxStackSize - _inventoryItems[i].Quantity;
//                    if (quantity > amountPossibleToTake)
//                    {
//                        _inventoryItems[i] = _inventoryItems[i].SetQuantity(_inventoryItems[i].Item.MaxStackSize);
//                        quantity -= amountPossibleToTake;
//                    }
//                    else
//                    {
//                        _inventoryItems[i] = _inventoryItems[i].SetQuantity(_inventoryItems[i].Quantity + quantity);
//                        InformAboutChange();
//                        return 0;
//                    }
//                }
//            }
//            while (quantity > 0 && !IsInventoryFull())
//            {
//                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
//                quantity -= newQuantity;
//                AddItemToFirstFreeSlot(item, newQuantity);
//            }
//            return quantity;
//        }

//        public void AddItemInBuffer(InventoryItem item)
//        {
//            InventoryItem newItem = new InventoryItem()
//            {
//                Item = item.Item,
//                Quantity = item.Quantity,
//            };
//            _itemInChangeState = newItem;
//        }

//        public void AddItemInBuffer(InventoryItem item, int quantity)
//        {
//            if (ItemInChangeState.IsEmpty)
//            {
//                InventoryItem newItem = new InventoryItem()
//                {
//                    Item = item.Item,
//                    Quantity = 1,
//                };
//                ItemInChangeState = newItem;
//            }
//            else
//            {
//                ItemInChangeState = ItemInChangeState.SetQuantity(ItemInChangeState.Quantity + quantity);
//            }
//        }

//        public void QuickSetArmor(int index)
//        {
//            InventoryItem item = GetItemAt(index);
//            ArmorItemSO armor = item.Item as ArmorItemSO;
//            RemoveItemAt(index);
//            switch (armor.ArmorType)
//            {
//                case ArmorTypes.Helmet:
//                    {
//                        if (!_helmet.IsEmpty)
//                        {
//                            AddItemAt(_helmet, index);
//                        }
//                        _helmet = item;
//                    }
//                    break;
//                case ArmorTypes.Chestplate:
//                    {
//                        if (!_chestplate.IsEmpty)
//                        {
//                            AddItemAt(_chestplate, index);
//                        }
//                        _chestplate = item;
//                    }
//                    break;
//                case ArmorTypes.Leggings:
//                    {
//                        if (!_leggings.IsEmpty)
//                        {
//                            AddItemAt(_leggings, index);
//                        }
//                        _leggings = item;
//                    }
//                    break;
//                default:
//                    break;
//            }
//            InformAboutChange();
//        }

//        public void SetArmor(ArmorTypes type)
//        {
//            if (ItemInChangeState.IsEmpty)
//            {
//                RemoveArmor(type);
//                return;
//            }
//            InventoryItem item = ItemInChangeState;
//            ArmorItemSO armor = item.Item as ArmorItemSO;
//            if (armor != null)
//            {
//                switch (type)
//                {
//                    case ArmorTypes.Helmet:
//                        {
//                            if (armor.ArmorType == type)
//                            {
//                                ItemInChangeState = InventoryItem.GetEmptyItem();
//                                if (!_helmet.IsEmpty)
//                                {
//                                    AddItemInBuffer(_helmet);
//                                }
//                                _helmet = item;
//                            }
//                        }
//                        break;
//                    case ArmorTypes.Chestplate:
//                        {
//                            if (armor.ArmorType == type)
//                            {
//                                ItemInChangeState = InventoryItem.GetEmptyItem();
//                                if (!_chestplate.IsEmpty)
//                                {
//                                    AddItemInBuffer(_chestplate);
//                                }
//                                _chestplate = item;
//                            }
//                        }
//                        break;
//                    case ArmorTypes.Leggings:
//                        {
//                            if (armor.ArmorType == type)
//                            {
//                                ItemInChangeState = InventoryItem.GetEmptyItem();
//                                if (!_leggings.IsEmpty)
//                                {
//                                    AddItemInBuffer(_leggings);
//                                }
//                                _leggings = item;
//                            }
//                        }
//                        break;
//                    case ArmorTypes.HelmetClothes:
//                        {
//                            if (armor.ArmorType == ArmorTypes.Helmet)
//                            {
//                                ItemInChangeState = InventoryItem.GetEmptyItem();
//                                if (!_helmetClothes.IsEmpty)
//                                {
//                                    AddItemInBuffer(_helmetClothes);
//                                }
//                                _helmetClothes = item;
//                            }
//                        }
//                        break;
//                    case ArmorTypes.ChestplateClothes:
//                        {
//                            if (armor.ArmorType == ArmorTypes.Chestplate)
//                            {
//                                ItemInChangeState = InventoryItem.GetEmptyItem();
//                                if (!_chestplateClothes.IsEmpty)
//                                {
//                                    AddItemInBuffer(_chestplateClothes);
//                                }
//                                _chestplateClothes = item;
//                            }
//                        }
//                        break;
//                    case ArmorTypes.LeggingsClothes:
//                        {
//                            if (armor.ArmorType == ArmorTypes.Leggings)
//                            {
//                                ItemInChangeState = InventoryItem.GetEmptyItem();
//                                if (!_leggingsClothes.IsEmpty)
//                                {
//                                    AddItemInBuffer(_leggingsClothes);
//                                }
//                                _leggingsClothes = item;
//                            }
//                        }
//                        break;
//                    default:
//                        break;
//                }
//                InformAboutChange();
//            }
//        }

//        public void SetArmorAt(InventoryItem armor, int i)
//        {
//            switch (i)
//            {
//                case 0:
//                    {
//                        _helmet = armor;
//                    }
//                    break;
//                case 1:
//                    {
//                        _chestplate = armor;
//                    }
//                    break;
//                case 2:
//                    {
//                        _leggings = armor;
//                    }
//                    break;
//                case 3:
//                    {
//                        _helmetClothes = armor;
//                    }
//                    break;
//                case 4:
//                    {
//                        _chestplateClothes = armor;
//                    }
//                    break;
//                case 5:
//                    {
//                        _leggingsClothes = armor;
//                    }
//                    break;
//                default:
//                    break;
//            }
//        }
//        #endregion

//        #region Get item
//        public InventoryItem GetItemAt(int itemIndex)
//        {
//            return _inventoryItems[itemIndex];
//        }
//        #endregion

//        #region Remove item
//        public int RemoveItemAt(int index, int quantity = 0)
//        {
//            if (quantity == 0)
//            {
//                _inventoryItems[index] = InventoryItem.GetEmptyItem();
//                InformAboutChange();
//                return 0;
//            }
//            else
//            {
//                int amountPossibliToRemove = _inventoryItems[index].Quantity - quantity;
//                if (amountPossibliToRemove > 0)
//                {
//                    _inventoryItems[index] = _inventoryItems[index].SetQuantity(_inventoryItems[index].Quantity - quantity);
//                    if (_inventoryItems[index].Quantity == 0)
//                    {
//                        _inventoryItems[index] = InventoryItem.GetEmptyItem();
//                    }
//                    InformAboutChange();
//                    return quantity;
//                }
//                else
//                {
//                    _inventoryItems[index] = InventoryItem.GetEmptyItem();
//                    InformAboutChange();
//                    return amountPossibliToRemove + quantity;
//                }
//            }
//        }

//        public void RemoveSelectedItem(int quantity)
//        {
//            int index = _hotbar.SelectedIndex;
//            _inventoryItems[index] = _inventoryItems[index].SetQuantity(_inventoryItems[index].Quantity - quantity);
//            if (_inventoryItems[index].Quantity == 0)
//            {
//                _inventoryItems[index] = InventoryItem.GetEmptyItem();
//            }
//            InformAboutChange();
//        }

//        public void QuickRemoveArmor(ArmorTypes type)
//        {
//            if (!IsInventoryFull())
//            {
//                switch (type)
//                {
//                    case ArmorTypes.Helmet:
//                        {
//                            AddItemToFirstFreeSlot(_helmet.Item, 1);
//                            _helmet = InventoryItem.GetEmptyItem();
//                        }
//                        break;
//                    case ArmorTypes.Chestplate:
//                        {
//                            AddItemToFirstFreeSlot(_chestplate.Item, 1);
//                            _chestplate = InventoryItem.GetEmptyItem();
//                        }
//                        break;
//                    case ArmorTypes.Leggings:
//                        {
//                            AddItemToFirstFreeSlot(_leggings.Item, 1);
//                            _leggings = InventoryItem.GetEmptyItem();
//                        }
//                        break;
//                    case ArmorTypes.HelmetClothes:
//                        {
//                            AddItemToFirstFreeSlot(_helmetClothes.Item, 1);
//                            _helmetClothes = InventoryItem.GetEmptyItem();
//                        }
//                        break;
//                    case ArmorTypes.ChestplateClothes:
//                        {
//                            AddItemToFirstFreeSlot(_chestplateClothes.Item, 1);
//                            _chestplateClothes = InventoryItem.GetEmptyItem();
//                        }
//                        break;
//                    case ArmorTypes.LeggingsClothes:
//                        {
//                            AddItemToFirstFreeSlot(_leggingsClothes.Item, 1);
//                            _leggingsClothes = InventoryItem.GetEmptyItem();
//                        }
//                        break;
//                    default:
//                        break;
//                }
//                InformAboutChange();
//            }
//        }

//        public void RemoveArmor(ArmorTypes type)
//        {
//            switch (type)
//            {
//                case ArmorTypes.Helmet:
//                    {
//                        AddItemInBuffer(_helmet);
//                        _helmet = InventoryItem.GetEmptyItem();
//                    }
//                    break;
//                case ArmorTypes.Chestplate:
//                    {
//                        AddItemInBuffer(_chestplate);
//                        _chestplate = InventoryItem.GetEmptyItem();
//                    }
//                    break;
//                case ArmorTypes.Leggings:
//                    {
//                        AddItemInBuffer(_leggings);
//                        _leggings = InventoryItem.GetEmptyItem();
//                    }
//                    break;
//                case ArmorTypes.HelmetClothes:
//                    {
//                        AddItemInBuffer(_helmetClothes);
//                        _helmetClothes = InventoryItem.GetEmptyItem();
//                    }
//                    break;
//                case ArmorTypes.ChestplateClothes:
//                    {
//                        AddItemInBuffer(_chestplateClothes);
//                        _chestplateClothes = InventoryItem.GetEmptyItem();
//                    }
//                    break;
//                case ArmorTypes.LeggingsClothes:
//                    {
//                        AddItemInBuffer(_leggingsClothes);
//                        _leggingsClothes = InventoryItem.GetEmptyItem();
//                    }
//                    break;
//                default:
//                    break;
//            }
//            InformAboutChange();
//        }
//        #endregion

//        #region Checks
//        private bool IsInventoryFull()
//        {
//            return !_inventoryItems.Where(item => item.IsEmpty).Any();
//        }

//        public bool CanAddItemToInventory(ItemSO item)
//        {
//            bool isSameItemInInventory = _inventoryItems.Where(x => x.Item == item && x.Quantity != item.MaxStackSize).Any();
//            if (isSameItemInInventory)
//            {
//                return true;
//            }
//            if (!IsInventoryFull())
//            {
//                return true;
//            }
//            return false;
//        }
//        #endregion

//        #endregion
//    }
//}