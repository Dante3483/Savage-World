using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    #region Private Fields
    [Header("Main")]
    [SerializeField] private UIInventoryItem _itemPrefab;
    [SerializeField] private UIInventoryItem _itemHotbarPrefab;
    [SerializeField] private RectTransform _contentPanel;
    [SerializeField] private UIAreaToDrop _dropPanel;
    [SerializeField] private List<UIInventoryItem> _listOfUIItems = new List<UIInventoryItem>();
    [SerializeField] private bool _isItemChangeCell = false;
    [Header("Tooltip")]
    [SerializeField] private Tooltip _tooltip;
    [SerializeField] private Vector3 _tooltipDesctiprionOffset;
    [Header("MouseFollower")]
    [SerializeField] private MouseFollower _mouseFollower;
    [SerializeField] private Vector3 _mouseFollowerOffset;
    #endregion

    #region Public Fields
    public event Action<int> OnDescpriptionRequested,
        OnItemStartChangingCell, OnItemEndChangingCell,
        OnItemChangeOne;

    public event Action OnItemStopChangeOne, OnItemDrop;

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
        if (inventorySize > 9)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                UIInventoryItem uiItem;

                if (i < 10)
                {
                    uiItem = Instantiate(_itemHotbarPrefab, Vector3.zero, Quaternion.identity);
                    if (i < 9)
                    {
                        uiItem.KeyTxt.text = (i + 1) + "";
                    }
                    else
                    {
                        uiItem.KeyTxt.text = 0 + "";
                    }
                }
                else
                {
                    uiItem = Instantiate(_itemPrefab, Vector3.zero, Quaternion.identity);
                }

                uiItem.transform.SetParent(_contentPanel, false);
                _listOfUIItems.Add(uiItem);
                uiItem.OnItemChangeCell += HandleItemChangeCell;
                uiItem.OnMouseHover += HandleShowItemTooltip;
                uiItem.OnMouseLeave += HandleHideItemTooltip;
                uiItem.OnRightMouseDown += HandleTakeItemChangeCell;
                uiItem.OnRightMouseUp += HandleStopTakeItemChangeCell;
            }
        }
        

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
        ResetDraggedItem();
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        if (_listOfUIItems.Count > itemIndex)
        {
            _listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
        }
    }
    #endregion

    #region Action handler
    private void HandleTakeItemChangeCell(UIInventoryItem inventoryItemUI)
    {
        int index = _listOfUIItems.IndexOf(inventoryItemUI);
        if (index == -1)
        {
            return;
        }
        OnItemChangeOne?.Invoke(index);
        ResetTooltipDescription();
        _isItemChangeCell = true;
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
        if (!_isItemChangeCell)
        {
            _isItemChangeCell = true;
            ResetTooltipDescription();
            OnItemStartChangingCell?.Invoke(index);
        }
        else
        {
            OnItemEndChangingCell?.Invoke(index);
            _isItemChangeCell = _mouseFollower.gameObject.activeSelf;
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
        if (!_isItemChangeCell)
        {
            _tooltip.Offset = _tooltipDesctiprionOffset;
            _tooltip.Show(description);
        }
        else
        {
            ResetTooltipDescription();
        }
    }

    public void ResetTooltipDescription()
    {
        _tooltip.Hide();
    }
    #endregion

    #endregion
}
