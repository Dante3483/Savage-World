using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHotbar : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private UIHotbarCell _hotbarCellPrefab;
    [SerializeField] private List<UIHotbarCell> _listOfHotbarCells = new List<UIHotbarCell>();

    public event Action<int> OnNeedSelectCell;
    public void InitializeHotbarUI()
    {
        for (int i = 0; i < 10; i++)
        {
            UIHotbarCell uiHotbarCell = Instantiate(_hotbarCellPrefab, Vector3.zero, Quaternion.identity);

            if (i < 9)
            {
                uiHotbarCell.KeyTxt.text = (i + 1) + "";
            }
            else
            {
                uiHotbarCell.KeyTxt.text = 0 + "";
            }

            uiHotbarCell.transform.SetParent(transform, false);
            _listOfHotbarCells.Add(uiHotbarCell);
            uiHotbarCell.OnLeftMouseClick += HandleSelectCell;
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
}
