using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotbar
{
    public class UIHotbar : MonoBehaviour
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private UIHotbarCell _hotbarCellPrefab;
        [SerializeField] private List<UIHotbarCell> _listOfHotbarCells = new List<UIHotbarCell>();
        #endregion

        #region Public fields
        public event Action<int> OnNeedSelectCell;
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void InitializeHotbarUI()
        {
            for (int i = 0; i < 10; i++)
            {
                UIHotbarCell uiHotbarCell = Instantiate(_hotbarCellPrefab, Vector3.zero, Quaternion.identity);

                uiHotbarCell.KeyTxt.text = (i + 1) % 10 + "";

                uiHotbarCell.transform.SetParent(transform, false);
                uiHotbarCell.OnLeftMouseClick += HandleSelectCell;

                _listOfHotbarCells.Add(uiHotbarCell);
            }

            DeselectAll();
        }

        public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
        {
            if (_listOfHotbarCells.Count > itemIndex)
            {
                _listOfHotbarCells[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        private void HandleSelectCell(UIHotbarCell hotbarCell)
        {
            int index = _listOfHotbarCells.IndexOf(hotbarCell);
            if (index == -1)
            {
                return;
            }
            OnNeedSelectCell?.Invoke(index);
        }

        public void UpdateSelection(int index)
        {
            UIHotbarCell selectedCell = _listOfHotbarCells.Find(cell => cell.IsSelected);
            if (selectedCell != null)
            {
                int selectedCellIndex = _listOfHotbarCells.IndexOf(selectedCell);
                if (selectedCellIndex != index)
                {
                    selectedCell.Deselect();
                    _listOfHotbarCells[index].Select();
                }
            }
        }

        public void DeselectAll()
        {
            foreach (var item in _listOfHotbarCells)
            {
                item.Deselect();
            }
            _listOfHotbarCells[0].Select();
        }
        #endregion
    }
}