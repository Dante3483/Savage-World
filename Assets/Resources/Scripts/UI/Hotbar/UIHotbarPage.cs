using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIHotbarPage : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UIHotbarItemCell _hotbarItemPrefab;
    [SerializeField] private RectTransform _hotbarContent;

    private List<UIHotbarItemCell> _listOfHotbarItems;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void InitializePage(int hotbarSize)
    {
        _listOfHotbarItems = new List<UIHotbarItemCell>();
        for (int i = 0; i < hotbarSize; i++)
        {
            UIHotbarItemCell uiItem = Instantiate(_hotbarItemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_hotbarContent, false);
            uiItem.name = "HotbarItemCell";
            uiItem.HotbarNumberTxt.text = $"{i + 1}";
            uiItem.Deselect();
            SetHandlers(uiItem);
            _listOfHotbarItems.Add(uiItem);
        }
    }

    private void SetHandlers(UIHotbarItemCell uiItem)
    {
        
    }

    public void UpdateHotbarItemData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        _listOfHotbarItems[itemIndex].SetData(itemImage, itemQuantity);
    }

    public void SelectCell(int index)
    {
        DeselectAllCells();
        _listOfHotbarItems[index].Select();
    }

    private void DeselectAllCells()
    {
        foreach (UIHotbarItemCell uiItem in _listOfHotbarItems)
        {
            uiItem.Deselect();
        }
    }
    #endregion
}
