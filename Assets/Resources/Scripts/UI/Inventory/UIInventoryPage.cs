using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class UIInventoryPage : MonoBehaviour
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private UIInventoryItem _itemPrefab;
        [SerializeField] private UIInventoryItem _itemHotbarPrefab;
        [SerializeField] private RectTransform _contentPanel;
        [SerializeField] private UIInventoryDrop _dropPanel;
        [SerializeField] private List<UIInventoryItem> _listOfUIItems = new List<UIInventoryItem>();
        [SerializeField] private bool _isItemChangeCell = false;
        [Header("Armor")]
        [SerializeField] private List<UIInventoryArmor> _armorList;
        [SerializeField] private List<Image> _playerView;
        [Header("Tooltip")]
        [SerializeField] private UITooltip _tooltip;
        [SerializeField] private Vector3 _tooltipDesctiprionOffset;
        [Header("MouseFollower")]
        [SerializeField] private UIMouseFollower _mouseFollower;
        [SerializeField] private Vector3 _mouseFollowerOffset;
        #endregion

        #region Public fields
        public event Action<int> OnDescpriptionRequested, OnItemStartChangingCell,
            OnItemEndChangingCell, OnItemApplyAction;

        public event Action OnItemStopChangeOne, OnItemDrop;

        public event Action<ArmorTypes> OnNeedEquipArmor, OnNeedRemoveArmor;
        #endregion

        #region Properties
        public bool IsItemChangeCell
        {
            get
            {
                return _isItemChangeCell;
            }

            set
            {
                _isItemChangeCell = value;
            }
        }

        #endregion

        #region Methods

        #region General methods
        private void Awake()
        {
            Hide();
            _mouseFollower.Toggle(false);
        }

        public void InitializeInventoryUI(int inventorySize)
        {
            //Define min size for inventorty content
            if (inventorySize > 9)
            {
                for (int i = 0; i < inventorySize; i++)
                {
                    UIInventoryItem uiItem;

                    //Set hotbar cells
                    if (i < 10)
                    {
                        uiItem = Instantiate(_itemHotbarPrefab, Vector3.zero, Quaternion.identity);
                        uiItem.KeyTxt.text = (i + 1) % 10 + "";
                    }
                    //Set common cells
                    else
                    {
                        uiItem = Instantiate(_itemPrefab, Vector3.zero, Quaternion.identity);
                    }

                    //Set parent
                    uiItem.transform.SetParent(_contentPanel, false);

                    //Set action's handler
                    //uiItem.OnItemChangeCell += HandleItemChangeCell;
                    //uiItem.OnMouseHover += HandleShowItemTooltip;
                    //uiItem.OnMouseLeave += HandleHideItemTooltip;
                    //uiItem.OnRightMouseDown += HandleRightClickMouse;
                    //uiItem.OnRightMouseUp += HandleStopTakeItemChangeCell;

                    //Add to list of cells
                    _listOfUIItems.Add(uiItem);
                }
            }

            //Configure armor content
            foreach (var item in _armorList)
            {
                //Set action's handler
                //item.OnLeftMouseButtonClicked += HandleEquipArmor;
                //item.OnRightMouseButtonClicked += HandleRemoveArmor;
            }

            //Set drop action's handler
            _dropPanel.OnRightMouseClick += HandleDropItem;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            ResetTooltipDescription();
            ResetDraggedItem();
            gameObject.SetActive(false);
        }

        public void UpdateItemData(int itemIndex, Sprite itemImage, int itemQuantity, ItemTypes type)
        {
            if (_listOfUIItems.Count > itemIndex)
            {
                _listOfUIItems[itemIndex].SetData(itemImage, itemQuantity, type);
            }
        }

        public void UpdateArmorData(int armorIndex, InventoryItem item)
        {
            if (item.IsEmpty)
            {
                _armorList[armorIndex].ResetData();
            }
            else
            {
                _armorList[armorIndex].SetData(item.ItemData.ItemImage);
            }
        }

        public void UpdatePlayerView(Sprite sprite, int index)
        {
            if (sprite == null)
            {
                _playerView[index].gameObject.SetActive(false);
            }
            else
            {
                _playerView[index].sprite = sprite;
                _playerView[index].gameObject.SetActive(true);
            }
        }
        #endregion

        #region Action handler
        private void HandleRightClickMouse(UIInventoryItem inventoryItemUI)
        {
            int index = _listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnItemApplyAction?.Invoke(index);
        }

        private void HandleStopTakeItemChangeCell(UIInventoryItem inventoryItemUI)
        {
            OnItemStopChangeOne?.Invoke();
        }

        private void HandleItemChangeCell(UIInventoryItem inventoryItemUI)
        {
            int index = _listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            IsItemChangeCell = _mouseFollower.gameObject.activeSelf;
            if (!IsItemChangeCell)
            {
                ResetTooltipDescription();
                OnItemStartChangingCell?.Invoke(index);
            }
            else
            {
                OnItemEndChangingCell?.Invoke(index);
                OnDescpriptionRequested?.Invoke(index);
            }
        }

        private void HandleShowItemTooltip(UIInventoryItem inventoryItemUI)
        {
            int index = _listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnDescpriptionRequested?.Invoke(index);
        }

        private void HandleHideItemTooltip(UIInventoryItem inventoryItemUI)
        {
            ResetTooltipDescription();
        }

        private void HandleDropItem()
        {
            OnItemDrop?.Invoke();
        }

        private void HandleEquipArmor(ArmorTypes type)
        {
            OnNeedEquipArmor?.Invoke(type);
        }

        private void HandleRemoveArmor(ArmorTypes type)
        {
            OnNeedRemoveArmor?.Invoke(type);
        }
        #endregion

        #region Drag and drop
        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            _mouseFollower.Offset = _mouseFollowerOffset;
            _mouseFollower.Toggle(true);
            _mouseFollower.SetData(sprite, quantity);
        }

        public void ResetDraggedItem()
        {
            _mouseFollower.Toggle(false);
        }
        #endregion

        #region Tooltip
        public void CreateTooltipDescription(string description)
        {
            IsItemChangeCell = _mouseFollower.gameObject.activeSelf;
            if (!IsItemChangeCell)
            {
                //_tooltip.Offset = _tooltipDesctiprionOffset;
                //_tooltip.Show(description);
            }
            else
            {
                ResetTooltipDescription();
            }
        }

        public void ResetTooltipDescription()
        {
            //_tooltip.Hide();
        }
        #endregion

        #endregion
    }
}
