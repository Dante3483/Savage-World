using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "newHotbar", menuName = "UI/Inventory/Hotbar")]
    public class HotbarSO : ScriptableObject
    {
        #region Private fields
        [SerializeField] private InventoryItem _selectedItem;
        [SerializeField] private int _selectedIndex;
        #endregion

        #region Public fields
        public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;
        public event Action<int> OnCellSelected;
        #endregion

        #region Properties
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }

            set
            {
                _selectedIndex = value;
            }
        }
        #endregion

        #region Methods
        public void ResetSelectedCell()
        {
            SelectedIndex = 0;
        }

        public void SelectCell(int index)
        {
            SelectedIndex = index;
            OnCellSelected?.Invoke(SelectedIndex);
        }

        public void InformAboutChange(Dictionary<int, InventoryItem> cells)
        {
            OnInventoryChanged?.Invoke(cells);
        }
        #endregion
    }
}
