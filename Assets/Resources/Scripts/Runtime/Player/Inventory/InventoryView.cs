using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.MVP;
using SavageWorld.Runtime.Player.Inventory.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SavageWorld.Runtime.Player.Inventory
{
    public class InventoryView : ViewBase
    {
        #region Fields
        [Header("Prefabs")]
        [SerializeField]
        private UIStorageItemCell _storageItemPrefab;
        [SerializeField]
        private UIStorageHotbarItemCell _hotbarItemPrefab;
        [SerializeField]
        private UIAccessoryItemCell _accessoryItemPrefab;

        [Header("Contents")]
        [SerializeField]
        private RectTransform _storageContent;
        [SerializeField]
        private RectTransform _hotbarContent;
        [SerializeField]
        private RectTransform _accessoriesContent;
        [SerializeField]
        private RectTransform _armorContent;

        [Header("Other")]
        [SerializeField]
        private UITooltip _tooltipUI;
        [SerializeField]
        private UIStorageItemCell _bufferCell;

        private int _storageSize;
        private int _hotbarSize;
        private int _accessoriesSize;
        private bool _isDescriptionEnabled;
        private UIStorageItemCell[] _arrayOfStorageCells;
        private UIStorageHotbarItemCell[] _arrayOfHotbarCells;
        private UIAccessoryItemCell[] _arrayOfAccessoryCells;
        private UIArmorItemCell[] _arrayOfArmorCells;
        private Dictionary<ItemLocations, UIItemCell[]> _cellsByLocation;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates
        public event Action<int, ItemLocations> DragItemRequested;
        public event Action<int, ItemLocations> TakeItemRequested;
        public event Action<int, ItemLocations> DescriptionRequested;
        public event Action StopTakeItemRequested;
        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public override void Initialize()
        {
            _cellsByLocation = new();
            InitializeStorage(_storageSize);
            InitializeHotbar(_hotbarSize);
            InitializeAccessories(_accessoriesSize);
            InitializeArmor();
        }

        public override void Show()
        {
            UIManager.Instance.InventoryUI.IsActive = true;
        }

        public override void Hide()
        {
            UIManager.Instance.InventoryUI.IsActive = false;
            HideTooltip();
        }

        public void Configure(int storageSize, int hotbarSize, int accessoriesSize)
        {
            _storageSize = storageSize;
            _hotbarSize = hotbarSize;
            _accessoriesSize = accessoriesSize;
        }

        public void UpdateCellSprite(Sprite sprite, int index, ItemLocations location)
        {
            if (location == ItemLocations.Buffer)
            {
                _bufferCell.SetSprite(sprite);
            }
            else
            {
                UIItemCell cell = _cellsByLocation[location][index];
                cell.SetSprite(sprite);
            }
        }

        public void UpdateCellQuantity(int quantity, int index, ItemLocations location)
        {
            if (location == ItemLocations.Buffer)
            {
                _bufferCell.SetQuantity(quantity);
            }
            else
            {
                UIItemCell cell = _cellsByLocation[location][index];
                cell.SetQuantity(quantity);
            }
        }

        public void UpdateTooltip(StringBuilder text)
        {
            if (text == null)
            {
                HideTooltip();
            }
            else
            {
                _tooltipUI.Show(text);
            }
        }
        #endregion

        #region Private Methods
        private void InitializeStorage(int inventorySize)
        {
            _arrayOfStorageCells = new UIStorageItemCell[inventorySize];
            for (int i = 0; i < inventorySize; i++)
            {
                UIStorageItemCell cell = CreateCell(_storageItemPrefab, _storageContent, "ItemCell");
                _arrayOfStorageCells[i] = cell;
            }
            _cellsByLocation.Add(ItemLocations.Storage, _arrayOfStorageCells);
        }

        private void InitializeHotbar(int hotbarSize)
        {
            _arrayOfHotbarCells = new UIStorageHotbarItemCell[hotbarSize];
            for (int i = 0; i < hotbarSize; i++)
            {
                UIStorageHotbarItemCell cell = CreateCell(_hotbarItemPrefab, _hotbarContent, "HotbarItemCell");
                cell.SetHotbarNumber($"{i + 1}");
                _arrayOfHotbarCells[i] = cell;
            }
            _cellsByLocation.Add(ItemLocations.Hotbar, _arrayOfHotbarCells);
        }

        private void InitializeAccessories(int accessoriesSize)
        {
            _arrayOfAccessoryCells = new UIAccessoryItemCell[accessoriesSize];
            for (int i = 0; i < accessoriesSize; i++)
            {
                UIAccessoryItemCell cell = CreateCell(_accessoryItemPrefab, _accessoriesContent, "AccessoryItemCell");
                _arrayOfAccessoryCells[i] = cell;
            }
            _cellsByLocation.Add(ItemLocations.Accessories, _arrayOfAccessoryCells);
        }

        private void InitializeArmor()
        {
            _arrayOfArmorCells = GetComponentsInChildren<UIArmorItemCell>().OrderBy(a => a.ArmorType).ToArray();
            foreach (UIArmorItemCell cell in _arrayOfArmorCells)
            {
                SetHandlers(cell);
            }
            _cellsByLocation.Add(ItemLocations.Armor, _arrayOfArmorCells);
        }

        private void SetHandlers(UIItemCell cell)
        {
            cell.LeftButtonClicked += OnDragItemRequested;
            cell.RightMouseDowned += OnTakeItemRequested;
            cell.RightMouseUpped += OnStopTakeItemRequested;
            cell.MouseEntered += OnDescriptionRequested;
            cell.MouseLeft += HideTooltip;
        }

        private T CreateCell<T>(T prefab, RectTransform parrent, string name) where T : UIItemCell
        {
            T cell = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            cell.transform.SetParent(parrent, false);
            cell.name = name;
            SetHandlers(cell);
            return cell;
        }

        private void HideTooltip()
        {
            _tooltipUI.Hide();
        }

        private int GetCellIndex(UIItemCell cell)
        {
            return Array.IndexOf(_cellsByLocation[cell.ItemLocation], cell);
        }

        private void OnDragItemRequested(UIItemCell cell)
        {
            int index = GetCellIndex(cell);
            DragItemRequested?.Invoke(index, cell.ItemLocation);
        }

        private void OnTakeItemRequested(UIItemCell cell)
        {
            int index = GetCellIndex(cell);
            TakeItemRequested?.Invoke(index, cell.ItemLocation);
        }

        private void OnStopTakeItemRequested()
        {
            StopTakeItemRequested?.Invoke();
        }

        private void OnDescriptionRequested(UIItemCell cell)
        {
            int index = GetCellIndex(cell);
            DescriptionRequested?.Invoke(index, cell.ItemLocation);
        }
        #endregion
    }
}