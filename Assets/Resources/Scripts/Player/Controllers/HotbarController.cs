using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotbar
{
    public class HotbarController : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private UIHotbar _hotbarUI;
        [SerializeField] private HotbarSO _hotbarData;

        private readonly KeyCode[] _hotbarKeys = new KeyCode[]
        {
        KeyCode.Alpha0, KeyCode.Alpha1,
        KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7,
        KeyCode.Alpha8, KeyCode.Alpha9,
        };
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public UIHotbar HotbarUI
        {
            get
            {
                return _hotbarUI;
            }

            set
            {
                _hotbarUI = value;
            }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            PrepareUI();
            PrepareInventoryData();
        }

        private void Update()
        {
            CheckHotKeyClick();
            CheckMouseScroll();
        }

        private void PrepareUI()
        {
            HotbarUI.InitializeHotbarUI();
            HotbarUI.OnNeedSelectCell += HandleSelectCell;
        }

        private void PrepareInventoryData()
        {
            _hotbarData.OnInventoryChanged += HandleNeedUpdateUI;
            _hotbarData.OnCellSelected += HandleUpdateSelection;
            _hotbarData.ResetSelectedCell();
        }

        private void CheckHotKeyClick()
        {
            for (int i = 0; i < _hotbarKeys.Length; i++)
            {
                if (Input.GetKeyDown(_hotbarKeys[i]))
                {
                    int index;
                    if (i == 0)
                    {
                        index = 9;
                    }
                    else
                    {
                        index = i - 1;
                    }
                    _hotbarData.SelectCell(index);
                    return;
                }
            }
        }

        private void CheckMouseScroll()
        {
            int scrollDeltaY = Mathf.Clamp((int)Input.mouseScrollDelta.y, -1, 1);
            int newIndex = _hotbarData.SelectedIndex + scrollDeltaY;
            if (newIndex > 9)
            {
                newIndex = 0;
            }
            if (newIndex < 0)
            {
                newIndex = 9;
            }
            _hotbarData.SelectCell(newIndex);
        }

        private void HandleNeedUpdateUI(Dictionary<int, InventoryItem> hotbarState)
        {
            foreach (var item in hotbarState)
            {
                if (item.Value.IsEmpty)
                {
                    HotbarUI.UpdateData(item.Key, null, 0);
                }
                else
                {
                    HotbarUI.UpdateData(item.Key, item.Value.Item.ItemImage, item.Value.Quantity);
                }
            }
        }

        private void HandleSelectCell(int index)
        {
            _hotbarData.SelectCell(index);
        }

        private void HandleUpdateSelection(int index)
        {
            HotbarUI.UpdateSelection(index);
        }
        #endregion
    }
}