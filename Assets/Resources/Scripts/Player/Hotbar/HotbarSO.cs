using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newInventory", menuName = "UI/Hotbar")]
public class HotbarSO : ScriptableObject
{
    [SerializeField] private InventoryItem _selectedItem;
    [SerializeField] private int _selectedIndex;

    public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;
    public event Action<int> OnCellSelected;

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
}
